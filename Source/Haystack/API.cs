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

namespace Haystack
{
    public class API
    {
        //public static API APIInstance;
        public static Boolean APIReady = false;
        public static HaystackResourceLoader fetch;

        //Init the API Hooks
        internal static void APIAwake()
        {
            //set up the hookable object

            fetch = HaystackResourceLoader.fetch;

            //set up any events we need from the core code

            //flag it ready
            APIReady = true;
        }

        private void APIDestroy()
        {
            //tear it down
            fetch = null;

            //Tear down any events we need to remove
            //try { 

            //} catch (Exception) { }

            APIReady = false;
        }



        public static Vessel SelectedVessel { get; internal set; }

        public static bool IsVisible()
        {
            if (Haystack.fetch != null)
            {
                return Haystack.fetch.IsVisible();
            }
            return false;
        }

        public static void SetVisibility(VesselType vt, bool visible)
        {
            string vtName = vt.ToString();
            for (int i = 0; i < Resources.vesselTypesList.Count; i++)
                if (Resources.vesselTypesList[i].name == vtName)
                {
                    Resources.vesselTypesList[i].visible = visible;
                }
        }

        public static void SetPosition(float x, float y)
        {
            Haystack.fetch.winRect.x = x;
            Haystack.fetch.winRect.y = y;
        }
    }
}

