- [Hardware Interrupts](#hardware-interrupts)
- [CPU Interrupts](#cpu-interrupts)
  - [int 0x00 Cpu Interrupts](#int-0x00-cpu-interrupts)
    - [Load interrupt Descriptor table AH = 0x00](#load-interrupt-descriptor-table-ah--0x00)
      - [Parameters](#parameters)
- [BIOS Interrupts](#bios-interrupts)
  - [int 0x10 terminal services](#int-0x10-terminal-services)
    - [PRINT CHAR AH = 0x00](#print-char-ah--0x00)
      - [Parameters](#parameters-1)
      - [Description](#description)
  - [int 0x13 services (Disk functions)](#int-0x13-services-disk-functions)
    - [READ SECTOR AH = 0x01](#read-sector-ah--0x01)
      - [Function code](#function-code)
      - [Parameters](#parameters-2)
      - [Description](#description-1)
    - [WRITE SECTOR AH = 0x02](#write-sector-ah--0x02)
      - [Function code](#function-code-1)
      - [Parameters](#parameters-3)
      - [Description](#description-2)
    - [READ DISK STATUS AH = 0x03](#read-disk-status-ah--0x03)
      - [Function code](#function-code-2)
      - [Parameters](#parameters-4)
      - [Description](#description-3)
    - [GET DRIVE PARAMETERS AH = 0x08](#get-drive-parameters-ah--0x08)
      - [Function code](#function-code-3)
      - [Parameters](#parameters-5)
      - [Description](#description-4)
  - [int 0x15 BIOS interrupts](#int-0x15-bios-interrupts)

# Hardware Interrupts

# CPU Interrupts

## int 0x00 Cpu Interrupts

### Load interrupt Descriptor table AH = 0x00

#### Parameters

```
AL = Interrupt routine index
HL = Pointer to interrupt table
```

# BIOS Interrupts

## int 0x10 terminal services

### PRINT CHAR AH = 0x00

#### Parameters

```
AL = char to be printed,
BX = color index
```

#### Description

prints the char in AL

## int 0x13 services (Disk functions)

### READ SECTOR AH = 0x01

#### Function code

AH = 0x01

#### Parameters

AL = disk,
BX = track,
CL = heads,
DX = sector

#### Description

returns in the [Data file cache](./SPECS_BEG-8-CPU.md#file-data-cache) and error in the [ERROR FLAG](./SPECS_BEG-8-CPU.md#registers)

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
