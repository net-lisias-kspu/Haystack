using System.Collections.Generic;
using System.IO;
using UnityEngine;

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
			Debug.Log(string.Format("Haystack: {0}", message));
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
    }
}
