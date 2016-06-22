using System;
using System.Linq;
using KSP.UI.Screens;
using ToolbarWrapper;
using UnityEngine;

namespace HaystackContinued
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class HaystackResourceLoader : MonoBehaviour
    {
        private static string toolbarButtonId = "haystackContinuedButton";

        public Settings Settings { get; private set; }
        private static HaystackResourceLoader instance;
        private IButton toolbarButton;
        private ApplicationLauncherButton appLauncherButton;

        // seems to be a unity idiom
        public static HaystackResourceLoader Instance
        {
            get
            {
                return instance ??
                       (instance =
                           (HaystackResourceLoader) UnityEngine.Object.FindObjectOfType(typeof (HaystackResourceLoader)));
            }
        }

        //blizzy toolbar
        private void setupToolbar()
        {
            HSUtils.DebugLog("HaystackResourceLoader#setupToolbar: toolbar detected, using it.");

            this.toolbarButton = ToolbarManager.Instance.add("HaystackContinued", toolbarButtonId);
            this.toolbarButton.Visibility = new GameScenesVisibility(GameScenes.FLIGHT, GameScenes.TRACKSTATION, GameScenes.SPACECENTER);
            this.toolbarButton.TexturePath = Resources.ToolbarIcon;
            this.toolbarButton.ToolTip = "Haystack Continued";

            this.toolbarButton.OnClick += toolbarButton_OnClick;
        }

        private void toolbarButton_OnClick(ClickEvent e)
        {
            this.displayButtonClick(new EventArgs());
        }

        private void appLauncherButton_OnFalse()
        {
            this.displayButtonClick(new EventArgs());
        }

        private void appLauncherButton_OnTrue()
        {
            this.displayButtonClick(new EventArgs());
        }

        public delegate void DisplayButtonClickHandler(EventArgs e);

        private event DisplayButtonClickHandler displayButtonClick;

        public event DisplayButtonClickHandler DisplayButtonOnClick
        {
            add
            {
                HSUtils.DebugLog("DisplayButtonOnClick add");
                this.displayButtonClick += value;
            }
            remove
            {
                HSUtils.DebugLog("DisplayButtonOnClick remove");
                this.displayButtonClick -= value;
            }
        }

        //this is needed since the main window can be saved in a visible state but the application launcher button
        //will have no knowledge if it is visisble or not on a scene switch
        public void FixApplicationLauncherButtonDisplay(bool visible)
        {
            if (this.appLauncherButton == null)
            {
                return;
            }

            if (visible)
            {
                this.appLauncherButton.SetTrue(false);
            }
            else
            {
                this.appLauncherButton.SetFalse(false);
            }
        }

        /// <summary>
        /// Awake Event - when the DLL is loaded
        /// </summary>
        public void Awake()
        {
            DontDestroyOnLoad(this);

            Resources.LoadTextures();

            Resources.PopulateVesselTypes(ref Resources.vesselTypesList);
            Resources.vesselTypesList.Sort(new HSUtils.SortByWeight());

            this.Settings = new Settings();

           if (ToolbarManager.ToolbarAvailable)
            {
                this.setupToolbar();
            }
            else // use applauncher icon
            {
                GameEvents.onGUIApplicationLauncherReady.Add(OnAppLauncherReady);
                GameEvents.onGUIApplicationLauncherDestroyed.Add(OnAppLauncherDestroyed);
            }
        }

        private void OnAppLauncherReady()
        {
            if (this.appLauncherButton != null)
            {
                HSUtils.DebugLog("application launcher button already exists");
                return;
            }

            var appLauncher = ApplicationLauncher.Instance;

            if (appLauncher == null)
            {
                HSUtils.DebugLog("application launcher instance is null");
                //maybe run a coroutine to try again.
                return;
            }

            var scenes = ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.TRACKSTATION |
                         ApplicationLauncher.AppScenes.MAPVIEW | ApplicationLauncher.AppScenes.SPACECENTER;

            this.appLauncherButton = appLauncher.AddModApplication(appLauncherButton_OnTrue, appLauncherButton_OnFalse,
                () => { },
                () => { },
                () => { },
                () => { },
                scenes,
                Resources.appLauncherIcon
                );
        }

        private void OnAppLauncherDestroyed()
        {
            var appLauncher = ApplicationLauncher.Instance;

            if (appLauncher == null)
            {
                HSUtils.DebugLog("OnApplicationLauncherDestroyed: application launcher instance is null.");
                return;
            }

            if (this.appLauncherButton == null)
            {
                HSUtils.DebugLog("app launcher button is null.");
                return;
            }


            appLauncher.RemoveModApplication(this.appLauncherButton);
            this.appLauncherButton = null;
        }

        public void OnDestroy()
        {
            if (this.toolbarButton != null)
            {
                this.toolbarButton.Destroy();
            }

            GameEvents.onGUIApplicationLauncherReady.Remove(this.OnAppLauncherReady);
            GameEvents.onGUIApplicationLauncherDestroyed.Remove(this.OnAppLauncherDestroyed);
        }
    }

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class HaystackFlightMode : HaystackContinued
    {
        protected override bool IsGuiDisplay
        {
            get
            {
                return HighLogic.LoadedScene == GameScenes.FLIGHT && !UIHide && this.WinVisible;
            }
        }

        protected override string SettingsName
        {
            get { return "flight"; }
        }
    }


    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class HaystackSpaceCentre : HaystackContinued
    {
        protected override bool IsGuiDisplay
        {
            get
            {
                return HighLogic.LoadedScene == GameScenes.SPACECENTER && !UIHide && this.WinVisible;
            }
        }

        protected override string SettingsName
        {
            get { return "space_center"; }
        }
    }

    [KSPAddon(KSPAddon.Startup.TrackingStation, false)]
    public class HaystackTrackingCenter : HaystackContinued
    {
        protected override bool IsGuiDisplay
        {
            get
            {
                return HSUtils.IsTrackingCenterActive && !UIHide && this.WinVisible;
            }
        }

        protected override string SettingsName
        {
            get { return "tracking_center"; }
        }
    }

    /// <summary>
    /// Need to wait until the user is in the space center to create the scenarios since a saved game is required for them.  
    /// </summary>
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class ScenarioModuleLoader : MonoBehaviour
    {
        public void Start()
        {
            this.setupScenarioModule();
        }

        private void setupScenarioModule()
        {
            ProtoScenarioModule protoScenarioModule =
                HighLogic.CurrentGame.scenarios.FirstOrDefault(i => i.moduleName == typeof (HaystackScenarioModule).Name);
            if (protoScenarioModule == null)
            {
                HSUtils.DebugLog("adding scenario module");
                HighLogic.CurrentGame.AddProtoScenarioModule(typeof (HaystackScenarioModule),
                    HaystackScenarioModule.Scenes);
            }
            else
            {
                var missing = HaystackScenarioModule.Scenes.Except(protoScenarioModule.targetScenes);
                foreach (var i in missing)
                {
                    HSUtils.DebugLog("missing scenario module scene: {0}", i);
                    protoScenarioModule.targetScenes.Add(i);
                }
            }
        }
    }
}