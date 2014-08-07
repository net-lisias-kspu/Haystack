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


    public abstract class HaystackContinued : MonoBehaviour
    {
        private Vessel switchToMe;
        private List<Vessel> hsVesselList = new List<Vessel>(); 
        private List<Vessel> filteredVesselList = new List<Vessel>();

        private List<CelestialBody> filteredBodyList = new List<CelestialBody>();
        private Dictionary<CelestialBody, List<Vessel>> groupedBodyVessel = new Dictionary<CelestialBody, List<Vessel>>();
        private bool showCelestialBodies = true;

        // number of vessles per type
        // TODO: move somewhere centralized
        private Dictionary<string, int> typeCount;

        // window vars
        private int windowId;
       
        private bool winHidden = true;
        private Rect winRect;

        // Search text
        private string filterVar = "";

        public void Awake()
        {
            HSUtils.DebugLog("HaystackContinued#Awake");

            typeCount = new Dictionary<string, int>();
            windowId = Resources.rnd.Next(1000, 2000000);
        }

        public void OnEnable()
        {
            HSUtils.DebugLog("HaystackContinued#OnEnable");

            GameEvents.onPlanetariumTargetChanged.Add(OnMapTargetChange);

            this.WinRect = HaystackResourceLoader.Instance.Settings.WindowPositions[this.SettingsName];

            InvokeRepeating("MainHSActivity", 5.0F, 5.0F); // Refresh from time to time just in case
            InvokeRepeating("RefreshDataSaveSettings", 0, 30.0F);
        }

        public void OnDisable()
        {
            HSUtils.DebugLog("HaystackContinued#OnDisable");
            CancelInvoke();

            GameEvents.onPlanetariumTargetChanged.Remove(this.OnMapTargetChange);
            HaystackResourceLoader.Instance.Settings.WindowPositions[this.SettingsName] = this.WinRect;

            HaystackResourceLoader.Instance.Settings.Save();
        }

        public void OnDestory()
        {
            HSUtils.DebugLog("HaystackContinued#OnDestroy");
        }

        private void OnMapTargetChange(MapObject mapObject)
        {
            if (mapObject == null)
            {
                return;
            }

            switch (mapObject.type)
            {
                case MapObject.MapObjectType.VESSEL:
                    this.defaultScrollerView.SelectedVessel = mapObject.vessel;
                    break;
                case MapObject.MapObjectType.CELESTIALBODY:
                    this.defaultScrollerView.SelectedBody = mapObject.celestialBody;
                    break;
                default:
                    this.defaultScrollerView.SelectedBody = null;
                    this.defaultScrollerView.SelectedVessel = null;
                    break;
            }
        }

        /// <summary>
        /// Refresh list of vessels
        /// </summary>
        private void RefetchVesselList()
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
           if (IsGuiScene)
            {
                // refresh filter lists
                filteredVesselList = new List<Vessel>(hsVesselList);
                filteredBodyList = new List<CelestialBody>(Resources.CelestialBodies);

                if (hsVesselList != null)
                {
                    if (Resources.vesselTypesList != null)
                    {
                        // For each hidden type remove it from the list
                        // FIXME: must be optimized
                        foreach (HSVesselType currentInvisibleType in Resources.vesselTypesList)
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
                            filteredVesselList.RemoveAll(
                                v => -1 == v.vesselName.IndexOf(this.filterVar, StringComparison.OrdinalIgnoreCase)
                            );

                        if (showCelestialBodies)
                        {
                            filteredBodyList.RemoveAll(
                                cb => -1 == cb.bodyName.IndexOf(filterVar, StringComparison.OrdinalIgnoreCase)
                            );
                        }
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
            if (!IsGuiScene) return;
            
            this.RefetchVesselList();

            HaystackResourceLoader.Instance.Settings.WindowPositions[this.SettingsName] = this.WinRect;
        }

        /// <summary>
        /// Repaint GUI (only in map view condition inside)
        /// </summary>
        public void OnGUI()
        {
            if (!this.isGUISetup)
            {
                Resources.LoadStyles();
                this.isGUISetup = true;
            }

            if (IsGuiScene)
            {
                DrawGUI();
            }
        }

        public Rect WinRect
        {
            get { return this.winRect; }
            set { this.winRect = value; }
        }

        public void DrawGUI()
        {
            GUI.skin = HighLogic.Skin;

            if (winHidden)
            {
                this.winRect.y = Screen.height - 1;
            }
            else
            {
                this.winRect.y = Screen.height - this.winRect.height;
                this.winRect = this.winRect.ClampToScreen();
            }
            
            this.winRect = GUILayout.Window(windowId, this.winRect, MainWindowConstructor,
                string.Format("Haystack {0}", Settings.version), Resources.winStyle, GUILayout.MinWidth(120),
                GUILayout.MinHeight(300));

            if (GUI.Button(new Rect(this.winRect.x + (this.winRect.width/2 - 24), this.winRect.y - 9, 48, 10), "",
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
        protected virtual bool IsGuiScene
        {
            get
            {
                return false;
            }
        }

        protected abstract string SettingsName { get; }

        // For the scrollview
        private Vector2 scrollPos = Vector2.zero;

        // Keep track of selections in GUILayouts
        private Vessel tmpVesselSelected;
        private CelestialBody tmpBodySelected;
        private string typeSelected;
        private bool groupByOrbitingBody;
        private readonly ResizeHandle resizeHandle = new ResizeHandle();
        private readonly DefaultScrollerView defaultScrollerView = new DefaultScrollerView();
        private readonly GroupedScrollerView groupedScrollerView = new GroupedScrollerView();
        private bool isGUISetup;

        private void MainWindowConstructor(int windowID)
        {
            GUILayout.BeginVertical();

            #region vessel types - horizontal

            GUILayout.BeginHorizontal();

            // Vessels
            for (int i = 0; i < Resources.vesselTypesList.Count(); i++)
            {
                var typeString = Resources.vesselTypesList[i].name;

                if (typeCount.ContainsKey(typeString))
                    typeString += string.Format(" ({0})", typeCount[typeString]);

                Resources.vesselTypesList[i].visible = GUILayout.Toggle(Resources.vesselTypesList[i].visible,
                    new GUIContent(Resources.vesselTypesList[i].icon, typeString), Resources.buttonVesselTypeStyle);
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
                this.groupedScrollerView.Draw(filteredVesselList, groupedBodyVessel);
            }   
            else
            {
                this.defaultScrollerView.Draw(filteredVesselList, filteredBodyList);
            }


            #region bottom buttons - horizontal

            GUILayout.BeginHorizontal();

            //group by toggle
            var previous = this.groupByOrbitingBody;
            this.groupByOrbitingBody = GUILayout.Toggle(this.groupByOrbitingBody, new GUIContent("GB", "Group by orbit"),
                Resources.buttonVesselTypeStyle);

            if (previous != this.groupByOrbitingBody)
            {
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

            this.resizeHandle.Draw(ref this.winRect);


            // If user input detected, force mapObject refresh
            if (GUI.changed)
            {
                MainHSActivity();
            }

            GUI.DragWindow();
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

        private class GroupedScrollerView
        {
            private Vector2 scrollPos = Vector2.zero;
            private Vessel selectedVessel;
            private CelestialBody selectedBody;


            internal void Draw(List<Vessel> filteredVessels, Dictionary<CelestialBody, List<Vessel>> groupedBodyVessel)
            {
                if (filteredVessels == null || filteredVessels.IsEmpty())
                {
                    GUILayout.Label("No matched vessels foud");

                    GUILayout.FlexibleSpace();
                    return;
                }

                this.scrollPos = GUILayout.BeginScrollView(scrollPos);

                Vessel preSelectedVessel = null;

                var clicked = false;

                GUILayout.BeginVertical();
                
                foreach (var kv in groupedBodyVessel)
                {
                    var body = kv.Key;
                    var vessels = kv.Value;

                    var selected = body == selectedBody;

                    selected = GUILayout.Toggle(selected, new GUIContent(body.name), GUI.skin.button);

                    if (selected)
                    {
                        this.selectedBody = body;
                    }
                    else
                    {
                        if (this.selectedBody == body)
                        {
                            this.selectedBody = null;
                        }
                        continue;
                    }

                    foreach (var vessel in vessels)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(20f);

                        GUILayout.BeginVertical(vessel == this.selectedVessel 
                            ? Resources.buttonVesselListPressed
                            : GUI.skin.button);
                        
                        GUILayout.Label(vessel.vesselName, Resources.textListHeaderStyle);
                        
                        GUILayout.Label(
                            string.Format("{0}. {1}{2}", vessel.vesselType.ToString(), Vessel.GetSituationString(vessel),
                                (FlightGlobals.ActiveVessel == vessel) ? ". Currently active" : ""),
                            Resources.textSituationStyle);

                        GUILayout.EndVertical();

                        var check = GUILayoutUtility.GetLastRect();
                        
                        GUILayout.EndHorizontal();

                        if (Event.current == null || Event.current.type != EventType.Repaint ||
                            !Input.GetMouseButtonDown(0) || !check.Contains(Event.current.mousePosition) || this.selectedVessel == vessel) 
                        {
                            continue;
                        }

                        preSelectedVessel = vessel;
                        clicked = true;
                    }
                }

                GUILayout.EndVertical();
                GUILayout.EndScrollView();

                var checkInScroll = GUILayoutUtility.GetLastRect();
                if (!clicked || !checkInScroll.Contains(Event.current.mousePosition))
                {
                    return;
                }

                this.selectedVessel = preSelectedVessel;

                this.changeCameraTarget();
            }

            private void changeCameraTarget()
            {
                if (this.selectedVessel == null)
                {
                    return;
                }

                if (HSUtils.IsTrackingCenterActive)
                {
                    HSUtils.RequestCameraFocus(this.selectedVessel);
                }
                else
                {
                    HSUtils.FocusMapObject(this.selectedVessel.GetInstanceID());
                }
            }
        }

        private class DefaultScrollerView
        {

            private Vector2 scrollPos = Vector2.zero;
            private Vessel selectedVessel;
            private CelestialBody selectedBody;
            internal bool showCelestialBodies;

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
                
            }

            internal void Draw(List<Vessel> filteredVessels, List<CelestialBody> filteredBodies)
            {
                if ((filteredVessels == null || filteredVessels.IsEmpty()) && showCelestialBodies != true)
                {
                    GUILayout.Label("No match found");
                    GUILayout.FlexibleSpace();
                    return;
                }

                var clicked = false;
                Vessel preSelectedVessel = null;
                CelestialBody preSelecedBody = null;

                this.scrollPos = GUILayout.BeginScrollView(this.scrollPos);

                GUILayout.BeginVertical();

                foreach (var vessel in filteredVessels)
                {
                    GUILayout.BeginVertical(vessel == this.SelectedVessel ? Resources.buttonVesselListPressed : GUI.skin.button);
                    GUILayout.Label(vessel.vesselName, Resources.textListHeaderStyle);
                    
                    var labelText = string.Format("{0}. {1}{2}", vessel.vesselType, Vessel.GetSituationString(vessel),
                        (FlightGlobals.ActiveVessel == vessel) ? ". Currently active" : "");

                    GUILayout.Label(labelText, Resources.textSituationStyle);
                    GUILayout.EndVertical();

                    var check = GUILayoutUtility.GetLastRect();

                    if (Event.current == null || Event.current.type != EventType.Repaint || !Input.GetMouseButtonDown(0) ||
                        !check.Contains(Event.current.mousePosition))
                    {
                        continue;
                    }
                    
                    if (this.SelectedVessel == vessel)
                    {
                        continue;
                    }

                    preSelectedVessel = vessel;
                    clicked = true;
                }

                // celestial bodies
                if (showCelestialBodies)
                {
                    foreach (var body in filteredBodies)
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

                            preSelecedBody = body;
                            clicked = true;
                        }
                    }
                }

                GUILayout.EndVertical();
                GUILayout.EndScrollView();

                var checkInScrollClick = GUILayoutUtility.GetLastRect();

                //clicks to items in scroll view can happen outside of the scroll view
                if (!clicked || Event.current == null || !checkInScrollClick.Contains(Event.current.mousePosition))
                {
                    return;
                }
                
                
                if (preSelecedBody != null)
                {
                    this.SelectedBody = preSelecedBody;
                }
                if (preSelectedVessel != null)
                {
                    this.SelectedVessel = preSelectedVessel;
                }

                this.changeCameraTarget();
                this.fireOnSelectionChanged(this);

            }

            private void changeCameraTarget()
            {
                if (this.SelectedVessel != null)
                {
                    if (HSUtils.IsTrackingCenterActive)
                    {
                        HSUtils.RequestCameraFocus(this.SelectedVessel);
                    }
                    else
                    {
                        HSUtils.FocusMapObject(this.selectedVessel.GetInstanceID());
                    }
                }
                if (this.SelectedBody != null)
                {
                    HSUtils.FocusMapObject(this.SelectedBody.GetInstanceID());
                }
            }

            internal delegate void OnSelectionChangedHandler(DefaultScrollerView scrollerView);

            internal event OnSelectionChangedHandler OnSelectionChanged;

            protected virtual void fireOnSelectionChanged(DefaultScrollerView scrollerview)
            {
                OnSelectionChangedHandler handler = this.OnSelectionChanged;
                if (handler != null) handler(scrollerview);
            }
        }

        private class ResizeHandle
        {
            private bool resizing;
            private Vector2 lastPosition = new Vector2(0,0);

            internal void Draw(ref Rect winRect)
            {
                var resizer = new Rect(winRect.width - 24f - 2f, 2f, 24f, 24f);
                GUI.Box(resizer, "//", GUI.skin.box);
                
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

                    winRect.xMax += deltaX;
                    winRect.yMin -= deltaY;
                    
                    Event.current.Use();
                }

                if (this.resizing && Event.current.type == EventType.MouseUp && Event.current.button == 0)
                {
                    this.resizing = false;

                    Event.current.Use();
                }
            }
        } // ResizeHandle
    } // HaystackContinued
}
