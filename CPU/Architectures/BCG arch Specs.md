# CPU Specifications

- [CPU Specifications](#cpu-specifications)
  - [INSTRUCTIONS](#instructions)
    - [INSTRUCTION SET](#instruction-set)
    - [INSTRUCTION LAYOUT](#instruction-layout)
    - [Base argument modes from the BC018 architecture](#base-argument-modes-from-the-bc018-architecture)
  - [Global Descriptor Table](#global-descriptor-table)
    - [Layout](#layout)
      - [Access Byte](#access-byte)
      - [GDT flags](#gdt-flags)
  - [Interrupt tables](#interrupt-tables)
    - [Interrupt descriptor table](#interrupt-descriptor-table)
      - [Layout](#layout-1)
    - [Interrupt vector table](#interrupt-vector-table)
      - [Layout](#layout-2)
    - [Interrupt entres](#interrupt-entres)
      - [interrupt](#interrupt)
      - [exception](#exception)
        - [Stack](#stack)
  - [privilege level](#privilege-level)
    - [Ring 0 BIOS](#ring-0-bios)
    - [Ring 1 Kernel mode](#ring-1-kernel-mode)
    - [Ring 2 User mode](#ring-2-user-mode)
    - [Ring 3 Restricted user mode](#ring-3-restricted-user-mode)
  - [Gate type](#gate-type)
  - [Pipelining](#pipelining)
  - [CPUID](#cpuid)
  - [Caches](#caches)
  - [Calling convention](#calling-convention)
    - [Caller](#caller)
    - [Callee](#callee)
  - [Virtual mode](#virtual-mode)
  - [Extended mode](#extended-mode)
  - [Protected mode](#protected-mode)
  - [Protected extended mode](#protected-extended-mode)
  - [Long mode](#long-mode)
  - [long long mode](#long-long-mode)
  - [MEMORY](#memory)
    - [MEMORY LAYOUT](#memory-layout)
  - [Base registers for BC8](#base-registers-for-bc8)
  - [Extended registers](#extended-registers)
  - [Protected registers](#protected-registers)
  - [long mode registers](#long-mode-registers)
  - [long long mode registers](#long-long-mode-registers)
  - [Virtual mode flags](#virtual-mode-flags)
  - [Extended mode flags](#extended-mode-flags)
  - [Extended mode CR0 flags](#extended-mode-cr0-flags)
  - [Protected mode flags](#protected-mode-flags)
  - [Protected mode CR0 flags](#protected-mode-cr0-flags)
  - [Long mode CR0 flags](#long-mode-cr0-flags)

## INSTRUCTIONS

### INSTRUCTION SET

### INSTRUCTION LAYOUT

XXXXXXXX_XXXXXXXX_AAAAAAAA_BBBBBBBB

- U = Unused
- X = instruction code
- A = argument 1 if empty then not there
- B = argument 2 if empty then not there

### Base argument modes from the BC018 architecture

- 0x00: immediate byte:             number
- 0x01: immediate word:             number
- 0x10: register:                   register
- 0x11: register AL:                AL
- 0x12: register BL:                BL
- 0x13: register CL:                CL
- 0x14: register DL:                DL
- 0x15: register H:                 H
- 0x16: register L:                 L
- 0x2E: register address:           address [register]
- 0x2F: address register HL:        [HL]
- 0x30: Relative address:           [byte address]      an 8 bit offset to the PC
- 0x31: Near address:               Near [address]      a 8 bit address
- 0x32: Short address:              short [address]     a 16 bit address
- 0x39: 32 bit segment address:     [register:register]
- 0x3A: 32 bit segment DS register: [DS:register]
- 0x3B: 32 bit segment DS B:        [DS:B]
- 0x40: SP relative address byte:   [SP + sbyte number]
- 0x41: BP relative address byte:   [BP + sbyte number]

## Global Descriptor Table

### Layout

|Offset |Size |Name                     |Description
|-------|-    |-                        |-
|`00`   |`16` |Limit                    | this is the limit of the entry or the size of the segment This limit is the low part
|`16`   |`32` |Base                     | this is the base of the entry or where the segment starts
|`48`   |`08` |Access Byte              | this is the [access Byte](#access-byte)
|`56`   |`04` |Other flags              | this is the [flags](#gdt-flags)
|`60`   |`04` |Limit                    | this is the limit of the entry or the size of the segment This limit is the high part

#### Access Byte

|7|6&nbsp;5|4|3 |2|1 |0
|-|--------|-|--|-|--|-
|0|DPL     |T|DC|0|RW|0

- DPL [Descriptor privilege level field](#privilege-level)
- T Descriptor type bit.
  - if clear(0) the descriptor defines a code segment. if set (1) it defines a data segment
- DC Direction bit
  - For Data segments set(1) the segment grows down (from base + limit to base), clear(0) the segment grows up (from base to base + limit)
- WR Readable /Writable bit
  - For data segments, clear (0) read and write access, if set (1) write access is not allowed
  - For code segments, clear (0) read access is not allowed, if set (1) read access

#### GDT flags

|3|2|1|0
|-|-|-|-
|G|T|L|Reserved

- G Granularity flag
  - if clear (0) the limit is in 1 byte blocks, if set (1) the limlt is in 4 KB blocks
- T Size flag
  - if clear (0), the descriptor defines a 16 bit Protected extended mode, if set (1), the descriptor defines a 16 bit extended mode
- L Long-mode code flag
  - if set (1) the descriptor defines a 32 bit Long long mode segment, if clear (0) the T flag should be used

## Interrupt tables

### Interrupt descriptor table

#### Layout

|Offset |Size |Name                     |Description
|-------|-    |-                        |-
|`00`   |`32` |Segment                  | this is the segment to the interrupt function
|`32`   |`20` |Offset                   | this is the offset to the interrupt function
|`52`   |`04` |reserved                 | reserved NOT IN USE
|`56`   |`02` |CPU Privilege level      | this is the privilege level [more here](#privilege-level)
|`58`   |`02` |Gate type                | this is the Gate type [more here](#gate-type)
|`60`   |`04` |reserved                 | reserved NOT IN USE

### Interrupt vector table

The Interrupt vector table is 2 KB in size

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
|User defined IRQ interrupt |17-31          |interrupt        |true           |The IRQ pin
|MMU Interrupt              |32             |interrupt        |false          |INT 0x03 or the MMU Interrupt function calls
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

##### Stack

|offset |BP offset|type     |name
|-------|---------|---------|-
|0      |BP - 8   |uint32   |Return address
|4      |BP - 4   |uint16   |Flags
|6      |BP - 2   |uint16   |enter
|8      |BP + 0   |uint16   |Error code
|10     |BP + 2   |byte[20] |Pushr

## privilege level

The privilege level goes from 0 to 3 where 0 is [ring 0](#ring-0-bios) and 3 is [ring 3](#ring-3-restricted-user-mode)

### Ring 0 BIOS

### Ring 1 Kernel mode

### Ring 2 User mode

### Ring 3 Restricted user mode

## Gate type

- 0b00 or 0x0: Trap Gate
- 0b01 or 0x1: 16 bit Interrupt Gate
- 0b10 or 0x2: 32 bit Interrupt Gate

## Pipelining

The BCG arch can do 3 stage pipelining like this

|Stage      |Stage1             |Stage2               |Stage3                 |Stage4
|-----------|-------------------|---------------------|-----------------------|-
|Opertion1  |Fetch instruction  |instruction Decoding |instruction execution  |Fetch instruction
|Opertion2  |                   |Fetch instruction    |instruction Decoding   |instruction execution
|Opertion2  |                   |                     |Fetch instruction      |instruction Decoding

## CPUID

|Index  |Name |Description
|-------|-----|-
|0x0000 |WIP  |WIP

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

- 8 bit data bus
- 20 bit adderss bus
- a 4 KB [IVT](#interrupt-vector-table)
- And only has the [base registers](#base-registers-for-bc8)
- The CPU will emulate the [BC8](./BC018/BC018%20Architecture%20Specs.md) CPU

## Extended mode

in Extended mode the CPU will get

- 16 bit data bus
- 24 bit address bus
- Make use of the [extended registers](#extended-registers)
- new argument modes
  - 0x02: immediate tbyte:            number
  - 0x03: immediate dword:            number
  - 0x08: immediate_float:            numberf
  - 0x17: register A                  A
  - 0x18: register B                  B
  - 0x19: register C                  C
  - 0x1A: register D                  D
  - 0x33: long address:               long [address]      a 24 bit address
  - 0x50: Short X indexed address:    [short address],X
  - 0x51: Short Y indexed address:    [short address],Y
  - 0x3C: 32 bit segment ES register: [ES:register]
  - 0x3D: 32 bit segment ES B:        [ES:B]
  - 0x3E: 32 bit segment FS register: [FS:register]
  - 0x3F: 32 bit segment FS B:        [FS:B]
  - 0x40: 32 bit segment GS register: [GS:register]
  - 0x41: 32 bit segment GS B:        [GS:B]
  - 0x42: 32 bit segment HS register: [HS:register]
  - 0x43: 32 bit segment HS B:        [HS:B]
  - 0x60: register AF:                AF
  - 0x61: register BF:                BF
  - 0x62: register CF:                CF
  - 0x63: register DF:                DF

## Protected mode

in Protected mode the CPU will get

- Make use of the [protected registers](#protected-registers)
- you will get the IDT instead of the IVT
- the IDT will be 16 kb starting at address 0
- you will get the GDT

## Protected extended mode

in Protected Extended mode the CPU will get

- 32 bit address bus
- 16 bit data bus
- segments are now 16 bits
- offsets are now 32 bits
- BIOS interrupt function no longer work
- PC becomes a 32 bit register
- \+ Protected mode
- new argument modes
  - 0x04: immediate qword:            number
  - 0x1B: register AX:                AX
  - 0x1C: register BX:                BX
  - 0x1D: register CX:                CX
  - 0x1E: register DX:                DX
  - 0x34: Far address:                far [address]       a 32 bit address
  - 0x52: SP relative address short:  [SP + short number]
  - 0x53: BP relative address short:  [BP + short number]

## Long mode

in long mode the CPU will get

- 32 bit address bus
- 32 bit data bus
- segments are now 20 bits
- offsets are now 32 bits
- Make use of the [long registers](#long-mode-registers)
- a 16 KB [IDT](#interrupt-descriptor-table)
- new argument modes
  - 0x09: immediate_double:           double numberf
  - 0x1F: register EX:                EX
  - 0x20: register GX:                GX
  - 0x54: SPX relative address word:  [SPX + word number]
  - 0x55: BPX relative address word:  [BPX + word number]
  - 0x56: SPX relative address tbyte: [SPX + tbyte number]
  - 0x57: BPX relative address tbyte: [BPX + tbyte number]
  - 0x58: SPX relative address int:   [SPX + int number]
  - 0x59: BPX relative address int:   [BPX + int number]
  - 0x70: register AD:                AD
  - 0x71: register BD:                BD
  - 0x72: register CD:                CD
  - 0x73: register DD:                DD

## long long mode

in long long mode the CPU will get

- 40 bit address bus
- 64 bit data bus
- segments are now 32 bits
- offsets are now 32 bits
- Make use of the [long long registers](#long-long-mode-registers)
- PC becomes a 32 bit register
- \+ Protected mode
- new argument mode
  - 0x05: immediate dqword:           number
  - 0x5A: SPX relative address word:  [SPX + word number]
  - 0x5B: BPX relative address word:  [BPX + word number]
  - 0x5C: BPX relative address int:   [BPX + int number]
  - 0x5D: BPX relative address int:   [BPX + int number]

## MEMORY

### MEMORY LAYOUT

|Base Address |Size       |Name                     |Description
|-------------|-----------|-------------------------|-
|`0x0000_0000`|`0x00_0400`| Interrupt vector table  | Interrupt vector table more [here](#interrupt-vector-table)
|`0x0000_0400`|`0x00_FC00`| RAM                     | RAM in the first segment
|`0x0001_0000`|`0x02_0000`| VRAM                    | video ram
|`0x0003_0000`|end of Memory|RAM                    | RAM

## Base registers for BC8

|Name     |Type   |Size in bits |Description
|---------|-------|-------------|-
|AL       | GP    |8            | an 8 bit general purpose register
|AH       | GP    |8            | an 8 bit general purpose register
|BL       | GP    |8            | an 8 bit general purpose register
|BH       | GP    |8            | an 8 bit general purpose register
|CL       | GP    |8            | an 8 bit general purpose register
|CH       | GP    |8            | an 8 bit general purpose register
|DL       | GP    |8            | an 8 bit general purpose register
|DH       | GP    |8            | an 8 bit general purpose register
|&nbsp;   |&nbsp; |&nbsp;       |&nbsp;
|CS       |Segment|16           | an 16 bit code segment register
|DS       |Segment|16           | an 16 bit data segment register
|SS       |Segment|16           | an 16 bit stack segment register
|&nbsp;   |&nbsp; |&nbsp;       |&nbsp;
|PC       |address|16           | an 16 bit program counter
|HL       |address|32           | an 32 bit address register
|H        |address|16           | an 16 bit address register high part of HL
|L        |address|16           | an 16 bit address register low part of HL
|&nbsp;   |&nbsp; |&nbsp;       |&nbsp;
|SP       |stack  |16           | an 16 bit stack pointer
|BP       |stack  |16           | an 16 bit base pointer
|&nbsp;   |&nbsp; |&nbsp;       |&nbsp;
|R1-4     |GP     |8            | an 8 bit general purpose registers
|&nbsp;   |&nbsp; |&nbsp;       |&nbsp;
|F        |flags  |8            | an 8 bit flags register [flags here](#virtual-mode-flags)

## Extended registers

|Name     |Type     |Size in bits |Description
|---------|---------|-------------|-
|A(L & H) | GP      |8            | an 8 bit general purpose register the low/high part of A
|B(L & H) | GP      |8            | an 8 bit general purpose register the low/high part of B
|C(L & H) | GP      |8            | an 8 bit general purpose register the low/high part of C
|D(L & H) | GP      |8            | an 8 bit general purpose register the low/high part of D
|A        | GP      |16           | an 16 bit general purpose register
|B        | GP      |16           | an 16 bit general purpose register
|C        | GP      |16           | an 16 bit general purpose register
|D        | GP      |16           | an 16 bit general purpose register
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|CS       |Segment  |16           | an 16 bit code segment register
|DS       |Segment  |16           | an 16 bit data segment register
|ES       |Segment  |16           | an 16 bit extra data segment register
|FS       |Segment  |16           | an 16 bit extra data segment register
|GS       |Segment  |16           | an 16 bit extra data segment register
|HS       |Segment  |16           | an 16 bit extra data segment register
|SS       |Segment  |16           | an 16 bit stack segment register
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|PC       |address  |24           | an 24 bit program counter
|HL       |address  |32           | an 32 bit address register
|H        |address  |16           | an 16 bit address register high part of HL
|L        |address  |16           | an 16 bit address register low part of HL
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|SP       |stack    |16           | an 16 bit stack pointer
|BP       |stack    |16           | an 16 bit base pointer
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|AF       |GB float |32           | a 32 bit single precision float register
|BF       |GB float |32           | a 32 bit single precision float register
|CF       |GB float |32           | a 32 bit single precision float register
|DF       |GB float |32           | a 32 bit single precision float register
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|R1-16    |GP       |16           | an 16 bit general purpose registers
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|CR0      |control  |8            | an 8 bit control register [flags here](#extended-mode-cr0-flags)
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|F        |flags    |16           | an 16 bit flags register [flags here](#extended-mode-flags)

## Protected registers

|Name     |Type     |Size in bits |Description
|---------|---------|-------------|-
|A(L & H) | GP      |8            | an 8 bit general purpose register the low/high part of A
|B(L & H) | GP      |8            | an 8 bit general purpose register the low/high part of B
|C(L & H) | GP      |8            | an 8 bit general purpose register the low/high part of C
|D(L & H) | GP      |8            | an 8 bit general purpose register the low/high part of D
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|A        | GP      |16           | an 16 bit general purpose register the low part of the AX register
|B        | GP      |16           | an 16 bit general purpose register the low part of the BX register
|C        | GP      |16           | an 16 bit general purpose register the low part of the CX register
|D        | GP      |16           | an 16 bit general purpose register the low part of the DX register
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|AX       | GP      |32           | an 32 bit general purpose register
|BX       | GP      |32           | an 32 bit general purpose register
|CX       | GP      |32           | an 32 bit general purpose register
|DX       | GP      |32           | an 32 bit general purpose register
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|CS       |Segment  |16           | an 16 bit code segment register low part of the ECS register
|DS       |Segment  |16           | an 16 bit data segment register low part of the EDS register
|ES       |Segment  |16           | an 16 bit extra data segment register low part of the EES register
|FS       |Segment  |16           | an 16 bit extra data segment register low part of the EFS register
|GS       |Segment  |16           | an 16 bit extra data segment register low part of the EGS register
|HS       |Segment  |16           | an 16 bit extra data segment register low part of the EHS register
|SS       |Segment  |16           | an 16 bit stack segment register low part of the ESS register
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|ECS      |Segment  |32           | an 32 bit code segment register
|EDS      |Segment  |32           | an 32 bit data segment register
|EES      |Segment  |32           | an 32 bit extra data segment register
|EFS      |Segment  |32           | an 32 bit extra data segment register
|EGS      |Segment  |32           | an 32 bit extra data segment register
|EHS      |Segment  |32           | an 32 bit extra data segment register
|ESS      |Segment  |32           | an 32 bit stack segment register
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|PC       |address  |32           | an 32 bit program counter
|HL       |address  |32           | an 32 bit address register
|H        |address  |16           | an 16 bit address register high part of HL
|L        |address  |16           | an 16 bit address register low part of HL
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|PTA      |address  |16           | this register will point to the page table
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|GDA      |address  |32           | this register will point to the global descriptor table in memory
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|SP       |stack    |16           | an 16 bit stack pointer
|BP       |stack    |16           | an 16 bit base pointer
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|AF       |GB float |32           | a 32 bit single precision float register
|BF       |GB float |32           | a 32 bit single precision float register
|CF       |GB float |32           | a 32 bit single precision float register
|DF       |GB float |32           | a 32 bit single precision float register
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|R1-16    |GP       |32           | an 32 bit general purpose registers
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|CR0      |control  |16           | an 16 bit control register [flags here](#protected-mode-cr0-flags)
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|F        |flags    |24           | an 24 bit flags register [flags here](#protected-mode-flags)

## long mode registers

|Name     |Type     |Size in bits |Description
|---------|---------|-------------|-
|A(L & H) | GP      |8            | an 8 bit general purpose register the low/high part of A
|B(L & H) | GP      |8            | an 8 bit general purpose register the low/high part of B
|C(L & H) | GP      |8            | an 8 bit general purpose register the low/high part of C
|D(L & H) | GP      |8            | an 8 bit general purpose register the low/high part of D
|E(L & H) | GP      |8            | an 8 bit general purpose register the low/high part of E
|G(L & H) | GP      |8            | an 8 bit general purpose register the low/high part of G
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|A        | GP      |16           | an 16 bit general purpose register the low part of the AX register
|B        | GP      |16           | an 16 bit general purpose register the low part of the BX register
|C        | GP      |16           | an 16 bit general purpose register the low part of the CX register
|D        | GP      |16           | an 16 bit general purpose register the low part of the DX register
|E        | GP      |16           | an 16 bit general purpose register the low part of the EX register
|G        | GP      |16           | an 16 bit general purpose register the low part of the GX register
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|AX       | GP      |32           | an 32 bit general purpose register
|BX       | GP      |32           | an 32 bit general purpose register
|CX       | GP      |32           | an 32 bit general purpose register
|DX       | GP      |32           | an 32 bit general purpose register
|EX       | GP      |32           | an 32 bit general purpose register
|GX       | GP      |32           | an 32 bit general purpose register
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|CS       |Segment  |16           | an 16 bit code segment register low part of the ECS register
|DS       |Segment  |16           | an 16 bit data segment register low part of the EDS register
|ES       |Segment  |16           | an 16 bit extra data segment register low part of the EES register
|FS       |Segment  |16           | an 16 bit extra data segment register low part of the EFS register
|GS       |Segment  |16           | an 16 bit extra data segment register low part of the EGS register
|HS       |Segment  |16           | an 16 bit extra data segment register low part of the EHS register
|SS       |Segment  |16           | an 16 bit stack segment register low part of the ESS register
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|ECS      |Segment  |32           | an 32 bit code segment register
|EDS      |Segment  |32           | an 32 bit data segment register
|EES      |Segment  |32           | an 32 bit extra data segment register
|EFS      |Segment  |32           | an 32 bit extra data segment register
|EGS      |Segment  |32           | an 32 bit extra data segment register
|EHS      |Segment  |32           | an 32 bit extra data segment register
|ESS      |Segment  |32           | an 32 bit stack segment register
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|PC       |address  |32           | an 32 bit program counter
|HL       |address  |32           | an 32 bit address register
|H        |address  |16           | an 16 bit address register high part of HL
|L        |address  |16           | an 16 bit address register low part of HL
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|SP       |stack    |16           | an 16 bit stack pointer
|BP       |stack    |16           | an 16 bit base pointer
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|SPX      |stack    |20           | an 20 bit stack pointer
|BPX      |stack    |20           | an 20 bit base pointer
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|AF       |GB float |32           | a 32 bit single precision float register
|BF       |GB float |32           | a 32 bit single precision float register
|CF       |GB float |32           | a 32 bit single precision float register
|DF       |GB float |32           | a 32 bit single precision float register
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|AD       |GB float |64           | a 64 bit double precision float register
|BD       |GB float |64           | a 64 bit double precision float register
|CD       |GB float |64           | a 64 bit double precision float register
|DD       |GB float |64           | a 64 bit double precision float register
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|R1-16    |GP       |32           | an 32 bit general purpose registers
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|X        |index    |32           | an 32 bit index registers
|Y        |index    |32           | an 32 bit index registers
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|CR0      |control  |24           | an 24 bit control register [flags here](#long-mode-cr0-flags)
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|F        |flags    |24           | an 24 bit flags register [flags here](#protected-mode-flags)

## long long mode registers

|Name     |Type     |Size in bits |Description
|---------|---------|-------------|-
|A(L & H) | GP      |8            | an 8 bit general purpose register the low/high part of A
|B(L & H) | GP      |8            | an 8 bit general purpose register the low/high part of B
|C(L & H) | GP      |8            | an 8 bit general purpose register the low/high part of C
|D(L & H) | GP      |8            | an 8 bit general purpose register the low/high part of D
|E(L & H) | GP      |8            | an 8 bit general purpose register the low/high part of E
|G(L & H) | GP      |8            | an 8 bit general purpose register the low/high part of G
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|A        | GP      |16           | an 16 bit general purpose register the low part of the AX register
|B        | GP      |16           | an 16 bit general purpose register the low part of the BX register
|C        | GP      |16           | an 16 bit general purpose register the low part of the CX register
|D        | GP      |16           | an 16 bit general purpose register the low part of the DX register
|E        | GP      |16           | an 16 bit general purpose register the low part of the EX register
|G        | GP      |16           | an 16 bit general purpose register the low part of the GX register
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|AX       | GP      |32           | an 32 bit general purpose register the low part of the EA register
|BX       | GP      |32           | an 32 bit general purpose register the low part of the EB register
|CX       | GP      |32           | an 32 bit general purpose register the low part of the EC register
|DX       | GP      |32           | an 32 bit general purpose register the low part of the ED register
|EX       | GP      |32           | an 32 bit general purpose register the low part of the EE register
|GX       | GP      |32           | an 32 bit general purpose register the low part of the EG register
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|EA       | GP      |64           | an 64 bit general purpose register
|EB       | GP      |64           | an 64 bit general purpose register
|EC       | GP      |64           | an 64 bit general purpose register
|ED       | GP      |64           | an 64 bit general purpose register
|EE       | GP      |64           | an 64 bit general purpose register
|EG       | GP      |64           | an 64 bit general purpose register
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|CS       |Segment  |16           | an 16 bit code segment register low part of the ECS register
|DS       |Segment  |16           | an 16 bit data segment register low part of the EDS register
|ES       |Segment  |16           | an 16 bit extra data segment register low part of the EES register
|FS       |Segment  |16           | an 16 bit extra data segment register low part of the EFS register
|GS       |Segment  |16           | an 16 bit extra data segment register low part of the EGS register
|HS       |Segment  |16           | an 16 bit extra data segment register low part of the EHS register
|SS       |Segment  |16           | an 16 bit stack segment register low part of the ESS register
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|ECS      |Segment  |32           | an 32 bit code segment register
|EDS      |Segment  |32           | an 32 bit data segment register
|EES      |Segment  |32           | an 32 bit extra data segment register
|EFS      |Segment  |32           | an 32 bit extra data segment register
|EGS      |Segment  |32           | an 32 bit extra data segment register
|EHS      |Segment  |32           | an 32 bit extra data segment register
|ESS      |Segment  |32           | an 32 bit stack segment register
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|PC       |address  |32           | an 32 bit program counter
|HL       |address  |64           | an 64 bit address register
|H        |address  |32           | an 32 bit address register high part of HL
|L        |address  |32           | an 32 bit address register low part of HL
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|PTA      |address  |32           | this register will point to the page table
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|GDA      |address  |40           | this register will point to the global descriptor table in memory
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|SP       |stack    |16           | an 16 bit stack pointer
|BP       |stack    |16           | an 16 bit base pointer
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|SPX      |stack    |32           | an 32 bit stack pointer
|BPX      |stack    |32           | an 32 bit base pointer
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|AF       |GB float |32           | a 32 bit single precision float register
|BF       |GB float |32           | a 32 bit single precision float register
|CF       |GB float |32           | a 32 bit single precision float register
|DF       |GB float |32           | a 32 bit single precision float register
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|AD       |GB float |64           | a 64 bit double precision float register
|BD       |GB float |64           | a 64 bit double precision float register
|CD       |GB float |64           | a 64 bit double precision float register
|DD       |GB float |64           | a 64 bit double precision float register
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|R1-16    |GP       |64           | an 64 bit general purpose registers
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|X        |index    |32           | an 32 bit index registers
|Y        |index    |32           | an 32 bit index registers
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|CR0      |control  |24           | an 24 bit control register [flags here](#long-mode-cr0-flags)
|&nbsp;   |&nbsp;   |&nbsp;       |&nbsp;
|F        |flags    |24           | an 24 bit flags register [flags here](#protected-mode-flags)

## Virtual mode flags

|bit  |name             |Description
|-----|---------        |-
|0x01 |zero             | this flag is set if a register is zero or instruction gets a zero
|0x02 |equals           | this flag is set if an CMP instruction is true
|0x04 |signed           | this flag is set if the signed bit is set on a register in an instruction
|0x08 |carry            | this flag is set if a arithmetic instruction overflows and can be used in arithmetic instruction
|0x10 |overflow         | this flag is set if a arithmetic instruction overflows
|0x20 |less then        | this flag is set if the operand1 is less then operand2 in a CMP instruction
|0x40 |interrupt enable | this flag is used to enable the IRQ interrupts
|0x80 |HALT             | this flag will halt the CPU

## Extended mode flags

|bit    |name             |Description
|-------|---------        |-
|0x0001 |zero             | this flag is set if a register is zero or instruction gets a zero
|0x0002 |equals           | this flag is set if an CMP instruction is true
|0x0004 |signed           | this flag is set if the signed bit is set on a register in an instruction
|0x0008 |carry            | this flag is set if a arithmetic instruction overflows and can be used in arithmetic instruction
|0x0010 |overflow         | this flag is set if a arithmetic instruction overflows
|0x0020 |less then        | this flag is set if the operand1 is less then operand2 in a CMP instruction
|0x0040 |interrupt enable | this flag is used to enable the IRQ interrupts
|0x0080 |HALT             | this flag will halt the CPU
|0x0100 |reserved         | reserved
|0x0200 |under flow       | this flag is set if a arithmetic instruction underflows
|0x0400 |shift flag       | this flag is used with the ROR ROL instructions
|0x0800 |greater then     | this flag is set if the operand1 is greater then operand2 in a CMP instruction
|0x1000 |float overflow   | this flag is set if a float arithmetic instruction overflows
|0x2000 |reserved         | reserved
|0x4000 |reserved         | reserved
|0x8000 |reserved         | reserved

## Extended mode CR0 flags

|bit  |name                   |Description
|-----|---------              |-
|0x01 |Boot mode              | this flag is used by the BIOS
|0x02 |FPU enabled            | this flag is used to enable the FPU for float arithmetic
|0x04 |low power mode         | this flag will set the CPU in a state where it cuts the power but is still operational
|0x08 |reserved               | reserved
|0x10 |Enable extended mode   | this flag is used to enable extended mode
|0x20 |Enable A24             | this flag is used to enable the A24 line
|0x40 |reserved               | reserved
|0x80 |Enable protected mode  | this flag is used to enable protected mode

## Protected mode flags

|bit      |name             |Description
|---------|---------        |-
|0x000001 |zero             | this flag is set if a register is zero or instruction gets a zero
|0x000002 |equals           | this flag is set if an CMP instruction is true
|0x000004 |signed           | this flag is set if the signed bit is set on a register in an instruction
|0x000008 |carry            | this flag is set if a arithmetic instruction overflows and can be used in arithmetic instruction
|0x000010 |overflow         | this flag is set if a arithmetic instruction overflows
|0x000020 |less then        | this flag is set if the operand1 is less then operand2 in a CMP instruction
|0x000040 |interrupt enable | this flag is used to enable the IRQ interrupts
|0x000080 |HALT             | this flag will halt the CPU
|0x000100 |reserved         | reserved
|0x000200 |under flow       | this flag is set if a arithmetic instruction underflows
|0x000400 |shift flag       | this flag is used with the ROR ROL instructions
|0x000800 |greater then     | this flag is set if the operand1 is greater then operand2 in a CMP instruction
|0x001000 |reserved         | reserved
|0x002000 |reserved         | reserved
|0x004000 |reserved         | reserved
|0x008000 |reserved         | reserved
|0x010000 |reserved         | reserved
|0x020000 |reserved         | reserved
|0x040000 |reserved         | reserved
|0x080000 |reserved         | reserved
|0x100000 |reserved         | reserved
|0x200000 |reserved         | reserved
|0x400000 |reserved         | reserved
|0x800000 |reserved         | reserved

## Protected mode CR0 flags

|bit    |name                   |Description
|-------|---------              |-
|0x0001 |Boot mode              | this flag is used by the BIOS
|0x0002 |FPU enabled            | this flag is used to enable the FPU for float arithmetic
|0x0004 |low power mode         | this flag will set the CPU in a state where it cuts the power but is still operational
|0x0008 |reserved               | reserved
|0x0010 |Enable extended mode   | this flag is used to enable extended mode
|0x0020 |Enable A24             | this flag is used to enable the A24 line
|0x0040 |reserved               | reserved
|0x0080 |Enable protected mode  | this flag is used to enable protected mode
|0x0100 |Enable paging          | this flag is used to enable paging
|0x0200 |reserved               | reserved
|0x0400 |reserved               | reserved
|0x0800 |reserved               | reserved
|0x1000 |reserved               | reserved
|0x2000 |reserved               | reserved
|0x4000 |reserved               | reserved
|0x8000 |reserved               | reserved

## Long mode CR0 flags

|bit    |name                   |Description
|-------|---------              |-
|0x000001 |Boot mode              | this flag is used by the BIOS
|0x000002 |FPU enabled            | this flag is used to enable the FPU for float arithmetic
|0x000004 |low power mode         | this flag will set the CPU in a state where it cuts the power but is still operational
|0x000008 |reserved               | reserved
|0x000010 |Enable extended mode   | this flag is used to enable extended mode
|0x000020 |Enable A24             | this flag is used to enable the A24 line
|0x000040 |reserved               | reserved
|0x000080 |Enable protected mode  | this flag is used to enable protected mode
|0x000100 |Enable paging          | this flag is used to enable paging
|0x000200 |reserved               | reserved
|0x000400 |reserved               | reserved
|0x000800 |reserved               | reserved
|0x001000 |reserved               | reserved
|0x002000 |reserved               | reserved
|0x004000 |reserved               | reserved
|0x008000 |reserved               | reserved
|0x010000 |reserved               | reserved
|0x020000 |reserved               | reserved
|0x040000 |reserved               | reserved
|0x080000 |reserved               | reserved
|0x100000 |Enable long mode       | this flag is used to enable long mode
|0x200000 |reserved               | reserved
|0x400000 |reserved               | reserved
|0x800000 |reserved               | reserved
