# CPU Specifications

- [CPU Specifications](#cpu-specifications)
  - [OVERVIEW](#overview)
  - [INSTRUCTIONS](#instructions)
    - [INSTRUCTION SET](#instruction-set)
    - [INSTRUCTION LAYOUT](#instruction-layout)
    - [ARGUMENT MODE](#argument-mode)
      - [Note](#note)
  - [Interrupt descriptor table](#interrupt-descriptor-table)
    - [Layout](#layout)
  - [Date and Time](#date-and-time)
  - [INTERRUPTS](#interrupts)
  - [Pipelining](#pipelining)
  - [CACHES](#caches)
  - [Floating values](#floating-values)
  - [Calling convention](#calling-convention)
    - [Caller](#caller)
    - [Callee](#callee)
  - [Protected mode](#protected-mode)
  - [MEMORY](#memory)
    - [Protected Memory](#protected-memory)
    - [MEMORY LAYOUT](#memory-layout)
  - [REGISTERS](#registers)

## OVERVIEW

- Name: BCG1684
- Data bus: 16/32 bits
- Clock speed: 20 MHz
- Address bus: 16 bits to a max of 24 bits

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
- 0x04: Unused                            Unused
- 0x05: Unused                            Unused
- 0x06: immediate qword                   qword number
- 0x07: float immediate                   float_numberf
- 0x08: register                          register
- 0x09: register address                  [register]
- 0x0B: Near address                      Near [address]      a 8 bit address
- 0x0C: address                           [address]           a 16 bit address
- 0x0D: long address                      long [address]      a 24 bit address
- 0x0E: far address                       far [address]       a 32 bit address
- 0x0F: Relative address                  [byte address]      an 8 bit offset to the PC
- 0x10: 32 bit segment address            [register:register]
- 0x11: 32 bit segment address immediate  [register:immediate]
- 0x12: 32 bit segment DS register        [DS:register]
- 0x13: 32 bit segment DS B               [DS:B]
- 0x1A: Register AL                       AL
- 0x1B: Register A                        A
- 0x1C: Register AX                       AX
- 0x1D: Register HL                       HL
- 0x1E: Register address HL               [HL]
- 0x1F: BP relative address               [BP - number]/[BP + number]
- 0x20: Register BL                       BL
- 0x21: Register B                        B
- 0x22: Register BX                       BX
- 0xF0: Register AF                       AF
- 0xF1: Register BF                       BF
- 0xF2: Register CF                       CF
- 0xF3: Register DF                       DF
- 0xFF: None

#### Note

## Interrupt descriptor table

The Interrupt vector table is 32 KB in size

### Layout

The first word is the Address Segment for the routine

The second word is the Address offset for the routine

## Date and Time

## INTERRUPTS

Interrupts can be triggered by hardware where the CPU will look into the [Interrupt descriptor table](#interrupt-descriptor-table)(IDT) and jump to the address specified by the IDT.

## Pipelining

The BCG16 can do 3 stage pipelining like this

|Stage      |Stage1             |Stage2               |Stage3                 |Stage4
|-----------|-------------------|---------------------|-----------------------|-
|Opertion1  |Fetch instruction  |instruction Decoding |instruction execution  |Fetch instruction
|Opertion2  |                   |Fetch instruction    |instruction Decoding   |instruction execution
|Opertion2  |                   |                     |Fetch instruction      |instruction Decoding

## CACHES

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
   1. Push the old [BPX](#registers) onto the stack
2. Move the Base Pointer to the Stack Pointer
   1. Move the [BPX](#registers) to [SPX](#registers) `mov BPX, SPX`
3. Save All Registers
   1. Push all registers using the [PUSHR](./instructions%20BCG16.md#special-instructions) instruction.
   2. To access arguments, pop them off the stack or reference them using an offset by doing [BP - X].
4. Return Value
   1. Move the return value to the appropriate register:
      1. HL register if it's a pointer.
      2. AX register if it's an immediate value.
5. Restore Registers
   1. Pop all registers using the [POPR](./instructions%20BCG16.md#special-instructions) instruction.
6. Return from Function
    1. Return from the function with an offset depending on the size of the stack frame using the [RET](./instructions%20BCG16.md#general-instructions) instruction.

## Protected mode

In Protected mode there are some memory regions that is [protected](#protected-memory)

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

### Protected Memory

Protected memory are some regions that the CPU can't read from or write to

### MEMORY LAYOUT

- Address bus: 16 bits to a max of 24 bits

|Base Address |Size         |Name         |Is Protected |Description
|-------------|-------------|-------------|-------------|-
|`0x0000_0000`|`0x0000_0200`| IO ports    |true         | this is where the Ports is at
|`0x0000_0200`|`0x0000_FE00`| RAM         |false        | RAM in the first segment
|`0x0001_0200`|`0x0002_8000`| RAM         |false        | RAM
|`0x0003_8000`|`0x0000_8000`| Char set    |true         | char set
|`0x0004_0000`|`0x0004_0000`| VRAM        |false        | video ram
|`0x0008_0000`|`0x0008_0000`| RAM Banked  |false        | this is the data/prgram is at but banked
|`0x0010_0000`|`0x00F0_0000`| RAM         |false        | this is the data/prgram is at

the end is `0x0FF_FFFF`/`0x100_0000`

## REGISTERS

- A:    (AH + AL) 16  bit general purpose register
- B:    (BH + BL) 16  bit general purpose register
- C:    (CH + CL) 16  bit general purpose register
- D:    (CH + DL) 16  bit general purpose register

- AX:             32  bit general purpose register based on A
  - if Protected mode is enabled
- BX:             32  bit general purpose register based on B
  - if Protected mode is enabled
- CX:             32  bit general purpose register based on C
  - if Protected mode is enabled
- DX:             32  bit general purpose register based on D
  - if Protected mode is enabled

- ABX:  (A + B)   32  bit general purpose register if the CR0 bit 0x04 is 1
- CDX:  (C + D)   32  bit general purpose register if the CR0 bit 0x04 is 1

- EAB:  (AX + BX) 64  bit general purpose register if the CR0 bit 0x0200 is 1
- ECD:  (CX + DX) 64  bit general purpose register if the CR0 bit 0x0200 is 1
- HL:   (H + L)   32  bit general purpose address register

- DS:             16  bit data segment register
- ES:             16  bit extra data segment register
- SS:             16  bit stack segment register
- S:              16  bit segment register

- PC:             24  bit program counter

- AF:             32  bit float register
- BF:             32  bit float register
- CF:             32  bit float register
- DF:             32  bit float register

- EABF: (FA + FB) 64  bit float register if the CR0 bit 0x0200 is 1
- ECDF: (FC + FD) 64  bit float register if the CR0 bit 0x0200 is 1

- BP:             16  bit Stack register
- SP:             16  bit Stack register

- BPX:            32  bit Stack register
- SPX:            32  bit Stack register

- R1:             16  bit temp register
- R2:             16  bit temp register
- R3:             16  bit temp register
- R4:             16  bit temp register

- MB:             8   bit memory bank register
  - Starting at 4 bit until 0x00100(Enable extended mode) in CR0 is 1

- CR0:            24   bit control register
  - 0   0x00001 Enable A20                      enableing 20 bits of address
  - 1   0x00002
  - 2   0x00004 Use extended registers
  - 3   0x00008
  - 4   0x00010 Enable extended mode            enableing 24 bits of address and data
  - 5   0x00020
  - 6   0x00040
  - 7   0x00080
  - 8   0x00100
  - 9   0x00200 Use extended 32 bit registers   enableing the 32 bit registers
  - 10  0x00400
  - 11  0x00800
  - 12  0x01000
  - 13  0x02000
  - 14  0x04000
  - 15  0x08000 Enable Protected mode           enableing 32 bits of data
  - 16  0x10000
  - 17  0x20000
  - 18  0x40000
  - 19  0x80000 Enable Paging                   enableing memory paging

- CR1:            16   bit control register
  - 0   0x0001 Point to BIOS ROM                *readonly*
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

- F:              20: bit (F)lags register
  - 0x00001 zero
  - 0x00002 equals
  - 0x00004 signed
  - 0x00008 carry
  - 0x00010 overflow
  - 0x00020 less
  - 0x00040 interrupt enable
  - 0x00080 HALT
  - 0x00100
  - 0x00200
  - 0x00400 enable virtual BCG8 mode
  - 0x00800
  - 0x01000
  - 0x02000
  - 0x04000
  - 0x08000
  - 0x10000
  - 0x20000
  - 0x40000
  - 0x80000
