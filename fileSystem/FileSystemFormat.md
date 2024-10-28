# BCG File System

- [BCG File System](#bcg-file-system)
- [File system](#file-system)
  - [Send instruction to a disk](#send-instruction-to-a-disk)
    - [Disk registers](#disk-registers)
      - [Floppy disk controller](#floppy-disk-controller)
      - [instruction](#instruction)
  - [Header Format Info](#header-format-info)
- [Disk Sizes](#disk-sizes)
- [Entries](#entries)
  - [Entry Format](#entry-format)
    - [flags](#flags)

# File system

## Send instruction to a disk

### Disk registers

#### Floppy disk controller

|Offset |   |Register
|-------|---|-
|`0x100`| R |Status Register A
|`0x101`| R |Unused
|`0x102`| R |Master Status Register
|`0x103`| W |Data Rate Select Register
|`0x104`|R/W|Data Register
|`0x105`| R |Digital Output Register
|`0x106`| R |Unused
|`0x107`| R |Unused

#### instruction

More [here](../CPU/Floppy%20disk%20controller/BFDCG12.md)

## Header Format Info

|offset |size in bytes  |type     |name                                     |inline   |Description
|-------|---------------|---------|-----------------------------------------|---------|-
|`0x000`|   `0x200`     |byte[]   |Boot sector                              |         |yes the boot sector
|`0x200`|   `0x006`     |char     |disk header                              |  true   |This is the disk version
|`0x206`|   `0x001`     |char     |disk letter                              |  true   |the letter
|`0x207`|   `0x001`     |byte     |Number of total heads                    |  false  |The number of total heads
|`0x208`|   `0x002`     |ushort   |Number of track per head                 |  false  |The number of track per head
|`0x20A`|   `0x002`     |ushort   |Number of total sectors                  |  false  |The number of total sectors
|`0x20C`|   `0x001`     |byte     |Number of sectors per track              |  false  |The number of sectors per track
|`0x20D`|   `0x002`     |ushort   |Bytes per sector                         |  false  |The number of bytes in a sector
|`0x20F`|   `0x002`     |ushort   |root directory                           |  false  |the address of the root directory
|`0x211`|   `0x002`     |ushort   |Maximum number of root directory entries |  false  |here is the maximum number of root directory entries
|`0x213`|   `0x001`     |byte     |Allocated FAT                            |  false  |here is the allocated FAT in sectors
|`0x214`|   `0x010`     |string   |Volume label                             |  true   |Volume label
|`0x224`|   `0x0DC`     |string   |metadata                                 |  true   |some more metadata
|`0x300`|   `0x300`     |entries  |entries                                  |         |more info in the [entries Formatting](#entry-format)

inline: ``if something is inline that means that it uses ascii chars to make its value if some is not inline that means that the value is in the cell itself``

# Disk Sizes

``` text
0x00: 00-20 MB
0x01: 20-40 MB HDD
0x02: 40-80 MB HDD

0x80: 5,4 inch floppy disk at 1.2 MB
0x81: 3,5 inch floppy disk at 1,44 MB
```

# Entries

## Entry Format

|offset|size in bytes |type   |inline  |name
|------|--------------|-------|-------|-
|`0x00`|    `0x0B`    |string | true  |File name
|`0x0B`|    `0x05`    |string | true  |File type
|`0x10`|    `0x02`    |ushort | false |Starting Sector
|`0x12`|    `0x02`    |ushort | false |File size in Sectors
|`0x14`|    `0x01`    |byte   | false |[flags](#flags)
|`0x15`|    `0x02`    |ushort | false |Create Date
|`0x17`|    `0x02`    |ushort | false |Create Time
|`0x19`|    `0x01`    |byte   | false |Entry Count only for directory
|`0x19`|    `0x06`    |byte[] | true  |unused

inline: ``if something is inline that means that it uses ascii chars to make its value if some is not inline that means that the value is in the cell itself``

### flags

|Bit      |Name         |Description
|---------|-------------|-
|``Bit 0``|Is directory |says if the entry is a directory or a file
|``Bit 1``|Is Hidden    |says if the entry is hidden when shown the entries (applied in software)
|``Bit 2``|Is Protected |says if the entry is protected meaning the file will not be able to be written to (applied in software)
|``Bit 4``|Is Readonly  |says if the entry is readonly
