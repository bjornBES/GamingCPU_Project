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
    SS  = 0b00_000110,

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

    X   = 0b00_010010,
    Y   = 0b01_010010,

    EX  = 0b00_010011,
    FX  = 0b00_010100,
    GX  = 0b00_010101,
    HX  = 0b00_010110,

    MB  = 0b00_100000,

    CR0 = 0b00_100001,
    CR1 = 0b00_100010,

    F   = 0b00_111111,

    none
}

public class Registers
{
    public static Dictionary<Register, RegisterInfo> m_Regs = new Dictionary<Register, RegisterInfo>()
    {
        { Register.AL,  new RegisterInfo(Register.AL,   CPUType.BC8) },
        { Register.AH,  new RegisterInfo(Register.AH,   CPUType.BC8) },
        { Register.A,   new RegisterInfo(Register.A,    CPUType.BC8) },
        { Register.AX,  new RegisterInfo(Register.AX,   CPUType.BC16) },

        { Register.BL,  new RegisterInfo(Register.BL,   CPUType.BC8) },
        { Register.BH,  new RegisterInfo(Register.BH,   CPUType.BC8) },
        { Register.B,   new RegisterInfo(Register.B,    CPUType.BC8) },
        { Register.BX,  new RegisterInfo(Register.BX,   CPUType.BC16) },

        { Register.CL,  new RegisterInfo(Register.CL,   CPUType.BC8) },
        { Register.CH,  new RegisterInfo(Register.CH,   CPUType.BC8) },
        { Register.C,   new RegisterInfo(Register.C,    CPUType.BC8) },
        { Register.CX,  new RegisterInfo(Register.CX,   CPUType.BC16) },
        
        { Register.DL,  new RegisterInfo(Register.DL,   CPUType.BC8) },
        { Register.DH,  new RegisterInfo(Register.DH,   CPUType.BC8) },
        { Register.D,   new RegisterInfo(Register.D,    CPUType.BC8) },
        { Register.DX,  new RegisterInfo(Register.DX,   CPUType.BC16) },
    
        { Register.CS,  new RegisterInfo(Register.CS,   CPUType.BC8) },
        { Register.DS,  new RegisterInfo(Register.DS,   CPUType.BC8) },
        { Register.ES,  new RegisterInfo(Register.ES,   CPUType.BC16) },
        { Register.FS,  new RegisterInfo(Register.FS,   CPUType.BC16) },
        { Register.GS,  new RegisterInfo(Register.GS,   CPUType.BC32) },
        { Register.SS,  new RegisterInfo(Register.SS,   CPUType.BC8) },
    
        { Register.PC,  new RegisterInfo(Register.PC,   CPUType.BC8) },
    
        { Register.HL,  new RegisterInfo(Register.HL,   CPUType.BC8) },
        { Register.H,   new RegisterInfo(Register.H,    CPUType.BC8) },
        { Register.L,   new RegisterInfo(Register.L,    CPUType.BC8) },
        
        { Register.BP,  new RegisterInfo(Register.BP,   CPUType.BC8) },
        { Register.BPX, new RegisterInfo(Register.BPX,  CPUType.BC32) },

        { Register.SP,  new RegisterInfo(Register.SP,   CPUType.BC8) },
        { Register.SPX, new RegisterInfo(Register.SPX,  CPUType.BC32) },
        
        { Register.AF,  new RegisterInfo(Register.AF,   CPUType.BC16) },
        { Register.BF,  new RegisterInfo(Register.BF,   CPUType.BC16) },
        { Register.CF,  new RegisterInfo(Register.CF,   CPUType.BC32) },
        { Register.DF,  new RegisterInfo(Register.DF,   CPUType.BC32) },
    
    
        { Register.R1,  new RegisterInfo(Register.R1,   CPUType.BC8) },
        { Register.R2,  new RegisterInfo(Register.R2,   CPUType.BC8) },
        { Register.R3,  new RegisterInfo(Register.R3,   CPUType.BC8) },
        { Register.R4,  new RegisterInfo(Register.R4,   CPUType.BC8) },
        { Register.R5,  new RegisterInfo(Register.R5,   CPUType.BC16) },
        { Register.R6,  new RegisterInfo(Register.R6,   CPUType.BC16) },
        { Register.R7,  new RegisterInfo(Register.R7,   CPUType.BC32) },
        { Register.R8,  new RegisterInfo(Register.R8,   CPUType.BC32) },

        { Register.X,   new RegisterInfo(Register.X,    CPUType.BC16) },
        { Register.Y,   new RegisterInfo(Register.Y,    CPUType.BC16) },

        { Register.EX,  new RegisterInfo(Register.EX,   CPUType.BC32) },
        { Register.FX,  new RegisterInfo(Register.FX,   CPUType.BC32) },
        { Register.GX,  new RegisterInfo(Register.GX,   CPUType.BC32) },
        { Register.HX,  new RegisterInfo(Register.HX,   CPUType.BC32) },

        { Register.MB,  new RegisterInfo(Register.MB,   CPUType.BC8) },
    
        { Register.CR0, new RegisterInfo(Register.CR0,  CPUType.BC16) },
        { Register.CR1, new RegisterInfo(Register.CR1,  CPUType.BC16) },
    
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

    public Register GetByteVersion()
    {
        switch (m_Register)
        {
            case Register.AH:
                return Register.AH;
            case Register.AL:
            case Register.A:
                return Register.AL;
            case Register.BH:
                return Register.BH;
            case Register.BL:
            case Register.B:
                return Register.BL;
            case Register.CH:
                return Register.CH;
            case Register.CL:
            case Register.C:
                return Register.CL;
            case Register.DH:
                return Register.DH;
            case Register.DL:
            case Register.D:
                return Register.DL;
            default:
                return m_Register;
        }
    }

    public int GetSize()
    {
        switch (AssemblerSettings.m_CPUType)
        {
            case CPUType.BC8:
            case CPUType.BC816:
            case CPUType.BC810680:
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
            case CPUType.BC24:
            case CPUType.BC24F:
                return getSizeBC24();
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
            Register.SS     => 2,

            Register.PC     => 2,
            Register.HL     => 4,
            Register.H      => 2,
            Register.L      => 2,
            
            Register.BP     => 1,
            Register.SP     => 1,
            
            Register.R1     => 2,
            Register.R2     => 2,
            Register.R3     => 2,
            Register.R4     => 2,
            
            Register.MB     => 1,
            
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
            Register.AX     => 4,

            Register.BH     => 1,
            Register.BL     => 1,
            Register.B      => 2,
            Register.BX     => 4,

            Register.CH     => 1,
            Register.CL     => 1,
            Register.C      => 2,
            Register.CX     => 4,

            Register.DH     => 1,
            Register.DL     => 1,
            Register.D      => 2,
            Register.DX     => 4,

            Register.CS     => 2,
            Register.DS     => 2,
            Register.ES     => 2,
            Register.FS     => 2,
            Register.SS     => 2,

            Register.PC     => 3,
            Register.HL     => 4,
            Register.H      => 2,
            Register.L      => 2,

            Register.BP     => 2,
            Register.SP     => 2,

            Register.AF     => 4,
            Register.BF     => 4,

            Register.R1     => 2,
            Register.R2     => 2,
            Register.R3     => 2,
            Register.R4     => 2,
            Register.R5     => 2,
            Register.R6     => 2,

            Register.X      => 2,
            Register.Y      => 2,

            Register.MB     => 2,

            Register.CR0    => 1,
            Register.CR1    => 1,

            Register.F      => 2,
            _ => -1,
        };
    }
    int getSizeBC24()
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

            Register.CS => 2,
            Register.DS => 2,
            Register.ES => 2,
            Register.FS => 2,
            Register.SS => 2,

            Register.PC => 2,
            Register.HL => 4,
            Register.H => 2,
            Register.L => 2,

            Register.BP => 2,
            Register.SP => 2,

            Register.AF => 4,
            Register.BF => 4,

            Register.R1 => 2,
            Register.R2 => 2,
            Register.R3 => 2,
            Register.R4 => 2,
            Register.R5 => 2,
            Register.R6 => 2,

            Register.X => 2,
            Register.Y => 2,

            Register.MB => 1,

            Register.CR0 => 1,
            Register.CR1 => 1,

            Register.F => 3,
            _ => -1,
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

            Register.CS => 2,
            Register.DS => 2,
            Register.ES => 2,
            Register.FS => 2,
            Register.GS => 2,
            Register.SS => 2,
            
            Register.PC => 4,
            Register.L => 2,
            Register.H => 2,
            Register.HL => 4,
            
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
            
            Register.R1 => 2,
            Register.R2 => 2,
            Register.R3 => 2,
            Register.R4 => 2,
            Register.R5 => 2,
            Register.R6 => 2,
            Register.R7 => 2,
            Register.R8 => 2,
            
            Register.X => 4,
            Register.Y => 4,
            
            Register.EX => 4,
            Register.FX => 4,
            Register.GX => 4,
            Register.HX => 4,
            
            Register.MB => 4,
            
            Register.CR0 => 2,
            Register.CR1 => 2,
            
            Register.F => 3,

            _ => -1,
        };
    }

    public static implicit operator string(RegisterInfo info)
    {
        return Convert.ToString((byte)info.m_Register, 16);
    }
}
