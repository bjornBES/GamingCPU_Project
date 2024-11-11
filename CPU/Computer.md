# CPU Specifications

- [CPU Specifications](#cpu-specifications)
  - [Date and Time](#date-and-time)
  - [PORTS](#ports)
    - [PS/2 Ports](#ps2-ports)
    - [VGA Screen port (more in Screen)](#vga-screen-port-more-in-screen)
  - [Parallel Port](#parallel-port)
    - [Expaction cards](#expaction-cards)
    - [Floppy Disk Controller Port](#floppy-disk-controller-port)
    - [ATA HDD](#ata-hdd)
  - [Screen](#screen)
    - [Modes](#modes)
    - [Commands](#commands)
    - [Video Layout](#video-layout)
      - [Mode 0, 1](#mode-0-1)

## Date and Time

## PORTS

The Computer has a few ports for example the [Keyboard port or Mouse port](#ps2-ports)

### PS/2 Ports

Keyboard port at `0x00`

Mouse port at `0x01`

### VGA Screen port (more in [Screen](#screen))

The VGA Screen port is at `0x02`

## Parallel Port

The Parallel Port is a printer port at `0x06`

### Expaction cards

There are 2 expaction cards at `0xFC-0xFD` using the Industry Standard Architecture(ISA) at 10 Mhz

There are 2 expaction cards at `0xFE-0xFF` using the Micro Channel Architecture(MCA) at 10 Mhz

### Floppy Disk Controller Port

The Floppy Disk Controller Port is at `0x100 to 0x107`

These ports will use the `34-pin floppy drive connectors`

### ATA HDD

The 20MB HDD disk is at `0x130 to 0x131`

These ports will use the `40-pin IDE connectors`

## Screen

The screens default mode is 360×400 VGA screen with 16 colors

### Modes

|Mode |Resolution |Colors |T/G  |CharBlock  |AlphaRes
|-----|-----------|-------|-----|-----------|-
|0    |320 x 200  |16     |T    |8 x 8      | 40 x 25
|1    |360 x 400  |16     |T    |8 x 8      | 40 x 25
|2    |640 x 400  |16     |T    |8 x 16     | 80 x 25
|3    |720 x 400  |16     |T    |8 x 16     | 80 x 25
|7    |720 x 400  |mono   |T    |8 x 16     | 80 x 25
|11   |640 x 480  |2      |G    |8 x 16     | 80 x 30
|12   |640 x 480  |16     |G    |8 x 16     | 80 x 30
|13   |320 x 200  |256    |G    |8 x 8      | 40 x 25

### Commands

|Command  |Name               |argument1|argument2|argument3|Description|number of ticks
|-        |-                  |-        |-        |-        |-          |-
|0x0005   |Clear buffer       |&nbsp;   |&nbsp;   |&nbsp;   |Will clear the buffers|2
|0x0010   |Get resolution     |&nbsp;   |&nbsp;   |&nbsp;   |Will load the screen resolution and font size into the outputBuffer|2
|0x0011   |Set Mode           |Mode     |&nbsp;   |&nbsp;   |Will set the graphic cards mode to the mode|3
|0x8001   |reset output index |&nbsp;   |&nbsp;   |&nbsp;   |Will reset the outputIndex to zero|1

### Video Layout

#### Mode 0, 1

CCCCCCCC_IIIIUUUU

- C = char index
- I = color index
- U = unused
