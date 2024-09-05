using System;
using System.Collections.Generic;

public enum Instruction : byte
{
    none,

    MOV = 0x00,
    MOVW = 0x01,
    MOVT = 0x02,
    MOVD = 0x03,
    CMP = 0x04,
    PUSH = 0x05,
    POP = 0x06,
    POPW = 0x07,
    POPT = 0x08,
    POPD = 0x09,
    CALL = 0x0A,
    RET = 0x0B,
    RETZ = 0x0C,
    SEZ = 0x0D,
    TEST = 0x0E,

    JMP = 0x10,
    JZ = 0x11,
    JNZ = 0x12,
    JS = 0x13,
    JNS = 0x14,
    JE = 0x15,
    JNE = 0x16,
    JL = 0x17,
    JG = 0x18,
    JLE = 0x19,
    JGE = 0x1A,
    JNV = 0x1B,

    JTZ = 0x2A,
    JBA = 0x2B,

    SEF = 0x2E,
    CLF = 0x2F,
    ADD = 0x30,
    SUB = 0x31,
    MUL = 0x32,
    DIV = 0x33,
    AND = 0x34,
    OR = 0x35,
    NOR = 0x36,
    XOR = 0x37,
    NOT = 0x38,
    SHL = 0x39,
    SHR = 0x3A,
    ROL = 0x3B,
    ROR = 0x3C,
    INC = 0x3D,
    DEC = 0x3E,
    NEG = 0x3F,
    AVG = 0x40,
    EXP = 0x41,
    SQRT = 0x42,
    RNG = 0x43,
    SEB = 0x44,
    CLB = 0x45,
    TOB = 0x46,
    MOD = 0x47,

    FADD = 0x50,
    FSUB = 0x51,
    FMUL = 0x52,
    FDIV = 0x53,
    FAND = 0x54,
    FOR = 0x55,
    FNOR = 0x56,
    FXOR = 0x57,
    FNOT = 0x58,

    INB = 0x60,
    OUTB = 0x61,

    MOVS = 0x93,
    CMPSTR = 0x94,
    MOVF = 0x95,

    DATE = 0x9D,
    DELAY = 0x9E,
    TIME = 0x9F,

    RTI = 0xF8,
    NOP = 0xF9,
    RISERR = 0xFA,
    PUSHR = 0xFB,
    POPR = 0xFC,
    INT = 0xFD,
    BRK = 0xFE,
    HALT = 0xFF,
}

public static class Instructions
{
    public static Dictionary<Instruction, InstructionInfo> instr = new Dictionary<Instruction, InstructionInfo>()
    {
        { Instruction.MOV,      new InstructionInfo(2,Instruction.MOV, CPUType.BCG8)},
        { Instruction.MOVW,     new InstructionInfo(2,Instruction.MOVW, CPUType.BCG8)},
        { Instruction.MOVT,     new InstructionInfo(2,Instruction.MOVT, CPUType.BCG8)},
        { Instruction.MOVD,     new InstructionInfo(2,Instruction.MOVD, CPUType.BCG8)},
        { Instruction.CMP,      new InstructionInfo(2,Instruction.CMP, CPUType.BCG8)},
        { Instruction.PUSH,     new InstructionInfo(1,Instruction.PUSH, CPUType.BCG8)},
        { Instruction.POP,      new InstructionInfo(1,Instruction.POP, CPUType.BCG8)},
        { Instruction.POPW,     new InstructionInfo(1,Instruction.POPW, CPUType.BCG8)},
        { Instruction.POPT,     new InstructionInfo(1,Instruction.POPT, CPUType.BCG8)},
        { Instruction.POPD,     new InstructionInfo(1,Instruction.POPD, CPUType.BCG8)},
        { Instruction.CALL,     new InstructionInfo(1,Instruction.CALL, CPUType.BCG8)},
        { Instruction.RET,      new InstructionInfo(1,Instruction.RET, CPUType.BCG8)},
        { Instruction.RETZ,     new InstructionInfo(0,Instruction.RETZ, CPUType.BCG8)},
        { Instruction.SEZ,      new InstructionInfo(1,Instruction.SEZ, CPUType.BCG8)},
        { Instruction.TEST,     new InstructionInfo(1,Instruction.TEST, CPUType.BCG8)},

        { Instruction.JMP,      new InstructionInfo(1,Instruction.JMP, CPUType.BCG8)},
        { Instruction.JZ,       new InstructionInfo(1,Instruction.JZ, CPUType.BCG8)},
        { Instruction.JNZ,      new InstructionInfo(1,Instruction.JNZ, CPUType.BCG8)},
        { Instruction.JS,       new InstructionInfo(1,Instruction.JS, CPUType.BCG8)},
        { Instruction.JNS,      new InstructionInfo(1,Instruction.JNS, CPUType.BCG8)},
        { Instruction.JE,       new InstructionInfo(1,Instruction.JE, CPUType.BCG8)},
        { Instruction.JNE,      new InstructionInfo(1,Instruction.JNE, CPUType.BCG8)},
        { Instruction.JL,       new InstructionInfo(1,Instruction.JL, CPUType.BCG8)},
        { Instruction.JG,       new InstructionInfo(1,Instruction.JG, CPUType.BCG8)},
        { Instruction.JLE,      new InstructionInfo(1,Instruction.JLE, CPUType.BCG8)},
        { Instruction.JGE,      new InstructionInfo(1,Instruction.JGE, CPUType.BCG8)},
        { Instruction.JNV,      new InstructionInfo(1,Instruction.JNV, CPUType.BCG8)},

        { Instruction.JTZ,      new InstructionInfo(0,Instruction.JTZ, CPUType.BCG8)},
        { Instruction.JBA,      new InstructionInfo(2,Instruction.JBA, CPUType.BCG8)},

        { Instruction.SEF,      new InstructionInfo(1,Instruction.SEF, CPUType.BCG8)},
        { Instruction.CLF,      new InstructionInfo(1,Instruction.CLF, CPUType.BCG8)},
        { Instruction.ADD,      new InstructionInfo(2,Instruction.ADD, CPUType.BCG8)},
        { Instruction.SUB,      new InstructionInfo(2,Instruction.SUB, CPUType.BCG8)},
        { Instruction.MUL,      new InstructionInfo(2,Instruction.MUL, CPUType.BCG8)},
        { Instruction.DIV,      new InstructionInfo(2,Instruction.DIV, CPUType.BCG8)},
        { Instruction.AND,      new InstructionInfo(2,Instruction.AND, CPUType.BCG8)},
        { Instruction.OR,       new InstructionInfo(2,Instruction.OR, CPUType.BCG8)},
        { Instruction.NOR,      new InstructionInfo(2,Instruction.NOR, CPUType.BCG8)},
        { Instruction.XOR,      new InstructionInfo(2,Instruction.XOR, CPUType.BCG8)},
        { Instruction.NOT,      new InstructionInfo(1,Instruction.NOT, CPUType.BCG8)},
        { Instruction.SHL,      new InstructionInfo(2,Instruction.SHL, CPUType.BCG8)},
        { Instruction.SHR,      new InstructionInfo(2,Instruction.SHR, CPUType.BCG8)},
        { Instruction.ROL,      new InstructionInfo(2,Instruction.ROL, CPUType.BCG8)},
        { Instruction.ROR,      new InstructionInfo(2,Instruction.ROR, CPUType.BCG8)},
        { Instruction.INC,      new InstructionInfo(1,Instruction.INC, CPUType.BCG8)},
        { Instruction.DEC,      new InstructionInfo(1,Instruction.DEC, CPUType.BCG8)},
        { Instruction.NEG,      new InstructionInfo(1,Instruction.NEG, CPUType.BCG8)},
        { Instruction.AVG,      new InstructionInfo(2,Instruction.AVG, CPUType.BCG8)},
        { Instruction.EXP,      new InstructionInfo(2,Instruction.EXP, CPUType.BCG8)},
        { Instruction.SQRT,     new InstructionInfo(1,Instruction.SQRT, CPUType.BCG8)},
        { Instruction.RNG,      new InstructionInfo(1,Instruction.RNG, CPUType.BCG8)},
        { Instruction.SEB,      new InstructionInfo(2,Instruction.SEB, CPUType.BCG8)},
        { Instruction.CLB,      new InstructionInfo(2,Instruction.CLB, CPUType.BCG8)},
        { Instruction.TOB,      new InstructionInfo(2,Instruction.TOB, CPUType.BCG8)},
        { Instruction.MOD,      new InstructionInfo(2,Instruction.MOD, CPUType.BCG8)},

        { Instruction.FADD,     new InstructionInfo(2,Instruction.FADD, CPUType.BCG16)},
        { Instruction.FSUB,     new InstructionInfo(2,Instruction.FSUB, CPUType.BCG16)},
        { Instruction.FMUL,     new InstructionInfo(2,Instruction.FMUL, CPUType.BCG16)},
        { Instruction.FDIV,     new InstructionInfo(2,Instruction.FDIV, CPUType.BCG16)},
        { Instruction.FAND,     new InstructionInfo(2,Instruction.FAND, CPUType.BCG16)},
        { Instruction.FOR,      new InstructionInfo(2,Instruction.FOR, CPUType.BCG16)},
        { Instruction.FNOR,     new InstructionInfo(2,Instruction.FNOR, CPUType.BCG16)},
        { Instruction.FXOR,     new InstructionInfo(2,Instruction.FXOR, CPUType.BCG16)},
        { Instruction.FNOT,     new InstructionInfo(1,Instruction.FNOT, CPUType.BCG16)},

        { Instruction.MOVS,     new InstructionInfo(2,Instruction.MOVS, CPUType.BCG8)},
        { Instruction.CMPSTR,   new InstructionInfo(2,Instruction.CMPSTR, CPUType.BCG8)},
        { Instruction.MOVF,     new InstructionInfo(2,Instruction.MOVF, CPUType.BCG16)},

        { Instruction.INB,      new InstructionInfo(2,Instruction.INB, CPUType.BCG8)},
        { Instruction.OUTB,     new InstructionInfo(2,Instruction.OUTB, CPUType.BCG8)},

        { Instruction.DATE,     new InstructionInfo(1,Instruction.DATE, CPUType.BCG8)},
        { Instruction.DELAY,    new InstructionInfo(1,Instruction.DELAY, CPUType.BCG8)},
        { Instruction.TIME,     new InstructionInfo(1,Instruction.TIME, CPUType.BCG8)},

        { Instruction.RTI,      new InstructionInfo(0,Instruction.RTI, CPUType.BCG8)},
        { Instruction.NOP,      new InstructionInfo(0,Instruction.NOP, CPUType.BCG8)},
        { Instruction.RISERR,   new InstructionInfo(1,Instruction.RISERR, CPUType.BCG8)},
        { Instruction.PUSHR,    new InstructionInfo(0,Instruction.PUSHR, CPUType.BCG8)},
        { Instruction.POPR,     new InstructionInfo(0,Instruction.POPR, CPUType.BCG8)},
        { Instruction.INT,      new InstructionInfo(1,Instruction.INT, CPUType.BCG8)},
        { Instruction.BRK,      new InstructionInfo(1,Instruction.BRK, CPUType.BCG8)},
        { Instruction.HALT,     new InstructionInfo(0,Instruction.HALT, CPUType.BCG8)},
    };
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
        if (AssemblerSettings.CPUType >= m_CPUType)
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
                default:
                    //Console.WriteLine($"There are on byte versions of {m_Instruction}");
                    return m_Instruction;
            }
        }
        throw new NotImplementedException();
    }
    public Instruction GetWordVersion()
    {
        if (AssemblerSettings.CPUType >= m_CPUType)
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
                default:
                    //Console.WriteLine($"There are no word versions of {m_Instruction}");
                    return m_Instruction;
            }
        }
        throw new NotImplementedException();
    }
    public Instruction GetTbyteVersion()
    {
        if (AssemblerSettings.CPUType >= m_CPUType)
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
                default:
                    //Console.WriteLine($"There are no tbyte versions of {m_Instruction}");
                    return m_Instruction;
            }
        }
        throw new NotImplementedException();
    }
    public Instruction GetDwordVersion()
    {
        if (AssemblerSettings.CPUType >= m_CPUType)
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
