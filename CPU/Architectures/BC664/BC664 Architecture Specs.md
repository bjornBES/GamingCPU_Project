# CPU Specifications

- [CPU Specifications](#cpu-specifications)
  - [Architecture overview](#architecture-overview)
  - [CPUs](#cpus)
    - [BC64](#bc64)
    - [Note](#note)
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
  - [Caches](#caches)
  - [Floating values](#floating-values)
    - [Floating-Point Registers](#floating-point-registers)
  - [Virtual mode](#virtual-mode)
  - [Extended mode](#extended-mode)
  - [Protected mode](#protected-mode)
  - [Long mode](#long-mode)
  - [Long long mode](#long-long-mode)
  - [Memory](#memory)
    - [Memory layout](#memory-layout)
  - [The Stack](#the-stack)
  - [REGISTERS](#registers)

## Architecture overview

- Over all Architecture: BCG architecture
- Name: BC664
- Base data bus: 64 bits
- Base address bus: 40 bits

## CPUs

- BC64          Base 64 bit CPU
- BC6403        Starts in long mode
- BC64F         The BC64 CPU with in built float registers and functions
- BC6403F       The BC6403 CPU with in built float registers and functions
- BC6403FD      The BC6403 CPU with in built float and double registers and functions

### BC64

- Starts in [virtual mode](#virtual-mode)
- Can enable [extended mode](#extended-mode) in CR0
- Can enable [long mode](#long-mode) in CR0 after [extended mode](#extended-mode) has been enabled
- Can enable [protected mode](#protected-mode) in CR0 after [long mode](#long-mode) has been enabled
- Can enable [long long mode](#long-long-mode) if [long mode](#long-mode) and [protected mode](#protected-mode) are enabled

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
- 0x05: immediate dqword:           number
- 0x08: immediate_float:            numberf
- 0x09: immediate_double:           double numberf
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
- 0x24: register EX:                EX
- 0x25: register FX:                FX
- 0x26: register GX:                GX
- 0x27: register HX:                HX
- 0x20: register address:           address [register]
- 0x21: address register HL:        [HL]
- 0x30: Relative address:           [byte address]      an 8 bit offset to the PC
- 0x31: Near address:               Near [address]      a 8 bit address
- 0x32: Short address:              short [address]     a 16 bit address
- 0x33: long address:               long [address]      a 24 bit address
- 0x34: Far address:                far [address]       a 32 bit address
- 0x36: Short X indexed address:    [short address],X
- 0x37: Short Y indexed address:    [short address],Y
- 0x38: SPX relative address int:   [SPX + int number]
- 0x39: BPX relative address int:   [BPX + int number]
- 0x3A: 32 bit segment address:     [register:register]
- 0x3B: 32 bit segment DS register: [DS:register]
- 0x3C: 32 bit segment DS B:        [DS:B]
- 0x3D: 32 bit segment ES register: [ES:register]
- 0x3E: 32 bit segment ES B:        [ES:B]
- 0x60: register AF:                AF
- 0x61: register BF:                BF
- 0x62: register CF:                CF
- 0x63: register DF:                DF
- 0x70: register AD:                AD
- 0x71: register BD:                BD
- 0x72: register CD:                CD
- 0x73: register DD:                DD

## Interrupt descriptor table

The Interrupt descriptor table is 16 KB in size

### Layout

|Offset |Size |Name                     |Description
|-------|-    |-                        |-
|`00`   |`32` |Offset                   | this is the offset to the interrupt function
|`32`   |`20` |Segment                  | this is the segment to the interrupt function
|`52`   |`01` |CPU Privilege level      | this is the privilege level [more here](#privilege-level)
|`53`   |`01` |Gate type                | this is the Gate type [more here](#gate-type)
|`54`   |`10` |reserved                 | reserved NOT IN USE

#### privilege level

#### Gate type

### Interrupt descriptor table entres

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

when the BC664 Architecture gets an Abort exception it will stop and the Halt flag will be set

when the BC664 Architecture gets an Fault exception it will skip that instruction and continue

## Caches

## Floating values

The BC664 architecture supports floating-point operations through specialized registers and instructions. The floating-point unit (FPU) can handle both single-precision and double-precision values. Below are the key components and capabilities related to floating-point operations.

### Floating-Point Registers

The architecture provides several dedicated floating-point registers:

- AF, BF, CF, DF: 32-bit single-precision floating-point registers.
- AD, BD, CD, DD: 64-bit double-precision floating-point registers.

## Virtual mode

In virtual mode the CPU has [here](../BCG%20arch%20Specs.md#virtual-mode)

## Extended mode

in Extended mode the CPU will get [here](../BCG%20arch%20Specs.md#extended-mode)

## Protected mode

in Protected mode the CPU will get [here](../BCG%20arch%20Specs.md#protected-mode)

## Long mode

in Long mode the CPU will get [here](../BCG%20arch%20Specs.md#long-mode)

## Long long mode

in long long mode the CPU will get [here](../BCG%20arch%20Specs.md#long-long-mode)

## Memory

### Memory layout

- Address bus: 16 bits to a max of 48 bits

|Base Address   |Size           |Name                       |Description
|---------------|---------------|---------------------------|-
|`0x0000_000000`|`0x0000_004000`| Interrupt descriptor table| Interrupt descriptor table more [here](#interrupt-descriptor-table)
|`0x0000_004400`|`0x0000_000200`| IO ports                  | this is where the Ports is at
|`0x0000_004600`|`0x0000_00BA00`| RAM                       | RAM in the first segment
|`0x0000_010000`|`0x0000_020000`| VRAM                      | video ram
|`0x0000_030000`|`0xFFFF_FD0000`| RAM                       | RAM

the end is `0x10000_0000`

## The Stack

The Stack is a 16 MB section set using the [Stack Segment register](#registers) and the SPX(Stack pointer) as the offset, you can push data onto the stack using [PUSH](./instructions%20BC116.md) and pop data off the stack using POP, **the stack grows down**

## REGISTERS

- EA              64 bit general purpose register
- EB              64 bit general purpose register
- EC              64 bit general purpose register
- ED              64 bit general purpose register
- EX              64 bit general purpose register
- FX              64 bit general purpose register
- GX              64 bit general purpose register
- HX              64 bit general purpose register

- CS:             16  bit code segment register
- DS:             16  bit data segment register
- ES:             16  bit extra data segment register
- FS:             16  bit extra data segment register
- GS:             16  bit extra data segment register
- SS:             16  bit stack segment register

- PC:             40  bit program counter

- HL (H + L):     64 bit general purpose address register
- H:              32 bit general purpose address register
- L:              32 bit general purpose address register

- BPX:            32  bit Stack register
- SPX:            32  bit Stack register

- AF:             32  bit float register
- BF:             32  bit float register
- CF:             32  bit float register
- DF:             32  bit float register

- AD:             64  bit float register
- BD:             64  bit float register
- CD:             64  bit float register
- DD:             64  bit float register

- R1..20:         64  bit general purpose register

- CR0:            24  bit control register
  - 0x000001 Boot mode
  - 0x000002 FPU enabled
  - 0x000004 Low power Mode
  - 0x000008
  - 0x000010 Enable extended mode             Enableing [extended mode](#extended-mode)
  - 0x000020
  - 0x000040
  - 0x000080
  - 0x000100 Enable Protected mode            Enableing [Protected mode](#protected-mode)
  - 0x000200
  - 0x000400
  - 0x000800
  - 0x001000
  - 0x002000
  - 0x004000
  - 0x008000 Enable Long mode                 Enableing [Long mode](#long-mode)
  - 0x010000
  - 0x020000
  - 0x040000
  - 0x080000
  - 0x100000
  - 0x200000
  - 0x400000
  - 0x800000

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
