# PCK Studio
Modify .PCK archives as you please!

## Features:
#### If you'd like to suggested a feature to be added open an [issue](https://github.com/PhoenixARC/-PCK-Studio/issues) on the github.
* Open, Edit and Save .PCK archives (this means models, animations, entire custom packs, etc.)
* Add / Remove / Replace skins, audios, textures, animations and much more.
* Edit localization data to make your mods support every language supported by minecraft itself!
* PNG previewing
* And much more!

## Supported File formats

| Name | Extention |
|:-:|:-:|
| Localization files | **.loc** |
| Game Rule files | **.grh/.grf** |
| Music Cues file |**audio.pck** |
| Color/Colour Table file | **.col** |

### Known Issues
 - `.resx been flagged by windows(when downloading source.zip)`

### Setup:
```shell
$ git clone https://github.com/PhoenixARC/-PCK-Studio.git
$ cd "-PCK-Studio"
```

## How to Build:

* Run if windows flags .resx files
    ```powershell
    $ dir -Path .\ -Recurse | Unblock-File
    ```
- ### Building using [Visual Studio 2022 (or later)](https://visualstudio.microsoft.com/downloads)
    * Open [PCK_Studio.sln](./PCK_Studio.sln) in Visual Studio 2022 or later
    * Click `Run` or press `Shift + B`

- ### Building using [MSBuild](https://github.com/dotnet/msbuild/releases) and [Nuget](https://www.nuget.org/downloads)
  * ```shell
    $ nuget restore PCK_Studio.sln
    $ msbuild PCK_Studio.sln -property:Configuration=Release
    ```

### A quick note:

* Forms will not load in viewer until the solution is build _at least_ once

## Dependencies
  > [OMI-Filetype-Library](https://github.com/PhoenixARC/-OMI-Filetype-Library) for Minecraft filetype handling.<br>
  > [CrEaTiiOn-Brotherhood-Official-C-Theme](https://github.com/EternalModz/CrEaTiiOn-Brotherhood-Official-C-Theme) for better User Interface.<br>
  > [MechanikaDesign.WinForms.UI.ColorPicker](https://www.mechanikadesign.com/software/colorpicker-controls-for-windows-forms/) for better color picking.<br>
 

## Active Dev Team:
*  [PhoenixARC](https://github.com/PhoenixARC)
*  [MattNL](https://github.com/MattN-L)
*  [Miku-666](https://github.com/NessieHax)
*  [EternalModz](https://github.com/EternalModz)

## Legacy PCK Studio Devs and contributors:
*  [Nobledez](https://github.com/Nobledez)
*  [XxModZxXWiiPlaza](https://github.com/XxModZxXWiiPlaza)
*  [SlothWiiPlaza](https://github.com/Kashiiera)
*  [jam1garner](https://github.com/jam1garner)

## Other Credits
*  [yaboiFoxx](https://github.com/yaboiFoxx) for Improved UI concept
