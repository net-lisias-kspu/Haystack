/*
	This file is part of Haystack /L Unleashed
		© 2018-2021 LisiasT
		© 2018 linuxgurugamer
		© 2016-2018 Qberticus
		© 2013-2016 hermes-jr, enamelizer

	THIS FILE is licensed to you under:

	* WTFPL - http://www.wtfpl.net
		* Everyone is permitted to copy and distribute verbatim or modified
			copies of this license document, and changing it is allowed as long
			as the name is changed.

	THIS FILE is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
*/
using UnityEngine;

using Logger = KSPe.Util.Log.Logger;
using System.Diagnostics;

namespace Haystack
{
    public static class Log
    {
        private static readonly Logger LOG = Logger.CreateForType<Startup>();

        public static int debuglevel {
            get => (int)LOG.level;
            set => LOG.level = (KSPe.Util.Log.Level)(value % 6);
        }

        public static void force(string format, params object[] @parms)
        {
            LOG.force(format, parms);
        }

        public static void detail(string format, params object[] @parms)
        {
            LOG.detail(format, parms);
        }

        public static void info(string format, params object[] @parms)
        {
            LOG.info(format, parms);
        }

        public static void warn(string format, params object[] @parms)
        {
            LOG.warn(format, parms);
        }

        public static void err(string format, params object[] parms)
        {
            LOG.error(format, parms);
        }

        public static void ex(MonoBehaviour offended, System.Exception e)
        {
            LOG.error(offended, e);
        }

        public static void ex(object offended, System.Exception e)
        {
            LOG.error(offended, e);
        }

        [Conditional("DEBUG")]
        public static void dbg(string format, params object[] @parms)
        {
            LOG.dbg(format, parms);
        }
    }
}
