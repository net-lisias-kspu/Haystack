using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using KSP.UI.Screens;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HaystackContinued
{
    internal class HSUtils
    {
        /// <summary>
        /// Structure to house vessel types along with icons and sort order for the plugin
        /// </summary>
        public class SortByWeight : IComparer<HSVesselType>
        {
            public int Compare(HSVesselType a, HSVesselType b)
            {
                return a.sort.CompareTo(b.sort);
            }
        }

        /// <summary>
        ///  Comparer class for HSVesselType to sort list by name
        /// </summary>
        public class SortByName : IComparer<HSVesselType>
        {
            public int Compare(HSVesselType a, HSVesselType b)
            {
                return a.name.CompareTo(b.name);
            }
        }

        /// <summary>
        /// Standard debug log with plugin name attached
        /// </summary>
        /// <param name="message">Message to be logged</param>
        public static void Log(string message)
        {
            Debug.Log(string.Format("HaystackContinued: {0}", message));
        }

        public static void Log(string format, params object[] objects)
        {
            Log(string.Format(format, objects));
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void DebugLog(string message)
        {
            Debug.Log(string.Format("HaystackContinued: {0}", message));
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void DebugLog(string format, params object[] objects)
        {
            DebugLog(string.Format(format, objects));
        }

        internal static void RequestCameraFocus(Vessel vessel)
        {
            var spaceTracking = (SpaceTracking) Object.FindObjectOfType(typeof (SpaceTracking));

            var method = spaceTracking.GetType()
                .GetMethod("RequestVessel", BindingFlags.NonPublic | BindingFlags.Instance);

            method.Invoke(spaceTracking, new object[] {vessel});
        }

        internal static void SwitchAndFly(Vessel vessel)
        {
            if (vessel.DiscoveryInfo.Level == DiscoveryLevels.Owned)
            {
                GamePersistence.SaveGame("persistent", HighLogic.SaveFolder, SaveMode.OVERWRITE);
                FlightDriver.StartAndFocusVessel("persistent", FlightGlobals.Vessels.IndexOf(vessel));
            }
            else
                ScreenMessages.PostScreenMessage("Cannot switch to " + (vessel.vesselType <= VesselType.Unknown ? "an object" : "a vessel") + " we do not own.", 5f, ScreenMessageStyle.UPPER_CENTER);
        }

        internal static void TrackingSwitchToVessel(Vessel vessel)
        {
            var spaceTracking = (SpaceTracking) Object.FindObjectOfType(typeof (SpaceTracking));

            var method = spaceTracking.GetType()
                .GetMethod("FlyVessel", BindingFlags.NonPublic | BindingFlags.Instance);

            method.Invoke(spaceTracking, new object[] {vessel});
        }

        /// <summary>
        /// Focuses the map object matching the intanceID passed in
        /// if the scene is map mode.
        /// Searches both vessels and bodies.
        /// Assumes the instance ID is valid.
        /// </summary>
        /// <param name="instanceID"></param>
        internal static void FocusMapObject(int instanceID)
        {
            // focus on the object
            PlanetariumCamera cam;
            if (IsTrackingCenterActive)
            {
                cam = (PlanetariumCamera)Object.FindObjectOfType(typeof(PlanetariumCamera));
            }
            else if (IsMapActive)
            {
                cam = MapView.MapCamera;
            }
            else
            {
                Log("FocusMapObject: invalid scene");
                return;
            }

            foreach (var mapObject in cam.targets)
            {
                if (mapObject.vessel != null && mapObject.vessel.GetInstanceID() == instanceID)
                {
                    DebugLog("vessel map object: " + mapObject.vessel.name);
                    cam.SetTarget(mapObject);
                    break;
                }
                if (mapObject.celestialBody != null && mapObject.celestialBody.GetInstanceID() == instanceID)
                {
                    cam.SetTarget(mapObject);
                    break;
                }
            }
        }

        /// <summary>
        /// Check if the user is currently viewing the map
        /// </summary>
        internal static bool IsMapActive
        {
            get { return (HighLogic.LoadedScene == GameScenes.FLIGHT) && MapView.MapIsEnabled; }
        }

        internal static bool IsInFlight
        {
            get { return (HighLogic.LoadedScene == GameScenes.FLIGHT); }
        }

        internal static bool IsTrackingCenterActive
        {
            get { return HighLogic.LoadedScene == GameScenes.TRACKSTATION; }
        }

        internal static bool IsSpaceCenterActive
        {
            get { return HighLogic.LoadedScene == GameScenes.SPACECENTER; }
        }

        //from mechjeb: figured it'd be better to keep conversion consistant between various plugins
        //Puts numbers into SI format, e.g. 1234 -> "1.234 k", 0.0045678 -> "4.568 m"
        //maxPrecision is the exponent of the smallest place value that will be shown; for example
        //if maxPrecision = -1 and digitsAfterDecimal = 3 then 12.345 will be formatted as "12.3"
        //while 56789 will be formated as "56.789 k"
        public static string ToSI(double d, int maxPrecision = -99, int sigFigs = 4)
        {
            if (d == 0 || double.IsInfinity(d) || double.IsNaN(d)) return d.ToString() + " ";

            int exponent = (int) Math.Floor(Math.Log10(Math.Abs(d)));
                //exponent of d if it were expressed in scientific notation

            string[] units = new string[]
            {"y", "z", "a", "f", "p", "n", "μ", "m", "", "k", "M", "G", "T", "P", "E", "Z", "Y"};
            const int unitIndexOffset = 8; //index of "" in the units array
            int unitIndex = (int) Math.Floor(exponent/3.0) + unitIndexOffset;
            if (unitIndex < 0) unitIndex = 0;
            if (unitIndex >= units.Length) unitIndex = units.Length - 1;
            string unit = units[unitIndex];

            int actualExponent = (unitIndex - unitIndexOffset)*3; //exponent of the unit we will us, e.g. 3 for k.
            d /= Math.Pow(10, actualExponent);

            int digitsAfterDecimal = sigFigs - (int) (Math.Ceiling(Math.Log10(Math.Abs(d))));

            if (digitsAfterDecimal > actualExponent - maxPrecision) digitsAfterDecimal = actualExponent - maxPrecision;
            if (digitsAfterDecimal < 0) digitsAfterDecimal = 0;

            string ret = d.ToString("F" + digitsAfterDecimal) + " " + unit;

            return ret;
        }
    }

    public static class Extensions
    {
        public static Rect ClampToScreen(this Rect rect)
        {
            rect.x = Mathf.Clamp(rect.x, 0, Screen.width - rect.width);
            rect.y = Mathf.Clamp(rect.y, 0, Screen.height - rect.height);

            return rect;
        }

        public static bool IsEmpty<T>(this List<T> list)
        {
            return list.Count == 0;
        }

        public static string SafeName(this Vessel vessel)
        {
            return vessel == null ? "null" : vessel.name;
        }
    }
}