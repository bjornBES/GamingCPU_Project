using AssemblerBCG;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public enum OLdInstruction : ushort
{
    MOV =       0x0000,
    MOVRAL =    0x0001,
    MOVRBL =    0x0002,
    MOVRCL =    0x0003,
    MOVRDL =    0x0004,
    MOVRALCR0 = 0x0005,
    MOVRCR0AL = 0x0006,
    MOVW =      0x0100,
    MOVWRA =    0x0101,
    MOVWRB =    0x0102,
    MOVWRC =    0x0103,
    MOVWRD =    0x0104,
    MOVWRACR0 = 0x0105,
    MOVWRCR0A = 0x0106,
    MOVT =      0x0208,
    MOVD =      0x0300,
    MOVDRAX =   0x0301,
    MOVDRBX =   0x0302,
    MOVDRCX =   0x0303,
    MOVDRDX =   0x0304,
    CMP =       0x0400,
    CMPZ =      0x0401,
    CMPRA =     0x0402,
    CMPRAX =    0x0403,
    PUSH =      0x0500,
    POP =       0x0600,
    CALL =      0x0700,
    CALLRHL =   0x0701,
    RET =       0x0800,
    RETL =      0x0801,
    RETZ =      0x0900,
    SEZ =       0x0A00,
    SEZRAL =    0x0A01,
    SEZRBL =    0x0A02,
    SEZRCL =    0x0A03,
    SEZRDL =    0x0A04,
    SEZRA =     0x0A05,
    SEZRB =     0x0A06,
    SEZRC =     0x0A07,
    SEZRD =     0x0A08,
    SEZRAX =    0x0A09,
    SEZRBX =    0x0A0A,
    SEZRCX =    0x0A0B,
    SEZRDX =    0x0A0C,
    TEST =      0x0B00,
    TESTRAL =   0x0B01,
    TESTRBL =   0x0B02,
    TESTRCL =   0x0B03,
    TESTRDL =   0x0B04,
    TESTRA =    0x0B05,
    TESTRB =    0x0B06,
    TESTRC =    0x0B07,
    TESTRD =    0x0B08,
    TESTRAX =   0x0B09,
    TESTRBX =   0x0B0A,
    TESTRCX =   0x0B0B,
    TESTRDX =   0x0B0C,
    SWAP =      0x0C00,

    OUTB =      0x1000,
    OUTW =      0x1004,
    INP =       0x1010,
    INB =       0x1010,
    INPW =      0x1014,
    
    SZE =       0x2000,
    SEE =       0x2001,
    SES =       0x2002,
    SEC =       0x2003,
    SEL =       0x2004,
    SEI =       0x2005,
    SEH =       0x2006,
    CZE =       0x2010,
    CLE =       0x2011,
    CLS =       0x2012,
    CLC =       0x2013,
    CLL =       0x2014,
    CLI =       0x2015,
    CLH =       0x2016,
    
    ADD =       0x2020,
    ADDRA =     0x2021,
    ADDRAX =    0x2022,
    ADC =       0x2024,
    ADCRA =     0x2025,
    ADCRAX =    0x2026,
    SUB =       0x2030,
    SUBRA =     0x2031,
    SUBRAX =    0x2032,
    SBB =       0x2034,
    SBBRA =     0x2035,
    SBBRAX =    0x2036,
    MUL =       0x2040,
    MULRA =     0x2041,
    MULRAX =    0x2042,
    DIV =       0x2050,
    DIVRA =     0x2051,
    DIVRAX =    0x2052,
    AND =       0x2060,
    ANDRA =     0x2061,
    ANDRAX =    0x2062,
    OR =        0x2070,
    ORRA =      0x2071,
    ORRAX =     0x2072,
    NOR =       0x2080,
    NORRA =     0x2081,
    NORRAX =    0x2082,
    XOR =       0x2090,
    XORRA =     0x2091,
    XORRAX =    0x2092,
    NOT =       0x20A0,
    NOTRA =     0x20A1,
    NOTRAX =    0x20A2,
    SHL =       0x20B0,
    SHR =       0x20C0,
    ROL =       0x20D0,
    ROR =       0x20E0,
    INC =       0x20F0,
    DEC =       0x2100,
    NEG =       0x2110,
    EXP =       0x2120,
    SQRT =      0x2130,
    RNG =       0x2140,
    SEB =       0x2150,
    CLB =       0x2160,
    TOB =       0x2170,
    MOD =       0x2180,
    
    FADD =      0x2190,
    FSUB =      0x21A0,
    FMUL =      0x21B0,
    FDIV =      0x21C0,
    FAND =      0x21D0,
    FOR =       0x21E0,
    FNOR =      0x21F0,
    FXOR =      0x2200,
    FNOT =      0x2210,
    
    JMP =       0x3000,
    JZ =        0x3010,
    JNZ =       0x3011,
    JS =        0x3020,
    JNS =       0x3021,
    JE =        0x3030,
    JNE =       0x3031,
    JC =        0x3040,
    JNC =       0x3041,
    JL =        0x3090,
    JG =        0x3091,
    JLE =       0x3092,
    JGE =       0x3093,
    JNV =       0x30A1,
    
    CBTA =      0x4000,

    CMPL =      0x4010,
    LOOR =      0x4020,
    MOVF =      0x4030,
    
    RETI =      0xF000,
    NOP =       0xF010,
    PUSHR =     0xF020,
    POPR =      0xF030,
    INT =       0xF040,
    BRK =       0xF050,
    ENTER =     0xF060,
    LEAVE =     0xF070,
    CPUID =     0xF080,
    LGDT =      0xF090,
    HALT =      0xFFF0,
    none,
}

public class OldInstructions
{
    public static Dictionary<OLdInstruction, OldInstructionInfo> m_Instr;
    public OldInstructions()
    {
        m_Instr = new Dictionary<OLdInstruction, OldInstructionInfo>()
        {
            { OLdInstruction.MOV,           new OldInstructionInfo(2,OLdInstruction.MOV, CPUType.BC8)},
            { OLdInstruction.MOVRAL,        new OldInstructionInfo(1,OLdInstruction.MOVRAL, CPUType.BC8)},
            { OLdInstruction.MOVRBL,        new OldInstructionInfo(1,OLdInstruction.MOVRBL, CPUType.BC8)},
            { OLdInstruction.MOVRCL,        new OldInstructionInfo(1,OLdInstruction.MOVRCL, CPUType.BC8)},
            { OLdInstruction.MOVRDL,        new OldInstructionInfo(1,OLdInstruction.MOVRDL, CPUType.BC8)},
            { OLdInstruction.MOVRALCR0,     new OldInstructionInfo(0,OLdInstruction.MOVRALCR0, CPUType.BC16)},
            { OLdInstruction.MOVRCR0AL,     new OldInstructionInfo(0,OLdInstruction.MOVRCR0AL, CPUType.BC16)},
            { OLdInstruction.MOVW,          new OldInstructionInfo(2,OLdInstruction.MOVW, CPUType.BC8)},
            { OLdInstruction.MOVWRA,        new OldInstructionInfo(1,OLdInstruction.MOVWRA, CPUType.BC8)},
            { OLdInstruction.MOVWRB,        new OldInstructionInfo(1,OLdInstruction.MOVWRB, CPUType.BC8)},
            { OLdInstruction.MOVWRC,        new OldInstructionInfo(1,OLdInstruction.MOVWRC, CPUType.BC8)},
            { OLdInstruction.MOVWRD,        new OldInstructionInfo(1,OLdInstruction.MOVWRD, CPUType.BC8)},
            { OLdInstruction.MOVWRACR0,     new OldInstructionInfo(0,OLdInstruction.MOVWRACR0, CPUType.BC32)},
            { OLdInstruction.MOVWRCR0A,     new OldInstructionInfo(0,OLdInstruction.MOVWRCR0A, CPUType.BC32)},
            { OLdInstruction.MOVT,          new OldInstructionInfo(2,OLdInstruction.MOVT, CPUType.BC16)},
            { OLdInstruction.MOVD,          new OldInstructionInfo(2,OLdInstruction.MOVD, CPUType.BC16)},
            { OLdInstruction.MOVDRAX,       new OldInstructionInfo(1,OLdInstruction.MOVDRAX, CPUType.BC16)},
            { OLdInstruction.MOVDRBX,       new OldInstructionInfo(1,OLdInstruction.MOVDRBX, CPUType.BC16)},
            { OLdInstruction.MOVDRCX,       new OldInstructionInfo(1,OLdInstruction.MOVDRCX, CPUType.BC16)},
            { OLdInstruction.MOVDRDX,       new OldInstructionInfo(1,OLdInstruction.MOVDRDX, CPUType.BC16)},
            { OLdInstruction.CMP,           new OldInstructionInfo(2,OLdInstruction.CMP, CPUType.BC16)},
            { OLdInstruction.CMPZ,          new OldInstructionInfo(2,OLdInstruction.CMPZ, CPUType.BC16)},
            { OLdInstruction.CMPRA,         new OldInstructionInfo(2,OLdInstruction.CMPRA, CPUType.BC16)},
            { OLdInstruction.CMPRAX,        new OldInstructionInfo(2,OLdInstruction.CMPRAX, CPUType.BC16)},
            { OLdInstruction.PUSH,          new OldInstructionInfo(1,OLdInstruction.PUSH, CPUType.BC16)},
            { OLdInstruction.POP,           new OldInstructionInfo(1,OLdInstruction.POP, CPUType.BC8)},
            { OLdInstruction.CALL,          new OldInstructionInfo(1,OLdInstruction.CALL, CPUType.BC8)},
            { OLdInstruction.CALLRHL,       new OldInstructionInfo(0,OLdInstruction.CALLRHL, CPUType.BC8)},
            { OLdInstruction.RET,           new OldInstructionInfo(1,OLdInstruction.RET, CPUType.BC8)},
            { OLdInstruction.RETL,          new OldInstructionInfo(0,OLdInstruction.RETL, CPUType.BC8)},
            { OLdInstruction.RETZ,          new OldInstructionInfo(0,OLdInstruction.RETZ, CPUType.BC8)},
            { OLdInstruction.SEZ,           new OldInstructionInfo(1,OLdInstruction.SEZ, CPUType.BC8)},
            { OLdInstruction.SEZRAL,        new OldInstructionInfo(0,OLdInstruction.SEZRAL, CPUType.BC8)},
            { OLdInstruction.SEZRA,         new OldInstructionInfo(0,OLdInstruction.SEZRA, CPUType.BC8)},
            { OLdInstruction.SEZRAX,        new OldInstructionInfo(0,OLdInstruction.SEZRAX, CPUType.BC16)},
            { OLdInstruction.TEST,          new OldInstructionInfo(1,OLdInstruction.TEST, CPUType.BC8)},
            { OLdInstruction.TESTRAL,       new OldInstructionInfo(0,OLdInstruction.TESTRAL, CPUType.BC8)},
            { OLdInstruction.TESTRA,        new OldInstructionInfo(0,OLdInstruction.TESTRA, CPUType.BC8)},
            { OLdInstruction.TESTRAX,       new OldInstructionInfo(0,OLdInstruction.TESTRAX, CPUType.BC16)},
            { OLdInstruction.SWAP,          new OldInstructionInfo(2,OLdInstruction.SWAP, CPUType.BC8)},

            { OLdInstruction.JMP,           new OldInstructionInfo(1,OLdInstruction.JMP, CPUType.BC8)},
            { OLdInstruction.JZ,            new OldInstructionInfo(1,OLdInstruction.JZ, CPUType.BC8)},
            { OLdInstruction.JNZ,           new OldInstructionInfo(1,OLdInstruction.JNZ, CPUType.BC8)},
            { OLdInstruction.JS,            new OldInstructionInfo(1,OLdInstruction.JS, CPUType.BC8)},
            { OLdInstruction.JNS,           new OldInstructionInfo(1,OLdInstruction.JNS, CPUType.BC8)},
            { OLdInstruction.JE,            new OldInstructionInfo(1,OLdInstruction.JE, CPUType.BC8)},
            { OLdInstruction.JNE,           new OldInstructionInfo(1,OLdInstruction.JNE, CPUType.BC8)},
            { OLdInstruction.JC,            new OldInstructionInfo(1,OLdInstruction.JC, CPUType.BC8)},
            { OLdInstruction.JNC,           new OldInstructionInfo(1,OLdInstruction.JNC, CPUType.BC8)},
            { OLdInstruction.JL,            new OldInstructionInfo(1,OLdInstruction.JL, CPUType.BC8)},
            { OLdInstruction.JG,            new OldInstructionInfo(1,OLdInstruction.JG, CPUType.BC8)},
            { OLdInstruction.JLE,           new OldInstructionInfo(1,OLdInstruction.JLE, CPUType.BC8)},
            { OLdInstruction.JGE,           new OldInstructionInfo(1,OLdInstruction.JGE, CPUType.BC8)},
            { OLdInstruction.JNV,           new OldInstructionInfo(1,OLdInstruction.JNV, CPUType.BC8)},

            { OLdInstruction.SZE,           new OldInstructionInfo(0,OLdInstruction.SZE, CPUType.BC8)},
            { OLdInstruction.SEE,           new OldInstructionInfo(0,OLdInstruction.SEE, CPUType.BC8)},
            { OLdInstruction.SES,           new OldInstructionInfo(0,OLdInstruction.SES, CPUType.BC8)},
            { OLdInstruction.SEC,           new OldInstructionInfo(0,OLdInstruction.SEC, CPUType.BC8)},
            { OLdInstruction.SEL,           new OldInstructionInfo(0,OLdInstruction.SEL, CPUType.BC8)},
            { OLdInstruction.SEI,           new OldInstructionInfo(0,OLdInstruction.SEI, CPUType.BC8)},
            { OLdInstruction.SEH,           new OldInstructionInfo(0,OLdInstruction.SEH, CPUType.BC8)},
            { OLdInstruction.CZE,           new OldInstructionInfo(0,OLdInstruction.CZE, CPUType.BC8)},
            { OLdInstruction.CLE,           new OldInstructionInfo(0,OLdInstruction.CLE, CPUType.BC8)},
            { OLdInstruction.CLS,           new OldInstructionInfo(0,OLdInstruction.CLS, CPUType.BC8)},
            { OLdInstruction.CLC,           new OldInstructionInfo(0,OLdInstruction.CLC, CPUType.BC8)},
            { OLdInstruction.CLL,           new OldInstructionInfo(0,OLdInstruction.CLL, CPUType.BC8)},
            { OLdInstruction.CLI,           new OldInstructionInfo(0,OLdInstruction.CLI, CPUType.BC8)},
            { OLdInstruction.CLH,           new OldInstructionInfo(0,OLdInstruction.CLH, CPUType.BC8)},
            { OLdInstruction.ADD,           new OldInstructionInfo(2,OLdInstruction.ADD, CPUType.BC8)},
            { OLdInstruction.ADDRA,         new OldInstructionInfo(2,OLdInstruction.ADDRA, CPUType.BC8)},
            { OLdInstruction.ADDRAX,        new OldInstructionInfo(2,OLdInstruction.ADDRAX, CPUType.BC8)},
            { OLdInstruction.ADC,           new OldInstructionInfo(2,OLdInstruction.ADC, CPUType.BC8)},
            { OLdInstruction.ADCRA,         new OldInstructionInfo(2,OLdInstruction.ADCRA, CPUType.BC8)},
            { OLdInstruction.ADCRAX,        new OldInstructionInfo(2,OLdInstruction.ADCRAX, CPUType.BC8)},
            { OLdInstruction.SUB,           new OldInstructionInfo(2,OLdInstruction.SUB, CPUType.BC8)},
            { OLdInstruction.SUBRA,         new OldInstructionInfo(2,OLdInstruction.SUBRA, CPUType.BC8)},
            { OLdInstruction.SUBRAX,        new OldInstructionInfo(2,OLdInstruction.SUBRAX, CPUType.BC8)},
            { OLdInstruction.SBB,           new OldInstructionInfo(2,OLdInstruction.SBB, CPUType.BC8)},
            { OLdInstruction.SBBRA,         new OldInstructionInfo(2,OLdInstruction.SBBRA, CPUType.BC8)},
            { OLdInstruction.SBBRAX,        new OldInstructionInfo(2,OLdInstruction.SBBRAX, CPUType.BC8)},
            { OLdInstruction.MUL,           new OldInstructionInfo(2,OLdInstruction.MUL, CPUType.BC8)},
            { OLdInstruction.MULRA,         new OldInstructionInfo(2,OLdInstruction.MULRA, CPUType.BC8)},
            { OLdInstruction.MULRAX,        new OldInstructionInfo(2,OLdInstruction.MULRAX, CPUType.BC8)},
            { OLdInstruction.DIV,           new OldInstructionInfo(2,OLdInstruction.DIV, CPUType.BC8)},
            { OLdInstruction.DIVRA,         new OldInstructionInfo(2,OLdInstruction.DIVRA, CPUType.BC8)},
            { OLdInstruction.DIVRAX,        new OldInstructionInfo(2,OLdInstruction.DIVRAX, CPUType.BC8)},
            { OLdInstruction.AND,           new OldInstructionInfo(2,OLdInstruction.AND, CPUType.BC8)},
            { OLdInstruction.ANDRA,         new OldInstructionInfo(2,OLdInstruction.ANDRA, CPUType.BC8)},
            { OLdInstruction.ANDRAX,        new OldInstructionInfo(2,OLdInstruction.ANDRAX, CPUType.BC8)},
            { OLdInstruction.OR,            new OldInstructionInfo(2,OLdInstruction.OR, CPUType.BC8)},
            { OLdInstruction.ORRA,          new OldInstructionInfo(2,OLdInstruction.ORRA, CPUType.BC8)},
            { OLdInstruction.ORRAX,         new OldInstructionInfo(2,OLdInstruction.ORRAX, CPUType.BC8)},
            { OLdInstruction.NOR,           new OldInstructionInfo(2,OLdInstruction.NOR, CPUType.BC8)},
            { OLdInstruction.NORRA,         new OldInstructionInfo(2,OLdInstruction.NORRA, CPUType.BC8)},
            { OLdInstruction.NORRAX,        new OldInstructionInfo(2,OLdInstruction.NORRAX, CPUType.BC8)},
            { OLdInstruction.XOR,           new OldInstructionInfo(2,OLdInstruction.XOR, CPUType.BC8)},
            { OLdInstruction.XORRA,         new OldInstructionInfo(2,OLdInstruction.XORRA, CPUType.BC8)},
            { OLdInstruction.XORRAX,        new OldInstructionInfo(2,OLdInstruction.XORRAX, CPUType.BC8)},
            { OLdInstruction.NOT,           new OldInstructionInfo(1,OLdInstruction.NOT, CPUType.BC8)},
            { OLdInstruction.NOTRA,         new OldInstructionInfo(1,OLdInstruction.NOTRA, CPUType.BC8)},
            { OLdInstruction.NOTRAX,        new OldInstructionInfo(1,OLdInstruction.NOTRAX, CPUType.BC8)},
            { OLdInstruction.SHL,           new OldInstructionInfo(2,OLdInstruction.SHL, CPUType.BC8)},
            { OLdInstruction.SHR,           new OldInstructionInfo(2,OLdInstruction.SHR, CPUType.BC8)},
            { OLdInstruction.ROL,           new OldInstructionInfo(2,OLdInstruction.ROL, CPUType.BC8)},
            { OLdInstruction.ROR,           new OldInstructionInfo(2,OLdInstruction.ROR, CPUType.BC8)},
            { OLdInstruction.INC,           new OldInstructionInfo(1,OLdInstruction.INC, CPUType.BC8)},
            { OLdInstruction.DEC,           new OldInstructionInfo(1,OLdInstruction.DEC, CPUType.BC8)},
            { OLdInstruction.NEG,           new OldInstructionInfo(1,OLdInstruction.NEG, CPUType.BC8)},
            { OLdInstruction.EXP,           new OldInstructionInfo(2,OLdInstruction.EXP, CPUType.BC8)},
            { OLdInstruction.SQRT,          new OldInstructionInfo(1,OLdInstruction.SQRT, CPUType.BC8)},
            { OLdInstruction.RNG,           new OldInstructionInfo(1,OLdInstruction.RNG, CPUType.BC8)},
            { OLdInstruction.SEB,           new OldInstructionInfo(2,OLdInstruction.SEB, CPUType.BC8)},
            { OLdInstruction.CLB,           new OldInstructionInfo(2,OLdInstruction.CLB, CPUType.BC8)},
            { OLdInstruction.TOB,           new OldInstructionInfo(2,OLdInstruction.TOB, CPUType.BC8)},
            { OLdInstruction.MOD,           new OldInstructionInfo(2,OLdInstruction.MOD, CPUType.BC8)},

            { OLdInstruction.FADD,          new OldInstructionInfo(2,OLdInstruction.FADD, CPUType.BC16)},
            { OLdInstruction.FSUB,          new OldInstructionInfo(2,OLdInstruction.FSUB, CPUType.BC16)},
            { OLdInstruction.FMUL,          new OldInstructionInfo(2,OLdInstruction.FMUL, CPUType.BC16)},
            { OLdInstruction.FDIV,          new OldInstructionInfo(2,OLdInstruction.FDIV, CPUType.BC16)},
            { OLdInstruction.FAND,          new OldInstructionInfo(2,OLdInstruction.FAND, CPUType.BC16)},
            { OLdInstruction.FOR,           new OldInstructionInfo(2,OLdInstruction.FOR, CPUType.BC16)},
            { OLdInstruction.FNOR,          new OldInstructionInfo(2,OLdInstruction.FNOR, CPUType.BC16)},
            { OLdInstruction.FXOR,          new OldInstructionInfo(2,OLdInstruction.FXOR, CPUType.BC16)},
            { OLdInstruction.FNOT,          new OldInstructionInfo(1,OLdInstruction.FNOT, CPUType.BC16)},

            { OLdInstruction.CBTA,          new OldInstructionInfo(2,OLdInstruction.CBTA, CPUType.BC16)},
            
            { OLdInstruction.CMPL,          new OldInstructionInfo(0,OLdInstruction.CMPL, CPUType.BC8)},
            { OLdInstruction.LOOR,          new OldInstructionInfo(0,OLdInstruction.LOOR, CPUType.BC8)},
            { OLdInstruction.MOVF,          new OldInstructionInfo(2,OLdInstruction.MOVF, CPUType.BC16)},

            { OLdInstruction.OUTB,          new OldInstructionInfo(2,OLdInstruction.OUTB, CPUType.BC8)},
            { OLdInstruction.OUTW,          new OldInstructionInfo(2,OLdInstruction.OUTW, CPUType.BC8)},
            { OLdInstruction.INP,           new OldInstructionInfo(2,OLdInstruction.INP, CPUType.BC8)},
            { OLdInstruction.INPW,          new OldInstructionInfo(2,OLdInstruction.INPW, CPUType.BC8)},

            { OLdInstruction.RETI,          new OldInstructionInfo(0,OLdInstruction.RETI, CPUType.BC8)},
            { OLdInstruction.NOP,           new OldInstructionInfo(0,OLdInstruction.NOP, CPUType.BC8)},
            { OLdInstruction.PUSHR,         new OldInstructionInfo(0,OLdInstruction.PUSHR, CPUType.BC8)},
            { OLdInstruction.POPR,          new OldInstructionInfo(0,OLdInstruction.POPR, CPUType.BC8)},
            { OLdInstruction.INT,           new OldInstructionInfo(1,OLdInstruction.INT, CPUType.BC8)},
            { OLdInstruction.BRK,           new OldInstructionInfo(0,OLdInstruction.BRK, CPUType.BC8)},
            { OLdInstruction.ENTER,         new OldInstructionInfo(0,OLdInstruction.ENTER, CPUType.BC8)},
            { OLdInstruction.LEAVE,         new OldInstructionInfo(0,OLdInstruction.LEAVE, CPUType.BC8)},
            { OLdInstruction.CPUID,         new OldInstructionInfo(1,OLdInstruction.CPUID, CPUType.BC16)},
            { OLdInstruction.LGDT,          new OldInstructionInfo(1,OLdInstruction.LGDT, CPUType.BC16)},
            { OLdInstruction.HALT,          new OldInstructionInfo(0,OLdInstruction.HALT, CPUType.BC8)},
        };
    }
}

public class OldInstructionInfo
{
    public int m_NumberOfOperands;
    public string m_OpCode;
    public OLdInstruction m_Instruction;
    public CPUType m_CPUType;
    public OldInstructionInfo(int NumberOfOperands, OLdInstruction instruction, CPUType cPU)
    {
        m_NumberOfOperands = NumberOfOperands;
        m_Instruction = instruction;

        m_OpCode = Convert.ToString((byte)instruction, 16);
        m_CPUType = cPU;
    }

    public OLdInstruction GetByteVersion()
    {
        if (AssemblerSettings.m_CPUType >= m_CPUType)
        {
            switch (m_Instruction)
            {
                case OLdInstruction.MOV:
                case OLdInstruction.MOVW:
                case OLdInstruction.MOVT:
                case OLdInstruction.MOVD:
                    return OLdInstruction.MOV;
                default:
                    //Console.WriteLine($"There are on byte versions of {m_Instruction}");
                    return m_Instruction;
            }
        }
        throw new NotImplementedException();
    }
    public OLdInstruction GetWordVersion()
    {
        if (AssemblerSettings.m_CPUType >= m_CPUType)
        {
            switch (m_Instruction)
            {
                case OLdInstruction.MOV:
                case OLdInstruction.MOVW:
                case OLdInstruction.MOVT:
                case OLdInstruction.MOVD:
                    return OLdInstruction.MOVW;
                default:
                    //Console.WriteLine($"There are no word versions of {m_Instruction}");
                    return m_Instruction;
            }
        }
        throw new NotImplementedException();
    }
    public OLdInstruction GetTbyteVersion()
    {
        if (AssemblerSettings.m_CPUType >= m_CPUType)
        {
            switch (m_Instruction)
            {
                case OLdInstruction.MOV:
                case OLdInstruction.MOVW:
                case OLdInstruction.MOVT:
                case OLdInstruction.MOVD:
                    return OLdInstruction.MOVT;
                default:
                    //Console.WriteLine($"There are no tbyte versions of {m_Instruction}");
                    return m_Instruction;
            }
        }
        throw new NotImplementedException();
    }
    public OLdInstruction GetDwordVersion()
    {
        if (AssemblerSettings.m_CPUType >= m_CPUType)
        {
            switch (m_Instruction)
            {
                case OLdInstruction.MOV:
                case OLdInstruction.MOVW:
                case OLdInstruction.MOVT:
                case OLdInstruction.MOVD:
                    return OLdInstruction.MOVD;
                case OLdInstruction.POP:
                default:
                    //Console.WriteLine($"There are no dword versions of {m_Instruction}");
                    return m_Instruction;
            }
        }
        throw new NotImplementedException();
    }

    public static implicit operator string(OldInstructionInfo info)
    {
        return info.m_OpCode;
    }
}

public enum Instruction : ushort
{
    MOV,
    CMP,
    PUSH,
    POP,
    CALL,
    RET,
    RETL,
    SEZ,
    TEST,
    SWAP,
    OUT,
    INP,
    SZE,
    SEE,
    SES,
    SEC,
    SEL,
    SEI,
    SEH,
    CZE,
    CLE,
    CLS,
    CLC,
    CLL,
    CLI,
    CLH,
    ADD,
    ADC,
    SUB,
    SBB,
    MUL,
    DIV,
    AND,
    OR,
    NOR,
    XOR,
    NOT,
    SHL,
    SHR,
    ROL,
    ROR,
    INC,
    DEC,
    NEG,
    EXP,
    SQRT,
    RNG,
    SEB,
    CLB,
    MOD,
    ADDF,
    SUBF,
    MULF,
    DIVF,
    CMPF,
    SQRTF,
    MODF,
    JMP,
    JZ,
    JNZ,
    JS,
    JNS,
    JE,
    JNE,
    JC,
    JNC,
    JL,
    JG,
    JLE,
    JGE,
    JNV,
    CBTA,
    CMPL,
    MOVF,
    RETI,
    NOP,
    PUSHR,
    POPR,
    INT,
    BRK,
    ENTER,
    LEAVE,
    CPUID,
    PUSHRR,
    POPRR,
    LGDT,
    HALT,
    none,
}

public class Instructions
{
    public static Dictionary<InstructionArgumentInfo, InstructionInfo> m_Instr;
    public Instructions()
    {
        m_Instr = new Dictionary<InstructionArgumentInfo, InstructionInfo>()
        {
            { new InstructionArgumentInfo(Instruction.MOV, ArgumentMode.GPregister, ArgumentMode.immediate),            new InstructionInfo(Instruction.MOV, CPUType.BC16,  "TTTTTTTT_TTSSGPRE_")},
            { new InstructionArgumentInfo(Instruction.MOV, ArgumentMode.GPregister, ArgumentMode.register),             new InstructionInfo(Instruction.MOV, CPUType.BC16,  "TTTTTTTT_TTSSGPRE_REGISTER")},
            { new InstructionArgumentInfo(Instruction.MOV, ArgumentMode.GPregister, ArgumentMode.RM, ArgumentMode.MOD), new InstructionInfo(Instruction.MOV, CPUType.BC16,  "TTTTTTTT_TTTTGPRE_SSRMMODE")},
            { new InstructionArgumentInfo(Instruction.MOV, ArgumentMode.register, ArgumentMode.immediate),              new InstructionInfo(Instruction.MOV, CPUType.BC16,  "TTTTTTSS_REG_")},
        };
    }
}

public class InstructionArgumentInfo
{
    public Instruction Instruction;
    public ArgumentMode[] Arguments;

    public InstructionArgumentInfo(Instruction instruction, params ArgumentMode[] arguments)
    {
        Instruction = instruction;
        Arguments = arguments;
    }

    public override bool Equals(object obj)
    {
        if (obj is not InstructionArgumentInfo other)
        {
            return false;
        }

        if (Instruction != other.Instruction)
        {
            return false;
        }

        if (Arguments.Length != other.Arguments.Length)
            return false;

        for (int i = 0; i < Arguments.Length; i++)
        {
            if (Arguments[i] != other.Arguments[i])
                return false;
        }

        return true;
    }

    public override int GetHashCode()
    {
        int hash = Instruction.GetHashCode();
        foreach (var arg in Arguments)
        {
            hash = (hash * 31) + arg.GetHashCode();
        }
        return hash;
    }

    public override string ToString()
    {
        string result = $"{Instruction} with {Arguments.Length} arguments ";

        for (int i = 0; i < Arguments.Length; i++)
        {
            result += $"{i}:{Arguments[i]} ";
        }

        return result;
    }
}

public class InstructionInfo
{
    public int m_NumberOfOperands;
    public string m_OpCode;
    public Instruction m_Instruction;
    public ArgumentMode[] m_argumentModes;
    public CPUType m_CPUType;
    public InstructionInfo(Instruction instruction, CPUType cPU, string Opcode, params ArgumentMode[] argumentModes)
    {
        m_NumberOfOperands = argumentModes.Length;
        m_Instruction = instruction;

        m_OpCode = Opcode;
        m_CPUType = cPU;
        m_argumentModes = argumentModes;
    }

    public string GenInstr(SizeAlignment instructionSize, params OperandArgument[] data)
    {
        string result = "";
        string instructionSizeBin = "";
        string instructionCodeBin = Convert.ToString((ushort)m_Instruction, 16).PadLeft(16, '0');
        int instructionCodeBinIndex = 0;
        int dataIndex = 0;

        switch (instructionSize)
        {
            case SizeAlignment._byte:
                instructionSizeBin = "00";
                break;
            case SizeAlignment._word:
                instructionSizeBin = "01";
                break;
            case SizeAlignment._dword:
                instructionSizeBin = "10";
                break;
            case SizeAlignment._qword:
                instructionSizeBin = "11";
                break;
            default:
                break;
        }

        for (int i = 0; i < m_OpCode.Length; i++)
        {
            if (m_OpCode[i] == 'T')
            {
                result += instructionCodeBin[instructionCodeBinIndex];
                instructionCodeBinIndex++;
                continue;
            }
            else if (m_OpCode[i] == 'R')
            {
                if (i + 1 < m_OpCode.Length && m_OpCode[i + 1] == 'M')
                {
                    if (GetNewOperandArgument(ref dataIndex, out OperandArgument _out, data))
                    {
                        result += _out.GetDataBin();
                        i += 3;
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                else if (i + 1 < m_OpCode.Length && m_OpCode[i + 1] == 'E')
                {
                    if (GetNewOperandArgument(ref dataIndex, out OperandArgument _out, data))
                    {
                        result += _out.GetDataBin();
                        i += 7;
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                else
                {
                    result += instructionSizeBin;
                    continue;
                }
                continue;
            }
            else if (m_OpCode[i] == 'M')
            {
                if (i + 1 < m_OpCode.Length && m_OpCode[i + 1] == 'O')
                {
                    if (GetNewOperandArgument(ref dataIndex, out OperandArgument _out, data))
                    {
                        result += _out.GetDataBin();
                        i += 3;
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                continue;
            }
            else if (m_OpCode[i] == 'S')
            {
                if (i + 1 < m_OpCode.Length && m_OpCode[i + 1] == 'R')
                {

                }
                else if (i + 1 < m_OpCode.Length && m_OpCode[i + 1] == 'S')
                {
                    result += instructionSizeBin;
                    i += 1;
                }
                else
                {
                    continue;
                }
                continue;
            }
            else if (m_OpCode[i] == 'G')
            {
                if (i + 1 < m_OpCode.Length && m_OpCode[i + 1] == 'P')
                {
                    if (GetNewOperandArgument(ref dataIndex, out OperandArgument _out, data))
                    {
                        result += _out.GetDataBin();
                        i += 3;
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                continue;
            }
            else if (m_OpCode[i] == '_')
            {
                // result += "_";
                continue;
            }
            else if (m_OpCode[i] == '0' || m_OpCode[i] == '1')
            {
                result += m_OpCode[i];
                continue;
            }
        }

        for (; dataIndex < data.Length; dataIndex++)
        {
            result += data[dataIndex].GetDataBin();
        }

        return result.TrimEnd('_');
    }

    bool GetNewOperandArgument(ref int index, out OperandArgument _out, params OperandArgument[] operandArguments)
    {
        if (index + 1 <= operandArguments.Length)
        {
            _out = operandArguments[index++];
            return true;
        }
        _out = new OperandArgument();
        return false;
    }

    public static implicit operator string(InstructionInfo info)
    {
        return info.m_OpCode;
    }
}
