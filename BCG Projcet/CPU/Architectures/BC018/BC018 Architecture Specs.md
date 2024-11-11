# CPU Specifications

- [CPU Specifications](#cpu-specifications)
  - [Architecture overview](#architecture-overview)
  - [CPUs](#cpus)
  - [INSTRUCTIONS](#instructions)
    - [INSTRUCTION SET](#instruction-set)
    - [INSTRUCTION LAYOUT](#instruction-layout)
    - [ARGUMENT MODE](#argument-mode)
  - [Interrupt vector table](#interrupt-vector-table)
  - [Caches](#caches)
  - [MEMORY](#memory)
    - [MEMORY LAYOUT](#memory-layout)
  - [REGISTERS](#registers)

## Architecture overview

- Over all Architecture: BCG Architecture
- Name: BC018 Architecture
- Base data bus: 8 bits
- Base address bus: 20 bits

## CPUs

- BC8           Base 8 bit CPU

## INSTRUCTIONS

### INSTRUCTION SET

### INSTRUCTION LAYOUT

XXXXXXXX_XXXXXXXX_AAAAAAAA_BBBBBBBB

- U = Unused
- X = instruction code
- A = argument 1
- B = argument 2

### ARGUMENT MODE

- 0x00: immediate byte              number
- 0x01: immediate word              number
- 0x10: register                    register
- 0x11: register AL:                AL
- 0x12: register BL:                BL
- 0x13: register CL:                CL
- 0x14: register DL:                DL
- 0x15: register H:                 H
- 0x16: register L:                 L
- 0x20: register address:           address [register]
- 0x21: address register HL:        [HL]
- 0x30: Relative address:           [byte address]      an 8 bit offset to the PC
- 0x31: Near address:               Near [address]      a 8 bit address
- 0x32: Short address:              short [address]     a 16 bit address
- 0x38: SP relative address byte:   [SP + sbyte number]
- 0x39: BP relative address byte:   [BP + sbyte number]
- 0x3A: 32 bit segment address:     [register:register]
- 0x3B: 32 bit segment DS register: [DS:register]
- 0x3C: 32 bit segment DS B:        [DS:B]s

## Interrupt vector table

[Here](../BCG%20arch%20Specs.md#interrupt-entres)

## Caches

## MEMORY

### MEMORY LAYOUT

- Address bus: 16 bits to a max of 20 bits

|Base Address |Size       |Name                     |Description
|-------------|-----------|-------------------------|-
|`0x0000_0000`|`0x00_1000`| Interrupt vector table  | Interrupt vector table more [here](#interrupt-vector-table)
|`0x0000_1000`|`0x00_0200`| IO ports                | this is where the Ports is at
|`0x0000_1200`|`0x00_EE00`| RAM                     | RAM in the first segment
|`0x0001_0000`|`0x01_0000`| VRAM                    | video ram
|`0x0002_0000`|`0x08_0000`| RAM Banked              | this is the data/prgram is at but banked
|`0x000A_0000`|`0x06_0000`| RAM                     | RAM

the end is `0x10_0000`

## REGISTERS

- AL:             8   bit general purpose register
- AH:
- BL:             8   bit general purpose register
- BH:
- CL:             8   bit general purpose register
- CH:
- DL:             8   bit general purpose register
- DH:

- CS:             16  bit code segment register
- DS:             16  bit data segment register
- SS:             16  bit stack segment register

- PC:             16  bit program counter

- HL (H + L):     32  bit general purpose address register
- H:              16  bit general purpose address register
- L:              16  bit general purpose address register

- BP:             16  bit Stack register
- SP:             16  bit Stack register

- R1:             16  bit temp register
- R2:             16  bit temp register
- R3:             16  bit temp register
- R4:             16  bit temp register

- MB:             8   bit bank register

- F:              8:  bit (F)lags register
  - 0x0001 zero
  - 0x0002 equals
  - 0x0004 signed
  - 0x0008 carry
  - 0x0010 overflow
  - 0x0020 less
  - 0x0040 interrupt enable
  - 0x0080 HALT
