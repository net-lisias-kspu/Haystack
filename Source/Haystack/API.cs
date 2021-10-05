﻿using System;

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
            if (HaystackContinued.fetch != null)
            {
                return HaystackContinued.fetch.IsVisible();
            }
            return false;
        }
        public static void ButtonClick()
        {
            if (HaystackResourceLoader.fetch != null)
            {
                if (!HaystackContinued.fetch.WinVisible)
                    HaystackResourceLoader.fetch.appLauncherButton_OnTrue();
                else
                    HaystackResourceLoader.fetch.appLauncherButton_OnFalse();
            }
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
            HaystackContinued.fetch.winRect.x = x;
            HaystackContinued.fetch.winRect.y = y;
        }
    }
}

