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
    - [Floating-Point Registers](#floating-point-registers)
  - [Virtual mode](#virtual-mode)
  - [Extended mode](#extended-mode)
  - [Protected mode](#protected-mode)
  - [Memory](#memory)
    - [Memory layout](#memory-layout)
  - [REGISTERS](#registers)
    - [Extended registers](#extended-registers)

## Architecture overview

- Over all Architecture: BCG Architecture
- Name: BC224
- Base data bus: 24 bits
- Base address bus: 24 bits

## CPUs

- BC24          Base 24 bit CPU
- BC24F         With in built float registers and functions

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

- 0x00: immediate byte:             number
- 0x01: immediate word:             number
- 0x02: immediate tbyte:            number
- 0x03: immediate dword:            number
- 0x04: immediate_float:            numberf
- 0x0A: register:                   register
- 0x0B: register address:           address [register]
- 0x10: register A:                 A
- 0x11: register B:                 B
- 0x12: register C:                 C
- 0x13: register D:                 D
- 0x14: register H:                 H
- 0x15: register L:                 L
- 0x16: address register HL:        [HL]
- 0x17: register MB:                MB
- 0x20: register AX:                AX
- 0x21: register BX:                BX
- 0x22: register CX:                CX
- 0x23: register DX:                DX
- 0x50: Relative address:           [byte address]      an 8 bit offset to the PC
- 0x51: Near address:               Near [address]      a 8 bit address
- 0x52: Short address:              short [address]     a 16 bit address
- 0x53: long address:               long [address]      a 24 bit address
- 0x54: Far address:                far [address]       a 32 bit address
- 0x55: Short X indexed address:    [short address],X
- 0x56: Short Y indexed address:    [short address],Y
- 0x60: SP relative address byte:   [SP + sbyte number]
- 0x61: BP relative address byte:   [BP + sbyte number]
- 0x62: 32 bit segment address:     [register:register]
- 0x63: 32 bit segment DS register: [DS:register]
- 0x64: 32 bit segment DS B:        [DS:B]
- 0x65: 32 bit segment ES register: [ES:register]
- 0x66: 32 bit segment ES B:        [ES:B]
- 0x70: register AF:                AF
- 0x71: register BF:                BF

## Interrupt vector table

The Interrupt vector table is 4 KB in size

### Layout

The first word is the Address Segment for the routine

The second word is the Address offset for the routine

### Interrupt vector assignments

|Function                   |Interupt number|Type             |Is User defined|Related instructions
|---------------------------|---------------|-----------------|---------------|-
|Divide error               |0              |Fault exception  |true           |DIV or DIVF
|NMI interrupt              |1              |interrupt        |true           |INT 2 or NMI pin
|BRK interrupt              |2              |interrupt        |true           |BRK
|invalid opcode             |6              |Abort exception  |false          |Any undefined opcode
|Keyboard IRQ               |16             |interrupt        |false          |Keyboard IRQ0
|User defined IRQ interrupt |17-30          |interrupt        |true           |The IRQ pin
|FDC IRQ                    |31             |interrupt        |true           |FDC IRQ15
|reserved                   |32             |interrupt        |false          |Unused
|User defined interrupts    |33             |interrupt        |true           |INT 0x04
|User defined interrupts    |34             |interrupt        |true           |INT 0x05
|User defined interrupts    |35             |interrupt        |true           |INT 0x06
|User defined interrupts    |36             |interrupt        |true           |INT 0x10
|User defined interrupts    |37             |interrupt        |true           |INT 0x13
|Stack overflow             |38             |Abort exception  |true           |If the stack overflows
|reserved                   |39-255         |interrupt        |true           |DO NOT USE

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

In virtual mode the CPU has

- 16 bit adderss bus
- 16 bit data bus
- a 2 KB [IVT](#interrupt-vector-table)
- And only has the base registers like in the BC116 Architecture

## Extended mode

in Extended mode the CPU will get

- 24 bit address bus
- 16 bit data bus
- a 4 KB [IVT](#interrupt-vector-table)
- Make use of the extended registers [List here](#extended-registers)
- Long and Far address argument modes
- PC becomes a 24 bit register

## Protected mode

in Protected mode the CPU will get

- 28 bit address bus

## Memory

### Memory layout

- Address bus: 16 bits to a max of 28 bits

|Base Address |Size         |Name                     |Description
|-------------|-------------|-------------------------|-
|`0x0000_0000`|`0x0000_2000`| Interrupt vector table  | Interrupt vector table more [here](#interrupt-vector-table)
|`0x0000_2200`|`0x0000_0200`| IO ports                | this is where the Ports is at
|`0x0000_2400`|`0x0000_DC00`| RAM                     | RAM in the first segment
|`0x0001_0000`|`0x0002_0000`| VRAM                    | video ram
|`0x0003_0000`|`0x0008_0000`| RAM Banked              | this is the data/prgram is at but banked
|`0x000B_0000`|`0x0FF5_0000`| RAM                     | RAM

the end is `0x1000_0000`

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

- MB:             16  bit bank register

- CR0:            8   bit control register
  - 0 0x01 Boot mode
  - 1 0x02 FPU enabled
  - 2 0x04 Low power Mode
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

- F:              24: bit (F)lags register
  - 0x000001 zero
  - 0x000002 equals
  - 0x000004 signed
  - 0x000008 carry
  - 0x000010 overflow
  - 0x000020 less
  - 0x000040 interrupt enable
  - 0x000080 HALT
  - 0x000100 emulate BC8
  - 0x000200 under flow
  - 0x000400
  - 0x000800
  - 0x001000
  - 0x002000
  - 0x004000
  - 0x008000
  - 0x010000
  - 0x020000
  - 0x040000
  - 0x080000
  - 0x100000
  - 0x200000
  - 0x400000
  - 0x800000

### Extended registers

- X:              16 bit index register
- Y:              16 bit index register

- AX              32 bit general purpose register
- BX              32 bit general purpose register
- CX              32 bit general purpose register
- DX              32 bit general purpose register
