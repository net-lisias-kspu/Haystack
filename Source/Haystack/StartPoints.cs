/*
	This file is part of Haystack /L Unleashed
		© 2018-2023 LisiasT
		© 2018 linuxgurugamer
		© 2016-2018 Qberticus
		© 2013-2016 hermes-jr, enamelizer

	Haystack /L is double licensed, as follows:

		* SKL 1.0 : https://ksp.lisias.net/SKL-1_0.txt
		* GPL 2.0 : https://www.gnu.org/licenses/gpl-2.0.txt

	And you are allowed to choose the License that better suit your needs.

	Haystack /L Unleashed is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

	You should have received a copy of the SKL Standard License 1.0
	along with Haystack /L Unleashed.
	If not, see <https://ksp.lisias.net/SKL-1_0.txt>.

	You should have received a copy of the GNU General Public License 2.0
	along with Haystack /L Unleashed.
	If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using KSP.UI.Screens;
using UnityEngine;

using FILE = KSPe.IO.File<Haystack.Startup>;
using Toolbar = KSPe.UI.Toolbar;

namespace Haystack
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, true)]
    public class HaystackResourceLoader : MonoBehaviour
    {
        public Settings Settings { get; private set; }

        internal static HaystackResourceLoader instance;
        // seems to be a unity idiom
        public static HaystackResourceLoader Instance
        {
            get
            {
                return instance ??
                       (instance =
                           (HaystackResourceLoader) UnityEngine.Object.FindObjectOfType(typeof (HaystackResourceLoader)));
            }
        }

        /// <summary>
        /// Awake Event - when the DLL is loaded
        /// </summary>
        public void Awake()
        {
            API.APIAwake();
            DontDestroyOnLoad(this);

            Resources.LoadTextures();

            Resources.PopulateVesselTypes(ref Resources.vesselTypesList);
            Resources.vesselTypesList.Sort(new HSUtils.SortByWeight());

            this.Settings = new Settings();
        }

        private KSPe.UI.Toolbar.Button button;
        public void Start()
        {
			Log.dbg("Start");
			{
				button = Toolbar.Button.Create(this
						 , ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.TRACKSTATION | ApplicationLauncher.AppScenes.MAPVIEW | ApplicationLauncher.AppScenes.SPACECENTER
						 , Resources.appLauncherIcon, Resources.toolbarIcon
					 )
				 ;

				button.Toolbar
							.Add(Toolbar.Button.ToolbarEvents.Kind.Active,
								new Toolbar.Button.Event(this.handleButtonClicked, this.handleButtonClicked)
							);
				;

				ToolbarController.Instance.Add(button);
			}
		}

		internal delegate void ClickHandler();
		internal readonly List<ClickHandler> clickHandlers = new List<ClickHandler>();
		private void handleButtonClicked()
		{
			foreach (ClickHandler h in this.clickHandlers)
				h();
		}

		public void OnDestroy()
        {
            this.clickHandlers.Clear();
            ToolbarController.Instance.Destroy();
        }
       
    }

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class HaystackFlightMode : Haystack
    {
        protected override bool IsGuiDisplay
        {
            get
            {
                return HighLogic.LoadedScene == GameScenes.FLIGHT && !UIHide && this.WinVisible;
            }
        }

        protected override string SettingsName
        {
            get { return "flight"; }
        }
    }


    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class HaystackSpaceCentre : Haystack
    {
        protected override bool IsGuiDisplay
        {
            get
            {
                return HighLogic.LoadedScene == GameScenes.SPACECENTER && !UIHide && this.WinVisible;
            }
        }

        protected override string SettingsName
        {
            get { return "space_center"; }
        }
    }

    [KSPAddon(KSPAddon.Startup.TrackingStation, false)]
    public class HaystackTrackingCenter : Haystack
    {
        protected override bool IsGuiDisplay
        {
            get
            {
                return HSUtils.IsTrackingCenterActive && !UIHide && this.WinVisible;
            }
        }

        protected override string SettingsName
        {
            get { return "tracking_center"; }
        }
    }

    /// <summary>
    /// Need to wait until the user is in the space center to create the scenarios since a saved game is required for them.  
    /// </summary>
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class ScenarioModuleLoader : MonoBehaviour
    {
        public void Start()
        {
            this.setupScenarioModule();
        }

        private void setupScenarioModule()
        {
            ProtoScenarioModule protoScenarioModule =
                HighLogic.CurrentGame.scenarios.FirstOrDefault(i => i.moduleName == typeof (HaystackScenarioModule).Name);
            if (protoScenarioModule == null)
            {
                Log.dbg("adding scenario module");
                HighLogic.CurrentGame.AddProtoScenarioModule(typeof (HaystackScenarioModule),
                    HaystackScenarioModule.Scenes);
            }
            else
            {
                System.Collections.Generic.IEnumerable<GameScenes> missing = HaystackScenarioModule.Scenes.Except(protoScenarioModule.targetScenes);
                foreach (GameScenes i in missing)
                {
                    Log.dbg("missing scenario module scene: {0}", i);
                    protoScenarioModule.targetScenes.Add(i);
                }
            }
        }
    }
}