# AutoRaid
A RAID: Shadow Legends Toolkit to automate various tasks.

AutoRaid is a c# 8.0 PRISM application that makes heavy use of ADB Debug Bridge. Instead of sending key or mouse input directly to the 
Emulator-Window, AutoRaid reads the UI for common states and emulates touch gestures. 

A big advantage of using ADB instead of sending Input events directly to the window, the target window (BlueStacks Emulator) does not need any window focus. Meaning you can do other stuff on your PC without focus hoping.

**WORK IN PROGRESS**

![Image of AutoRaid UI](https://github.com/nschoenberg/AutoRaid/blob/master/autoraid.png?raw=true)

## How  does it work
UI is captured via Screenshot and further analyzed using simple pattern matching. 
 * Take Screenshot
 * Crop out different screen areas (For example pause button, see picture above)
 * Downsize picture to 16x16 monochrom
 * Calculate hash, 16x16 = 256 pixels. Eeach black pixel gets a '1', each white pixel gets a '0'
 
The calculated hash will be used to determine if a specific UI element is currently visible.

## Requirements
* Android SDK installed, Environment Variable %ANDROID_SDK_ROOT% configured
* BlueStacks Emulator, using Open GLEngine with Display Settings 1600x900 on 240 DPI
* In BlueStacks Settings Menu / Preferences, Enable Android Debug Bridge (ADB)
