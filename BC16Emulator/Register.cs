﻿using System;
using System.Collections.Generic;

namespace BC16CPUEmulator
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
        GS = 0b11_000101,
        HS = 0b00_000110,
        SS = 0b11_000110,

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

        R1L = 0b00_010100,
        R2L = 0b01_010100,
        R3L = 0b10_010100,
        R4L = 0b11_010100,
        R5L = 0b00_010101,
        R6L = 0b01_010101,
        R7L = 0b10_010101,
        R8L = 0b11_010101,
        R9L = 0b00_010110,
        R10L = 0b01_010110,
        R11L = 0b10_010110,
        R12L = 0b11_010110,
        R13L = 0b00_010111,
        R14L = 0b01_010111,
        R15L = 0b10_010111,
        R16L = 0b11_010111,

        R1H = 0b00_011000,
        R2H = 0b01_011000,
        R3H = 0b10_011000,
        R4H = 0b11_011000,
        R5H = 0b00_011001,
        R6H = 0b01_011001,
        R7H = 0b10_011001,
        R8H = 0b11_011001,
        R9H = 0b00_011010,
        R10H = 0b01_011010,
        R11H = 0b10_011010,
        R12H = 0b11_011010,
        R13H = 0b00_011011,
        R14H = 0b01_011011,
        R15H = 0b10_011011,
        R16H = 0b11_011011,

        CR0 = 0b00_100100,

        PTA = 0b00_100110,

        GDA = 0b00_100111,

        ECS = 0b00_101000,
        EDS = 0b00_101001,
        EES = 0b01_101001,
        EFS = 0b10_101001,
        EGS = 0b11_101001,
        EHS = 0b00_101010,
        ESS = 0b11_101010,


        F = 0b00_111111,
        none = 0xFF
    }
}