/*
	This file is part of Haystack /L Unleashed
		© 2018-2021 LisiasT
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

namespace Haystack
{
    internal class DataManager
    {
        public delegate void OnDataLoadedHandler();

        private readonly SaveLoadV1 saveLoadV1;

        static DataManager()
        {
            Instance = new DataManager();
        }

        private DataManager()
        {
            this.saveLoadV1 = new SaveLoadV1(this);
            this.HiddenVessels = new HiddenVessels();
        }

        public static DataManager Instance { get; private set; }

        public HiddenVessels HiddenVessels { get; private set; }

        public void Save(ConfigNode node)
        {
            this.saveLoadV1.Save(node);
        }

        public void Load(ConfigNode node)
        {
            this.saveLoadV1.Load(node);

            this.fireOnDataLoaded();
        }

        public event OnDataLoadedHandler OnDataLoaded;

        private void fireOnDataLoaded()
        {
            OnDataLoadedHandler handler = this.OnDataLoaded;
            if (handler == null)
            {
                return;
            }

            handler();
        }

        private class SaveLoadV1
        {
            private const int VERSION = 1;
            private readonly DataManager parent;

            public SaveLoadV1(DataManager parent)
            {
                this.parent = parent;
            }

            public void Save(ConfigNode node)
            {
                node.AddValue("VERSION", VERSION);
                this.parent.HiddenVessels.Save(node);
            }

            public void Load(ConfigNode node)
            {
                HiddenVessels hiddenVessles = new HiddenVessels();
                hiddenVessles.Load(node);

                this.parent.HiddenVessels = hiddenVessles;
            }
        }
    }

    internal class HiddenVessels
    {
        public static VesselType[] ExcludedTypes = {VesselType.SpaceObject, VesselType.Unknown, VesselType.Debris};
        private HashSet<Guid> hiddenVessels = new HashSet<Guid>();

        /// <summary>
        ///     Treat as immutable
        /// </summary>
        public HashSet<Guid> VesselList
        {
            get { return this.hiddenVessels; }
        }

        public void AddVessel(Vessel vessel)
        {
            if (ExcludedTypes.Contains(vessel.vesselType))
            {
                throw new ArgumentOutOfRangeException("vessel type: " + vessel.vesselType + " is invalid.");
            }

            this.hiddenVessels.Add(vessel.id);
        }

        public void RemoveVessel(Vessel vessel)
        {
            if (ExcludedTypes.Contains(vessel.vesselType))
            {
                throw new ArgumentOutOfRangeException("vessel type: " + vessel.vesselType + " is invalid.");
            }

            this.hiddenVessels.Remove(vessel.id);
        }

        public void Save(ConfigNode node)
        {
            NodeSeralizer ns = new NodeSeralizer {hiddenVessels = this.hiddenVessels};

            try
            {
                ConfigNode saveNode = new ConfigNode(this.GetType().Name);
                ConfigNode nsNode = ConfigNode.CreateConfigFromObject(ns);

                saveNode.AddNode(nsNode);
                node.AddNode(saveNode);
            }
            catch (Exception e)
            {
                Log.err("HiddenVessles#Save: exception: {0}", e.Message);
                Log.ex(this, e);
            }
        }

        public void Load(ConfigNode node)
        {
            ConfigNode loadNode = node.GetNode(this.GetType().Name);
            if (loadNode == null)
            {
                Log.dbg("HiddenVessles#Load: node is null");
                return;
            }

            NodeSeralizer ns = new NodeSeralizer();

            ConfigNode.LoadObjectFromConfig(ns, loadNode.GetNode(ns.GetType().FullName));

            this.hiddenVessels = ns.hiddenVessels;
        }


        public class NodeSeralizer : IPersistenceLoad, IPersistenceSave
        {
            public HashSet<Guid> hiddenVessels = new HashSet<Guid>();

            [Persistent] public List<string> hiddenVesselsList = new List<string>();

            public void PersistenceLoad()
            {
                foreach (string i in this.hiddenVesselsList)
                {
                    this.hiddenVessels.Add(new Guid(i));
                }
            }

            public void PersistenceSave()
            {
                foreach (Guid i in this.hiddenVessels)
                {
                    this.hiddenVesselsList.Add(i.ToString());
                }
            }
        }
    }
}