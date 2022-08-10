# PCK Studio
Modify .PCK archives as you please!

_`previously known as MinecraftUSkinEditor`_\
`A minecraft for Wii U editor`

## Features:
* Open, Edit and Save .PCK archives (this means models, animations, entire custom packs, etc.)
* Add / Remove / Replace skins, audios, textures, animations and much more.
* Edit localization data to make your mods support every language supported by minecraft itself!
* PNG previewing
* And much more!

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
- ### Building using [Visual Studio](https://visualstudio.microsoft.com/downloads)
    * Open [MinecraftUSkinEditor.sln](./MinecraftUSkinEditor.sln) in Visual Studio 2022 or later
    * Click `Run` or press `Shift + B`

- ### Building using [MSBuild](https://github.com/dotnet/msbuild/releases)
  * ```shell
    $ nuget restore MinecraftUSkinEditor.sln
    $ msbuild MinecraftUSkinEditor.sln -property:Configuration=Release
    ```

### A quick note:

* Forms will not load in viewer until the solution is build _at least_ once


## Contributers:
*  [PhoenixARC](https://github.com/PhoenixARC)
*  [MNL](https://github.com/MattN-L)
*  [Miku-666](https://github.com/NessieHax)
*  [Nobledez](https://github.com/Nobledez)
*  [XxModZxXWiiPlaza](https://github.com/XxModZxXWiiPlaza)
*  [SlothWiiPlaza](https://github.com/Kashiiera)
*  [jam1garner](https://github.com/jam1garner)

### Credits
*  [yaboiFoxx]() for Improved UI concept
