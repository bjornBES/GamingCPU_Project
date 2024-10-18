# FDC Specifications

- Name: **BFDCG** or **BEs Floppy disk controller Gaming**
- Interrupt Index: 15

## Register

|A1|A2|A3|   |Name
|--|--|--|---|-
|0 |0 |0 | R |Unused
|1 |0 |0 | R |Unused
|0 |1 |0 |R/W|[Master Status Register](#master-status-register)
|1 |1 |0 | W |Unused
|0 |0 |1 |R/W|[Data Register](#data-register)
|1 |0 |1 | R |Unused
|0 |1 |1 | R |Unused
|1 |1 |1 | R |Unused

### Master Status Register

|Index  |Name             |Description
|-------|-----------------|-
|0x0001 |RQM              | Indicates that the host can transfor data
|0x0002 |DIO              | Indicates the direction of the data transfor once RQM is set. A 1 indicates read and a 0 indicates write
|0x0004 |BUSY             | This is set to 1 when there is a command in progress
|0x0008 |Unused           | Unused
|0x0010 |Unused           | Unused
|0x0020 |Unused           | Unused
|0x0040 |WP               | like the control line it is 1 the disk is write protected
|0x0080 |Unused           | Unused
|0x0100 |Unused           | Unused
|0x0200 |Unused           | Unused
|0x0400 |Unused           | Unused
|0x0800 |Unused           | Unused
|0x1000 |Unused           | Unused
|0x2000 |Unused           | Unused
|0x4000 |Unused           | Unused
|0x8000 |Unused           | Unused

### Data Register

## Size

|N  |Sector size
|---|-
|00 |128 bytes
|01 |256 bytes
|02 |512 bytes
|03 |1024 bytes

## Drive number

|N  |drive
|---|-
|00 |Drive 0
|01 |Drive 1
|02 |Drive 2
|03 |Drive 3

## Commands

### Read Data 0x01

#### Description

Reads data from a disk

#### Calling

- Send the command (0x00) to the [Data register](#data-register)
- Send the [Drive number](#drive-number) + Head to the [Data register](#data-register)
  - 0000DDDD_0000HHHH
  - D = drive
  - H = head
- Send the track number (0x0000-0xFFFF) to the [Data register](#data-register)
- Send the Sector number (0x0000-0xFFFF) to the [Data register](#data-register)
- Send the size in multiple of 128 bytes

#### Return

Read from the [Data register](#data-register)

### Write Data 0x02

#### Description

Writes data to a disk

#### Calling

- Send the command (0x02) to the [Data register](#data-register)
- Send the [Drive number](#drive-number) + Head to the [Data register](#data-register)
  - 0000DDDD_0000HHHH
  - D = drive
  - H = head
- Send the track number (0x0000-0xFFFF) to the [Data register](#data-register)
- Send the Sector number (0x0000-0xFFFF) to the [Data register](#data-register)
- Send the size in multiple of 128 bytes
- Send the data to the [Data register](#data-register) for 512 times

### Read Drive Parameters 0x03

#### Description

Reads the drives parameters

#### Calling

- Send the command (0x03) to the [Data register](#data-register)
- Send the [Drive number](#drive-number)
- Returns
  - number of heads on the disk in the drive as a word
  - number of tracks on the disk in the drive as a word
  - number of sectors on the disk in the drive as a word
  - [disk type/size](../../fileSystem/FileSystemFormat.md#disk-sizes) on the disk in the drive as a word
  - drive info as a word
    - 0000DDDD_0000MMMM
    - D = number of drives taken as a number
    - M = masking the drives taken as a bit map
