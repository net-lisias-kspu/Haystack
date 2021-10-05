using System;
using System.Collections.Generic;
using System.Reflection;
using KSP.UI.Screens;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Haystack
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

        internal static void FocusMapObject(CelestialBody body)
        {
            if (!(IsMapActive || IsTrackingCenterActive))
            {
                return;
            }

            Log.dbg("FocusMapObject(body)");

            var cam = getPlanetariumCamera();

            foreach (var mapObject in cam.targets)
            {
                if (mapObject.celestialBody != null && mapObject.celestialBody.GetInstanceID() == body.GetInstanceID())
                {
                    cam.SetTarget(mapObject);
                    return;
                }
            }
        }

        internal static void FocusMapObject(Vessel vessel)
        {
            if (!(IsMapActive || IsTrackingCenterActive))
            {
                return;
            }

            Log.dbg("FocusMapObject(vessel)");

            var cam = getPlanetariumCamera();

            if (IsTrackingCenterActive)
            {
                foreach (var mapObject in cam.targets)
                {
                    if (mapObject.vessel != null && mapObject.vessel.GetInstanceID() == vessel.GetInstanceID())
                    {
                        cam.SetTarget(mapObject);
                        return;
                    }
                }
                return;
            }

            // else
            // in flight map mode the active vessel is the only vessel in the list of targets
            // don't know why but will attempt to maintain that
            cam.targets.RemoveAll(mapObject => mapObject.vessel != null);

            var activeVessel = FlightGlobals.ActiveVessel;
            if (activeVessel != null && activeVessel.GetInstanceID() != vessel.GetInstanceID())
            {
                cam.AddTarget(FlightGlobals.ActiveVessel.mapObject);
            } 

            cam.AddTarget(vessel.mapObject);
            cam.SetTarget(vessel.mapObject);
        }

        private static PlanetariumCamera getPlanetariumCamera()
        {
            if (IsTrackingCenterActive)
            {
                return Object.FindObjectOfType<PlanetariumCamera>();
            }
            else if (IsMapActive)
            {
                return MapView.MapCamera;
            }
            
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Focuses the map object matching the intanceID passed in
        /// if the scene is map mode.
        /// Searches both vessels and bodies.
        /// Assumes the instance ID is valid.
        /// </summary>
        /// <param name="instanceID"></param>
        //internal static void FocusMapObject(int instanceID)
        //{
        //    Log.dbg("FocusMapObject: {0}", instanceID);
        //    // focus on the object
        //    PlanetariumCamera cam;
        //    if (IsTrackingCenterActive)
        //    {
        //        cam = (PlanetariumCamera)Object.FindObjectOfType(typeof(PlanetariumCamera));
        //    }
        //    else if (IsMapActive)
        //    {
        //        cam = MapView.MapCamera;
        //    }
        //    else
        //    {
        //        DebugLog("FocusMapObject: invalid scene");
        //        return;
        //    }

        //    foreach (var mapObject in cam.targets)
        //    {
        //        if (mapObject.vessel != null && mapObject.vessel.GetInstanceID() == instanceID)
        //        {
        //            DebugLog("vessel map object: " + mapObject.vessel.name);
        //            cam.SetTarget(mapObject);
        //            return;
        //        }
        //        if (mapObject.celestialBody != null && mapObject.celestialBody.GetInstanceID() == instanceID)
        //        {
        //            cam.SetTarget(mapObject);
        //            return;
        //        }
        //    }
        //}

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

    public static class Converters
    {

        public static string Distance(double distance)
        {
            //var sizes = new[]
            //{
            //    new {max = 1000000d, suffix = "m", divisor = 1},
            //    new {max = 1000000d*1000, suffix = "km", divisor = 1000},
            //    new {max = double.MaxValue, suffix = "Mm", divisor = 1000*1000}
            //};
            
            var sizes = new[]
            {
                new {max = 1000000d, suffix = "m", divisor = 1},
                new {max = double.MaxValue, suffix = "km", divisor = 1000},
            };

            foreach (var size in sizes)
            {
                if (distance < size.max)
                {
                    var unit = distance/size.divisor;
                    return unit.ToString("N") + size.suffix;
                }
            }

            return "";
        }

        public static string Duration(double seconds, int secondSigd = 0)
        {
            double secondsLeft = seconds;
            int years = 0;
            int days = 0;
            int hours = 0;
            int minutes = 0;

            var formatter = KSPUtil.dateTimeFormatter;

            years = (int) (secondsLeft/formatter.Year);
            secondsLeft -= ((double)years)*formatter.Year; // cast is to keep from int overflow

            days = (int) (secondsLeft/formatter.Day);
            secondsLeft -= days*formatter.Day;

            hours = (int) (secondsLeft/formatter.Hour);
            secondsLeft -= hours*formatter.Hour;

            minutes = (int) (secondsLeft/formatter.Minute);
            secondsLeft -= minutes*formatter.Minute;

            var secondFormat = secondSigd > 0 ? string.Format("00.{0}", new string('#', secondSigd)) : "00";

            var formatted = string.Format("{0}:{1}s", minutes.ToString("00"), secondsLeft.ToString(secondFormat));

            if (hours > 0)
            {
                formatted = string.Format("{0}:", hours) + formatted;
            }

            if (days > 0)
            {
                formatted = string.Format("{0}d ", days) + formatted;
            }

            if (years > 0)
            {
                formatted = string.Format("{0}y ", years) + formatted;
            }

            return formatted;
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

        public static Rect ClampTo(this Rect rect, Rect clampTo)
        {
            rect.x = Mathf.Clamp(rect.x, clampTo.x, clampTo.xMax);
            rect.y = Mathf.Clamp(rect.y, clampTo.y, clampTo.yMax);

            return rect;
        }

        public static Rect ClampToPosIn(this Rect rect, Rect clampTo)
        {
            rect.x = Mathf.Clamp(rect.x, 0, clampTo.width - rect.width);
            rect.y = Mathf.Clamp(rect.y, 0, clampTo.height - rect.height);

            return rect;
        }

        public static Rect ToScreen(this Rect rect)
        {
            var point = new Vector2(rect.x, rect.y);
            point = GUIUtility.GUIToScreenPoint(point);
            return new Rect(point.x, point.y, rect.width, rect.height);
        }

        public static bool IsEmpty<T>(this List<T> list)
        {
            return list.Count == 0;
        }

        public static string SafeName(this Vessel vessel)
        {
            return vessel == null ? "null" : vessel.name;
        }

        public static Color ToColor(this string hex)
        {
            Color color;
            ColorUtility.TryParseHtmlString(hex, out color);
            return color;
        }

        public static double ToDouble(this string value)
        {
            double d = 0d;
            double.TryParse(value, out d);
            return d;
        }
    }
}