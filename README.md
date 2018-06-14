# Spotify Controller

A software that controls tasks of Spotify with gestures and voice.

## Dependencies

First of all, install Microsoft Speech Platform's dependencies: [SDK](https://www.microsoft.com/en-us/download/details.aspx?id=27226), [Runtime](http://www.microsoft.com/en-us/download/details.aspx?id=27225) and [Languages](http://www.microsoft.com/en-us/download/details.aspx?id=27224), where you've to download MSSpeech_SR_en-US_TELE.msi and MSSpeech_TTS_en-US_ZiraPro.msi. <br>
Then, download and install [Kinect SDK](https://www.microsoft.com/en-us/download/details.aspx?id=44561) to enable the Kinect device in your computer. <br>
Finally, install [SpotifyAPI-NET](https://github.com/JohnnyCrazy/SpotifyAPI-NET) and Microsoft Kinect dependencies. 
You can install the packages from NuGet Package Manager Console of Visual Studio's Tools bar, by using the following commands:

```
Install-Package Microsoft.Kinect
Install-Package Microsoft.Kinect.VisualGestureBuilder.x64 -Version 2.0.1410.19000
Install-Package SpotifyAPI-NET -Version 2.19.0
```
After that, go to Tools tab again and choose the option "Manage NuGet Packages for Solution" to check if you have all updates installed.

## How To Run
1. Go to **IM Runtime** folder, open the cmd executable and use the following command:
```
java -jar mmiframeworkV2
```
2. Go to **FusionEngine Runtime** folder, open the cmd executable and use the following command:
```
java -jar FusionEngine
```
3. Open **Spotify**
4. Open and run all the three Visual Studio projects: **AppGui**, **speechModality** and **gestureModality** 

## Login

To enter in Spotify, you've to use the following credentials:

**E-mail:** d1533276@nwytg.com <br>
**Password:** IM20172018
