# General instructions

- `0x00`: MOV       destination source      `Moves the value from the source into the destination`
- `0x01`: MOV       AL source               `Moves the value from the source into the AL register`
- `0x02`: MOVW      destination source      `moves a word from the specified source to the destination`
- `0x03`: MOVW      A source                `Moves the value from the source into the A register`
- `0x04`: MOVT      destination source      `moves a tbyte from the specified source to the destination`
- `0x05`: MOVD      destination source      `moves a dword from the specified source to the destination`
- `0x06`: CMP       operand1 operand2       `Compares operand1 and operand2 and sets the flags in the flags register`
- `0x07`: CMP       A operand2              `Compares A and operand2 and sets the flags in the flags register`
- `0x08`: PUSH      register                `Pushes the register onto the stack and increments the SP`
- `0x09`: POP       register                `Decrements the SP and pops the current value into the register`
- `0x0A`: POPW      register                `Decrements the SP and pops the current word into the register`
- `0x0B`: POPT      register                `Decrements the SP and pops the current 24 bit into the register`
- `0x0C`: POPD      register                `Decrements the SP and pops the current dword into the register`
- `0x0D`: CALL      address                 `pushes the PC register and jumps to the function specified by the address`
- `0x0E`: RET       operand1                `pops the PC register and subtracts the SP by operand1`
- `0x0F`: RETZ                              `pops the PC register`
- `0x10`: SEZ       register                `Sets a register to zero`
- `0x11`: SEZ       AL                      `Sets a AL to zero`
- `0x12`: SEZ       A                       `Sets a A to zero`
- `0x13`: TEST      register                `Compares the destination with itself and sets the flag`
- `0x14`: TEST      AL                      `Compares the AL with itself and sets the flag`
- `0x15`: TEST      A                       `Compares the A with itself and sets the flag`

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
- `0x3D`: INC       register                `Increments the value at the register by 1.`
- `0x3E`: DEC       register                `Decrements the value at the register by 1.`
- `0x3F`: NEG       destination             `Sets/clears the signed bit of the destination.`
- `0x40`: Unused
- `0x41`: EXP       destination operand1    `Raises the value at the destination to the power of the value specified by operand1.`
- `0x42`: SQRT      destination             `Calculates the square root of the destination.`
- `0x43`: RNG       destination             `Generates a random byte and puts the value into the destination`
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
- `0x8A`: JTZ                               `Jumps to address 0x0000_0000`

# Memory Operations instructions

- `0x93`: MOVS      destination address     `moves a null terminated string from the specified address to the destination`
- `0x94`: CMPSTR    address1 address2       `Compares to null terminated strings specified by the addresses and outputs the result in the {equal} flag`
- `0x95`: MOVF      destination immediate   `moves a float from the specified immediate to the float register(destination)`
- `0x96`: MOVQ      destination immediate   `moves a qword from the specified immediate into the destination`

# Time and Date instructions

- `0x9D`: DATE      destination             `Gets the date and puts it in the destination (more in the DATE AND TIME)`
- `0x9E`: DELAY     operand1                `Sets a delay specified by the operand1 in milliseconds`
- `0x9F`: TIME      destination             `Gets the time and puts it in the destination (more in the DATE AND TIME)`

# Special instructions

- `0xF9`: RTI                               `returns from an interrupt routine`
- `0xFA`: NOP                               `No operation`
- `0xFB`: PUSHR                             `Pushes (A B C D H L) on to the stack`
- `0xFC`: POPR                              `Pops (A B C D H L) off the stack`
- `0xFD`: INT       INTERRUPT_ROUTINE       `Generates an interrupt routine (more in the INTERRUPTS)`
- `0xFE`: BRK                               `Generates a software interrupt (more in the INTERRUPTS)`
- `0xFF`: HALT                              `Stops the CPU`
