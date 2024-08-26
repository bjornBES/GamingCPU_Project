# BCG File System

- [BCG File System](#bcg-file-system)
- [File system](#file-system)
  - [Send instruction to a disk](#send-instruction-to-a-disk)
    - [Disk registers](#disk-registers)
      - [Floppy disks](#floppy-disks)
      - [instruction](#instruction)
  - [Header Format Info](#header-format-info)
- [Disk Sizes](#disk-sizes)
- [Entries](#entries)
  - [Entry Format](#entry-format)
    - [flags](#flags)
  - [Disk Errors](#disk-errors)

# File system

## Send instruction to a disk

### Disk registers

#### Floppy disks

|Offset |   |Register                         |Register Name Short
|-------|---|---------------------------------|-
|`0x100`| R |Status Register A                |SRA
|`0x101`| R |Status Register B                |SRB
|`0x102`|R/W|Digital Output Register          |DOR
|`0x103`|R/W|Tape Drive Register              |TDR
|`0x104`| R |Main Status Register             |MSR
|`0x104`| W |Data Rate Select Register        |DSR
|`0x105`|R/W|DATA (FIFO)                      |FIFO
|`0x107`| R |Digital Input Register           |DIR
|`0x107`| W |Configuration Control Register   |CCR

#### instruction

|instruction Code |instruction Name         |Description
|-----------------|-------------------------|-
|0x02             |READ_TRACK               |This is used to read all sectors on a track in one command.
|0x03             |SPECIFY                  |Sets up timing parameters like the data rate and the head step rate.
|0x04             |SENSE_DRIVE_STATUS       |Provides information like whether the drive is ready, if a disk is present, and if the disk is write-protected.
|0x05             |WRITE_DATA               |The controller positions the read/write head over the correct track, writing the data from the host to the specified sector on the disk.
|0x06             |READ_DATA                |The controller positions the read/write head over the correct track, finds the requested sector, and transfers the data to the host.
|0x07             |RECALIBRATE              |Moves the head to track 0
|0x08             |SENSE_INTERRUPT          |Retrieves the status of the controller following an interrupt.
|0x09             |WRITE_DELETED_DATA       |Similar to the Write Data command, but it writes the data with a "deleted data mark," which signifies the sector should not be used for normal data storage.
|0x0A             |READ_ID                  |This command helps to verify the physical location of the sector on the disk by reading the ID field that contains information like the cylinder number, head number, sector number, and sector size.
|0x0C             |READ_DELETED_DATA        |This is used to recover or check data that was previously marked as deleted.
|0x0D             |FORMAT_TRACK             |This prepares a track on the floppy disk for data storage by writing sector markers and ID fields.
|0x0E             |DUMPREG                  |Dumps the contents of the controller's internal registers.
|0x0F             |SEEK                     |Moves the read/write head to a specified cylinder (track).
|0x10             |VERSION                  |Reads the version number of the controller.
|0x11             |SCAN_EQUAL               |Scans the data on the disk and checks if it matches a specific value.
|0x12             |PERPENDICULAR_MODE       |Configures the drive for perpendicular recording.
|0x13             |CONFIGURE                |This command allows the host to configure settings such as the precompensation delay, FIFO threshold, and other internal parameters to optimize performance for specific drives and media types.
|0x14             |LOCK                     |When locked, the current configuration settings cannot be changed. This is useful to prevent accidental reconfiguration during operation.
|0x16             |VERIFY                   |Verifies the data on a specified sector without transferring the data to the host.
|0x19             |SCAN_LOW_OR_EQUAL        |Scans the data on the disk and checks if it is less than or equal to a specific value.

## Header Format Info

|offset |size in bytes  |type     |name                                     |inline   |Description
|-------|---------------|---------|-----------------------------------------|---------|-
|`0x000`|   `0x200`     |byte[]   |Boot sector                              |         |yes the boot sector
|`0x200`|   `0x005`     |string   |header version                           |  true   |here is where the version of the file system is at
|`0x205`|   `0x001`     |char     |disk letter                              |  true   |the first disk always has the ``C`` letter
|`0x206`|   `0x001`     |byte     |Number of total heads                    |  false  |The number of total heads
|`0x207`|   `0x002`     |ushort   |Number of total tracks                   |  false  |The number of total tracks
|`0x209`|   `0x002`     |ushort   |Number of total sectors                  |  false  |The number of total sectors
|`0x20B`|   `0x002`     |ushort   |Number of sectors per track              |  false  |The number of sectors per track
|`0x20D`|   `0x002`     |ushort   |Bytes per sector                         |  false  |The number of bytes in a sector
|`0x20F`|   `0x004`     |string   |root directory                           |  true   |the address of the root directory
|`0x213`|   `0x001`     |byte     |Write enable                             |  false  |here you can see if the disk can be written to
|`0x214`|   `0x001`     |byte     |Maximum number of root directory entries |  false  |here is the maximum number of root directory entries
|`0x215`|   `0x001`     |byte     |Allocated FAT                            |  false  |here is the allocated FAT in pagese
|`0x216`|   `0x002`     |ushort   |root directory Entry Conut               |  false  |root directory Entry Conut
|`0x218`|   `0x0E8`     |string   |metadata                                 |  true   |some more metadata
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

|offset|size in bytes  |type    |inline  |name
|------|---------------|--------|--------|-
|`0x00`|     `0x09`    |string  |  true  |entry name
|`0x09`|     `0x01`    |char    |  true  |zero for the entry name
|`0x0A`|     `0x02`    |ushort  |  false |Starting Sector
|`0x0C`|     `0x02`    |ushort  |  false |File size in Sectors
|`0x0E`|     `0x02`    |ushort  |  false |[flags](#flags)
|`0x10`|     `0x01`    |byte    |  false |Entry count (only if directory)
|`0x11`|     `0x01`    |byte    |  false |Day
|`0x12`|     `0x01`    |byte    |  false |Month
|`0x13`|     `0x02`    |ushort  |  false |Year
|`0x15`|     `0x01`    |byte    |  false |Hours
|`0x16`|     `0x01`    |byte    |  false |Minutes
|`0x17`|     `0x02`    |ushort  |  false |Continue Address
|`0x19`|     `0x07`    |byte[]  |  true  |unused

inline: ``if something is inline that means that it uses ascii chars to make its value if some is not inline that means that the value is in the cell itself``

### flags

|Bit      |Name         |Description
|---------|-------------|-
|``Bit 0``|Is directory |says if the entry is a directory or a file
|``Bit 1``|Is Hidden    |says if the entry is hidden when shown the entries (applied in software)
|``Bit 2``|Is Protected |says if the entry is protected meaning the file will not be able to be written to (applied in software)
|``Bit 4``|Is Readonly  |says if the entry is readonly

## Disk Errors

- 0x00: there was no errors in the last disk operation
- 0x01: the disk is connected
