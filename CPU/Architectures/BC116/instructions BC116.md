# General instructions

- `0x0000`: MOV:  destination, source       `Moves the value from the source into the destination`
- `0x0001`: MOV:  AL, source                `Moves the value from the source into the AL register`
- `0x0002`: MOV:  BL, source                `Moves the value from the source into the BL register`
- `0x0003`: MOV:  CL, source                `Moves the value from the source into the CL register`
- `0x0004`: MOV:  DL, source                `Moves the value from the source into the DL register`
- `0x0005`: MOV:  AL, CR0                   `Moves CR0 register into the AL register`
- `0x0006`: MOV:  CR0, AL                   `Moves AL register into the CR0 register`
- `0x0100`: MOVW: destination, source       `Moves a word from the specified source to the destination`
- `0x0101`: MOVW: A, source                 `Moves a word from the specified source to the A register`
- `0x0102`: MOVW: B, source                 `Moves a word from the specified source to the B register`
- `0x0103`: MOVW: C, source                 `Moves a word from the specified source to the C register`
- `0x0104`: MOVW: D, source                 `Moves a word from the specified source to the D register`
- `0x0105`: MOVW: A, CR0                    `Moves CR0 register into the A register`
- `0x0106`: MOVW: CR0, A                    `Moves A register into the CR0 register`
- `0x0208`: MOVT: destination, source       `Moves a tbyte from the specified source to the destination`
- `0x0300`: MOVD: destination, source       `Moves a dword from the specified source to the destination`
- `0x0301`: MOVD: AX, source                `Moves a dword from the specified source to the AX register`
- `0x0302`: MOVD: BX, source                `Moves a dword from the specified source to the BX register`
- `0x0303`: MOVD: CX, source                `Moves a dword from the specified source to the CX register`
- `0x0304`: MOVD: DX, source                `Moves a dword from the specified source to the DX register`
- `0x0400`: CMP:  operand1 operand2         `Compares operand1 and operand2 and sets the flags in the flags register`
- `0x0401`: CMPZ: operand1                  `Compares operand1 and 0 and sets the flags in the flags register`
- `0x0402`: CMP:  A operand2                `Compares A and operand2 and sets the flags in the flags register`
- `0x0403`: CMP:  AX operand2               `Compares AX and operand2 and sets the flags in the flags register`
- `0x0500`: PUSH: regisger                  `Pushes the regisger onto the stack and increments the SP`
- `0x0600`: POP:  register                  `Decrements the SP and pops the current byte into the register`
- `0x0604`: POPW: register                  `Decrements the SP and pops the current word into the register`
- `0x0608`: POPT: register                  `Decrements the SP and pops the current tbyte into the register`
- `0x060C`: POPD: register                  `Decrements the SP and pops the current dword into the register`
- `0x0700`: CALL: address                   `pushes the PC register and jumps to the function specified by the address`
- `0x0701`: CALL: HL                        `pushes the PC register and jumps to the function specified by the HL register`
- `0x0800`: RET:  operand1                  `pops the PC register and subtracts the SP by operand1`
- `0x0801`: RETL:                           `pops the CS:PC registers off the stack`
- `0x0900`: RETZ:                           `pops the PC register off the stack`
- `0x0A00`: SEZ:  register                  `Sets a register to zero`
- `0x0A01`: SEZ:  AL                        `Sets the AL register to zero`
- `0x0A02`: SEZ:  BL                        `Sets the BL register to zero`
- `0x0A03`: SEZ:  CL                        `Sets the CL register to zero`
- `0x0A04`: SEZ:  DL                        `Sets the DL register to zero`
- `0x0A05`: SEZ:  A                         `Sets the A register to zero`
- `0x0A06`: SEZ:  B                         `Sets the B register to zero`
- `0x0A07`: SEZ:  C                         `Sets the C register to zero`
- `0x0A08`: SEZ:  D                         `Sets the D register to zero`
- `0x0A09`: SEZ:  AX                        `Sets the AX register to zero`
- `0x0A0A`: SEZ:  BX                        `Sets the BX register to zero`
- `0x0A0B`: SEZ:  CX                        `Sets the CX register to zero`
- `0x0A0C`: SEZ:  DX                        `Sets the DX register to zero`
- `0x0B00`: TEST: register                  `Compares the destination with itself and sets the flag`
- `0x0B01`: TEST: AL                        `Compares the AL register with itself and sets the flag`
- `0x0B02`: TEST: BL                        `Compares the BL register with itself and sets the flag`
- `0x0B03`: TEST: CL                        `Compares the CL register with itself and sets the flag`
- `0x0B04`: TEST: DL                        `Compares the DL register with itself and sets the flag`
- `0x0B05`: TEST: A                         `Compares the A register with itself and sets the flag`
- `0x0B06`: TEST: B                         `Compares the B register with itself and sets the flag`
- `0x0B07`: TEST: C                         `Compares the C register with itself and sets the flag`
- `0x0B08`: TEST: D                         `Compares the D register with itself and sets the flag`
- `0x0B09`: TEST: AX                        `Compares the AX register with itself and sets the flag`
- `0x0B0A`: TEST: BX                        `Compares the BX register with itself and sets the flag`
- `0x0B0B`: TEST: CX                        `Compares the CX register with itself and sets the flag`
- `0x0B0C`: TEST: DX                        `Compares the DX register with itself and sets the flag`
- `0x0C00`: SWAP register register          `swaps the contents of register1 with register2`

# IO instructions

- `0x1000`: OUT:  port source
- `0x1004`: OUTW: port source
- `0x1010`: INP:  port destination
- `0x1014`: INPW: port destination

# Arithmetic and logic operations

- `0x2000`: SZE:                            `Sets the zero flag`
- `0x2001`: SEE:                            `Sets the equals flag`
- `0x2002`: SES:                            `Sets the signed flag`
- `0x2003`: SEC:                            `Sets the carry flag`
- `0x2004`: SEL:                            `Sets the less flag`
- `0x2005`: SEI:                            `Sets the interrupt enable flag`
- `0x2006`: SEH:                            `Sets the halt flag`
- `0x2010`: CZE:                            `Clears the zero flag`
- `0x2011`: CLE:                            `Clears the equals flag`
- `0x2012`: CLS:                            `Clears the signed flag`
- `0x2013`: CLC:                            `Clears the carry flag`
- `0x2014`: CLL:                            `Clears the less flag`
- `0x2015`: CLI:                            `Clears the interrupt enable flag`
- `0x2016`: CLH:                            `Clears the halt flag`
- `0x2020`: ADD:    destination, source     `Adds the values of the source and the destination.`
- `0x2021`: ADD:    A, source               `Adds the values of the source and the A register.`
- `0x2022`: ADD:    AX, source              `Adds the values of the source and the AX register.`
- `0x2024`: ADC:    destination, source     `Adds the values of the source and the destination + carry.`
- `0x2025`: ADC:    A, source               `Adds the values of the source and the A register + carry.`
- `0x2026`: ADC:    AX, source              `Adds the values of the source and the AX register + carry.`
- `0x2030`: SUB:    destination, source     `Subtracts the values of the source and the destination.`
- `0x2031`: SUB:    A, source               `Subtracts the values of the source and the A register.`
- `0x2032`: SUB:    AX, source              `Subtracts the values of the source and the AX register.`
- `0x2034`: SBB:    destination, source     `Subtracts the values of the source and the destination + carry.`
- `0x2035`: SBB:    A, source               `Subtracts the values of the source and the A register + carry.`
- `0x2036`: SBB:    AX, source              `Subtracts the values of the source and the AX register + carry.`
- `0x2040`: MUL:    destination, source     `Multiplies the values of the source and the destination.`
- `0x2041`: MUL:    A, source               `Multiplies the values of the source and the A register.`
- `0x2042`: MUL:    AX, source              `Multiplies the values of the source and the AX register.`
- `0x2050`: DIV:    destination, source     `Divides the values of the source and the destination.`
- `0x2051`: DIV:    A, source               `Divides the values of the source and the A register.`
- `0x2052`: DIV:    AX, source              `Divides the values of the source and the AX register.`
- `0x2060`: AND:    destination, source     `Performs a bitwise AND operation between the destination.`
- `0x2061`: AND:    A, source               `Performs a bitwise AND operation between the A register.`
- `0x2062`: AND:    AX, source              `Performs a bitwise AND operation between the AX register.`
- `0x2070`: OR:     destination, source     `Performs a bitwise OR operation between the destination.`
- `0x2071`: OR:     A, source               `Performs a bitwise OR operation between the A register.`
- `0x2072`: OR:     AX, source              `Performs a bitwise OR operation between the AX register.`
- `0x2080`: NOR:    destination, source     `Performs a bitwise NOR operation between the destination.`
- `0x2081`: NOR:    A, source               `Performs a bitwise NOR operation between the A register.`
- `0x2082`: NOR:    AX, source              `Performs a bitwise NOR operation between the AX register.`
- `0x2090`: XOR:    destination, source     `Performs a bitwise XOR operation between the destination.`
- `0x2091`: XOR:    A, source               `Performs a bitwise XOR operation between the A register.`
- `0x2092`: XOR:    AX, source              `Performs a bitwise XOR operation between the AX register.`
- `0x20A0`: NOT:    destination             `Performs a bitwise NOT operation on the destination.`
- `0x20A1`: NOT:    A                       `Performs a bitwise NOT operation on the A register.`
- `0x20A2`: NOT:    AX                      `Performs a bitwise NOT operation on the AX register.`
- `0x20B0`: SHL:    destination, operand1   `Shifts all the bits left in the destination by specified operand1. The shift flag is the overflowing bit, and the next bit is zero.`
- `0x20C0`: SHR:    destination, operand1   `Shifts all the bits rigth in the destination by specified by operand1. The shift flag is the overflowing bit, and the next bit is zero.`
  - `0x20D0`: ROL:    destination, operand1   `Rotates all the bits left in the destination by specified by operand1. The shift flag is used as the next bit, and the overflowing bit.`
- `0x20E0`: ROR:    destination, operand1   `Rotates all the bits right in the destination by specified by operand1. The shift flag is used as the next bit, and the overflowing bit.`
- `0x20F0`: INC:    register                `Increments the value at the register by 1.`
- `0x2100`: DEC:    register                `Decrements the value at the register by 1.`
- `0x2110`: NEG:    destination             `Sets/clears the signed bit of the destination.`
- `0x2120`: EXP:    destination, operand1   `Raises the value at the destination to the power of the value specified by operand1.`
- `0x2130`: SQRT:   destination             `Calculates the square root of the destination.`
- `0x2140`: RNG:    destination             `Generates a random byte and puts the value into the destination`
- `0x2150`: SEB:    source, operand1        `Sets a bit in the source specified by the operand1`
- `0x2160`: CLB:    source, operand1        `Clears a bit in the source specified by the operand1`
- `0x2170`: MOD:    destination, source     `WIP`

# Single-Precision Float Arithmetic Operations

- `0x2190`: ADDF:   destination, source     `Adds the values of the source and the destination and stores the result in the destination.`
- `0x2191`: ADDF:   AF, source              `Adds the values of the source and the AF register and stores the result in the AF register.`
- `0x21A0`: SUBF:   destination, source     `Subtracts the source from the destination and stores the result in the destination.`
- `0x21A0`: SUBF:   AF, source              `Subtracts the source from the AF register and stores the result in the AF register.`
- `0x21B0`: MULF:   destination, source     `Multiplies the source and the destination and stores the result in the destination.`
- `0x21B0`: MULF:   AF, source              `Multiplies the source and the AF register and stores the result in the AF register.`
- `0x21C0`: DIVF:   destination, source     `Divides the destination by the source and stores the result in the destination.`
- `0x21C0`: DIVF:   AF, source              `Divides the AF register by the source and stores the result in the AF register.`
- `0x21D0`: CMPF:   destination, source     `Compares the source and destination values, setting flags accordingly.`
- `0x21E0`: SQRTF:  destination             `Computes the square root of the value in the destination and stores the result in the destination.`
- `0x21F0`: MODF:   destination, source     `Computes the modulus of the destination by the source and stores the result in the destination.`

# Conditional jumps

- `0x3000`: JMP:    address                 `Jumps to the specified address`
- `0x3010`: JZ:     address                 `Jumps to the specified address if the zero flag is set`
- `0x3011`: JNZ:    address                 `Jumps to the specified address if the zero flag is cleared`
- `0x3020`: JS:     address                 `Jumps to the specified address if the signed flag is set`
- `0x3021`: JNS:    address                 `Jumps to the specified address if the signed flag is cleared`
- `0x3030`: JE:     address                 `Jumps to the specified address if the equal flag is set`
- `0x3031`: JNE:    address                 `Jumps to the specified address if the equal flag is cleared`
- `0x3040`: JC:     address                 `Jumps to the specified address if the carry flag is set`
- `0x3041`: JNC:    address                 `Jumps to the specified address if the carry flag is cleared`
- `0x3090`: JL:     address                 `Jumps to the specified address if the less flag is set`
- `0x3091`: JG:     address                 `Jumps to the specified address if the less flag is cleared`
- `0x3092`: JLE:    address                 `Jumps to the specified address if the equal flag or the less flag is set`
- `0x3093`: JGE:    address                 `Jumps to the specified address if the equal flag is set or the less flag is cleared`
- `0x30A1`: JNV:    address                 `Jumps to the specified address if the overflow flag is cleared`

# Convert instructions

- `0x4000`: CBTA:   register, address       `Convertes the register into an ASCII string and puts the result into memory using the address and the length of the string is in the C register`

# Memory Operations instructions

- `0x4010`: CMPL:                           `Compares the memory address value in HL and DS:B for C times and updates the flags register`
- `0x4020`: MOVF:   destination, immediate  `moves a float from the specified immediate to the float register(destination)`

# Special instructions

- `0xF000`: RETI:                           `returns from an interrupt routine`
- `0xF010`: NOP:                            `No operation`
- `0xF020`: PUSHR:                          `Pushes (A B C D H L) on to the stack`
- `0xF030`: POPR:                           `Pops (A B C D H L) off the stack`
- `0xF040`: INT:    INTERRUPT_ROUTINE       `Generates an interrupt routine (more in the INTERRUPTS)`
- `0xF050`: BRK:                            `Generates a software interrupt (more in the INTERRUPTS)`
- `0xF060`: ENTER:                          `Creates a stack frame`
- `0xF070`: LEAVE:                          `Leaves the current stack frame`
- `0xF080`: CPUID:  register                `Puts some info about the CPU into the destination register`
- `0xF090`: PUSHRR:                         `Pushes R1 to R16 on to the stack`
- `0xF0A0`: POPRR:                          `Pops R1 to R16 off the stack`
- `0xF0B0`: LGDT:   address                 `Loads the GDA register with the address`
- `0xFFF0`: HALT:                           `Stops the CPU`
