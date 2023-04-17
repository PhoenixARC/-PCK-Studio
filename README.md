# PCK Studio
Modify .PCK archives as you please!<br>
**If you have any issues or would like to suggested a feature, we encourage you to open a new [issue](https://github.com/PhoenixARC/-PCK-Studio/issues) on the github.**

## Features:
* Open, Edit and Save .PCK archives (this means models, animations, entire custom packs, etc.)
* Add / Remove / Replace skins, audios, textures, animations and much more.
* Edit localization data to make your mods support every language supported by minecraft itself!
* PNG previewing
* And much more!\

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
$ git clone --recursive https://github.com/PhoenixARC/-PCK-Studio.git PCK-Studio
$ cd PCK-Studio
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

## Active Developers:
>  [PhoenixARC](https://github.com/PhoenixARC)<br>
>  [MattNL](https://github.com/MattN-L)<br>
>  [Miku-666](https://github.com/NessieHax)<br>
>  [EternalModz](https://github.com/EternalModz)<br>

## Previous Developers and Contributors:
>  [Nobledez](https://github.com/Nobledez)<br>
>  [jam1garner](https://github.com/jam1garner)<br>
>  [XxModZxXWiiPlaza](https://github.com/XxModZxXWiiPlaza)<br>
>  [SlothWiiPlaza](https://github.com/Kashiiera)<br>

## Other Credits
*  [yaboiFoxx](https://github.com/yaboiFoxx) for Improved UI concept
