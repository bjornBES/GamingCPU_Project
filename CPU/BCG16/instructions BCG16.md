# General instructions

- `0x00`: MOV       destination source      `Moves the value from the source into the destination`
- `0x01`: MOVW      destination source      `moves a word from the specified source to the destination`
- `0x02`: MOVT      destination source      `moves a tbyte from the specified source to the destination`
- `0x03`: MOVD      destination source      `moves a dword from the specified source to the destination`
- `0x04`: CMP       operand1 operand2       `Compares operand1 and operand2 and sets the flags in the flags register`
- `0x05`: PUSH      register                `Pushes the register onto the stack and increments the SP`
- `0x06`: POP       register                `Decrements the SP and pops the current value into the register`
- `0x07`: POPW      register                `Decrements the SP and pops the current word into the register`
- `0x08`: POPT      register                `Decrements the SP and pops the current 24 bit into the register`
- `0x09`: POPD      register                `Decrements the SP and pops the current dword into the register`
- `0x0A`: CALL      address                 `pushes the PC register and jumps to the function specified by the address`
- `0x0B`: RET       operand1                `pops the PC register and subtracts the SP by operand1`
- `0x0C`: RETZ                              `pops the PC register`
- `0x0D`: SEZ       register                `Sets a register to zero`
- `0x0E`: TEST      register                `Compares the destination with itself and sets the flag`

# Conditional jumps

- `0x10`: JMP       address                 `Jumps to the specified address`
- `0x11`: JZ        address                 `Jumps to the specified address if the zero flag is set`
- `0x12`: JNZ       address                 `Jumps to the specified address if the zero flag is cleared`
- `0x13`: JS        address                 `Jumps to the specified address if the signed flag is set`
- `0x14`: JNS       address                 `Jumps to the specified address if the signed flag is cleared`
- `0x15`: JE        address                 `Jumps to the specified address if the equal flag is set`
- `0x16`: JNE       address                 `Jumps to the specified address if the equal flag is cleared`
- `0x17`: JL        address                 `Jumps to the specified address if the less flag is set`
- `0x18`: JG        address                 `Jumps to the specified address if the less flag is cleared`
- `0x19`: JLE       address                 `Jumps to the specified address if the equal flag or the less flag is set`
- `0x1A`: JGE       address                 `Jumps to the specified address if the equal flag is set or the less flag is cleared`
- `0x1B`: JNV       address                 `Jumps to the specified address if the overflow flag is cleared`
- `0x2A`: JTZ                               `Jumps to address 0x0000_0000`
- `0x2B`: JBA       address bank            `Jumps to the specified address and specified bank`

# Arithmetic and logic operations

- `0x2E`: SEF       flag                    `Sets a flag specified by the flag operand`
- `0x2F`: CLF       flag                    `Clears a flag specified by the flag operand`
- `0x30`: ADD       destination source      `Adds the values of the source and the destination and stores the value in destination.`
- `0x31`: SUB       destination source      `Subtracts the values of the source and the destination and stores the value in destination.`
- `0x32`: MUL       destination source      `Multiplies the values of the source and the destination and stores the value in destination.`
- `0x33`: DIV       destination source      `Divides the values of the source and the destination and stores the value in destination.`
- `0x34`: AND       destination source      `Performs a bitwise AND operation between the destination and source and stores the value in destination.`
- `0x35`: OR        destination source      `Performs a bitwise OR operation between the destination and source and stores the value in destination.`
- `0x36`: NOR       destination source      `Performs a bitwise NOR operation between the destination and source and stores the value in destination.`
- `0x37`: XOR       destination source      `Performs a bitwise XOR operation between the destination and source and stores the value in destination.`
- `0x38`: NOT       destination             `Performs a bitwise NOT operation on the destination and stores the value in destination.`
- `0x39`: SHL       destination operand1    `Shifts all the bits left in the destination by specified operand1. The {SHIFT_FLAG} is the overflowing bit, and the next bit is zero.`
- `0x3A`: SHR       destination operand1    `Shifts all the bits rigth in the destination by specified by operand1. The {SHIFT_FLAG} is the overflowing bit, and the next bit is zero.`
- `0x3B`: ROL       destination operand1    `Rotates all the bits left in the destination by specified by operand1. The {SHIFT_FLAG} is used as the next bit, and the overflowing bit.`
- `0x3C`: ROR       destination operand1    `Rotates all the bits right in the destination by specified by operand1. The {SHIFT_FLAG} is used as the next bit, and the overflowing bit.`
- `0x3D`: INC       destination             `Increments the value at the destination by 1.`
- `0x3E`: DEC       destination             `Decrements the value at the destination by 1.`
- `0x3F`: NEG       destination             `Sets/clears the signed bit of the destination.`
- `0x40`: AVG       destination operand1    `Calculates the average of the values in the destination and operand1 and puts the value in the destination.`
- `0x41`: EXP       destination operand1    `Raises the value at the destination to the power of the value specified by operand1.`
- `0x42`: SQRT      destination             `Calculates the square root of the destination.`
- `0x43`: RNG       destination             `Generates a random number and puts the value into the destination`
- `0x44`: SEB       source operand1         `Sets a bit in the source specified by the operand1`
- `0x45`: CLB       source operand1         `Clears a bit in the source specified by the operand1`
- `0x46`: TOB       source operand1         `Toggles a bit in the source specified by the operand1`
- `0x47`: MOD       destination source      `WIP`

# Float arithmetic operations

- `0x50`: FADD      destination source      `Adds the values of the source and the destination and stores the value in destination.`
- `0x51`: FSUB      destination source      `Subtracts the values of the source and the destination and stores the value in destination.`
- `0x52`: FMUL      destination source      `Multiplies the values of the source and the destination and stores the value in destination.`
- `0x53`: FDIV      destination source      `Divides the values of the source and the destination and stores the value in destination.`
- `0x54`: FAND      destination source      `Performs a bitwise AND operation between the destination and source and stores the value in destination.`
- `0x55`: FOR       destination source      `Performs a bitwise OR operation between the destination and source and stores the value in destination.`
- `0x56`: FNOR      destination source      `Performs a bitwise NOR operation between the destination and source and stores the value in destination.`
- `0x57`: FXOR      destination source      `Performs a bitwise XOR operation between the destination and source and stores the value in destination.`
- `0x58`: FNOT      destination             `Performs a bitwise NOT operation on the destination and stores the value in destination.`

# IO instructions

- `0x60`: INB       port source
- `0x61`: OUTB      port destination

# Memory Operations instructions

- `0x93`: MOVS      destination address     `moves a null terminated string from the specified address to the destination`
- `0x94`: CMPSTR    address1 address2       `Compares to null terminated strings specified by the addresses and outputs the result in the {equal} flag`
- `0x95`: MOVF      destination immediate   `moves a float from the specified immediate to the float register(destination)`

# Time and Date instructions

- `0x9D`: DATE      destination             `Gets the date and puts it in the destination (more in the DATE AND TIME)`
- `0x9E`: DELAY     operand1                `Sets a delay specified by the operand1 in milliseconds`
- `0x9F`: TIME      destination             `Gets the time and puts it in the destination (more in the DATE AND TIME)`

# Special instructions

- `0xF8`: RTI                               `returns from an intercept routine`
- `0xF9`: NOP                               `No operation`
- `0xFA`: RISERR    error_source            `Raises the error flag and sets the A register with the value from error_source`
- `0xFB`: PUSHR                             `Pushes (AX BX CX DX H L) on to the stack`
- `0xFC`: POPR                              `Pops (AX BX CX DX H L) off the stack`
- `0xFD`: INT       INTERRUPT_ROUTINE       `Generates an interrupt routine (more in the INTERRUPTS)`
- `0xFE`: BRK       INDEX                   `Generates a software interrupt and the INDEX will be moved into the X register (more in the INTERRUPTS)`
- `0xFF`: HALT                              `Stops the CPU`
