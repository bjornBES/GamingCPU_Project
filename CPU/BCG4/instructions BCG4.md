# General instructions

- `0x00`: MOV       destination source      `Moves the value from the source into the destination`
- `0x01`: MOV       A source                `Moves the value from the source into the A register`
- `0x02`: CMP       A operand2              `Compares A and operand2 and sets the flags in the flags register`
- `0x03`: PUSH      register                `Pushes the register onto the stack and increments the SP`
- `0x03`: PUSH      A                       `Pushes the A register onto the stack and increments the SP`
- `0x04`: POP       register                `Decrements the SP and pops the current value into the register`
- `0x04`: POP       A                       `Decrements the SP and pops the current value into the A register`
- `0x05`: CALL      address                 `pushes the PC register and jumps to the function specified by the address`
- `0x06`: RET       operand1                `pops the PC register and subtracts the SP by operand1`
- `0x07`: RETZ                              `pops the PC register`
- `0x08`: SEZ       register                `Sets a register to zero`
- `0x09`: SEZ       A                       `Sets a A to zero`
- `0x0A`: LDA       imm16                   `Loades the HL register with the imm16`

# Arithmetic and logic operations

- `0x30`: ADD       A source      `Adds the values of the source and the A and stores the value in A.`
- `0x31`: SUB       A source      `Subtracts the values of the source and the A and stores the value in A.`
- `0x32`: MUL       A source      `Multiplies the values of the source and the A and stores the value in A.`
- `0x33`: AND       A source      `Performs a bitwise AND operation between the A and source and stores the value in A.`
- `0x34`: OR        A source      `Performs a bitwise OR operation between the A and source and stores the value in A.`
- `0x35`: NOR       A source      `Performs a bitwise NOR operation between the A and source and stores the value in A.`
- `0x36`: XOR       A source      `Performs a bitwise XOR operation between the A and source and stores the value in A.`
- `0x37`: NOT       A             `Performs a bitwise NOT operation on the A and stores the value in A.`
- `0x38`: SHL       A operand1    `Shifts all the bits left in the A by specified operand1. The {SHIFT_FLAG} is the overflowing bit, and the next bit is zero.`
- `0x39`: SHR       A operand1    `Shifts all the bits rigth in the A by specified by operand1. The {SHIFT_FLAG} is the overflowing bit, and the next bit is zero.`
- `0x3A`: ROL       A operand1    `Rotates all the bits left in the A by specified by operand1. The {SHIFT_FLAG} is used as the next bit, and the overflowing bit.`
- `0x3B`: ROR       A operand1    `Rotates all the bits right in the A by specified by operand1. The {SHIFT_FLAG} is used as the next bit, and the overflowing bit.`
- `0x3C`: INC       register                `Increments the value at the register by 1.`
- `0x3D`: DEC       register                `Decrements the value at the register by 1.`
- `0x3E`: NEG       destination             `Sets/clears the signed bit of the destination.`
- `0x3F`: SEB       source operand1         `Sets a bit in the source specified by the operand1`
- `0x40`: CLB       source operand1         `Clears a bit in the source specified by the operand1`
- `0x41`: TOB       source operand1         `Toggles a bit in the source specified by the operand1`
- `0x42`: MOD       destination source      `WIP`

# IO instructions

- `0x60`: INB       port source
- `0x61`: OUTB      port destination

# Conditional jumps

- `0x70`: JMP       address                 `Jumps to the specified address`
- `0x71`: JZ        address                 `Jumps to the specified address if the zero flag is set`
- `0x72`: JNZ       address                 `Jumps to the specified address if the zero flag is cleared`
- `0x73`: JS        address                 `Jumps to the specified address if the signed flag is set`
- `0x74`: JNS       address                 `Jumps to the specified address if the signed flag is cleared`
- `0x75`: JE        address                 `Jumps to the specified address if the equal flag is set`
- `0x76`: JNE       address                 `Jumps to the specified address if the equal flag is cleared`
- `0x77`: JL        address                 `Jumps to the specified address if the less flag is set`
- `0x78`: JG        address                 `Jumps to the specified address if the less flag is cleared`
- `0x79`: JLE       address                 `Jumps to the specified address if the equal flag or the less flag is set`
- `0x7A`: JGE       address                 `Jumps to the specified address if the equal flag is set or the less flag is cleared`
- `0x7B`: JNV       address                 `Jumps to the specified address if the overflow flag is cleared`

# Special instructions

- `0xF9`: RTI                               `returns from an interrupt routine`
- `0xFA`: NOP                               `No operation`
- `0xFB`: PUSHR                             `Pushes (A B C D E H L) on to the stack`
- `0xFC`: POPR                              `Pops (A B C D H E L) off the stack`
- `0xFE`: BRK                               `Generates a software interrupt (more in the INTERRUPTS)`
- `0xFF`: HALT                              `Stops the CPU`
