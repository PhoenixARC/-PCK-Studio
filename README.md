# PCK Studio
Modify .PCK archives as you please.<br>

> **If you have any issues or would like to suggested a feature, we encourage you to [open a new issue on Github](https://github.com/PhoenixARC/-PCK-Studio/issues).**

## Features:
* Open, edit and save .PCK archives (including models, animations, entire custom packs, etc.)
* Add, replace or remove skins, audios, textures, animations and much more.
* Add translations to make your mods support every language supported by minecraft itself!
* Image previewing

## Supported File formats

| Name | Extention |
|:-:|:-:|
| Localization files | **.loc** |
| Game Rule files | **.grh/.grf** |
| Music Cues file | **audio.pck** |
| Color/Colour Table file | **.col** |

### Known Issues
 - `.resx been flagged by windows(when downloading source.zip)`

### Setup:
```shell
$ git clone --recursive https://github.com/PhoenixARC/-PCK-Studio.git PCK-Studio
$ cd PCK-Studio
```

## How to Build:

* Run if windows flags .resx files
    ```powershell
    $ dir -Path .\ -Recurse | Unblock-File
    ```
- ### Building using [Visual Studio 2022 (or later)](https://visualstudio.microsoft.com/downloads)
    * Open [PCK_Studio.sln](./PCK_Studio.sln) in Visual Studio
    * Click `Build → Build Solution`

- ### Building using [MSBuild](https://github.com/dotnet/msbuild/releases) and [Nuget](https://www.nuget.org/downloads)
    ```shell
    $ nuget restore PCK_Studio.sln
    $ msbuild PCK_Studio.sln -property:Configuration=Release
    ```

### A quick note:
* Forms will not load in the designer until the solution is build _at least_ once.

## Dependencies
  > [OMI-Filetype-Library](https://github.com/PhoenixARC/-OMI-Filetype-Library) for Minecraft filetype handling.<br>
  > [CrEaTiiOn-Brotherhood-Official-C-Theme](https://github.com/EternalModz/CrEaTiiOn-Brotherhood-Official-C-Theme) for better User Interface.<br>
  > [MechanikaDesign.WinForms.UI.ColorPicker](https://www.mechanikadesign.com/software/colorpicker-controls-for-windows-forms/) for better color picking.<br>
 
## Active Developers:
>  [PhoenixARC](https://github.com/PhoenixARC)<br>
>  [MattNL](https://github.com/MattN-L)<br>
>  [Miku-666](https://github.com/NessieHax)<br>
>  [EternalModz](https://github.com/EternalModz)<br>

## Previous Developers:
>  [Nobledez](https://github.com/Nobledez)<br>
>  [jam1garner](https://github.com/jam1garner)<br>
>  SlothWiiPlaza<br>

## Contributors
>  [yaboiFoxx](https://github.com/yaboiFoxx) for Improved UI concept<br>
>  [XxModZxXWiiPlaza](https://github.com/XxModZxXWiiPlaza)<br>