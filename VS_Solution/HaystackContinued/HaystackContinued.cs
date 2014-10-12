using System;
using System.Collections;
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
        private List<Vessel> hsVesselList = new List<Vessel>(); 
        private List<Vessel> filteredVesselList = new List<Vessel>();

        private List<CelestialBody> filteredBodyList = new List<CelestialBody>();
        private readonly Dictionary<CelestialBody, List<Vessel>> groupedBodyVessel = new Dictionary<CelestialBody, List<Vessel>>();
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

        //controls
        private readonly ResizeHandle resizeHandle = new ResizeHandle();
        private readonly DefaultScrollerView defaultScrollerView;
        private readonly GroupedScrollerView groupedScrollerView;
        private readonly BottomButtons bottomButtons = new BottomButtons();

        protected HaystackContinued()
        {
            this.defaultScrollerView = new DefaultScrollerView(this);
            this.groupedScrollerView = new GroupedScrollerView(this);
        }

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

        public void Start()
        {
            GameEvents.onHideUI.Add(onHideUI);
            GameEvents.onShowUI.Add(onShowUI);
        }

        private void onShowUI()
        {
            this.UIHide = false;
        }

        private void onHideUI()
        {
            this.UIHide = true;
        }

        protected bool UIHide { get; set; }

        public void OnDestory()
        {
            HSUtils.DebugLog("HaystackContinued#OnDestroy");

            GameEvents.onHideUI.Remove(this.onHideUI);
            GameEvents.onShowUI.Remove(this.onShowUI);
        }

        private void OnMapTargetChange(MapObject mapObject)
        {
            if (!HSUtils.IsTrackingCenterActive)
            {
                return;
            }

            if (mapObject == null)
            {
                return;
            }

            switch (mapObject.type)
            {
                case MapObject.MapObjectType.VESSEL:
                    this.defaultScrollerView.SelectedVessel = mapObject.vessel;
                    this.groupedScrollerView.SelectedVessel = mapObject.vessel;
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

                if (this.bottomButtons.GroupByOrbitingBody)
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

                this.groupedScrollerView.GUISetup(this.bottomButtons);
                this.defaultScrollerView.GUISetup(this.bottomButtons);
                this.bottomButtons.GUISetup(this.groupedScrollerView, this.defaultScrollerView);

                this.bottomButtons.OnSwitchVessel += vessel => this.StartCoroutine(SwitchToVessel(vessel));

                this.isGUISetup = true;
            }

            if (IsGuiScene)
            {
                DrawGUI();
            }
        }

        private static IEnumerator SwitchToVessel(Vessel vessel)
        {
            yield return new WaitForFixedUpdate();

            if (HSUtils.IsTrackingCenterActive)
            {
                HSUtils.TrackingSwitchToVessel(vessel);
            }
            else
            {
                FlightGlobals.SetActiveVessel(vessel);
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
            
            this.winRect = GUILayout.Window(windowId, this.winRect, this.mainWindowConstructor,
                string.Format("Haystack Continued {0}", Settings.version), Resources.winStyle, GUILayout.MinWidth(120),
                GUILayout.MinHeight(300));

            // do this here since if it's done within the window you only recieve events that are inside of the window
            this.resizeHandle.DoResize(ref this.winRect);

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

        private bool isGUISetup;

        private void mainWindowConstructor(int windowID)
        {
            GUILayout.BeginVertical();

            #region vessel types - horizontal

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
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

            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();

            #endregion vessel types

            //Search area

            GUILayout.BeginHorizontal();
            GUILayout.Label("Search:", Resources.textSearchStyle);
            filterVar = GUILayout.TextField(filterVar, GUILayout.MinWidth(50.0F), GUILayout.ExpandWidth(true));

            // clear search text
            if (GUILayout.Button("x", Resources.buttonSearchClearStyle))
            {
                filterVar = "";
            }

            // handle tooltips here so it paints over the find entry
            // TODO: clamp to window position.
            if (GUI.tooltip != "")
            {
                // get mouse position
                var mousePosition = Event.current.mousePosition;
                var width = GUI.tooltip.Length * 11;
                GUI.Box(new Rect(mousePosition.x - 30, mousePosition.y - 30, width, 25), GUI.tooltip);
            }

            GUILayout.EndHorizontal();

            if (this.bottomButtons.GroupByOrbitingBody)
            {
                this.groupedScrollerView.Draw(filteredVesselList, groupedBodyVessel);
            }   
            else
            {
                this.defaultScrollerView.Draw(filteredVesselList, filteredBodyList);
            }

            this.bottomButtons.Draw();

            GUILayout.EndVertical();

            this.resizeHandle.Draw(ref this.winRect);

            // If user input detected, force mapObject refresh
            if (GUI.changed)
            {
                MainHSActivity();
            }

            GUI.DragWindow();
        }
        
        private class GroupedScrollerView
        {
            private Vector2 scrollPos = Vector2.zero;
            private Vessel selectedVessel;
            private CelestialBody selectedBody;
            private readonly VesselInfoView vesselInfoView;


            internal GroupedScrollerView(HaystackContinued haystackContinued)
            {
                this.vesselInfoView = new VesselInfoView(haystackContinued);
            }

            public Vessel SelectedVessel
            {
                get { return this.selectedVessel; } 
                set { this.selectedVessel = value; }
            }

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

                    selected = GUILayout.Toggle(selected, new GUIContent(body.name), Resources.buttonTextOnly);

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
                        //this typically happens when debris is going out of physics range and is deleted by the game
                        if (vessel == null)
                        {
                            continue;
                        }

                        this.vesselInfoView.Draw(vessel, vessel == this.selectedVessel);

                        if (!this.vesselInfoView.Clicked)
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
                this.vesselInfoView.Reset();

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

            internal void GUISetup(BottomButtons bottomButtons)
            {
                bottomButtons.OnGroupByChanged += view => this.reset();
            }

            private void reset()
            {
                this.scrollPos = Vector2.zero;
                this.selectedVessel = null;
                this.selectedBody = null;
                this.vesselInfoView.Reset();
            }
        }

        private class DefaultScrollerView
        {
            private Vector2 scrollPos = Vector2.zero;
            private Vessel selectedVessel;
            private CelestialBody selectedBody;

            private VesselInfoView vesselInfoView;

            internal DefaultScrollerView(HaystackContinued haystackContinued)
            {
                this.vesselInfoView = new VesselInfoView(haystackContinued);
            }

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

            private void reset()
            {
                this.scrollPos = Vector2.zero;
                this.selectedVessel = null;
                this.selectedBody = null;
                this.vesselInfoView.Reset();
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
                    //this typically happens when debris is going out of physics range and is deleted by the game
                    if (vessel == null)
                    {
                        continue;
                    }

                    this.vesselInfoView.Draw(vessel, vessel == this.selectedVessel);

                    if (!this.vesselInfoView.Clicked)
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
                        GUILayout.BeginVertical(body == this.SelectedBody ? Resources.buttonVesselListPressed : Resources.buttonTextOnly);
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

                this.vesselInfoView.Reset();

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

            internal void GUISetup(BottomButtons bottomButtons)
            {
                bottomButtons.OnGroupByChanged += view => this.reset();
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
            }

            internal void DoResize(ref Rect winRect)
            {
                if (!this.resizing)
                {
                    return;
                }

                if (Input.GetMouseButton(0))
                {
                    var deltaX = Input.mousePosition.x - this.lastPosition.x;
                    var deltaY = Input.mousePosition.y - this.lastPosition.y;

                    //Event.current.delta does not make resizing very smooth.

                    this.lastPosition.x = Input.mousePosition.x;
                    this.lastPosition.y = Input.mousePosition.y;

                    winRect.xMax += deltaX;
                    winRect.yMin -= deltaY;

                    if (Event.current.isMouse)
                    {
                        Event.current.Use();
                    }
                }

                if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
                {
                    this.resizing = false;

                    Event.current.Use();
                }
            }
        } // ResizeHandle


        private class BottomButtons
        {
            private GroupedScrollerView groupedScrollerView;
            private DefaultScrollerView defaultScrollerView;

            internal bool GroupByOrbitingBody { get; set; }

            private void groupByButton()
            {
                //group by toggle
                var previous = this.GroupByOrbitingBody;
                this.GroupByOrbitingBody = GUILayout.Toggle(this.GroupByOrbitingBody, new GUIContent(Resources.btnOrbitIcon, "Group by orbiting body"),
                    GUI.skin.button, GUILayout.Width(32f), GUILayout.Height(32f));

                if (previous != this.GroupByOrbitingBody)
                {
                    this.fireOnGroupByChanged(this);
                }
            }

            private void targetButton()
            {
                // Disable buttons for current vessel or nothing selected
                if (this.isTargetButtonDisabled())
                {
                    GUI.enabled = false;
                }

                // target button
                if (GUILayout.Button(Resources.btnTarg, Resources.buttonTargStyle))
                {
                    ITargetable selected = null;

                    if (this.GroupByOrbitingBody)
                    {
                        selected = this.groupedScrollerView.SelectedVessel;
                    }
                    else
                    {
                        selected = (ITargetable)this.defaultScrollerView.SelectedVessel ?? this.defaultScrollerView.SelectedBody;
                    }

                    if (selected != null)
                    {
                        FlightGlobals.fetch.SetVesselTarget(selected);
                    }
                }

                GUI.enabled = true;
            }

            private void flyButton()
            {
                // Disable fly button if we selected a body, have no selection, or selected the current vessel
                if (this.isFlyButtonDisabled())
                {
                    GUI.enabled = false;
                }

                // fly button
                if (GUILayout.Button(Resources.btnGoHover, Resources.buttonGoStyle))
                {
                    this.fireOnSwitchVessel(this.getSelectedVessel());
                }

                GUI.enabled = true;
            }

            internal void Draw()
            {
                GUILayout.BeginHorizontal();
                
                this.groupByButton();
                
                GUILayout.FlexibleSpace();

                this.targetButton();
                this.flyButton();

                GUILayout.EndHorizontal();
            }

            private bool isTargetButtonDisabled()
            {
                if (HSUtils.IsTrackingCenterActive)
                {
                    return true;
                }

                if (this.GroupByOrbitingBody)
                {
                    return this.groupedScrollerView.SelectedVessel == null ||
                           this.groupedScrollerView.SelectedVessel == FlightGlobals.ActiveVessel;
                }

                return this.defaultScrollerView.SelectedVessel == null ||
                       FlightGlobals.ActiveVessel == this.defaultScrollerView.SelectedVessel;
            }

            private bool isFlyButtonDisabled()
            {
                var vessel = this.GroupByOrbitingBody ? this.groupedScrollerView.SelectedVessel : this.defaultScrollerView.SelectedVessel;

                return vessel == null || FlightGlobals.ActiveVessel == vessel;
            }

            private Vessel getSelectedVessel()
            {
                return this.GroupByOrbitingBody
                    ? this.groupedScrollerView.SelectedVessel
                    : this.defaultScrollerView.SelectedVessel;
            }

            internal delegate void OnGroupByChangedHandler(BottomButtons view);
            internal event OnGroupByChangedHandler OnGroupByChanged;
            protected virtual void fireOnGroupByChanged(BottomButtons view)
            {
                var handler = this.OnGroupByChanged;
                if (handler != null) handler(view);
            }

            internal delegate void OnSwitchVesselHandler(Vessel vessel);
            internal event OnSwitchVesselHandler OnSwitchVessel;
            protected virtual void fireOnSwitchVessel(Vessel vessel)
            {
                var handler = this.OnSwitchVessel;
                if (handler != null) handler(vessel);
            }

            internal void GUISetup(GroupedScrollerView groupedScrollerView, DefaultScrollerView defaultScrollerView)
            {
                this.groupedScrollerView = groupedScrollerView;
                this.defaultScrollerView = defaultScrollerView;
            }
        }

        internal class VesselInfoView
        {
            internal bool Clicked { get; set; }
            private Vessel expandedVessel = null;
            private bool selected;
            private DockingPortListView dockingPortListView;

            private HaystackContinued haystackContinued;

            internal VesselInfoView(HaystackContinued haystackContinued)
            {
                this.haystackContinued = haystackContinued;
                this.dockingPortListView = new DockingPortListView(haystackContinued);
            }

            internal void Reset()
            {
                this.Clicked = false;
                this.expandedVessel = null;
                this.selected = false;
                this.dockingPortListView.CurrentVessel = null;
            }

            internal void Draw(Vessel vessel, bool selected)
            {
                this.Clicked = false;
                this.selected = selected;

                GUILayout.BeginVertical(selected ? Resources.buttonVesselListPressed : Resources.buttonTextOnly);

                GUILayout.BeginHorizontal();
                GUILayout.Label(vessel.vesselName, Resources.textListHeaderStyle);
                GUILayout.FlexibleSpace();
                this.drawDistance(vessel);
                GUILayout.EndHorizontal();

                drawVesselInfoText(vessel);

                GUILayout.EndVertical();

                var check = GUILayoutUtility.GetLastRect();

                if (!this.selected && Event.current != null && Event.current.type == EventType.Repaint && Input.GetMouseButtonDown(0) &&
                    check.Contains(Event.current.mousePosition))
                {
                    this.Clicked = true;
                }
            }

            private void drawDistance(Vessel vessel)
            {
                string distance = "";
                var activeVessel = FlightGlobals.ActiveVessel;
                if (!HSUtils.IsTrackingCenterActive && vessel != activeVessel)
                {
                    var calcDistance = Vector3.Distance(activeVessel.transform.position, vessel.transform.position);
                    distance = HSUtils.ToSI(calcDistance) + "m";
                }

                GUILayout.Label(distance, Resources.textSituationStyle);
            }


            private void drawVesselInfoText(Vessel vessel)
            {

                var activeVessel = FlightGlobals.ActiveVessel;
                var status = "";
                if (activeVessel == vessel)
                {
                    status = ". Currently active";
                }
                else if (vessel.loaded)
                {
                    status = ". Loaded";
                }

                var situation = string.Format("{0}. {1}{2}",
                    vessel.vesselType,
                    Vessel.GetSituationString(vessel),
                    status);

                GUILayout.BeginHorizontal();

                GUILayout.Label(situation, Resources.textSituationStyle);
                if (this.selected)
                { 
                    GUILayout.FlexibleSpace();

                    drawDockingExpandButton(vessel);
                }
                GUILayout.EndHorizontal();


                this.dockingPortListView.Draw(vessel);
            }

            private void drawDockingExpandButton(Vessel vessel)
            {
                //can't show docking ports in the tracking center
                if (HSUtils.IsTrackingCenterActive)
                {
                    return;
                }

                var enabled = vessel == this.expandedVessel;
                var icon = enabled ? Resources.btnDownArrow : Resources.btnUpArrow;

                var result = GUILayout.Toggle(enabled, new GUIContent(icon, "Show Docking Ports"), Resources.buttonExpandStyle);

                if (result != enabled)
                {
                    this.expandedVessel = result ? vessel : null;
                    this.dockingPortListView.CurrentVessel = this.expandedVessel;
                }
            }
        }

        internal class DockingPortListView
        {
            private Vessel currentVessel;
            private readonly List<PortInfo> portList = new List<PortInfo>();
            private readonly HaystackContinued haystackContinued;
            private bool runUpdate;

            private static readonly Type moduleDockingNodeNamedType;
            private static readonly FieldInfo modulePortName;
            private static bool namedDockingPortSupport;

            static DockingPortListView()
            {
                try
                {
                   moduleDockingNodeNamedType =  AssemblyLoader.loadedAssemblies.SelectMany(a => a.assembly.GetExportedTypes())
                        .SingleOrDefault(t => t.FullName == "DockingPortAlignment.ModuleDockingNodeNamed");

                    modulePortName = moduleDockingNodeNamedType.GetField("portName",
                        BindingFlags.Instance | BindingFlags.Public);

                }
                catch (Exception e)
                {
                    moduleDockingNodeNamedType = null;
                    modulePortName = null;
                    HSUtils.DebugLog("exception getting docking port alignment indicator type");
                    HSUtils.DebugLog("{0}", e.Message);
                }

                if (moduleDockingNodeNamedType != null && modulePortName != null)
                {
                    namedDockingPortSupport = true;

                    HSUtils.Log("Docking Port Alignment Indicator mod detected: using named docking node support.");
                    HSUtils.DebugLog("{0} {1}", moduleDockingNodeNamedType.FullName, moduleDockingNodeNamedType.AssemblyQualifiedName);
                }
                else
                {
                    HSUtils.DebugLog("Docking Port Alignment Indicator mod was not detected");
                }
            }

            internal DockingPortListView(HaystackContinued haystackContinued)
            {
                this.haystackContinued = haystackContinued;
            }

            internal Vessel CurrentVessel
            {
                get { return this.currentVessel; }
                set
                {
                    this.currentVessel = value;
                    this.handleVesselChange();
                }
            }

            private void handleVesselChange()
            {
                if (this.currentVessel == null)
                {
                    this.portList.Clear();
                    this.runUpdate = false;
                    return;
                }

                this.runUpdate = true;
                this.haystackContinued.StartCoroutine(this.updatePortList());
            }

            public IEnumerator updatePortList()
            {
                while (this.runUpdate)
                {
                    yield return new WaitForEndOfFrame();

                    this.populatePortList();

                    yield return new WaitForSeconds(30f);
                }
            }

            public void populatePortList()
            {
                this.portList.Clear();

                var targetables = this.currentVessel.FindPartModulesImplementing<ITargetable>();
                foreach (var targetable in targetables)
                {
                    var port = targetable as ModuleDockingNode;
                    if (port == null)
                    {
                        continue;
                    }

                    if (port.state.StartsWith("Docked", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    // don't display docking ports that have all their attach nodes used.
                    var usedNodeCount = port.part.attachNodes.Count(node => node.attachedPart != null);
                    if (usedNodeCount == port.part.attachNodes.Count)
                    {
                        continue;
                    }

                    var info = new PortInfo
                    {
                        Name = getPortName(port),
                        PortNode = port,
                    };

                    this.portList.Add(info);
                }

                portList.Sort((a, b) => a.Name.CompareTo(b.Name));
            }

            private string getPortName(ModuleDockingNode port)
            {
                HSUtils.DebugLog("DockingPortListView#getPortName: start");

                if (!namedDockingPortSupport)
                {
                    return port.part.partInfo.title;
                }
            
                PartModule found = null;
                for (int i = 0; i < port.part.Modules.Count; i++)
                {
                    var module = port.part.Modules[i];
                    if (module.GetType() == moduleDockingNodeNamedType)
                    {
                        found = module;
                        break;
                    }
                }

                if (found == null)
                {
                    HSUtils.DebugLog("DockingPortListView#getPortName: named docking port support enabled but could not find the part module");
                    return port.part.partInfo.title;
                }

                var portName = (string) modulePortName.GetValue(found);

                HSUtils.DebugLog("DockingPortListView#getPortName: found name: {0}", portName);

                return portName;
            }

            internal void Draw(Vessel vessel)
            {
                if (this.CurrentVessel == null || vessel != this.CurrentVessel)
                {
                    return;
                }

                if (this.CurrentVessel.packed)
                {
                    GUILayout.BeginVertical();
                    GUILayout.Label("The vessel is out of range: cannot list docking ports", Resources.textSituationStyle);
                    GUILayout.EndVertical();
                    return;
                }

                if (this.portList.IsEmpty())
                {
                    GUILayout.BeginVertical();
                    GUILayout.Label("This vessel does not have any docking ports", Resources.textSituationStyle);
                    GUILayout.EndVertical();
                    return;
                }

                GUILayout.BeginVertical();

                GUILayout.Label("Docking Ports", Resources.textDockingPortHeaderStyle);

                foreach (var i in portList)
                {
                    GUILayout.Box((Texture)null, Resources.hrSepLineStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false));

                    GUILayout.BeginHorizontal();

                    GUILayout.Label(i.Name, Resources.textDockingPortStyle, GUILayout.ExpandHeight(false));
                    GUILayout.FlexibleSpace();

                    if (FlightGlobals.ActiveVessel != this.currentVessel) 
                    {
                        var distance = this.getDistanceText(i.PortNode);
                        GUILayout.Label(distance, Resources.textDockingPortDistanceStyle, GUILayout.ExpandHeight(true));
                        GUILayout.Space(10f);
                        if (GUILayout.Button(Resources.btnTargetAlpha, Resources.buttonDockingPortTarget, GUILayout.Width(18f),
                              GUILayout.Height(18f)))
                        {
                            setDockingPortTarget(i.PortNode);
                        }

                        GUILayout.Space(10f);
                    }

                    GUILayout.EndHorizontal();
                }

                GUILayout.EndVertical();
            }

            private void setDockingPortTarget(ModuleDockingNode portNode)
            {
                var vessel = portNode.GetVessel();

                //can't set target if the vessel is not loaded or is the active vessel
                if (vessel.packed || vessel.isActiveVessel)
                {
                    return;
                }

                FlightGlobals.fetch.SetVesselTarget(portNode);
            }

            private string getDistanceText(ModuleDockingNode port)
            {
                var activeVessel = FlightGlobals.ActiveVessel;
                var distance = Vector3.Distance(activeVessel.transform.position, port.GetTransform().position);

                return string.Format("{0}m", HSUtils.ToSI(distance));
            }

            private struct PortInfo
            {
                internal string Name;
                internal ModuleDockingNode PortNode;
            }
        }
    } // HaystackContinued
}
