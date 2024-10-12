- [Hardware Interrupts](#hardware-interrupts)
- [BIOS Interrupts](#bios-interrupts)
  - [int 0x10 terminal services](#int-0x10-terminal-services)
    - [PRINT CHAR AH = 0x00](#print-char-ah--0x00)
      - [Inputs](#inputs)
      - [Description](#description)
    - [Read char key AH = 0x01](#read-char-key-ah--0x01)
      - [Inputs](#inputs-1)
      - [Return](#return)
      - [Description](#description-1)
    - [Print string AH = 0x02](#print-string-ah--0x02)
      - [Inputs](#inputs-2)
      - [Description](#description-2)
  - [int 0x13 services (Disk functions)](#int-0x13-services-disk-functions)
    - [READ SECTOR AH = 0x01](#read-sector-ah--0x01)
      - [Function code](#function-code)
      - [Parameters](#parameters)
      - [Description](#description-3)
      - [Return](#return-1)
    - [WRITE SECTOR AH = 0x02](#write-sector-ah--0x02)
      - [Function code](#function-code-1)
      - [Parameters](#parameters-1)
      - [Description](#description-4)
    - [READ DISK STATUS AH = 0x03](#read-disk-status-ah--0x03)
      - [Function code](#function-code-2)
      - [Parameters](#parameters-2)
      - [Description](#description-5)
    - [GET DRIVE PARAMETERS AH = 0x08](#get-drive-parameters-ah--0x08)
      - [Function code](#function-code-3)
      - [Parameters](#parameters-3)
      - [Description](#description-6)
  - [int 0x15 BIOS interrupts](#int-0x15-bios-interrupts)

# Hardware Interrupts

# BIOS Interrupts

## int 0x10 terminal services

### PRINT CHAR AH = 0x00

#### Inputs

```
AL = char to be printed,
BL = color index
```

#### Description

prints the char in AL

### Read char key AH = 0x01

#### Inputs

```
```

#### Return

```
AL = ASCII charactor of the button pressed
```

#### Description

prints the char in AL

### Print string AH = 0x02

#### Inputs

```
DS = the segment of the string
B = the offset of the string
```

#### Description

prints a string to the screen

## int 0x13 services (Disk functions)

### READ SECTOR AH = 0x01

#### Function code

AH = 0x01

#### Parameters

AL = heads,
B = sector,
C = track,
DL = Drive,
HL = destination address

#### Description

Reads a sector from the disk and moves to data to HL

#### Return

carry flag = 1 = error :D

### WRITE SECTOR AH = 0x02

#### Function code

AH = 0x02

#### Parameters

AL = disk,
BX = track,
CL = heads,
DX = sector,
HL = address of the data buffer,

#### Description

write a sector size from the `address of the data buffer` to the disk using the page and sector

error in the [ERROR FLAG](./SPECS_BEG-8-CPU.md#registers)

### READ DISK STATUS AH = 0x03

#### Function code

AH = 0x03

#### Parameters

AL = disk,

#### Description

returns in the [AL register](./SPECS_BEG-8-CPU.md#registers) more [here](./../fileSystem/FileSystemFormat.md#disk-errors)

### GET DRIVE PARAMETERS AH = 0x08

#### Function code

AH = 0x08

#### Parameters

AL = disk,

#### Description

this function will return in:
BX the bytes per sector,
CH the total heads,
CL the total tracks,
HL the disk size

## int 0x15 BIOS interrupts
