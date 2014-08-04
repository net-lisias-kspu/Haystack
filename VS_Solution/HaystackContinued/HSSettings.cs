using System.Linq;
using KSP.IO;
using UnityEngine;

namespace HaystackContinued
{
	public class HSSettings
	{
		public static string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
		public static bool minimized;

		public static void Load()
		{
#if DEBUG
			HSUtils.Log("loading settings");
#endif
			PluginConfiguration cfg = PluginConfiguration.CreateForType<HaystackContinued>();
			cfg.load();

			HaystackContinued.WinRect = cfg.GetValue<Rect>("winPos");
			if (HaystackContinued.WinRect == null)
			{
#if DEBUG
				HSUtils.Log("rectangle failed");
#endif
				HaystackContinued.WinRect = new Rect(Screen.width - 320, Screen.height / 2 - 200, 300, 600);
			}
#if DEBUG
			HSUtils.Log(string.Format("rectangle success: {0} {1} {2} {3}", HaystackContinued.WinRect.x, HaystackContinued.WinRect.y, HaystackContinued.WinRect.width, HaystackContinued.WinRect.height));
#endif

			for (ushort iter = 0; iter < HaystackContinued.vesselTypesList.Count(); iter++)
			{
				HaystackContinued.vesselTypesList[iter].visible = cfg.GetValue("type_visible_" + HaystackContinued.vesselTypesList[iter].name, true);
			}
		}

		public static void Save()
		{
#if DEBUG
			HSUtils.Log("saving settings");
#endif
			PluginConfiguration cfg = PluginConfiguration.CreateForType<HaystackContinued>();
			cfg.SetValue("winPos", HaystackContinued.WinRect);

			foreach(HSVesselType type in HaystackContinued.vesselTypesList)
			{
				cfg.SetValue("type_visible_" + type.name, type.visible);
			}

			cfg.SetValue("bodies_visible", HaystackContinued.showCelestialBodies);

			cfg.save();
		}
		
	}
}
