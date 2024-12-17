# BCG File System

- [BCG File System](#bcg-file-system)
- [File system](#file-system)
  - [Send instruction to a disk](#send-instruction-to-a-disk)
  - [Header Format Info](#header-format-info)
  - [Entries](#entries)
    - [Entry Format](#entry-format)
      - [flags](#flags)

# File system

This format can go up to 50 MB

## Send instruction to a disk

More [here](../CPU/Floppy%20disk%20controller/BFDCG12.md)

## Header Format Info

|offset |size   |type   |name                                     |inline|Description
|-------|-------|-------|-----------------------------------------|-----|-
|`0x000`|`0x200`|byte[] |Boot sector                              |     |yes the boot sector
|`0x200`|`0x006`|char   |disk header                              |true |This is the disk version
|`0x206`|`0x001`|byte   |total heads                              |false|The number of total heads
|`0x207`|`0x002`|ushort |track per head                           |false|The number of track per head
|`0x209`|`0x002`|ushort |total sectors                            |false|The number of total sectors
|`0x20B`|`0x001`|byte   |sectors per track                        |false|The number of sectors per track
|`0x20C`|`0x002`|ushort |Sectors per page                         |false|The number of sectors per page
|`0x20E`|`0x002`|ushort |Bytes per sector                         |false|The number of bytes in a sector
|`0x210`|`0x002`|ushort |root directory sector                    |false|the sector of the root directory
|`0x212`|`0x002`|ushort |FAT sector                               |false|the sector of the FAT
|`0x214`|`0x002`|ushort |Maximum number of root directory entries |false|here is the maximum number of root directory entries
|`0x216`|`0x001`|byte   |Allocated FAT                            |false|here is the allocated FAT in sectors
|`0x217`|`0x001`|byte   |Name length                              |false|this is the total length of the name and
|`0x400`|`0x200`|entry[]|entries                                  |     |more info in the [entries Formatting](#entry-format)

inline: ``if something is inline that means that it uses ascii chars to make its value if some is not inline that means that the value is in the cell itself``

## Entries

### Entry Format

|offset|size in bytes |type   |inline  |name
|------|--------------|-------|-------|-
|`0x00`|    `0x0B`    |string | true  |File name
|`0x0B`|    `0x05`    |string | true  |File type
|`0x10`|    `0x02`    |ushort | false |Starting Sector
|`0x12`|    `0x02`    |ushort | false |size in Sectors
|`0x14`|    `0x01`    |byte   | false |[flags](#flags)
|`0x15`|    `0x02`    |ushort | false |Create Date
|`0x17`|    `0x02`    |ushort | false |Create Time
|`0x19`|    `0x01`    |byte   | false |Entry Count only for directory
|`0x1A`|    `0x06`    |byte[] | true  |unused

inline: ``if something is inline that means that it uses ascii chars to make its value if some is not inline that means that the value is in the cell itself``

#### flags

|Bit   |Name         |Description
|------|-------------|-
|`0x01`|Is directory |says if the entry is a directory or a file
|`0x02`|Is Hidden    |says if the entry is hidden when shown the entries (applied in software)
|`0x04`|Is Protected |says if the entry is protected meaning the file will not be able to be written to (applied in software)
|`0x08`|Is Readonly  |says if the entry is readonly
