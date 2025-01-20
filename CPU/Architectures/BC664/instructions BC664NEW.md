# General instructions

- `0x0000`: MOV:  destination, source
- `0x0100`: MOVW: destination, source
- `0x0200`: MOVT: destination, source
- `0x0300`: MOVD: destination, source
- `0x0400`: CMP:  source1 source2
- `0x0401`: CMPZ: source
- `0x0500`: PUSH: source
- `0x0600`: POP:  register
- `0x0604`: POPW: register
- `0x0608`: POPT: register
- `0x060C`: POPD: register
- `0x0700`: CALL: address
- `0x0800`: RET:  source
- `0x0900`: RETZ:
- `0x0A00`: SEZ:  register
- `0x0B00`: TEST: register
- `0x0C00`: SWAP register register

# IO instructions

- `0x1000`: OUT:  source source
- `0x1004`: OUTW: source source
- `0x1010`: INP:  source destination
- `0x1014`: INPW: source destination

# Arithmetic and logic operations

- `0x2000`: SZE:
- `0x2001`: SEE:
- `0x2002`: SES:
- `0x2003`: SEC:
- `0x2004`: SEL:
- `0x2005`: SEI:
- `0x2006`: SEH:
- `0x2010`: CZE:
- `0x2011`: CLE:
- `0x2012`: CLS:
- `0x2013`: CLC:
- `0x2014`: CLL:
- `0x2015`: CLI:
- `0x2016`: CLH:
- `0x2020`: ADD:    destination, source
- `0x2021`: ADC:    destination, source
- `0x2030`: SUB:    destination, source
- `0x2031`: SBB:    destination, source
- `0x2040`: MUL:    destination, source
- `0x2050`: DIV:    destination, source
- `0x2060`: AND:    destination, source
- `0x2070`: OR:     destination, source
- `0x2080`: NOR:    destination, source
- `0x2090`: XOR:    destination, source
- `0x20A0`: NOT:    destination
- `0x20B0`: SHL:    destination, source
- `0x20C0`: SHR:    destination, source
- `0x20D0`: ROL:    destination, source
- `0x20E0`: ROR:    destination, source
- `0x20F0`: INC:    register
- `0x2100`: DEC:    register
- `0x2110`: NEG:    destination
- `0x2120`: EXP:    destination, source
- `0x2130`: SQRT:   destination
- `0x2140`: RNG:    destination
- `0x2150`: SEB:    source, destination
- `0x2160`: CLB:    source, destination
- `0x2170`: TOB:    source, destination
- `0x2180`: MOD:    destination, source

# Single-Precision Float Arithmetic Operations

- `0x2190`: ADDF:   destination, source
- `0x21A0`: SUBF:   destination, source
- `0x21B0`: MULF:   destination, source
- `0x21C0`: DIVF:   destination, source
- `0x21D0`: CMPF:   destination, source
- `0x21E0`: SQRTF:  destination
- `0x21F0`: MODF:   destination, source

# Double-Precision Float Arithmetic Operations

- `0x2290`: ADDD:   destination, source
- `0x22A0`: SUBD:   destination, source
- `0x22B0`: MULD:   destination, source
- `0x22C0`: DIVD:   destination, source
- `0x22D0`: CMPD:   destination, source
- `0x22E0`: SQRTD:  destination
- `0x22F0`: MODD:   destination, source

# Conditional jumps

- `0x3000`: JMP:    address
- `0x3010`: JZ:     address
- `0x3011`: JNZ:    address
- `0x3020`: JS:     address
- `0x3021`: JNS:    address
- `0x3030`: JE:     address
- `0x3031`: JNE:    address
- `0x3040`: JL:     address
- `0x3041`: JG:     address
- `0x3042`: JLE:    address
- `0x3043`: JGE:    address
- `0x3051`: JNV:    address

# Memory Operations instructions

- `0x4010`: CMPL:
- `0x4020`: LODR;
- `0x4030`: MOVF: destination, immediate
- `0x4040`: MOFD: destination, immediate
- `0x4050`: MOVQ: destination, source

# Special instructions

- `0xF000`: RETI:
- `0xF010`: NOP:
- `0xF020`: PUSHR:
- `0xF030`: POPR:
- `0xF040`: INT:    source
- `0xF050`: BRK:
- `0xF060`: ENTER:
- `0xF070`: LEAVE:
- `0xF080`: CPUID:  register
- `0xF090`: PUSHRR:
- `0xF0A0`: POPRR:
- `0xF0B0`: LGDT:   address
- `0xFFF0`: HALT:
