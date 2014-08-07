using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using KSP.IO;
using UnityEngine;

namespace HaystackContinued
{
	public class Settings
	{
		public static string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

	    private static readonly string SettingsFile = Resources.PathPlugin + Path.DirectorySeparatorChar + "settings.cfg";

        

	    private const string NodeSettings = "settings";
	    private const string NodeWindowPositions = "window_positions";
	    private const string NodeVesselTypeVisibility = "type_visibility";
	    private const string WindowPosition = "position";
	    private const string Visible = "visible";

	    private readonly Dictionary<string, Rect> windowPositions = new Dictionary<string, Rect>();

	    public Settings()
	    {
            this.WindowPositions = new WindowPositionsIndexer(this.windowPositions);
	        
            Load();
	    }

		private void Load()
		{
            HSUtils.Log("loading settings");
            HSUtils.DebugLog("Settings#Load: start");

		    var load = ConfigNode.Load(SettingsFile) ?? new ConfigNode();

		    if (!load.HasNode(NodeSettings))
		    {
                HSUtils.DebugLog("Settings#Load: no settings node");
		        return;
		    }

		    var config = load.GetNode(NodeSettings);

		    var nodeWindowPositions = config.GetNode(NodeWindowPositions) ?? new ConfigNode();

            var defaultPos = new Rect(0, 0, 0, 0);
		    foreach (var i in nodeWindowPositions.nodes)
		    {
		        var node = (ConfigNode) i;
		        var name = node.name;
		        var position = node.FromNode(WindowPosition, defaultPos);
		        
                HSUtils.DebugLog("Settings#load name: {0} position: {1}", name, position);

		        this.windowPositions[name] = position;
		    }

		    var nodeTypeVisibility = config.GetNode(NodeVesselTypeVisibility) ?? new ConfigNode();
		    foreach (var i in Resources.vesselTypesList)
		    {
		        i.visible = nodeTypeVisibility.GetBuiltinValue(i.name, true);
		    }
		}

	    public class WindowPositionsIndexer
	    {
	        private Dictionary<string, Rect> windowPositions;

	        public WindowPositionsIndexer(Dictionary<string, Rect> windowPositions)
	        {
	            this.windowPositions = windowPositions;
	        }

	        public Rect this[string name]
	        {
	            get
	            {
	                Rect outRect;
	                return this.windowPositions.TryGetValue(name, out outRect) ? outRect : new Rect(0, 0, 0, 0);
	            }
	            set
	            {
	                HSUtils.DebugLog("settings: window WindowPosition: {0} {1}", name, value);
                    this.windowPositions[name] = value;
	            }
	        }
	    }

	    public readonly WindowPositionsIndexer WindowPositions;

	    public void Save()
		{
			HSUtils.Log("saving settings");

	        var t = new ConfigNode();
	        var config = t.AddNode(NodeSettings);

	        var nodeWindows = config.AddNode(NodeWindowPositions);
	        foreach (var kv in windowPositions)
	        {
	            var name = kv.Key;
	            var position = kv.Value;

	            var node = nodeWindows.AddNode(name);
	            node.AddNode(WindowPosition).AddNode(position.ToNode());
	        }

	        var nodeVesselTypeVisibility = config.AddNode(NodeVesselTypeVisibility);

	        var typeList = Resources.vesselTypesList;
	        foreach (var type in typeList)
	        {
	            nodeVesselTypeVisibility.AddValue(type.name, type.visible);
	        }

            t.Save(SettingsFile);
		}
	}

    static class NodeSerializers
    {
        private static readonly Dictionary<Type, CFromNode> converters = new Dictionary<Type, CFromNode>();

        private delegate object CFromNode(ConfigNode node);

        static NodeSerializers()
        {
            converters[typeof(Rect)] = RectFromNode;
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

        public static T FromNode<T>(this ConfigNode node, T defaultValue) where T: class
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

            var method = converters[typeof (T)];
            return (T)method.Invoke(node.GetNode(name));
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
