# CPU Specifications

- [CPU Specifications](#cpu-specifications)
  - [OVERVIEW](#overview)
  - [INSTRUCTIONS](#instructions)
    - [INSTRUCTION SET](#instruction-set)
    - [INSTRUCTION LAYOUT](#instruction-layout)
    - [ARGUMENT MODE](#argument-mode)
      - [Note](#note)
  - [Interrupt vector table](#interrupt-vector-table)
    - [Layout](#layout)
    - [Interrupt vector assignments](#interrupt-vector-assignments)
      - [interrupt](#interrupt)
      - [exception](#exception)
  - [INTERRUPTS](#interrupts)
  - [Pipelining](#pipelining)
  - [Caches](#caches)
  - [Calling convention](#calling-convention)
    - [Caller](#caller)
    - [Callee](#callee)
  - [MEMORY](#memory)
    - [MEMORY LAYOUT](#memory-layout)
  - [REGISTERS](#registers)

## OVERVIEW

- Name: BCG8
- Data bus: 8 bits
- Clock speed: 10 MHz
- Address bus: 16/20 bits

## INSTRUCTIONS

### INSTRUCTION SET

see the [Instruction Set](./instructions%20BCG8.md) here

### INSTRUCTION LAYOUT

XXXXXXXX_AAAAAAAA_BBBBBBBB

X = Op code
A = argument1
B = argument2
U = unused

### ARGUMENT MODE

- 0x00: immediate byte                    byte number
- 0x01: immediate word                    word number
- 0x02: immediate tbyte                   tbyte number
- 0x03: immediate dword                   dword number
- 0x08: register                          register
- 0x09: register address                  [register]
- 0x0B: Near address                      Near [address]      a 8 bit address
- 0x0C: address                           [address]           a 16 bit address
- 0x0F: Relative address                  [byte address]      an 8 bit offset to the PC
- 0x10: 32 bit segment address            [register:register]
- 0x12: 32 bit segment DS register        [DS:register]
- 0x13: 32 bit segment DS B               [DS:B]
- 0x20: Register AL                       AL
- 0x21: Register A                        A
- 0x22: Register BL                       BL
- 0x23: Register B                        B
- 0x24: Register CL                       CL
- 0x25: Register C                        C
- 0x26: Register DL                       DL
- 0x27: Register D                        D
- 0x30: Register HL                       HL
- 0x31: Register address HL               [HL]
- 0x3A: BP relative address               [BP - number]/[BP + number]
- 0x3B: SP relative address               [SP - number]/[SP + number]

#### Note

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

when the BCG8 gets an Abort exception it will stop and the Halt flag will be set

when the BCG8 gets an Fault exception it will skip that instruction and continue

## INTERRUPTS

Interrupts can be triggered by hardware where the CPU will look into the [Interrupt vector table](#interrupt-vector-table)(IVT) and jump to the address specified by the IVT.

## Pipelining

The BCG8 can do 3 stage pipelining like this

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
   1. Push all the arguments from left to rigth onto the stack
2. Call the function using [CALL](./instructions%20BCG8.md#special-instructions)
3. Retrieve Return Value
   1. The return value will be in either the AL or HL register:
      1. Use the [HL](#registers) register if it's a pointer/address.
      2. Use the [AL](#registers) register if it's an immediate value.

### Callee

This is the sequence within the called function:

1. Save the Base Pointer
   1. Push the old [BP](#registers) onto the stack
2. Move the Base Pointer to the Stack Pointer
   1. Move the [BP](#registers) to [SP](#registers) `mov BP, SP`
3. Save All Registers
   1. Push all registers using the [PUSHR](./instructions%20BCG8.md#special-instructions) instruction.
   2. To access arguments, pop them off the stack or reference them using an offset.
4. Return Value
   1. Move the return value to the appropriate register:
      1. HL register if it's a pointer.
      2. AL register if it's an immediate value.
5. Restore Registers
   1. Pop all registers using the [POPR](./instructions%20BCG8.md#special-instructions) instruction.
6. Return from Function
    1. Return from the function with an offset depending on the size of the stack frame using the [RET](./instructions%20BCG8.md#general-instructions) instruction.

## MEMORY

The memory works by setting a segment and an offset like this

``` ACL
mov DS, 0x0080  ; moving 0x0080 into DS(data segment)
mov BX, 0x1000  ; moving 0x1000 into the BX register
mov AX, [DS:BX] ; here we are using the BX register for the offset
                ; so the real address is DS:BX or 0x00801000 
```

note: `all cells is in bytes`

### MEMORY LAYOUT

|Base Address |Size         |Name                   |Description
|-------------|-------------|-----------------------|-
|`0x0000_0000`|`0x0000_1000`| Interrupt vector table| Interrupt vector table more [here](#interrupt-vector-table)
|`0x0000_1000`|`0x0000_0200`| IO ports              | this is where the Ports is at
|`0x0000_1200`|`0x0000_EE00`| RAM                   | RAM in the first segment
|`0x0000_8000`|`0x0000_8000`| RAM Banked            | this is the data/prgram is at but banked
|`0x0001_0000`|`0x0002_8000`| extended RAM memory   | this is the data/prgram is at
|`0x0003_8000`|`0x0000_8000`| Char set              | char set
|`0x0004_0000`|`0x0002_0000`| VRAM                  | video ram
|`0x0006_0000`|`0x000A_0000`| RAM                   | this is the data/prgram is at

## REGISTERS

- A: (AH + AL)    16  bit general purpose register
- B: (BH + BL)    16  bit general purpose register
- C: (CH + CL)    16  bit general purpose register
- D: (CH + DL)    16  bit general purpose register
  - the 16 bit registers can only be used if the CR0 bit 0x04 is 1

- H     8  bit general purpose address register
- L     8  bit general purpose address register
- HL:   (H + L)   16  bit general purpose address register
  - if the Use extended registers in CR0 is 1 the HL will be 20 bits

- DS:   16  bit segment register the Data segment register
- ES:   16  bit extra data segment register
- S:    16  bit segment register

- PC:   16  bit program counter
  - if the Use extended registers in CR0 is 1 the PC will be 20 bits

- BP:   8   bit Stack register
- SP:   8   bit Stack register

- R1:   8   bit temp register
- R2:   8   bit temp register

- MB:   8   bit memory bank register

- CR0:  8   bit control register 0
  - 0 0x01
  - 1 0x02
  - 2 0x04 Use extended registers
  - 3 0x08
  - 4 0x10
  - 5 0x20
  - 6 0x40
  - 7 0x80

- CR1:  8   bit control register
  - 0 0x01
  - 1 0x02
  - 2 0x04
  - 3 0x08
  - 4 0x10
  - 5 0x20
  - 6 0x40
  - 7 0x80

- F:  8  bit (F)lags register
  - 0x01 zero
  - 0x02 equals
  - 0x04 signed
  - 0x08 carry
  - 0x10 overflow
  - 0x20 less
  - 0x40 interrupt enable
  - 0x80 HALT
