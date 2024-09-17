using CommonBCGCPU.Types;
using System;
using System.Collections.Generic;
using System.Text;
using static HexLibrary.SplitFunctions;
using static HexLibrary.HexConverter;

namespace BCG16CPUEmulator
{
    public class BCG16CPU_HelperFunctions : BCG16CPU_Registers
    {
        internal int argumentOffset = 0;

        public uint GetFullStackAddress()
        {
            return GetSegment(SS, SP);
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

        public _32Bit_Register GetABX()
        {
            return (A << 16) | B;
        }
        public _32Bit_Register GetCDX()
        {
            return (C << 16) | D;
        }

        public byte FetchByte()
        {
            byte result = m_BUS.m_Memory.ReadByte(PC);
            argumentOffset += 1;
            PC++;
            return result;
        }
        public ushort FetchWord()
        {
            ushort result = m_BUS.m_Memory.ReadWord(PC);
            argumentOffset += 2;
            PC += 2;
            return result;
        }
        public uint FetchTByte()
        {
            uint result = m_BUS.m_Memory.ReadTByte(PC);
            argumentOffset += 3;
            PC += 3;
            return result;
        }
        public int FetchDWord()
        {
            int result = m_BUS.m_Memory.ReadDWord(PC);
            argumentOffset += 4;
            PC += 4;
            return result;
        }
        public bool GetFlag(int flag)
        {
            return (F & flag) == flag;
        }
        public void SetFlag(int flag, bool value)
        {
            if (value)
            {
                F |= flag;
            }
            else
            {
                F &= ~flag;
            }
        }

        public int GetRegisterValue(Register register)
        {
            if ((CR0 & CR0_UseExtendedRegisters) == CR0_UseExtendedRegisters)
            {
                switch (register)
                {
                    case Register.ABX:
                        return GetABX();
                    case Register.CDX:
                        return GetCDX();
                }
            }

            return register switch
            {
                Register.A => A,
                Register.AH => A[true],
                Register.AL => A[false],

                Register.B => B,
                Register.BH => B[true],
                Register.BL => B[false],

                Register.C => C,
                Register.CH => C[true],
                Register.CL => C[false],

                Register.D => D,
                Register.DH => D[true],
                Register.DL => D[false],

                Register.HL => HL,
                Register.H => H,
                Register.L => L,

                Register.S => S,
                Register.SS => SS,
                Register.DS => DS,
                Register.ES => ES,

                Register.PC => PC,

                Register.AF => AF,
                Register.BF => BF,

                Register.SP => SP,
                Register.BP => BP,

                Register.R1 => R1,
                Register.R2 => R2,

                Register.MB => MB,

                Register.CR0 => CR0,
                Register.CR1 => CR1,

                Register.F => F,
                Register.FH => F[true],
                Register.FL => F[false],

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
                case Register.IL:
                case Register.MB:
                case Register.CR0:
                case Register.CR1:
                case Register.FH:
                case Register.FL:
                    return 1;
                case Register.A:
                case Register.B:
                case Register.C:
                case Register.D:
                case Register.H:
                case Register.L:
                case Register.S:
                case Register.SS:
                case Register.DS:
                case Register.ES:
                case Register.BP:
                case Register.SP:
                case Register.R1:
                case Register.R2:
                case Register.F:
                    return 2;
                case Register.PC:
                    return 3;
                case Register.HL:
                case Register.AF:
                case Register.BF:
                case Register.ABX:
                case Register.CDX:
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
                    A = value;
                    break;
                case Register.AH:
                    A[true] = value;
                    break;
                case Register.AL:
                    A[false] = value;
                    break;
                case Register.B:
                    B = value;
                    break;
                case Register.BH:
                    B[true] = value;
                    break;
                case Register.BL:
                    B[false] = value;
                    break;
                case Register.C:
                    C = value;
                    break;
                case Register.CH:
                    C[true] = value;
                    break;
                case Register.CL:
                    C[false] = value;
                    break;
                case Register.D:
                    D = value;
                    break;
                case Register.DH:
                    D[true] = value;
                    break;
                case Register.DL:
                    D[false] = value;
                    break;
                case Register.ABX:
                    if ((CR0 & CR0_UseExtendedRegisters) == CR0_UseExtendedRegisters)
                    {
                        A = (ushort)((value & 0xFFFF0000) >> 16);
                        B = (ushort)value & 0x0000FFFF;
                    }
                    break;
                case Register.CDX:
                    if ((CR0 & CR0_UseExtendedRegisters) == CR0_UseExtendedRegisters)
                    {
                        C = (ushort)((value & 0xFFFF0000) >> 16);
                        D = (ushort)value & 0x0000FFFF;
                    }
                    break;
                case Register.HL:
                    HL = value;
                    break;
                case Register.H:
                    H = value;
                    break;
                case Register.L:
                    L = value;
                    break;
                case Register.S:
                    S = value;
                    break;
                case Register.SS:
                    SS = value;
                    break;
                case Register.DS:
                    DS = value;
                    break;
                case Register.ES:
                    ES = value;
                    break;
                case Register.PC:
                    PC = value;
                    break;
                case Register.AF:
                    AF = value;
                    break;
                case Register.BF:
                    BF = value;
                    break;
                case Register.BP:
                    BP = value;
                    break;
                case Register.SP:
                    SP = value;
                    break;
                case Register.R1:
                    R1 = value;
                    break;
                case Register.R2:
                    R2 = value;
                    break;
                case Register.MB:
                    MB = value;
                    break;
                case Register.F:
                    F = value;
                    break;
                case Register.FH:
                    F[true] = value;
                    break;
                case Register.FL:
                    F[false] = value;
                    break;
                case Register.CR0:
                    CR0 = value;
                    break;
                case Register.CR1:
                    CR1 = value;
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
            SP -= offset;
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
            Compare(operand1, operand2, FZ);
            Compare(operand1, operand2, FE);
            Compare(operand1, operand2, FS);
            Compare(operand1, operand2, FL);
        }
        public void Compare(int operand1, int operand2, int Flag)
        {
            bool result;
            switch (Flag)
            {
                case FZ:
                    result = operand1 == 0;
                    SetFlag(Flag, result);
                    break;
                case FE:
                    result = operand1 == operand2;
                    SetFlag(Flag, result);
                    break;
                case FS:
                    result = operand1 < 0;
                    SetFlag(Flag, result);
                    break;
                case FL:
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
                SP++;
            }
            catch (OverflowException)
            {
                SP = 0;
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
                SP--;
            }
            catch (OverflowException)
            {
                SS--;
                SP = ushort.MaxValue;
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
            PC = address;
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
            Push(Register.SS);
            Push(Register.S);

            Push(Register.PC);

            Push(Register.F);
        }
    }
}
