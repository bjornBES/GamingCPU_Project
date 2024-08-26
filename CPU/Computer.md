# CPU Specifications

- [CPU Specifications](#cpu-specifications)
  - [OVERVIEW](#overview)
  - [Date and Time](#date-and-time)
  - [PORTS](#ports)
    - [RS232C DE-9 Serial Ports](#rs232c-de-9-serial-ports)
    - [VGA Screen port (more in Screen)](#vga-screen-port-more-in-screen)
  - [Partial Port](#partial-port)
    - [Expaction cards](#expaction-cards)
    - [Floppy Disk Controller Port](#floppy-disk-controller-port)
    - [ATA HDD](#ata-hdd)
  - [Screen](#screen)
    - [Writing to the screen](#writing-to-the-screen)
    - [Video Layout](#video-layout)

## OVERVIEW

- Name: BCG16
- Data bus: 16 bits
- Clock speed: 10 MHz
- Address bus: 24 bits

The BCG-16 CPU is a 16-bit processor designed to reflect the technology and performance characteristics of the DOS era (versions 3.x, 4.x, and potentially 5.x). It aims to compete with the Intel 80286 in terms of raw performance.

based of the **IBM Personal System/2 Model 25**

## Date and Time

## PORTS

The Computer has a few ports for example the [Keyboard port or Mouse port](#rs232c-de-9-serial-ports)

### RS232C DE-9 Serial Ports

Keyboard port at `0x00`

Mouse port at `0x01`

### VGA Screen port (more in [Screen](#screen))

The VGA Screen port is at `0x02`

## Partial Port

The Partial Port is a printer port at `0x06`

### Expaction cards

There are 2 expaction cards at `0xFE-0xFF` using the Micro Channel Architecture(MCA) at 10 Mhz

### Floppy Disk Controller Port

The Floppy Disk Controller Port is at `0x100 to 0x107`

These ports will use the `34-pin floppy drive connectors`

### ATA HDD

The 20MB HDD disk is at `0x130 to 0x131`

These ports will use the `40-pin IDE connectors`

## Screen

The screen is a 320×240 VGA screen with a 8 bpp

### Writing to the screen

to write to the screen you can use the [INT 0x10 interrupt routine](./interrupts.md#int-0x10-services) or write to the video memory at `0x0A18100 - 0x0A63100`

### Video Layout

CCCCCCCC

- C = color index
