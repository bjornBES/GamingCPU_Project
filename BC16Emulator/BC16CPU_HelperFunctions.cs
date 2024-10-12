using CommonBCGCPU.Types;
using System;
using System.Collections.Generic;
using System.Text;
using static HexLibrary.SplitFunctions;
using static HexLibrary.HexConverter;
using System.Windows.Markup;

namespace BCG16CPUEmulator
{
    public class BC16CPU_HelperFunctions : BC16CPU_Registers
    {
        internal int m_argumentOffset = 0;

        public uint GetFullStackAddress()
        {
            return GetSegment(m_SS, m_SP);
        }
        public Address GetSegment(Address segment, Address offset)
        {
            return (segment << 16) | offset;
        }
        public Address GetSegment(Register segment, Register offset)
        {
            int Vsegment = GetRegisterValue(segment);
            int Voffset = GetRegisterValue(offset);
            return GetSegment(Vsegment, Voffset);
        }
        public Address GetSegment(Register segment, Address offset)
        {
            int Vsegment = GetRegisterValue(segment);
            return GetSegment(Vsegment, offset);
        }

        public byte FetchByte()
        {
            byte result = m_BUS.m_Memory.ReadByte(m_PC);
            m_argumentOffset += 1;
            m_PC++;
            return result;
        }
        public ushort FetchWord()
        {
            ushort result = m_BUS.m_Memory.ReadWord(m_PC);
            m_argumentOffset += 2;
            m_PC += 2;
            return result;
        }
        public uint FetchTByte()
        {
            uint result = m_BUS.m_Memory.ReadTByte(m_PC);
            m_argumentOffset += 3;
            m_PC += 3;
            return result;
        }
        public int FetchDWord()
        {
            int result = m_BUS.m_Memory.ReadDWord(m_PC);
            m_argumentOffset += 4;
            m_PC += 4;
            return result;
        }
        public float FetchFloat()
        {
            string hex = ""; //Convert.ToString(m_BUS.m_Memory.ReadDWord(m_PC), 16);
            uint intValue = Convert.ToUInt32(hex, 16);
            byte[] floatBytes = BitConverter.GetBytes(intValue);
            float result = BitConverter.ToSingle(floatBytes, 0);
            m_argumentOffset += 4;
            m_PC += 4;
            return result;
        }
        public bool GetFlag(int flag)
        {
            return (m_F & flag) == flag;
        }
        public void SetFlag(int flag, bool value)
        {
            if (value)
            {
                m_F |= flag;
            }
            else
            {
                m_F &= ~flag;
            }
        }

        public int GetRegisterValue(Register register)
        {
            if ((m_CR0 & CR0_EnableExtendedMode) == CR0_EnableExtendedMode)
            {
                switch (register)
                {
                    case Register.AX:
                        return m_AX;
                    case Register.BX:
                        return m_BX;
                    case Register.CX:
                        return m_CX;
                    case Register.DX:
                        return m_DX;
                }
            }

            return register switch
            {
                Register.A => m_A,
                Register.AH => m_A[true],
                Register.AL => m_A[false],

                Register.B => m_B,
                Register.BH => m_B[true],
                Register.BL => m_B[false],

                Register.C => m_C,
                Register.CH => m_C[true],
                Register.CL => m_C[false],

                Register.D => m_D,
                Register.DH => m_D[true],
                Register.DL => m_D[false],

                Register.HL => m_HL,
                Register.H => m_H,
                Register.L => m_L,

                Register.CS => m_CS,
                Register.SS => m_SS,
                Register.DS => m_DS,
                Register.ES => m_ES,

                Register.PC => m_PC,

                Register.AF => m_AF,
                Register.BF => m_BF,

                Register.SP => m_SP,
                Register.BP => m_BP,

                Register.R1 => m_R1,
                Register.R2 => m_R2,
                Register.R3 => m_R3,
                Register.R4 => m_R4,
                Register.R5 => m_R5,
                Register.R6 => m_R6,

                Register.MB => m_MB,

                Register.CR0 => m_CR0,
                Register.CR1 => m_CR1,

                Register.F => m_F,

                _ => throw new NotImplementedException()
            };
        }
        public int GetRegisterSize(Register register)
        {
            switch (register)
            {
                case Register.AH:
                case Register.AL:
                case Register.BH:
                case Register.BL:
                case Register.CH:
                case Register.CL:
                case Register.DH:
                case Register.DL:
                case Register.MB:
                case Register.CR0:
                case Register.CR1:
                    return 1;
                case Register.A:
                case Register.B:
                case Register.C:
                case Register.D:
                case Register.H:
                case Register.L:
                case Register.CS:
                case Register.SS:
                case Register.DS:
                case Register.ES:
                case Register.BP:
                case Register.SP:
                case Register.R1:
                case Register.R2:
                case Register.R3:
                case Register.R4:
                case Register.R5:
                case Register.R6:
                case Register.F:
                    return 2;
                case Register.PC:
                    return 3;
                case Register.HL:
                case Register.AF:
                case Register.BF:
                case Register.AX:
                case Register.BX:
                case Register.CX:
                case Register.DX:
                    return 4;
                case Register.none:
                default:
                    throw new NotImplementedException();
            }
        }
        public void SetRegisterValue(Register register, int value)
        {
            switch (register)
            {
                case Register.A:
                    m_A = value;
                    break;
                case Register.AH:
                    m_A[true] = value;
                    break;
                case Register.AL:
                    m_A[false] = value;
                    break;
                case Register.B:
                    m_B = value;
                    break;
                case Register.BH:
                    m_B[true] = value;
                    break;
                case Register.BL:
                    m_B[false] = value;
                    break;
                case Register.C:
                    m_C = value;
                    break;
                case Register.CH:
                    m_C[true] = value;
                    break;
                case Register.CL:
                    m_C[false] = value;
                    break;
                case Register.D:
                    m_D = value;
                    break;
                case Register.DH:
                    m_D[true] = value;
                    break;
                case Register.DL:
                    m_D[false] = value;
                    break;
                case Register.AX:
                    if ((m_CR0 & CR0_EnableExtendedMode) == CR0_EnableExtendedMode)
                    {
                        m_AX = value;
                        m_A = m_AX[false];
                    }
                    break;
                case Register.BX:
                    if ((m_CR0 & CR0_EnableExtendedMode) == CR0_EnableExtendedMode)
                    {
                        m_BX = value;
                        m_B = m_BX[false];
                    }
                    break;
                case Register.CX:
                    if ((m_CR0 & CR0_EnableExtendedMode) == CR0_EnableExtendedMode)
                    {
                        m_CX = value;
                        m_C = m_CX[false];
                    }
                    break;
                case Register.DX:
                    if ((m_CR0 & CR0_EnableExtendedMode) == CR0_EnableExtendedMode)
                    {
                        m_DX = value;
                        m_D = m_DX[false];
                    }
                    break;
                case Register.HL:
                    m_HL = value;
                    break;
                case Register.H:
                    m_H = value;
                    break;
                case Register.L:
                    m_L = value;
                    break;
                case Register.CS:
                    m_CS = value;
                    break;
                case Register.SS:
                    m_SS = value;
                    break;
                case Register.DS:
                    m_DS = value;
                    break;
                case Register.ES:
                    m_ES = value;
                    break;
                case Register.PC:
                    m_PC = value;
                    break;
                case Register.AF:
                    m_AF = value;
                    break;
                case Register.BF:
                    m_BF = value;
                    break;
                case Register.BP:
                    m_BP = value;
                    break;
                case Register.SP:
                    m_SP = value;
                    break;
                case Register.R1:
                    m_R1 = value;
                    break;
                case Register.R2:
                    m_R2 = value;
                    break;
                case Register.R3:
                    m_R3 = value;
                    break;
                case Register.R4:
                    m_R4 = value;
                    break;
                case Register.R5:
                    m_R5 = value;
                    break;
                case Register.R6:
                    m_R6 = value;
                    break;
                case Register.MB:
                    m_MB = value;
                    break;
                case Register.F:
                    m_F = value;
                    break;
                case Register.CR0:
                    m_CR0 = value;
                    break;
                case Register.CR1:
                    m_CR1 = value;
                    break;
            }
        }
        public void SetRegisterValue(Register register, Register value)
        {
            SetRegisterValue(register, GetRegisterValue(value));
        }

        public void ALU_Sef(int flag)
        {
            SetFlag(flag, true);
        }

        public void ALU_Clf(int flag)
        {
            SetFlag(flag, false);
        }

        public void ALU_Add(Register destination, int source)
        {
            int V = GetRegisterValue(destination);
            int result = V + source;
            SetRegisterValue(destination, result);
        }
        public int ALU_Add(Register arg1, Register arg2)
        {
            int V1 = GetRegisterValue(arg1);
            int V2 = GetRegisterValue(arg2);

            return V1 + V2;
        }
        public int ALU_Add(Register destination, int source, out int result)
        {
            int V = GetRegisterValue(destination);
            result = V + source;
            return result;
        }

        public void ALU_Sub(Register destination, int source)
        {
            int V = GetRegisterValue(destination);
            int result = V - source;
            SetRegisterValue(destination, result);
        }
        public int ALU_Sub(Register destination, int source, out int result)
        {
            int V = GetRegisterValue(destination);
            result = V - source;
            return result;
        }

        public void ALU_Mul(Register destination, int source)
        {
            int V = GetRegisterValue(destination);
            int result = V - source;
            SetRegisterValue(destination, result);
        }

        public void ALU_Div(Register destination, int source)
        {
            int V = GetRegisterValue(destination);
            int result = V / source;
            SetRegisterValue(destination, result);
        }

        public void ALU_And(Register destination, int source)
        {
            int V = GetRegisterValue(destination);
            int result = V & source;
            SetRegisterValue(destination, result);
        }

        public void ALU_Or(Register destination, int source)
        {
            int V = GetRegisterValue(destination);
            int result = V | source;
            SetRegisterValue(destination, result);
        }
        public int ALU_Or(Register destination, int source, out int result)
        {
            int V = GetRegisterValue(destination);
            result = V | source;
            return result;
        }

        public void ALU_Nor(Register destination, int source)
        {
            int V = GetRegisterValue(destination);
            int result = ~(V | source);
            SetRegisterValue(destination, result);
        }

        public void ALU_Xor(Register destination, int source)
        {
            int V = GetRegisterValue(destination);
            int result = V ^ source;
            SetRegisterValue(destination, result);
        }

        public void ALU_Not(Register destination)
        {
            int V = GetRegisterValue(destination);
            int result = ~V;
            SetRegisterValue(destination, result);
        }

        public void ALU_Neg(Register destination)
        {
            int V = GetRegisterValue(destination);
            int result = -V;
            SetRegisterValue(destination, result);
        }

        public void ALU_RNG(Register destination)
        {
            int V = new Random().Next(0, 0xFF);
            SetRegisterValue(destination, V);
        }

        public void Return(int offset)
        {
            Pop(Register.PC);
            m_SP -= offset;
        }
        public void Compare(Register operand1, Register operand2)
        {
            int Roperand1 = GetRegisterValue(operand1);
            int Roperand2 = GetRegisterValue(operand2);

            Compare(Roperand1, Roperand2);
        }
        public void Compare(Register operand1, int operand2)
        {
            int Roperand1 = GetRegisterValue(operand1);

            Compare(Roperand1, operand2);
        }
        public void Compare(int operand1, Register operand2)
        {
            int Roperand2 = GetRegisterValue(operand2);

            Compare(operand1, Roperand2);
        }
        public void Compare(int operand1, int operand2)
        {
            Compare(operand1, operand2, FL_Z);
            Compare(operand1, operand2, FL_E);
            Compare(operand1, operand2, m_FS);
            Compare(operand1, operand2, FL_L);
        }
        public void Compare(int operand1, int operand2, int Flag)
        {
            bool result;
            switch (Flag)
            {
                case FL_Z:
                    result = operand1 == 0;
                    SetFlag(Flag, result);
                    break;
                case FL_E:
                    result = operand1 == operand2;
                    SetFlag(Flag, result);
                    break;
                case FL_S:
                    result = operand1 < 0;
                    SetFlag(Flag, result);
                    break;
                case FL_L:
                    result = operand1 < operand2;
                    SetFlag(Flag, result);
                    break;
                default:
                    break;
            }
        }

        public void Push(byte value)
        {
            uint StackAddress = GetFullStackAddress();

            m_BUS.m_Memory.WriteByte(StackAddress, value);

            try
            {
                m_SP++;
            }
            catch (OverflowException)
            {
                m_SP = 0;
            }
        }
        public void Push(ushort value)
        {
            uint StackAddress = GetFullStackAddress();

            m_BUS.m_Memory.WriteWord(StackAddress, value);

            byte[] bytes = BitConverter.GetBytes(value);

            for (int i = 0; i < bytes.Length; i++)
            {
                Push(bytes[i]);
            }
        }
        public void Push(uint value)
        {
            uint StackAddress = GetFullStackAddress();

            m_BUS.m_Memory.WriteDWord(StackAddress, value);

            byte[] bytes = BitConverter.GetBytes(value);

            for (int i = 0; i < bytes.Length; i++)
            {
                Push(bytes[i]);
            }
        }
        public void Push(int value, ArgumentMode source)
        {
            switch (source)
            {
                case ArgumentMode.immediate_byte:
                    Push((byte)value);
                    break;
                case ArgumentMode.immediate_word:
                    Push((ushort)value);
                    break;
                case ArgumentMode.immediate_tbyte:
                    Push((uint)value & 0x00FF_FFFF);
                    break;
                case ArgumentMode.immediate_dword:
                    Push((uint)value);
                    break;
            }
        }
        public void Push(Register register)
        {
            int RegisterSize = GetRegisterSize(register);
            int RegisterValue = GetRegisterValue(register);

            string RegisterHex = ToHexString(RegisterValue);
            string[] HexString = SplitHexString(RegisterHex, RegisterSize);

            for (int i = 0; i < HexString.Length; i++)
            {
                byte v = Convert.ToByte(HexString[i], 16);
                Push(v);
            }
        }

        public byte Pop(out byte result)
        {
            try
            {
                m_SP--;
            }
            catch (OverflowException)
            {
                m_SS--;
                m_SP = ushort.MaxValue;
            }

            uint StackAddress = GetFullStackAddress();

            result = m_BUS.m_Memory.ReadByte(StackAddress);
            return result;
        }
        public ushort Pop(out ushort result)
        {
            byte[] bytes = new byte[]
            {
                Pop(out byte _),
                Pop(out byte _)
            };

            result = BitConverter.ToUInt16(bytes);
            return result;
        }
        public uint Pop(out uint result)
        {
            byte[] bytes = new byte[]
            {
                Pop(out byte _),
                Pop(out byte _),
                Pop(out byte _),
                0
            };

            result = BitConverter.ToUInt32(bytes);
            return result;
        }
        public int Pop(out int result)
        {
            byte[] bytes = new byte[]
            {
                Pop(out byte _),
                Pop(out byte _),
                Pop(out byte _),
                Pop(out byte _)
            };

            result = BitConverter.ToInt32(bytes);
            return result;
        }
        public void Pop(Register result)
        {
            int size = GetRegisterSize(result) - 1;

            switch ((Size)size)
            {
                case Size._byte:
                    SetRegisterValue(result, Pop(out byte _));
                    break;
                case Size._word:
                    SetRegisterValue(result, Pop(out ushort _));
                    break;
                case Size._tbyte:
                    SetRegisterValue(result, (int)Pop(out uint _));
                    break;
                case Size._dword:
                    SetRegisterValue(result, (int)Pop(out int _));
                    break;
            }
        }

        public void Jump(Address address)
        {
            m_AddressSave = address;
            SetFlag(FL_J, true);
        }
        public void Call(Address address)
        {
            Push(Register.PC);
            Jump(address);
        }
        public void PushInterrupt()
        {
            Push(Register.DS);
            Push(Register.ES);
            Push(Register.FS);
            Push(Register.SS);
            Push(Register.CS);

            Push(Register.PC);

            Push(Register.F);
        }
    }
}
