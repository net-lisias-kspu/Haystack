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
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;


namespace Haystack
{
    // http://forum.kerbalspaceprogram.com/index.php?/topic/147576-modders-notes-for-ksp-12/#comment-2754813
    // search for "Mod integration into Stock Settings

    public class HS : GameParameters.CustomParameterNode
    {
        public override string Title { get { return "Haystack /L"; } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "Haystack"; } }
        public override string DisplaySection { get { return "Haystack"; } }
        public override int SectionOrder { get { return 3; } }
        public override bool HasPresets { get { return false; } }


        //[GameParameters.CustomParameterUI("Use Blizzy Toolbar", toolTip = "If available (will require scene change to activate)")]
        //public bool useBlizzy = false;
        [GameParameters.CustomParameterUI("Use Alternate Skin")]
        public bool useAltSkin = false;




        public override void SetDifficultyPreset(GameParameters.Preset preset)
        {
        }

        public override bool Enabled(MemberInfo member, GameParameters parameters)
        {

            return true;
        }


        public override bool Interactible(MemberInfo member, GameParameters parameters)
        {

            return true;
        }

        public override IList ValidValues(MemberInfo member)
        {
            return null;
        }
    }
}
