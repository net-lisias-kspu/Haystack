using System;
using System.Collections.Generic;
using System.Linq;

namespace HaystackReContinued
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
                var hiddenVessles = new HiddenVessels();
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

        public void RemoveVessle(Vessel vessel)
        {
            if (ExcludedTypes.Contains(vessel.vesselType))
            {
                throw new ArgumentOutOfRangeException("vessel type: " + vessel.vesselType + " is invalid.");
            }

            this.hiddenVessels.Remove(vessel.id);
        }

        public void Save(ConfigNode node)
        {
            var ns = new NodeSeralizer {hiddenVessels = this.hiddenVessels};

            try
            {
                var saveNode = new ConfigNode(this.GetType().Name);
                ConfigNode nsNode = ConfigNode.CreateConfigFromObject(ns);

                saveNode.AddNode(nsNode);
                node.AddNode(saveNode);
            }
            catch (Exception e)
            {
                HSUtils.Log("HiddenVessles#Save: exception: " + e.Message);
            }
        }

        public void Load(ConfigNode node)
        {
            ConfigNode loadNode = node.GetNode(this.GetType().Name);
            if (loadNode == null)
            {
                HSUtils.DebugLog("HiddenVessles#Load: node is null");
                return;
            }

            var ns = new NodeSeralizer();

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