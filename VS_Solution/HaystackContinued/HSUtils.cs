using System;
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

	/// <summary>
	/// All plugin resources such as textures and styles are here
	/// </summary>
	public static class HSResources
	{

	    public static String PathPlugin = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
	    public static String PathImages = string.Format("{0}/icons", PathPlugin);


	    public static string btnGoFilePath = string.Format("{0}/button_go.png", PathImages);
	    public static string btnGoHoverFilePath = string.Format("{0}/button_go_hover.png", PathImages);
	    public static string btnGoTargFilePath = string.Format("{0}/button_targ.png", PathImages);
	    public static string btnGoTargHoverFilePth = string.Format("{0}/button_targ.png", PathImages);
	    public static string btnFoldFilePath = string.Format("{0}/button_fold.png", PathImages);
	    public static string btnFoldHoverFilePath = string.Format("{0}/button_fold_hover.png", PathImages);
	    public static string btnBodiesFilePath = string.Format("{0}/button_bodies.png", PathImages);



		public static Texture2D btnGo = new Texture2D(32, 32, TextureFormat.ARGB32, false);
		public static Texture2D btnGoHover = new Texture2D(32, 32, TextureFormat.ARGB32, false);
		public static Texture2D btnTarg = new Texture2D(32, 32, TextureFormat.ARGB32, false);
		public static Texture2D btnTargHover = new Texture2D(32, 32, TextureFormat.ARGB32, false);
		public static Texture2D btnFold = new Texture2D(48, 16, TextureFormat.ARGB32, false);
		public static Texture2D btnFoldHover = new Texture2D(48, 16, TextureFormat.ARGB32, false);
		public static Texture2D btnBodies = new Texture2D(32, 32, TextureFormat.ARGB32, false);

		/// <summary>
		/// Load images into corresponding textures
		/// </summary>
		public static void LoadTextures()
		{
            
			try
			{
				LoadImage(ref btnGo, btnGoFilePath);
				LoadImage(ref btnGoHover, btnGoHoverFilePath);
				LoadImage(ref btnTarg, btnGoTargFilePath);
				LoadImage(ref btnTargHover, btnGoTargHoverFilePth);
				//LoadImage(ref btnTargHover, "button_targ_hover.png"); // TODO: Create hover image, it is missing
				LoadImage(ref btnFold, btnFoldFilePath);
				LoadImage(ref btnFoldHover, btnFoldHoverFilePath);
				LoadImage(ref btnBodies, btnBodiesFilePath); // handled separate from vessels
			}
			catch (Exception e)
			{
				Debug.LogException(e);
				HSUtils.Log("Exception caught, probably failed to load file");
			}
		}

		/// <summary>
		///  Function that populates haystack vessel type list with images
		/// </summary>
		public static void PopulateVesselTypes(ref List<HSVesselType> list)
		{
			foreach (string type in Enum.GetNames(typeof(VesselType)))
			{
				// Kinda dirty and superfluous method...
				byte sort;
				switch (type.ToLower())
				{
					case "ship":
						sort = 0; // ships go first for no obvious reason
						break;
					case "debris":
						sort = 250; // next to last as we don't care much about garbage
						break;
					case "unknown":
						sort = 255; // unknown last
						break;
					default:
						sort = 128; // everything else in between :)
						break;
				}

				Texture2D icon = new Texture2D(32, 32, TextureFormat.ARGB32, false);
				try
				{
					HSResources.LoadImage(ref icon, string.Format("{0}/button_vessel_{1}.png", PathImages, type.ToLower()));
				}
				catch (Exception e)
				{
					Debug.LogException(e);
				}

				list.Add(new HSVesselType(type, sort, icon, true));
			}
		}

		/// <summary>
		/// Load image from file
		/// </summary>
		/// <param name="targ"></param>
		/// <param name="filename">File name in images directory. Path is hardcoded: PluginData/HrmHaystack/images/</param>
		private static void LoadImage(ref Texture2D targ, string filename)
		{
			targ.LoadImage(System.IO.File.ReadAllBytes(filename));
		}

		public static GUIStyle winStyle;
		public static GUIStyle buttonGoStyle;
		public static GUIStyle buttonTargStyle;
		public static GUIStyle buttonFoldStyle;
		public static GUIStyle buttonVesselTypeStyle;
		public static GUIStyle buttonVesselListName, buttonVesselListNameAct;
		public static GUIStyle textListHeaderStyle, textSituationStyle, buttonVesselListPressed;

		/// <summary>
		/// Set up styles
		/// </summary>
		public static void LoadStyles()
		{
			GUI.skin = HighLogic.Skin;

			// Main window
			winStyle = new GUIStyle(GUI.skin.window);
			// winStyle.stretchWidth = true;
			// winStyle.stretchHeight = false;

			// Switch to vessel button
			buttonGoStyle = new GUIStyle(GUI.skin.button);
			buttonGoStyle.fixedWidth = 32.0F;
			buttonGoStyle.fixedHeight = 32.0F;

			// Set target button
			buttonTargStyle = new GUIStyle(GUI.skin.button);
			buttonTargStyle.fixedWidth = 32.0F;
			buttonTargStyle.fixedHeight = 32.0F;

			// Vessel type toggle
			buttonVesselTypeStyle = new GUIStyle(GUI.skin.button);
			buttonVesselTypeStyle.fixedWidth = 32.0F;
			buttonVesselTypeStyle.fixedHeight = 32.0F;

			// Hide window button
			buttonFoldStyle = new GUIStyle(GUI.skin.label);
			buttonFoldStyle.fixedWidth = 48;
			buttonFoldStyle.fixedHeight = 10;
			buttonFoldStyle.active.background = btnFold;
			buttonFoldStyle.hover.background = btnFoldHover;
			buttonFoldStyle.normal.background = btnFold;
			buttonFoldStyle.focused.background = btnFold;

			// Kind of inverted state button
			buttonVesselListPressed = new GUIStyle(GUI.skin.button);
			buttonVesselListPressed.normal = GUI.skin.button.active;
			buttonVesselListPressed.hover = GUI.skin.button.active;
			buttonVesselListPressed.active = GUI.skin.button.normal;

			// Each list item is actually a button
			buttonVesselListName = new GUIStyle(GUI.skin.button);
			buttonVesselListName.wordWrap = true;
			//buttonVesselListName.stretchWidth = true;
			//buttonVesselListName.active.textColor = XKCDColors.GreenApple;

			textListHeaderStyle = new GUIStyle(GUI.skin.label);
			//textListHeaderStyle.normal.textColor = XKCDColors.YellowTan;
			textListHeaderStyle.normal.textColor = XKCDColors.Yellow;
			textListHeaderStyle.fontSize = 14;
			textListHeaderStyle.fontStyle = FontStyle.Bold;
			textListHeaderStyle.margin = new RectOffset(6, 6, 2, 0);
			textListHeaderStyle.padding = new RectOffset(0, 0, 0, 0);
			//textListHeaderStyle.stretchWidth = true;
			textListHeaderStyle.wordWrap = true;

			textSituationStyle = new GUIStyle(GUI.skin.label);
			textSituationStyle.normal.textColor = XKCDColors.LightGrey;
			textSituationStyle.fontSize = 11;
			textSituationStyle.fontStyle = FontStyle.Normal;
			textSituationStyle.margin = new RectOffset(6, 6, 0, 1);
			textSituationStyle.padding = new RectOffset(0, 0, 0, 0);
			//textSituationStyle.stretchWidth = true;
		}
	}
}
