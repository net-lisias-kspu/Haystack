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