﻿using System.Collections.Generic;

namespace BC16CPUEmulator
{
    public enum Instruction : ushort
    {
        none,

        MOV = 0x0000,
        MOVRAL = 0x0001,
        MOVRBL = 0x0002,
        MOVRCL = 0x0003,
        MOVRDL = 0x0004,
        MOVRALCR0 = 0x0005,
        MOVRCR0AL = 0x0006,
        MOVW = 0x0100,
        MOVWRA = 0x0101,
        MOVWRB = 0x0102,
        MOVWRC = 0x0103,
        MOVWRD = 0x0104,
        MOVT = 0x0208,
        MOVD = 0x0300,
        MOVDRAX = 0x0301,
        MOVDRBX = 0x0302,
        MOVDRCX = 0x0303,
        MOVDRDX = 0x0304,
        CMP = 0x0400,
        CMPZ = 0x0401,
        CMPRA = 0x0402,
        CMPRAX = 0x0403,
        PUSH = 0x0500,
        POP = 0x0600,
        CALL = 0x0700,
        CALLRHL = 0x0701,
        RET = 0x0800,
        RETL = 0x0801,
        RETZ = 0x0900,
        SEZ = 0x0A00,
        SEZRAL = 0x0A01,
        SEZRBL = 0x0A02,
        SEZRCL = 0x0A03,
        SEZRDL = 0x0A04,
        SEZRA = 0x0A05,
        SEZRB = 0x0A06,
        SEZRC = 0x0A07,
        SEZRD = 0x0A08,
        SEZRAX = 0x0A09,
        SEZRBX = 0x0A0A,
        SEZRCX = 0x0A0B,
        SEZRDX = 0x0A0C,
        TEST = 0x0B00,
        TESTRAL = 0x0B01,
        TESTRBL = 0x0B02,
        TESTRCL = 0x0B03,
        TESTRDL = 0x0B04,
        TESTRA = 0x0B05,
        TESTRB = 0x0B06,
        TESTRC = 0x0B07,
        TESTRD = 0x0B08,
        TESTRAX = 0x0B09,
        TESTRBX = 0x0B0A,
        TESTRCX = 0x0B0B,
        TESTRDX = 0x0B0C,
        SWAP = 0x0C00,
        OUTB = 0x1000,
        OUTW = 0x1004,
        INB = 0x1010,
        INW = 0x1014,
        SZE = 0x2000,
        SEE = 0x2001,
        SES = 0x2002,
        SEC = 0x2003,
        SEL = 0x2004,
        SEI = 0x2005,
        SEH = 0x2006,
        CZE = 0x2010,
        CLE = 0x2011,
        CLS = 0x2012,
        CLC = 0x2013,
        CLL = 0x2014,
        CLI = 0x2015,
        CLH = 0x2016,
        ADD = 0x2020,
        ADDRA = 0x2021,
        ADDRAX = 0x2022,
        ADC = 0x2024,
        ADCRA = 0x2025,
        ADCRAX = 0x2026,
        SUB = 0x2030,
        SUBRA = 0x2031,
        SUBRAX = 0x2032,
        SBB = 0x2034,
        SBBRA = 0x2035,
        SBBRAX = 0x2036,
        MUL = 0x2040,
        MULRA = 0x2041,
        MULRAX = 0x2042,
        DIV = 0x2050,
        DIVRA = 0x2051,
        DIVRAX = 0x2052,
        AND = 0x2060,
        ANDRA = 0x2061,
        ANDRAX = 0x2062,
        OR = 0x2070,
        ORRA = 0x2071,
        ORRAX = 0x2072,
        NOR = 0x2080,
        NORRA = 0x2081,
        NORRAX = 0x2082,
        XOR = 0x2090,
        XORRA = 0x2091,
        XORRAX = 0x2092,
        NOT = 0x20A0,
        NOTRA = 0x20A1,
        NOTRAX = 0x20A2,
        SHL = 0x20B0,
        SHR = 0x20C0,
        ROL = 0x20D0,
        ROR = 0x20E0,
        INC = 0x20F0,
        DEC = 0x2100,
        NEG = 0x2110,
        EXP = 0x2120,
        SQRT = 0x2130,
        RNG = 0x2140,
        SEB = 0x2150,
        CLB = 0x2160,
        TOB = 0x2170,
        MOD = 0x2180,
        FADD = 0x2190,
        FSUB = 0x21A0,
        FMUL = 0x21B0,
        FDIV = 0x21C0,
        FAND = 0x21D0,
        FOR = 0x21E0,
        FNOR = 0x21F0,
        FXOR = 0x2200,
        FNOT = 0x2210,
        JMP = 0x3000,
        JZ = 0x3010,
        JNZ = 0x3011,
        JS = 0x3020,
        JNS = 0x3021,
        JE = 0x3030,
        JNE = 0x3031,
        JC = 0x3040,
        JNC = 0x3041,
        JL = 0x3090,
        JG = 0x3091,
        JLE = 0x3092,
        JGE = 0x3093,
        JNV = 0x30A1,
        CBTA = 0x4000,
        CMPL = 0x4010,
        MOVF = 0x4020,
        RETI = 0xF000,
        NOP = 0xF010,
        PUSHR = 0xF020,
        POPR = 0xF030,
        INT = 0xF040,
        BRK = 0xF050,
        ENTER = 0xF060,
        LEAVE = 0xF070,
        CPUID = 0xF080,
        HALT = 0xFFF0,
    }
    public class InstructionArguments
    {
        public InstructionArguments()
        {
            m_Instructions.Add(Instruction.MOV,                 new InstructionInfo(2));
            m_Instructions.Add(Instruction.MOVRAL,              new InstructionInfo(1));
            m_Instructions.Add(Instruction.MOVRBL,              new InstructionInfo(1));
            m_Instructions.Add(Instruction.MOVRCL,              new InstructionInfo(1));
            m_Instructions.Add(Instruction.MOVRDL,              new InstructionInfo(1));
            m_Instructions.Add(Instruction.MOVRALCR0,           new InstructionInfo(0));
            m_Instructions.Add(Instruction.MOVRCR0AL,           new InstructionInfo(0));
            m_Instructions.Add(Instruction.MOVW,                new InstructionInfo(2));
            m_Instructions.Add(Instruction.MOVWRA,              new InstructionInfo(1));
            m_Instructions.Add(Instruction.MOVWRB,              new InstructionInfo(1));
            m_Instructions.Add(Instruction.MOVWRC,              new InstructionInfo(1));
            m_Instructions.Add(Instruction.MOVWRD,              new InstructionInfo(1));
            m_Instructions.Add(Instruction.MOVT,                new InstructionInfo(2));
            m_Instructions.Add(Instruction.MOVD,                new InstructionInfo(2));
            m_Instructions.Add(Instruction.MOVDRAX,             new InstructionInfo(1));
            m_Instructions.Add(Instruction.MOVDRBX,             new InstructionInfo(1));
            m_Instructions.Add(Instruction.MOVDRCX,             new InstructionInfo(1));
            m_Instructions.Add(Instruction.MOVDRDX,             new InstructionInfo(1));
            m_Instructions.Add(Instruction.CMP,                 new InstructionInfo(2));
            m_Instructions.Add(Instruction.CMPZ,                new InstructionInfo(1));
            m_Instructions.Add(Instruction.CMPRA,               new InstructionInfo(1));
            m_Instructions.Add(Instruction.CMPRAX,              new InstructionInfo(1));
            m_Instructions.Add(Instruction.PUSH,                new InstructionInfo(1));
            m_Instructions.Add(Instruction.POP,                 new InstructionInfo(1));
            m_Instructions.Add(Instruction.CALL,                new InstructionInfo(1));
            m_Instructions.Add(Instruction.CALLRHL,             new InstructionInfo(0));
            m_Instructions.Add(Instruction.RET,                 new InstructionInfo(1));
            m_Instructions.Add(Instruction.RETL,                new InstructionInfo(0));
            m_Instructions.Add(Instruction.RETZ,                new InstructionInfo(0));
            m_Instructions.Add(Instruction.SEZ,                 new InstructionInfo(1));
            m_Instructions.Add(Instruction.SEZRAL,              new InstructionInfo(0));
            m_Instructions.Add(Instruction.SEZRBL,              new InstructionInfo(0));
            m_Instructions.Add(Instruction.SEZRCL,              new InstructionInfo(0));
            m_Instructions.Add(Instruction.SEZRDL,              new InstructionInfo(0));
            m_Instructions.Add(Instruction.SEZRA,               new InstructionInfo(0));
            m_Instructions.Add(Instruction.SEZRB,               new InstructionInfo(0));
            m_Instructions.Add(Instruction.SEZRC,               new InstructionInfo(0));
            m_Instructions.Add(Instruction.SEZRD,               new InstructionInfo(0));
            m_Instructions.Add(Instruction.SEZRAX,              new InstructionInfo(0));
            m_Instructions.Add(Instruction.SEZRBX,              new InstructionInfo(0));
            m_Instructions.Add(Instruction.SEZRCX,              new InstructionInfo(0));
            m_Instructions.Add(Instruction.SEZRDX,              new InstructionInfo(0));
            m_Instructions.Add(Instruction.TEST,                new InstructionInfo(1));
            m_Instructions.Add(Instruction.TESTRAL,             new InstructionInfo(0));
            m_Instructions.Add(Instruction.TESTRBL,             new InstructionInfo(0));
            m_Instructions.Add(Instruction.TESTRCL,             new InstructionInfo(0));
            m_Instructions.Add(Instruction.TESTRDL,             new InstructionInfo(0));
            m_Instructions.Add(Instruction.TESTRA,              new InstructionInfo(0));
            m_Instructions.Add(Instruction.TESTRB,              new InstructionInfo(0));
            m_Instructions.Add(Instruction.TESTRC,              new InstructionInfo(0));
            m_Instructions.Add(Instruction.TESTRD,              new InstructionInfo(0));
            m_Instructions.Add(Instruction.TESTRAX,             new InstructionInfo(0));
            m_Instructions.Add(Instruction.TESTRBX,             new InstructionInfo(0));
            m_Instructions.Add(Instruction.TESTRCX,             new InstructionInfo(0));
            m_Instructions.Add(Instruction.TESTRDX,             new InstructionInfo(0));
            m_Instructions.Add(Instruction.SWAP,                new InstructionInfo(2));
            m_Instructions.Add(Instruction.OUTB,                new InstructionInfo(2));
            m_Instructions.Add(Instruction.OUTW,                new InstructionInfo(2));
            m_Instructions.Add(Instruction.INB,                 new InstructionInfo(2));
            m_Instructions.Add(Instruction.INW,                 new InstructionInfo(2));
            m_Instructions.Add(Instruction.SZE,                 new InstructionInfo(0));
            m_Instructions.Add(Instruction.SEE,                 new InstructionInfo(0));
            m_Instructions.Add(Instruction.SES,                 new InstructionInfo(0));
            m_Instructions.Add(Instruction.SEC,                 new InstructionInfo(0));
            m_Instructions.Add(Instruction.SEL,                 new InstructionInfo(0));
            m_Instructions.Add(Instruction.SEI,                 new InstructionInfo(0));
            m_Instructions.Add(Instruction.SEH,                 new InstructionInfo(0));
            m_Instructions.Add(Instruction.CZE,                 new InstructionInfo(0));
            m_Instructions.Add(Instruction.CLE,                 new InstructionInfo(0));
            m_Instructions.Add(Instruction.CLS,                 new InstructionInfo(0));
            m_Instructions.Add(Instruction.CLC,                 new InstructionInfo(0));
            m_Instructions.Add(Instruction.CLL,                 new InstructionInfo(0));
            m_Instructions.Add(Instruction.CLI,                 new InstructionInfo(0));
            m_Instructions.Add(Instruction.CLH,                 new InstructionInfo(0));
            m_Instructions.Add(Instruction.ADD,                 new InstructionInfo(2));
            m_Instructions.Add(Instruction.ADDRA,               new InstructionInfo(1));
            m_Instructions.Add(Instruction.ADDRAX,              new InstructionInfo(1));
            m_Instructions.Add(Instruction.ADC,                 new InstructionInfo(2));
            m_Instructions.Add(Instruction.ADCRA,               new InstructionInfo(1));
            m_Instructions.Add(Instruction.ADCRAX,              new InstructionInfo(1));
            m_Instructions.Add(Instruction.SUB,                 new InstructionInfo(2));
            m_Instructions.Add(Instruction.SUBRA,               new InstructionInfo(1));
            m_Instructions.Add(Instruction.SUBRAX,              new InstructionInfo(1));
            m_Instructions.Add(Instruction.SBB,                 new InstructionInfo(2));
            m_Instructions.Add(Instruction.SBBRA,               new InstructionInfo(1));
            m_Instructions.Add(Instruction.SBBRAX,              new InstructionInfo(1));
            m_Instructions.Add(Instruction.MUL,                 new InstructionInfo(2));
            m_Instructions.Add(Instruction.MULRA,               new InstructionInfo(1));
            m_Instructions.Add(Instruction.MULRAX,              new InstructionInfo(1));
            m_Instructions.Add(Instruction.DIV,                 new InstructionInfo(2));
            m_Instructions.Add(Instruction.DIVRA,               new InstructionInfo(1));
            m_Instructions.Add(Instruction.DIVRAX,              new InstructionInfo(1));
            m_Instructions.Add(Instruction.AND,                 new InstructionInfo(2));
            m_Instructions.Add(Instruction.ANDRA,               new InstructionInfo(1));
            m_Instructions.Add(Instruction.ANDRAX,              new InstructionInfo(1));
            m_Instructions.Add(Instruction.OR,                  new InstructionInfo(2));
            m_Instructions.Add(Instruction.ORRA,                new InstructionInfo(1));
            m_Instructions.Add(Instruction.ORRAX,               new InstructionInfo(1));
            m_Instructions.Add(Instruction.NOR,                 new InstructionInfo(2));
            m_Instructions.Add(Instruction.NORRA,               new InstructionInfo(1));
            m_Instructions.Add(Instruction.NORRAX,              new InstructionInfo(1));
            m_Instructions.Add(Instruction.XOR,                 new InstructionInfo(2));
            m_Instructions.Add(Instruction.XORRA,               new InstructionInfo(1));
            m_Instructions.Add(Instruction.XORRAX,              new InstructionInfo(1));
            m_Instructions.Add(Instruction.NOT,                 new InstructionInfo(1));
            m_Instructions.Add(Instruction.NOTRA,               new InstructionInfo(1));
            m_Instructions.Add(Instruction.NOTRAX,              new InstructionInfo(1));
            m_Instructions.Add(Instruction.SHL,                 new InstructionInfo(2));
            m_Instructions.Add(Instruction.SHR,                 new InstructionInfo(2));
            m_Instructions.Add(Instruction.ROL,                 new InstructionInfo(2));
            m_Instructions.Add(Instruction.ROR,                 new InstructionInfo(2));
            m_Instructions.Add(Instruction.INC,                 new InstructionInfo(1));
            m_Instructions.Add(Instruction.DEC,                 new InstructionInfo(1));
            m_Instructions.Add(Instruction.NEG,                 new InstructionInfo(1));
            m_Instructions.Add(Instruction.EXP,                 new InstructionInfo(2));
            m_Instructions.Add(Instruction.SQRT,                new InstructionInfo(1));
            m_Instructions.Add(Instruction.RNG,                 new InstructionInfo(1));
            m_Instructions.Add(Instruction.SEB,                 new InstructionInfo(2));
            m_Instructions.Add(Instruction.CLB,                 new InstructionInfo(2));
            m_Instructions.Add(Instruction.TOB,                 new InstructionInfo(2));
            m_Instructions.Add(Instruction.MOD,                 new InstructionInfo(2));
            m_Instructions.Add(Instruction.FADD,                new InstructionInfo(2));
            m_Instructions.Add(Instruction.FSUB,                new InstructionInfo(2));
            m_Instructions.Add(Instruction.FMUL,                new InstructionInfo(2));
            m_Instructions.Add(Instruction.FDIV,                new InstructionInfo(2));
            m_Instructions.Add(Instruction.FAND,                new InstructionInfo(2));
            m_Instructions.Add(Instruction.FOR,                 new InstructionInfo(2));
            m_Instructions.Add(Instruction.FNOR,                new InstructionInfo(2));
            m_Instructions.Add(Instruction.FXOR,                new InstructionInfo(2));
            m_Instructions.Add(Instruction.FNOT,                new InstructionInfo(2));
            m_Instructions.Add(Instruction.JMP,                 new InstructionInfo(1));
            m_Instructions.Add(Instruction.JZ,                  new InstructionInfo(1));
            m_Instructions.Add(Instruction.JNZ,                 new InstructionInfo(1));
            m_Instructions.Add(Instruction.JS,                  new InstructionInfo(1));
            m_Instructions.Add(Instruction.JNS,                 new InstructionInfo(1));
            m_Instructions.Add(Instruction.JE,                  new InstructionInfo(1));
            m_Instructions.Add(Instruction.JNE,                 new InstructionInfo(1));
            m_Instructions.Add(Instruction.JC,                  new InstructionInfo(1));
            m_Instructions.Add(Instruction.JNC,                 new InstructionInfo(1));
            m_Instructions.Add(Instruction.JL,                  new InstructionInfo(1));
            m_Instructions.Add(Instruction.JG,                  new InstructionInfo(1));
            m_Instructions.Add(Instruction.JLE,                 new InstructionInfo(1));
            m_Instructions.Add(Instruction.JGE,                 new InstructionInfo(1));
            m_Instructions.Add(Instruction.JNV,                 new InstructionInfo(1));
            m_Instructions.Add(Instruction.CBTA,                new InstructionInfo(2));
            m_Instructions.Add(Instruction.CMPL,                new InstructionInfo(0));
            m_Instructions.Add(Instruction.MOVF,                new InstructionInfo(2));
            m_Instructions.Add(Instruction.RETI,                new InstructionInfo(0));
            m_Instructions.Add(Instruction.NOP,                 new InstructionInfo(0));
            m_Instructions.Add(Instruction.PUSHR,               new InstructionInfo(0));
            m_Instructions.Add(Instruction.POPR,                new InstructionInfo(0));
            m_Instructions.Add(Instruction.INT,                 new InstructionInfo(1));
            m_Instructions.Add(Instruction.BRK,                 new InstructionInfo(0));
            m_Instructions.Add(Instruction.ENTER,               new InstructionInfo(0));
            m_Instructions.Add(Instruction.LEAVE,               new InstructionInfo(0));
            m_Instructions.Add(Instruction.CPUID,               new InstructionInfo(1));
            m_Instructions.Add(Instruction.HALT,                new InstructionInfo(0));
        }

        public static Dictionary<Instruction, InstructionInfo> m_Instructions = new Dictionary<Instruction, InstructionInfo>();
    }
    public struct InstructionInfo
    {
        public int m_OperandSize;

        public InstructionInfo(int operandSize)
        {
            m_OperandSize = operandSize;
        }
    }
}