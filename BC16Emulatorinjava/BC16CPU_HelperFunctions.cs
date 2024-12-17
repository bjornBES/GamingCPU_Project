using CommonBCGCPU.Types;
using System;
using System.Collections.Generic;
using System.Text;
using static HexLibrary.SplitFunctions;
using static HexLibrary.HexConverter;
using System.Windows.Markup;
using System.Linq;

namespace BC16CPUEmulator
{
    public class BC16CPU_HelperFunctions : BC16CPU_Registers
    {
        internal int m_argumentOffset = 0;

        public uint GetFullStackAddress()
        {
            return GetSegment(m_ESS[false], m_SP);
        }
        public Address GetSegment(Address segment, Address offset)
        {
            return (segment * 16) | offset;
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

        public void Pushr()
        {
            if ((m_CR0 & CR0_EnableExtendedMode) == CR0_EnableExtendedMode)
            {
                Push(Register.AX);
                Push(Register.BX);
                Push(Register.CX);
                Push(Register.DX);
                Push(Register.H);
                Push(Register.L);
            }
            else
            {
                Push(Register.A);
                Push(Register.B);
                Push(Register.C);
                Push(Register.D);
                Push(Register.H);
                Push(Register.L);
            }
        }
        public void Popr()
        {
            if ((m_CR0 & CR0_EnableExtendedMode) == CR0_EnableExtendedMode)
            {
                Pop(Register.L);
                Pop(Register.H);
                Pop(Register.DX);
                Pop(Register.CX);
                Pop(Register.BX);
                Pop(Register.AX);
            }
            else
            {
                Pop(Register.L);
                Pop(Register.H);
                Pop(Register.D);
                Pop(Register.C);
                Pop(Register.B);
                Pop(Register.A);
            }
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
            byte[] bytes = BitConverter.GetBytes(m_BUS.m_Memory.ReadWord(m_PC));
            m_argumentOffset += 2;
            m_PC += 2;
            ushort result = BitConverter.ToUInt16(bytes.Reverse().ToArray());
            return result;
        }
        public uint FetchTByte()
        {
            byte[] bytes = BitConverter.GetBytes(m_BUS.m_Memory.ReadTByte(m_PC));
            m_argumentOffset += 3;
            m_PC += 3;
            uint result = BitConverter.ToUInt32(bytes.Reverse().ToArray());
            return result;
        }
        public int FetchDWord()
        {
            byte[] bytes = BitConverter.GetBytes(m_BUS.m_Memory.ReadDWord(m_PC));
            m_argumentOffset += 4;
            m_PC += 4;
            int result = BitConverter.ToInt32(bytes.Reverse().ToArray());
            return result;
        }
        public int FetchQWord()
        {
            List<byte> bytes = new List<byte>();

            bytes.AddRange(BitConverter.GetBytes(FetchDWord()));
            bytes.AddRange(BitConverter.GetBytes(FetchDWord()));
            m_argumentOffset += 4;
            m_PC += 4;
            int result = BitConverter.ToInt32(bytes.ToArray().Reverse().ToArray());
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
            if ((m_CR0 & CR0_EnableProtectedMode) == CR0_EnableProtectedMode)
            {
                switch (register)
                {
                    case Register.AX:
                        return m_AX[false];
                    case Register.BX:
                        return m_BX;
                    case Register.CX:
                        return m_CX;
                    case Register.DX:
                        return m_DX;

                    case Register.PC:
                        return m_PC;

                    case Register.R1: return m_R1;
                    case Register.R2: return m_R2;
                    case Register.R3: return m_R3;
                    case Register.R4: return m_R4;
                    case Register.R5: return m_R5;
                    case Register.R6: return m_R6;
                    case Register.R7: return m_R7;
                    case Register.R8: return m_R8;
                    case Register.R9: return m_R9;
                    case Register.R10: return m_R10;
                    case Register.R11: return m_R11;
                    case Register.R12: return m_R12;
                    case Register.R13: return m_R13;
                    case Register.R14: return m_R14;
                    case Register.R15: return m_R15;
                    case Register.R16: return m_R16;
                    case Register.R1L: return m_R1[false];
                    case Register.R2L: return m_R2[false];
                    case Register.R3L: return m_R3[false];
                    case Register.R4L: return m_R4[false];
                    case Register.R5L: return m_R5[false];
                    case Register.R6L: return m_R6[false];
                    case Register.R7L: return m_R7[false];
                    case Register.R8L: return m_R8[false];
                    case Register.R9L: return m_R9[false];
                    case Register.R10L: return m_R10[false];
                    case Register.R11L: return m_R11[false];
                    case Register.R12L: return m_R12[false];
                    case Register.R13L: return m_R13[false];
                    case Register.R14L: return m_R14[false];
                    case Register.R15L: return m_R15[false];
                    case Register.R16L: return m_R16[false];
                    case Register.R1H: return m_R1[true];
                    case Register.R2H: return m_R2[true];
                    case Register.R3H: return m_R3[true];
                    case Register.R4H: return m_R4[true];
                    case Register.R5H: return m_R5[true];
                    case Register.R6H: return m_R6[true];
                    case Register.R7H: return m_R7[true];
                    case Register.R8H: return m_R8[true];
                    case Register.R9H: return m_R9[true];
                    case Register.R10H: return m_R10[true];
                    case Register.R11H: return m_R11[true];
                    case Register.R12H: return m_R12[true];
                    case Register.R13H: return m_R13[true];
                    case Register.R14H: return m_R14[true];
                    case Register.R15H: return m_R15[true];
                    case Register.R16H: return m_R16[true];
                }
            }
            else
            {
                switch (register)
                {
                    case Register.PC:
                        return m_PC[false];
                    case Register.R1:
                        return m_R1[false];
                    case Register.R2:
                        return m_R2[false];
                    case Register.R3:
                        return m_R3[false];
                    case Register.R4:
                        return m_R4[false];
                    case Register.R5:
                        return m_R5[false];
                    case Register.R6:
                        return m_R6[false];
                    case Register.R7:
                        return m_R7[false];
                    case Register.R8:
                        return m_R8[false];
                    case Register.R9:
                        return m_R9[false];
                    case Register.R10:
                        return m_R10[false];
                    case Register.R11:
                        return m_R11[false];
                    case Register.R12:
                        return m_R12[false];
                    case Register.R13:
                        return m_R13[false];
                    case Register.R14:
                        return m_R14[false];
                    case Register.R15:
                        return m_R15[false];
                    case Register.R16:
                        return m_R16[false];
                }
            }

            return register switch
            {
                Register.AH => m_AX[4],
                Register.AL => m_AX[3],
                Register.A => m_AX[false],
                Register.AX => m_AX,

                Register.BH => m_BX[4],
                Register.BL => m_BX[3],
                Register.B => m_BX[false],
                Register.BX => m_BX,

                Register.CH => m_CX[4],
                Register.CL => m_CX[3],
                Register.C => m_CX[false],
                Register.CX => m_CX,

                Register.DH => m_DX[4],
                Register.DL => m_DX[3],
                Register.D => m_DX[false],
                Register.DX => m_DX,

                Register.CS => m_ECS[false],
                Register.DS => m_EDS[false],
                Register.ES => m_EES[false],
                Register.FS => m_EFS[false],
                Register.GS => m_EGS[false],
                Register.HS => m_EHS[false],
                Register.SS => m_ESS[false],

                Register.PC => m_PC,

                Register.HL => m_HL,
                Register.H => m_HL[true],
                Register.L => m_HL[false],

                Register.AF => m_AF,
                Register.BF => m_BF,
                Register.CF => m_CF,
                Register.DF => m_DF,

                Register.SP => m_SP,
                Register.BP => m_BP,

                Register.CR0 => m_CR0,

                Register.F => m_F,

                Register.PTA => m_PTA,
                Register.GDA => m_GDA,

                Register.ECS => m_ECS,
                Register.EDS => m_EDS,
                Register.EES => m_EES,
                Register.EFS => m_EFS,
                Register.EGS => m_EGS,
                Register.EHS => m_EHS,
                Register.ESS => m_ESS,

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
                case Register.CR0:
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
                case Register.FS:
                case Register.BP:
                case Register.SP:
                case Register.F:
                    return 2;
                case Register.PC:
                    if ((m_CR0 & CR0_EnableExtendedMode) == CR0_EnableExtendedMode)
                    {
                        return 3;
                    }
                    else if ((m_CR0 & CR0_EnableProtectedMode) == CR0_EnableProtectedMode)
                    {
                        return 4;
                    }
                    else
                    {
                        return 2;
                    }
                case Register.HL:
                case Register.AF:
                case Register.BF:
                case Register.AX:
                case Register.BX:
                case Register.CX:
                case Register.DX:
                    return 4;

                case Register.R1L:
                case Register.R2L:
                case Register.R3L:
                case Register.R4L:
                case Register.R5L:
                case Register.R6L:
                case Register.R7L:
                case Register.R8L:
                case Register.R9L:
                case Register.R10L:
                case Register.R11L:
                case Register.R12L:
                case Register.R13L:
                case Register.R14L:
                case Register.R15L:
                case Register.R16L:
                case Register.R1H:
                case Register.R2H:
                case Register.R3H:
                case Register.R4H:
                case Register.R5H:
                case Register.R6H:
                case Register.R7H:
                case Register.R8H:
                case Register.R9H:
                case Register.R10H:
                case Register.R11H:
                case Register.R12H:
                case Register.R13H:
                case Register.R14H:
                case Register.R15H:
                case Register.R16H:
                    return 2;

                case Register.R1:
                case Register.R2:
                case Register.R3:
                case Register.R4:
                case Register.R5:
                case Register.R6:
                case Register.R7:
                case Register.R8:
                case Register.R9:
                case Register.R10:
                case Register.R11:
                case Register.R12:
                case Register.R13:
                case Register.R14:
                case Register.R15:
                case Register.R16:
                    if ((m_CR0 & CR0_EnableProtectedMode) == CR0_EnableProtectedMode)
                    {
                        return 4;
                    }
                    else
                    {
                        return 2;
                    }
                case Register.none:
                default:
                    throw new NotImplementedException();
            }
        }
        public void SetRegisterValue(Register register, int value)
        {
            int OldValue = GetRegisterValue(register);
            Compare(OldValue, value, FL_S, FL_Z);

            switch (register)
            {
                case Register.A:
                    m_AX[false] = value;
                    break;
                case Register.AH:
                    m_AX[4] = (byte)value;
                    break;
                case Register.AL:
                    m_AX[3] = (byte)value;
                    break;
                case Register.B:
                    m_BX[false] = value;
                    break;
                case Register.BH:
                    m_BX[4] = value;
                    break;
                case Register.BL:
                    m_BX[3] = value;
                    break;
                case Register.C:
                    m_CX[false] = value;
                    break;
                case Register.CH:
                    m_CX[4] = value;
                    break;
                case Register.CL:
                    m_CX[3] = value;
                    break;
                case Register.D:
                    m_DX[false] = value;
                    break;
                case Register.DH:
                    m_DX[4] = value;
                    break;
                case Register.DL:
                    m_DX[3] = value;
                    break;
                case Register.AX:
                    if ((m_CR0 & CR0_EnableProtectedMode) == CR0_EnableProtectedMode)
                    {
                        m_AX = value;
                    }
                    break;
                case Register.BX:
                    if ((m_CR0 & CR0_EnableProtectedMode) == CR0_EnableProtectedMode)
                    {
                        m_BX = value;
                    }
                    break;
                case Register.CX:
                    if ((m_CR0 & CR0_EnableProtectedMode) == CR0_EnableProtectedMode)
                    {
                        m_CX = value;
                    }
                    break;
                case Register.DX:
                    if ((m_CR0 & CR0_EnableProtectedMode) == CR0_EnableProtectedMode)
                    {
                        m_DX = value;
                    }
                    break;
                case Register.HL:
                    m_HL = value;
                    break;
                case Register.H:
                    m_HL[true] = value;
                    break;
                case Register.L:
                    m_HL[false] = value;
                    break;
                case Register.CS:
                    m_ECS[false] = value;
                    break;
                case Register.SS:
                    m_ESS[false] = value;
                    break;
                case Register.DS:
                    m_EDS[false] = value;
                    break;
                case Register.ES:
                    m_EES[false] = value;
                    break;
                case Register.FS:
                    m_EFS[false] = value;
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
                case Register.F:
                    m_F = value;
                    break;
                case Register.CR0:
                    m_CR0 = value;
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
                case Register.R7:
                    m_R7 = value;
                    break;
                case Register.R8:
                    m_R8 = value;
                    break;
                case Register.R9:
                    m_R9 = value;
                    break;
                case Register.R10:
                    m_R10 = value;
                    break;
                case Register.R11:
                    m_R11 = value;
                    break;
                case Register.R12:
                    m_R12 = value;
                    break;
                case Register.R13:
                    m_R13 = value;
                    break;
                case Register.R14:
                    m_R14 = value;
                    break;
                case Register.R15:
                    m_R15 = value;
                    break;
                case Register.R16:
                    m_R16 = value;
                    break;
            }
        }
        public void SetRegisterValue(Register register, Register value)
        {
            SetRegisterValue(register, GetRegisterValue(value));
        }

        public void Return(int offset)
        {
            PopPC();
            m_SP -= offset;
        }

        public void PopPC()
        {
            if ((m_CR0 & CR0_EnableExtendedMode) == CR0_EnableExtendedMode)
            {
                Pop(out uint result);
                SetRegisterValue(Register.PC, (int)result);
            }
            else if ((m_CR0 & CR0_EnableProtectedMode) == CR0_EnableProtectedMode)
            {
                Pop(out uint result);
                SetRegisterValue(Register.PC, (int)result);
            }
            else
            {
                Pop(out ushort result);                     // PC
                SetRegisterValue(Register.PC, result);
                Pop(out result);                            // CS
                SetRegisterValue(Register.CS, result);

            }
        }
        public void PushPC()
        {
            if ((m_CR0 & CR0_EnableExtendedMode) == CR0_EnableExtendedMode)
            {
                Push(Register.PC);
            }
            else if ((m_CR0 & CR0_EnableProtectedMode) == CR0_EnableProtectedMode)
            {
                Push(Register.PC);
            }
            else
            {
                Push(Register.CS);
                Push(Register.PC);

            }
        }

        public void Sez(Register operand1)
        {
            SetRegisterValue(operand1, 0);
        }
        public void Test(Register operand1)
        {
            int Roperand1 = GetRegisterValue(operand1);
            int Roperand2 = GetRegisterValue(operand1);

            Compare(Roperand1, Roperand2, FL_S, FL_Z, FL_C, FL_E);
        }
        public void Compare(Register operand1, Register operand2, params int[] flags)
        {
            int Roperand1 = GetRegisterValue(operand1);
            int Roperand2 = GetRegisterValue(operand2);

            Compare(Roperand1, Roperand2, flags);
        }
        public void Compare(Register operand1, int operand2, params int[] flags)
        {
            int Roperand1 = GetRegisterValue(operand1);

            Compare(Roperand1, operand2, flags);
        }
        public void Compare(int operand1, Register operand2, params int[] flags)
        {
            int Roperand2 = GetRegisterValue(operand2);

            Compare(operand1, Roperand2, flags);
        }
        public void Compare(int operand1, int operand2, params int[] flags)
        {
            for (int i = 0; i < flags.Length; i++)
            {
                Compare(operand1, operand2, flags[i]);
            }
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
                case FL_G:
                    result = operand1 > operand2;
                    SetFlag(Flag, result);
                    break;
                case FL_C:
                    long t = operand1 + operand2;
                    if (t > int.MaxValue)
                    {
                        SetFlag(Flag, true);
                    }
                    else
                    {
                        SetFlag(Flag, false);
                    }
                    break;
                case FL_O:
                    try
                    {
                        int a = operand1 + operand2;
                    }
                    catch (OverflowException)
                    {
                        SetFlag(Flag, true);
                    }
                    break;
                case FL_U:
                    try
                    {
                        int a = operand1 - operand2;
                    }
                    catch (OverflowException)
                    {
                        SetFlag(Flag, true);
                    }
                    break;
                default:
                    break;
            }
        }

        public void Push(byte value)
        {
            uint StackAddress = GetFullStackAddress();

            m_BUS.m_Memory.WriteByte(StackAddress, value);

            m_SP++;
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

        public void Push(Register register)
        {
            int RegisterSize = GetRegisterSize(register);
            int RegisterValue = GetRegisterValue(register);


            string RegisterHex = ToHexString(RegisterValue);
            string[] HexString = SplitHexString(RegisterHex, RegisterSize);

            for (int i = 0; i < RegisterSize; i++)
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
                m_ESS[false]--;
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
                    SetRegisterValue(result, Pop(out int _));
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
            PushPC();
            Jump(address);
        }
        public void PushInterrupt()
        {
            Pushr();
            PushPC();
            Push(Register.F);
        }
        public static int ChsToLba(int cylinder, int head, int sector, int headsPerCylinder, int sectorsPerTrack)
        {
            return (cylinder * headsPerCylinder + head) * sectorsPerTrack + (sector - 1);
        }

        public static (int cylinder, int head, int sector) LbaToChs(int lba, int headsPerCylinder, int sectorsPerTrack)
        {
            int cylinder = lba / (headsPerCylinder * sectorsPerTrack);
            int temp = lba % (headsPerCylinder * sectorsPerTrack);
            int head = temp / sectorsPerTrack;
            int sector = (temp % sectorsPerTrack) + 1;
            return (cylinder, head, sector);
        }
    }
}
