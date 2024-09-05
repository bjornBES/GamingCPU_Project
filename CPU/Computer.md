# CPU Specifications

- [CPU Specifications](#cpu-specifications)
  - [Date and Time](#date-and-time)
  - [PORTS](#ports)
    - [RS232C DE-9 Serial Ports](#rs232c-de-9-serial-ports)
    - [VGA Screen port (more in Screen)](#vga-screen-port-more-in-screen)
  - [Parallel Port](#parallel-port)
    - [Expaction cards](#expaction-cards)
    - [Floppy Disk Controller Port](#floppy-disk-controller-port)
    - [ATA HDD](#ata-hdd)
  - [Screen](#screen)
    - [Modes](#modes)
    - [Writing to the screen](#writing-to-the-screen)
    - [Video Layout](#video-layout)

## Date and Time

## PORTS

The Computer has a few ports for example the [Keyboard port or Mouse port](#rs232c-de-9-serial-ports)

### RS232C DE-9 Serial Ports

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

The screen is a 640×480 VGA screen with a 8 bpp

### Modes

|Mode |Resolution |Colors |T/G  |CharBlock  |AlphaRes
|-----|-----------|-------|-----|-----------|-
|0    |360 x 400  |16     |T    |8 x 16     | 40 x 25
|2    |720 x 400  |16     |T    |8 x 16     | 80 x 25
|7    |720 x 400  |mono   |T    |8 x 16     | 80 x 25
|11   |640 x 480  |2      |G    |8 x 16     | 80 x 30
|12   |640 x 480  |16     |G    |8 x 16     | 80 x 30
|13   |320 x 200  |256    |G    |8 x 8      | 40 x 25

### Writing to the screen

to write to the screen you can use the [INT 0x10 interrupt routine](./interrupts.md#int-0x10-services) or write to the video memory at `0x0A18100 - 0x0A63100`

### Video Layout

CCCCCCCC

- C = color index
