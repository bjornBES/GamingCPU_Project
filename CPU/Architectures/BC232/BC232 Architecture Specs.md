# CPU Specifications

- [CPU Specifications](#cpu-specifications)
  - [Architecture overview](#architecture-overview)
  - [CPUs](#cpus)
  - [INSTRUCTIONS](#instructions)
    - [INSTRUCTION SET](#instruction-set)
    - [INSTRUCTION LAYOUT](#instruction-layout)
    - [ARGUMENT MODE](#argument-mode)
  - [Interrupt descriptor table](#interrupt-descriptor-table)
    - [Layout](#layout)
      - [privilege level](#privilege-level)
      - [Gate type](#gate-type)
    - [Interrupt descriptor table entres](#interrupt-descriptor-table-entres)
      - [interrupt](#interrupt)
      - [exception](#exception)
  - [Global descriptor table](#global-descriptor-table)
    - [Layout](#layout-1)
      - [Flags](#flags)
  - [Caches](#caches)
  - [Floating values](#floating-values)
  - [Virtual mode](#virtual-mode)
  - [Extended mode](#extended-mode)
  - [Protected mode](#protected-mode)
  - [Memory](#memory)
    - [Memory Management unit (MMU)](#memory-management-unit-mmu)
    - [Memory layout](#memory-layout)
  - [REGISTERS](#registers)
    - [Extended registers](#extended-registers)
    - [Protected registers](#protected-registers)

## Architecture overview

- Over all Architecture: BCG architecture
- Name: BC232
- Base data bus: 32 bits
- Base address bus: 24 bits

## CPUs

- BC32          Base 32 bit CPU
- BC32X         Starts in Protected mode

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
- 0x05: immediate tword             number
- 0x06: immediate_double            double numberf
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

## Interrupt descriptor table

The Interrupt descriptor table is 16 KB in size

### Layout

|Offset |Size   |Name                     |Description
|-------|-      |-                        |-
|`0x00` |`0x32` |Offset                   | this is the offset to the interrupt function
|`0x32` |`0x16` |Segment                  | this is the segment to the interrupt function
|`0x48` |`0x01` |CPU Privilege level      | this is the privilege level [more here](#privilege-level)
|`0x49` |`0x01` |Gate type                | this is the Gate type [more here](#gate-type)
|`0x4A` |`0x14` |reserved                 | reserved NOT IN USE

#### privilege level

#### Gate type

### Interrupt descriptor table entres

|Function                   |Interupt number|Type             |Is User defined|Related instructions
|---------------------------|---------------|-----------------|---------------|-
|Divide error               |0              |Fault exception  |true           |DIV
|NMI interrupt              |1              |interrupt        |true           |INT 2 or NMI pin
|BRK interrupt              |2              |interrupt        |true           |BRK
|invalid opcode             |6              |Abort exception  |false          |Any undefined opcode
|User defined IRQ interrupt |16-31          |interrupt        |true           |The IRQ pin
|User defined interrupts    |32             |interrupt        |true           |INT 0x03
|User defined interrupts    |33             |interrupt        |true           |INT 0x04
|User defined interrupts    |34             |interrupt        |true           |INT 0x05
|User defined interrupts    |35             |interrupt        |true           |INT 0x06
|User defined interrupts    |36             |interrupt        |true           |INT 0x07
|User defined interrupts    |37             |interrupt        |true           |INT 0x10
|Stack overflow             |38             |Abort exception  |false          |If the stack overflows
|reserved                   |39-255         |interrupt        |true           |DO NOT USE

#### interrupt

an interrupt can be caused by the IRQ or NMI pins or the INT instruction

#### exception

an exception can be caused an instruction

when the BC232 Architecture gets an Abort exception it will stop and the Halt flag will be set

when the BC232 Architecture gets an Fault exception it will skip that instruction and continue

## Global descriptor table

### Layout

|Offset |Size   |Name                     |Description
|-------|-      |-                        |-
|`00`   |`32`   |Base address             | this is the Base address to the interrupt function
|`32`   |`20`   |Segment limit            | this is the segment limit to the interrupt function
|`52`   |`01`   |Flags                    | [More here](#flags)
|`53`   |`11`   |reserved                 | reserved NOT IN USE

#### Flags

|Offset |Size   |Name                     |Description
|-------|-      |-                        |-
|`0`    |`01`   |Granularity              | this is the granularity bit if 1 the Segment limit is specified in 4k blocks
|`1`    |`01`   |Executable               | if 0 the entry is a Data segment and 1 it's a Code segment
|`2`    |`01`   |Read write               | if Executable bit is 0 then if this is 1 it is Read only and 1 read and write
|`3`    |`02`   |Gate type                | this is the Gate type [more here](#gate-type)

## Caches

## Floating values

## Virtual mode

In virtual mode the CPU has

- 16 bit adderss bus
- 16 bit data bus
- a 2 KB IVT
- And only has the base registers

## Extended mode

in Extended mode the CPU will get

- 24 bit address bus
- 16 bit data bus
- a 4 KB IVT
- Make use of the extended registers [List here](#extended-registers)

## Protected mode

in Extended mode the CPU will get

- 32 bit address bus
- 32 bit data bus
- a 16 KB [IDT](#interrupt-descriptor-table)
- Make use of the protected registers [List here](#protected-registers)

## Memory

### Memory Management unit (MMU)

### Memory layout

- Address bus: 16 bits to a max of 32 bits

|Base Address |Size         |Name                       |Description
|-------------|-------------|---------------------------|-
|`0x0000_0000`|`0x0000_4000`| Interrupt descriptor table| Interrupt descriptor table more [here](#interrupt-descriptor-table)
|`0x0000_4000`|`0x0000_0200`| global descriptor table   | global descriptor table more [here](#global-descriptor-table)
|`0x0000_4200`|`0x0000_0200`| IO ports                  | this is where the Ports is at
|`0x0000_4400`|`0x0000_BC00`| RAM                       | RAM in the first segment
|`0x0001_0000`|`0x0002_8000`| RAM                       | RAM
|`0x0003_8000`|`0x0000_8000`| Char set                  | Char set
|`0x0004_0000`|`0x0004_0000`| VRAM                      | video ram
|`0x0008_0000`|`0x0008_0000`| RAM Banked                | this is the data/prgram is at but banked
|`0x0010_0000`|`0x00F0_0000`| RAM                       | this is the data/prgram is at
|`0x0100_0000`|`0xFF00_0000`| RAM                       | this is the data/prgram is at

the end is `0xFFFFFF`/`0x1000000`

## REGISTERS

- AX              32 bit general purpose register
- BX              32 bit general purpose register
- CX              32 bit general purpose register
- DX              32 bit general purpose register

- CS:             16  bit code segment register
- DS:             16  bit data segment register
- ES:             16  bit extra data segment register
- FS:             16  bit extra data segment register
- GS:             16  bit extra data segment register
- SS:             16  bit stack segment register

- PC:             32  bit program counter

- HL (H + L):     32 bit general purpose address register
- H:              16 bit general purpose address register
- L:              16 bit general purpose address register

- BPX:            32  bit Stack register
- SPX:            32  bit Stack register

- AF:             32  bit float register
- BF:             32  bit float register
- CF:             32  bit float register
- DF:             32  bit float register

- R1:             16  bit temp register
- R2:             16  bit temp register
- R3:             16  bit temp register
- R4:             16  bit temp register
- R5:             16  bit temp register
- R6:             16  bit temp register
- R7:             16  bit temp register
- R8:             16  bit temp register

- MB:             16  bit bank register

- CR0:            16  bit control register
  - 0x0001
  - 0x0002
  - 0x0004
  - 0x0008
  - 0x0010 Enable extended mode             Enableing [extended mode](#extended-mode)
  - 0x0020
  - 0x0040
  - 0x0080
  - 0x0100 Enable Protected mode            Enableing [Protected mode](#protected-mode)
  - 0x0200
  - 0x0400
  - 0x0800
  - 0x1000
  - 0x2000
  - 0x4000
  - 0x8000

- CR1:            16   bit control register
  - 0x0001
  - 0x0002
  - 0x0004
  - 0x0008
  - 0x0010
  - 0x0020
  - 0x0040
  - 0x0080
  - 0x0100
  - 0x0200
  - 0x0400
  - 0x0800
  - 0x1000
  - 0x2000
  - 0x4000
  - 0x8000

- F:              24: bit (F)lags register
  - 0x000001 zero
  - 0x000002 equals
  - 0x000004 signed
  - 0x000008 carry
  - 0x000010 overflow
  - 0x000020 less
  - 0x000040 interrupt enable
  - 0x000080 HALT
  - 0x000100
  - 0x000200
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

### Protected registers
