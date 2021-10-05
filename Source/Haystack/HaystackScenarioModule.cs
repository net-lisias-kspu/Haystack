namespace Haystack
{
    /// <summary>
    /// This class hooks into KSP to save data into the save game file. This is independant on the settings which is global for this install.
    /// </summary>
    public class HaystackScenarioModule : ScenarioModule
    {
        internal static GameScenes[] Scenes = { GameScenes.FLIGHT, GameScenes.TRACKSTATION, GameScenes.SPACECENTER};


        public override void OnSave(ConfigNode node)
        {
            Log.dbg("HaystackScenarioModule#OnSave: {0}", HighLogic.LoadedScene);
            DataManager.Instance.Save(node);
        }

        public override void OnLoad(ConfigNode node)
        {
            Log.dbg("HaystackScenarioModule#OnLoad: {0}", HighLogic.LoadedScene);
            DataManager.Instance.Load(node);
        }
    }
}