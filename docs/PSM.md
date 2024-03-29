# Pck Skin Model / PSM - v1

<h3> A technical documentation about the custom format PCK-Studio uses to save custom skin data.</h3>

| Name | Size<br>(in bytes) | Description |
|:-:|:-:|:-:|
| Version | 1 | Describes the version of the file format. |
| Anim | 4 | Animation flags for the custom skin. |
| Box Count | 4 | The amount of [box data](#box-data) that has to be read. |
| [Box Data](#box-data)[] | Box Count | |
| Offset Count | 4 | The amount of [offset data](#offset-data) that has to be read. |
| [Offset Data](#offset-data)[] | Offset Count | |


### Box data
| Name | Size<br>(in bytes) | Description |
|:-:|:--:|:-:|
| Type | 1 | Type of model part ([see PSMParentType](../PCK-Studio/Internal/FileFormats/PSMFile.cs#L83)). |
| Postion | 24 | Position in 3D space (3 Floats). |
| Size | 24 | Size in 3D space (3 Floats). |
| Mirror | 1 (bit) | Specifies if the texture should be mirrored. |
| U | 7 (bits) | U texture coordinate. |
| Hide with armor | 1 (bit) | Specifies if the box should hide when wearing armor. |
| V | 7 (bits) | V texture coordinate. |
| Inflate | 4 | Defines box scale without changing the UV. |


### Offset data
| Name | Size<br>(in bytes) | Description |
|:-:|:--:|:-:|
| Type | 1 | Type of offset ([see PSMOffsetType](../PCK-Studio/Internal/FileFormats/PSMFile.cs#L60)). |
| Value | 4 | The Vertical Offset. |