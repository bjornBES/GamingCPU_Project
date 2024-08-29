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
  - [Screen](#screen)
    - [Writing to the screen](#writing-to-the-screen)
  - [Floating values](#floating-values)
  - [Calling convention](#calling-convention)
    - [Caller](#caller)
    - [Callee](#callee)
  - [MEMORY](#memory)
    - [MEMORY LAYOUT](#memory-layout)
  - [REGISTERS](#registers)

## OVERVIEW

- Name: BCG1684
- Data bus: 16/32 bits
- Clock speed: 10 MHz
- Address bus: 20 bits to a max of 32 bits

The BCG-1684 CPU is a 16/32-bit processor designed to reflect the technology and performance characteristics of the DOS era (versions 3.x, 4.x, and potentially 5.x). It aims to compete with the Intel 80286 in terms of raw performance.

based of the **IBM Personal System/2 Model 25**

## INSTRUCTIONS

### INSTRUCTION SET

see the [Instruction Set](./instructions%20BCG1684.md.md) here

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
- 0x04: address                           [address]           a 16 bit address
- 0x05: register                          register
- 0x06: register address                  [register]
- 0x07: Near address                      Near [address]      a 8 bit address
- 0x08: long address                      long [address]      a 24 bit address
- 0x09: far address                       far [address]       a 32 bit address
- 0x0A: float immediate                   float_numberf
- 0x0B: Unused
- 0x0C: 32 bit segment address            [register:register]
- 0x0D: 32 bit segment address immediate  [register:immediate]
- 0x0E: 32 bit segment immediate address  [immediate:register]
- 0x0F: 32 bit segment DS register        [DS:register]
- 0x10: Register A                        A
- 0x11: Register AX                       AX
- 0x1E: immediate qword                   qword number
- 0xFF: None

#### Note

## Date and Time

## CPU PINS

- IRQ: interrupt request

## INTERRUPTS

Interrupts can be triggered by software or hardware specified by the interrupt location port.

### SOFTWARE INTERRUPTS

A software interrupt can be generated using the [BRK](./instructions%20BCG16.md#special-instructions) Instruction and will make a long jump to 0x010000 in bank 0xF, before jumping the PC, MB and the F(flags register) will be push onto the stack and the [PUSHR](./instructions%20BCG16.md#special-instructions) Instruction will also be called.

## CACHES

## Screen

The screen is a 320×240 VGA screen with a 8 bpp and 1 bit for Alpha

### Writing to the screen

to write to the screen you can use the [INT 0x10 interrupt routine](../interrupts.md#int-0x10-services) or write to the video memory at `0x0098100 - 0x00E3100`

## Floating values

the BCG-16 does floating point values using an externel chip the BCG16FL-8F where the [float registers](#registers) so it will take longer to get/set the [floating point registers](#registers)

[datasheet](./SPECS_BCG16F-8F-FloatPointHelper%20CPU.md)

## Calling convention

The calling convention for the BCG architecture is as follows:

### Caller

1. Push Argments
   1. Push all the arguments from left to rigth onto the stack
2. Call the function using [CALL](./instructions%20BCG16.md#special-instructions)
3. Retrieve Return Value
   1. The return value will be in either the A or HL register:
      1. Use the [HL](#registers) register if it's a pointer/address.
      2. Use the [A](#registers) register if it's an immediate value.

### Callee

This is the sequence within the called function:

1. Save the Base Pointer
   1. Push the old [BP](#registers) onto the stack
2. Move the Base Pointer to the Stack Pointer
   1. Move the [BP](#registers) to [SP](#registers) `mov BP, SP`
3. Save All Registers
   1. Push all registers using the [PUSHR](./instructions%20BCG16.md#special-instructions) instruction.
   2. To access arguments, pop them off the stack or reference them using an offset.
4. Return Value
   1. Move the return value to the appropriate register:
      1. HL register if it's a pointer.
      2. AX register if it's an immediate value.
5. Restore Registers
   1. Pop all registers using the [POPR](./instructions%20BCG16.md#special-instructions) instruction.
6. Return from Function
    1. Return from the function with an offset depending on the size of the stack frame using the [RET](./instructions%20BCG16.md#general-instructions) instruction.

## MEMORY

The memory works by setting a segment and an offset like this

``` ACL
mov DS, 0x0080  ; moving 0x0080 into DS(data segment)
mov B,  0x1000  ; moving 0x1000 into the B register
mov A,  [DS:B]  ; here we are using the B register for the offset
                ; so the real address is DS:B or 0x00801000 
```

You can also do this in the `extended Addresing` mode

note: `all cells is in bytes`

### MEMORY LAYOUT

- Address bus: 20 bits to a max of 32 bits

|Base Address |Size         |Name                           |Description
|-------------|-------------|-------------------------------|-
|`0x0000_0000`|`0x0000_0200`| IO ports                      | this is where the Ports is at
|`0x0000_0200`|`0x0003_FE00`| RAM                           | RAM
|`0x0004_0000`|`0x0003_3100`| VRAM                          | video ram
|`0x0007_3100`|`0x0000_8000`| Char set                      | char set
|`0x0007_B100`|`0x0000_4F00`| RESERVED MEMORY               | this memory should not be used
|`0x0008_0000`|`0x0008_0000`| RAM Banked                    | this is the data/prgram is at but banked
|`0x0010_0000`|`0x00F0_0000`| RAM                           | this is the data/prgram is at
|`0x0100_0000`|`0x0F00_0000`| RAM                           | this is the data/prgram is at

the end is `0x0FFF_FFFF`/`0x1000_0000`

## REGISTERS

- A:    (AH + AL) 16  bit general purpose register
- B:    (BH + BL) 16  bit general purpose register
- C:    (CH + CL) 16  bit general purpose register
- D:    (CH + DL) 16  bit general purpose register

- AX:             32  bit general purpose register based on A
- BX:             32  bit general purpose register based on B
- CX:             32  bit general purpose register based on C
- DX:             32  bit general purpose register based on D

- ABX:  (A + B)   32  bit general purpose register if the CR0 bit 0x04 is 1
- CDX:  (C + D)   32  bit general purpose register if the CR0 bit 0x04 is 1

- EAB:  (AX + BX) 64  bit general purpose register if the CR0 bit 0x04 is 1
- ECD:  (CX + DX) 64  bit general purpose register if the CR0 bit 0x04 is 1
- HL:   (H + L)   32  bit general purpose address register

- DS:             16  bit data segment register
- ES:             16  bit extra data segment register
- CS:             16  bit code segment register
- SS:             16  bit stack segment register
- S:              16  bit segment register

- PC:             32  bit program counter

- AF:             32  bit float register
- BF:             32  bit float register
- CF:             32  bit float register
- DF:             32  bit float register

- EABF: (FA + FB) 64  bit float register if the CR0 bit 0x04 is 1
- ECDF: (FC + FD) 64  bit float register if the CR0 bit 0x04 is 1

- BP:             16  bit Stack register
- SP:             16  bit Stack register

- BPX:            32  bit Stack register
- SPX:            32  bit Stack register

- IL:             16  bit register
  the IL(interrupt location register) it's a register that can be read from to get the interrupt location as in where the interrupt came from for example from the keyboard or somewhere else.

- R1:             16  bit temp register
- R2:             16  bit temp register
- R3:             16  bit temp register
- R4:             16  bit temp register

- MB:             8   bit memory bank register

- CR0:            16   bit control register
  - 0   0x0001 Enable A20                       is 1 by defult
  - 1   0x0002
  - 2   0x0004 Use extended registers
  - 3   0x0008
  - 4   0x0010 Enable extended Addresing        enableing 24 bits of address
  - 5   0x0020
  - 6   0x0040
  - 7   0x0080
  - 8   0x0100 Enable Protected mode            enableing 24 bits of data
  - 9   0x0200 Enable extended Protected mode   enableing 32 bits of data
  - 10  0x0400
  - 11  0x0800
  - 12  0x1000
  - 13  0x2000
  - 14  0x4000
  - 15  0x8000 Enable extended mode             enableing 32 bits of address

- CR1:            16   bit control register
  - 0   0x0001 Point to BIOS ROM readonly
  - 1   0x0002
  - 2   0x0004
  - 3   0x0008
  - 4   0x0010
  - 5   0x0020
  - 6   0x0040
  - 7   0x0080
  - 8   0x0100
  - 9   0x0200
  - 10  0x0400
  - 11  0x0800
  - 12  0x1000
  - 13  0x2000
  - 14  0x4000
  - 15  0x8000

- F:              16: bit (F)lags register
  - 0 0x0001 zero
  - 1 0x0002 equals
  - 2 0x0004 signed
  - 3 0x0008 carry
  - 4 0x0010 overflow
  - 5 0x0020 less
  - 6 0x0040
  - 7 0x0080
  - 8 0x0100 error
  - 9 0x0200 interrupt enable
  - 10 0x0400
  - 11 0x0800
  - 12 0x1000
  - 13 0x2000
  - 14 0x4000
  - 15 0x8000 HALT
