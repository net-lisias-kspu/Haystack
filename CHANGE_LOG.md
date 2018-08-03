# Haystack :: Change Log

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
 
