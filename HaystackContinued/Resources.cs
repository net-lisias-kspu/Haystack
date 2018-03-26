using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using ToolbarControl_NS;

namespace HaystackReContinued
{
    /// <summary>
    /// All plugin resources such as textures and styles are here
    /// </summary>
    public static class Resources
    {
        public static System.Random rnd = new System.Random();

        public static string PathPlugin = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
            .Replace("\\", "/");

//        public static string PathImages = String.Format("../{0}/icons", PathPlugin);
        public static string PathImages = String.Format("{0}/PluginData/icons", "GameData/HaystackContinued");

        public static readonly string BODIES = "Bodies";

        //toolbar expects to begin in the "gamedata" directory as the root of the provided path
        //so we're going use a relative path starting at our plugin's folder in the "gamedata" directory
        //        public static string ToolbarIconPath = PathImages.Substring(PathImages.ToLower().IndexOf("/gamedata/") + 10);
        public static string ToolbarIconPath = String.Format("{0}/PluginData/icons", "HaystackContinued");
        //for the toolbar do not append the extension
        public static string ToolbarIcon = String.Format("{0}/toolbar_icon", ToolbarIconPath);

        public static string appLauncherIconPath = String.Format("{0}/applauncher_icon", ToolbarIconPath);

        private static string btnGoFilePath = String.Format("{0}/button_go.png", PathImages);
        private static string btnGoHoverFilePath = String.Format("{0}/button_go_hover.png", PathImages);
        private static string btnGoTargFilePath = String.Format("{0}/button_targ.png", PathImages);
        private static string btnGoTargHoverFilePth = String.Format("{0}/button_targ.png", PathImages);
        private static string btnFoldFilePath = String.Format("{0}/button_fold.png", PathImages);
        private static string btnFoldHoverFilePath = String.Format("{0}/button_fold_hover.png", PathImages);
        private static string btnBodiesFilePath = String.Format("{0}/button_bodies.png", PathImages);
        private static string btnDownArrowFilePath = String.Format("{0}/down_arrow.png", PathImages);
        private static string btnUpArrowFilePath = String.Format("{0}/up_arrow.png", PathImages);
        private static string btnTargetAlphaFilePath = string.Format("{0}/button_targ_alpha.png", PathImages);
        private static string btnAscendingFilePath = string.Format("{0}/button_ascending.png", PathImages);
        private static string btnDescendingFilePath = string.Format("{0}/button_descending.png", PathImages);
        private static string btnExtendedHoverFilePath = string.Format("{0}/button_extended_background_hover.png", PathImages);
        private static string btnExtendedPressedFilePath = string.Format("{0}/button_extended_background_pressed.png", PathImages);
        private static string imgLineFilePath = String.Format("{0}/line.png", PathImages);
        private static string imgOutlineFilePath = String.Format("{0}/outline.png", PathImages);
        private static string imgVesselInfoNormalFilePath = string.Format("{0}/vessel_info_normal.png", PathImages);
        private static string imgVesselInfoPressedFilePath = string.Format("{0}/vessel_info_pressed.png", PathImages);
        private static string imgVesselInfoHoverFilePath = string.Format("{0}/vessel_info_hover.png", PathImages);
        private static string imgVesselInfoSelectedFilePath = string.Format("{0}/vessel_info_selected.png", PathImages);
        private static string imgDockingPortButtonPressedFilePath = String.Format("{0}/docking_port_button_pressed.png", PathImages);
        private static string btnOrbitIconFilePath = string.Format("{0}/orbit_icon.png", PathImages);
        private static string btnHiddenIconFilePath = string.Format("{0}/hidden_icon.png", PathImages);
        private static string btnExtendedIconOpenFilePath = string.Format("{0}/button_extended_icon_open.png", PathImages);
        private static string btnExtendedIconCloseFilePath = string.Format("{0}/button_extended_icon_close.png", PathImages);
        private static string btnFlatNormalFilePath = string.Format("{0}/button_flat_normal.png", PathImages);
        private static string btnFlatHoverFilePath = string.Format("{0}/button_flat_hover.png", PathImages);
        private static string btnFlatPressedFilePath = string.Format("{0}/button_flat_pressed.png", PathImages);

        private static string btnTerminateFlatFilePath = String.Format("{0}/button_terminate.png", PathImages);
        private static string btnTerminateHoverFilePath = String.Format("{0}/button_terminate_hover.png", PathImages);

        //public static Texture2D appLauncherIcon = new Texture2D(38, 38, TextureFormat.ARGB32, false);
        public static Texture2D btnGo = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D btnGoHover = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D btnTarg = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D btnTargHover = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D btnFold = new Texture2D(48, 16, TextureFormat.ARGB32, false);
        public static Texture2D btnFoldHover = new Texture2D(48, 16, TextureFormat.ARGB32, false);
        public static Texture2D btnBodies = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D btnDownArrow = new Texture2D(21, 21, TextureFormat.ARGB32, false);
        public static Texture2D btnUpArrow = new Texture2D(21, 21, TextureFormat.ARGB32, false);
        public static Texture2D btnTargetAlpha = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D imgLine = new Texture2D(10, 4, TextureFormat.ARGB32, false);
        public static Texture2D imgOutline = new Texture2D(18, 18, TextureFormat.ARGB32, false);
        public static Texture2D imgVesselListButtonPressed = new Texture2D(14, 14, TextureFormat.ARGB32, false);
        public static Texture2D btnOrbitIcon = new Texture2D(20, 20, TextureFormat.ARGB32, false);
        public static Texture2D btnHiddenIcon = new Texture2D(24, 24, TextureFormat.ARGB32, false);
        public static Texture2D btnAscendingIcon = new Texture2D(24, 24, TextureFormat.ARGB32, false);
        public static Texture2D btnDescendingIcon = new Texture2D(24, 24, TextureFormat.ARGB32, false);
        public static Texture2D imgVesselInfoNormal = new Texture2D(64, 64, TextureFormat.ARGB32, false);
        public static Texture2D imgVesselInfoPressed = new Texture2D(64, 64, TextureFormat.ARGB32, false);
        public static Texture2D imgVesselInfoHover = new Texture2D(64, 64, TextureFormat.ARGB32, false);
        public static Texture2D imgVesselInfoSelected = new Texture2D(64, 64, TextureFormat.ARGB32, false);
        public static Texture2D btnExtendedHoverBackground = new Texture2D(12, 12, TextureFormat.ARGB32, false);
        public static Texture2D btnExtendedPressedBackground = new Texture2D(12, 12, TextureFormat.ARGB32, false);
        public static Texture2D btnExtendedIconOpen = new Texture2D(16, 32, TextureFormat.ARGB32, false);
        public static Texture2D btnExtendedIconClose = new Texture2D(16, 32, TextureFormat.ARGB32, false);
        public static Texture2D btnFlatNormalBackground = new Texture2D(12, 12, TextureFormat.ARGB32, false);
        public static Texture2D btnFlatPressedBackground = new Texture2D(12, 12, TextureFormat.ARGB32, false);

       // public static Texture2D btnFlatNormalBackground = new Texture2D(12, 12, TextureFormat.ARGB32, false);
        public static Texture2D btnFlatHoverBackground = new Texture2D(12, 12, TextureFormat.ARGB32, false);

        public static Texture2D btnTerminateNormalBackground = new Texture2D(16, 14, TextureFormat.ARGB32, false);
        public static Texture2D btnTerminateHoverBackground = new Texture2D(16, 14, TextureFormat.ARGB32, false);
        //public static Texture2D btnTerminate = new Texture2D(16, 14, TextureFormat.ARGB32, false);

        public static RectOffset imgOutlineBorder = new RectOffset(2, 2, 2, 2);
        public static RectOffset imgVesselListButtonBorder = new RectOffset(2, 2, 2, 2);


        /// <summary>
        /// Load images into corresponding textures
        /// </summary>
        public static void LoadTextures()
        {
            try
            {
               // LoadImage(ref appLauncherIcon, appLauncherIconPath);

                LoadImage(ref btnGo, btnGoFilePath);
                LoadImage(ref btnGoHover, btnGoHoverFilePath);
                LoadImage(ref btnTarg, btnGoTargFilePath);
                LoadImage(ref btnTargHover, btnGoTargHoverFilePth);
                //LoadImage(ref btnTargHover, "button_targ_hover.png"); // TODO: Create hover image, it is missing
                LoadImage(ref btnFold, btnFoldFilePath);
                LoadImage(ref btnFoldHover, btnFoldHoverFilePath);
                LoadImage(ref btnBodies, btnBodiesFilePath); // handled separate from vessels

                LoadImage(ref btnDownArrow, btnDownArrowFilePath);
                LoadImage(ref btnUpArrow, btnUpArrowFilePath);
                LoadImage(ref btnOrbitIcon, btnOrbitIconFilePath);
                LoadImage(ref btnHiddenIcon, btnHiddenIconFilePath);
                LoadImage(ref btnTargetAlpha, btnTargetAlphaFilePath);
                LoadImage(ref btnAscendingIcon, btnAscendingFilePath);
                LoadImage(ref btnDescendingIcon, btnDescendingFilePath);

                LoadImage(ref btnExtendedHoverBackground, btnExtendedHoverFilePath);
                LoadImage(ref btnExtendedPressedBackground, btnExtendedPressedFilePath);
                LoadImage(ref btnExtendedIconClose, btnExtendedIconCloseFilePath);
                LoadImage(ref btnExtendedIconOpen, btnExtendedIconOpenFilePath);

                LoadImage(ref btnFlatNormalBackground, btnFlatNormalFilePath);
                LoadImage(ref btnFlatHoverBackground, btnFlatHoverFilePath);
                LoadImage(ref btnFlatPressedBackground, btnFlatPressedFilePath);

                LoadImage(ref imgLine, imgLineFilePath);
                LoadImage(ref imgOutline, imgOutlineFilePath);
                LoadImage(ref imgVesselInfoHover, imgVesselInfoHoverFilePath);
                LoadImage(ref imgVesselInfoNormal, imgVesselInfoNormalFilePath);
                LoadImage(ref imgVesselInfoPressed, imgVesselInfoPressedFilePath);
                LoadImage(ref imgVesselInfoSelected, imgVesselInfoSelectedFilePath);

                LoadImage(ref imgVesselListButtonPressed, imgDockingPortButtonPressedFilePath);

                LoadImage(ref btnTerminateHoverBackground, btnTerminateFlatFilePath);
                LoadImage(ref btnTerminateNormalBackground, btnTerminateHoverFilePath);


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
            foreach (string type in Enum.GetNames(typeof (VesselType)))
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
                        sort = 254; // unknown last
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

            list.Add(new HSVesselType(BODIES, 255, btnBodies, true));
        }

        /// <summary>
        /// Load image from file
        /// </summary>
        /// <param name="targ"></param>
        /// <param name="filename">File name in images directory. Path is hardcoded: PluginData/HrmHaystack/images/</param>
        private static void LoadImage(ref Texture2D targ, string filename)
        {
#if false
            Debug.Log("LoadImage: " + filename);
            if (!File.Exists(filename))
            {
                Debug.Log("Missing file: " + filename);
                return;
            }
            targ.LoadImage(File.ReadAllBytes(filename));
#endif
            if (!ToolbarControl.LoadImageFromFile(ref targ, filename))
            {
                Debug.Log("Missing file: " + filename);
                return;
            }
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
        public static GUIStyle buttonExpandStyle;
        public static GUIStyle hrSepLineStyle;
        public static GUIStyle textDockingPortStyle;
        public static GUIStyle emptyStyle = new GUIStyle();
        public static GUIStyle textDockingPortHeaderStyle;
        public static GUIStyle textSearchStyle;
        public static GUIStyle boxOutlineStyle;
        public static GUIStyle textDockingPortDistanceStyle;
        public static GUIStyle buttonDockingPortTarget;
        public static GUIStyle resizeBoxStyle;
        public static GUIStyle vesselInfoSelected;
        public static GUIStyle vesselInfoDefault;
        public static GUIStyle textVesselExpandedInfoItem;
        public static GUIStyle buttonExtendedStyle;
        public static GUIStyle tooltipBoxStyle;
        public static GUIStyle buttonFlatStyle;
        public static GUIStyle buttonRenameStyle;
        public static GUIStyle buttonTerminateStyle;
        public static GUIStyle textExpandedVesselNameStyle;

        private static bool stylesLoaded;
        

        /// <summary>
        /// Set up styles
        /// </summary>
        public static void LoadStyles()
        {

            if (stylesLoaded)
            {
                return;
            }

            GUI.skin = HighLogic.Skin;

            // Main window
            winStyle = new GUIStyle(GUI.skin.window);
            winStyle.fontSize = 10;
            winStyle.normal.textColor = XKCDColors.LightGrey;

            // resize button
            resizeBoxStyle = new GUIStyle(GUI.skin.box);
            resizeBoxStyle.fontSize = 10;
            resizeBoxStyle.normal.textColor = XKCDColors.LightGrey;

            //tooltip
            tooltipBoxStyle = new GUIStyle(GUI.skin.box);
            tooltipBoxStyle.fontSize = 12;
            tooltipBoxStyle.normal.textColor = "#f4eac0".ToColor();


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
            buttonVesselTypeStyle.fixedWidth = 28.0F;
            buttonVesselTypeStyle.fixedHeight = 28.0F;
            buttonVesselTypeStyle.margin.left -= 2;
            buttonVesselTypeStyle.margin.right -= 2;
            buttonVesselTypeStyle.margin.top -= 2;
            buttonVesselTypeStyle.margin.bottom -= 2;
            buttonVesselTypeStyle.padding.top -= 2;
            buttonVesselTypeStyle.padding.bottom -= 2;
            buttonVesselTypeStyle.padding.left -= 2;
            buttonVesselTypeStyle.padding.right -= 2;

            // vessel info list item default
            vesselInfoDefault = new GUIStyle(GUI.skin.box);
            vesselInfoDefault.normal.background = imgVesselInfoNormal;
            vesselInfoDefault.hover.background = imgVesselInfoHover;
            vesselInfoDefault.active.background = imgVesselInfoPressed;
            vesselInfoDefault.padding = new RectOffset(2, 2, 3, 0);
            vesselInfoDefault.border = new RectOffset(2, 2, 3, 0);
            vesselInfoDefault.margin = new RectOffset(3, 3, 6, 6);

            // vessel info list item when selected
            vesselInfoSelected = new GUIStyle(GUI.skin.box);
            vesselInfoSelected.active.background = imgVesselInfoSelected;
            vesselInfoSelected.normal.background = imgVesselInfoSelected;
            vesselInfoSelected.hover.background = imgVesselInfoSelected;
            vesselInfoSelected.border = new RectOffset(2, 2, 3, 0);
            vesselInfoSelected.padding = new RectOffset(2, 2, 3, 0);
            vesselInfoSelected.margin = new RectOffset(3, 3, 6, 6);
            
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
            buttonVesselListPressed.padding.top -= 3;
            buttonVesselListPressed.padding.bottom -= 2;

            buttonTextOnly = new GUIStyle(GUI.skin.button);
            buttonTextOnly.padding.top -= 2;
            buttonTextOnly.padding.bottom -= 2;

            buttonExpandStyle = new GUIStyle(GUI.skin.label);
            buttonExpandStyle.imagePosition = ImagePosition.ImageOnly;
            buttonExpandStyle.alignment = TextAnchor.MiddleCenter;
            buttonExpandStyle.active.background = imgVesselListButtonPressed;
            buttonExpandStyle.onActive.background = imgVesselListButtonPressed;
            buttonExpandStyle.border = imgVesselListButtonBorder;
            buttonExpandStyle.fixedHeight = 16;
            buttonExpandStyle.fixedWidth = 16;
            buttonExpandStyle.padding = new RectOffset(2, 2, 2, 2);
            buttonExpandStyle.margin = new RectOffset(0, 0, 0, 0);

            buttonFlatStyle = new GUIStyle(GUI.skin.label);
            buttonFlatStyle.alignment = TextAnchor.MiddleCenter;
            buttonFlatStyle.normal.background = btnFlatNormalBackground;
            buttonFlatStyle.active.background = btnFlatPressedBackground;
            buttonFlatStyle.onActive.background = btnFlatPressedBackground;
            buttonFlatStyle.onHover.background = btnFlatHoverBackground;
            buttonFlatStyle.hover.background = btnFlatHoverBackground;
            buttonFlatStyle.padding = new RectOffset(2, 2, 2, 2);
            buttonFlatStyle.border = new RectOffset(1, 1, 1, 1);
            buttonFlatStyle.margin = new RectOffset(2, 2, 2, 2);

            buttonRenameStyle = new GUIStyle(buttonFlatStyle);
            buttonRenameStyle.normal.textColor = XKCDColors.LightGrey;
            buttonRenameStyle.onActive.textColor = XKCDColors.LightGrey;
            buttonRenameStyle.onHover.textColor = XKCDColors.LightGrey;
            buttonRenameStyle.fontSize = 11;


            buttonTerminateStyle = new GUIStyle(GUI.skin.label);
            buttonTerminateStyle.alignment = TextAnchor.MiddleCenter;
            buttonTerminateStyle.normal.background = btnFlatNormalBackground;
            buttonTerminateStyle.active.background = btnFlatPressedBackground;
            buttonTerminateStyle.onActive.background = btnFlatPressedBackground;
            buttonTerminateStyle.onHover.background = btnFlatHoverBackground;
            buttonTerminateStyle.hover.background = btnFlatHoverBackground;
            buttonTerminateStyle.padding = new RectOffset(2, 2, 2, 2);
            buttonTerminateStyle.border = new RectOffset(1, 1, 1, 1);
            buttonTerminateStyle.margin = new RectOffset(2, 2, 2, 2);

            hrSepLineStyle = new GUIStyle(GUI.skin.box);
            hrSepLineStyle.normal.background = imgLine;
            hrSepLineStyle.border = new RectOffset(1, 1, 2, 1);
            hrSepLineStyle.padding = new RectOffset(0, 0, 0, 0);
            hrSepLineStyle.margin = new RectOffset(10, 10, 0, 0);
            hrSepLineStyle.fixedHeight = 4;
            hrSepLineStyle.stretchHeight = false;
            hrSepLineStyle.stretchWidth = true;

            // Each list item is actually a button
            buttonVesselListName = new GUIStyle(GUI.skin.button);
            buttonVesselListName.wordWrap = true;

            textListHeaderStyle = new GUIStyle(GUI.skin.label);
            textListHeaderStyle.normal.textColor = "#f4eac0".ToColor();
            textListHeaderStyle.fontSize = 14;
            textListHeaderStyle.fontStyle = FontStyle.Bold;
            textListHeaderStyle.margin = new RectOffset(6, 6, 2, 2);
            textListHeaderStyle.padding = new RectOffset(0, 0, 0, 2);
            textListHeaderStyle.stretchWidth = true;
            textListHeaderStyle.wordWrap = false;

            textExpandedVesselNameStyle = new GUIStyle(textListHeaderStyle);
            textExpandedVesselNameStyle.alignment = TextAnchor.MiddleLeft;
            textExpandedVesselNameStyle.margin = new RectOffset(6, 6, 4, 0);


            textSituationStyle = new GUIStyle(GUI.skin.label);
            textSituationStyle.normal.textColor = XKCDColors.LightGrey;
            textSituationStyle.fontSize = 11;
            textSituationStyle.fontStyle = FontStyle.Normal;
            textSituationStyle.margin = new RectOffset(6, 6, 0, 1);
            textSituationStyle.padding = new RectOffset(0, 0, 0, 0);
            textSituationStyle.stretchWidth = false;

            textDockingPortHeaderStyle = new GUIStyle(textSituationStyle);
            textDockingPortHeaderStyle.fontSize = 12;
            textDockingPortHeaderStyle.margin = new RectOffset(6, 6, 4, 2);

            textDockingPortStyle = new GUIStyle(textSituationStyle);
            textDockingPortStyle.alignment = TextAnchor.MiddleLeft;
            textDockingPortStyle.normal.textColor = XKCDColors.LightOlive;
            textDockingPortStyle.fontSize = 12;
            textDockingPortStyle.margin = new RectOffset(10, 10, 4, 2);

            textDockingPortDistanceStyle = new GUIStyle(textDockingPortStyle);
            textDockingPortDistanceStyle.alignment = TextAnchor.MiddleRight;

            textSearchStyle = new GUIStyle(GUI.skin.label);
            textSearchStyle.normal.textColor = XKCDColors.LightGrey;

            boxOutlineStyle = new GUIStyle(GUI.skin.box);
            boxOutlineStyle.normal.background = imgOutline;
            boxOutlineStyle.border = imgOutlineBorder;

            buttonDockingPortTarget = new GUIStyle(GUI.skin.box);
            buttonDockingPortTarget.normal.background = null;
            buttonDockingPortTarget.active.background = imgVesselListButtonPressed;
            buttonDockingPortTarget.border = imgVesselListButtonBorder;
            buttonDockingPortTarget.margin = new RectOffset(0, 0, 0, 2);

            textVesselExpandedInfoItem = new GUIStyle(textSituationStyle);

            buttonExtendedStyle = new GUIStyle(GUI.skin.box);
            buttonExtendedStyle.normal.background = null;
            buttonExtendedStyle.active.background = btnExtendedPressedBackground;
            buttonExtendedStyle.hover.background = btnExtendedHoverBackground;
            buttonExtendedStyle.border = new RectOffset(2, 2, 2, 2);
            buttonExtendedStyle.margin = new RectOffset(2, 2, 2, 2);
            buttonExtendedStyle.padding = new RectOffset(1, 1, 1, 1);

            stylesLoaded = true;
        }
    }
}