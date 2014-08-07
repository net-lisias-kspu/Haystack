using System.Linq;
using KSP.IO;
using UnityEngine;

namespace HaystackContinued
{
	public class Settings
	{
		public static string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
		public static bool minimized;

	    public Settings()
	    {
	        Load();
	    }

		private void Load()
		{
			HSUtils.DebugLog("loading settings");

            PluginConfiguration cfg = PluginConfiguration.CreateForType<HaystackContinued>();
			cfg.load();

			this.WinRect = cfg.GetValue<Rect>("winPos");
			if (this.WinRect == null)
			{
				HSUtils.DebugLog("rectangle failed");

                this.WinRect = new Rect(Screen.width - 320, Screen.height / 2 - 200, 300, 600);
			}
			HSUtils.DebugLog(string.Format("rectangle success: {0} {1} {2} {3}", this.WinRect.x, this.WinRect.y, this.WinRect.width, this.WinRect.height));

			for (ushort iter = 0; iter < Resources.vesselTypesList.Count(); iter++)
			{
				Resources.vesselTypesList[iter].visible = cfg.GetValue("type_visible_" + Resources.vesselTypesList[iter].name, true);
			}
		}

	    public Rect WinRect { get; set; }

	    public void Save()
		{
			HSUtils.DebugLog("saving settings");

            PluginConfiguration cfg = PluginConfiguration.CreateForType<HaystackContinued>();

			cfg.SetValue("winPos", this.WinRect);

			foreach(HSVesselType type in Resources.vesselTypesList)
			{
				cfg.SetValue("type_visible_" + type.name, type.visible);
			}

			//cfg.SetValue("bodies_visible", HaystackContinued.showCelestialBodies);

			cfg.save();
		}
		
	}
}
