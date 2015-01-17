using System.Linq;
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
        public bool ToolbarButtonHide { get; private set; }

        // seems to be a unity idiom
        public static HaystackResourceLoader Instance
        {
            get {
                return instance ??
                       (instance =
                           (HaystackResourceLoader) UnityEngine.Object.FindObjectOfType(typeof (HaystackResourceLoader)));
            }
        }

        private void setupToolbar()
        {
            HSUtils.DebugLog("HaystackResourceLoader#setupToolbar: toolbar detected, using it.");

            this.ToolbarButtonHide = true; // window is hidden at first loading of the game

            this.toolbarButton = ToolbarManager.Instance.add("HaystackContinued", toolbarButtonId);
            this.toolbarButton.Visibility = new GameScenesVisibility(GameScenes.FLIGHT, GameScenes.TRACKSTATION);
            this.toolbarButton.TexturePath = Resources.ToolbarIcon;
            this.toolbarButton.ToolTip = "Haystack Continued";
            this.toolbarButton.OnClick += (e) =>
            {
                this.ToolbarButtonHide = !this.ToolbarButtonHide;
            };
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

            this.InvokeRepeating("RepeatingTask", 0, 30F);

            if (ToolbarManager.ToolbarAvailable)
            {
                this.setupToolbar();
            }
        }

    

        public void RepeatingTask()
        {
            this.Settings.Save();
        }

        public void OnDestroy()
        {
            this.toolbarButton.Destroy();
        }
    }

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class HaystackFlightMode : HaystackContinued
    {
        protected override bool IsGuiDisplay
        {
            get
            {
                return HighLogic.LoadedScene == GameScenes.FLIGHT && !UIHide && !HaystackResourceLoader.Instance.ToolbarButtonHide;
            }
        }

        protected override string SettingsName
        {
            get { return "flight"; }
        }
    }

    [KSPAddon(KSPAddon.Startup.TrackingStation, false)]
    public class HaystackTrackingCenter : HaystackContinued
    {
        protected override bool IsGuiDisplay
        {
            get
            {
                return HSUtils.IsTrackingCenterActive && !UIHide && !HaystackResourceLoader.Instance.ToolbarButtonHide;
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
                HighLogic.CurrentGame.scenarios.FirstOrDefault(i => i.moduleName == typeof(HaystackScenarioModule).Name);
            if (protoScenarioModule == null)
            {
                HSUtils.DebugLog("adding scenario module");
                HighLogic.CurrentGame.AddProtoScenarioModule(typeof(HaystackScenarioModule),
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