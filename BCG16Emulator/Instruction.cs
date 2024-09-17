using System.Collections.Generic;

namespace BCG16CPUEmulator
{
    public enum Instruction : ushort
    {
        none,

    MOV =       0x0000,
    MOVRAL =    0x0001,
    MOVW =      0x0004,
    MOVWRA =    0x0005,
    MOVT =      0x0008,
    MOVD =      0x000C,
    CMP =       0x0100,
    PUSH =      0x0200,
    PUSHW =     0x0204,
    PUSHT =     0x0208,
    PUSHD =     0x020C,
    POP =       0x0300,
    POPW =      0x0304,
    POPT =      0x0308,
    POPD =      0x030C,
    CALL =      0x0400,
    RET =       0x0500,
    RETZ =      0x0600,
    SEZ =       0x0700,
    SEZRAL =    0x0701,
    SEZRA =     0x0702,
    TEST =      0x0800,
    TESTRAL =   0x0801,
    TESTRA =    0x0802,

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
    JTZ =       0x3001,
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
    
    CMPSTR =    0x4010,
    MOVF =      0x4020,
    
    RETI =      0xF000,
    NOP =       0xF010,
    PUSHR =     0xF020,
    POPR =      0xF030,
    INT =       0xF040,
    BRK =       0xF050,
    HALT =      0xF060,
    }
    public class InstructionArguments
    {
        public InstructionArguments()
        {
            instructions.Add(Instruction.MOV,       new InstructionInfo(2));
            instructions.Add(Instruction.MOVRAL,    new InstructionInfo(1));
            instructions.Add(Instruction.MOVW,      new InstructionInfo(2));
            instructions.Add(Instruction.MOVWRA,     new InstructionInfo(1));
            instructions.Add(Instruction.MOVT,      new InstructionInfo(2));
            instructions.Add(Instruction.MOVD,      new InstructionInfo(2));

            instructions.Add(Instruction.CMP,       new InstructionInfo(2));
            
            instructions.Add(Instruction.PUSH,      new InstructionInfo(1));

            instructions.Add(Instruction.POP,       new InstructionInfo(1));
            instructions.Add(Instruction.POPW,      new InstructionInfo(1));
            instructions.Add(Instruction.POPT,      new InstructionInfo(1));
            instructions.Add(Instruction.POPD,      new InstructionInfo(1));

            instructions.Add(Instruction.CALL,      new InstructionInfo(1));
            
            instructions.Add(Instruction.RET,       new InstructionInfo(1));
            instructions.Add(Instruction.RETZ,      new InstructionInfo(0));

            instructions.Add(Instruction.SEZ,       new InstructionInfo(1));
            instructions.Add(Instruction.SEZRAL,    new InstructionInfo(0));
            instructions.Add(Instruction.SEZRA,     new InstructionInfo(0));

            instructions.Add(Instruction.TEST,      new InstructionInfo(1));
            instructions.Add(Instruction.TESTRAL,   new InstructionInfo(0));
            instructions.Add(Instruction.TESTRA,    new InstructionInfo(0));

            instructions.Add(Instruction.SZE,       new InstructionInfo(0));
            instructions.Add(Instruction.SEE,       new InstructionInfo(0));
            instructions.Add(Instruction.SES,       new InstructionInfo(0));
            instructions.Add(Instruction.SEC,       new InstructionInfo(0));
            instructions.Add(Instruction.SEL,       new InstructionInfo(0));
            instructions.Add(Instruction.SEI,       new InstructionInfo(0));
            instructions.Add(Instruction.SEH,       new InstructionInfo(0));
            instructions.Add(Instruction.CZE,       new InstructionInfo(0));
            instructions.Add(Instruction.CLE,       new InstructionInfo(0));
            instructions.Add(Instruction.CLS,       new InstructionInfo(0));
            instructions.Add(Instruction.CLC,       new InstructionInfo(0));
            instructions.Add(Instruction.CLL,       new InstructionInfo(0));
            instructions.Add(Instruction.CLI,       new InstructionInfo(0));
            instructions.Add(Instruction.CLH,       new InstructionInfo(0));
            instructions.Add(Instruction.ADD,       new InstructionInfo(2));
            instructions.Add(Instruction.SUB,       new InstructionInfo(2));
            instructions.Add(Instruction.MUL,       new InstructionInfo(2));
            instructions.Add(Instruction.DIV,       new InstructionInfo(2));
            instructions.Add(Instruction.AND,       new InstructionInfo(2));
            instructions.Add(Instruction.OR,        new InstructionInfo(2));
            instructions.Add(Instruction.NOR,       new InstructionInfo(2));
            instructions.Add(Instruction.XOR,       new InstructionInfo(2));
            instructions.Add(Instruction.NOT,       new InstructionInfo(1));
            instructions.Add(Instruction.SHL,       new InstructionInfo(2));
            instructions.Add(Instruction.SHR,       new InstructionInfo(2));
            instructions.Add(Instruction.ROL,       new InstructionInfo(2));
            instructions.Add(Instruction.ROR,       new InstructionInfo(2));
            instructions.Add(Instruction.INC,       new InstructionInfo(1));
            instructions.Add(Instruction.DEC,       new InstructionInfo(1));
            instructions.Add(Instruction.NEG,       new InstructionInfo(1));
            instructions.Add(Instruction.EXP,       new InstructionInfo(2));
            instructions.Add(Instruction.SQRT,      new InstructionInfo(1));
            instructions.Add(Instruction.RNG,       new InstructionInfo(1));
            instructions.Add(Instruction.SEB,       new InstructionInfo(2));
            instructions.Add(Instruction.CLB,       new InstructionInfo(2));
            instructions.Add(Instruction.TOB,       new InstructionInfo(2));
            instructions.Add(Instruction.MOD,       new InstructionInfo(2));

            instructions.Add(Instruction.FADD,      new InstructionInfo(2));
            instructions.Add(Instruction.FSUB,      new InstructionInfo(2));
            instructions.Add(Instruction.FMUL,      new InstructionInfo(2));
            instructions.Add(Instruction.FDIV,      new InstructionInfo(2));
            instructions.Add(Instruction.FAND,      new InstructionInfo(2));
            instructions.Add(Instruction.FOR,       new InstructionInfo(2));
            instructions.Add(Instruction.FNOR,      new InstructionInfo(2));
            instructions.Add(Instruction.FXOR,      new InstructionInfo(2));
            instructions.Add(Instruction.FNOT,      new InstructionInfo(1));

            instructions.Add(Instruction.INPB,      new InstructionInfo(2));
            instructions.Add(Instruction.INPW,      new InstructionInfo(2));
            instructions.Add(Instruction.OUTB,      new InstructionInfo(2));
            instructions.Add(Instruction.OUTW,      new InstructionInfo(2));

            instructions.Add(Instruction.JMP,       new InstructionInfo(1));
            instructions.Add(Instruction.JZ,        new InstructionInfo(1));
            instructions.Add(Instruction.JNZ,       new InstructionInfo(1));
            instructions.Add(Instruction.JS,        new InstructionInfo(1));
            instructions.Add(Instruction.JNS,       new InstructionInfo(1));
            instructions.Add(Instruction.JE,        new InstructionInfo(1));
            instructions.Add(Instruction.JNE,       new InstructionInfo(1));
            instructions.Add(Instruction.JL,        new InstructionInfo(1));
            instructions.Add(Instruction.JG,        new InstructionInfo(1));
            instructions.Add(Instruction.JLE,       new InstructionInfo(1));
            instructions.Add(Instruction.JGE,       new InstructionInfo(1));
            instructions.Add(Instruction.JNV,       new InstructionInfo(1));
            instructions.Add(Instruction.JTZ,       new InstructionInfo(0));

            instructions.Add(Instruction.CMPSTR,    new InstructionInfo(2));
            instructions.Add(Instruction.MOVF,      new InstructionInfo(2));

            instructions.Add(Instruction.RETI,      new InstructionInfo(0));
            instructions.Add(Instruction.NOP,       new InstructionInfo(0));
            instructions.Add(Instruction.PUSHR,     new InstructionInfo(0));
            instructions.Add(Instruction.POPR,      new InstructionInfo(0));
            instructions.Add(Instruction.INT,       new InstructionInfo(1));
            instructions.Add(Instruction.BRK,       new InstructionInfo(0));
            instructions.Add(Instruction.HALT,      new InstructionInfo(0));
        }

        public static Dictionary<Instruction, InstructionInfo> instructions = new Dictionary<Instruction, InstructionInfo>();
    }
    public struct InstructionInfo
    {
        public int m_operandSize;

        public InstructionInfo(int operandSize)
        {
            m_operandSize = operandSize;
        }
    }
}