# CPU Specifications

- [CPU Specifications](#cpu-specifications)
  - [OVERVIEW](#overview)
  - [INSTRUCTIONS](#instructions)
    - [INSTRUCTION SET](#instruction-set)
    - [INSTRUCTION LAYOUT](#instruction-layout)
    - [ARGUMENT MODE](#argument-mode)
      - [Note](#note)
  - [INTERRUPTS](#interrupts)
  - [Calling convention](#calling-convention)
    - [Caller](#caller)
    - [Callee](#callee)
  - [MEMORY](#memory)
    - [MEMORY LAYOUT](#memory-layout)
  - [REGISTERS](#registers)

## OVERVIEW

- Name: BCG4
- Data bus: 8 bits
- Clock speed: 10 MHz
- Address bus: 16 bits

## INSTRUCTIONS

### INSTRUCTION SET

see the [Instruction Set](./instructions%20BCG4.md) here

### INSTRUCTION LAYOUT

XXXXXXXX_AAAABBBB

X = Op code
A = argument1
B = argument2

### ARGUMENT MODE

- 0x00: immediate byte                    byte number
- 0x01: immediate word                    word number
- 0x02: register                          register
- 0x03: register address                  [register]
- 0x04: Near address                      Near [address]      a 8 bit address
- 0x05: address                           [address]           a 16 bit address
- 0x06: Relative address                  [byte address]      an 8 bit offset to the PC
- 0x07: Register A                        A
- 0x08: Register HL                       HL
- 0x09: Register address HL               [HL]
- 0x0A: Register B                        B
- 0x0B: Register F                        F

#### Note

## INTERRUPTS

The [BRK](./instructions%20BCG4.md#special-instructions) will jump to the IRQ vector

## Calling convention

The calling convention for the BCG4 architecture is as follows:

### Caller

1. load Argments
   1. load the arguments from into the registers
2. Call the function using [CALL](./instructions%20BCG4.md#special-instructions)
3. Retrieve Return Value
   1. The return value will be in either the A or HL register:
      1. Use the [HL](#registers) register if it's a pointer/address.
      2. Use the [A](#registers) register if it's an immediate value.

### Callee

This is the sequence within the called function:

1. Return Value
   1. Move the return value to the appropriate register:
      1. HL register if it's a pointer.
      2. A register if it's an immediate value.
2. Return from Function
    1. Return from the function using the [RET](./instructions%20BCG4.md#general-instructions) instruction.

## MEMORY

note: `all cells is in bytes`

### MEMORY LAYOUT

|Address |Size    |Name          |Description
|--------|--------|--------------|-
|`0x0000`|`0x0100`| Zero page    | This is the first page of the memory
|`0x0100`|`0x0100`| IO ports     | this is where the Ports is at
|`0x0200`|`0x0100`| Stack        | Stack
|`0x0300`|`0x7D00`| RAM          | RAM
|`0x8000`|`0x7FFA`| RAM Banked   | this is the data/prgram is at but banked
|`0xFFFA`|`0x0002`| Reset Vector | this is where the program address is at
|`0xFFFC`|`0x0002`| IRQ Vector   | this is where the IRQ/BRK handler
|`0xFFFE`|`0x0002`| NMI Vector   | this is where the NMI handler

## REGISTERS

- A:              8  bit general purpose register
- B:              8  bit general purpose register
- C:              8  bit general purpose register
- D:              8  bit general purpose register
- E:              8  bit general purpose register
- G:              8  bit general purpose register

- H:              8  bit general purpose address register
- L:              8  bit general purpose address register
- HL:   (H + L)   16  bit general purpose address register

- X:              8  bit index register
- Y:              8  bit index register

- IP:   16  bit program counter

- SP:   8   bit Stack register

- MB:   8   bit memory bank register

- F:    8  bit Flags register
  - 0x01 zero
  - 0x02 equals
  - 0x04 signed
  - 0x08 carry
  - 0x10 overflow
  - 0x20 less
  - 0x40 interrupt enable
  - 0x80 HALT
