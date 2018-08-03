# Haystack (Archive)

Plugin for [Kerbal Space Program](http://www.kerbalspaceprogram.com/) which helps to easily switch vessels and targets. Unofficial fork by Lisias.


## In a Hurry

* [Binaries](./Archive)
	* [Latest Release](https://github.com/net-lisias-kspu/HaystackContinued/releases)
* [Source](https://github.com/net-lisias-kspu/HaystackContinued)
* [Change Log](./CHANGE_LOG.md)


## Description

This is a fork from Haystack written by hermes-jr, with additions for compatibility by enamelizer. This fork is to continue its development and maintence for KSP 0.24 and beyond.


### Releases

TODO: add in fourm links

### Building

1. Add references to the Assembly-CSharp and UnityEngine assemblies from the version of KSP you wish to target. (A plugin build targeted to one version may not work on another, even if no code changes are necessary for compatibility.)
2. Build in Release configuration. This will copy the Release DLL to \GameData\HrmHaystack\.
3. Zip and release to the world.

### Debugging Setup

1. Copy a testbed version of Kerbal Space Program to the root directory of the repository in the KSP directory.
2. Copy the Assembly-CSharp.dll and the UnityEngine.dll files for the KSP version you are complining against in the References directory.
2. Deploy Haystack to this testbed version
3. Build in Debug configuration, this will copy the Debug dll to the testbed version
4. The testbed now contains the Debug version of Haystack, run the KSP.exe from this testbed to debug Haystack

Bonus: Add the testbed version of KSP.exe to "External Tools" in VS, then you can add this command to a tool bar to launch the testbed version of KSP.exe. You are now two clicks away from a build, deploy, and debug in VS! 

### Reporting Issues

If you wish to test and report errors with this plugin please make sure you have done the following:

1. Make sure you are running this in a copy of KSP with as few other plugins as possible. Preferably with only this plugin and any required plugins installed.
2. Make sure you can reproduce the bug / error condition. If not make sure to record as much detail as possible about what caused the bug / error condition to occur.
3. Provide access to a copy of the output_log.txt located in your KSP_Install_Folder\KSP_Data or KSP_x64_Data if running in 64bit.
4. If running in 64bit on Windows run this plugin in 32bit mode before reporting any errors. 64bit KSP still has many issues and bugs
5. Feel free to leave a fourm post or create an issue here. PMs on the fourm will not generally be answered.
6. Please keep in mind that a bug / error is not the same as a feature request. 


### Licence
The source code is licensed under MIT Licence.

All other materials is licensed under a [Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License](http://creativecommons.org/licenses/by-nc-sa/4.0/).


## UPSTREAM

* [linuxgurugamer](https://forum.kerbalspaceprogram.com/index.php?/profile/129964-linuxgurugamer/) CURRENT MAINTAINER
	* [Forum](https://forum.kerbalspaceprogram.com/index.php?/topic/170111-141-haystack-recontinued/)
	* [SpaceDock](https://spacedock.info/mod/1680/Haystack%20ReContinued?ga=%253CGame+3102+%2527Kerbal+Space+Program%2527%253E)
	* [GitHub](https://github.com/linuxgurugamer/HaystackContinued)
* [Qberticus](https://forum.kerbalspaceprogram.com/index.php?/profile/115166-qberticus/) PARENT
	* [Forum](https://forum.kerbalspaceprogram.com/index.php?/topic/81114-12-2016-10-28-haystack-continued-v0521/)
	* [SpaceDock](https://spacedock.info/mod/547/Haystack%20Continued)
	* [GitHub](https://github.com/qberticus/HaystackContinued)
* [enamelizer](https://forum.kerbalspaceprogram.com/index.php?/profile/90944-enamelizer/) GRAND PARENT
	* [Forum](https://forum.kerbalspaceprogram.com/index.php?/topic/51275-022-haystack-is-back-v0022/)
	* [GitHub](https://github.com/aarondemarre/KSP-Haystack-Plugin)
* [hermes-jr](https://github.com/hermes-jr) ROOT
	* Found no further data.

