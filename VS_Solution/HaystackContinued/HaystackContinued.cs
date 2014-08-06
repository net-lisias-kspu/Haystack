using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using LibNoise.Unity.Operator;
using UnityEngine;

namespace HaystackContinued
{
	/// <summary>
	/// Class to house vessel types along with icons and sort order for the plugin
	/// Used to be a structure
	/// </summary>
	public class HSVesselType
	{
		public string name; // Type name defined by KSP devs
		public byte sort; // Sort order, lowest first
		public Texture2D icon; // Icon texture, loaded from PluginData directory. File must be named 'button_vessel_TYPE.png'
		public bool visible; // Is this type shown in list

		public HSVesselType(string name, byte sort, Texture2D icon, bool visible)
		{
			this.name = name;
			this.sort = sort;
			this.icon = icon;
			this.visible = visible;
		}
	};


    public class HaystackContinued : MonoBehaviour
    {
        private const int MainWindowID = 1823748;

        // Game object that keeps us running
        public static GameObject gameObjectInstance;

        public static List<HSVesselType> vesselTypesList = new List<HSVesselType>();

        private static Vessel switchToMe;
        private static List<Vessel> hsVesselList = new List<Vessel>(); 
        private static List<Vessel> filteredVesselList = new List<Vessel>();

        private static List<CelestialBody> celestialBodyList = new List<CelestialBody>();
        private static List<CelestialBody> filteredBodyList = new List<CelestialBody>();
        private static Dictionary<CelestialBody, List<Vessel>> groupedBodyVessel = new Dictionary<CelestialBody, List<Vessel>>();
        public static bool showCelestialBodies = true;

        // count types
        private static Dictionary<string, int> typeCount;

        // Resizeable window vars
        private bool winHidden = true;
        private static Rect _winRect;

        // Search text
        private string filterVar = "";

        public void Awake()
        {
#if DEBUG
            HSUtils.Log("awake Behaviour, DLL loaded");
#endif

            // Populate list of vessel types and load textures - should happen once
            Resources.LoadTextures();

            Resources.PopulateVesselTypes(ref vesselTypesList);
            vesselTypesList.Sort(new HSUtils.SortByWeight());

            celestialBodyList = new List<CelestialBody>();
            typeCount = new Dictionary<string, int>();

            HSSettings.Load();

            DontDestroyOnLoad(this);
            CancelInvoke();

            GameEvents.onPlanetariumTargetChanged.Add(OnMapTargetChange);

            InvokeRepeating("MainHSActivity", 5.0F, 5.0F); // Refresh from time to time just in case
            InvokeRepeating("RefreshDataSaveSettings", 0, 30.0F);
        }

        public void OnDestory()
        {
            GameEvents.onPlanetariumTargetChanged.Remove(this.OnMapTargetChange);
        }

        private void OnMapTargetChange(MapObject mapObject)
        {
            if (mapObject == null)
            {
                return;
            }

            if (mapObject.type == MapObject.MapObjectType.VESSEL)
            {
                this.defaultScrollerView.SelectedVessel = mapObject.vessel;
                HSUtils.Log("OnMapTargetChange: vessel: " + mapObject.vessel.name);
            } else if (mapObject.type == MapObject.MapObjectType.CELESTIALBODY)
            {
                this.defaultScrollerView.SelectedBody = mapObject.celestialBody;
            }
            else
            {
                this.defaultScrollerView.SelectedBody = null;
                this.defaultScrollerView.SelectedVessel = null;
            }
        }

        /// <summary>
        /// Refresh list of vessels
        /// </summary>
        private static void RefetchVesselList()
        {
            hsVesselList = (FlightGlobals.fetch == null ? FlightGlobals.Vessels : FlightGlobals.fetch.vessels);
            // count vessel types
            typeCount.Clear();
            foreach (var vessel in hsVesselList)
            {
                var typeString = vessel.vesselType.ToString();

                if (typeCount.ContainsKey(typeString))
                    typeCount[typeString]++;
                else
                    typeCount.Add(typeString, 1);
            }
        }

        /// <summary>
        /// Every second refresh seems to be enough. Data filtering here and switching to selected vessel too.
        /// </summary>
        public void MainHSActivity()
        {
            // populate the list of bodies if empty
            if (celestialBodyList.Count < 1)
                celestialBodyList = FlightGlobals.fetch.bodies;

            if (IsGuiScene)
            {
                // refresh filter lists
                filteredVesselList = new List<Vessel>(hsVesselList);
                filteredBodyList = new List<CelestialBody>(celestialBodyList);

                if (hsVesselList != null)
                {
                    if (vesselTypesList != null)
                    {
                        // For each hidden type remove it from the list
                        // FIXME: must be optimized
                        foreach (HSVesselType currentInvisibleType in vesselTypesList)
                        {
                            if (currentInvisibleType.visible == false)
                            {
                                //filter out type
                                filteredVesselList = filteredVesselList.FindAll(sr => sr.vesselType.ToString() != currentInvisibleType.name);
                            }
                        }
                    }

                    // And then filter by the search string
                    if (!string.IsNullOrEmpty(filterVar))
                    {
                        //filteredVesselList = hsVesselList.FindAll(delegate(Vessel v) { return -1 != v.vesselName.IndexOf(filterVar, StringComparison.OrdinalIgnoreCase); });
                        filteredVesselList =
                            filteredVesselList.FindAll(
                                delegate(Vessel v)
                                {
                                    return -1 != v.vesselName.IndexOf(filterVar, StringComparison.OrdinalIgnoreCase);
                                });

                        if (showCelestialBodies == true)
                            filteredBodyList =
                                celestialBodyList.FindAll(
                                    delegate(CelestialBody cb)
                                    {
                                        return -1 != cb.bodyName.IndexOf(filterVar, StringComparison.OrdinalIgnoreCase);
                                    });
                    }
                }

                if (groupByOrbitingBody)
                {
                    groupedBodyVessel.Clear();

                    foreach(var vessel in filteredVesselList)
                    {
                        var body = vessel.orbit.referenceBody;

                        List<Vessel> list;
                        if (!groupedBodyVessel.TryGetValue(body, out list))
                        {
                            list = new List<Vessel>();
                            groupedBodyVessel.Add(body, list);
                        }

                        list.Add(vessel);
                    }
                }
            }

            // Detect if there's a request to switch vessel
            if (switchToMe != null)
            {
                FlightGlobals.SetActiveVessel(switchToMe);
                switchToMe = null;
                winHidden = true;
                filterVar = "";
                /*
				if (!switchToMe.loaded)
				{
					switchToMe.Load();
				}

				if (!HighLogic.LoadedSceneIsFlight)
				{
					HighLogic.LoadScene(GameScenes.FLIGHT);
					CameraManager.Instance.SetCameraFlight();
				}
				*/
            }
        }

        /// <summary>
        /// Function called every 30 seconds
        /// </summary>
        public void RefreshDataSaveSettings()
        {
            if (IsGuiScene)
            {
                RefetchVesselList();

                // FIXME: temporary for testing
                HSSettings.Save();
            }
        }

        /// <summary>
        /// Repaint GUI (only in map view condition inside)
        /// </summary>
        public void OnGUI()
        {
            if (IsGuiScene)
            {
                DrawGUI();
            }
        }

        public static Rect WinRect
        {
            get { return _winRect; }
            set { _winRect = value; }
        }

        public void DrawGUI()
        {
            GUI.skin = HighLogic.Skin;

            if (Resources.winStyle == null)
            {
                Resources.LoadStyles();
            }

            if (winHidden)
            {
                _winRect.y = Screen.height - 1;
            }
            else
            {
                _winRect.y = Screen.height - _winRect.height;
                _winRect = _winRect.ClampToScreen();
            }
            
            _winRect = GUILayout.Window(MainWindowID, _winRect, MainWindowConstructor,
                string.Format("Haystack {0}", HSSettings.version), Resources.winStyle, GUILayout.MinWidth(120),
                GUILayout.MinHeight(300));

            if (GUI.Button(new Rect(_winRect.x + (_winRect.width/2 - 24), _winRect.y - 9, 48, 10), "",
                Resources.buttonFoldStyle))
            {
                winHidden = !winHidden; // toggle window state
                RefetchVesselList();
                MainHSActivity();
            }
        }

        /// <summary>
        /// Checks if the GUI should be drawn in the current scene
        /// </summary>
        private static bool IsGuiScene
        {
            get
            {
                return HighLogic.LoadedScene == GameScenes.FLIGHT || HighLogic.LoadedScene == GameScenes.TRACKSTATION;
            }
        }

        // For the scrollview
        private Vector2 scrollPos = Vector2.zero;

        // Keep track of selections in GUILayouts
        private Vessel tmpVesselSelected;
        private CelestialBody tmpBodySelected;
        private string typeSelected;
        private bool groupByOrbitingBody;
        private readonly ResizeHandler resizeHandler = new ResizeHandler();
        private readonly DefaultScrollerView defaultScrollerView = new DefaultScrollerView();

        private void MainWindowConstructor(int windowID)
        {
            GUILayout.BeginVertical();

            #region vessel types - horizontal

            GUILayout.BeginHorizontal();

            // Vessels
            for (int i = 0; i < vesselTypesList.Count(); i++)
            {
                var typeString = vesselTypesList[i].name;

                if (typeCount.ContainsKey(typeString))
                    typeString += String.Format(" ({0})", typeCount[typeString]);

                vesselTypesList[i].visible = GUILayout.Toggle(vesselTypesList[i].visible,
                    new GUIContent(vesselTypesList[i].icon, typeString), Resources.buttonVesselTypeStyle);
            }

            // Bodies
            showCelestialBodies = GUILayout.Toggle(showCelestialBodies, new GUIContent(Resources.btnBodies, "Bodies"),
                Resources.buttonVesselTypeStyle);
            defaultScrollerView.showCelestialBodies = showCelestialBodies;

            GUILayout.EndHorizontal();

            #endregion vessel types

            GUILayout.BeginHorizontal();
            GUILayout.Label("Find:");
            filterVar = GUILayout.TextField(filterVar, GUILayout.MinWidth(50.0F), GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();

            // handle tooltips here so it paints over the find entry
            if (GUI.tooltip != "")
            {
                // get mouse position
                var mousePosition = Event.current.mousePosition;
                var width = GUI.tooltip.Length*11;
                GUI.Box(new Rect(mousePosition.x - 30, mousePosition.y - 30, width, 25), GUI.tooltip);
            }

            if (groupByOrbitingBody)
            {
                drawGroupedScroller();
            }   
            else
            {
                this.defaultScrollerView.Draw();
            }


            #region bottom buttons - horizontal

            GUILayout.BeginHorizontal();

            //group by toggle
            var previous = this.groupByOrbitingBody;
            this.groupByOrbitingBody = GUILayout.Toggle(this.groupByOrbitingBody, new GUIContent("GB", "Group by orbit"),
                Resources.buttonVesselTypeStyle);

            if (previous != this.groupByOrbitingBody)
            {
                this.scrollPos = Vector2.zero;
                this.defaultScrollerView.Reset();
            }

            GUILayout.FlexibleSpace();

            // Disable buttons for current vessel or nothing selected
            if (IsTargetButtonDisabled())
            {
                GUI.enabled = false;
            }

            //TODO: fix these
            // target button
            if (GUILayout.Button(Resources.btnTarg, Resources.buttonTargStyle))
            {
                if (typeSelected == "vessel")
                {
                    FlightGlobals.fetch.SetVesselTarget(tmpVesselSelected);
                }
                else if (typeSelected == "body")
                {
                    FlightGlobals.fetch.SetVesselTarget(tmpBodySelected);
                }
            }

            GUI.enabled = true;

            // Disable fly button if we selected a body, have no selection, or selected the current vessel
            if (IsFlyButtonDisabled())
            {
                GUI.enabled = false;
            }

            // fly button
            if (GUILayout.Button(Resources.btnGoHover, Resources.buttonGoStyle))
            {
                if (typeSelected == "vessel")
                {
                    // Delayed switch to vessel
                    switchToMe = tmpVesselSelected;
                }
            }

            GUI.enabled = true;

            GUILayout.EndHorizontal();

            #endregion bottom buttons

            GUILayout.EndVertical();

            //resize controls
            var resizeRect = new Rect(_winRect.width - 24f - 2f, 2f, 24f, 24f);
            GUI.Box(resizeRect, "//", GUI.skin.box);

            this.resizeHandler.Run(resizeRect);


            // If user input detected, force mapObject refresh
            if (GUI.changed)
            {
                MainHSActivity();
            }

            GUI.DragWindow();
        }

        private void drawGroupedScroller()
        {

            if (filteredVesselList.Count == 0)
            {
                GUILayout.Label("No matched vessels foud");
                tmpVesselSelected = null;
                tmpBodySelected = null;
                GUILayout.FlexibleSpace();
                return;
            }

            scrollPos = GUILayout.BeginScrollView(scrollPos);

            GUILayout.BeginVertical();

            foreach (var kv in groupedBodyVessel)
            {
                var body = kv.Key;
                var vessels = kv.Value;

                var selected = body == tmpBodySelected;

                selected = GUILayout.Toggle(selected, new GUIContent(body.name), GUI.skin.button);

                if (selected)
                {
                    this.tmpBodySelected = body;
                }
                else
                {
                    if (tmpBodySelected == body)
                    {
                        tmpBodySelected = null;
                    }
                    continue;
                }

                foreach (var vessel in vessels)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20f);

                    GUILayout.BeginVertical(vessel == tmpVesselSelected
                        ? Resources.buttonVesselListPressed
                        : GUI.skin.button);
                    GUILayout.Label(vessel.vesselName, Resources.textListHeaderStyle);
                    GUILayout.Label(
                        string.Format("{0}. {1}{2}", vessel.vesselType.ToString(), Vessel.GetSituationString(vessel),
                            (FlightGlobals.ActiveVessel == vessel && vessel != null) ? ". Currently active" : ""),
                        Resources.textSituationStyle);
                    
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }

        private bool IsTargetButtonDisabled()
        {
            bool returnVal = true;

            if (typeSelected == "vessel")
            {
                returnVal = (tmpVesselSelected == null || FlightGlobals.ActiveVessel == tmpVesselSelected);
                //HSUtils.Log(string.Format("IsTargetButtonDisabled: {0} {1} {2} {3}", returnVal, typeSelected, tmpVesselSelected, FlightGlobals.ActiveVessel));
            }
            else if (typeSelected == "body")
            {
                returnVal = (tmpBodySelected == null || FlightGlobals.currentMainBody == tmpBodySelected);
                //HSUtils.Log(string.Format("IsTargetButtonDisabled: {0} {1} {2} {3}", returnVal, typeSelected, tmpBodySelected, FlightGlobals.currentMainBody));
            }

            return returnVal;
        }

        private bool IsFlyButtonDisabled()
        {
            bool returnVal = true;

            if (typeSelected == "vessel")
            {
                returnVal = (tmpVesselSelected == null || FlightGlobals.ActiveVessel == tmpVesselSelected);
                //HSUtils.Log(string.Format("IsFlyButtonDisabled: {0} {1} {2} {3}", typeSelected, returnVal, tmpVesselSelected, FlightGlobals.ActiveVessel));
            }

            return returnVal;
        }

        private class DefaultScrollerView
        {

            private Vector2 scrollPos = Vector2.zero;
            private Vessel selectedVessel;
            private CelestialBody selectedBody;
            internal bool showCelestialBodies;
            private bool clicked = false;

            internal Vessel SelectedVessel
            {
                get { return this.selectedVessel; }
                set
                {
                    this.selectedVessel = value;
                    this.selectedBody = null;
                }
            }

            internal CelestialBody SelectedBody
            {
                get { return this.selectedBody; }
                set
                {
                    this.selectedBody = value;
                    this.selectedVessel = null;
                }
            }

            internal void Reset()
            {
                this.scrollPos = Vector2.zero;
                this.selectedVessel = null;
                this.selectedBody = null;
                this.clicked = false;
            }

            internal void Draw()
            {
                if ((filteredVesselList == null || !filteredVesselList.Any()) && showCelestialBodies != true)
                {
                    GUILayout.Label("No match found");
                    GUILayout.FlexibleSpace();
                    return;
                }

                this.scrollPos = GUILayout.BeginScrollView(this.scrollPos);

                GUILayout.BeginVertical();

                foreach (Vessel vessel in filteredVesselList)
                {
                    GUILayout.BeginVertical(vessel == this.SelectedVessel ? Resources.buttonVesselListPressed : GUI.skin.button);
                    GUILayout.Label(vessel.vesselName, Resources.textListHeaderStyle);
                    
                    var labelText = string.Format("{0}. {1}{2}", vessel.vesselType, Vessel.GetSituationString(vessel),
                        (FlightGlobals.ActiveVessel == vessel && vessel != null) ? ". Currently active" : "");

                    GUILayout.Label(labelText, Resources.textSituationStyle);
                    GUILayout.EndVertical();

                    Rect check = GUILayoutUtility.GetLastRect();

                    if (Event.current != null && Event.current.type == EventType.Repaint &&
                        Input.GetMouseButtonDown(0) && check.Contains(Event.current.mousePosition))
                    {

                        if (this.SelectedVessel == vessel)
                        {
                            continue;
                        }

                        this.SelectedVessel = vessel;
                        clicked = true;
                    }
                }

                // celestial bodies
                if (showCelestialBodies)
                {
                    foreach (CelestialBody body in filteredBodyList)
                    {
                        GUILayout.BeginVertical(body == this.SelectedBody ? Resources.buttonVesselListPressed : GUI.skin.button);
                        GUILayout.Label(body.name, Resources.textListHeaderStyle);
                        GUILayout.EndVertical();

                        Rect check = GUILayoutUtility.GetLastRect();

                        if (Event.current != null && Event.current.type == EventType.Repaint &&
                        Input.GetMouseButtonDown(0) && check.Contains(Event.current.mousePosition))
                        {

                            if (this.SelectedBody == body)
                            {
                                continue;
                            }

                            this.SelectedBody = body;
                            clicked = true;
                        }
                    }
                }

                GUILayout.EndVertical();
                GUILayout.EndScrollView();

                if (clicked)
                {
                    handleSelected();
                    clicked = false;
                }
            }

            private void handleSelected()
            {
                if (this.SelectedVessel != null)
                {
                    if (HSUtils.IsTrackingCenterActive)
                    {
                        HSUtils.RequestCameraFocus(this.SelectedVessel);
                    }
                    else
                    {
                        HSUtils.FocusMapObject(this.SelectedVessel.GetInstanceID());
                    }
                }
                if (this.SelectedBody != null)
                {
                    HSUtils.FocusMapObject(this.SelectedBody.GetInstanceID());
                }
            }
        }

        private class ResizeHandler
        {
            private bool resizing;
            private Vector2 lastPosition = new Vector2(0,0);

            internal void Run(Rect resizer)
            {
                if (!Event.current.isMouse)
                {
                    return;
                }

                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 &&
                    resizer.Contains(Event.current.mousePosition))
                {
                    this.resizing = true;
                    this.lastPosition.x = Input.mousePosition.x;
                    this.lastPosition.y = Input.mousePosition.y;
                    
                    Event.current.Use();
                }

                if (this.resizing && Input.GetMouseButton(0))
                {
                    var deltaX = Input.mousePosition.x - this.lastPosition.x;
                    var deltaY = Input.mousePosition.y - this.lastPosition.y;

                    //Event.current.delta does not make resizing very smooth.

                    this.lastPosition.x = Input.mousePosition.x;
                    this.lastPosition.y = Input.mousePosition.y;

                    _winRect.xMax += deltaX;
                    _winRect.yMin -= deltaY;
                    
                    Event.current.Use();
                }

                if (this.resizing && Event.current.type == EventType.MouseUp && Event.current.button == 0)
                {
                    this.resizing = false;

                    Event.current.Use();
                }
            }
        } // ResizeHandler
    } // HaystackContinued
}
