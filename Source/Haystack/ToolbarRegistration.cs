using UnityEngine;
using ToolbarControl_NS;

namespace Haystack
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class RegisterToolbar : MonoBehaviour
    {
        void Start()
        {
            ToolbarControl.RegisterMod(HaystackResourceLoader.MODID, HaystackResourceLoader.MODNAME);
        }
    }
}