# General instructions

- `0x0000`: MOV:      destination source        `Moves the value from the source into the destination`
- `0x0001`: MOV:      AL source                 `Moves the value from the source into the AL register`
- `0x0002`: MOV:      AL CR0                    `Moves the value of the CR0 register into the AL register`
- `0x0003`: MOV:      CR0 AL                    `Moves the value of the AL register into the CR0 register`
- `0x0010`: MOVW:     destination source        `moves a word from the specified source to the destination`
- `0x0011`: MOVW:     A source                  `Moves the value from the source into the A register`
- `0x0020`: MOVT:     destination source        `moves a tbyte from the specified source to the destination`
- `0x0030`: MOVD:     destination source        `moves a dword from the specified source to the destination`
- `0x0100`: CMP:      operand1 operand2         `Compares operand1 and operand2 and sets the flags in the flags register`
- `0x0101`: CMP:      A operand2                `Compares A and operand2 and sets the flags in the flags register`
- `0x0102`: CMP:      AL operand2               `Compares AL and operand2 and sets the flags in the flags register`
- `0x0200`: PUSH:     source                    `Pushes the source onto the stack and increments the SP`
- `0x0300`: POP:      register                  `Decrements the SP and pops the current byte into the register`
- `0x0301`: POP:      AL                        `Decrements the SP and pops the current byte into the AL register`
- `0x0310`: POPW:     register                  `Decrements the SP and pops the current word into the register`
- `0x0311`: POPW:     A                         `Decrements the SP and pops the current word into the A register`
- `0x0400`: CALL:     address                   `pushes the PC register and jumps to the function specified by the address`
- `0x0410`: RET:      operand1                  `pops the PC register and subtracts the SP by operand1`
- `0x0420`: RETZ:                               `pops the PC register`
- `0x0500`: SEZ:      register                  `Sets a register to zero`
- `0x0501`: SEZ:      AL                        `Sets a AL to zero`
- `0x0502`: SEZ:      A                         `Sets a A to zero`
- `0x0600`: TEST:     register                  `Compares the destination with itself and sets the flag`
- `0x0601`: TEST:     AL                        `Compares the AL with itself and sets the flag`
- `0x0602`: TEST:     A                         `Compares the A with itself and sets the flag`

# Arithmetic and logic operations

- `0x2E00`: SEF:      flag                      `Sets a flag specified by the flag operand`
- `0x2E01`: SZE:                                `Sets the zero flag`
- `0x2E02`: SEE:                                `Sets the equals flag`
- `0x2E03`: SES:                                `Sets the signed flag`
- `0x2E04`: SEC:                                `Sets the carry flag`
- `0x2E05`: SEL:                                `Sets the less flag`
- `0x2E06`: SEI:                                `Sets the interrupt enable flag`
- `0x2E07`: SEH:                                `Sets the halt flag`
- `0x2F00`: CLF:      flag                      `Clears a flag specified by the flag operand`
- `0x2F01`: CZE:                                `Clears the zero flag`
- `0x2F02`: CLE:                                `Clears the equals flag`
- `0x2F03`: CLS:                                `Clears the signed flag`
- `0x2F04`: CLC:                                `Clears the carry flag`
- `0x2F05`: CLL:                                `Clears the less flag`
- `0x2F06`: CLI:                                `Clears the interrupt enable flag`
- `0x2F07`: CLH:                                `Clears the halt flag`
- `0x3000`: ADD:      destination source        `Adds the values of the source and the destination and stores the value in destination.`
- `0x3001`: ADD:      AL source                 `Adds the values of the source and the AL and stores the value in AL.`
- `0x3002`: ADD:      A source                  `Adds the values of the source and the A and stores the value in A.`
- `0x3100`: SUB:      destination source        `Subtracts the values of the source and the destination and stores the value in destination.`
- `0x3101`: SUB:      AL source                 `Subtracts the values of the source and the AL and stores the value in AL.`
- `0x3102`: SUB:      A source                  `Subtracts the values of the source and the A and stores the value in A.`
- `0x3200`: MUL:      destination source        `Multiplies the values of the source and the destination and stores the value in destination.`
- `0x3201`: MUL:      AL source                 `Multiplies the values of the source and the AL and stores the value in AL.`
- `0x3202`: MUL:      A source                  `Multiplies the values of the source and the A and stores the value in A.`
- `0x3300`: DIV:      destination source        `Divides the values of the source and the destination and stores the value in destination.`
- `0x3301`: DIV:      AL source                 `Divides the values of the source and the AL and stores the value in AL.`
- `0x3302`: DIV:      A source                  `Divides the values of the source and the A and stores the value in A.`
- `0x3400`: AND:      destination source        `Performs a bitwise AND operation between the destination and source and stores the value in destination.`
- `0x3500`: OR:       destination source        `Performs a bitwise OR operation between the destination and source and stores the value in destination.`
- `0x3600`: NOR:      destination source        `Performs a bitwise NOR operation between the destination and source and stores the value in destination.`
- `0x3700`: XOR:      destination source        `Performs a bitwise XOR operation between the destination and source and stores the value in destination.`
- `0x3800`: NOT:      destination               `Performs a bitwise NOT operation on the destination and stores the value in destination.`
- `0x3900`: SHL:      destination operand1      `Shifts all the bits left in the destination by specified operand1. The {SHIFT_FLAG} is the overflowing bit, and the next bit is zero.`
- `0x3A00`: SHR:      destination operand1      `Shifts all the bits rigth in the destination by specified by operand1. The {SHIFT_FLAG} is the overflowing bit, and the next bit is zero.`
- `0x3B00`: ROL:      destination operand1      `Rotates all the bits left in the destination by specified by operand1. The {SHIFT_FLAG} is used as the next bit, and the overflowing bit.`
- `0x3C00`: ROR:      destination operand1      `Rotates all the bits right in the destination by specified by operand1. The {SHIFT_FLAG} is used as the next bit, and the overflowing bit.`
- `0x3D00`: INC:      register                  `Increments the value at the register by 1.`
- `0x3E00`: DEC:      register                  `Decrements the value at the register by 1.`
- `0x3F00`: NEG:      destination               `Sets/clears the signed bit of the destination.`
- `0x4100`: EXP:      destination operand1      `Raises the value at the destination to the power of the value specified by operand1.`
- `0x4200`: SQRT:     destination               `Calculates the square root of the destination.`
- `0x4300`: RNG:      destination               `Generates a random byte and puts the value into the destination`
- `0x4400`: SEB:      source operand1           `Sets a bit in the source specified by the operand1`
- `0x4500`: CLB:      source operand1           `Clears a bit in the source specified by the operand1`
- `0x4600`: TOB:      source operand1           `Toggles a bit in the source specified by the operand1`
- `0x4700`: MOD:      destination source        `WIP`

# IO instructions

- `0x6000`: INB:      port source
- `0x6100`: OUTB:     port destination

# Conditional jumps

- `0x7000`: JMP:      address                   `Jumps to the specified address`
- `0x7100`: JZ:       address                   `Jumps to the specified address if the zero flag is set`
- `0x7200`: JNZ:      address                   `Jumps to the specified address if the zero flag is cleared`
- `0x7300`: JS:       address                   `Jumps to the specified address if the signed flag is set`
- `0x7400`: JNS:      address                   `Jumps to the specified address if the signed flag is cleared`
- `0x7500`: JE:       address                   `Jumps to the specified address if the equal flag is set`
- `0x7600`: JNE:      address                   `Jumps to the specified address if the equal flag is cleared`
- `0x7700`: JL:       address                   `Jumps to the specified address if the less flag is set`
- `0x7800`: JG:       address                   `Jumps to the specified address if the less flag is cleared`
- `0x7900`: JLE:      address                   `Jumps to the specified address if the equal flag or the less flag is set`
- `0x7A00`: JGE:      address                   `Jumps to the specified address if the equal flag is set or the less flag is cleared`
- `0x7B00`: JNV:      address                   `Jumps to the specified address if the overflow flag is cleared`
- `0x8A00`: JTZ:                                `Jumps to address 0x0000_0000`

# Convert instructions

- `0x8000`: CBTA:     destination source        `Convertes a byte into a ASCII string`

# Memory Operations instructions

- `0x9300`: MOVS:     destination address count `move an array of bytes from the specified address to the destination specified by the count`
- `0x9400`: CMPSTR:   address1 address2         `Compares to null terminated strings specified by the addresses and outputs the result in the {equal} flag`

# Special instructions

- `0xF900`: RTI:                                `returns from an interrupt routine`
- `0xFA00`: NOP:                                `No operation`
- `0xFB00`: PUSHR:                              `Pushes (A B C D H L) on to the stack`
- `0xFC00`: POPR:                               `Pops (A B C D H L) off the stack`
- `0xFD00`: INT:      INTERRUPT_ROUTINE         `Generates an interrupt routine (more in the INTERRUPTS)`
- `0xFE00`: BRK:                                `Generates a software interrupt (more in the INTERRUPTS)`
- `0xFF00`: HALT:                               `Stops the CPU`
