# FDC Specifications

- Name: **BFDCG** or **BEs Floppy disk controller Gaming**
- Interrupt Index: 0x01

## Register

|A1|A2|A3|   |Name
|--|--|--|---|-
|0 |0 |0 | R |[Status Register](#status-register)
|1 |0 |0 | R |Data
|0 |1 |0 | R |[Master Status Register](#master-status-register)
|1 |1 |0 | W |[Data Rate Select Register](#data-rate-select-register)
|0 |0 |1 |R/W|[Data Register](#data-register)
|1 |0 |1 | R |Data
|0 |1 |1 | R |Data
|1 |1 |1 | R |Data

### Status Register

|15  |14  |13  |12  |11  |10  |9   |8   |7   |6   |5   |4   |3   |2   |1   |0
|----|----|----|----|----|----|----|----|----|----|----|----|----|----|----|----
|test|test|test|test|test|test|test|test|test|test|test|test|test|test|test|test

### Master Status Register

|0x8000 |0x4000 |0x2000 |0x1000 |0x0800 |0x0400 |0x0200 |0x0100 |0x0080 |0x0040 |0x0020 |0x0010 |0x0008 |0x0004 |0x0002 |0x0001
|-------|-------|-------|-------|-------|-------|-------|-------|-------|-------|-------|-------|-------|-------|-------|----
|test   |test   |test   |test   |test   |test   |test   |OK     |test   |test   |test   |test   |test   |test   |test   |User Write

### Data Rate Select Register

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

Reads a sector from the disk

#### Calling

- Send the command (0x00) to the [Data register](#data-register)
- Send the [Drive number](#drive-number) + Head to the [Data register](#data-register)
  - DD00HHHH
  - D = drive
  - H = head
- Send the track number (0x0000-0xFFFF) to the [Data register](#data-register)
- Send the Sector number (0x00-0xFF) to the [Data register](#data-register)
- Send the size in multiple of 128 bytes

#### Return

Read from the [Data register](#data-register)

### Write Data 0x02

#### Description

Writes a sector to the disk

#### Calling

- Send the command (0x01) to the [Data register](#data-register)
- Send the [Drive number](#drive-number) + Head to the [Data register](#data-register)
  - DD00HHHH
  - D = drive
  - H = head
- Send the track number to the [Data register](#data-register)
- Send the Sector number to the [Data register](#data-register)
- Send the data to the [Data register](#data-register) for 512 times

### Recalibrate 0x03

#### Description

Moves the head back to track 0

#### Calling

- Send the command (0x03) to the [Data register](#data-register)
- Send the [Drive number](#drive-number) + Head to the [Data register](#data-register)
  - DD00HHHH
  - D = drive
  - H = head

### Seek 0x04

Moves the head to the track

#### Calling

- Send the command (0x04) to the [Data register](#data-register)
- Send the [Drive number](#drive-number) + Head to the [Data register](#data-register)
  - DD00HHHH
  - D = drive
  - H = head
- Send the track number (0x0000-0xFFFF) to the [Data register](#data-register)
