using System;
using System.Linq;
using KSP.UI.Screens;
using UnityEngine;
using ToolbarControl_NS;

namespace HaystackReContinued
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
                HSUtils.DebugLog("DisplayButtonOnClick add");
                this.displayButtonClick += value;
            }
            remove
            {
                HSUtils.DebugLog("DisplayButtonOnClick remove");
                this.displayButtonClick -= value;
            }
        }

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
        }
        internal const string MODID = "HaystackReContinued_NS";
        internal const string MODNAME = "Haystack ReContinued";
        public void Start()
        {
            toolbarControl = gameObject.AddComponent<ToolbarControl>();

            toolbarControl.AddToAllToolbars(appLauncherButton_OnTrue, appLauncherButton_OnFalse,
                      scenes,
                      MODID,
                      toolbarButtonId,
                      Resources.appLauncherIconPath,
                      Resources.ToolbarIcon,
                      MODNAME
              );
        }

        public void OnDestroy()
        {
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