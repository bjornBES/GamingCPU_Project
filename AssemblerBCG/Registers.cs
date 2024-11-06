using System;
using System.Collections.Generic;
using System.Text;

public enum Register
{
    AL  = 0b00_000000,
    AH  = 0b01_000000,
    A   = 0b10_000000,
    AX  = 0b11_000000,

    BL  = 0b00_000001,
    BH  = 0b01_000001,
    B   = 0b10_000001,
    BX  = 0b11_000001,

    CL  = 0b00_000010,
    CH  = 0b01_000010,
    C   = 0b10_000010,
    CX  = 0b11_000010,

    DL  = 0b00_000011,
    DH  = 0b01_000011,
    D   = 0b10_000011,
    DX  = 0b11_000011,

    CS  = 0b00_000100,
    DS  = 0b00_000101,
    ES  = 0b01_000101,
    FS  = 0b10_000101,
    GS  = 0b11_000101,
    HS  = 0b00_000110,
    SS  = 0b11_000110,

    PC  = 0b00_000111,

    L   = 0b01_000111,
    H   = 0b10_000111,
    HL  = 0b11_000111,

    BP  = 0b00_001000,
    BPX = 0b01_001000,
    SP  = 0b00_001001,
    SPX = 0b01_001001,

    AF  = 0b00_001010,
    BF  = 0b00_001011,
    CF  = 0b00_001100,
    DF  = 0b00_001101,

    AD  = 0b01_001010,
    BD  = 0b01_001011,
    CD  = 0b01_001100,
    DD  = 0b01_001101,

    R1  = 0b00_010000,
    R2  = 0b01_010000,
    R3  = 0b10_010000,
    R4  = 0b11_010000,
    R5  = 0b00_010001,
    R6  = 0b01_010001,
    R7  = 0b10_010001,
    R8  = 0b11_010001,
    R9  = 0b00_010010,
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
    R10L =0b01_010110,
    R11L =0b10_010110,
    R12L =0b11_010110,
    R13L =0b00_010111,
    R14L =0b01_010111,
    R15L =0b10_010111,
    R16L =0b11_010111,

    R1H = 0b00_011000,
    R2H = 0b01_011000,
    R3H = 0b10_011000,
    R4H = 0b11_011000,
    R5H = 0b00_011001,
    R6H = 0b01_011001,
    R7H = 0b10_011001,
    R8H = 0b11_011001,
    R9H = 0b00_011010,
    R10H =0b01_011010,
    R11H =0b10_011010,
    R12H =0b11_011010,
    R13H =0b00_011011,
    R14H =0b01_011011,
    R15H =0b10_011011,
    R16H =0b11_011011,

    X   = 0b00_011100,
    Y   = 0b01_011100,

    EL  = 0b00_100000,
    EH  = 0b01_100000,
    E   = 0b10_100000,
    EX  = 0b11_100000,

    GL  = 0b00_100001,
    GH  = 0b01_100001,
    G   = 0b10_100001,
    GX  = 0b11_100001,
    
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

    EA  = 0b00_101011,
    EB  = 0b01_101011,
    EC  = 0b10_101011,
    ED  = 0b11_101011,
    EE  = 0b00_101100,
    EG  = 0b01_101100,

    F   = 0b00_111111,

    none
}
public enum GPRegisterByte
{
    AL, BL, CL, DL, EL, GL,
    AH, BH, CH, DH, EH, GH,
}
public enum GPRegisterWord
{
    A, B, C, D, E, G, H, L, SP, BP,
}
public enum GPRegisterDword
{
    AX, BX, CX, DX, EX, GX, HL, SPX, BPX,
}
public enum SRegisterWord
{
    CS, DS, ES, FS, GS, HS, SS
}
public enum SRegisterDword
{
    ECS, EDS, EES, EFS, EGS, EHS, ESS
}
public class Registers
{
    public static Dictionary<Register, RegisterInfo> m_Regs = new Dictionary<Register, RegisterInfo>()
    {
        { Register.AL,  new RegisterInfo(Register.AL,   CPUType.BC8) },
        { Register.AH,  new RegisterInfo(Register.AH,   CPUType.BC8) },
        { Register.A,   new RegisterInfo(Register.A,    CPUType.BC16) },
        { Register.AX,  new RegisterInfo(Register.AX,   CPUType.BC16) },
        { Register.EA,  new RegisterInfo(Register.EA,   CPUType.BC32) },

        { Register.BL,  new RegisterInfo(Register.BL,   CPUType.BC8) },
        { Register.BH,  new RegisterInfo(Register.BH,   CPUType.BC8) },
        { Register.B,   new RegisterInfo(Register.B,    CPUType.BC16) },
        { Register.BX,  new RegisterInfo(Register.BX,   CPUType.BC16) },
        { Register.EB,  new RegisterInfo(Register.EB,   CPUType.BC32) },

        { Register.CL,  new RegisterInfo(Register.CL,   CPUType.BC8) },
        { Register.CH,  new RegisterInfo(Register.CH,   CPUType.BC8) },
        { Register.C,   new RegisterInfo(Register.C,    CPUType.BC16) },
        { Register.CX,  new RegisterInfo(Register.CX,   CPUType.BC16) },
        { Register.EC,  new RegisterInfo(Register.EC,   CPUType.BC32) },
        
        { Register.DL,  new RegisterInfo(Register.DL,   CPUType.BC8) },
        { Register.DH,  new RegisterInfo(Register.DH,   CPUType.BC8) },
        { Register.D,   new RegisterInfo(Register.D,    CPUType.BC16) },
        { Register.DX,  new RegisterInfo(Register.DX,   CPUType.BC16) },
        { Register.ED,  new RegisterInfo(Register.ED,   CPUType.BC32) },

        { Register.EL,  new RegisterInfo(Register.EL,   CPUType.BC32) },
        { Register.EH,  new RegisterInfo(Register.EH,   CPUType.BC32) },
        { Register.E,   new RegisterInfo(Register.E,    CPUType.BC32) },
        { Register.EX,  new RegisterInfo(Register.EX,   CPUType.BC32) },
        { Register.EE,  new RegisterInfo(Register.EE,   CPUType.BC32) },

        { Register.GL,  new RegisterInfo(Register.GL,   CPUType.BC32) },
        { Register.GH,  new RegisterInfo(Register.GH,   CPUType.BC32) },
        { Register.G,   new RegisterInfo(Register.G,    CPUType.BC32) },
        { Register.GX,  new RegisterInfo(Register.GX,   CPUType.BC32) },
        { Register.EG,  new RegisterInfo(Register.EG,   CPUType.BC32) },

        { Register.CS,  new RegisterInfo(Register.CS,   CPUType.BC8) },
        { Register.DS,  new RegisterInfo(Register.DS,   CPUType.BC8) },
        { Register.ES,  new RegisterInfo(Register.ES,   CPUType.BC16) },
        { Register.FS,  new RegisterInfo(Register.FS,   CPUType.BC16) },
        { Register.GS,  new RegisterInfo(Register.GS,   CPUType.BC16) },
        { Register.HS,  new RegisterInfo(Register.HS,   CPUType.BC16) },
        { Register.SS,  new RegisterInfo(Register.SS,   CPUType.BC8) },

        { Register.ECS, new RegisterInfo(Register.ECS,  CPUType.BC16) },
        { Register.EDS, new RegisterInfo(Register.EDS,  CPUType.BC16) },
        { Register.EES, new RegisterInfo(Register.EES,  CPUType.BC16) },
        { Register.EFS, new RegisterInfo(Register.EFS,  CPUType.BC16) },
        { Register.EGS, new RegisterInfo(Register.EGS,  CPUType.BC16) },
        { Register.EHS, new RegisterInfo(Register.EHS,  CPUType.BC16) },
        { Register.ESS, new RegisterInfo(Register.ESS,  CPUType.BC16) },

        { Register.PC,  new RegisterInfo(Register.PC,   CPUType.BC8) },
    
        { Register.HL,  new RegisterInfo(Register.HL,   CPUType.BC8) },
        { Register.H,   new RegisterInfo(Register.H,    CPUType.BC8) },
        { Register.L,   new RegisterInfo(Register.L,    CPUType.BC8) },

        { Register.PTA, new RegisterInfo(Register.PTA,  CPUType.BC16) },

        { Register.GDA, new RegisterInfo(Register.GDA,  CPUType.BC16) },
        
        { Register.BP,  new RegisterInfo(Register.BP,   CPUType.BC8) },
        { Register.BPX, new RegisterInfo(Register.BPX,  CPUType.BC32) },

        { Register.SP,  new RegisterInfo(Register.SP,   CPUType.BC8) },
        { Register.SPX, new RegisterInfo(Register.SPX,  CPUType.BC32) },
        
        { Register.AF,  new RegisterInfo(Register.AF,   CPUType.BC16) },
        { Register.BF,  new RegisterInfo(Register.BF,   CPUType.BC16) },
        { Register.CF,  new RegisterInfo(Register.CF,   CPUType.BC16) },
        { Register.DF,  new RegisterInfo(Register.DF,   CPUType.BC16) },

        { Register.AD,  new RegisterInfo(Register.AD,   CPUType.BC32) },
        { Register.BD,  new RegisterInfo(Register.BD,   CPUType.BC32) },
        { Register.CD,  new RegisterInfo(Register.CD,   CPUType.BC32) },
        { Register.DD,  new RegisterInfo(Register.DD,   CPUType.BC32) },

        { Register.R1,  new RegisterInfo(Register.R1,   CPUType.BC8) },
        { Register.R2,  new RegisterInfo(Register.R2,   CPUType.BC8) },
        { Register.R3,  new RegisterInfo(Register.R3,   CPUType.BC8) },
        { Register.R4,  new RegisterInfo(Register.R4,   CPUType.BC8) },
        { Register.R5,  new RegisterInfo(Register.R5,   CPUType.BC16) },
        { Register.R6,  new RegisterInfo(Register.R6,   CPUType.BC16) },
        { Register.R7,  new RegisterInfo(Register.R7,   CPUType.BC16) },
        { Register.R8,  new RegisterInfo(Register.R8,   CPUType.BC16) },
        { Register.R9,  new RegisterInfo(Register.R9,   CPUType.BC16) },
        { Register.R10, new RegisterInfo(Register.R10,  CPUType.BC16) },
        { Register.R11, new RegisterInfo(Register.R11,  CPUType.BC16) },
        { Register.R12, new RegisterInfo(Register.R12,  CPUType.BC16) },
        { Register.R13, new RegisterInfo(Register.R13,  CPUType.BC16) },
        { Register.R14, new RegisterInfo(Register.R14,  CPUType.BC16) },
        { Register.R15, new RegisterInfo(Register.R15,  CPUType.BC16) },
        { Register.R16, new RegisterInfo(Register.R16,  CPUType.BC16) },

        { Register.R1L, new RegisterInfo(Register.R1L,  CPUType.BC8) },
        { Register.R2L, new RegisterInfo(Register.R2L,  CPUType.BC8) },
        { Register.R3L, new RegisterInfo(Register.R3L,  CPUType.BC8) },
        { Register.R4L, new RegisterInfo(Register.R4L,  CPUType.BC8) },
        { Register.R5L, new RegisterInfo(Register.R5L,  CPUType.BC16) },
        { Register.R6L, new RegisterInfo(Register.R6L,  CPUType.BC16) },
        { Register.R7L, new RegisterInfo(Register.R7L,  CPUType.BC16) },
        { Register.R8L, new RegisterInfo(Register.R8L,  CPUType.BC16) },
        { Register.R9L, new RegisterInfo(Register.R9L,  CPUType.BC16) },
        { Register.R10L,new RegisterInfo(Register.R10L, CPUType.BC16) },
        { Register.R11L,new RegisterInfo(Register.R11L, CPUType.BC16) },
        { Register.R12L,new RegisterInfo(Register.R12L, CPUType.BC16) },
        { Register.R13L,new RegisterInfo(Register.R13L, CPUType.BC16) },
        { Register.R14L,new RegisterInfo(Register.R14L, CPUType.BC16) },
        { Register.R15L,new RegisterInfo(Register.R15L, CPUType.BC16) },
        { Register.R16L,new RegisterInfo(Register.R16L, CPUType.BC16) },

        { Register.R1H, new RegisterInfo(Register.R1H,  CPUType.BC8) },
        { Register.R2H, new RegisterInfo(Register.R2H,  CPUType.BC8) },
        { Register.R3H, new RegisterInfo(Register.R3H,  CPUType.BC8) },
        { Register.R4H, new RegisterInfo(Register.R4H,  CPUType.BC8) },
        { Register.R5H, new RegisterInfo(Register.R5H,  CPUType.BC16) },
        { Register.R6H, new RegisterInfo(Register.R6H,  CPUType.BC16) },
        { Register.R7H, new RegisterInfo(Register.R7H,  CPUType.BC16) },
        { Register.R8H, new RegisterInfo(Register.R8H,  CPUType.BC16) },
        { Register.R9H, new RegisterInfo(Register.R9H,  CPUType.BC16) },
        { Register.R10H,new RegisterInfo(Register.R10H, CPUType.BC16) },
        { Register.R11H,new RegisterInfo(Register.R11H, CPUType.BC16) },
        { Register.R12H,new RegisterInfo(Register.R12H, CPUType.BC16) },
        { Register.R13H,new RegisterInfo(Register.R13H, CPUType.BC16) },
        { Register.R14H,new RegisterInfo(Register.R14H, CPUType.BC16) },
        { Register.R15H,new RegisterInfo(Register.R15H, CPUType.BC16) },
        { Register.R16H,new RegisterInfo(Register.R16H, CPUType.BC16) },

        { Register.X,   new RegisterInfo(Register.X,    CPUType.BC16) },
        { Register.Y,   new RegisterInfo(Register.Y,    CPUType.BC16) },

        { Register.CR0, new RegisterInfo(Register.CR0,  CPUType.BC16) },
    
        { Register.F,   new RegisterInfo(Register.F,    CPUType.BC8) },
    };
}

public class RegisterInfo
{
    public int m_Size;
    public Register m_Register;
    public CPUType m_CPUType;
    public RegisterInfo(Register register, CPUType cPU)
    {
        m_Size = GetSize();
        m_Register = register;
        m_CPUType = cPU;
    }

    public int GetSize()
    {
        switch (AssemblerSettings.m_CPUType)
        {
            case CPUType.BC8:
                return getSizeBC8();
            case CPUType.BC16:
            case CPUType.BC16C:
            case CPUType.BC16CE:
            case CPUType.BC1602C:
            case CPUType.BC16F:
            case CPUType.BC16CF:
            case CPUType.BC16CEF:
            case CPUType.BC1602CF:
                return getSizeBC16();
            case CPUType.BC32:
            case CPUType.BC3203:
            case CPUType.BC32F:
            case CPUType.BC3203F:
            case CPUType.BC3203FD:
                return getSizeBC32();
            default:
                break;
        }
        return 0;
    }

    int getSizeBC8()
    {
        return m_Register switch
        {
            Register.AH     => 1,
            Register.AL     => 1,

            Register.BH     => 1,
            Register.BL     => 1,
            
            Register.CH     => 1,
            Register.CL     => 1,
            
            Register.DH     => 1,
            Register.DL     => 1,

            Register.CS     => 2,
            Register.DS     => 2,
            Register.SS     => 2,

            Register.PC     => 2,
            Register.HL     => 4,
            Register.H      => 2,
            Register.L      => 2,
            
            Register.BP     => 2,
            Register.SP     => 2,
            
            Register.R1     => 1,
            Register.R2     => 1,
            Register.R3     => 1,
            Register.R4     => 1,

            Register.F      => 1,
            _ => -1,
        };
    }
    int getSizeBC16()
    {
        return m_Register switch
        {
            Register.AH     => 1,
            Register.AL     => 1,
            Register.A      => 2,

            Register.BH     => 1,
            Register.BL     => 1,
            Register.B      => 2,

            Register.CH     => 1,
            Register.CL     => 1,
            Register.C      => 2,

            Register.DH     => 1,
            Register.DL     => 1,
            Register.D      => 2,

            Register.CS     => 2,
            Register.DS     => 2,
            Register.ES     => 2,
            Register.FS     => 2,
            Register.GS     => 2,
            Register.HS     => 2,
            Register.SS     => 2,

            Register.PC     => 3,
            Register.HL     => 4,
            Register.H      => 2,
            Register.L      => 2,

            Register.PTA    => 2,

            Register.GDA    => 4,

            Register.BP     => 2,
            Register.SP     => 2,

            Register.AF     => 4,
            Register.BF     => 4,
            Register.CF     => 4,
            Register.DF     => 4,

            Register.R1     => 2,
            Register.R2     => 2,
            Register.R3     => 2,
            Register.R4     => 2,
            Register.R5     => 2,
            Register.R6     => 2,
            Register.R7     => 2,
            Register.R8     => 2,
            Register.R9     => 2,
            Register.R10    => 2,
            Register.R11    => 2,
            Register.R12    => 2,
            Register.R13    => 2,
            Register.R14    => 2,
            Register.R15    => 2,
            Register.R16    => 2,

            Register.R1L    => 1,
            Register.R2L    => 1,
            Register.R3L    => 1,
            Register.R4L    => 1,
            Register.R5L    => 1,
            Register.R6L    => 1,
            Register.R7L    => 1,
            Register.R8L    => 1,
            Register.R9L    => 1,
            Register.R10L   => 1,
            Register.R11L   => 1,
            Register.R12L   => 1,
            Register.R13L   => 1,
            Register.R14L   => 1,
            Register.R15L   => 1,
            Register.R16L   => 1,
            Register.R1H    => 1,
            Register.R2H    => 1,
            Register.R3H    => 1,
            Register.R4H    => 1,
            Register.R5H    => 1,
            Register.R6H    => 1,
            Register.R7H    => 1,
            Register.R8H    => 1,
            Register.R9H    => 1,
            Register.R10H   => 1,
            Register.R11H   => 1,
            Register.R12H   => 1,
            Register.R13H   => 1,
            Register.R14H   => 1,
            Register.R15H   => 1,
            Register.R16H   => 1,

            Register.X      => 2,
            Register.Y      => 2,

            Register.CR0    => 1,

            Register.F      => 2,
            _ => -1,
        };
    }
    int getSizeBC16PM()
    {
        return m_Register switch
        {
            Register.AX => 4,

            Register.BX => 4,

            Register.CX => 4,

            Register.DX => 4,

            Register.ECS => 4,
            Register.EDS => 4,
            Register.EES => 4,
            Register.EFS => 4,
            Register.EGS => 4,
            Register.EHS => 4,
            Register.ESS => 4,

            Register.PC => 4,

            Register.R1 => 4,
            Register.R2 => 4,
            Register.R3 => 4,
            Register.R4 => 4,
            Register.R5 => 4,
            Register.R6 => 4,
            Register.R7 => 4,
            Register.R8 => 4,
            Register.R9 => 4,
            Register.R10 => 4,
            Register.R11 => 4,
            Register.R12 => 4,
            Register.R13 => 4,
            Register.R14 => 4,
            Register.R15 => 4,
            Register.R16 => 4,

            Register.R1L => 2,
            Register.R2L => 2,
            Register.R3L => 2,
            Register.R4L => 2,
            Register.R5L => 2,
            Register.R6L => 2,
            Register.R7L => 2,
            Register.R8L => 2,
            Register.R9L => 2,
            Register.R10L => 2,
            Register.R11L => 2,
            Register.R12L => 2,
            Register.R13L => 2,
            Register.R14L => 2,
            Register.R15L => 2,
            Register.R16L => 2,
            Register.R1H => 2,
            Register.R2H => 2,
            Register.R3H => 2,
            Register.R4H => 2,
            Register.R5H => 2,
            Register.R6H => 2,
            Register.R7H => 2,
            Register.R8H => 2,
            Register.R9H => 2,
            Register.R10H => 2,
            Register.R11H => 2,
            Register.R12H => 2,
            Register.R13H => 2,
            Register.R14H => 2,
            Register.R15H => 2,
            Register.R16H => 2,

            Register.X => 4,
            Register.Y => 4,

            Register.CR0 => 2,

            Register.F => 3,
            _ => getSizeBC16(),
        };
    }
    int getSizeBC32()
    {
        return m_Register switch
        {
            Register.AH => 1,
            Register.AL => 1,
            Register.A => 2,
            Register.AX => 4,

            Register.BH => 1,
            Register.BL => 1,
            Register.B => 2,
            Register.BX => 4,

            Register.CH => 1,
            Register.CL => 1,
            Register.C => 2,
            Register.CX => 4,

            Register.DH => 1,
            Register.DL => 1,
            Register.D => 2,
            Register.DX => 4,

            Register.EH => 1,
            Register.EL => 1,
            Register.E => 2,
            Register.EX => 4,

            Register.GH => 1,
            Register.GL => 1,
            Register.G => 2,
            Register.GX => 4,

            Register.CS => 2,
            Register.DS => 2,
            Register.ES => 2,
            Register.FS => 2,
            Register.GS => 2,
            Register.HS => 2,
            Register.SS => 2,

            Register.ECS => 2,
            Register.EDS => 2,
            Register.EES => 2,
            Register.EFS => 2,
            Register.EGS => 2,
            Register.EHS => 2,
            Register.ESS => 2,
            
            Register.PC => 4,
            Register.HL => 4,
            Register.L => 2,
            Register.H => 2,
            
            Register.BP => 2,
            Register.BPX => 4,
            
            Register.SP => 2,
            Register.SPX => 4,
            
            Register.AF => 4,
            Register.BF => 4,
            Register.CF => 4,
            Register.DF => 4,
            
            Register.AD => 8,
            Register.BD => 8,
            Register.CD => 8,
            Register.DD => 8,

            Register.R1 => 4,
            Register.R2 => 4,
            Register.R3 => 4,
            Register.R4 => 4,
            Register.R5 => 4,
            Register.R6 => 4,
            Register.R7 => 4,
            Register.R8 => 4,
            Register.R9 => 4,
            Register.R10 => 4,
            Register.R11 => 4,
            Register.R12 => 4,
            Register.R13 => 4,
            Register.R14 => 4,
            Register.R15 => 4,
            Register.R16 => 4,

            Register.R1L => 2,
            Register.R2L => 2,
            Register.R3L => 2,
            Register.R4L => 2,
            Register.R5L => 2,
            Register.R6L => 2,
            Register.R7L => 2,
            Register.R8L => 2,
            Register.R9L => 2,
            Register.R10L => 2,
            Register.R11L => 2,
            Register.R12L => 2,
            Register.R13L => 2,
            Register.R14L => 2,
            Register.R15L => 2,
            Register.R16L => 2,
            Register.R1H => 2,
            Register.R2H => 2,
            Register.R3H => 2,
            Register.R4H => 2,
            Register.R5H => 2,
            Register.R6H => 2,
            Register.R7H => 2,
            Register.R8H => 2,
            Register.R9H => 2,
            Register.R10H => 2,
            Register.R11H => 2,
            Register.R12H => 2,
            Register.R13H => 2,
            Register.R14H => 2,
            Register.R15H => 2,
            Register.R16H => 2,

            Register.X => 4,
            Register.Y => 4,
            
            Register.CR0 => 3,
            
            Register.F => 3,

            _ => -1,
        };
    }
    int getSizeBC32PM()
    {
        return m_Register switch
        {
            Register.EA => 8,
            Register.EB => 8,
            Register.EC => 8,
            Register.ED => 8,
            Register.EE => 8,
            Register.EG => 8,

            Register.PC => 4,
            Register.HL => 8,
            Register.L => 2,
            Register.H => 2,

            Register.BP => 2,
            Register.BPX => 4,

            Register.SP => 2,
            Register.SPX => 4,

            Register.PTA => 4,
            Register.GDA => 6,

            Register.AF => 4,
            Register.BF => 4,
            Register.CF => 4,
            Register.DF => 4,

            Register.AD => 8,
            Register.BD => 8,
            Register.CD => 8,
            Register.DD => 8,

            Register.R1     => 8,
            Register.R2     => 8,
            Register.R3     => 8,
            Register.R4     => 8,
            Register.R5     => 8,
            Register.R6     => 8,
            Register.R7     => 8,
            Register.R8     => 8,
            Register.R9     => 8,
            Register.R10    => 8,
            Register.R11    => 8,
            Register.R12    => 8,
            Register.R13    => 8,
            Register.R14    => 8,
            Register.R15    => 8,
            Register.R16    => 8,

            Register.R1L    => 4,
            Register.R2L    => 4,
            Register.R3L    => 4,
            Register.R4L    => 4,
            Register.R5L    => 4,
            Register.R6L    => 4,
            Register.R7L    => 4,
            Register.R8L    => 4,
            Register.R9L    => 4,
            Register.R10L   => 4,
            Register.R11L   => 4,
            Register.R12L   => 4,
            Register.R13L   => 4,
            Register.R14L   => 4,
            Register.R15L   => 4,
            Register.R16L   => 4,
            Register.R1H    => 4,
            Register.R2H    => 4,
            Register.R3H    => 4,
            Register.R4H    => 4,
            Register.R5H    => 4,
            Register.R6H    => 4,
            Register.R7H    => 4,
            Register.R8H    => 4,
            Register.R9H    => 4,
            Register.R10H   => 4,
            Register.R11H   => 4,
            Register.R12H   => 4,
            Register.R13H   => 4,
            Register.R14H   => 4,
            Register.R15H   => 4,
            Register.R16H   => 4,

            Register.CR0 => 3,

            Register.F => 3,

            _ => getSizeBC32(),
        };
    }

    public static implicit operator string(RegisterInfo info)
    {
        return Convert.ToString((byte)info.m_Register, 16);
    }
}
