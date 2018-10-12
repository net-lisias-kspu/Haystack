﻿#if false
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;


namespace HaystackReContinued
{
    // http://forum.kerbalspaceprogram.com/index.php?/topic/147576-modders-notes-for-ksp-12/#comment-2754813
    // search for "Mod integration into Stock Settings

    public class HS : GameParameters.CustomParameterNode
    {
        public override string Title { get { return "Haystack ReContinued"; } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "Haystack ReContinued"; } }
        public override string DisplaySection { get { return "Haystack ReContinued"; } }
        public override int SectionOrder { get { return 3; } }
        public override bool HasPresets { get { return false; } }


        [GameParameters.CustomParameterUI("Use Blizzy Toolbar", toolTip = "If available (will require scene change to activate)")]
        public bool useBlizzy = false;




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
#endif