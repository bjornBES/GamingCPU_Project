# CPU Specifications

- [CPU Specifications](#cpu-specifications)
  - [Architecture overview](#architecture-overview)
  - [CPUs](#cpus)
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
  - [MEMORY](#memory)
    - [MEMORY LAYOUT](#memory-layout)
  - [REGISTERS](#registers)
    - [Extended registers](#extended-registers)

## Architecture overview

- Over all Architecture: BCG Architecture
- Name: BC018 Architecture
- Base data bus: 8 bits
- Base address bus: 16 bits

## CPUs

- BC8           Base 8 bit CPU
- BC816         With a 20 bit address bus
- BC810680      With some [extended registers](#extended-registers)

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
- 0x50: Relative address            [byte address]      an 8 bit offset to the PC
- 0x51: Near address                Near [address]      a 8 bit address
- 0x52: Short address               short [address]     a 16 bit address
- 0x53: long address                long [address]      a 24 bit address
- 0x58: SP relative address         [SP - number]/[SP + number]
- 0x59: BP relative address         [BP - number]/[BP + number]
- 0x5A: 32 bit segment address      [register:register]
- 0x5B: 32 bit segment DS register  [DS:register]
- 0x5C: 32 bit segment DS B         [DS:B]

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
|User defined IRQ interrupt |16-31          |15   |interrupt        |`0x0064`-`0x0124`|true           |-
|User defined interrupts    |32-255         |223  |interrupt        |`0x0080`-`0x03FC`|true           |INT 0x04
|INT 0x10 interrupts        |256-281        |25   |interrupt        |`0x0400`-`0x0464`|true           |INT 0x0F

#### interrupt

an interrupt can be caused by the IRQ or NMI pins or the INT instruction

#### exception

an exception can be caused an instruction

when the BCG16 gets an Abort exception it will stop and the Halt flag will be set

when the BCG16 gets an Fault exception it will skip that instruction and continue

## Caches

## MEMORY

### MEMORY LAYOUT

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

the end is `0xFFFFFF`/`0x1000000`

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

### Extended registers

- A (AH + AL):    16 bit general purpose register
- B (BH + BL):    16 bit general purpose register
- C (CH + CL):    16 bit general purpose register
- D (DH + DL):    16 bit general purpose register

- PC:             24 bit program counter
