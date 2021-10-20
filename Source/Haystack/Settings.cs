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
using System.ComponentModel;
using UnityEngine;

using DATA = KSPe.IO.Data<Haystack.Settings>;

namespace Haystack
{
    public class Settings
    {
		private static readonly string SettingsFile = "settings";

        private const string NODE_SETTINGS = "settings";
        private const string NODE_WINDOW_POSITIONS = "window_positions";
        private const string NODE_WINDOW_VISIBILITIES = "window_visibilities";
        private const string BOTTOM_BUTTONS = "bottom_buttons";
        private const string NODE_VESSEL_TYPE_VISIBILITY = "type_visibility";
        private const string WINDOW_POSITION = "position";
        private const string WINDOW_VISIBLE = "window_visible";
        private const String BUTTON_STATE = "button_state";
        internal const string VALUE = "value";

        private readonly Dictionary<string, Rect> windowPositions = new Dictionary<string, Rect>();
        private readonly Dictionary<string, bool> windowVisibilities = new Dictionary<string, bool>();
        private readonly Dictionary<string, bool> bottomButtons = new Dictionary<string, bool>();

        private static readonly DATA.ConfigNode SETTINGS = DATA.ConfigNode.For(NODE_SETTINGS, SettingsFile);

        public Settings()
        {
            this.WindowPositions = new GenericIndexer<Rect>(windowPositions, () => new Rect(0, 0, 0, 0),
                "settings: window WindowPosition: {0} {1}");
            this.WindowVisibilities = new GenericIndexer<bool>(windowVisibilities, () => false,
                "settings: window WindowVisibility: {0} {1}");
            this.BottomButtons = new GenericIndexer<bool>(bottomButtons, () => false, "settings: bottom buttons: {0} {1}");
            
            this.Load();
        }

        private void Load()
        {
            Log.detail("loading settings");
            Log.dbg("Settings#Load: start");

            if (!SETTINGS.IsLoadable)
            {
                SETTINGS.Clear();
                SETTINGS.Save();
            }

            ConfigNode config = SETTINGS.Load().Node;

            ConfigNode nodeWindowPositions = config.GetNode(NODE_WINDOW_POSITIONS) ?? new ConfigNode();

            ConfigNode nodeWindowVisibility = config.GetNode(NODE_WINDOW_VISIBILITIES) ?? new ConfigNode();

            ConfigNode bottomButtons = config.GetNode(BOTTOM_BUTTONS) ?? new ConfigNode();

            Rect defaultPos = new Rect(0, 0, 0, 0);
            foreach (object i in nodeWindowPositions.nodes)
            {
                ConfigNode node = (ConfigNode) i;
                string name = node.name;
                Rect position = node.FromNode(WINDOW_POSITION, defaultPos);

                Log.dbg("Settings#load name: {0} position: {1}", name, position);

                this.windowPositions[name] = position;
            }

            foreach (object n in nodeWindowVisibility.nodes)
            {
                ConfigNode node = (ConfigNode) n;
                string name = node.name;
                bool visible = node.FromNode(WINDOW_VISIBLE, false);

                Log.dbg("Settings#Load name: {0} visible: {1}", name, visible);

                this.windowVisibilities[name] = visible;
            }

            foreach (object n in bottomButtons.nodes)
            {
                ConfigNode node = (ConfigNode) n;
                string name = node.name;
                bool value = node.FromNode(BUTTON_STATE, false);

                Log.dbg("Settings#Load name: {0} value: {1}", name, value);

                this.bottomButtons[name] = value;
            }

            ConfigNode nodeTypeVisibility = config.GetNode(NODE_VESSEL_TYPE_VISIBILITY) ?? new ConfigNode();
            foreach (HSVesselType i in Resources.vesselTypesList)
            {
                i.visible = nodeTypeVisibility.GetBuiltinValue(i.name, true);
            }

        }

        public readonly GenericIndexer<Rect> WindowPositions;
        public readonly GenericIndexer<bool> WindowVisibilities;
        public readonly GenericIndexer<bool> BottomButtons;


        public class GenericIndexer<V>
        {
            private Dictionary<string, V> index;
            private Func<V> defaultValueFactory;
            private string setDebugMessage;

            public GenericIndexer(Dictionary<string, V> index, Func<V> defaultValueFactory, string setDebugMessage)
            {
                this.index = index;
                this.defaultValueFactory = defaultValueFactory;
                this.setDebugMessage = setDebugMessage;
            }

            public V this[string name]
            {
                get
                {
                    V ret;
                    return index.TryGetValue(name, out ret) ? ret : defaultValueFactory();
                }

                set
                {
                    Log.dbg(setDebugMessage, name, value);
                    this.index[name] = value;
                }
            }
        }

        public void Save()
        {
            Log.detail("saving settings");

            ConfigNode config = SETTINGS.Node;

            ConfigNode nodeWindowPositions = config.AddNode(NODE_WINDOW_POSITIONS);
            ConfigNode nodeVisibilities = config.AddNode(NODE_WINDOW_VISIBILITIES);
            ConfigNode nodeBottomButtons = config.AddNode(BOTTOM_BUTTONS);

            saveDicValuesToNode(windowPositions, nodeWindowPositions, WINDOW_POSITION,
                (node, position) => node.AddNode(position.ToNode()));

            saveDicValuesToNode(windowVisibilities, nodeVisibilities, WINDOW_VISIBLE,
                (node, visible) => node.AddValue(VALUE, visible));

            saveDicValuesToNode(bottomButtons, nodeBottomButtons, BUTTON_STATE, (node, value) => node.AddValue(VALUE, value));

            ConfigNode nodeVesselTypeVisibility = config.AddNode(NODE_VESSEL_TYPE_VISIBILITY);

            List<HSVesselType> typeList = Resources.vesselTypesList;
            foreach (HSVesselType type in typeList)
            {
                nodeVesselTypeVisibility.AddValue(type.name, type.visible);
            }

			SETTINGS.Save();
        }

        private static void saveDicValuesToNode<V>(Dictionary<string, V> dic, ConfigNode node, string configName,
            Action<ConfigNode, V> saveAction)
        {
            foreach (KeyValuePair<string, V> kv in dic)
            {
                ConfigNode n = node.AddNode(kv.Key);
                ConfigNode saveTo = n.AddNode(configName);
                saveAction(saveTo, kv.Value);
            }
        }
    }

    internal static class NodeSerializers
    {
        private static readonly Dictionary<Type, CFromNode> converters = new Dictionary<Type, CFromNode>();

        private delegate object CFromNode(ConfigNode node);

        static NodeSerializers()
        {
            converters[typeof (Rect)] = RectFromNode;
        }

        public static ConfigNode ToNode(this Rect rect)
        {
            ConfigNode node = new ConfigNode("rect");

            node.AddValue("x", rect.x);
            node.AddValue("y", rect.y);
            node.AddValue("width", rect.width);
            node.AddValue("height", rect.height);

            return node;
        }

        public static T GetBuiltinValue<T>(this ConfigNode node, string name, T defaultValue)
        {
            if (!node.HasValue(name))
            {
                return defaultValue;
            }

            Type type = typeof (T);
            TypeConverter typeConveter = TypeDescriptor.GetConverter(type);

            string strValue = node.GetValue(name);

            return (T) typeConveter.ConvertFromInvariantString(strValue);
        }

        public static T FromNode<T>(this ConfigNode node, T defaultValue) where T : class
        {
            CFromNode method = converters[typeof (T)];

            T value = (T) method.Invoke(node) ?? defaultValue;

            return value;
        }

        public static T FromNode<T>(this ConfigNode node, string name, T defaultValue)
        {
            if (!node.HasNode(name))
            {
                return defaultValue;
            }

            Type t = typeof(T);

            if (t.IsPrimitive)
            {
                return GetBuiltinValue(node.GetNode(name), Settings.VALUE, defaultValue);
            }
            else
            {
                CFromNode method = converters[t];
                return (T) method.Invoke(node.GetNode(name));
            }
        }

        public static object RectFromNode(ConfigNode node)
        {
            ConfigNode n = node.GetNode("rect");
            return new Rect
            {
                x = n.GetBuiltinValue("x", 0f),
                y = n.GetBuiltinValue("y", 0f),
                height = n.GetBuiltinValue("height", 0f),
                width = n.GetBuiltinValue("width", 0f)
            };
        }
    }
}