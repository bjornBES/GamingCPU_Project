using System;
using System.Collections.Generic;
using System.Text;

public enum Register
{
    A = 0b00_00_0000, AH = 0b00_01_0000, AL = 0b00_10_0000,
    B = 0b00_00_0001, BH = 0b00_01_0001, BL = 0b00_10_0001,
    C = 0b00_00_0010, CH = 0b00_01_0010, CL = 0b00_10_0010,
    D = 0b00_00_0011, DH = 0b00_01_0011, DL = 0b00_10_0011,

    HL = 0b00_00_0100, H = 0b00_01_0100, L = 0b00_10_0100,
    S = 0b00_00_0101, SS = 0b00_01_0101, DS = 0b00_10_0101, ES = 0b01_10_0101, CS = 0b00_11_0101,
    PC = 0b00_00_0110,

    AF = 0b00_00_0111, BF = 0b00_01_0111,
    CF = 0b00_10_0111, DF = 0b00_11_0111,

    EABF = 0b01_00_0111,
    ECDF = 0b01_01_0111,

    BPX = 0b00_00_1000, BP = 0b00_01_1000,
    SPX = 0b00_10_1000, SP = 0b00_11_1000,

    IL = 0b00_00_1001,

    R1 = 0b00_00_1010, R2 = 0b00_01_1010, R3 = 0b00_10_1010, R4 = 0b00_11_1010,

    MB = 0b00_00_1011,
    CR0 = 0b00_00_1100, CR1 = 0b00_01_1100,

    ABX = 0b00_11_0000,
    CDX = 0b00_11_0001,

    EAB = 0b01_11_0000,
    ECD = 0b01_11_0001,

    F = 0b00_00_1111, FH = 0b00_01_1111, FL = 0b00_10_1111,

    AX = 0b01_00_0000,
    BX = 0b01_00_0001,
    CX = 0b01_00_0010,
    DX = 0b01_00_0011,
    none
}

public class Registers
{
    public static Dictionary<Register, RegisterInfo> regs = new Dictionary<Register, RegisterInfo>()
    {
        { Register.A,   new RegisterInfo(Register.A, CPUType.BCG8) },
        { Register.AH,  new RegisterInfo(Register.AH, CPUType.BCG8) },
        { Register.AL,  new RegisterInfo(Register.AL, CPUType.BCG8) },
        { Register.B,   new RegisterInfo(Register.B, CPUType.BCG8) },
        { Register.BH,  new RegisterInfo(Register.BH, CPUType.BCG8) },
        { Register.BL,  new RegisterInfo(Register.BL, CPUType.BCG8) },
        { Register.C,   new RegisterInfo(Register.C, CPUType.BCG16) },
        { Register.CH,  new RegisterInfo(Register.CH, CPUType.BCG8) },
        { Register.CL,  new RegisterInfo(Register.CL, CPUType.BCG8) },
        { Register.D,   new RegisterInfo(Register.D, CPUType.BCG16) },
        { Register.DH,  new RegisterInfo(Register.DH, CPUType.BCG8) },
        { Register.DL,  new RegisterInfo(Register.DL, CPUType.BCG8) },
    
        { Register.AX,  new RegisterInfo(Register.AX, CPUType.BCG1684) },
        { Register.BX,  new RegisterInfo(Register.BX, CPUType.BCG1684) },
        { Register.CX,  new RegisterInfo(Register.CX, CPUType.BCG1684) },
        { Register.DX,  new RegisterInfo(Register.DX, CPUType.BCG1684) },
    
        { Register.HL,  new RegisterInfo(Register.HL, CPUType.BCG8)},
        { Register.H,   new RegisterInfo(Register.H, CPUType.BCG8) },
        { Register.L,   new RegisterInfo(Register.L, CPUType.BCG8) },
    
        { Register.S,   new RegisterInfo(Register.S, CPUType.BCG8) },
        { Register.SS,  new RegisterInfo(Register.SS, CPUType.BCG16) },
        { Register.DS,  new RegisterInfo(Register.DS, CPUType.BCG8) },
        { Register.CS,  new RegisterInfo(Register.CS, CPUType.BCG8) },
        { Register.ES,  new RegisterInfo(Register.ES, CPUType.BCG8) },
    
        { Register.EAB, new RegisterInfo(Register.EAB, CPUType.BCG1684) },
        { Register.ECD, new RegisterInfo(Register.ECD, CPUType.BCG1684) },
    
        { Register.PC,  new RegisterInfo(Register.PC, CPUType.BCG8) },
    
        { Register.AF,  new RegisterInfo(Register.AF, CPUType.BCG16) },
        { Register.BF,  new RegisterInfo(Register.BF, CPUType.BCG16) },
        { Register.CF,  new RegisterInfo(Register.CF, CPUType.BCG1684) },
        { Register.DF,  new RegisterInfo(Register.DF, CPUType.BCG1684) },
    
        { Register.SP,  new RegisterInfo(Register.SP, CPUType.BCG8) },
        { Register.BP,  new RegisterInfo(Register.BP, CPUType.BCG8) },
    
        { Register.SPX, new RegisterInfo(Register.SPX, CPUType.BCG1684) },
        { Register.BPX, new RegisterInfo(Register.BPX, CPUType.BCG1684) },
    
        { Register.IL,new RegisterInfo(Register.IL, CPUType.BCG8) },
    
        { Register.R1,new RegisterInfo(Register.R1, CPUType.BCG8) },
        { Register.R2,new RegisterInfo(Register.R2, CPUType.BCG8) },
        { Register.R3,new RegisterInfo(Register.R3, CPUType.BCG1680) },
        { Register.R4,new RegisterInfo(Register.R4, CPUType.BCG1684) },
    
        { Register.MB,new RegisterInfo(Register.MB, CPUType.BCG8) },
    
        { Register.CR0,new RegisterInfo(Register.CR0, CPUType.BCG8) },
        { Register.CR1,new RegisterInfo(Register.CR1, CPUType.BCG16) },
    
        { Register.F, new RegisterInfo(Register.F, CPUType.BCG8) },
        { Register.FH,new RegisterInfo(Register.FH, CPUType.BCG8) },
        { Register.FL,new RegisterInfo(Register.FL, CPUType.BCG8) },
    
        { Register.ABX, new RegisterInfo(Register.ABX, CPUType.BCG16) },
        { Register.CDX, new RegisterInfo(Register.CDX, CPUType.BCG16) },
    };
}

public class RegisterInfo
{
    public int m_size;
    public Register m_Register;
    public CPUType m_CPUType;
    public RegisterInfo(Register register, CPUType cPU)
    {
        m_size = GetSize();
        m_Register = register;
        m_CPUType = cPU;
    }

    public Register GetByteVersion()
    {
        switch (m_Register)
        {
            case Register.AH:
            case Register.AL:
            case Register.A:
                return Register.AL;
            case Register.BH:
            case Register.BL:
            case Register.B:
                return Register.BL;
            case Register.CH:
            case Register.CL:
            case Register.C:
                return Register.CL;
            case Register.DH:
            case Register.DL:
            case Register.D:
                return Register.DL;
            default:
                return m_Register;
        }
    }

    public int GetSize()
    {
        return AssemblerSettings.CPUType switch
        {
            CPUType.BCG8 => GetSizeBCG8(),
            CPUType.BCG16 => GetSizeBCG16(),
            CPUType.BCG1680 => GetSizeBCG1680(),
            CPUType.BCG1684 => GetSizeBCG1684(),
            _ => 0,
        };
    }

    int GetSizeBCG8()
    {
        return m_Register switch
        {
            Register.A => 2,
            Register.AH => 1,
            Register.AL => 1,
            Register.B => 2,
            Register.BH => 1,
            Register.BL => 1,
            Register.CH => 1,
            Register.CL => 1,
            Register.DH => 1,
            Register.DL => 1,
            Register.HL => 2,
            Register.H => 1,
            Register.L => 1,
            Register.S => 2,
            Register.DS => 2,
            Register.ES => 2,
            Register.CS => 2,
            Register.PC => 2,
            Register.BP => 1,
            Register.SP => 1,
            Register.IL => 1,
            Register.R1 => 2,
            Register.R2 => 2,
            Register.MB => 1,
            Register.CR0 => 1,
            Register.F => 2,
            Register.FH => 1,
            Register.FL => 1,
            _ => throw new NotImplementedException(),
        };
    }
    int GetSizeBCG16()
    {
        return m_Register switch
        {
            Register.A => 2,
            Register.AH => 1,
            Register.AL => 1,

            Register.B => 2,
            Register.BH => 1,
            Register.BL => 1,

            Register.C => 2,
            Register.CH => 1,
            Register.CL => 1,

            Register.D => 2,
            Register.DH => 1,
            Register.DL => 1,

            Register.ABX => 4,
            Register.CDX => 4,

            Register.HL => 4,
            Register.H => 2,
            Register.L => 2,

            Register.S => 2,
            Register.DS => 2,
            Register.ES => 2,
            Register.CS => 2,
            Register.SS => 2,

            Register.PC => 4,

            Register.AF => 4,
            Register.BF => 4,

            Register.BP => 2,
            Register.SP => 2,

            Register.IL => 1,

            Register.R1 => 2,
            Register.R2 => 2,

            Register.MB => 1,

            Register.CR0 => 1,
            Register.CR1 => 1,

            Register.F => 2,
            Register.FH => 1,
            Register.FL => 1,
            _ => throw new NotImplementedException(),
        };
    }
    int GetSizeBCG1680()
    {
        return m_Register switch
        {
            Register.A => 2,
            Register.AH => 1,
            Register.AL => 1,

            Register.B => 2,
            Register.BH => 1,
            Register.BL => 1,

            Register.C => 2,
            Register.CH => 1,
            Register.CL => 1,

            Register.D => 2,
            Register.DH => 1,
            Register.DL => 1,

            Register.ABX => 4,
            Register.CDX => 4,

            Register.HL => 4,
            Register.H => 2,
            Register.L => 2,

            Register.S => 2,
            Register.DS => 2,
            Register.ES => 2,
            Register.CS => 2,
            Register.SS => 2,

            Register.PC => 4,

            Register.AF => 4,
            Register.BF => 4,

            Register.BP => 2,
            Register.SP => 2,

            Register.IL => 1,

            Register.R1 => 2,
            Register.R2 => 2,
            Register.R3 => 2,

            Register.MB => 1,

            Register.CR0 => 2,
            Register.CR1 => 2,

            Register.F => 2,
            Register.FH => 1,
            Register.FL => 1,
            _ => throw new NotImplementedException(),
        };
    }
    int GetSizeBCG1684()
    {
        return m_Register switch
        {
            Register.A => 2,
            Register.AH => 1,
            Register.AL => 1,

            Register.B => 2,
            Register.BH => 1,
            Register.BL => 1,

            Register.C => 2,
            Register.CH => 1,
            Register.CL => 1,

            Register.D => 2,
            Register.DH => 1,
            Register.DL => 1,

            Register.AX => 4,
            Register.BX => 4,
            Register.CX => 4,
            Register.DX => 4,

            Register.ABX => 4,
            Register.CDX => 4,

            Register.EAB => 8,
            Register.ECD => 8,

            Register.HL => 4,
            Register.H => 2,
            Register.L => 2,

            Register.S => 2,
            Register.DS => 2,
            Register.ES => 2,
            Register.CS => 2,
            Register.SS => 2,

            Register.PC => 4,

            Register.AF => 4,
            Register.BF => 4,
            Register.CF => 4,
            Register.DF => 4,

            Register.EABF => 8,
            Register.ECDF => 8,

            Register.BP => 2,
            Register.SP => 2,

            Register.BPX => 4,
            Register.SPX => 4,

            Register.IL => 2,

            Register.R1 => 2,
            Register.R2 => 2,
            Register.R3 => 2,
            Register.R4 => 2,

            Register.MB => 1,

            Register.CR0 => 2,
            Register.CR1 => 2,

            Register.F => 2,
            Register.FH => 1,
            Register.FL => 1,
            _ => throw new NotImplementedException(),
        };
    }

    public static implicit operator string(RegisterInfo info)
    {
        return Convert.ToString((byte)info.m_Register, 16);
    }
}
