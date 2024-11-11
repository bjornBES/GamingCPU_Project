
- 0x00: immediate byte:             number
- 0x01: immediate word:             number
- 0x02: immediate tbyte:            number
- 0x03: immediate dword:            number
- 0x04: immediate qword:            number
- 0x05: immediate dqword:           number
- 0x08: immediate_float:            numberf
- 0x09: immediate_double:           double numberf
- 0x10: register:                   register
- 0x11: register AL:                AL
- 0x12: register BL:                BL
- 0x13: register CL:                CL
- 0x14: register DL:                DL
- 0x15: register H:                 H
- 0x16: register L:                 L
- 0x17: register A                  A
- 0x18: register B                  B
- 0x19: register C                  C
- 0x1A: register D                  D
- 0x1B: register AX:                AX
- 0x1C: register BX:                BX
- 0x1D: register CX:                CX
- 0x1E: register DX:                DX
- 0x1F: register EX:                EX
- 0x20: register GX:                GX
- 0x2E: register address:           address [register]
- 0x2F: address register HL:        [HL]
- 0x30: Relative address:           [byte address]      an 8 bit offset to the PC
- 0x31: Near address:               Near [address]      a 8 bit address
- 0x32: Short address:              short [address]     a 16 bit address
- 0x33: long address:               long [address]      a 24 bit address
- 0x34: Far address:                far [address]       a 32 bit address
- 0x36: Short X indexed address:    [short address],X
- 0x37: Short Y indexed address:    [short address],Y
- 0x39: 32 bit segment address:     [register:register]
- 0x3A: 32 bit segment DS register: [DS:register]
- 0x3B: 32 bit segment DS B:        [DS:B]
- 0x3C: 32 bit segment ES register: [ES:register]
- 0x3D: 32 bit segment ES B:        [ES:B]
- 0x3E: 32 bit segment FS register: [FS:register]
- 0x3F: 32 bit segment FS B:        [FS:B]
- 0x40: 32 bit segment GS register: [GS:register]
- 0x41: 32 bit segment GS B:        [GS:B]
- 0x42: 32 bit segment HS register: [HS:register]
- 0x43: 32 bit segment HS B:        [HS:B]
- 0x50: SP relative address byte:   [SP + sbyte number]
- 0x51: BP relative address byte:   [BP + sbyte number]
- 0x52: SP relative address short:  [SP + short number]
- 0x53: BP relative address short:  [BP + short number]
- 0x54: SPX relative address word:  [SPX + word number]
- 0x55: BPX relative address word:  [BPX + word number]
- 0x56: SPX relative address tbyte: [SPX + tbyte number]
- 0x57: BPX relative address tbyte: [BPX + tbyte number]
- 0x58: SPX relative address int:   [SPX + int number]
- 0x59: BPX relative address int:   [BPX + int number]
- 0x5A: SPX relative address word:  [SPX + word number]
- 0x5B: BPX relative address word:  [BPX + word number]
- 0x5C: BPX relative address int:   [BPX + int number]
- 0x5D: BPX relative address int:   [BPX + int number]
- 0x60: register AF:                AF
- 0x61: register BF:                BF
- 0x62: register CF:                CF
- 0x63: register DF:                DF
- 0x70: register AD:                AD
- 0x71: register BD:                BD
- 0x72: register CD:                CD
- 0x73: register DD:                DD