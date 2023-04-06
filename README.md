# PCK Studio Melon Build
Melon Build: A Revamp of PCK Studio!

## Melon Build:
This is a revamp of PCK Studio! With a whole new UI and much more new features, you can make .pck files in a much cooler fashon! 

## Supported File formats

| Name | Extention |
|:-:|:-:|
| Localization files | **.loc** |
| Game Rule files | **.grh/.grf** |
| Music Cues file |**audio.pck** |
| Color/Colour Table file | **.col** |

## Build Creators:
* [yaboiFoxx](https://github.com/yaboiFoxx) Main Developer of the build

## Original Creators:
*  [PhoenixARC](https://github.com/PhoenixARC)
*  [MattNL](https://github.com/MattN-L)
*  [Miku-666](https://github.com/NessieHax)
*  [EternalModz](https://github.com/EternalModz)
*  [Nobledez](https://github.com/Nobledez)
*  [XxModZxXWiiPlaza](https://github.com/XxModZxXWiiPlaza)
*  [SlothWiiPlaza](https://github.com/Kashiiera)
*  [jam1garner](https://github.com/jam1garner)


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

### Other Notes:

* Forms will not load in viewer until the solution is build _at least_ once
* This is not my PROGRAM! Ive only made the build!


