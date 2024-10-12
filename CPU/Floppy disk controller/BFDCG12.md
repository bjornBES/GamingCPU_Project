# FDC Specifications

- Name: **BFDCG** or **BEs Floppy disk controller Gaming**
- Interrupt Index: 0x01

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
|0 |0 |0 | R |Unused
|1 |0 |0 | R |Unused
|0 |1 |0 | R |Unused
|1 |1 |0 | R |Unused
|0 |0 |1 | R |Unused
|1 |0 |1 | R |Unused
|0 |1 |1 | R |Unused
|1 |1 |1 | R |Unused

### Status Register

|Index  |Name
|-------|-
|0x0001 |Unused
|0x0002 |Unused
|0x0004 |Unused
|0x0008 |Unused
|0x0010 |Unused
|0x0020 |Unused
|0x0040 |Unused
|0x0080 |Unused
|0x0100 |Unused
|0x0200 |Unused
|0x0400 |Unused
|0x0800 |Unused
|0x1000 |Unused
|0x2000 |Unused
|0x4000 |Unused
|0x8000 |Unused

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
|0x0080 |DIR              | like the control line it is 1 the head steps towards the center
|0x0100 |Index            | like the control line it is 1 if the head is passing the index hole
|0x0200 |Track 0          | like the control line it is 1 if the head is on track 0
|0x0400 |Unused           | Unused
|0x0800 |Unused           | Unused
|0x1000 |DRS1             | \|
|0x2000 |DRS2             | \|
|0x4000 |DRS3             | \|
|0x8000 |DRS4             | data rate

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
  - DD00HHHH
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

- Send the command (0x01) to the [Data register](#data-register)
- Send the [Drive number](#drive-number) + Head to the [Data register](#data-register)
  - DD00HHHH
  - D = drive
  - H = head
- Send the track number (0x0000-0xFFFF) to the [Data register](#data-register)
- Send the Sector number (0x0000-0xFFFF) to the [Data register](#data-register)
- Send the size in multiple of 128 bytes
- Send the data to the [Data register](#data-register) for 512 times

### Recalibrate 0x03

#### Description

Moves the head back to track 0

#### Calling

- Send the command (0x03) to the [Data register](#data-register)
- Send the [Drive number](#drive-number)

### Seek 0x04

Moves the head to the track

#### Calling

- Send the command (0x04) to the [Data register](#data-register)
- Send the [Drive number](#drive-number) + Head to the [Data register](#data-register)
  - DD00HHHH
  - D = drive
  - H = head
- Send the track number (0x0000-0xFFFF) to the [Data register](#data-register)
