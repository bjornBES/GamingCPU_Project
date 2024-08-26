using System;
using System.Collections.Generic;
using System.Linq;

namespace assembler.global
{
    public enum Register
    {
        AX = 0b00_00_0000, AL = 0b00_01_0000, AH = 0b00_10_0000,
        BX = 0b00_00_0001, BL = 0b00_01_0001, BH = 0b00_10_0001,
        CX = 0b00_00_0010, CL = 0b00_01_0010, CH = 0b00_10_0010,
        DX = 0b00_00_0011, DL = 0b00_01_0011, DH = 0b00_10_0011,
        HL = 0b00_00_0110, H = 0b00_01_0110, L = 0b00_10_0110,
        PC = 0b00_00_0111, PCH = 0b00_01_0111, PCL = 0b00_10_0111,
        F = 0b00_00_1000,  IL = 0b00_01_1000,
        AF = 0b00_00_1001, BF = 0b00_01_1001,
        SP = 0b00_01_1010, BP = 0b00_10_1010,
        R1 = 0b00_00_1011, R2 = 0b00_01_1011, R3 = 0b00_10_1011, R4 = 0b00_11_1011,
        F1 = 0b01_00_1011, F2 = 0b01_01_1011, F3 = 0b01_10_1011, F4 = 0b01_11_1011,
        S = 0b00_00_1100,  DS = 0b00_01_1100, FDS = 0b00_10_1100,JS = 0b00_11_1100,
        MB = 0b00_00_1111,

        none = 0b11_11_1111,
    }

    public class AllRegister
    {
        public static List<RegisterInfo> s_registerInfos = new List<RegisterInfo>()
        {
            new RegisterInfo(Register.AX, 2),   new RegisterInfo(Register.AL, 1),   new RegisterInfo(Register.AH, 1),
            new RegisterInfo(Register.BX, 2),   new RegisterInfo(Register.BL, 1),   new RegisterInfo(Register.BH, 1),
            new RegisterInfo(Register.CX, 2),   new RegisterInfo(Register.CL, 1),   new RegisterInfo(Register.CH, 1),
            new RegisterInfo(Register.DX, 2),   new RegisterInfo(Register.DL, 1),   new RegisterInfo(Register.DH, 1),
            new RegisterInfo(Register.HL, 4),   new RegisterInfo(Register.L, 2),    new RegisterInfo(Register.H, 2),
            new RegisterInfo(Register.PC, 4),   new RegisterInfo(Register.PCL, 2),  new RegisterInfo(Register.PCH, 2),

            new RegisterInfo(Register.F, 2),    new RegisterInfo(Register.IL, 1),

            new RegisterInfo(Register.AF, 5),   new RegisterInfo(Register.BF, 5),
            new RegisterInfo(Register.SP, 3),   new RegisterInfo(Register.BP, 2),

            new RegisterInfo(Register.R1, 2),   new RegisterInfo(Register.R2, 2),   new RegisterInfo(Register.R3, 2),   new RegisterInfo(Register.R4, 2),
            new RegisterInfo(Register.F1, 5),   new RegisterInfo(Register.F2, 5),   new RegisterInfo(Register.F3, 5),   new RegisterInfo(Register.F4, 5),

            new RegisterInfo(Register.S, 2),    new RegisterInfo(Register.DS, 2),   new RegisterInfo(Register.FDS, 2),  new RegisterInfo(Register.JS, 2),

            new RegisterInfo(Register.MB, 1),
        };
    }
    public struct RegisterInfo
    {
        public Register m_Register;
        public int m_Size;
        public Arguments m_ArgumentStyle;
        public RegisterInfo(Register register)
        {
            m_Register = register;

            int size;

            switch (register)
            {
                case Register.AX:
                case Register.BX:
                case Register.CX:
                case Register.DX:
                case Register.H:
                case Register.L:
                case Register.PCH:
                case Register.PCL:
                case Register.F:
                case Register.S:
                case Register.DS:
                case Register.R1:
                case Register.R2:
                case Register.R3:
                case Register.R4:
                    size = 2;
                    break;
                case Register.AL:
                case Register.AH:
                case Register.BL:
                case Register.BH:
                case Register.CL:
                case Register.CH:
                case Register.DL:
                case Register.DH:
                case Register.IL:
                case Register.MB:
                    size = 1;
                    break;
                case Register.HL:
                case Register.PC:
                case Register.AF:
                case Register.BF:
                case Register.SP:
                case Register.BP:
                case Register.F1:
                case Register.F2:
                case Register.F3:
                case Register.F4:
                    size = 4;
                    break;
                default:
                    size = 0;
                    break;
            }

            m_Size = size;

            switch (size)
            {
                case 0:
                case 1:
                    m_ArgumentStyle = Arguments.reg8;
                    break;
                case 2:
                    m_ArgumentStyle = Arguments.reg16;
                    break;
                case 3:
                case 4:
                    m_ArgumentStyle = Arguments.reg32;
                    break;
                case 5:
                    m_ArgumentStyle = Arguments.f_reg;                    
                    break;
                default:
                    m_ArgumentStyle = Arguments.none;
                    break;
            }
        }

        public string GetCode()
        {
            return Convert.ToString((int)m_Register, 16).PadLeft(2,'0');
        }

        public RegisterInfo(Register register, int size)
        {
            m_Register = register;
            m_Size = size;
            switch (size)
            {
                case 0:
                case 1:
                    m_ArgumentStyle = Arguments.reg8;
                    break;
                case 2:
                    m_ArgumentStyle = Arguments.reg16;
                    break;
                case 3:
                case 4:
                    m_ArgumentStyle = Arguments.reg32;
                    break;
                case 5:
                    m_ArgumentStyle = Arguments.f_reg;
                    break;
                default:
                    m_ArgumentStyle = Arguments.none;
                    break;
            }
        }
        public RegisterInfo(Register register, int size, Arguments argument)
        {
            m_Register = register;
            m_Size = size;
            m_ArgumentStyle = argument;
        }

        public static implicit operator Register(RegisterInfo info)
        {
            return info.m_Register;
        }
        public static implicit operator string(RegisterInfo info)
        {
            return info.m_Register.ToString();
        }

        public static explicit operator RegisterInfo(string v)
        {
            if (Enum.TryParse(v.ToUpper(), out Register result))
            {
                return new RegisterInfo(result);
            }
            else
            {
                try
                {
                    int a = int.Parse(v, System.Globalization.NumberStyles.HexNumber);

                }
                catch (Exception)
                {
                    return new RegisterInfo(Register.none, 0);
                }
            }
            return new RegisterInfo(Register.none, 0);
        }

        public static explicit operator RegisterInfo(Register v)
        {
            return new RegisterInfo(v);
        }

        public static explicit operator RegisterInfo(int v)
        {
            int[] registerValues = Enum.GetValues(typeof(Register)).Cast<int>().ToArray();

            for (int i = 0; i < registerValues.Length; i++)
            {
                if (registerValues[i] == v)
                {
                    return new RegisterInfo((Register)Enum.Parse(typeof(Register), v.ToString()));
                }
            }
            return new RegisterInfo(Register.none);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException();

            if (obj.GetType() == typeof(RegisterInfo))
            {
                RegisterInfo instruction = (RegisterInfo)obj;
                if (m_Register == instruction.m_Register)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(RegisterInfo left, RegisterInfo right)
        {
            return left.m_Register == right.m_Register;
        }

        public static bool operator !=(RegisterInfo left, RegisterInfo right)
        {
            return left.m_Register != right.m_Register;
        }
    }

}