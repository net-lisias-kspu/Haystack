using System;
using System.Linq;
using KSP.UI.Screens;
using UnityEngine;

using ToolbarControl_NS;
using FILE = KSPe.IO.File<Haystack.Startup>;

namespace Haystack
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, true)]
    public class HaystackResourceLoader : MonoBehaviour
    {
        internal static HaystackResourceLoader fetch = null;

        private static string toolbarButtonId = "haystackContinuedButton";

        public Settings Settings { get; private set; }
        internal static HaystackResourceLoader instance;
        //private IButton toolbarButton;
       // private ApplicationLauncherButton appLauncherButton;


        ToolbarControl toolbarControl;

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

#if false
        //blizzy toolbar
        private void setupToolbar()
        {
            Log.dbg("HaystackResourceLoader#setupToolbar: toolbar detected, using it.");

            this.toolbarButton = ToolbarManager.Instance.add("HaystackContinued", toolbarButtonId);
            this.toolbarButton.Visibility = new GameScenesVisibility(GameScenes.FLIGHT, GameScenes.TRACKSTATION, GameScenes.SPACECENTER);
            this.toolbarButton.TexturePath = Resources.ToolbarIcon;
            this.toolbarButton.ToolTip = "Haystack ReContinued";

            this.toolbarButton.OnClick += toolbarButton_OnClick;
        }
#endif
        private void toolbarButton_OnClick(ClickEvent e)
        {
            this.displayButtonClick(new EventArgs());
        }

        internal void appLauncherButton_OnFalse()
        {
            this.displayButtonClick(new EventArgs());
        }

        internal void appLauncherButton_OnTrue()
        {
            Debug.Log("Haystack.appLauncherButton_OnTrue");
            this.displayButtonClick(new EventArgs());
        }

        public delegate void DisplayButtonClickHandler(EventArgs e);

        private event DisplayButtonClickHandler displayButtonClick;

        public event DisplayButtonClickHandler DisplayButtonOnClick
        {
            add
            {
                Log.dbg("DisplayButtonOnClick add");
                this.displayButtonClick += value;
            }
            remove
            {
                Log.dbg("DisplayButtonOnClick remove");
                this.displayButtonClick -= value;
            }
        }
#if false
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
#endif
        ApplicationLauncher.AppScenes scenes = ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.TRACKSTATION |
                         ApplicationLauncher.AppScenes.MAPVIEW | ApplicationLauncher.AppScenes.SPACECENTER;
        /// <summary>
        /// Awake Event - when the DLL is loaded
        /// </summary>
        public void Awake()
        {
            fetch = this;
            API.APIAwake();
            DontDestroyOnLoad(this);

            Resources.LoadTextures();

            Resources.PopulateVesselTypes(ref Resources.vesselTypesList);
            Resources.vesselTypesList.Sort(new HSUtils.SortByWeight());

            this.Settings = new Settings();

#if false

            if (ToolbarManager.ToolbarAvailable)
            {
                this.setupToolbar();
            }
            else // use applauncher icon
            {
                GameEvents.onGUIApplicationLauncherReady.Add(OnAppLauncherReady);
                GameEvents.onGUIApplicationLauncherDestroyed.Add(OnAppLauncherDestroyed);
            }
#endif

#if false
        private void OnAppLauncherReady()
        {
            if (this.appLauncherButton != null)
            {
                Log.dbg("application launcher button already exists");
                return;
            }

                ApplicationLauncher appLauncher = ApplicationLauncher.Instance;

            if (appLauncher == null)
            {
                Log.dbg("application launcher instance is null");
                //maybe run a coroutine to try again.
                return;
            }

            

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
                ApplicationLauncher appLauncher = ApplicationLauncher.Instance;

            if (appLauncher == null)
            {
                Log.dbg("OnApplicationLauncherDestroyed: application launcher instance is null.");
                return;
            }

            if (this.appLauncherButton == null)
            {
                Log.dbg("app launcher button is null.");
                return;
            }


            appLauncher.RemoveModApplication(this.appLauncherButton);
            this.appLauncherButton = null;
        }
#endif


        }
        internal const string MODID = "HaystackReContinued_NS";
        internal const string MODNAME = "Haystack ReContinued";
        public void Start()
        {
            Debug.Log("HaystackContinued.Start");
            toolbarControl = gameObject.AddComponent<ToolbarControl>();
            //Debug.Log("HaystackContinued, useBlizzy: " + HighLogic.CurrentGame.Parameters.CustomParams<HS>().useBlizzy);
            Debug.Log("HaystackContinued, Resources.appLauncherIconPath: " + Resources.appLauncherIconPath);
            Debug.Log("HaystackContinued, Resources.ToolbarIcon: " + Resources.ToolbarIcon);

            //toolbarControl.UseBlizzy(HighLogic.CurrentGame.Parameters.CustomParams<HS>().useBlizzy);
            toolbarControl.AddToAllToolbars(
                        appLauncherButton_OnTrue, appLauncherButton_OnFalse,
                        scenes,
                        MODID,
                        toolbarButtonId,
                        FILE.Asset.Solve(Resources.appLauncherIconPath),
                        FILE.Asset.Solve(Resources.ToolbarIcon),
                        MODNAME
                    );
        }

        public void OnDestroy()
        {
#if false
            if (this.toolbarButton != null)
            {
                this.toolbarButton.Destroy();
            }

            GameEvents.onGUIApplicationLauncherReady.Remove(this.OnAppLauncherReady);
            GameEvents.onGUIApplicationLauncherDestroyed.Remove(this.OnAppLauncherDestroyed);
#endif
            toolbarControl.OnDestroy();
            Destroy(toolbarControl);
            fetch = null;
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
                Log.dbg("adding scenario module");
                HighLogic.CurrentGame.AddProtoScenarioModule(typeof (HaystackScenarioModule),
                    HaystackScenarioModule.Scenes);
            }
            else
            {
                System.Collections.Generic.IEnumerable<GameScenes> missing = HaystackScenarioModule.Scenes.Except(protoScenarioModule.targetScenes);
                foreach (GameScenes i in missing)
                {
                    Log.dbg("missing scenario module scene: {0}", i);
                    protoScenarioModule.targetScenes.Add(i);
                }
            }
        }
    }
}