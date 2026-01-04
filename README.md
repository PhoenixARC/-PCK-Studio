# PCK Studio ![GitHub All Releases](https://img.shields.io/github/downloads/PhoenixARC/-PCK-Studio/total?label=Total%20Downloads&logo=github) ![GitHub Stars](https://img.shields.io/github/stars/PhoenixARC/-PCK-Studio?label=Stars&logo=github) ![GitHub Forks](https://img.shields.io/github/forks/PhoenixARC/-PCK-Studio?label=Forks&logo=github) ![GitHub Issues](https://img.shields.io/github/issues/PhoenixARC/-PCK-Studio?label=Open%20Issues&logo=github) ![GitHub Pull Requests](https://img.shields.io/github/issues-pr/PhoenixARC/-PCK-Studio?label=Pull%20Requests&logo=github)  
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

## Active Developers:
>  [PhoenixARC](https://github.com/PhoenixARC)<br>
>  [MayNL](https://github.com/MayN-L)<br>
>  [Miku-666](https://github.com/NessieHax)<br>
>  [EternalModz](https://github.com/EternalModz)<br>

## Previous Developers:
>  [Nobledez](https://github.com/Nobledez)<br>
>  [jam1garner](https://github.com/jam1garner)<br>
>  SlothWiiPlaza<br>

## Contributors
>  [yaboiFoxx](https://github.com/yaboiFoxx) for Improved UI concept<br>
>  [XxModZxXWiiPlaza](https://github.com/XxModZxXWiiPlaza)<br>
