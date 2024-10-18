# CPU Specifications

- [CPU Specifications](#cpu-specifications)
  - [INSTRUCTIONS](#instructions)
    - [INSTRUCTION SET](#instruction-set)
    - [INSTRUCTION LAYOUT](#instruction-layout)
    - [Base argument modes from the BC018 architecture](#base-argument-modes-from-the-bc018-architecture)
  - [Interrupt tables](#interrupt-tables)
    - [Interrupt descriptor table](#interrupt-descriptor-table)
      - [Layout](#layout)
      - [privilege level](#privilege-level)
      - [Gate type](#gate-type)
    - [Interrupt vector table](#interrupt-vector-table)
      - [Layout](#layout-1)
    - [Interrupt entres](#interrupt-entres)
      - [interrupt](#interrupt)
      - [exception](#exception)
  - [Pipelining](#pipelining)
  - [Caches](#caches)
  - [Calling convention](#calling-convention)
    - [Caller](#caller)
    - [Callee](#callee)
  - [Virtual mode](#virtual-mode)
  - [Extended mode](#extended-mode)
  - [Protected mode](#protected-mode)
  - [Long mode](#long-mode)
  - [long long mode](#long-long-mode)
  - [MEMORY](#memory)
    - [MEMORY LAYOUT](#memory-layout)
  - [Base registers for BC8](#base-registers-for-bc8)
  - [Extended registers](#extended-registers)
    - [Bigger registers](#bigger-registers)
  - [Protected registers](#protected-registers)
    - [Bigger registers](#bigger-registers-1)
  - [long mode registers](#long-mode-registers)
  - [long long mode registers](#long-long-mode-registers)
    - [Bigger registers](#bigger-registers-2)

## INSTRUCTIONS

### INSTRUCTION SET

### INSTRUCTION LAYOUT

XXXXXXXX_XXXXXXXX_AAAAAAAA_BBBBBBBB

- U = Unused
- X = instruction code
- A = argument 1 if empty then not there
- B = argument 2 if empty then not there

### Base argument modes from the BC018 architecture

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
- 0x3C: 32 bit segment DS B:        [DS:B]

## Interrupt tables

### Interrupt descriptor table

#### Layout

|Offset |Size |Name                     |Description
|-------|-    |-                        |-
|`00`   |`32` |Offset                   | this is the offset to the interrupt function
|`32`   |`20` |Segment                  | this is the segment to the interrupt function
|`52`   |`01` |CPU Privilege level      | this is the privilege level [more here](#privilege-level)
|`53`   |`01` |Gate type                | this is the Gate type [more here](#gate-type)
|`54`   |`10` |reserved                 | reserved NOT IN USE

#### privilege level

#### Gate type

### Interrupt vector table

The Interrupt vector table is 4 KB in size

#### Layout

The first word is the Address Segment for the routine

The second word is the Address offset for the routine

### Interrupt entres

|Function                   |Interupt number|Type             |Is User defined|Related instructions
|---------------------------|---------------|-----------------|---------------|-
|Divide error               |0              |Fault exception  |true           |DIV or DIVF
|NMI/int 0x2 interrupt      |1              |interrupt        |true           |INT 2 or NMI pin
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

when the CPU gets an Abort exception it will stop and the Halt flag will be set

when the CPU gets an Fault exception it will skip that instruction and continue

## Pipelining

The BCG arch can do 3 stage pipelining like this

|Stage      |Stage1             |Stage2               |Stage3                 |Stage4
|-----------|-------------------|---------------------|-----------------------|-
|Opertion1  |Fetch instruction  |instruction Decoding |instruction execution  |Fetch instruction
|Opertion2  |                   |Fetch instruction    |instruction Decoding   |instruction execution
|Opertion2  |                   |                     |Fetch instruction      |instruction Decoding

## Caches

## Calling convention

The calling convention for the BCG architecture is as follows:

### Caller

1. Push Argments
   1. Push all the arguments from rigth to left onto the stack
2. Call the function using CALL
3. Retrieve Return Value
   1. The return value will be in either the A/AX or HL register:
      1. Use the HL register if it's a pointer/address.
      2. Use the A/AX register if it's an immediate value.

### Callee

This is the sequence within the called function:

1. Save the Base Pointer
   1. Push the old BP/BPX onto the stack
2. Move the Base Pointer to the Stack Pointer
   1. Move the BP/BPX to SP/SPX
3. Save All Registers
   1. Push all registers using the PUSHR instruction.
   2. To access arguments, pop them off the stack or reference them using an offset.
4. Return Value
   1. Move the return value to the appropriate register:
      1. HL register if it's a pointer.
      2. A/AX register if it's an immediate value.
5. Restore Registers
   1. Pop all registers using the POPR instruction.
6. Return from Function
    1. Return from the function with an offset depending on the size of the stack frame using the RET instruction.

## Virtual mode

In virtual mode the CPU has

- 16 bit adderss bus
- 8 bit data bus
- a 4 KB [IVT](#interrupt-vector-table)
- And only has the [base registers](#base-registers-for-bc8)
- The CPU will emulate the [BC8](./BC018/BC018%20Architecture%20Specs.md) CPU

## Extended mode

in Extended mode the CPU will get

- 24 bit address bus
- 16 bit data bus
- Make use of the [extended registers](#extended-registers)
- PC becomes a 24 bit register
- new argument modes
  - 0x02: immediate tbyte:            number
  - 0x03: immediate dword:            number
  - 0x08: immediate_float:            numberf
  - 0x17: register A                  A
  - 0x18: register B                  B
  - 0x19: register C                  C
  - 0x1A: register D                  D
  - 0x20: register AX:                AX
  - 0x21: register BX:                BX
  - 0x22: register CX:                CX
  - 0x23: register DX:                DX
  - 0x33: long address:               long [address]      a 24 bit address
  - 0x36: Short X indexed address:    [short address],X
  - 0x37: Short Y indexed address:    [short address],Y
  - 0x3D: 32 bit segment ES register: [ES:register]
  - 0x3E: 32 bit segment ES B:        [ES:B]
  - 0x60: register AF:                AF
  - 0x61: register BF:                BF
  - 0x62: register CF:                CF
  - 0x63: register DF:                DF

## Protected mode

in Protected mode the CPU will get

- 32 bit address bus
- 32 bit data bus
- Make use of the [protected registers](#protected-registers)
- PC becomes a 32 bit register
- new argument modes
  - 0x34: Far address:                far [address]       a 32 bit address
  - 0x38: SP relative address short:  [SP + short number]
  - 0x39: BP relative address short:  [BP + short number]

## Long mode

in long mode the CPU will get

- 32 bit address bus
- 64 bit data bus
- Make use of the [long registers](#long-mode-registers)
- a 16 KB [IDT](#interrupt-descriptor-table)
- new argument modes
  - 0x04: immediate qword:            number
  - 0x09: immediate_double:           double numberf
  - 0x24: register EX:                EX
  - 0x25: register FX:                FX
  - 0x26: register GX:                GX
  - 0x27: register HX:                HX
  - 0x38: SPX relative address tbyte: [SPX + tbyte number]
  - 0x39: BPX relative address tbyte: [BPX + tbyte number]
  - 0x70: register AD:                AD
  - 0x71: register BD:                BD
  - 0x72: register CD:                CD
  - 0x73: register DD:                DD

## long long mode

in long long mode the CPU will get

- 40 bit address bus
- 64 bit data bus
- Make use of the [long long registers](#long-long-mode-registers)
- PC becomes a 32 bit register
- \+ Protected mode
- new argument mode
  - 0x05: immediate dqword:           number
  - 0x38: SPX relative address int:   [SPX + int number]
  - 0x39: BPX relative address int:   [BPX + int number]

## MEMORY

### MEMORY LAYOUT

|Base Address |Size       |Name                     |Description
|-------------|-----------|-------------------------|-
|`0x0000_0000`|`0x00_1000`| Interrupt vector table  | Interrupt vector table more [here](#interrupt-vector-table)
|`0x0000_1000`|`0x00_0200`| IO ports                | this is where the Ports is at
|`0x0000_1200`|`0x00_EE00`| RAM                     | RAM in the first segment
|`0x0001_0000`|`0x02_0000`| VRAM                    | video ram
|`0x0003_0000`|end of Memory|RAM                    | RAM

## Base registers for BC8

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

## Extended registers

- A (AH + AL):    16  bit general purpose register
- B (BH + BL):    16  bit general purpose register
- C (CH + CL):    16  bit general purpose register
- D (DH + DL):    16  bit general purpose register

- CS:             16  bit code segment register
- DS:             16  bit data segment register
- ES:             16  bit extra data segment register
- FS:             16  bit extra data segment register
- SS:             16  bit stack segment register

- PC:             the PC is now 32 bits can only use 24 bits

- AF:             32  bit float register
- BF:             32  bit float register

- CR0:            8   bit control register
  - 0 0x01 Boot mode
  - 1 0x02 FPU enabled
  - 2 0x04 Low power Mode
  - 3 0x08 Enable bigger registers        Enableing [bigger registers](#bigger-registers)
  - 4 0x10 Enable extended mode           Enableing [extended mode](#extended-mode)
  - 5 0x20 reserved
  - 6 0x40 reserved
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
  - 0x0400 reserved
  - 0x0800 reserved
  - 0x1000 reserved
  - 0x2000 reserved
  - 0x4000 reserved
  - 0x8000 reserved

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

## Protected registers

- AX              32 bit general purpose register
- BX              32 bit general purpose register
- CX              32 bit general purpose register
- DX              32 bit general purpose register

- ES:             16  bit extra data segment register
- FS:             16  bit extra data segment register
- GS:             16  bit extra data segment register

- PC:             32  bit program counter

- R1..16:         32 bit general purpose register

- CR0:            16  bit control register
  - 0x0001 Boot mode
  - 0x0002 FPU enabled
  - 0x0004 Low power Mode
  - 0x0008 Enable bigger registers        Enableing [bigger registers](#bigger-registers)
  - 0x0010 Enable extended mode             Enableing [extended mode](#extended-mode)
  - 0x0020
  - 0x0040
  - 0x0080 Enable Protected mode            Enableing [Protected mode](#protected-mode)
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
  - 0x000100 reserved
  - 0x000200 under flow
  - 0x000400 reserved
  - 0x000800 reserved
  - 0x001000 reserved
  - 0x002000 reserved
  - 0x004000 reserved
  - 0x008000 reserved
  - 0x010000 reserved
  - 0x020000 reserved
  - 0x040000 reserved
  - 0x080000 reserved
  - 0x100000 reserved
  - 0x200000 reserved
  - 0x400000 reserved
  - 0x800000 reserved

- X:              32 bit index register
- Y:              32 bit index register

### Bigger registers

## long mode registers

- EX              32 bit general purpose register
- FX              32 bit general purpose register
- GX              32 bit general purpose register
- HX              32 bit general purpose register

- BPX:            20  bit Stack register
- SPX:            20  bit Stack register

- AD:             64  bit float register
- BD:             64  bit float register
- CD:             64  bit float register
- DD:             64  bit float register

## long long mode registers

- EA              64 bit general purpose register
- EB              64 bit general purpose register
- EC              64 bit general purpose register
- ED              64 bit general purpose register
- EX              64 bit general purpose register
- FX              64 bit general purpose register
- GX              64 bit general purpose register
- HX              64 bit general purpose register

- PC:             32  bit program counter

- HL (H + L):     64 bit general purpose address register
- H:              32 bit general purpose address register
- L:              32 bit general purpose address register

- BPX:            32  bit Stack register
- SPX:            32  bit Stack register

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

### Bigger registers
