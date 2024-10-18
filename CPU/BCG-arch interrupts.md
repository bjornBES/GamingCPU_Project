- [Hardware Interrupts](#hardware-interrupts)
- [BIOS Interrupts](#bios-interrupts)
  - [int 0x10 terminal services](#int-0x10-terminal-services)
  - [int 0x13 services (Disk functions)](#int-0x13-services-disk-functions)
    - [Get drive parameters returns](#get-drive-parameters-returns)
    - [CHS to LBA AH = 0x10](#chs-to-lba-ah--0x10)
      - [Function code](#function-code)
      - [Parameters](#parameters)
      - [Return](#return)
    - [LBA to CHS AH = 0x11](#lba-to-chs-ah--0x11)
      - [Function code](#function-code-1)
      - [Parameters](#parameters-1)
      - [Return](#return-1)
  - [int 0x15 BIOS interrupts](#int-0x15-bios-interrupts)

# Hardware Interrupts

# BIOS Interrupts

## int 0x10 terminal services

|AH   |Function Name|Inputs                     |Outputs|Description
|--   |-            |-                          |-      |-
|0x00 |Print char   |AL = char, BL = char color |&nbsp; |Prints the charactor in AL
|0x01 |Read key     |&nbsp;                     |AL = ASCII charactor of the button pressed|reading the last key in the input buffer

## int 0x13 services (Disk functions)

|AH   |Function Name        |Inputs                     |Outputs|Description
|--   |-                    |-                          |-      |-
|0x01 |Read sector          |R1 = heads, R2 = tracks, R3 = sector, DL = drive, HL = destination address|CF = error|reading a sector
|0x02 |Read sector          |R1 = heads, R2 = tracks, R3 = sector, DL = drive, HL = source address|CF = error|reading a sector
|0x08 |get drive parameters |DL = drive|[Here](#get-drive-parameters-returns|Getting the drives paramters

### Get drive parameters returns

this function will return in:
R4 = head per cylinders
R5 = track per heads
R6 = sectors per track
R7 = the number of total tracks
R8 = the number of total heads
CF = error

### CHS to LBA AH = 0x10

#### Function code

AH = 0x10

#### Parameters

R1 = heads,
R2 = track,
R3 = sector,

#### Return

HL = LBA

### LBA to CHS AH = 0x11

#### Function code

AH = 0x11

#### Parameters

HL = LBA,
R4 = heads per cylinders,
R5 = sectors per track,

#### Return

R1 = heads,
R2 = track,
R3 = sector,

## int 0x15 BIOS interrupts
