using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UnityEngine;

namespace HaystackReContinued
{
    public class Settings
    {
        public static string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

        private static readonly string PluginDataDir = Resources.PathPlugin + Path.DirectorySeparatorChar + "PluginData";
        private static readonly string SettingsFile = PluginDataDir + Path.DirectorySeparatorChar + "settings.cfg";

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

        public Settings()
        {
            this.WindowPositions = new GenericIndexer<Rect>(windowPositions, () => new Rect(0, 0, 0, 0),
                "settings: window WindowPosition: {0} {1}");
            this.WindowVisibilities = new GenericIndexer<bool>(windowVisibilities, () => false,
                "settings: window WindowVisibility: {0} {1}");
            this.BottomButtons = new GenericIndexer<bool>(bottomButtons, () => false, "settings: bottom buttons: {0} {1}");
            
            this.Convert();
            this.Load();
        }

        private void Convert()
        {
           this.convertToNewDirectory();
        }

        private void convertToNewDirectory()
        {
            var oldSettingsFile = Resources.PathPlugin + Path.DirectorySeparatorChar + "settings.cfg";

            var oldSettingsExists = File.Exists(oldSettingsFile);
            var newSettingsExists = File.Exists(SettingsFile);

            if (!Directory.Exists(PluginDataDir))
            {
                HSUtils.Log("Creating missing PluginData directory.");
                Directory.CreateDirectory(PluginDataDir);
            }

            if (oldSettingsExists && !newSettingsExists)
            {
                HSUtils.Log("Moving settings file to new location.");
                
                File.Move(oldSettingsFile, SettingsFile);
            } else if (oldSettingsExists)
            {   HSUtils.Log("Deleting old settings file.");

                File.Delete(oldSettingsFile);
            }
        }

        private void Load()
        {
            HSUtils.Log("loading settings");
            HSUtils.DebugLog("Settings#Load: start");

            var load = ConfigNode.Load(SettingsFile) ?? new ConfigNode();

            if (!load.HasNode(NODE_SETTINGS))
            {
                HSUtils.DebugLog("Settings#Load: no settings node");
                return;
            }

            var config = load.GetNode(NODE_SETTINGS);

            var nodeWindowPositions = config.GetNode(NODE_WINDOW_POSITIONS) ?? new ConfigNode();

            var nodeWindowVisibility = config.GetNode(NODE_WINDOW_VISIBILITIES) ?? new ConfigNode();

            var bottomButtons = config.GetNode(BOTTOM_BUTTONS) ?? new ConfigNode();

            var defaultPos = new Rect(0, 0, 0, 0);
            foreach (var i in nodeWindowPositions.nodes)
            {
                var node = (ConfigNode) i;
                var name = node.name;
                var position = node.FromNode(WINDOW_POSITION, defaultPos);

                HSUtils.DebugLog("Settings#load name: {0} position: {1}", name, position);

                this.windowPositions[name] = position;
            }

            foreach (var n in nodeWindowVisibility.nodes)
            {
                var node = (ConfigNode) n;
                var name = node.name;
                var visible = node.FromNode(WINDOW_VISIBLE, false);

                HSUtils.DebugLog("Settings#Load name: {0} visible: {1}", name, visible);

                this.windowVisibilities[name] = visible;
            }

            foreach (var n in bottomButtons.nodes)
            {
                var node = (ConfigNode) n;
                var name = node.name;
                var value = node.FromNode(BUTTON_STATE, false);

                HSUtils.DebugLog("Settings#Load name: {0} value: {1}", name, value);

                this.bottomButtons[name] = value;
            }

            var nodeTypeVisibility = config.GetNode(NODE_VESSEL_TYPE_VISIBILITY) ?? new ConfigNode();
            foreach (var i in Resources.vesselTypesList)
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
                    HSUtils.DebugLog(setDebugMessage, name, value);
                    this.index[name] = value;
                }
            }
        }

        public void Save()
        {
            HSUtils.DebugLog("saving settings");

            var t = new ConfigNode();
            var config = t.AddNode(NODE_SETTINGS);

            var nodeWindowPositions = config.AddNode(NODE_WINDOW_POSITIONS);
            var nodeVisibilities = config.AddNode(NODE_WINDOW_VISIBILITIES);
            var nodeBottomButtons = config.AddNode(BOTTOM_BUTTONS);

            saveDicValuesToNode(windowPositions, nodeWindowPositions, WINDOW_POSITION,
                (node, position) => node.AddNode(position.ToNode()));

            saveDicValuesToNode(windowVisibilities, nodeVisibilities, WINDOW_VISIBLE,
                (node, visible) => node.AddValue(VALUE, visible));

            saveDicValuesToNode(bottomButtons, nodeBottomButtons, BUTTON_STATE, (node, value) => node.AddValue(VALUE, value));

            var nodeVesselTypeVisibility = config.AddNode(NODE_VESSEL_TYPE_VISIBILITY);

            var typeList = Resources.vesselTypesList;
            foreach (var type in typeList)
            {
                nodeVesselTypeVisibility.AddValue(type.name, type.visible);
            }

            t.Save(SettingsFile);
        }

        private static void saveDicValuesToNode<V>(Dictionary<string, V> dic, ConfigNode node, string configName,
            Action<ConfigNode, V> saveAction)
        {
            foreach (var kv in dic)
            {
                var n = node.AddNode(kv.Key);
                var saveTo = n.AddNode(configName);
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
            var node = new ConfigNode("rect");

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

            var type = typeof (T);
            var typeConveter = TypeDescriptor.GetConverter(type);

            var strValue = node.GetValue(name);

            return (T) typeConveter.ConvertFromInvariantString(strValue);
        }

        public static T FromNode<T>(this ConfigNode node, T defaultValue) where T : class
        {
            var method = converters[typeof (T)];

            var value = (T) method.Invoke(node) ?? defaultValue;

            return value;
        }

        public static T FromNode<T>(this ConfigNode node, string name, T defaultValue)
        {
            if (!node.HasNode(name))
            {
                return defaultValue;
            }

            var t = typeof(T);

            if (t.IsPrimitive)
            {
                return GetBuiltinValue(node.GetNode(name), Settings.VALUE, defaultValue);
            }
            else
            {
                var method = converters[t];
                return (T) method.Invoke(node.GetNode(name));
            }
        }

        public static object RectFromNode(ConfigNode node)
        {
            var n = node.GetNode("rect");
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