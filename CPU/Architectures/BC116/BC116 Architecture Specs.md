# CPU Specifications

- [CPU Specifications](#cpu-specifications)
  - [Architecture overview](#architecture-overview)
  - [CPUs](#cpus)
    - [BC16](#bc16)
    - [BC16C](#bc16c)
    - [BC16CE](#bc16ce)
    - [BC1602C](#bc1602c)
    - [Note](#note)
  - [INSTRUCTIONS](#instructions)
    - [INSTRUCTION SET](#instruction-set)
    - [INSTRUCTION LAYOUT](#instruction-layout)
    - [ARGUMENT MODE](#argument-mode)
  - [Interrupt vector table](#interrupt-vector-table)
    - [Layout](#layout)
    - [Interrupt vector assignments](#interrupt-vector-assignments)
      - [interrupt](#interrupt)
      - [exception](#exception)
  - [Caches](#caches)
  - [Floating values](#floating-values)
    - [Floating-Point Registers](#floating-point-registers)
  - [Virtual mode](#virtual-mode)
  - [Extended mode](#extended-mode)
  - [Protected mode](#protected-mode)
  - [Memory](#memory)
    - [Memory layout](#memory-layout)
  - [The Stack](#the-stack)
  - [REGISTERS](#registers)
    - [Extended registers](#extended-registers)
    - [Bigger registers](#bigger-registers)

## Architecture overview

- Over all Architecture: BCG Architecture
- Name: BC116
- Base data bus: 16 bits
- Base address bus: 16 bits

## CPUs

- BC16          Base 16 bit CPU starts in [virtual mode](#virtual-mode)
- BC16C         Can enable `extended mode` in the CR0 register
- BC16CE        Can enable bigger registers [more here](#bigger-registers)
- BC1602C       With all the other things before but starts in [extended mode](#extended-mode)
- BC16F         The BC16 CPU with in built float registers and functions
- BC16CF        The BC16C CPU with in built float registers and functions
- BC16CEF       The BC16CE CPU with in built float registers and functions
- BC1602CF      The BC1602C CPU with in built float registers and functions

### BC16

- Starts in [virtual mode](#virtual-mode)

### BC16C

- The base BC16 CPU
- Starts in [virtual mode](#virtual-mode)
- Can enable [extended mode](#extended-mode) in CR0

### BC16CE

- The base BC16 CPU
- Starts in [virtual mode](#virtual-mode)
- Can enable [extended mode](#extended-mode) in CR0
- Can enable bigger registers [more here](#bigger-registers)

### BC1602C

- The base BC16 CPU
- Starts in [extended mode](#extended-mode)
- Can enable bigger registers [more here](#bigger-registers)

### Note

if the CPU name has a `F` in its name it has builtin float functions

## INSTRUCTIONS

### INSTRUCTION SET

### INSTRUCTION LAYOUT

XXXXXXXX_XXXXUUUU_AAAAAAAA_BBBBBBBB

- U = Unused
- X = instruction code
- A = argument 1
- B = argument 2

### ARGUMENT MODE

- 0x00: immediate byte              number
- 0x01: immediate word              number
- 0x02: immediate tbyte:            number
- 0x03: immediate dword:            number
- 0x04: immediate qword:            number
- 0x08: immediate_float:            numberf
- 0x10: register                    register
- 0x11: register AL:                AL
- 0x12: register BL:                BL
- 0x13: register CL:                CL
- 0x14: register DL:                DL
- 0x15: register H:                 H
- 0x16: register L:                 L
- 0x17: register A                  A
- 0x18: register B                  B
- 0x19: register C                  C
- 0x1A: register D                  D
- 0x20: register AX:                AX
- 0x21: register BX:                BX
- 0x22: register CX:                CX
- 0x23: register DX:                DX
- 0x20: register address:           address [register]
- 0x21: address register HL:        [HL]
- 0x30: Relative address:           [byte address]      an 8 bit offset to the PC
- 0x31: Near address:               Near [address]      a 8 bit address
- 0x32: Short address:              short [address]     a 16 bit address
- 0x33: long address:               long [address]      a 24 bit address
- 0x34: Far address:                far [address]       a 32 bit address
- 0x36: Short X indexed address:    [short address],X
- 0x37: Short Y indexed address:    [short address],Y
- 0x38: SP relative address short:  [SP + short number]
- 0x39: BP relative address short:  [BP + short number]
- 0x3A: 32 bit segment address:     [register:register]
- 0x3B: 32 bit segment DS register: [DS:register]
- 0x3C: 32 bit segment DS B:        [DS:B]
- 0x3D: 32 bit segment ES register: [ES:register]
- 0x3E: 32 bit segment ES B:        [ES:B]
- 0x60: register AF:                AF
- 0x61: register BF:                BF
- 0x62: register CF:                CF
- 0x63: register DF:                DF

## Interrupt vector table

The Interrupt vector table is 4 KB in size

### Layout

The first word is the Address Segment for the routine

The second word is the Address offset for the routine

### Interrupt vector assignments

[Here](../BCG%20arch%20Specs.md#interrupt-entres)

#### interrupt

an interrupt can be caused by the IRQ or NMI pins or the INT instruction

#### exception

an exception can be caused an instruction

when the BC116 Architecture gets an Abort exception it will stop and the Halt flag will be set

when the BC116 Architecture gets an Fault exception it will skip that instruction and continue

## Caches

## Floating values

The BC232 architecture supports floating-point operations through specialized registers and instructions. The floating-point unit (FPU) can handle both single-precision and double-precision values. Below are the key components and capabilities related to floating-point operations.

### Floating-Point Registers

The architecture provides several dedicated floating-point registers:

- AF, BF: 32-bit single-precision floating-point registers.

## Virtual mode

In virtual mode the CPU has [here](../BCG%20arch%20Specs.md#virtual-mode)

## Extended mode

in Extended mode the CPU will get [here](../BCG%20arch%20Specs.md#extended-mode)

## Protected mode

in Protected mode the CPU will get [here](../BCG%20arch%20Specs.md#protected-mode)

## Memory

### Memory layout

- Address bus: 16 bits to a max of 24 bits

|Base Address |Size       |Name                     |Description
|-------------|-----------|-------------------------|-
|`0x0000_0000`|`0x00_1000`| Interrupt vector table  | Interrupt vector table more [here](#interrupt-vector-table)
|`0x0000_1000`|`0x00_0200`| IO ports                | this is where the Ports is at
|`0x0000_1200`|`0x00_EE00`| RAM                     | RAM in the first segment
|`0x0001_0000`|`0x02_0000`| VRAM                    | video ram
|`0x0003_0000`|`0xFD_0000`| RAM                     | RAM

the end is `0x100_0000`

## The Stack

The Stack is a 64 KB section set using the [Stack Segment register](#registers) and the SP(Stack pointer) as the offset, you can push data onto the stack using [PUSH](./instructions%20BC116.md) and pop data off the stack using POP, **the stack grows down**

## REGISTERS

- A (AH + AL):    16  bit general purpose register
- B (BH + BL):    16  bit general purpose register
- C (CH + CL):    16  bit general purpose register
- D (DH + DL):    16  bit general purpose register

- CS:             16  bit code segment register
- DS:             16  bit data segment register
- ES:             16  bit extra data segment register
- FS:             16  bit extra data segment register
- SS:             16  bit stack segment register

- PC:             16  bit program counter

- HL (H + L):     32  bit general purpose address register
- H:              16  bit general purpose address register
- L:              16  bit general purpose address register

- BP:             16  bit Stack register
- SP:             16  bit Stack register

- AF:             32  bit float register
- BF:             32  bit float register

- R1..16:         16  bit general purpose register

- CR0:            8   bit control register
  - 0 0x01 Boot mode
  - 1 0x02 FPU enabled
  - 2 0x04 Low power Mode
  - 3 0x08 Enable bigger registers        Enableing [bigger registers](#bigger-registers)
  - 4 0x10 Enable extended mode           Enableing [extended mode](#extended-mode)
  - 5 0x20
  - 6 0x40
  - 7 0x80 Enable Protected mode          Enableing [Protected mode](#protected-mode)

- F:              16: bit (F)lags register
  - 0x0001 zero
  - 0x0002 equals
  - 0x0004 signed
  - 0x0008 carry
  - 0x0010 overflow
  - 0x0020 less
  - 0x0040 interrupt enable
  - 0x0080 HALT
  - 0x0100 reserved
  - 0x0200 under flow
  - 0x0400 shift flag
  - 0x0800 greater
  - 0x1000 reserved
  - 0x2000 reserved
  - 0x4000 reserved
  - 0x8000 reserved

### Extended registers

- PC:             the PC is now 32 bits can only use 24 bits

- X:              16 bit index register
- Y:              16 bit index register

- AX              32 bit general purpose register
- BX              32 bit general purpose register
- CX              32 bit general purpose register
- DX              32 bit general purpose register

- R1..16:         32 bit general purpose register

### Bigger registers

- CF:             32  bit float register
- DF:             32  bit float register
