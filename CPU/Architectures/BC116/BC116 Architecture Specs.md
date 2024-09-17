# CPU Specifications

- [CPU Specifications](#cpu-specifications)
  - [Architecture overview](#architecture-overview)
  - [CPUs](#cpus)
    - [INFO](#info)
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
  - [Virtual mode](#virtual-mode)
  - [Extended mode](#extended-mode)
  - [Memory](#memory)
    - [Memory layout](#memory-layout)
  - [REGISTERS](#registers)
    - [Extended registers](#extended-registers)

## Architecture overview

- Over all Architecture: BCG Architecture
- Name: BC116
- Base data bus: 16 bits
- Base address bus: 16 bits

## CPUs

- BC16          Base 16 bit CPU
- BC16C0        Can enable `extended mode` in the CR0 register
- BC16C0G       With in built float registers and functions
- BC16C0GX      With

### INFO

all CPUs here will start in [virtual mode](#virtual-mode)

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
- 0x02: immediate tbyte             number
- 0x03: immediate dword             number
- 0x04: immediate_float             numberf
- 0x10: register                    register
- 0x11: register address            address [register]
- 0x18: register A                  A
- 0x19: register B                  B
- 0x1A: register C                  C
- 0x1B: register D                  D
- 0x1C: register H                  H
- 0x1D: register L                  L
- 0x1E: address register HL         [HL]
- 0x1F: register MB                 MB
- 0x30: register AX                 AX
- 0x31: register BX                 BX
- 0x32: register CX                 CX
- 0x33: register DX                 DX
- 0x50: Relative address            [byte address]      an 8 bit offset to the PC
- 0x51: Near address                Near [address]      a 8 bit address
- 0x52: Short address               short [address]     a 16 bit address
- 0x53: long address                long [address]      a 24 bit address
- 0x54: Far address                 far [address]       an 32 bit address
- 0x58: SP relative address         [SP - number]/[SP + number]
- 0x59: BP relative address         [BP - number]/[BP + number]
- 0x5A: 32 bit segment address      [register:register]
- 0x5B: 32 bit segment DS register  [DS:register]
- 0x5C: 32 bit segment DS B         [DS:B]
- 0x5D: 32 bit segment ES register  [ES:register]

## Interrupt vector table

The Interrupt vector table is 4 KB in size

### Layout

The first word is the Address Segment for the routine

The second word is the Address offset for the routine

### Interrupt vector assignments

|Function                   |Interupt number|Size |Type             |IVT address      |Is User defined|Related instructions
|---------------------------|---------------|-----|-----------------|-----------------|---------------|-
|Divide error               |0              |1    |Fault exception  |`0x0000`         |true           |DIV
|NMI interrupt              |1              |1    |interrupt        |`0x0004`         |true           |INT 2 or NMI pin
|BRK interrupt              |2              |1    |interrupt        |`0x0008`         |true           |BRK
|invalid opcode             |6              |1    |Abort exception  |`0x0018`         |false          |Any undefined opcode
|User defined IRQ interrupt |16-31          |15   |interrupt        |`0x0064`-`0x0124`|true           |The IRQ pin
|User defined interrupts    |32             |1    |interrupt        |`0x0080`         |true           |INT 0x03
|User defined interrupts    |33             |1    |interrupt        |`0x0084`         |true           |INT 0x04
|User defined interrupts    |34             |1    |interrupt        |`0x0088`         |true           |INT 0x05
|User defined interrupts    |35             |1    |interrupt        |`0x008C`         |true           |INT 0x06
|User defined interrupts    |36             |1    |interrupt        |`0x0090`         |true           |INT 0x07
|User defined interrupts    |37             |1    |interrupt        |`0x0094`         |true           |INT 0x10
|reserved                   |38-255         |217  |interrupt        |`0x0098`-`0x03FC`|true           |DO NOT USE

#### interrupt

an interrupt can be caused by the IRQ or NMI pins or the INT instruction

#### exception

an exception can be caused an instruction

when the BC116 Architecture gets an Abort exception it will stop and the Halt flag will be set

when the BC116 Architecture gets an Fault exception it will skip that instruction and continue

## Caches

## Floating values

## Virtual mode

In virtual mode the CPU has

- 16 bit adderss bus
- 16 bit data bus
- a 2 KB [IVT](#interrupt-vector-table)
- And only has the base registers

## Extended mode

in Extended mode the CPU will get

- 24 bit address bus
- 16 bit data bus
- a 4 KB [IVT](#interrupt-vector-table)
- Make use of the extended registers [List here](#extended-registers)

## Memory

### Memory layout

- Address bus: 16 bits to a max of 24 bits

|Base Address |Size       |Name                   |Description
|-------------|-----------|-----------------------|-
|`0x0000_0000`|`0x00_1000`| Interrupt vector table| Interrupt vector table more [here](#interrupt-vector-table)
|`0x0000_1000`|`0x00_0200`| IO ports              | this is where the Ports is at
|`0x0000_1200`|`0x00_EE00`| RAM                   | RAM in the first segment
|`0x0001_0000`|`0x02_8000`| RAM                   | RAM
|`0x0003_8000`|`0x00_8000`| Char set              | Char set
|`0x0004_0000`|`0x04_0000`| VRAM                  | video ram
|`0x0008_0000`|`0x08_0000`| RAM Banked            | this is the data/prgram is at but banked
|`0x0010_0000`|`0xF0_0000`| RAM                   | this is the data/prgram is at

the end is `0xFFFFFF`/`0x1000000`

## REGISTERS

- A (AH + AL):    16 bit general purpose register
- B (BH + BL):    16 bit general purpose register
- C (CH + CL):    16 bit general purpose register
- D (DH + DL):    16 bit general purpose register

- CS:             16  bit code segment register
- DS:             16  bit data segment register
- ES:             16  bit extra data segment register
- FS:             16  bit extra data segment register
- SS:             16  bit stack segment register

- PC:             24  bit program counter

- HL (H + L):     32 bit general purpose address register
- H:              16 bit general purpose address register
- L:              16 bit general purpose address register

- BP:             16  bit Stack register
- SP:             16  bit Stack register

- AF:             32  bit float register
- BF:             32  bit float register

- R1:             16  bit temp register
- R2:             16  bit temp register
- R3:             16  bit temp register
- R4:             16  bit temp register
- R5:             16  bit temp register
- R6:             16  bit temp register

- MB:             8 bit bank register

- CR0:            8   bit control register
  - 0 0x01
  - 1 0x02
  - 2 0x04
  - 3 0x08
  - 4 0x10 Enable extended mode            enableing [extended mode](#extended-mode)
  - 5 0x20
  - 6 0x40
  - 7 0x80

- CR1:            8   bit control register
  - 0 0x01
  - 1 0x02
  - 2 0x04
  - 3 0x08
  - 4 0x10
  - 5 0x20
  - 6 0x40
  - 7 0x80

- F:              16: bit (F)lags register
  - 0x0001 zero
  - 0x0002 equals
  - 0x0004 signed
  - 0x0008 carry
  - 0x0010 overflow
  - 0x0020 less
  - 0x0040 interrupt enable
  - 0x0080 HALT
  - 0x0100
  - 0x0200
  - 0x0400
  - 0x0800
  - 0x1000
  - 0x2000
  - 0x4000
  - 0x8000

### Extended registers

- AX              32 bit general purpose register
- BX              32 bit general purpose register
- CX              32 bit general purpose register
- DX              32 bit general purpose register
