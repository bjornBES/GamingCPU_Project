﻿using System;
using System.Collections.Generic;

namespace BCG16CPUEmulator
{
    public enum Register
    {
        AL = 0b00_000000,
        AH = 0b01_000000,
        A = 0b10_000000,
        AX = 0b11_000000,

        BL = 0b00_000001,
        BH = 0b01_000001,
        B = 0b10_000001,
        BX = 0b11_000001,

        CL = 0b00_000010,
        CH = 0b01_000010,
        C = 0b10_000010,
        CX = 0b11_000010,

        DL = 0b00_000011,
        DH = 0b01_000011,
        D = 0b10_000011,
        DX = 0b11_000011,

        CS = 0b00_000100,
        DS = 0b00_000101,
        ES = 0b01_000101,
        FS = 0b10_000101,
        SS = 0b00_000110,

        PC = 0b00_000111,

        L = 0b01_000111,
        H = 0b10_000111,
        HL = 0b11_000111,

        BP = 0b00_001000,
        SP = 0b00_001001,

        AF = 0b00_001010,
        BF = 0b00_001011,
        CF = 0b00_001100,
        DF = 0b00_001101,

        R1 = 0b00_010000,
        R2 = 0b01_010000,
        R3 = 0b10_010000,
        R4 = 0b11_010000,
        R5 = 0b00_010001,
        R6 = 0b01_010001,
        R7 = 0b10_010001,
        R8 = 0b11_010001,
        R9 = 0b00_010010,
        R10 = 0b01_010010,
        R11 = 0b10_010010,
        R12 = 0b11_010010,
        R13 = 0b00_010011,
        R14 = 0b01_010011,
        R15 = 0b10_010011,
        R16 = 0b11_010011,

        X = 0b00_011000,    
        Y = 0b01_011000,

        CR0 = 0b00_100001,
        CR1 = 0b00_100010,

        F = 0b00_111111,
        none = 0xFF
    }
}