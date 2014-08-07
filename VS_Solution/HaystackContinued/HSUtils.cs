using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HaystackContinued
{
	class HSUtils
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
			Debug.Log(String.Format("Haystack: {0}", message));
		}

	    public static void Log(string format, params object[] objects)
	    {
	        Log(string.Format(format, objects));
	    }

        [System.Diagnostics.Conditional("DEBUG")]
	    public static void DebugLog(string message)
	    {
              Debug.Log(String.Format("Haystack: {0}", message));
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
	            var spaceTracking = (SpaceTracking)Object.FindObjectOfType(typeof (SpaceTracking));
	            cam = spaceTracking.mainCamera;
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

	    internal static bool IsTrackingCenterActive
	    {
	        get { return HighLogic.LoadedScene == GameScenes.TRACKSTATION; }
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
