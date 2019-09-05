# LogBox

[![nuget](https://img.shields.io/nuget/v/LogBox.svg)](https://www.nuget.org/packages/LogBox/)
[![GitHub](https://img.shields.io/github/license/M1S2/LogBox)](LICENSE.md)

A WPF control to display log events.

Available types of log events are:

| Type    | Description                                                            | 
| ------- | ---------------------------------------------------------------------- |
| Info    | Use this to show informations.                                         |
| Warning | Use this to show warning.                                              |
| Error   | Use this to show errors.                                               |
| Image   | Use this to display image to the user (for example for debug purpose). |

## Screenshots

![LogBox screenshot](LogBoxTest/Screenshots/Screenshot_LogBox.png)

## Installation
Include the [latest release from nuget.org](https://www.nuget.org/packages/LogBox/) in your project.

You can also use the Package Manager console with: `PM> Install-Package LogBox`

## Usage
Include the following namespace in your .xaml:
```C#
xmlns:logBox="clr-namespace:LogBox;assembly=LogBox"
```

Insert the control with:
```C#
<logBox:LogBoxControl x:Name="logBoxCtrl" EnableImageLogs="True"/>
```

To add log entries use the controls `LogEvent()` method like:
```C#
LogBoxCtrl.LogEvent(new LogBox.LogEvents.LogEventInfo("Info log message"));
```

For further examples see the LogBoxTest project.

## App Icon
Icon made by Flat Icons from www.flaticon.com