# CPU Specifications

- [CPU Specifications](#cpu-specifications)
  - [OVERVIEW](#overview)
  - [INSTRUCTIONS](#instructions)
    - [INSTRUCTION SET](#instruction-set)
    - [INSTRUCTION LAYOUT](#instruction-layout)
    - [ARGUMENT MODE](#argument-mode)
      - [Note](#note)
  - [Date and Time](#date-and-time)
  - [CPU PINS](#cpu-pins)
  - [INTERRUPTS](#interrupts)
    - [SOFTWARE INTERRUPTS](#software-interrupts)
  - [CACHES](#caches)
  - [PORTS](#ports)
    - [PS/2 PORTS](#ps2-ports)
    - [Screen (more in Screen)](#screen-more-in-screen)
    - [Floppy Disk Ports](#floppy-disk-ports)
    - [HDD](#hdd)
  - [Partial Printer Port](#partial-printer-port)
    - [Expaction cards](#expaction-cards)
  - [Screen](#screen)
    - [Writing to the screen](#writing-to-the-screen)
    - [Video Layout](#video-layout)
      - [Flags](#flags)
  - [Calling convention](#calling-convention)
    - [Caller](#caller)
    - [Callee](#callee)
  - [MEMORY](#memory)
    - [MEMORY LAYOUT](#memory-layout)
  - [REGISTERS](#registers)

## OVERVIEW

- Name: BCG16
- Data bus: 8 bits
- Clock speed: 10 MHz
- Address bus: 16/20 bits

The BCG8 CPU is a 8-bit processor designed to reflect the technology and performance characteristics of the early DOS era (versions 3.x, 4.x, and potentially 5.x). It aims to compete with the Intel 8088/8086 in terms of raw performance.

based of the **IBM Personal System/2 Model 30286**

## INSTRUCTIONS

### INSTRUCTION SET

see the [Instruction Set](./instructions%20BCG8.md) here

### INSTRUCTION LAYOUT

XXXXXXXX_AAAAAAAA_BBBBBBBB

X = Op code
A = argument1
B = argument2
U = unused

### ARGUMENT MODE

- 0x00: immediate byte                    byte number
- 0x01: immediate word                    word number
- 0x02: immediate tbyte                   tbyte number
- 0x03: immediate dword                   dword number
- 0x04: address 16                        [address]
- 0x05: register                          register
- 0x06: register address                  [register]
- 0x07: Near address                      Near [address]      a 8 bit address
- 0x08: Unused
- 0x09: Unused
- 0x0A: Unused
- 0x0B: 32 bit segment DS address         [DS:register]
- 0x0C: 32 bit segment address            [register:register]
- 0x0D: 32 bit segment address immediate  [register:immediate]
- 0x0E: 32 bit segment immediate address  [immediate:register]
- 0x10: Register A                        A
- 0xFF: None

#### Note

## Date and Time

## CPU PINS

- IRQ: interrupt request

## INTERRUPTS

Interrupts can be triggered by software or hardware specified by the interrupt location port.

### SOFTWARE INTERRUPTS

A software interrupt can be generated using the [BRK](./instructions%20BCG8.md#special-instructions) Instruction and will make a long jump to 0x010000 in bank 0xF, before jumping the PC, MB and the F(flags register) will be push onto the stack and the [PUSHR](./instructions%20BCG8.md#special-instructions) Instruction will also be called.

## CACHES

## PORTS

The Ports can be communicate to and from using the [OUTB and INB](./instructions%20BCG8.md#io-instructions) instructions

Ports are memory-mapped IO.

### PS/2 PORTS

The PS/2 Mouse is at `0x00`

The PS/2 Keyboard is at `0x01`

### Screen (more in [Screen](#screen))

The VGA Screen port is at `0x02`

### Floppy Disk Ports

The Floppy Disk Ports are at `0x03 to 0x04`

### HDD

The 20MB HDD disk is at `0x05`

## Partial Printer Port

The Partial Printer Port is at `0x06`

### Expaction cards

There are 4 expaction cards at `0xFC-0xFF` using the Industry Standard Architecture(ISA)

## Screen

The screen is a 320×240 VGA screen with a 8 bpp and 1 bit for Alpha

### Writing to the screen

to write to the screen you can use the [INT 0x10 interrupt routine](./interrupts.md#int-0x10-services) or write to the video memory at `0xDA800 - 0xFFFFF`

### Video Layout

- FFFF_FFFF
- Bitmap mode if the upper halv of the flags are 0
  - CCCCCCCC
- Char mode
  - CCCCCCCC_DDDDDDDD_AAAAAAAA
- Sprite mode
  - SSSSSSSS_AAAAAAAA

- S = Sprite index
- C = color index
- F = Flags

#### Flags

NOTE: if all the bit are 0 then it will be a pixel in bitmap mode, the jumpper can also be set and the bitmap mode will allways be active.

- bit 0:   is WIP?
- bit 1:   is Background Layer?
- bit 2:   is alpha?
- bit 3:   is WIP?
- bit 4:   is char?
- bit 5:   is Sprite?
- bit 6:   is WIP?
- bit 7:   is WIP?
  - if this bit is set, the char data will contain the ASCII char to be displayed and colored using the COLOR info

## Calling convention

The calling convention for the BCG architecture is as follows:

### Caller

1. Push Argments
   1. Push all the arguments from left to rigth onto the stack
2. Call the function using [CALL](./instructions%20BCG8.md#special-instructions)
3. Retrieve Return Value
   1. The return value will be in either the AL or HL register:
      1. Use the [HL](#registers) register if it's a pointer/address.
      2. Use the [AL](#registers) register if it's an immediate value.

### Callee

This is the sequence within the called function:

1. Save the Base Pointer
   1. Push the old [BP](#registers) onto the stack
2. Move the Base Pointer to the Stack Pointer
   1. Move the [BP](#registers) to [SP](#registers) `mov BP, SP`
3. Save All Registers
   1. Push all registers using the [PUSHR](./instructions%20BCG8.md#special-instructions) instruction.
   2. To access arguments, pop them off the stack or reference them using an offset.
4. Return Value
   1. Move the return value to the appropriate register:
      1. HL register if it's a pointer.
      2. AL register if it's an immediate value.
5. Restore Registers
   1. Pop all registers using the [POPR](./instructions%20BCG8.md#special-instructions) instruction.
6. Return from Function
    1. Return from the function with an offset depending on the size of the stack frame using the [RET](./instructions%20BCG8.md#general-instructions) instruction.

## MEMORY

The memory works by setting a segment and an offset like this

``` ACL
mov DS, 0x0080  ; moving 0x0080 into DS(data segment)
mov BX, 0x1000  ; moving 0x1000 into the BX register
mov AX, [DS:BX] ; here we are using the BX register for the offset
                ; so the real address is DS:BX or 0x00801000 
```

note: `all cells is in bytes`

### MEMORY LAYOUT

|Base Address   |Size       |Name                           |Description
|---------------|-----------|-------------------------------|-
|`0x00000`      |`0x00200`  | IO ports                      | this is where the Ports is at
|`0x00200`      |`0x07E00`  | GENERAL PURPOSE RAM           | this is Ram that is not used
|`0x08000`      |`0x08000`  | RAM Banked                    | this is the data/prgram is at but banked
|`0x10000`      |`0xC2800`  | extended RAM memory           | this is the data/prgram is at
|`0xD2800`      |`0x08000`  | Char set                      | char set
|`0xDA800`      |`0x25800`  | VRAM                          | video ram

## REGISTERS

- AL    8  bit general purpose register
- AH    8  bit general purpose register
- A:    16 bit general purpose register can only be used if the CR0 bit 0x04 is 1

- BL    8  bit general purpose register
- BH    8  bit general purpose register
- B:    16 bit general purpose register can only be used if the CR0 bit 0x04 is 1

- CL    8  bit general purpose register
- CH    8  bit general purpose register

- DL    8  bit general purpose register
- DH    8  bit general purpose register

- H     8  bit general purpose address register
- L     8  bit general purpose address register
- HL    16 bit general purpose address register

- DS:   16  bit segment register the Data segment register
- ES:   16  bit extra data segment register
- CS:   16  bit code segment register
- S:    16  bit segment register

- PC:   16  bit program counter
  - if the Use extended registers in CR0 is 1 the PC will be 20 bits

- BP:   8   bit Stack register
- SP:   8   bit Stack register

- IL:   8   bit register
  the IL(interrupt location register) it's a register that can be read from to get the interrupt location as in where the interrupt came from for example from the keyboard or somewhere else.

- R1:   8   bit temp register
- R2:   8   bit temp register

- MB:   4   bit memory bank register

- CR0:  8   bit control register 0
  - 0 0x01 Enable A20
  - 1 0x02
  - 2 0x04 Use extended registers
  - 3 0x08
  - 4 0x10
  - 5 0x20
  - 6 0x40
  - 7 0x80

- FL:    8  bit (F)lags register
  - 0 0x01 zero
  - 1 0x02 equals
  - 2 0x04 signed
  - 3 0x08 carry
  - 4 0x10 overflow
  - 5 0x20 less
  - 6 0x40
  - 7 0x80

- FH:   8   bit (F)lags regiter
  - 0 0x01 error
  - 1 0x02 interrupt enable
  - 2 0x04
  - 3 0x08
  - 4 0x10
  - 5 0x20
  - 6 0x40
  - 7 0x80 HALT
