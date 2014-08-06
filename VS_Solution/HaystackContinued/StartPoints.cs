using UnityEngine;

namespace HaystackContinued
{
    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class HaystackResourceLoader : MonoBehaviour
    {
        //Awake Event - when the DLL is loaded
        public void Awake()
        {
            Resources.LoadTextures();

            Resources.PopulateVesselTypes(ref Resources.vesselTypesList);
            Resources.vesselTypesList.Sort(new HSUtils.SortByWeight());

            HSSettings.Load();
        }
    }

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class HaystackFlightMode : HaystackContinued {}

    [KSPAddon(KSPAddon.Startup.TrackingStation, false)]
    public class HaystackTrackingCenter : HaystackContinued {}
}