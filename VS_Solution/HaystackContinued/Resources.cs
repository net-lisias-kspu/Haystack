using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace HaystackContinued
{
    /// <summary>
    /// All plugin resources such as textures and styles are here
    /// </summary>
    public static class Resources
    {

        public static System.Random rnd = new System.Random();

        public static String PathPlugin = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static String PathImages = String.Format("{0}/icons", PathPlugin);

        public static string btnGoFilePath = String.Format("{0}/button_go.png", PathImages);
        public static string btnGoHoverFilePath = String.Format("{0}/button_go_hover.png", PathImages);
        public static string btnGoTargFilePath = String.Format("{0}/button_targ.png", PathImages);
        public static string btnGoTargHoverFilePth = String.Format("{0}/button_targ.png", PathImages);
        public static string btnFoldFilePath = String.Format("{0}/button_fold.png", PathImages);
        public static string btnFoldHoverFilePath = String.Format("{0}/button_fold_hover.png", PathImages);
        public static string btnBodiesFilePath = String.Format("{0}/button_bodies.png", PathImages);

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

        private static List<CelestialBody> celestialBodies = new List<CelestialBody>();

        public static List<CelestialBody> CelestialBodies
        {
            get
            {
                if (celestialBodies.Count < 1)
                {
                    celestialBodies = FlightGlobals.fetch.bodies;
                }

                return celestialBodies;
            }
        }


        public static List<HSVesselType> vesselTypesList = new List<HSVesselType>();
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
                    LoadImage(ref icon, String.Format("{0}/button_vessel_{1}.png", PathImages, type.ToLower()));
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
            targ.LoadImage(File.ReadAllBytes(filename));
        }

        public static GUIStyle winStyle;
        public static GUIStyle buttonGoStyle;
        public static GUIStyle buttonTargStyle;
        public static GUIStyle buttonFoldStyle;
        public static GUIStyle buttonVesselTypeStyle;
        public static GUIStyle buttonVesselListName, buttonVesselListNameAct;
        public static GUIStyle textListHeaderStyle, textSituationStyle, buttonVesselListPressed;
        public static GUIStyle buttonSearchClearStyle;
        public static GUIStyle buttonTextOnly;

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

            //search clear button
            buttonSearchClearStyle = new GUIStyle(GUI.skin.button);
            buttonSearchClearStyle.padding = new RectOffset(1, 3, 3, 1);
            buttonSearchClearStyle.margin = new RectOffset(1, 2, 4, 2);
            buttonSearchClearStyle.fixedWidth = 24f;
            buttonSearchClearStyle.fixedHeight = 24f;
            buttonSearchClearStyle.alignment = TextAnchor.MiddleCenter;
            

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
            buttonVesselListPressed.padding.top += 2;
            buttonVesselListPressed.padding.bottom -= 2;


            buttonTextOnly = new GUIStyle(GUI.skin.button);
            buttonTextOnly.padding.top += 2;
            buttonTextOnly.padding.bottom -= 2;


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