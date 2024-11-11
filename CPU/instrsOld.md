# General INFO

A register8 is one of the 8 bit register for example AL or BL

A register16 is one of the 16 bit registers for example AX or BX

A register32 is one of the 32 bit registers for example HL

A float_register is one of the float registers for example FA or FB

## terms

### tbyte

a tbyte or Tribyte is 3 bytes it can go `0x000000`/`0` from `0xFFFFFF`/`16.777.215

### dword

a dword or double word is what it says 2 words or 4 bytes

# General instructions

## 0x00 MOV destination source

**Note:** If you would like to clarify the instruction, you can replace `MOV` with `MOV byte`.

### MOV register8 immediate8

- **`0x0000`: MOV register, immediate**
  - **Description:** Moves a byte into the register from the immediate.
  - **Example:**

``` ACL
MOV AL, 0x55 ; here we are moving 0x55 into AL
```

### MOV register8 [address]

- **`0x0001`: MOV register, [address]**
  - **Description:** Moves a byte into the register from memory specified by the address.
  - **Example:**

``` ACL
MOV AL, [0x8000] ; here we are moving the contents of 0x8000 into AL from memory
```

### MOV register8 register8

- **`0x0002`: MOV register1, register2**
  - **Description:** Moves the contents of register2 into register1.
  - **Example:**

``` ACL
MOV AL, AH ; here we are moving the contents of AH into AL
```

### MOV register8 [register8]

- **`0x0003`: MOV register1, [register2]**
  - **Description:** Moves a byte into register1 from memory specified by register2.
  - **Example:**

``` ACL
MOV AL, [BL] ; Moves the byte at the memory address stored in BL into AL
```

### MOV [register8] immediate8

- **`0x0010`: MOV [register], immediate**
  - **Description:** Moves a byte immediate into memory specified by the register.
  - **Example:**

``` ACL
MOV [AX], 0x12 ; here we are moving 0x12 into the memory address stored in AX
```

### MOV [register8] [address]

- **`0x0011`: MOV [register], [address]**
  - **Description:** Moves the contents of memory specified by the address into memory specified by the register.
  - **Example:**

``` ACL
MOV [AX], [0x8000] ; here we are moving the byte at memory address 0x8000 into the memory address stored in AX
```

### MOV [register8] register8

- **`0x0012`: MOV [register1], register2**
  - **Description:** Moves the contents of register1 into memory specified by the register.
  - **Example:**

``` ACL
MOV [AX], BL ; here we are moving the contents of BL into the memory address stored in AX
```

### MOV [register8] [register8]

- **`0x0013`: MOV [register1], [register2]**
  - **Description:** Moves a byte from memory specified by register2 into memory specified by register1.
  - **Example:**

``` ACL
MOV [AX], [BX] ; here we are moving the byte at the memory address stored in BX into the memory address stored in AX
```

### MOV [address] immediate8

- **`0x0020`: MOV [address], immediate**
  - **Description:** Moves a byte immediate into memory specified by the address.
  - **Example:**

``` ACL
MOV [0x8000], 0x12 ; here we are moving 0x12 into memory address 0x8000
```

### MOV [address], [address]

- **`0x0021`: MOV [address1], [address2]**
  - **Description:** Moves a byte from memory specified by address2 into memory specified by address1.
  - **Example:**

``` ACL
MOV [0x8000], [0x8001] ; here we are moving the contents of 0x8000 to 0x8001
```

### MOV [address], register8

- **`0x0022`: MOV [address], register**
  - **Description:** Moves a byte from the register into memory specified by the address.
  - **Example:**

``` ACL
MOV [0x8000], AL ; here we are moving the contents of AL into memory address 0x8000
```

### MOV [address], [register8]

- **`0x0023`: MOV [address], [register]**
  - **Description:** Moves a byte mem using the address from the memory address stored in the register.
  - **Example:**

``` ACL
MOV [0x8000], [AX] ; Moves a byte from the memory address stored in AX into ram at 0x8000 
```

### MOV register float_register

- **`0x0040`: MOV float_register**
  - **Description:** Moves the bit in the float_register into the AX and BX registers
  - **Example:**

``` ACL
MOVF FB, 3.14f        ; Moves 3.14 into FB in hexadecimal 0x4048_F5C3
MOV FB                ; Moves the float into the AX and BX registers

; now the AX register is 0x4048
; now the BX register is 0xF5C3
```

### MOV MB, immediate8

- **`0x00F0`: MOV MB, immediate8**
  - **Description:** Moves a byte into the MB register from the immediate.
  - **Example:**

``` ACL
MOV MB, 0xF ; Moves 0xF into the MB register
; this instruction will save you a byte
```

## 0x01 MOV word destination source

**Note:** The `MOV word` can be shortened into `MOVW`.

### MOV word register16, immediate16

- **`0x0100`: MOV word register, immediate**
  - **Description:** Moves a word value into the register from the immediate.
  - **Example:**

``` ACL
MOV word AX, 0x55AA ; here we are moving 0x55AA into AX
```

### MOV word register16, [address]

- **`0x0101`: MOV word register, [address]**
  - **Description:** Moves a word into the register from memory specified by the address.
  - **Example:**

``` ACL
MOV word AX, [0x8000] ; here we are moving the word at memory address 0x8000 into AX
```

### MOV word register16, register16

- **`0x0102`: MOV word register1, register2**
  - **Description:** Moves the contents of register2 into register1.
  - **Example:**

``` ACL
MOV word AX, BX ; here we are moving the contents of BX into AX
```

### MOV word register16, [register16]

- **`0x0103`: MOV word register1, [register2]**
  - **Description:** Moves a word into register1 from memory specified by register2.
  - **Example:**

``` ACL
MOV word AX, [BX] ; here we are moving the word at the memory address stored in BX into AX
```

### MOV word [register16], immediate16

- **`0x0110`: MOV word [register], immediate**
  - **Description:** Moves a word immediate into memory specified by the register.
  - **Example:**

``` ACL
MOV word [AX], 0x0012 ; here we are moving 0x0012 into the memory address stored in AX
```

### MOV word [register16], [address]

- **`0x0111`: MOV word [register], [address]**
  - **Description:** Moves the contents of memory specified by the address into memory specified by the register.
  - **Example:**

``` ACL
MOV word [AX], [0x8000] ; here we are moving the word at memory address 0x8000 into the memory address stored in AX
```

### MOV word [register16], register16

- **`0x0112`: MOV word [register1], register2**
  - **Description:** Moves the contents of register2 into memory specified by register1.
  - **Example:**

``` ACL
MOV word [AX], BX ; here we are moving the contents of BX into the memory address stored in AX
```

### MOV word [register16], [register16]

- **`0x0113`: MOV word [register1], [register2]**
  - **Description:** Moves a word from memory specified by register2 into memory specified by register1.
  - **Example:**

``` ACL
MOV word [AX], [BX] ; here we are moving the word at the memory address stored in BX into the memory address stored in AX
```

### MOV word [Address] immediate16

- **`0x0120`: MOV word [address], immediate**
  - **Description:** Moves a word immediate into memory specified by the address.
  - **Example:**

``` ACL
MOV word [0x8000], 0x1200 ; here we are moving 0x1200 into memory address 0x8000
```

### MOV word [address1], [address2]

- **`0x0121`: MOV word [address1], [address2]**
  - **Description:** Moves a word from memory specified by address2 into memory specified by address1.
  - **Example:**

``` ACL
MOV word [0x8000], [0x8001] ; here we are moving the contents of 0x8000 to 0x8001
```

### MOV word [address], register16

- **`0x0122`: MOV word [address], register**
  - **Description:** Moves a word from the register into memory specified by the address.
  - **Example:**

``` ACL
MOV word [0x8000], AX ; here we are moving the contents of AX into memory address 0x8000
```

### MOV word [address], [register]

- **`0x0123`: MOV word [address], [register]**
  - **Description:** Moves a word from memory specified by register into memory specified by the address.
  - **Example:**

``` ACL
MOV word [0x8000], [AX] ; here we are moving the word at the memory address stored in AX into memory address 0x8000
```

### MOV AX, immediate16

- **`0x01F0`: MOV AX, immediate16**
  - **Description:** Moves a byte into the AX register from the immediate.
  - **Example:**

``` ACL
MOV AX, 0xFF ; Moves 0xFF into the AX register
; this instruction will save you a byte
```

## 0x02 MOV tbyte destination source

**Note:** The `MOV tbyte` can be shortened into `MOVT`.

### MOV Tbyte register32 Source

- **`0x0200`: MOV tbyte register, immediate**
  - **Description:** Moves a tbyte into the register from the immediate.
  - **Example:**

``` ACL
MOV HL, 0x8000_00 ; here we are moving 0x8000_00 into HL
```

- **`0x0201`: MOV tbyte register, [address]**
  - **Description:** Moves a tbyte into the register from memory specified by the address.
  - **Example:**

```ACL
MOV HL, [0x8000] ; here we are moving the tbyte at memory address 0x8000 into HL
```

- **`0x0202`: MOV tbyte register1, register2**
  - **Description:** Moves the contents of register2 into register1.
  - **Example:**

```ACL
MOV HL, BX ; here we are moving the contents of BX into HL
```

- **`0x0203`: MOV tbyte register1, [register2]**
  - **Description:** Moves a tbyte into register1 from memory specified by register2.
  - **Example:**

```ACL
MOV HL, [BX] ; here we are moving the tbyte at the memory address stored in BX into HL
```

### MOV Tbyte [register32] Source

**Note:** The `MOV tbyte` can be shortened into `MOVT`.

- **`0x0210`: MOV tbyte [register], immediate**
  - **Description:** Moves a tbyte immediate into memory specified by the register.
  - **Example:**

```ACL
MOV tbyte [AX], 0x1200_00 ; here we are moving 0x1200_00 into the memory address stored in AX
```

- **`0x0211`: MOV tbyte [register], [address]**
  - **Description:** Moves the contents of memory specified by the address into memory specified by the register.
  - **Example:**

```ACL
MOV tbyte [AX], [0x8000] ; here we are moving the tbyte at memory address 0x8000 into the memory address stored in AX
```

- **`0x0212`: MOV tbyte [register1], register2**
  - **Description:** Moves the contents of register2 into memory specified by register1.
  - **Example:**

```ACL
MOV tbyte [AX], BX ; here we are moving the contents of BX into the memory address stored in AX
```

- **`0x0213`: MOV tbyte [register1], [register2]**
  - **Description:** Moves a tbyte from memory specified by register2 into memory specified by register1.
  - **Example:**

```ACL
MOV tbyte [AX], [BX] ; here we are moving the tbyte at the memory address stored in BX into the memory address stored in AX
```

### MOV Tbyte [Address] Source

**Note:** The `MOV tbyte` can be shortened into `MOVT`.

- **`0x0220`: MOV tbyte [address], immediate**
  - **Description:** Moves a tbyte immediate into memory specified by the address.
  - **Example:**

```ACL
MOV tbyte [0x8000], 0x1200_00 ; here we are moving 0x1200_00 into memory address 0x8000
```

- **`0x0221`: MOV tbyte [address1], [address2]**
  - **Description:** Moves a tbyte from memory specified by address2 into memory specified by address1.
  - **Example:**

```ACL
MOV tbyte [0x8000], [0x8001] ; here we are moving the contents of 0x8000 to 0x8001
```

- **`0x0222`: MOV tbyte [address], register**
  - **Description:** Moves a tbyte from the register into memory specified by the address.
  - **Example:**

```ACL
MOV tbyte [0x8000], HL ; here we are moving the contents of HL into memory address 0x8000
```

- **`0x0223`: MOV tbyte [address], [register]**
  - **Description:** Moves a tbyte from memory specified by register into memory specified by the address.
  - **Example:**

```ACL
MOV tbyte [0x8000], [HL] ; here we are moving the tbyte at the memory address stored in HL into memory address 0x8000
```

## 0x03 MOV dword destination source

**Note:** The `MOV dword` can be shortened into `MOVD`.

### MOV Dword register32 Source

**Note:** The `MOV dword` can be shortened into `MOVD`.

- **`0x0300`: MOV dword register, immediate**
  - **Description:** Moves a dword value into the register from the immediate.
  - **Example:**

```ACL
MOV dword AX, 0x55AA_66FF ; here we are moving 0x55AA_66FF into AX
```

- **`0x0301`: MOV dword register, [address]**
  - **Description:** Moves a dword into the register from memory specified by the address.
  - **Example:**

```ACL
MOV dword AX, [0x8000] ; here we are moving the dword at memory address 0x8000 into AX
```

- **`0x0302`: MOV dword register1, register2**
  - **Description:** Moves the contents of register2 into register1.
  - **Example:**

```ACL
MOV dword AX, BX ; here we are moving the contents of BX into AX
```

- **`0x0303`: MOV dword register1, [register2]**
  - **Description:** Moves a dword into register1 from memory specified by register2.
  - **Example:**

```ACL
MOV dword AX, [BX] ; here we are moving the dword at the memory address stored in BX into AX
```

### MOV Dword [register32] Source

**Note:** The `MOV dword` can be shortened into `MOVD`.

- **`0x0310`: MOV dword [register], immediate**
  - **Description:** Moves a dword immediate into memory specified by the register.
  - **Example:**

```ACL
MOV dword [AX], 0x0012_0000 ; here we are moving 0x0012_0000 into the memory address stored in AX
```

- **`0x0311`: MOV dword [register], [address]**
  - **Description:** Moves the contents of memory specified by the address into memory specified by the register.
  - **Example:**

```ACL
MOV dword [AX], [0x8000] ; here we are moving the dword at memory address 0x8000 into the memory address stored in AX
```

- **`0x0312`: MOV dword [register1], register2**
  - **Description:** Moves the contents of register2 into memory specified by register1.
  - **Example:**

```ACL
MOV dword [AX], BX ; here we are moving the contents of BX into the memory address stored in AX
```

- **`0x0313`: MOV dword [register1], [register2]**
  - **Description:** Moves a dword from memory specified by register2 into memory specified by register1.
  - **Example:**

```ACL
MOV dword [AX], [BX] ; here we are moving the dword at the memory address stored in BX into the memory address stored in AX
```

### MOV Dword [Address] Source

**Note:** The `MOV dword` can be shortened into `MOVD`.

- **`0x0320`: MOV dword [address], immediate**
  - **Description:** Moves a dword immediate into memory specified by the address.
  - **Example:**

```ACL
MOV dword [0x8000], 0x1200_0000 ; here we are moving 0x1200_0000 into memory address 0x8000
```

- **`0x0321`: MOV dword [address1], [address2]**
  - **Description:** Moves a dword from memory specified by address2 into memory specified by address1.
  - **Example:**

```ACL
MOV dword [0x8000], [0x8001] ; here we are moving the contents of 0x8000 to 0x8001
```

- **`0x0322`: MOV dword [address], register**
  - **Description:** Moves a dword from the register into memory specified by the address.
  - **Example:**

```ACL
MOV dword [0x8000], AX ; here we are moving the contents of AX into memory address 0x8000
```

- **`0x0323`: MOV dword [address], [register]**
  - **Description:** Moves a dword from memory specified by register into memory specified by the address.
  - **Example:**

```ACL
MOV dword [0x8000], [AX] ; here we are moving the dword at the memory address stored in AX into memory address 0x8000
```

### MOV MB, immediate8

- **`0x00F0`: MOV HL, immediate32**
  - **Description:** Moves a dword into the HL register from the immediate.
  - **Example:**

``` ACL
MOV HL, 0xFFFF_FFFF ; Moves 0xFFFF_FFFF into the HL register
; this instruction will save you a byte
```

## 0x04 CMP operand1 operand2

### CMP register8 immediate8

- **`0x0400`: CMP register, immediate**
  - **Description:** Compares the register and immediate and sets the flags in the F register.
  - **Example:**

``` ACL
LABEL:
MOV AX, 0xF000    ; loading the AX register with 0xF000
CMP AX, 0xF001    ; compares AX with 0xF001
JNE [LABEL]       ; jumps if AX equals 0xF001 (which it dosen't)
```

### CMP register8 register8

- **`0x0401`: CMP register1, register2**
  - **Description:** Compares the register1 and register2 and sets the flags in the F register.
  - **Example:**

```ACL
LABEL:
MOV AX, 0xF000    ; loading the AX register with 0xF000
MOV BX, 0xF000    ; loading the BX register with 0xF000
CMP AX, BX        ; compares AX with BX
JE [LABEL]        ; jumps if the AX equals BX (which it does)
```

## 0x05 PUSH source

### PUSH immediate8

- **`0x0500`: PUSH immediate**
  - **Description:** `Pushes the immediate onto the stack and increments the SP register
  - **Example:**

```ACL
PUSH 0x55 ; here we are pushing 0x55 onto the stack
```

### PUSH immediate16

- **`0x0501`: PUSH immediate**
  - **Description:** `Pushes the immediate onto the stack and increments the SP register
  - **Example:**

```ACL
PUSH 0x55AA ; here we are pushing 0x55AA onto the stack
```

### PUSH immediate24

- **`0x0502`: PUSH immediate**
  - **Description:** `Pushes the immediate onto the stack and increments the SP register
  - **Example:**

```ACL
PUSH 0x55AA55 ; here we are pushing 0x55AA55 onto the stack
```

### PUSH immediate32

- **`0x0503`: PUSH immediate**
  - **Description:** `Pushes the immediate onto the stack and increments the SP register
  - **Example:**

```ACL
PUSH 0x55AA55AA ; here we are pushing 0x55AA55AA onto the stack
```

### PUSH register

- **`0x0510`: PUSH register**
  - **Description:** `Pushes the value contented in register onto the stack and increments the SP register
  - **Example:**

```ACL
PUSH AL ; here we are pushing the contents of AL onto the stack
PUSH AX ; here we are pushing the contents of AX onto the stack
PUSH HL ; here we are pushing the contents of HL onto the stack
```

## 0x06 POP register

### POP register

- **`0x0600`: POP register**
  - **Description:** `Decrements the SP register and pops the current byte off the stack and into the register
  - **Example:**
  
### POP word register16

- **`0x0610`: POP word register**
  - **Description:** `Decrements the SP register and pops the current word off the stack and into the register
  - **Example:**

### POP tbyte register32

- **`0x0620`: POP tbyte register**
  - **Description:** `Decrements the SP register and pops the current tbyte off the stack and into the register
  - **Example:**

### POP dword register32

- **`0x0630`: POP dword register**
  - **Description:** `Decrements the SP register and pops the current dword off the stack and into the register
  - **Example:**

## 0x07 CALL address

### CALL address

- **`0x0700`: CALL address**
  - **Description:** `pushes the PC register and jumps to the subroutine specified by the address`
  - **Example:**

``` ACL
; some other code
mov AL, 'C'           ; moving the letter 'C' into AL
call [Print]          ; calling the subroutine Print to print the letter 'C'

; AL is the char to be printed
Print:
  sez AH              ; AH = 0
  int 0x10            ; calling the PRINT CHAR interrupt routine because the AH register is 0
  ret 0               ; returning from the subroutine
```

## 0x08 RET immediate8

### RET immediate8

- **`0x0800`: RET immediate8**
  - **Description:** `pops the PC register and subtracts the SP by operand1`
  - **Example:**

``` ACL
; here the SP register is 0
PUSH 0xAA ; SP = 1
PUSH 0x55AA ; SP = 3
RET 3 ; SP = 0. this instruction will subtract the SP register by 3 to 0
```

## 0x09 SEZ register

### SEZ register

- **`0x0900`: SEZ register**
  - **Description:** `Sets a register to the default value
  - **Example:**

``` ACL
MOV AX, 0 ; instead of doing this do this \/
SEZ AX ; this will set the AX register to 0
SEZ FDS ; this will set the FDS register to 0xFF00
```

## 0x0A TEST register

### TEST register

- **`0x0A00`: TEST register**
  - **Description:** `Tests the destination and sets the flag`
  - **Example:**

``` ACL
MOV BX, BX ; instead of doing this do this
TEST BX ; this will test the BX register and set the flags
```

### TEST AX

- **`0x0AF0`: TEST AX**
  - **Description:** `Tests the destination and sets the flag`
  - **Example:**

``` ACL
MOV AX, AX ; instead of doing this do this
TEST AX ; this will test the AX register and set the flags
; this instruction will also save you a byte
```

## 0x50 PUSHA source

### PUSHA immediate8

- **`0x5000`: PUSHA immediate**
  - **Description:** `Pushes the immediate onto the argument stack and increments the BP register
  - **Example:**

```ACL
PUSHA 0x55 ; here we are pushing 0x55 onto the argument stack
```

### PUSHA immediate16

- **`0x5001`: PUSHA immediate**
  - **Description:** `Pushes the immediate onto the argument stack and increments the BP register
  - **Example:**

```ACL
PUSHA 0x55AA ; here we are pushing 0x55AA onto the argument stack
```

### PUSHA immediate24

- **`0x5002`: PUSHA immediate**
  - **Description:** `Pushes the immediate onto the argument stack and increments the BP register
  - **Example:**

```ACL
PUSHA 0x55AA55 ; here we are pushing 0x55AA55 onto the argument stack
```

### PUSHA immediate32

- **`0x5003`: PUSHA immediate**
  - **Description:** `Pushes the immediate onto the argument stack and increments the BP register
  - **Example:**

```ACL
PUSHA 0x55AA55AA ; here we are pushing 0x55AA55AA onto the stack
```

### PUSHA register

- **`0x5010`: PUSHA register**
  - **Description:** `Pushes the value contented in register onto the argument stack and increments the BP register
  - **Example:**

```ACL
PUSHA AL ; here we are pushing the contents of AL onto the argument stack
PUSHA AX ; here we are pushing the contents of AX onto the argument stack
PUSHA HL ; here we are pushing the contents of HL onto the argument stack
```

# Conditional jumps

## **JUMP NOTE**

- NOTE: all **`addresses`** can be over `0xFFFF` using a `long jmp` using the `long` keyword before the address like this `JMP long 0x10000 ; here we are doing a long jump to 0x10000`
- NOTE: all examples here can also be done with a `long jump`
- TERMS:
  - set is 1
  - cleared is 0

## 0x0B JMP address

### JMP address

- **`0x0B00`: JMP address**
  - **Description:** `Jumps to the specified address
  - **Example:**

``` ACL
JMP [LABEL]           ; unconditionally jumps to the LABEL

JMP long [LONG_LABEL] ; unconditionally jumps to the LONG_LABEL

.org 0x8000 
LABEL:

.org 0xF0000
LONG_LABEL:
```

## 0x0C JZ address

### JZ address

- **`0x0C00`: JZ address**
  - **Description:** `Jumps to the specified address if the zero flag is set
  - **Example:**

``` ACL
sez AX                ; the zero flag is set because we have just set AX to 0
JZ [LABEL]            ; conditionally jumps if the zero flag is set to the LABEL

inc AX                ; now AX is 1 so the zero flag is not set
JZ [LABEL]            ; this will not jump because the zero flag is not set

.org 0x8000 
LABEL:
```

## 0x0D JNZ address

### JNZ address

- **`0x0D00`: JNZ address**
  - **Description:** `Jumps to the specified address if the zero flag is cleared
  - **Example:**

``` ACL
sez AX                ; the zero flag is set because we have just set AX to 0
JNZ [LABEL]           ; this will not jump because the zero flag is set

inc AX                ; now AX is 1 so the zero flag is cleared
JNZ [LABEL]           ; conditionally jumps if the zero flag is cleared to the LABEL

.org 0x8000 
LABEL:
```

## 0x0E JS address

### JS address

- **`0x0E00`: JS address**
  - **Description:** `Jumps to the specified address if the signed if set
  - **Example:**

``` ACL
mov AX, 0x8000        ; the signed flag is set because we have just set AX to 0x8000 and the signed bit is bit
JS [LABEL]            ; conditionally jumps if the signed flag is set to the LABEL

dec AX                ; now AX is 0x7FFF so the signed flag is cleared
JS [LABEL]            ; this will not jump because the signed flag is cleared

.org 0x8000 
LABEL:
```

## 0x0F JNS address

### JNS address

- **`0x0F00`: JNS address**
  - **Description:** `Jumps to the specified address if the signed flag is cleared
  - **Example:**

``` ACL
mov AX, 0x8000        ; the signed flag is set because we have just set AX to 0x8000 and the signed bit is bit
JNS [LABEL]           ; this will not jump because the signed flag is set

dec AX                ; now AX is 0x7FFF so the signed flag is cleared
JNS [LABEL]           ; conditionally jumps if the signed flag is cleared to the LABEL

.org 0x8000 
LABEL:
```

## 0x10 JE address

### JE address

- **`0x1000`: JE address**
  - **Description:** `Jumps to the specified address if the equals flag is set
  - **Example:**

``` ACL
mov AX, 0x8000        ; moving 0x8000 into AX
cmp AX, 0x8000        ; the equals flag is set because AX equals to 0x8000
JE [LABEL]            ; conditionally jumps if the signed flag is set to the LABEL

cmp AX, 0x7FFF        ; now AX is not equals to 0x7FFF so the equals flag is cleared
JE [LABEL]            ; this will not jump because the equals flag is cleared

.org 0x8000 
LABEL:
```

## 0x11 JNE address

### JNE address

- **`0x1100`: JNE address**
  - **Description:** `Jumps to the specified address if the equals flag is cleared
  - **Example:**

``` ACL
mov AX, 0x8000        ; moving 0x8000 into AX
cmp AX, 0x8000        ; the equals flag is set because AX equals to 0x8000
JNE [LABEL]           ; this will not jump because the equals flag is set

cmp AX, 0x7FFF        ; now AX is not equals to 0x7FFF so the equals flag is cleared
JNE [LABEL]           ; conditionally jumps if the equals flag is cleared to the LABEL

.org 0x8000 
LABEL:
```

## 0x12 JL address

### JL address

- **`0x1200`: JL address**
  - **Description:** `Jumps to the specified address if the less flag is set
  - **Example:**

``` ACL
mov AX, 0x10          ; moving 0x10 into AX
cmp AX, 0x8000        ; the less flag is set because AX(0x10) is less then 0x8000
JL [LABEL]            ; conditionally jumps if the less flag is set to the LABEL

cmp AX, 0xA           ; now AX(0x10) is greater then 0xA so the less flag is cleared
JE [LABEL]            ; this will not jump because the less flag is cleared

.org 0x8000 
LABEL:
```

## 0x13 JG address

### JG address

- **`0x1300`: JG address**
  - **Description:** `Jumps to the specified address if the less flag is cleared
  - **Example:**

``` ACL
mov AX, 0x10          ; moving 0x10 into AX
cmp AX, 0x8000        ; the less flag is set because AX(0x10) is less then 0x8000
JG [LABEL]            ; conditionally jumps if the equals flag is set to the LABEL

cmp AX, 0xA           ; now AX(0x10) is greater then 0xA so the less flag is cleared
JG [LABEL]            ; this will not jump because the equals flag is cleared

.org 0x8000 
LABEL:
```

## 0x14 JLE address

### JLE address

- **`0x1400`: JLE address**
  - **Description:** `Jumps to the specified address if the less flag is set or the equals flag is set
  - **Example:**

``` ACL
; it's the same as the JL or JE
```

## 0x15 JGE address

## JGE address

- **`0x1500`: JGE address**
  - **Description:** `Jumps to the specified address if the less flag is cleared or the equals flag is set
  - **Example:**

``` ACL
; it's the same as the JG or JE
```

## 0x16 JNV address

### JNV address

- **`0x1600`: JNV address**
  - **Description:** `Jumps to the specified address if the overflow flag is cleared
  - **Example:**

``` ACL
mov AX, 0xFFFF        ; moving 0xFFFF into AX
; the overflow flag is cleared because AX(0xFFFF) has not overflowed
JNV [LABEL]           ; conditionally jumps if the equals flag is cleared to the LABEL

inc AX                ; now AX(0) has overflowed so the overflow flag is set
JNV [LABEL]           ; this will not jump because the overflow flag is set

.org 0x8000 
LABEL:
```

## 0x17 JME address

### JME address

- **`0x1600`: JME address**
  - **Description:** `Jumps to the specified address if the error flag is set
  - **Example:**

``` ACL
sef 0x8               ; setting the error flag

JME [LABEL]           ; conditionally jumps if the equals flag is cleared to the LABEL

clf 0x8               ; clearing the error flag
JME [LABEL]           ; this will not jump because the overflow flag is set

.org 0x8000 
LABEL:
```

# IO instructions

## 0x1C IN port register

### IN immediate register

- **`0x1C00`: IN immediate8, register**
  - **Description:**
  - **Example:**

``` ACL
```

### IN register register

- **`0x1C01`: IN register, register**
  - **Description:**
  - **Example:**

``` ACL
```

## 0x1D OUT port source

### OUT immediate register

- **`0x1D00`: OUT immediate8, register**
  - **Description:**
  - **Example:**

``` ACL
```

### OUT immediate8 immediate8

- **`0x1D01`: OUT immediate8, immediate8**
  - **Description:**
  - **Example:**

``` ACL
```

### OUT register immediate8

- **`0x1D02`: OUT register, immediate8**
  - **Description:**
  - **Example:**

``` ACL
```

### OUT register register

- **`0x1D03`: OUT register, register**
  - **Description:**
  - **Example:**

``` ACL
```

# Arithmetic and logic operations

## 0x1E SEF flag

### SEF immediate16

- **`0x1E00`: SEF immediate16**
  - **Description:** Sets the specified flag in the immediate16 operand.
  - **Example:**

``` ACL
Label:
SEF 0x0001    ; setting the Zero flag
JZ [Label]    ; this is now a jump instruction
```

## 0x1F CLF flag

### CLF immediate16

- **`0x1F00`: CLF immediate16**
  - **Description:** Clears the specified flag in the immediate16 operand.
  - **Example:**

``` ACL
Label:
CLF 0x0001    ; clearing the Zero flag
JZ [Label]    ; this will not jump as the Zero flag is cleared
```

## 0x20 ADD register source

### ADD register8 immediate8

- **`0x2000`: ADD register8, immediate8**
  - **Description:** Adds the immediate8 value to the value in the register8 and stores the result in the register8.
  - **Example:**

``` ACL
ADD AL, 0x05  ; adds 5 to the value in AL
```

### ADD register8 [address]

- **`0x2001`: ADD register8, [address]**
  - **Description:** Adds the memory address to the value in the register8 and stores the result in the register8.
  - **Example:**

``` ACL
ADD AL, [0x55AA]  ; adds the memory address 0x55AA to the value in AL
```

### ADD register8 register8

- **`0x2002`: ADD register8, register8**
  - **Description:** Adds the last register8 value to the value in the first register8 and stores the result in the first register8.
  - **Example:**

``` ACL
ADD AL, BL  ; adds BL to the value in AL
```

### ADD register8 [register]

- **`0x2003`: ADD register8, [register]**
  - **Description:** Adds the memory address in [register] to the value in the register8 and stores the result in the register8.
  - **Example:**

``` ACL
ADD AL, [BX]  ; adds the memory address in BX to the value in AL
```

### ADD word register16 immediate16

- **`0x2010`: ADD word register16, immediate16**
  - **Description:** Adds the immediate16 value to the value in the register16 and stores the result in the register16.
  - **Example:**

``` ACL
ADD AX, 0x1234  ; adds 0x1234 to the value in AX
```

### ADD word register16 [address]

- **`0x2011`: ADD word register16, [address]**
  - **Description:** Adds the memory address to the value in the register16 and stores the result in the register16.
  - **Example:**

``` ACL
ADD AX, [0x55AA]  ; adds the memory address 0x55AA to the value in AX
```

### ADD word register16 register16

- **`0x2012`: ADD word register16, register16**
  - **Description:** Adds the memory address to the value in the register16 and stores the result in the register16.
  - **Example:**

``` ACL
ADD word AX, BX  ; adds BX to the value in AX
```

### ADD word register16 [register]

- **`0x2013`: ADD word register16, [register]**
  - **Description:** Adds the memory address in [register] to the value in the register16 and stores the result in the register16.
  - **Example:**

``` ACL
ADD AX, [BX]  ; adds the memory address in BX to the value in AX
```

### ADD dword register32 immediate32

- **`0x2020`: ADD dword register32, immediate32**
  - **Description:** Adds the immediate32 value to the value in the register32 and stores the result in the register32.
  - **Example:**

``` ACL
ADD HL, 0x1234  ; adds 0x1234 to the value in HL
```

### ADD dword register32 [address]

- **`0x2021`: ADD dword register32, [address]**
  - **Description:** Adds the memory address to the value in the register32 and stores the result in the register32.
  - **Example:**

``` ACL
ADD HL, [0x55AA]  ; adds the memory address 0x55AA to the value in HL
```

### ADD dword register32 register32

- **`0x2022`: ADD dword register32, register32**
  - **Description:** Adds the memory address to the value in the register32 and stores the result in the register32.
  - **Example:**

``` ACL
ADD word HL, SP  ; adds SP to the value in HL
```

### ADD dword register32 [register]

- **`0x2023`: ADD dword register32, [register]**
  - **Description:** Adds the memory address in [register] to the value in the register32 and stores the result in the register32.
  - **Example:**

``` ACL
ADD HL, [BX]  ; adds the memory address in BX to the value in HL
```

## 0x21 SUB register source

### SUB register8 immediate8

- **`0x2100`: SUB register8, immediate8**
  - **Description:** Subtracts the immediate8 value from the value in register8 and stores the result in register8.
  - **Example:**

``` ACL
SUB AL, 0x05  ; subtracts 5 from the value in AL
```

### SUB register8 [address]

- **`0x2101`: SUB register8, [address]**
  - **Description:** Subtracts the value at memory address [address] from the value in register8 and stores the result in register8.
  - **Example:**

``` ACL
SUB AL, [0x55AA]  ; subtracts the value at memory address 0x55AA from the value in AL
```

### SUB register8 register8

- **`0x2102`: SUB register8, register8**
  - **Description:** Subtracts the value in the second register8 from the value in the first register8 and stores the result in the first register8.
  - **Example:**

``` ACL
SUB AL, BL  ; subtracts BL from the value in AL
```

### SUB register8 [register]

- **`0x2103`: SUB register8, [register]**
  - **Description:** Subtracts the value at memory address in [register] from the value in register8 and stores the result in register8.
  - **Example:**

``` ACL
SUB AL, [BX]  ; subtracts the value at memory address in BX from the value in AL
```

### SUB word register16 immediate16

- **`0x2110`: SUB word register16, immediate16**
  - **Description:** Subtracts the immediate16 value from the value in register16 and stores the result in register16.
  - **Example:**

``` ACL
SUB AX, 0x1234  ; subtracts 0x1234 from the value in AX
```

### SUB word register16 [address]

- **`0x2111`: SUB word register16, [address]**
  - **Description:** Subtracts the value at memory address [address] from the value in register16 and stores the result in register16.
  - **Example:**

``` ACL
SUB AX, [0x55AA]  ; subtracts the value at memory address 0x55AA from the value in AX
```

### SUB word register16 register16

- **`0x2112`: SUB word register16, register16**
  - **Description:** Subtracts the value in the second register16 from the value in the first register16 and stores the result in the first register16.
  - **Example:**

``` ACL
SUB AX, BX  ; subtracts BX from the value in AX
```

### SUB word register16 [register]

- **`0x2113`: SUB word register16, [register]**
  - **Description:** Subtracts the value at memory address in [register] from the value in register16 and stores the result in register16.
  - **Example:**

``` ACL
SUB AX, [BX]  ; subtracts the value at memory address in BX from the value in AX
```

### SUB dword register32 immediate32

- **`0x2120`: SUB dword register32, immediate32**
  - **Description:** Subtracts the immediate32 value from the value in register32 and stores the result in register32.
  - **Example:**

``` ACL
SUB HL, 0x1234  ; subtracts 0x1234 from the value in HL
```

### SUB dword register32 [address]

- **`0x2121`: SUB dword register32, [address]**
  - **Description:** Subtracts the value at memory address [address] from the value in register32 and stores the result in register32.
  - **Example:**

``` ACL
SUB HL, [0x55AA]  ; subtracts the value at memory address 0x55AA from the value in HL
```

### SUB dword register32 register32

- **`0x2122`: SUB dword register32, register32**
  - **Description:** Subtracts the value in the second register32 from the value in the first register32 and stores the result in the first register32.
  - **Example:**

``` ACL
SUB HL, SP  ; subtracts SP from the value in HL
```

### SUB dword register32 [register]

- **`0x2123`: SUB dword register32, [register]**
  - **Description:** Subtracts the value at memory address in [register] from the value in register32 and stores the result in register32.
  - **Example:**

``` ACL
SUB HL, [BX]  ; subtracts the value at memory address in BX from the value in HL
```

## 0x22 MUL register source

### MUL register8 immediate8

- **`0x2200`: MUL register8, immediate8**
  - **Description:** Multiplies the value in register8 by the immediate8 value and stores the result in register8 (8-bit unsigned multiplication).
  - **Example:**

``` ACL
MUL AL, 0x05  ; multiplies AL by 5
```

### MUL register8 [address]

- **`0x2201`: MUL register8, [address]**
  - **Description:** Multiplies the value in register8 by the value at memory address [address] and stores the result in register8 (8-bit unsigned multiplication).
  - **Example:**

``` ACL
MUL AL, [0x55AA]  ; multiplies AL by the value at memory address 0x55AA
```

### MUL register8 register8

- **`0x2202`: MUL register8, register8**
  - **Description:** Multiplies the value in the first register8 by the value in the second register8 and stores the result in the first register8 (8-bit unsigned multiplication).
  - **Example:**

``` ACL
MUL AL, BL  ; multiplies AL by BL
```

### MUL register8 [register]

- **`0x2203`: MUL register8, [register]**
  - **Description:** Multiplies the value in register8 by the value at memory address in [register] and stores the result in register8 (8-bit unsigned multiplication).
  - **Example:**

``` ACL
MUL AL, [BX]  ; multiplies AL by the value at memory address in BX
```

### MUL word register16 immediate16

- **`0x2210`: MUL word register16, immediate16**
  - **Description:** Multiplies the value in register16 by the immediate16 value and stores the result in register16 (16-bit unsigned multiplication).
  - **Example:**

``` ACL
MUL AX, 0x1234  ; multiplies AX by 0x1234
```

### MUL word register16 [address]

- **`0x2211`: MUL word register16, [address]**
  - **Description:** Multiplies the value in register16 by the value at memory address [address] and stores the result in register16 (16-bit unsigned multiplication).
  - **Example:**

``` ACL
MUL AX, [0x55AA]  ; multiplies AX by the value at memory address 0x55AA
```

### MUL word register16 register16

- **`0x2212`: MUL word register16, register16**
  - **Description:** Multiplies the value in the first register16 by the value in the second register16 and stores the result in the first register16 (16-bit unsigned multiplication).
  - **Example:**

``` ACL
MUL AX, BX  ; multiplies AX by BX
```

### MUL word register16 [register]

- **`0x2213`: MUL word register16, [register]**
  - **Description:** Multiplies the value in register16 by the value at memory address in [register] and stores the result in register16 (16-bit unsigned multiplication).
  - **Example:**

``` ACL
MUL AX, [BX]  ; multiplies AX by the value at memory address in BX
```

### MUL dword register32 immediate32

- **`0x2220`: MUL dword register32, immediate32**
  - **Description:** Multiplies the value in register32 by the immediate32 value and stores the result in register32 (32-bit unsigned multiplication).
  - **Example:**

``` ACL
MUL HL, 0x1234  ; multiplies HL by 0x1234
```

### MUL dword register32 [address]

- **`0x2221`: MUL dword register32, [address]**
  - **Description:** Multiplies the value in register32 by the value at memory address [address] and stores the result in register32 (32-bit unsigned multiplication).
  - **Example:**

``` ACL
MUL HL, [0x55AA]  ; multiplies HL by the value at memory address 0x55AA
```

### MUL dword register32 register32

- **`0x2222`: MUL dword register32, register32**
  - **Description:** Multiplies the value in the first register32 by the value in the second register32 and stores the result in the first register32 (32-bit unsigned multiplication).
  - **Example:**

``` ACL


MUL HL, SP  ; multiplies HL by SP
```

### MUL dword register32 [register]

- **`0x2223`: MUL dword register32, [register]**
  - **Description:** Multiplies the value in register32 by the value at memory address in [register] and stores the result in register32 (32-bit unsigned multiplication).
  - **Example:**

``` ACL
MUL HL, [BX]  ; multiplies HL by the value at memory address in BX
```

## 0x23 DIV register source

### DIV register8 immediate8

- **`0x2300`: DIV register8, immediate8**
  - **Description:** Divides the value in register8 by the immediate8 value and stores the quotient in register8 and the remainder in the designated flag register (8-bit unsigned division).
  - **Example:**

``` ACL
DIV AL, 0x05  ; divides AL by 5
```

### DIV register8 [address]

- **`0x2301`: DIV register8, [address]**
  - **Description:** Divides the value in register8 by the value at memory address [address] and stores the quotient in register8 and the remainder in the designated flag register (8-bit unsigned division).
  - **Example:**

``` ACL
DIV AL, [0x55AA]  ; divides AL by the value at memory address 0x55AA
```

### DIV register8 register8

- **`0x2302`: DIV register8, register8**
  - **Description:** Divides the value in the first register8 by the value in the second register8 and stores the quotient in the first register8 and the remainder in the designated flag register (8-bit unsigned division).
  - **Example:**

``` ACL
DIV AL, BL  ; divides AL by BL
```

### DIV register8 [register]

- **`0x2303`: DIV register8, [register]**
  - **Description:** Divides the value in register8 by the value at memory address in [register] and stores the quotient in register8 and the remainder in the designated flag register (8-bit unsigned division).
  - **Example:**

``` ACL
DIV AL, [BX]  ; divides AL by the value at memory address in BX
```

### DIV word register16 immediate16

- **`0x2310`: DIV word register16, immediate16**
  - **Description:** Divides the value in register16 by the immediate16 value and stores the quotient in register16 and the remainder in the designated flag register (16-bit unsigned division).
  - **Example:**

``` ACL
DIV AX, 0x1234  ; divides AX by 0x1234
```

### DIV word register16 [address]

- **`0x2311`: DIV word register16, [address]**
  - **Description:** Divides the value in register16 by the value at memory address [address] and stores the quotient in register16 and the remainder in the designated flag register (16-bit unsigned division).
  - **Example:**

``` ACL
DIV AX, [0x55AA]  ; divides AX by the value at memory address 0x55AA
```

### DIV word register16 register16

- **`0x2312`: DIV word register16, register16**
  - **Description:** Divides the value in the first register16 by the value in the second register16 and stores the quotient in the first register16 and the remainder in the designated flag register (16-bit unsigned division).
  - **Example:**

``` ACL
DIV AX, BX  ; divides AX by BX
```

### DIV word register16 [register]

- **`0x2313`: DIV word register16, [register]**
  - **Description:** Divides the value in register16 by the value at memory address in [register] and stores the quotient in register16 and the remainder in the designated flag register (16-bit unsigned division).
  - **Example:**

``` ACL
DIV AX, [BX]  ; divides AX by the value at memory address in BX
```

### DIV dword register32 immediate32

- **`0x2320`: DIV dword register32, immediate32**
  - **Description:** Divides the value in register32 by the immediate32 value and stores the quotient in register32 and the remainder in the designated flag register (32-bit unsigned division).
  - **Example:**

``` ACL
DIV HL, 0x1234  ; divides HL by 0x1234
```

### DIV dword register32 [address]

- **`0x2321`: DIV dword register32, [address]**
  - **Description:** Divides the value in register32 by the value at memory address [address] and stores the quotient in register32 and the remainder in the designated flag register (32-bit unsigned division).
  - **Example:**

``` ACL
DIV HL, [0x55AA]  ; divides HL by the value at memory address 0x55AA
```

### DIV dword register32 register32

- **`0x2322`: DIV dword register32, register32**
  - **Description:** Divides the value in the first register32 by the value in the second register32 and stores the quotient in the first register32 and the remainder in the designated flag register (32-bit unsigned division).
  - **Example:**

``` ACL
DIV HL, SP  ; divides HL by SP
```

### DIV dword register32 [register]

- **`0x2323`: DIV dword register32, [register]**
  - **Description:** Divides the value in register32 by the value at memory address in [register] and stores the quotient in register32 and the remainder in the designated flag register (32-bit unsigned division).
  - **Example:**

``` ACL
DIV HL, [BX]  ; divides HL by the value at memory address in BX
```

## 0x24 AND register source

### AND register8 immediate8

- **`0x2400`: AND register8, immediate8**
  - **Description:** Performs a bitwise AND operation between the value in register8 and the immediate8 value, and stores the result in register8.
  - **Example:**

``` ACL
AND AL, 0x0F  ; performs AND operation between AL and 0x0F
```

### AND register8 [address]

- **`0x2401`: AND register8, [address]**
  - **Description:** Performs a bitwise AND operation between the value in register8 and the value at memory address [address], and stores the result in register8.
  - **Example:**

``` ACL
AND AL, [0x55AA]  ; performs AND operation between AL and the value at memory address 0x55AA
```

### AND register8 register8

- **`0x2402`: AND register8, register8**
  - **Description:** Performs a bitwise AND operation between the value in the second register8 and the value in the first register8, and stores the result in the first register8.
  - **Example:**

``` ACL
AND AL, BL  ; performs AND operation between AL and BL
```

### AND register8 [register]

- **`0x2403`: AND register8, [register]**
  - **Description:** Performs a bitwise AND operation between the value in register8 and the value at memory address in [register], and stores the result in register8.
  - **Example:**

``` ACL
AND AL, [BX]  ; performs AND operation between AL and the value at memory address in BX
```

### AND word register16 immediate16

- **`0x2410`: AND word register16, immediate16**
  - **Description:** Performs a bitwise AND operation between the value in register16 and the immediate16 value, and stores the result in register16.
  - **Example:**

``` ACL
AND AX, 0xFFFF  ; performs AND operation between AX and 0xFFFF
```

### AND word register16 [address]

- **`0x2411`: AND word register16, [address]**
  - **Description:** Performs a bitwise AND operation between the value in register16 and the value at memory address [address], and stores the result in register16.
  - **Example:**

``` ACL
AND AX, [0x55AA]  ; performs AND operation between AX and the value at memory address 0x55AA
```

### AND word register16 register16

- **`0x2412`: AND word register16, register16**
  - **Description:** Performs a bitwise AND operation between the value in the second register16 and the value in the first register16, and stores the result in the first register16.
  - **Example:**

``` ACL
AND AX, BX  ; performs AND operation between AX and BX
```

### AND word register16 [register]

- **`0x2413`: AND word register16, [register]**
  - **Description:** Performs a bitwise AND operation between the value in register16 and the value at memory address in [register], and stores the result in register16.
  - **Example:**

``` ACL
AND AX, [BX]  ; performs AND operation between AX and the value at memory address in BX
```

### AND dword register32 immediate32

- **`0x2420`: AND dword register32, immediate32**
  - **Description:** Performs a bitwise AND operation between the value in register32 and the immediate32 value, and stores the result in register32.
  - **Example:**

``` ACL
AND HL, 0

xFFFFFFFF  ; performs AND operation between HL and 0xFFFFFFFF
```

### AND dword register32 [address]

- **`0x2421`: AND dword register32, [address]**
  - **Description:** Performs a bitwise AND operation between the value in register32 and the value at memory address [address], and stores the result in register32.
  - **Example:**

``` ACL
AND HL, [0x55AA]  ; performs AND operation between HL and the value at memory address 0x55AA
```

### AND dword register32 register32

- **`0x2422`: AND dword register32, register32**
  - **Description:** Performs a bitwise AND operation between the value in the second register32 and the value in the first register32, and stores the result in the first register32.
  - **Example:**

``` ACL
AND HL, SP  ; performs AND operation between HL and SP
```

### AND dword register32 [register]

- **`0x2423`: AND dword register32, [register]**
  - **Description:** Performs a bitwise AND operation between the value in register32 and the value at memory address in [register], and stores the result in register32.
  - **Example:**

``` ACL
AND HL, [BX]  ; performs AND operation between HL and the value at memory address in BX
```

## 0x25 OR register source

### OR register8 immediate8

- **`0x2500`: OR register8, immediate8**
  - **Description:** Performs a bitwise OR operation between the value in register8 and the immediate8 value, and stores the result in register8.
  - **Example:**

``` ACL
OR AL, 0x0F  ; performs OR operation between AL and 0x0F
```

### OR register8 [address]

- **`0x2501`: OR register8, [address]**
  - **Description:** Performs a bitwise OR operation between the value in register8 and the value at memory address [address], and stores the result in register8.
  - **Example:**

``` ACL
OR AL, [0x55AA]  ; performs OR operation between AL and the value at memory address 0x55AA
```

### OR register8 register8

- **`0x2502`: OR register8, register8**
  - **Description:** Performs a bitwise OR operation between the value in the second register8 and the value in the first register8, and stores the result in the first register8.
  - **Example:**

``` ACL
OR AL, BL  ; performs OR operation between AL and BL
```

### OR register8 [register]

- **`0x2503`: OR register8, [register]**
  - **Description:** Performs a bitwise OR operation between the value in register8 and the value at memory address in [register], and stores the result in register8.
  - **Example:**

``` ACL
OR AL, [BX]  ; performs OR operation between AL and the value at memory address in BX
```

### OR word register16 immediate16

- **`0x2510`: OR word register16, immediate16**
  - **Description:** Performs a bitwise OR operation between the value in register16 and the immediate16 value, and stores the result in register16.
  - **Example:**

``` ACL
OR AX, 0xFFFF  ; performs OR operation between AX and 0xFFFF
```

### OR word register16 [address]

- **`0x2511`: OR word register16, [address]**
  - **Description:** Performs a bitwise OR operation between the value in register16 and the value at memory address [address], and stores the result in register16.
  - **Example:**

``` ACL
OR AX, [0x55AA]  ; performs OR operation between AX and the value at memory address 0x55AA
```

### OR word register16 register16

- **`0x2512`: OR word register16, register16**
  - **Description:** Performs a bitwise OR operation between the value in the second register16 and the value in the first register16, and stores the result in the first register16.
  - **Example:**

``` ACL
OR AX, BX  ; performs OR operation between AX and BX
```

### OR word register16 [register]

- **`0x2513`: OR word register16, [register]**
  - **Description:** Performs a bitwise OR operation between the value in register16 and the value at memory address in [register], and stores the result in register16.
  - **Example:**

``` ACL
OR AX, [BX]  ; performs OR operation between AX and the value at memory address in BX
```

### OR dword register32 immediate32

- **`0x2520`: OR dword register32, immediate32**
  - **Description:** Performs a bitwise OR operation between the value in register32 and the immediate32 value, and stores the result in register32.
  - **Example:**

``` ACL
OR HL, 0xFFFFFFFF  ; performs OR operation between HL and 0xFFFFFFFF
```

### OR dword register32 [address]

- **`0x2521`: OR dword register32, [address]**
  - **Description:** Performs a bitwise OR operation between the value in register32 and the value at memory address [address], and stores the result in register32.
  - **Example:**

``` ACL
OR HL, [0x55AA]  ; performs OR operation between HL and the value at memory address 0x55AA
```

### OR dword register32 register32

- **`0x2522`: OR dword register32, register32**
  - **Description:** Performs a bitwise OR operation between the value in the second register32 and the value in the first register32, and stores the result in the first register32.
  - **Example:**

``` ACL
OR HL, SP  ; performs OR operation between HL and SP
```

### OR dword register32 [register]

- **`0x2523`: OR dword register32, [register]**
  - **Description:** Performs a bitwise OR operation between the value in register32 and the value at memory address in [register], and stores the result in register32.
  - **Example:**

``` ACL
OR HL, [BX]  ; performs OR operation between HL and the value at memory address in BX
```

## 0x26 NOR register source

### NOR register8 immediate8

- **`0x2600`: NOR register8, immediate8**
  - **Description:** Performs a bitwise NOR operation between the value in register8 and the immediate8 value, and stores the result in register8.
  - **Example:**

``` ACL
NOR AL, 0x0F  ; performs NOR operation between AL and 0x0F
```

### NOR register8 [address]

- **`0x2601`: NOR register8, [address]**
  - **Description:** Performs a bitwise NOR operation between the value in register8 and the value at memory address [address], and stores the result in register8.
  - **Example:**

``` ACL
NOR AL, [0x55AA]  ; performs NOR operation between AL and the value at memory address 0x55AA
```

### NOR register8 register8

- **`0x2602`: NOR register8, register8**
  - **Description:** Performs a bitwise NOR operation between the value in the second register8 and the value in the first register8, and stores the result in the first register8.
  - **Example:**

``` ACL
NOR AL, BL  ; performs NOR operation between AL and BL
```

### NOR register8 [register]

- **`0x2603`: NOR register8, [register]**
  - **Description:** Performs a bitwise NOR operation between the value in register8 and the value at memory address in [register], and stores the result in register8.
  - **Example:**

``` ACL
NOR AL, [BX]  ; performs NOR operation between AL and the value at memory address in BX
```

### NOR word register16 immediate16

- **`0x2610`: NOR word register16, immediate16**
  - **Description:** Performs a bitwise NOR operation between the value in register16 and the immediate16 value, and stores the result in register16.
  - **Example:**

``` ACL
NOR AX, 0xFFFF  ; performs NOR operation between AX and 0xFFFF
```

### NOR word register16 [address]

- **`0x2611`: NOR word register16, [address]**
  - **Description:** Performs a bitwise NOR operation between the value in register16 and the value at memory address [address], and stores the result in register16.
  - **Example:**

``` ACL
NOR AX, [0x55AA]  ; performs NOR operation between AX and the value at memory address 0x55AA
```

### NOR word register16 register16

- **`0x2612`: NOR word register16, register16**
  - **Description:** Performs a bitwise NOR operation between the value in the second register16 and the value in the first register16, and stores the result in the first register16.
  - **Example:**

``` ACL
NOR AX, BX  ; performs NOR operation between AX and BX
```

### NOR word register16 [register]

- **`0x2613`: NOR word register16, [register]**
  - **Description:** Performs a bitwise NOR operation between the value in register16 and the value at memory address in [register], and stores the result in register16.
  - **Example:**

``` ACL
NOR AX, [BX]  ; performs NOR operation between AX and the value at memory address in BX
```

### NOR dword register32 immediate32

- **`0x2620`: NOR dword register32, immediate32**
  - **Description:** Performs a bitwise NOR operation between the value in register32 and the immediate32 value, and stores the result in register32.
  - **Example:**

``` ACL
NOR HL, 0xFFFFFFFF  ; performs NOR operation between HL and 0xFFFFFFFF
```

### NOR dword register32 [address]

- **`0x2621`: NOR dword register32, [address]**
  - **Description:** Performs a bitwise NOR operation between the value in register32 and the value at memory address [address], and stores the result in register32.
  - **Example:**

``` ACL
NOR HL, [0x55AA]  ; performs NOR operation between HL and the value at memory address 0x55AA
```

### NOR dword register32 register32

- **`0x2622`: NOR dword register32, register32**
  - **Description:** Performs a bitwise NOR operation between the value in the second register32 and the value in the first register32, and stores the result in the first register32.
  - **Example:**

``` ACL
NOR HL, SP  ; performs NOR operation between HL and SP
```

### NOR dword register32 [register]

- **`0x2623`: NOR dword register32, [register]**
  - **Description:** Performs a bitwise NOR operation between the value in register32 and the value at memory address in [register], and stores the result in register32.
  - **Example:**

``` ACL
NOR HL, [BX]  ; performs NOR operation between HL and the value at memory address in BX
```

## 0x27 NOT register

### NOT register8

- **`0x2700`: NOT register8**
  - **Description:** Performs a bitwise NOT operation (bit inversion) on the value in register8.
  - **Example:**

``` ACL
NOT AL  ; performs bitwise NOT operation on AL
```

### NOT word register16

- **`0x2710`: NOT word register16**
  - **Description:** Performs a bitwise NOT operation (bit inversion) on the value in register16.
  - **Example:**

``` ACL
NOT AX  ; performs bitwise NOT operation on AX
```

### NOT dword register32

- **`0x2720`: NOT dword register32**
  - **Description:** Performs a bitwise NOT operation (bit inversion) on the value in register32.
  - **Example:**

``` ACL
NOT HL  ; performs bitwise NOT operation on HL
```

I apologize for the confusion earlier. Here are the revised instructions without specifying CL or any other specific registers in the syntax:

## 0x28 XOR register source

### XOR register8 immediate8

- **`0x2800`: XOR register8, immediate8**
  - **Description:** Performs a bitwise XOR operation between the value in register8 and the immediate8 value, and stores the result in register8.
  - **Example:**

``` ACL
XOR AL, 0x0F  ; performs XOR operation between AL and 0x0F
```

### XOR register8 [address]

- **`0x2801`: XOR register8, [address]**
  - **Description:** Performs a bitwise XOR operation between the value in register8 and the value at memory address [address], and stores the result in register8.
  - **Example:**

``` ACL
XOR AL, [0x55AA]  ; performs XOR operation between AL and the value at memory address 0x55AA
```

### XOR register8 register8

- **`0x2802`: XOR register8, register8**
  - **Description:** Performs a bitwise XOR operation between the value in the second register8 and the value in the first register8, and stores the result in the first register8.
  - **Example:**

``` ACL
XOR AL, BL  ; performs XOR operation between AL and BL
```

- **`0x2803`: XOR register8, [register]**
  - **Description:** Performs a bitwise XOR operation between the value in register8 and the value at memory address in [register], and stores the result in register8.
  - **Example:**

``` ACL
XOR AL, [BX]  ; performs XOR operation between AL and the value at memory address in BX
```

## 0x29 SHL destination operand1

### SHL register immediate16

- **`0x2900`: SHL register, immediate16**
  - **Description:** Shifts all the bits left in register by the number of bits specified by immediate16. The {SHIFT_FLAG} is the overflowing bit, and the next bit is zero.
  - **Example:**

``` ACL
SHL AL, 3   ; shifts AL left by 3 bits
SHL AX, 3   ; shifts AX left by 3 bits
SHL HL, 3   ; shifts HL left by 3 bits
```

## 0x2A SHR destination operand1

### SHR register immediate16

- **`0x2A00`: SHR register, immediate16**
  - **Description:** Shifts all the bits right in register by the number of bits specified by immediate16. The {SHIFT_FLAG} is the overflowing bit, and the next bit is zero.
  - **Example:**

``` ACL
SHR AL, 2  ; shifts AL right by 2 bits
SHR AX, 3  ; shifts AX right by 3 bits
SHR HL, 6  ; shifts HL right by 6 bits
```

## 0x2B ROL destination operand1

### ROL register immediate16

- **`0x2B00`: ROL register, immediate16**
  - **Description:** Rotates all the bits left in register by the number of bits specified by immediate16. The {SHIFT_FLAG} is used as the next bit, and the overflowing bit is rotated to the least significant bit.
  - **Example:**

``` ACL
ROL AL, 1  ; rotates AL left by 1 bit
ROL AX, 2  ; rotates AX left by 2 bits
ROL HL, 3  ; rotates HL left by 3 bits
```

## 0x2C ROR destination operand1

### ROR register immediate16

- **`0x2C00`: ROR register, immediate16**
  - **Description:** Rotates all the bits right in register by the number of bits specified by immediate. The {SHIFT_FLAG} is used as the next bit, and the overflowing bit is rotated to the most significant bit.
  - **Example:**

``` ACL
ROR AL, 2  ; rotates AL right by 2 bits
ROR AX, 4  ; rotates AX right by 4 bits
ROR HL, 5  ; rotates HL right by 5 bits
```

## 0x2D INC destination

### INC register

- **`0x2D00`: INC register**
  - **Description:** Increments the value in register by 1.
  - **Example:**

``` ACL
INC AL  ; increments AL by 1
INC AX  ; increments AX by 1
INC HL  ; increments HL by 1
```

## 0x2E DEC destination

### DEC register

- **`0x2E00`: DEC register**
  - **Description:** Decrements the value in register by 1.
  - **Example:**

``` ACL
DEC AL  ; decrements AL by 1
DEC AX  ; decrements AX by 1
DEC HL  ; decrements HL by 1
```

## 0x2F NEG destination

### NEG register8

- **`0x2F00`: NEG register8**
  - **Description:** Sets/clears the signed bit of the value in register8.
  - **Example:**

``` ACL
NEG AL  ; sets/clears the signed bit of AL
```

### NEG register16

- **`0x2F01`: NEG register16**
  - **Description:** Sets/clears the signed bit of the value in register16.
  - **Example:**

``` ACL
NEG AX  ; sets/clears the signed bit of AX
```

### NEG register32

- **`0x2F02`: NEG register32**
  - **Description:** Sets/clears the signed bit of the value in register32.
  - **Example:**

``` ACL
NEG HL  ; sets/clears the signed bit of HL
```

## 0x30 AVG destination operand1

### AVG register8 immediate8

- **`0x3000`: AVG register8, immediate8**
  - **Description:** Calculates the average of the value in register8 and the immediate8 value, and stores the result in register8.
  - **Example:**

``` ACL
AVG AL, 0x0A  ; calculates the average of AL and 0x0A
```

### AVG register16 immediate8

- **`0x3001`: AVG register16, immediate8**
  - **Description:** Calculates the average of the value in register16 and the immediate8 value, and stores the result in register16.
  - **Example:**

``` ACL
AVG AX, 0x10  ; calculates the average of AX and 0x10
```

### AVG register32 immediate8

- **`0x3002`: AVG register32, immediate8**
  - **Description:** Calculates the average of the value in register32 and the immediate8 value, and stores the result in register32.
  - **Example:**

``` ACL
AVG HL, 0x0F  ; calculates the average of HL and 0x0F
```

## 0x31 EXP destination operand1

### EXP register8 immediate8

- **`0x3100`: EXP register8, immediate8**
  - **Description:** Raises the value in register8 to the power of the immediate8 value, and stores the result in register8.
  - **Example:**

``` ACL
EXP AL, 0x02  ; raises AL to the power of 2
```

### EXP register16 immediate16

- **`0x3101`: EXP register16, immediate16**
  - **Description:** Raises the value in register16 to the power of the immediate16 value, and stores the result in register16.
  - **Example:**

``` ACL
EXP AX, 0x03  ; raises AX to the power of 3
```

### EXP register32 immediate32

- **`0x3102`: EXP register32, immediate32**
  - **Description:** Raises the value in register32 to the power of the immediate32 value, and stores the result in register32.
  - **Example:**

``` ACL
EXP HL, 0x04  ; raises HL to the power of 4
```

## 0x32 SQRT destination

### SQRT register8

- **`0x3200`: SQRT register8**
  - **Description:** Calculates the square root of the value in register8, and stores the result in register8.
  - **Example:**

``` ACL
SQRT AL  ; calculates the square root of AL
```

### SQRT register16

- **`0x3201`: SQRT register16**
  - **Description:** Calculates the square root of the value in register16, and stores the result in register16.
  - **Example:**

``` ACL
SQRT AX  ; calculates the square root of AX
```

### SQRT register32

- **`0x3202`: SQRT register32**
  - **Description:** Calculates the square root of the value in register32, and stores the result in register32.
  - **Example:**

``` ACL
SQRT HL  ; calculates the square root of HL
```

## 0x33 MOD destination source

### MOD register8 immediate8

- **`0x3300`: MOD register8, immediate8**
  - **Description:** WIP (Work in Progress)
  - **Example:**

``` ACL
MOD AL, 0x0F  ; performs MOD operation between AL and 0x0F
```

### MOD register16 immediate16

- **`0x3301`: MOD register16, immediate16**
  - **Description:** WIP (Work in Progress)
  - **Example:**

``` ACL
MOD AX, 0x10  ; performs MOD operation between AX and 0x10
```

### MOD register32 immediate32

- **`0x3302`: MOD register32, immediate32**
  - **Description:** WIP (Work in Progress)
  - **Example:**

``` ACL
MOD HL, 0x0A  ; performs MOD operation between HL and 0x0A
```

## 0x34 SEB source operand1

### SEB register8 immediate8

- **`0x3400`: SEB register8, immediate8**
  - **Description:** Sets a bit in the register8 specified by the immediate8.
  - **Example:**

``` ACL
SEB AL, 0x04  ; sets the 4th bit in the AL register
```

### SEB register16 immediate16

- **`0x3401`: SEB register16, immediate16**
  - **Description:** Sets a bit in the register16 specified by the immediate16.
  - **Example:**

``` ACL
SEB AL, 0x04  ; sets the 4th bit in the AL register
```

### SEB register32 immediate32

- **`0x3402`: SEB register32, immediate32**
  - **Description:** Sets a bit in the register32 specified by the immediate32.
  - **Example:**

``` ACL
SEB AL, 0x04  ; sets the 4th bit in the AL register
```

## 0x35 CLB source operand1

### CLB register8 immediate8

- **`0x3500`: CLB register8, immediate8**
  - **Description:** Clears a bit in the register8 specified by the immediate8.
  - **Example:**

``` ACL
CLB BL, 0x03  ; sets the 3th bit in the BL register
```

### CLB register16 immediate16

- **`0x3501`: CLB register16, immediate16**
  - **Description:** Clears a bit in the register16 specified by the immediate16.
  - **Example:**

``` ACL
CLB BX, 0x03  ; sets the 3th bit in the BX register
```

### CLB register32 immediate32

- **`0x3501`: CLB register32, immediate32**
  - **Description:** Clears a bit in the register32 specified by the immediate32.
  - **Example:**

``` ACL
CLB HL, 0x03  ; sets the 3th bit in the HL register
```

## 0x36 TOB source operand1

### TOB register8 immediate8

- **`0x3600`: TOB register8, immediate8**
  - **Description:** Toggles a bit in the register8 specified by the immediate8.
  - **Example:**

``` ACL
TOB BL, 0x05  ; toggles the 5th bit in the BL register
```

### TOB register16 immediate16

- **`0x3600`: TOB register16, immediate16**
  - **Description:** Toggles a bit in the register16 specified by the immediate16.
  - **Example:**

``` ACL
TOB BX, 0x05  ; toggles the 5th bit in the BX register
```

### TOB register32 immediate32

- **`0x3601`: TOB register32, immediate32**
  - **Description:** Toggles a bit in the register32 specified by the immediate32.
  - **Example:**

``` ACL
TOB HL, 0x05  ; toggles the 5th bit in the HL register
```

# Float arithmetic operations

## 0x37 MOVF destination source

### MOVF float_register float_immediate

- **`0x3700`: MOVF float_register, float_immediate**
  - **Description:** Moves a float into the float_register from the float_immediate.
  - **Example:**

``` ACL
MOVF FA, 3.14f        ; Moves 3.14 into FA
```

### MOVF float_register float_immediate

- **`0x37F0`: MOVF FA, float_immediate**
  - **Description:** Moves a float into the FA register from the float_immediate.
  - **Example:**

``` ACL
MOVF FA, 3.14f        ; Moves 3.14 into FA
; this instruction will save you a byte
```

### MOVF float_register float_immediate

- **`0x37F0`: MOVF FB, float_immediate**
  - **Description:** Moves a float into the FB register from the float_immediate.
  - **Example:**

``` ACL
MOVF FB, 3.14f        ; Moves 3.14 into FB
; this instruction will save you a byte
```

## 0x38 FADD float_register float_source

### ADDF float_register float_immediate

- **`0x3800`: ADD float_register, float_immediate**
  - **Description:** Adds the float_immediate value to the value in the float_register and stores the result in the float_register.
  - **Example:**

``` ACL
MOV float FA, 300,05f   ; Moves 300,05 into FA
ADDF FA, 1,95f          ; adds 1,95f to the value in FA
; now FA is 302,00f
```

### ADDF float_register float_register

- **`0x3801`: ADD float_register, float_register**
  - **Description:** Adds the last float_register value to the value in the first float_register and stores the result in the first float_register.
  - **Example:**

``` ACL
MOV float FA, 300,05f   ; Moves 300,05 into FA
MOV float FB, 1,95f     ; Moves 1,95 into FB
ADDF FA, FB             ; adds 1,95f to the value in FA
; now FA is 302,00f
```

- `0x50`: FADD      destination source      `Adds the values of the source and the destination and stores the value in destination.`
- `0x51`: FSUB      destination source      `Subtracts the values of the source and the destination and stores the value in destination.`
- `0x52`: FMUL      destination source      `Multiplies the values of the source and the destination and stores the value in destination.`
- `0x53`: FDIV      destination source      `Divides the values of the source and the destination and stores the value in destination.`
- `0x54`: FAND      destination source      `Performs a bitwise AND operation between the destination and source and stores the value in destination.`
- `0x55`: FOR       destination source      `Performs a bitwise OR operation between the destination and source and stores the value in destination.`
- `0x56`: FNOR      destination source      `Performs a bitwise NOR operation between the destination and source and stores the value in destination.`
- `0x57`: FXOR      destination source      `Performs a bitwise XOR operation between the destination and source and stores the value in destination.`
- `0x58`: FNOT      destination             `Performs a bitwise NOT operation on the destination and stores the value in destination.`

# Memory Operations

## 0x90 MBL source destination

### MBL [address] [address]

- **`0x9000`: MBL [address1], [address2]**
  - **Description:** Moves 256 bytes from [address1] to [address2].
  - **Example:**

111100000011000000011000_00000000101000001011000011101100

``` ACL
MBL   [0x5000], [0xF000]  ; moving 256 bytes from [0x5000] to [0xF000] in memory
```

# Time and Date instructions

## 0xA0 DATE destination

### DATE [address]

- **`0xA000`: DATE [address]**
  - **Description:** Loads the date starting from the [address] + 3 bytes [more here](./SPECS_BCG-8-CPU.md#date-and-time)
  - **Example:**

``` ACL
DATE  [0x8000]            ; loading the date at 0x8000
mov   AL,       [0x8000]  ; loading the day into AL
```

## 0xA1 TIME destination

### TIME [address]

- **`0xA100`: TIME [address]**
  - **Description:** Loads the time starting from the [address] + 4 bytes [more here](./SPECS_BCG-8-CPU.md#date-and-time)
  - **Example:**

``` ACL
TIME  [0x8000]            ; loading the time at 0x8000
mov   AL,       [0x8001]  ; loading the hour into AL
```

## 0xA1 SELMS source

### SELMS imm8

- **`0xA200`: SELMS imm8**
  - **Description:** the CPU sleeps for source in MS
  - **Example:**

``` ACL
SELMS 0xFF  ; the CPU sleeps for 255 ms
```

### SELMS imm16

- **`0xA201`: SELMS imm16**
  - **Description:** the CPU sleeps for source in MS
  - **Example:**

``` ACL
SELMS 0xFFFF  ; the CPU sleeps for 65535 ms
```

### SELMS imm16

- **`0xA202`: SELMS imm32**
  - **Description:** the CPU sleeps for source in MS
  - **Example:**

``` ACL
SELMS 0xFFFFFFFF  ; the CPU sleeps for 4294967295 ms (7 weeks)
```

# Memory Operations instructions

- `0x93`: MOVS      destination address     `moves a null terminated string from the specified address to the destination`
- `0x94`: MOVF      destination immediate   `moves a float from the specified immediate to the float register(destination)`
- `0x95`: CMPSTR    address1 address2       `Compares to null terminated strings specified by the addresses and outputs the result in the {equal} flag`

# Converts instructions (WIP)

- `0xA0`: CTA       immediate address       `Converts the immediate into a acsii string with a length of 1 byte and loads the value into memory specified by the address`
- `0xA1`: CTH       immediate address       `Converts the immediate into a HEX string with a length of 4 bytes and loads the value into memory specified by the address`

# Special instructions

- `0xF6`: SSF                               `this instruction will setup a stack fream in one instruction`
  - NOTE: this is what the SSF instruction does
  - `push    BP`
  - `mov     BP,     SP`
  - `pushr`
- `0xF7`: SMBR      BANK                    `pushes the MB register and then sets the MB register to the specified BANK specified by the BANK operander`
- `0xF8`: RTI                               `returns from an intercept routine`
- `0xF9`: NOP                               `No operation`
- `0xFA`: RISERR    error_source            `Raises the error flag and sets the A register with the value from error_source`
- `0xFB`: PUSHR                             `Pushes (AX BX CX DX H L) on to the stack`
- `0xFC`: POPR                              `Pops (AX BX CX DX H L) off the stack`
- `0xFD`: INT       INTERRUPT_ROUTINE       `Generates an interrupt routine (more in the INTERRUPTS)`
- `0xFE`: BRK       INDEX                   `Generates a software interrupt and the INDEX will be moved into the X register (more in the INTERRUPTS)`
- `0xFF`: HALT                              `Stops the CPU`
