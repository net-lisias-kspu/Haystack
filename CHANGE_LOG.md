# Haystack /L Unleashed :: Change Log

* 2021-1028: 0.5.7.5 (lisias) for KSP >= 1.3.1
	+ Updating code to the latest KSPe (2.4)
	+ Catching up with the Upstream:
		- 0.5.7.3
			- Fixed filtering of new vessel types
		- 0.5.7.2
			- Added new icons:
				- `button_vessel_droppedpart`
				- `button_vessel_deployedgroundpart`
			- Added AssemblyFileVersion
			- Added translation of vessel type with two words combined to two words, english only
			- Updated KSP version to 1.12
		- 0.5.7
			- Added API to support ResourceMonitors
		- 0.5.6.5
			- Added Stock Settings page to control alternate skin use
			- Added back normal stock skin
		- 0.5.6.4
			- Fixed localized vessel names
			- Added API for AlertMonitors to access last selected vessel & open/move the window
		- 0.5.6.1
			- Added red X, upperleft to close window
		- 0.5.6
			- Added icons for two new vessel types:
				- `Vesseltypes.DeployedScienceController`
				- `VesselTypes.DeployedSciencePart`
			- Added code to add spaces to the text names for the new vessel types
* 2021-1005: 0.5.7.4 (lisias) for KSP >= 1.4.1
	+ ***DITCHED***
* 2018-0803: 0.5.4.4 (lisias) for KSP 1.4.1
	+ Moving settings data file to <KSP_ROOT>/PluginsData . 
* 2018-0428: 0.5.4.3 (linuxgurugamer) for KSP 1.4.1
	+ Added ToolbarRegistration
	+ Removed settings page
	+ Updated version info
* 2018-0327: 0.5.4.2 (linuxgurugamer) for KSP 1.4.1
	+ Updated version file 
* 2018-0326: 0.5.4.1 (linuxgurugamer) for KSP 1.4.1
	+ Changed method of loading images to use the 	+ ToolbarControl.LoadImageFromFile
	+ Moved icon folder into PluginData to avoid useless error messages from Unity
* 2018-0318: 0.5.4 (linuxgurugamer) for KSP 1.4.1
	+ Updated for 1.4.1
	+ Added ClickThroughBlocker support
	+ Added part count to display
	+ Added delete button
* 2018-0126: 0.5.3 (linuxgurugamer) for KSP 1.3.1
	+ Adoption by LinuxGuruGamer
	+ Replaced toolbar code with ToolbarController
	+ Changed resource loading from MainMenu to SpaceCentre
	+ DLL moved into Plugins folder
	+ Updated name and namespace names
* 2016-1028: 0.5.2.1 (qberticus) for KSP 1.2
	+ Updated to be compatible with new Contract Configurator changes.
	+ Celestial bodies filter button state is now saved like the other filter buttons.
	+ Vessel type icons for plane and relay have been added.
* 2016-1026: 0.5.2.0 (qberticus) for KSP 1.2
	+ Updated for KSP 1.2
	+ A couple of UI updates due to changes in new KSP version
	+ Tweaked docking port list font size due to new font rendering code
	+ Known issues: missing filter button icons for new ship types: plane and relay
* 2016-0621: 0.5.1.0 (qberticus) for KSP 1.1.3
	+ Updated for KSP 1.1.3
	+ Made the UI look a bit nicer.
	+ Added extended vessel info window. It can be accessed by the arrow button on the right side of the window or by clicking the currently selected vessel as a toggle.
	+ Extended vessel info window also works for Celestial Bodies (i.e., planets and moons). It will display important information about them. Of note are the science High, and Low altitude values. 	+ You don't have to look them up anymore.
	+ Added sort order buttons for the vessel list. These let you change the sort order of the vessel list.
	+ Added a nearby button. This only displays the vessels in the current SOI. Vessels in physics range are sorted by name; vessels outside of physics range are sorted by distance from the active vessel. This button only works while flying a vessel.
	+ Added ability to rename a vessel. The button is accessed from the extended vessel info display.
	+ Search results are ordered by the location of the search term in the result. Closer to the beginning of the result the higher on the list.
	+ Added blizzy toolbar support back. Defaults to using blizzy toolbar if found, otherwise it will use the stock toolbar.
	+ Moved the settings.cfg file to PluginData directory. Thanks to Enceos and RealKolago for the heads up.
	+ Fixed tooltips so they are clamped to within the window and are not cut off.
	+ Added KSP-AVC support.
* 2016-0428: 0.5.0.0 (qberticus) for KSP 1.1.2
	+ Updated to KSP 1.1
	+ Added window to the space center
	+ Added app launcher button and icon
	+ Disabled use of blizzy toolbar until that is updated.
	+ Switched to allowing targeting of docking ports when a vessel is loaded instead of packed. Once the targeted vessel is within 200m (i.e., unpacked) the port needs to be retargeted for automated docking support to work (e.g., mechjeb)
	+ Updated support for Docking Port Alignment Indicator mod named docking port feature
* 2016-0404: 0.4.9.0 (qberticus) for KSP 1.1.0 PRE-RELEASE
	+ Updated to KSP 1.1
		+ This is a development release for KSP 1.1 experimental.
	+ Added window to the space center
	+ Added app launcher button and icon
	+ Disabled use of blizzy toolbar until that is updated.
* 2015-0428: 0.4.1.0 (qberticus) for KSP 0.90
	+ Fixed the missing spaceobject icon error message by adding the icon.
	+ The haystack window will now save the visible state depending on the scene. This means that leaving the window open in the tracking station will not leave it open during flight.
	+ You can now target the currently selected celestial body (i.e., planet or moon) when in the default view (i.e., not group by).
* 2015-0117: 0.4.0.0 (qberticus) for KSP 0.90
	+ There is a new button that allows you to set vessels to be hidden from the list. The hidden vessels are saved in the save game file and will be saved whenever the game is saved.
	+ The window will now default to hidden when first loading the game. In the future this will be updated so the tracking center window and the in flight window will have separate state for the being shown / hidden.
* 2015-0112: 0.3.3.0 (qberticus) for KSP 0.90
	+ This release updates the comple against KSP 0.90
	+ Added support for blizzy toolbar - this allows the window to be positioned anywhere on the screen when the toolbar is installed
	+ Minor tweaks to some UI elements
	+ Fixed a bug reported by Apollo13 where the names of vessels would wrap to a new line
* 2014-1012: 0.3.2.0 (qberticus) for KSP 0.25
	+ Compiled and checked against KSP 0.25
	+ Changed save log output to debug mode only
	+ Haystack window now respects the Hide GUI game event. It's now possible to take screenshots without the Haystack window appearing. 
* 2014-0818: 0.3.1.1 (qberticus) for KSP 0.24.2
	+ Fixed a bug where the display would break as debris left physics range and was removed by the game.
	+ Resizing the window should be smoother in low FPS situations.
* 2014-0811: 0.3.1.0 (qberticus) for KSP 0.24.2
	+ You can now display a list of docking ports for the nearby ships (i.e., loaded in physics range) and target them. The list only displays available docking ports (i.e., the docking port must not have a ship docked and must have a free attach node)
	+ The docking port list includes support for the Docking Port Alignment Indicator mod's named port feature. If it detects this mode it will use the name of the docking port that has been set in the display. If the mod is not detected it will default to using the name of the part.
	+ Ship list now displays the distance that ship is from the active ship.
	+ The Haystack Continued window will now show display in regular flight mode and in map mode
* 2014-0808: 0.3.0.0 (qberticus) for KSP 0.24.2
	+ Vessels can now be grouped by the celestial body that they are orbiting
	+ Window is also provided in the Tracking Center
	+ Window is now resizable to compliment its ability to be dragged
* 2013-1115: 0.2.2.1 (enamelizer) for KSP 0.22
	+ Allow the UI in flight for quick switching
	+ Added celestial bodies as a target type
	+ Added tool tips for the vessel types, along with a count of that type
	+ When in map view, when body or the active vessel is selected, the map focuses on it. This works like Tab, in that it does not seem to cycle thru packed vessels
* 2013-1031: 0.0.2.2 (enamelizer) for KSP 0.22 PRE-RELEASE
	+ Release of Haystack 0.0.2.2 with compatibility for KSP 0.22.
 
