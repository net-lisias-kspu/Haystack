using UnityEngine;
using ToolbarControl_NS;

namespace HaystackReContinued
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