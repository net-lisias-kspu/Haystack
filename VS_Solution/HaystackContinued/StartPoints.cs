using UnityEngine;

namespace HaystackContinued
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class HaystackResourceLoader : MonoBehaviour
    {
        public Settings Settings { get; private set; }
        private static HaystackResourceLoader instance;

        //Awake Event - when the DLL is loaded
        public void Awake()
        {
            DontDestroyOnLoad(this);

            Resources.LoadTextures();

            Resources.PopulateVesselTypes(ref Resources.vesselTypesList);
            Resources.vesselTypesList.Sort(new HSUtils.SortByWeight());
            
            this.Settings = new Settings();

            this.InvokeRepeating("RepeatingTask", 0, 30F);
        }

        // seems to be a unity idiom
        public static HaystackResourceLoader Instance
        {
            get {
                return instance ??
                       (instance =
                           (HaystackResourceLoader) UnityEngine.Object.FindObjectOfType(typeof (HaystackResourceLoader)));
            }
        }

        public void RepeatingTask()
        {
            this.Settings.Save();
        }
    }

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class HaystackFlightMode : HaystackContinued
    {
        protected override bool IsGuiScene
        {
            get
            {
                return HighLogic.LoadedScene == GameScenes.FLIGHT;
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
        protected override bool IsGuiScene
        {
            get
            {
                return HSUtils.IsTrackingCenterActive;
            }
        }

        protected override string SettingsName
        {
            get { return "tracking_center"; }
        }
    }
}