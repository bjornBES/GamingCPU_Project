using System;
using System.Collections.Generic;

public enum Instruction : ushort
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
    INB =       0x1010,
    INW =       0x1014,
    
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
    MOVF =      0x4020,
    
    RETI =      0xF000,
    NOP =       0xF010,
    PUSHR =     0xF020,
    POPR =      0xF030,
    INT =       0xF040,
    BRK =       0xF050,
    ENTER =     0xF060,
    LEAVE =     0xF070,
    HALT =      0xFFF0,
    none,
}

public class Instructions
{
    public static Dictionary<Instruction, InstructionInfo> m_Instr;
    public Instructions()
    {
        m_Instr = new Dictionary<Instruction, InstructionInfo>()
        {
            { Instruction.MOV,          new InstructionInfo(2,Instruction.MOV, CPUType.BC8)},
            { Instruction.MOVRAL,       new InstructionInfo(1,Instruction.MOVRAL, CPUType.BC8)},
            { Instruction.MOVRBL,       new InstructionInfo(1,Instruction.MOVRBL, CPUType.BC8)},
            { Instruction.MOVRCL,       new InstructionInfo(1,Instruction.MOVRCL, CPUType.BC8)},
            { Instruction.MOVRDL,       new InstructionInfo(1,Instruction.MOVRDL, CPUType.BC8)},
            { Instruction.MOVRALCR0,    new InstructionInfo(0,Instruction.MOVRALCR0, CPUType.BC16)},
            { Instruction.MOVRCR0AL,    new InstructionInfo(0,Instruction.MOVRCR0AL, CPUType.BC16)},
            { Instruction.MOVW,         new InstructionInfo(2,Instruction.MOVW, CPUType.BC8)},
            { Instruction.MOVWRA,       new InstructionInfo(1,Instruction.MOVWRA, CPUType.BC8)},
            { Instruction.MOVWRB,       new InstructionInfo(1,Instruction.MOVWRB, CPUType.BC8)},
            { Instruction.MOVWRC,       new InstructionInfo(1,Instruction.MOVWRC, CPUType.BC8)},
            { Instruction.MOVWRD,       new InstructionInfo(1,Instruction.MOVWRD, CPUType.BC8)},
            { Instruction.MOVWRACR0,    new InstructionInfo(0,Instruction.MOVWRACR0, CPUType.BC32)},
            { Instruction.MOVWRCR0A,    new InstructionInfo(0,Instruction.MOVWRCR0A, CPUType.BC32)},
            { Instruction.MOVT,         new InstructionInfo(2,Instruction.MOVT, CPUType.BC16)},
            { Instruction.MOVD,         new InstructionInfo(2,Instruction.MOVD, CPUType.BC16)},
            { Instruction.MOVDRAX,      new InstructionInfo(1,Instruction.MOVDRAX, CPUType.BC16)},
            { Instruction.MOVDRBX,      new InstructionInfo(1,Instruction.MOVDRBX, CPUType.BC16)},
            { Instruction.MOVDRCX,      new InstructionInfo(1,Instruction.MOVDRCX, CPUType.BC16)},
            { Instruction.MOVDRDX,      new InstructionInfo(1,Instruction.MOVDRDX, CPUType.BC16)},
            { Instruction.CMP,          new InstructionInfo(2,Instruction.CMP, CPUType.BC16)},
            { Instruction.CMPZ,         new InstructionInfo(2,Instruction.CMPZ, CPUType.BC16)},
            { Instruction.CMPRA,        new InstructionInfo(2,Instruction.CMPRA, CPUType.BC16)},
            { Instruction.CMPRAX,       new InstructionInfo(2,Instruction.CMPRAX, CPUType.BC16)},
            { Instruction.PUSH,         new InstructionInfo(1,Instruction.PUSH, CPUType.BC16)},
            { Instruction.POP,          new InstructionInfo(1,Instruction.POP, CPUType.BC8)},
            { Instruction.CALL,         new InstructionInfo(1,Instruction.CALL, CPUType.BC8)},
            { Instruction.CALLRHL,      new InstructionInfo(0,Instruction.CALLRHL, CPUType.BC8)},
            { Instruction.RET,          new InstructionInfo(1,Instruction.RET, CPUType.BC8)},
            { Instruction.RETL,         new InstructionInfo(0,Instruction.RETL, CPUType.BC8)},
            { Instruction.RETZ,         new InstructionInfo(0,Instruction.RETZ, CPUType.BC8)},
            { Instruction.SEZ,          new InstructionInfo(1,Instruction.SEZ, CPUType.BC8)},
            { Instruction.SEZRAL,       new InstructionInfo(0,Instruction.SEZRAL, CPUType.BC8)},
            { Instruction.SEZRA,        new InstructionInfo(0,Instruction.SEZRA, CPUType.BC8)},
            { Instruction.SEZRAX,       new InstructionInfo(0,Instruction.SEZRAX, CPUType.BC16)},
            { Instruction.TEST,         new InstructionInfo(1,Instruction.TEST, CPUType.BC8)},
            { Instruction.TESTRAL,      new InstructionInfo(0,Instruction.TESTRAL, CPUType.BC8)},
            { Instruction.TESTRA,       new InstructionInfo(0,Instruction.TESTRA, CPUType.BC8)},
            { Instruction.TESTRAX,      new InstructionInfo(0,Instruction.TESTRAX, CPUType.BC16)},
            { Instruction.SWAP,         new InstructionInfo(2,Instruction.SWAP, CPUType.BC8)},

            { Instruction.JMP,          new InstructionInfo(1,Instruction.JMP, CPUType.BC8)},
            { Instruction.JZ,           new InstructionInfo(1,Instruction.JZ, CPUType.BC8)},
            { Instruction.JNZ,          new InstructionInfo(1,Instruction.JNZ, CPUType.BC8)},
            { Instruction.JS,           new InstructionInfo(1,Instruction.JS, CPUType.BC8)},
            { Instruction.JNS,          new InstructionInfo(1,Instruction.JNS, CPUType.BC8)},
            { Instruction.JE,           new InstructionInfo(1,Instruction.JE, CPUType.BC8)},
            { Instruction.JNE,          new InstructionInfo(1,Instruction.JNE, CPUType.BC8)},
            { Instruction.JC,           new InstructionInfo(1,Instruction.JC, CPUType.BC8)},
            { Instruction.JNC,          new InstructionInfo(1,Instruction.JNC, CPUType.BC8)},
            { Instruction.JL,           new InstructionInfo(1,Instruction.JL, CPUType.BC8)},
            { Instruction.JG,           new InstructionInfo(1,Instruction.JG, CPUType.BC8)},
            { Instruction.JLE,          new InstructionInfo(1,Instruction.JLE, CPUType.BC8)},
            { Instruction.JGE,          new InstructionInfo(1,Instruction.JGE, CPUType.BC8)},
            { Instruction.JNV,          new InstructionInfo(1,Instruction.JNV, CPUType.BC8)},

            { Instruction.SZE,          new InstructionInfo(0,Instruction.SZE, CPUType.BC8)},
            { Instruction.SEE,          new InstructionInfo(0,Instruction.SEE, CPUType.BC8)},
            { Instruction.SES,          new InstructionInfo(0,Instruction.SES, CPUType.BC8)},
            { Instruction.SEC,          new InstructionInfo(0,Instruction.SEC, CPUType.BC8)},
            { Instruction.SEL,          new InstructionInfo(0,Instruction.SEL, CPUType.BC8)},
            { Instruction.SEI,          new InstructionInfo(0,Instruction.SEI, CPUType.BC8)},
            { Instruction.SEH,          new InstructionInfo(0,Instruction.SEH, CPUType.BC8)},
            { Instruction.CZE,          new InstructionInfo(0,Instruction.CZE, CPUType.BC8)},
            { Instruction.CLE,          new InstructionInfo(0,Instruction.CLE, CPUType.BC8)},
            { Instruction.CLS,          new InstructionInfo(0,Instruction.CLS, CPUType.BC8)},
            { Instruction.CLC,          new InstructionInfo(0,Instruction.CLC, CPUType.BC8)},
            { Instruction.CLL,          new InstructionInfo(0,Instruction.CLL, CPUType.BC8)},
            { Instruction.CLI,          new InstructionInfo(0,Instruction.CLI, CPUType.BC8)},
            { Instruction.CLH,          new InstructionInfo(0,Instruction.CLH, CPUType.BC8)},
            { Instruction.ADD,          new InstructionInfo(2,Instruction.ADD, CPUType.BC8)},
            { Instruction.ADDRA,        new InstructionInfo(2,Instruction.ADDRA, CPUType.BC8)},
            { Instruction.ADDRAX,       new InstructionInfo(2,Instruction.ADDRAX, CPUType.BC8)},
            { Instruction.ADC,          new InstructionInfo(2,Instruction.ADC, CPUType.BC8)},
            { Instruction.ADCRA,        new InstructionInfo(2,Instruction.ADCRA, CPUType.BC8)},
            { Instruction.ADCRAX,       new InstructionInfo(2,Instruction.ADCRAX, CPUType.BC8)},
            { Instruction.SUB,          new InstructionInfo(2,Instruction.SUB, CPUType.BC8)},
            { Instruction.SUBRA,        new InstructionInfo(2,Instruction.SUBRA, CPUType.BC8)},
            { Instruction.SUBRAX,       new InstructionInfo(2,Instruction.SUBRAX, CPUType.BC8)},
            { Instruction.SBB,          new InstructionInfo(2,Instruction.SBB, CPUType.BC8)},
            { Instruction.SBBRA,        new InstructionInfo(2,Instruction.SBBRA, CPUType.BC8)},
            { Instruction.SBBRAX,       new InstructionInfo(2,Instruction.SBBRAX, CPUType.BC8)},
            { Instruction.MUL,          new InstructionInfo(2,Instruction.MUL, CPUType.BC8)},
            { Instruction.MULRA,        new InstructionInfo(2,Instruction.MULRA, CPUType.BC8)},
            { Instruction.MULRAX,       new InstructionInfo(2,Instruction.MULRAX, CPUType.BC8)},
            { Instruction.DIV,          new InstructionInfo(2,Instruction.DIV, CPUType.BC8)},
            { Instruction.DIVRA,        new InstructionInfo(2,Instruction.DIVRA, CPUType.BC8)},
            { Instruction.DIVRAX,       new InstructionInfo(2,Instruction.DIVRAX, CPUType.BC8)},
            { Instruction.AND,          new InstructionInfo(2,Instruction.AND, CPUType.BC8)},
            { Instruction.ANDRA,        new InstructionInfo(2,Instruction.ANDRA, CPUType.BC8)},
            { Instruction.ANDRAX,       new InstructionInfo(2,Instruction.ANDRAX, CPUType.BC8)},
            { Instruction.OR,           new InstructionInfo(2,Instruction.OR, CPUType.BC8)},
            { Instruction.ORRA,         new InstructionInfo(2,Instruction.ORRA, CPUType.BC8)},
            { Instruction.ORRAX,        new InstructionInfo(2,Instruction.ORRAX, CPUType.BC8)},
            { Instruction.NOR,          new InstructionInfo(2,Instruction.NOR, CPUType.BC8)},
            { Instruction.NORRA,        new InstructionInfo(2,Instruction.NORRA, CPUType.BC8)},
            { Instruction.NORRAX,       new InstructionInfo(2,Instruction.NORRAX, CPUType.BC8)},
            { Instruction.XOR,          new InstructionInfo(2,Instruction.XOR, CPUType.BC8)},
            { Instruction.XORRA,        new InstructionInfo(2,Instruction.XORRA, CPUType.BC8)},
            { Instruction.XORRAX,       new InstructionInfo(2,Instruction.XORRAX, CPUType.BC8)},
            { Instruction.NOT,          new InstructionInfo(1,Instruction.NOT, CPUType.BC8)},
            { Instruction.NOTRA,        new InstructionInfo(1,Instruction.NOTRA, CPUType.BC8)},
            { Instruction.NOTRAX,       new InstructionInfo(1,Instruction.NOTRAX, CPUType.BC8)},
            { Instruction.SHL,          new InstructionInfo(2,Instruction.SHL, CPUType.BC8)},
            { Instruction.SHR,          new InstructionInfo(2,Instruction.SHR, CPUType.BC8)},
            { Instruction.ROL,          new InstructionInfo(2,Instruction.ROL, CPUType.BC8)},
            { Instruction.ROR,          new InstructionInfo(2,Instruction.ROR, CPUType.BC8)},
            { Instruction.INC,          new InstructionInfo(1,Instruction.INC, CPUType.BC8)},
            { Instruction.DEC,          new InstructionInfo(1,Instruction.DEC, CPUType.BC8)},
            { Instruction.NEG,          new InstructionInfo(1,Instruction.NEG, CPUType.BC8)},
            { Instruction.EXP,          new InstructionInfo(2,Instruction.EXP, CPUType.BC8)},
            { Instruction.SQRT,         new InstructionInfo(1,Instruction.SQRT, CPUType.BC8)},
            { Instruction.RNG,          new InstructionInfo(1,Instruction.RNG, CPUType.BC8)},
            { Instruction.SEB,          new InstructionInfo(2,Instruction.SEB, CPUType.BC8)},
            { Instruction.CLB,          new InstructionInfo(2,Instruction.CLB, CPUType.BC8)},
            { Instruction.TOB,          new InstructionInfo(2,Instruction.TOB, CPUType.BC8)},
            { Instruction.MOD,          new InstructionInfo(2,Instruction.MOD, CPUType.BC8)},

            { Instruction.FADD,         new InstructionInfo(2,Instruction.FADD, CPUType.BC16)},
            { Instruction.FSUB,         new InstructionInfo(2,Instruction.FSUB, CPUType.BC16)},
            { Instruction.FMUL,         new InstructionInfo(2,Instruction.FMUL, CPUType.BC16)},
            { Instruction.FDIV,         new InstructionInfo(2,Instruction.FDIV, CPUType.BC16)},
            { Instruction.FAND,         new InstructionInfo(2,Instruction.FAND, CPUType.BC16)},
            { Instruction.FOR,          new InstructionInfo(2,Instruction.FOR, CPUType.BC16)},
            { Instruction.FNOR,         new InstructionInfo(2,Instruction.FNOR, CPUType.BC16)},
            { Instruction.FXOR,         new InstructionInfo(2,Instruction.FXOR, CPUType.BC16)},
            { Instruction.FNOT,         new InstructionInfo(1,Instruction.FNOT, CPUType.BC16)},

            { Instruction.CBTA,         new InstructionInfo(2,Instruction.CBTA, CPUType.BC16)},
            
            { Instruction.CMPL,         new InstructionInfo(0,Instruction.CMPL, CPUType.BC8)},
            { Instruction.MOVF,         new InstructionInfo(2,Instruction.MOVF, CPUType.BC16)},

            { Instruction.OUTB,         new InstructionInfo(2,Instruction.OUTB, CPUType.BC8)},
            { Instruction.OUTW,         new InstructionInfo(2,Instruction.OUTW, CPUType.BC8)},
            { Instruction.INB,          new InstructionInfo(2,Instruction.INB, CPUType.BC8)},
            { Instruction.INW,          new InstructionInfo(2,Instruction.INW, CPUType.BC8)},

            { Instruction.RETI,         new InstructionInfo(0,Instruction.RETI, CPUType.BC8)},
            { Instruction.NOP,          new InstructionInfo(0,Instruction.NOP, CPUType.BC8)},
            { Instruction.PUSHR,        new InstructionInfo(0,Instruction.PUSHR, CPUType.BC8)},
            { Instruction.POPR,         new InstructionInfo(0,Instruction.POPR, CPUType.BC8)},
            { Instruction.INT,          new InstructionInfo(1,Instruction.INT, CPUType.BC8)},
            { Instruction.BRK,          new InstructionInfo(0,Instruction.BRK, CPUType.BC8)},
            { Instruction.ENTER,        new InstructionInfo(0,Instruction.ENTER, CPUType.BC8)},
            { Instruction.LEAVE,        new InstructionInfo(0,Instruction.LEAVE, CPUType.BC8)},
            { Instruction.HALT,         new InstructionInfo(0,Instruction.HALT, CPUType.BC8)},
        };
    }
}

public class InstructionInfo
{
    public int m_NumberOfOperands;
    public string m_OpCode;
    public Instruction m_Instruction;
    public CPUType m_CPUType;
    public InstructionInfo(int NumberOfOperands, Instruction instruction, CPUType cPU)
    {
        m_NumberOfOperands = NumberOfOperands;
        m_Instruction = instruction;

        m_OpCode = Convert.ToString((byte)instruction, 16);
        m_CPUType = cPU;
    }

    public Instruction GetByteVersion()
    {
        if (AssemblerSettings.m_CPUType >= m_CPUType)
        {
            switch (m_Instruction)
            {
                case Instruction.MOV:
                case Instruction.MOVW:
                case Instruction.MOVT:
                case Instruction.MOVD:
                    return Instruction.MOV;
                default:
                    //Console.WriteLine($"There are on byte versions of {m_Instruction}");
                    return m_Instruction;
            }
        }
        throw new NotImplementedException();
    }
    public Instruction GetWordVersion()
    {
        if (AssemblerSettings.m_CPUType >= m_CPUType)
        {
            switch (m_Instruction)
            {
                case Instruction.MOV:
                case Instruction.MOVW:
                case Instruction.MOVT:
                case Instruction.MOVD:
                    return Instruction.MOVW;
                default:
                    //Console.WriteLine($"There are no word versions of {m_Instruction}");
                    return m_Instruction;
            }
        }
        throw new NotImplementedException();
    }
    public Instruction GetTbyteVersion()
    {
        if (AssemblerSettings.m_CPUType >= m_CPUType)
        {
            switch (m_Instruction)
            {
                case Instruction.MOV:
                case Instruction.MOVW:
                case Instruction.MOVT:
                case Instruction.MOVD:
                    return Instruction.MOVT;
                default:
                    //Console.WriteLine($"There are no tbyte versions of {m_Instruction}");
                    return m_Instruction;
            }
        }
        throw new NotImplementedException();
    }
    public Instruction GetDwordVersion()
    {
        if (AssemblerSettings.m_CPUType >= m_CPUType)
        {
            switch (m_Instruction)
            {
                case Instruction.MOV:
                case Instruction.MOVW:
                case Instruction.MOVT:
                case Instruction.MOVD:
                    return Instruction.MOVD;
                case Instruction.POP:
                default:
                    //Console.WriteLine($"There are no dword versions of {m_Instruction}");
                    return m_Instruction;
            }
        }
        throw new NotImplementedException();
    }

    public static implicit operator string(InstructionInfo info)
    {
        return info.m_OpCode;
    }
}
