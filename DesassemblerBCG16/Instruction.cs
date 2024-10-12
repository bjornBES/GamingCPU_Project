using System;
using System.Collections.Generic;

public enum Instruction : ushort
{
    MOV =       0x0000,
    MOVRAL =    0x0001,
    MOVRBL =    0x0002,
    MOVRCL =    0x0003,
    MOVRDL =    0x0004,
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
    CMP =       0x0400,
    PUSH =      0x0500,
    PUSHW =     0x0504,
    PUSHT =     0x0508,
    PUSHD =     0x050C,
    POP =       0x0600,
    POPW =      0x0604,
    POPT =      0x0608,
    POPD =      0x060C,
    CALL =      0x0700,
    CALLRHL =   0x0701,
    RET =       0x0800,
    RETZ =      0x0900,
    SEZ =       0x0A00,
    SEZRAL =    0x0A01,
    SEZRA =     0x0A02,
    SEZRAX =    0x0A03,
    TEST =      0x0B00,
    TESTRAL =   0x0B01,
    TESTRA =    0x0B02,
    TESTRAX =   0x0B03,

    OUTB =      0x1000,
    OUTW =      0x1004,
    INPB =      0x1010,
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
    SUB =       0x2030,
    MUL =       0x2040,
    DIV =       0x2050,
    AND =       0x2060,
    OR =        0x2070,
    NOR =       0x2080,
    XOR =       0x2090,
    NOT =       0x20A0,
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
    JL =        0x3040,
    JG =        0x3041,
    JLE =       0x3042,
    JGE =       0x3043,
    JNV =       0x3051,
    
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
            { Instruction.MOVW,         new InstructionInfo(2,Instruction.MOVW, CPUType.BC8)},
            { Instruction.MOVWRA,       new InstructionInfo(1,Instruction.MOVWRA, CPUType.BC8)},
            { Instruction.MOVWRB,       new InstructionInfo(1,Instruction.MOVWRB, CPUType.BC8)},
            { Instruction.MOVWRC,       new InstructionInfo(1,Instruction.MOVWRC, CPUType.BC8)},
            { Instruction.MOVWRD,       new InstructionInfo(1,Instruction.MOVWRD, CPUType.BC8)},
            { Instruction.MOVWRACR0,    new InstructionInfo(0,Instruction.MOVWRACR0, CPUType.BC16)},
            { Instruction.MOVWRCR0A,    new InstructionInfo(0,Instruction.MOVWRCR0A, CPUType.BC16)},
            { Instruction.MOVT,         new InstructionInfo(2,Instruction.MOVT, CPUType.BC16)},
            { Instruction.MOVD,         new InstructionInfo(2,Instruction.MOVD, CPUType.BC16)},
            { Instruction.MOVDRAX,      new InstructionInfo(1,Instruction.MOVDRAX, CPUType.BC16)},
            { Instruction.CMP,          new InstructionInfo(2,Instruction.CMP, CPUType.BC16)},
            { Instruction.PUSH,         new InstructionInfo(1,Instruction.PUSH, CPUType.BC16)},
            { Instruction.PUSHW,        new InstructionInfo(1,Instruction.PUSHW, CPUType.BC8)},
            { Instruction.PUSHT,        new InstructionInfo(1,Instruction.PUSHT, CPUType.BC16)},
            { Instruction.PUSHD,        new InstructionInfo(1,Instruction.PUSHD, CPUType.BC16)},
            { Instruction.POP,          new InstructionInfo(1,Instruction.POP, CPUType.BC8)},
            { Instruction.POPW,         new InstructionInfo(1,Instruction.POPW, CPUType.BC8)},
            { Instruction.POPT,         new InstructionInfo(1,Instruction.POPT, CPUType.BC16)},
            { Instruction.POPD,         new InstructionInfo(1,Instruction.POPD, CPUType.BC16)},
            { Instruction.CALL,         new InstructionInfo(1,Instruction.CALL, CPUType.BC8)},
            { Instruction.CALLRHL,      new InstructionInfo(0,Instruction.CALLRHL, CPUType.BC8)},
            { Instruction.RET,          new InstructionInfo(1,Instruction.RET, CPUType.BC8)},
            { Instruction.RETZ,         new InstructionInfo(0,Instruction.RETZ, CPUType.BC8)},
            { Instruction.SEZ,          new InstructionInfo(1,Instruction.SEZ, CPUType.BC8)},
            { Instruction.SEZRAL,       new InstructionInfo(0,Instruction.SEZRAL, CPUType.BC8)},
            { Instruction.SEZRA,        new InstructionInfo(0,Instruction.SEZRA, CPUType.BC8)},
            { Instruction.SEZRAX,       new InstructionInfo(0,Instruction.SEZRAX, CPUType.BC16)},
            { Instruction.TEST,         new InstructionInfo(1,Instruction.TEST, CPUType.BC8)},
            { Instruction.TESTRAL,      new InstructionInfo(0,Instruction.TESTRAL, CPUType.BC8)},
            { Instruction.TESTRA,       new InstructionInfo(0,Instruction.TESTRA, CPUType.BC8)},
            { Instruction.TESTRAX,      new InstructionInfo(0,Instruction.TESTRAX, CPUType.BC16)},

            { Instruction.JMP,          new InstructionInfo(1,Instruction.JMP, CPUType.BC8)},
            { Instruction.JZ,           new InstructionInfo(1,Instruction.JZ, CPUType.BC8)},
            { Instruction.JNZ,          new InstructionInfo(1,Instruction.JNZ, CPUType.BC8)},
            { Instruction.JS,           new InstructionInfo(1,Instruction.JS, CPUType.BC8)},
            { Instruction.JNS,          new InstructionInfo(1,Instruction.JNS, CPUType.BC8)},
            { Instruction.JE,           new InstructionInfo(1,Instruction.JE, CPUType.BC8)},
            { Instruction.JNE,          new InstructionInfo(1,Instruction.JNE, CPUType.BC8)},
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
            { Instruction.SUB,          new InstructionInfo(2,Instruction.SUB, CPUType.BC8)},
            { Instruction.MUL,          new InstructionInfo(2,Instruction.MUL, CPUType.BC8)},
            { Instruction.DIV,          new InstructionInfo(2,Instruction.DIV, CPUType.BC8)},
            { Instruction.AND,          new InstructionInfo(2,Instruction.AND, CPUType.BC8)},
            { Instruction.OR,           new InstructionInfo(2,Instruction.OR, CPUType.BC8)},
            { Instruction.NOR,          new InstructionInfo(2,Instruction.NOR, CPUType.BC8)},
            { Instruction.XOR,          new InstructionInfo(2,Instruction.XOR, CPUType.BC8)},
            { Instruction.NOT,          new InstructionInfo(1,Instruction.NOT, CPUType.BC8)},
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
            { Instruction.INPB,         new InstructionInfo(2,Instruction.INPB, CPUType.BC8)},
            { Instruction.INPW,         new InstructionInfo(2,Instruction.INPW, CPUType.BC8)},

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
        if (DisAssemblerSettings.m_CPUType >= m_CPUType)
        {
            switch (m_Instruction)
            {
                case Instruction.MOV:
                case Instruction.MOVW:
                case Instruction.MOVT:
                case Instruction.MOVD:
                    return Instruction.MOV;
                case Instruction.POP:
                case Instruction.POPW:
                case Instruction.POPT:
                case Instruction.POPD:
                    return Instruction.POP;
                case Instruction.PUSH:
                case Instruction.PUSHW:
                case Instruction.PUSHT:
                case Instruction.PUSHD:
                    return Instruction.PUSH;
                default:
                    //Console.WriteLine($"There are on byte versions of {m_Instruction}");
                    return m_Instruction;
            }
        }
        throw new NotImplementedException();
    }
    public Instruction GetWordVersion()
    {
        if (DisAssemblerSettings.m_CPUType >= m_CPUType)
        {
            switch (m_Instruction)
            {
                case Instruction.MOV:
                case Instruction.MOVW:
                case Instruction.MOVT:
                case Instruction.MOVD:
                    return Instruction.MOVW;
                case Instruction.POP:
                case Instruction.POPW:
                case Instruction.POPT:
                case Instruction.POPD:
                    return Instruction.POPW;
                case Instruction.PUSH:
                case Instruction.PUSHW:
                case Instruction.PUSHT:
                case Instruction.PUSHD:
                    return Instruction.PUSHW;
                default:
                    //Console.WriteLine($"There are no word versions of {m_Instruction}");
                    return m_Instruction;
            }
        }
        throw new NotImplementedException();
    }
    public Instruction GetTbyteVersion()
    {
        if (DisAssemblerSettings.m_CPUType >= m_CPUType)
        {
            switch (m_Instruction)
            {
                case Instruction.MOV:
                case Instruction.MOVW:
                case Instruction.MOVT:
                case Instruction.MOVD:
                    return Instruction.MOVT;
                case Instruction.POP:
                case Instruction.POPW:
                case Instruction.POPT:
                case Instruction.POPD:
                    return Instruction.POPT;
                case Instruction.PUSH:
                case Instruction.PUSHW:
                case Instruction.PUSHT:
                case Instruction.PUSHD:
                    return Instruction.PUSHT;
                default:
                    //Console.WriteLine($"There are no tbyte versions of {m_Instruction}");
                    return m_Instruction;
            }
        }
        throw new NotImplementedException();
    }
    public Instruction GetDwordVersion()
    {
        if (DisAssemblerSettings.m_CPUType >= m_CPUType)
        {
            switch (m_Instruction)
            {
                case Instruction.MOV:
                case Instruction.MOVW:
                case Instruction.MOVT:
                case Instruction.MOVD:
                    return Instruction.MOVD;
                case Instruction.POP:
                case Instruction.POPW:
                case Instruction.POPT:
                case Instruction.POPD:
                    return Instruction.POPD;
                case Instruction.PUSH:
                case Instruction.PUSHW:
                case Instruction.PUSHT:
                case Instruction.PUSHD:
                    return Instruction.PUSHD;
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
