using CommonBCGCPU.Types;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using System.ComponentModel;

namespace BC16CPUEmulator
{
    public enum Size
    {
        _byte,
        _word,
        _tbyte,
        _dword
    }
    public enum ArgumentType
    {
        imm,
        Address,
        Register,
        Float,
        SegmentAddress,
    }
    public class BC16CPU_Instructions : BC16CPU_Interrupts
    {
        public void Move(Size InstructionSize, ArgumentMode destination, ArgumentMode source)
        {
            if (getDestinationAll(destination, InstructionSize, out Register? DestinationRegister, out Address? DestinationAddress))
            {
                throw new NotImplementedException();
            }
            if (getSourceAll(source, InstructionSize, out int? ImmSource))
            {
                throw new NotImplementedException();
            }

            if (DestinationRegister.HasValue)
            {
                if (ImmSource.HasValue)
                {
                    SetRegisterValue(DestinationRegister.Value, ImmSource.Value);
                }
                Compare(DestinationRegister.Value, DestinationRegister.Value, FL_S, FL_Z);
            }
            else if (DestinationAddress.HasValue)
            {
                if (ImmSource.HasValue)
                {
                    int OldValue = 0;
                    switch (InstructionSize)
                    {
                        case Size._byte:
                            OldValue = m_BUS.m_Memory.ReadByte(DestinationAddress.Value);
                            m_BUS.m_Memory.WriteByte(DestinationAddress.Value, (byte)ImmSource.Value);
                            break;
                        case Size._word:
                            OldValue = m_BUS.m_Memory.ReadWord(DestinationAddress.Value);
                            m_BUS.m_Memory.WriteWord(DestinationAddress.Value, (ushort)ImmSource.Value);
                            break;
                        case Size._tbyte:
                            OldValue = (int)m_BUS.m_Memory.ReadTByte(DestinationAddress.Value);
                            m_BUS.m_Memory.WriteTByte(DestinationAddress.Value, (uint)ImmSource.Value);
                            break;
                        case Size._dword:
                            OldValue = m_BUS.m_Memory.ReadDWord(DestinationAddress.Value);
                            m_BUS.m_Memory.WriteDWord(DestinationAddress.Value, (uint)ImmSource.Value);
                            break;
                        default:
                            break;
                    }

                    Compare(ImmSource.Value, OldValue, FL_S, FL_Z);
                }
            }


        }
        public void Move(Size InstructionSize, Register destination, ArgumentMode source)
        {
            if (getSourceAll(source, InstructionSize, out int? ImmSource))
            {
                throw new NotImplementedException();
            }

            if (ImmSource.HasValue)
            {
                int OldValue = GetRegisterValue(destination);
                SetRegisterValue(destination, ImmSource.Value);
                Compare(OldValue, ImmSource.Value, FL_S, FL_Z);
            }
        }
        public void Move(Register destination, Register source)
        {
            SetRegisterValue(destination, GetRegisterValue(source));
            int OldValue = GetRegisterValue(destination);
            Compare(OldValue, destination, FL_S, FL_Z);
        }

        public void Cmp(ArgumentMode operand1, ArgumentMode operand2)
        {
            if (getSourceAll(operand1, Size._word, out int? ImmSource1))
            {
                throw new NotImplementedException();
            }
            if (getSourceAll(operand2, Size._word, out int? ImmSource2))
            {
                throw new NotImplementedException();
            }

            if (ImmSource1.HasValue)
            {
                if (ImmSource2.HasValue)
                {
                    Compare(ImmSource1.Value, ImmSource2.Value, FL_E, FL_Z, FL_S, FL_C);
                }
            }
        }
        public void Cmp(Register operand1, ArgumentMode operand2)
        {
            if (getSourceAll(operand2, Size._word, out int? ImmSource2))
            {
                throw new NotImplementedException();
            }

            if (ImmSource2.HasValue)
            {
                Compare(operand1, ImmSource2.Value, FL_E, FL_Z, FL_S, FL_C, FL_G, FL_L);
            }
        }

        public void Push(Size size, ArgumentMode source)
        {
            Register? DestinationRegister = null;
            if (getSource(source, ArgumentType.Register, out object result))
            {
                DestinationRegister = (Register)result;
            }

            if (DestinationRegister.HasValue)
            {
                Push(DestinationRegister.Value);
            }
        }
        public void Pop(Size InstructionSize, ArgumentMode destination)
        {
            if (getDestinationAll(destination, Size._byte, out Register? DestinationRegister, out Address? DestinationAddress))
            {
                throw new NotImplementedException();
            }

            if (DestinationRegister.HasValue)
            {
                int OldValue = GetRegisterValue(DestinationRegister.Value);
                Pop(DestinationRegister.Value);
                Compare(OldValue, DestinationRegister.Value, FL_S, FL_Z);
            }
        }
        public void Call(ArgumentMode destination)
        {
            if (getDestinationAll(destination, Size._byte, out Register? _, out Address? DestinationAddress))
            {
                throw new NotImplementedException();
            }

            if (DestinationAddress.HasValue)
            {
                Call(DestinationAddress.Value);
            }
        }
        public void Ret(ArgumentMode source)
        {
            if (getSourceAll(source, Size._byte, out int? ImmSource))
            {
                throw new NotImplementedException();
            }

            if (ImmSource.HasValue)
            {
                Return(ImmSource.Value);
            }
        }
        public void Sez(ArgumentMode destination)
        {
            if (getDestinationAll(destination, Size._byte, out Register? DestinationRegister, out Address? DestinationAddress))
            {
                throw new NotImplementedException();
            }

            if (DestinationRegister.HasValue)
            {
                int OldValue = GetRegisterValue(DestinationRegister.Value);
                SetRegisterValue(DestinationRegister.Value, 0);
                Compare(OldValue, DestinationRegister.Value, FL_S, FL_Z);
            }
            else if (DestinationAddress.HasValue)
            {
                m_BUS.m_Memory.WriteByte(DestinationAddress.Value, 0);
            }
        }        
        public void Test(ArgumentMode destination)
        {
            if (getDestinationAll(destination, Size._byte, out Register? DestinationRegister, out Address? DestinationAddress))
            {
                throw new NotImplementedException();
            }

            if (DestinationRegister.HasValue)
            {
                Compare(DestinationRegister.Value, DestinationRegister.Value);
            }
            else if (DestinationAddress.HasValue)
            {
                byte b = m_BUS.m_Memory.ReadByte(DestinationAddress.Value);
                Compare(b, b, FL_E, FL_Z, FL_S, FL_C);
            }
        }

        public void Swap(ArgumentMode register1, ArgumentMode register2)
        {
            if (getDestinationAll(register1, Size._byte, out Register? DestinationRegister1, out Address? _))
            {
                throw new NotImplementedException();
            }
            if (getDestinationAll(register2, Size._byte, out Register? DestinationRegister2, out Address? _))
            {
                throw new NotImplementedException();
            }

            if (DestinationRegister1.HasValue)
            {
                if (DestinationRegister2.HasValue)
                {
                    int vRegister1 = GetRegisterValue(DestinationRegister1.Value);
                    int vRegister2 = GetRegisterValue(DestinationRegister2.Value);
                    SetRegisterValue(DestinationRegister1.Value, vRegister2);
                    SetRegisterValue(DestinationRegister2.Value, vRegister1);
                }
            }
        }

        public void Add(ArgumentMode destination, ArgumentMode source, bool carry)
        {
            if (getDestinationAll(destination, Size._byte, out Register? DestinationRegister, out Address? DestinationAddress))
            {
                throw new NotImplementedException();
            }
            if (getSourceAll(source, Size._word, out int? ImmSource))
            {
                throw new NotImplementedException();
            }

            if (DestinationRegister.HasValue)
            {
                if (ImmSource.HasValue)
                {
                    SetRegisterValue(DestinationRegister.Value, ALU_Add(DestinationRegister.Value, ImmSource.Value, carry, out _));
                }
            }
        }
        public void Add(Register destination, ArgumentMode source, bool carry)
        {
            if (getSourceAll(source, Size._word, out int? ImmSource))
            {
                throw new NotImplementedException();
            }

            if (ImmSource.HasValue)
            {
                SetRegisterValue(destination, ALU_Add(destination, ImmSource.Value, carry, out _));
            }
        }
        public void Sub(ArgumentMode destination, ArgumentMode source, bool carry)
        {
            if (getDestinationAll(destination, Size._byte, out Register? DestinationRegister, out Address? DestinationAddress))
            {
                throw new NotImplementedException();
            }
            if (getSourceAll(source, Size._word, out int? ImmSource))
            {
                throw new NotImplementedException();
            }

            if (DestinationRegister.HasValue)
            {
                if (ImmSource.HasValue)
                {
                    SetRegisterValue(DestinationRegister.Value, ALU_Sub(DestinationRegister.Value, ImmSource.Value, carry, out _));
                }
            }
        }
        public void Sub(Register destination, ArgumentMode source, bool carry)
        {
            if (getSourceAll(source, Size._word, out int? ImmSource))
            {
                throw new NotImplementedException();
            }

            if (ImmSource.HasValue)
            {
                SetRegisterValue(destination, ALU_Sub(destination, ImmSource.Value, carry, out _));
            }
        }
        public void Mul(ArgumentMode destination, ArgumentMode source)
        {
            if (getDestinationAll(destination, Size._byte, out Register? DestinationRegister, out Address? DestinationAddress))
            {
                throw new NotImplementedException();
            }
            if (getSourceAll(source, Size._word, out int? ImmSource))
            {
                throw new NotImplementedException();
            }

            if (DestinationRegister.HasValue)
            {
                if (ImmSource.HasValue)
                {
                    SetRegisterValue(DestinationRegister.Value, ALU_Mul(DestinationRegister.Value, ImmSource.Value, out _));
                }
            }
        }
        public void Mul(Register destination, ArgumentMode source)
        {
            if (getSourceAll(source, Size._word, out int? ImmSource))
            {
                throw new NotImplementedException();
            }

            if (ImmSource.HasValue)
            {
                SetRegisterValue(destination, ALU_Mul(destination, ImmSource.Value, out _));
            }
        }
        public void Div(ArgumentMode destination, ArgumentMode source)
        {
            if (getDestinationAll(destination, Size._byte, out Register? DestinationRegister, out Address? DestinationAddress))
            {
                throw new NotImplementedException();
            }
            if (getSourceAll(source, Size._word, out int? ImmSource))
            {
                throw new NotImplementedException();
            }

            if (DestinationRegister.HasValue)
            {
                if (ImmSource.HasValue)
                {
                    SetRegisterValue(DestinationRegister.Value, ALU_Div(DestinationRegister.Value, ImmSource.Value, out _));
                }
            }
        }
        public void Div(Register destination, ArgumentMode source)
        {
            if (getSourceAll(source, Size._word, out int? ImmSource))
            {
                throw new NotImplementedException();
            }

            if (ImmSource.HasValue)
            {
                SetRegisterValue(destination, ALU_Div(destination, ImmSource.Value, out _));
            }
        }
        public void And(ArgumentMode destination, ArgumentMode source)
        {
            if (getDestinationAll(destination, Size._byte, out Register? DestinationRegister, out Address? DestinationAddress))
            {
                throw new NotImplementedException();
            }
            if (getSourceAll(source, Size._word, out int? ImmSource))
            {
                throw new NotImplementedException();
            }

            if (DestinationRegister.HasValue)
            {
                if (ImmSource.HasValue)
                {
                    SetRegisterValue(DestinationRegister.Value, ALU_And(DestinationRegister.Value, ImmSource.Value, out _));
                }
            }
        }
        public void And(Register destination, ArgumentMode source)
        {
            if (getSourceAll(source, Size._word, out int? ImmSource))
            {
                throw new NotImplementedException();
            }

            if (ImmSource.HasValue)
            {
                SetRegisterValue(destination, ALU_And(destination, ImmSource.Value, out _));
            }
        }
        public void Or(ArgumentMode destination, ArgumentMode source)
        {
            if (getDestinationAll(destination, Size._byte, out Register? DestinationRegister, out Address? DestinationAddress))
            {
                throw new NotImplementedException();
            }
            if (getSourceAll(source, Size._word, out int? ImmSource))
            {
                throw new NotImplementedException();
            }

            if (DestinationRegister.HasValue)
            {
                if (ImmSource.HasValue)
                {
                    SetRegisterValue(DestinationRegister.Value, ALU_Or(DestinationRegister.Value, ImmSource.Value, out _));
                }
            }
        }
        public void Or(Register destination, ArgumentMode source)
        {
            if (getSourceAll(source, Size._word, out int? ImmSource))
            {
                throw new NotImplementedException();
            }

            if (ImmSource.HasValue)
            {
                SetRegisterValue(destination, ALU_Or(destination, ImmSource.Value, out _));
            }
        }
        public void Nor(ArgumentMode destination, ArgumentMode source)
        {
            if (getDestinationAll(destination, Size._byte, out Register? DestinationRegister, out Address? DestinationAddress))
            {
                throw new NotImplementedException();
            }
            if (getSourceAll(source, Size._word, out int? ImmSource))
            {
                throw new NotImplementedException();
            }

            if (DestinationRegister.HasValue)
            {
                if (ImmSource.HasValue)
                {
                    SetRegisterValue(DestinationRegister.Value, ALU_Nor(DestinationRegister.Value, ImmSource.Value, out _));
                }
            }
        }
        public void Nor(Register destination, ArgumentMode source)
        {
            if (getSourceAll(source, Size._word, out int? ImmSource))
            {
                throw new NotImplementedException();
            }

            if (ImmSource.HasValue)
            {
                SetRegisterValue(destination, ALU_Nor(destination, ImmSource.Value, out _));
            }
        }
        public void Xor(ArgumentMode destination, ArgumentMode source)
        {
            if (getDestinationAll(destination, Size._byte, out Register? DestinationRegister, out Address? DestinationAddress))
            {
                throw new NotImplementedException();
            }
            if (getSourceAll(source, Size._word, out int? ImmSource))
            {
                throw new NotImplementedException();
            }

            if (DestinationRegister.HasValue)
            {
                if (ImmSource.HasValue)
                {
                    SetRegisterValue(DestinationRegister.Value, ALU_Xor(DestinationRegister.Value, ImmSource.Value, out _));
                }
            }
        }
        public void Xor(Register destination, ArgumentMode source)
        {
            if (getSourceAll(source, Size._word, out int? ImmSource))
            {
                throw new NotImplementedException();
            }

            if (ImmSource.HasValue)
            {
                SetRegisterValue(destination, ALU_Xor(destination, ImmSource.Value, out _));
            }
        }
        public void Not(ArgumentMode destination)
        {
            if (getDestinationAll(destination, Size._byte, out Register? DestinationRegister, out Address? DestinationAddress))
            {
                throw new NotImplementedException();
            }

            if (DestinationRegister.HasValue)
            {
                SetRegisterValue(DestinationRegister.Value, ALU_Not(DestinationRegister.Value, out _));
            }
        }
        public void Not(Register destination)
        {
            SetRegisterValue(destination, ALU_Not(destination, out _));
        }
        public void Shl(ArgumentMode destination, ArgumentMode operand1)
        {
            if (getSourceAll(operand1, Size._word, out int? ImmSource))
            {
                throw new NotImplementedException();
            }
            if (getDestinationAll(destination, Size._byte, out Register? DestinationRegister, out Address? DestinationAddress))
            {
                throw new NotImplementedException();
            }

            if (DestinationRegister.HasValue)
            {
                if (ImmSource.HasValue)
                {
                    int Register = GetRegisterValue(DestinationRegister.Value);
                    Register <<= ImmSource.Value;
                    SetRegisterValue(DestinationRegister.Value, Register);
                }
            }
        }
        public void Shr(ArgumentMode destination, ArgumentMode operand1)
        {
            if (getSourceAll(operand1, Size._word, out int? ImmSource))
            {
                throw new NotImplementedException();
            }
            if (getDestinationAll(destination, Size._byte, out Register? DestinationRegister, out Address? DestinationAddress))
            {
                throw new NotImplementedException();
            }

            if (DestinationRegister.HasValue)
            {
                if (ImmSource.HasValue)
                {
                    int Register = GetRegisterValue(DestinationRegister.Value);
                    Register >>= ImmSource.Value;
                    SetRegisterValue(DestinationRegister.Value, Register);
                }
            }
        }
        public void Rol(ArgumentMode destination, ArgumentMode operand1)
        {
            if (getSourceAll(operand1, Size._word, out int? ImmSource))
            {
                throw new NotImplementedException();
            }
            if (getDestinationAll(destination, Size._byte, out Register? DestinationRegister, out Address? DestinationAddress))
            {
                throw new NotImplementedException();
            }

            if (DestinationRegister.HasValue)
            {
                if (ImmSource.HasValue)
                {
                    int Register = GetRegisterValue(DestinationRegister.Value);
                    Register <<= ImmSource.Value;
                    Register |= 0x0001;
                    SetRegisterValue(DestinationRegister.Value, Register);
                }
            }
        }
        public void Ror(ArgumentMode destination, ArgumentMode operand1)
        {
            if (getSourceAll(operand1, Size._word, out int? ImmSource))
            {
                throw new NotImplementedException();
            }
            if (getDestinationAll(destination, Size._byte, out Register? DestinationRegister, out Address? DestinationAddress))
            {
                throw new NotImplementedException();
            }

            if (DestinationRegister.HasValue)
            {
                if (ImmSource.HasValue)
                {
                    int Register = GetRegisterValue(DestinationRegister.Value);
                    Register >>= ImmSource.Value;
                    Register |= 0x8000;
                    SetRegisterValue(DestinationRegister.Value, Register);
                }
            }
        }
        public void Inc(ArgumentMode destination)
        {
            if (getDestinationAll(destination, Size._byte, out Register? DestinationRegister, out Address? DestinationAddress))
            {
                throw new NotImplementedException();
            }

            if (DestinationRegister.HasValue)
            {
                SetRegisterValue(DestinationRegister.Value, ALU_Add(DestinationRegister.Value, 1, false, out int _));
            }
        }
        public void Dec(ArgumentMode destination)
        {
            if (getDestinationAll(destination, Size._byte, out Register? DestinationRegister, out Address? DestinationAddress))
            {
                throw new NotImplementedException();
            }

            if (DestinationRegister.HasValue)
            {
                SetRegisterValue(DestinationRegister.Value, ALU_Sub(DestinationRegister.Value, 1, false, out _));
            }
        }
        public void Neg(ArgumentMode destination)
        {
            if (getDestinationAll(destination, Size._byte, out Register? DestinationRegister, out Address? DestinationAddress))
            {
                throw new NotImplementedException();
            }

            if (DestinationRegister.HasValue)
            {
                SetRegisterValue(DestinationRegister.Value, ALU_Neg(DestinationRegister.Value, out _));
            }
        }
        public void Exp(ArgumentMode destination, ArgumentMode operand1)
        {
            if (getSourceAll(operand1, Size._word, out int? ImmSource))
            {
                throw new NotImplementedException();
            }
            if (getDestinationAll(destination, Size._byte, out Register? DestinationRegister, out Address? DestinationAddress))
            {
                throw new NotImplementedException();
            }

            if (DestinationRegister.HasValue)
            {
                if (ImmSource.HasValue)
                {
                    int Register = GetRegisterValue(DestinationRegister.Value);
                    Register = (int)MathF.Pow(Register, ImmSource.Value);
                    SetRegisterValue(DestinationRegister.Value, Register);
                }
            }
        }
        public void Sqrt(ArgumentMode destination)
        {
            if (getDestinationAll(destination, Size._byte, out Register? DestinationRegister, out Address? DestinationAddress))
            {
                throw new NotImplementedException();
            }

            if (DestinationRegister.HasValue)
            {
                int Register = GetRegisterValue(DestinationRegister.Value);
                Register = (int)MathF.Sqrt(Register);
                SetRegisterValue(DestinationRegister.Value, Register);
            }
        }
        public void Rng(ArgumentMode destination)
        {
            if (getDestinationAll(destination, Size._byte, out Register? DestinationRegister, out Address? DestinationAddress))
            {
                throw new NotImplementedException();
            }

            if (DestinationRegister.HasValue)
            {
                SetRegisterValue(DestinationRegister.Value, ALU_RNG());
            }
        }
        public void Mod(ArgumentMode destination, ArgumentMode operand1)
        {
            if (getSourceAll(operand1, Size._word, out int? ImmSource))
            {
                throw new NotImplementedException();
            }
            if (getDestinationAll(destination, Size._byte, out Register? DestinationRegister, out Address? DestinationAddress))
            {
                throw new NotImplementedException();
            }

            if (DestinationRegister.HasValue)
            {
                if (ImmSource.HasValue)
                {
                    int Register = GetRegisterValue(DestinationRegister.Value);
                    Register %= ImmSource.Value;
                    SetRegisterValue(DestinationRegister.Value, Register);
                }
            }
        }

        public void Out(Size InstructionSize, ArgumentMode port, ArgumentMode source)
        {
            if (getSourceAll(port, InstructionSize, out int? ImmPort))
            {
                throw new NotImplementedException();
            }
            if (getSourceAll(source, InstructionSize, out int? ImmSource))
            {
                throw new NotImplementedException();
            }

            if (ImmPort.HasValue)
            {
                if (ImmSource.HasValue)
                {
                    switch (source)
                    {
                        case ArgumentMode.immediate_byte:
                        case ArgumentMode.register:
                        case ArgumentMode.register_AL:
                        case ArgumentMode.register_BL:
                        case ArgumentMode.register_CL:
                        case ArgumentMode.register_DL:
                            m_BUS.OutPort(ImmPort.Value, (byte)ImmSource.Value);
                            break;
                        case ArgumentMode.immediate_word:
                        case ArgumentMode.register_A:
                        case ArgumentMode.register_B:
                        case ArgumentMode.register_C:
                        case ArgumentMode.register_D:
                        case ArgumentMode.register_H:
                        case ArgumentMode.register_L:
                            m_BUS.OutPort(ImmPort.Value, (ushort)ImmSource.Value);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        public void Inp(Size InstructionSize, ArgumentMode port, ArgumentMode destination)
        {
            if (getSourceAll(port, InstructionSize, out int? ImmPort))
            {
                throw new NotImplementedException();
            }
            if (getDestinationAll(destination, InstructionSize, out Register? DestinationRegister, out Address? DestinationAddress))
            {
                throw new NotImplementedException();
            }

            if (DestinationRegister.HasValue)
            {
                if (ImmPort.HasValue)
                {
                    Size size = (Size)GetRegisterSize(DestinationRegister.Value);
                    int result;
                    switch (size)
                    {
                        case Size._byte:
                            result = m_BUS.InPort(ImmPort.Value, out byte _);
                            break;
                        case Size._word:
                        default:
                            result = m_BUS.InPort(ImmPort.Value, out ushort _);
                            break;
                    }
                    SetRegisterValue(DestinationRegister.Value, result);
                }
            }
        }
        public void Jump(ArgumentMode Address)
        {
            if (getDestinationAll(Address, Size._word, out Register? _, out Address? DestinationAddress))
            {
                throw new NotImplementedException();
            }

            if (DestinationAddress.HasValue)
            {
                Jump(DestinationAddress.Value);
            }
        }
        public void JumpC(ArgumentMode Address, int flag, int value)
        {
            if (getDestinationAll(Address, Size._word, out Register? _, out Address? DestinationAddress))
            {
                throw new NotImplementedException();
            }

            if (GetFlag(flag) == Convert.ToBoolean(value))
            {
                if (DestinationAddress.HasValue)
                {
                    Jump(DestinationAddress.Value);
                }
            }
        }
        public void JumpC(ArgumentMode Address, int flag1, int flag2, int value1, int value2)
        {
            if (getDestinationAll(Address, Size._word, out Register? _, out Address? DestinationAddress))
            {
                throw new NotImplementedException();
            }

            if (GetFlag(flag1) == Convert.ToBoolean(value1))
            {
                if (GetFlag(flag2) == Convert.ToBoolean(value2))
                {
                    if (DestinationAddress.HasValue)
                    {
                        Jump(DestinationAddress.Value);
                    }
                }
            }
        }
        public void CBTA(ArgumentMode register, ArgumentMode address)
        {
            if (getSourceAll(register, Size._word, out int? ImmSource))
            {
                throw new NotImplementedException();
            }
            if (getDestinationAll(address, Size._word, out Register? _, out Address? DestinationAddress))
            {
                throw new NotImplementedException();
            }

            if (ImmSource.HasValue)
            {
                if (DestinationAddress.HasValue)
                {
                    string hexNumber = Convert.ToString(ImmSource.Value, 16);
                    for (int i = 0; i < hexNumber.Length; i++)
                    {
                        byte AsciiCode = Encoding.ASCII.GetBytes(hexNumber[i].ToString())[0];
                        Address addr = DestinationAddress.Value + i;
                        m_BUS.m_Memory.WriteByte(addr, AsciiCode);
                    }
                    SetRegisterValue(Register.C, hexNumber.Length);
                }
            }
        }
        public void Cmpl()
        {
            int address1 = GetRegisterValue(Register.HL);
            int address2 = GetSegment(Register.DS, Register.B);
            int times = m_CX[false];

            bool[] bools = new bool[times];
            for (int i = 0; i < times; i++)
            {
                byte value1 = m_BUS.m_Memory.ReadByte(address1);
                byte value2 = m_BUS.m_Memory.ReadByte(address2);
                bools[i] = value1 == value2;
            }

            SetFlag(FL_E, true);
            for (int i = 0; i < bools.Length; i++)
            {
                if (bools[i] == false)
                {
                    SetFlag(FL_E, false);
                    break;
                }
            }
        }
        public void Reti()
        {
            Pop(Register.F);
            PopPC();
            Popr();

            if ((m_CR0 & CR0_EnableExtendedMode) == CR0_EnableExtendedMode)
            {
                Pop(Register.X);
                Pop(Register.Y);
            }
            else
            {
            }

            Pop(Register.SP);
            Pop(Register.BP);
            Pop(Register.DS);
            Pop(Register.SS);
        }
        public void Int(ArgumentMode source)
        {
            if (getSourceAll(source, Size._byte, out int? ImmSource))
            {
                throw new NotImplementedException();
            }

            if(ImmSource.HasValue)
            {
                Interrupt((byte)ImmSource.Value);
            }
        }

        bool getDestination(ArgumentMode Destination, ArgumentType type, out object result)
        {
            Register Segment;
            Register Offset;
            result = default;
            int reg;
            switch (Destination)
            {
                case ArgumentMode.register:
                    if (type != ArgumentType.Register)
                    {
                        return false;
                    }
                    Register register = (Register)FetchByte();
                    result = register;
                    return true;
                case ArgumentMode.register_address:
                    if (type != ArgumentType.Address)
                    {
                        return false;
                    }
                    Register registerAddress = (Register)FetchByte();
                    result = (Address)GetRegisterValue(registerAddress);
                    return true;

                case ArgumentMode.near_address:
                    if (type != ArgumentType.Address)
                    {
                        return false;
                    }
                    Address addressNear = FetchByte();
                    result = addressNear;
                    return true;
                case ArgumentMode.short_address:
                    if (type != ArgumentType.Address)
                    {
                        return false;
                    }
                    Address address = FetchWord();
                    result = address;
                    return true;
                case ArgumentMode.long_address:
                    if (type != ArgumentType.Address)
                    {
                        return false;
                    }
                    Address addressLong = FetchTByte();
                    result = addressLong;
                    return true;
                case ArgumentMode.far_address:
                    if (type != ArgumentType.Address)
                    {
                        return false;
                    }
                    Address addressFar = FetchDWord();
                    result = addressFar;
                    return true;
                case ArgumentMode.relative_address:
                    if (type != ArgumentType.Address)
                    {
                        return false;
                    }
                    Address relativeAddress = FetchByte();
                    result = (Address)(m_PC + (relativeAddress - 1));
                    return true;

                case ArgumentMode.segment_address:
                    if (type != ArgumentType.SegmentAddress)
                    {
                        return false;
                    }

                    Segment = (Register)FetchByte();
                    Offset = (Register)FetchByte();

                    result = GetSegment(Segment, Offset);
                    return true;
                case ArgumentMode.segment_DS_register:
                    if (type != ArgumentType.SegmentAddress)
                    {
                        return false;
                    }

                    Offset = (Register)FetchByte();

                    result = GetSegment(Register.DS, Offset);
                    return true;
                case ArgumentMode.segment_DS_B:
                    if (type != ArgumentType.SegmentAddress)
                    {
                        return false;
                    }

                    result = GetSegment(Register.DS, Register.B);
                    return true;
                case ArgumentMode.segment_ES_register:
                    if (type != ArgumentType.SegmentAddress)
                    {
                        return false;
                    }

                    Offset = (Register)FetchByte();

                    result = GetSegment(Register.ES, Offset);
                    return true;
                case ArgumentMode.segment_ES_B:
                    if (type != ArgumentType.SegmentAddress)
                    {
                        return false;
                    }

                    result = GetSegment(Register.ES, Register.B);
                    return true;

                case ArgumentMode.register_AL:
                    if (type != ArgumentType.Register)
                    {
                        return false;
                    }
                    result = Register.AL;
                    return true;
                case ArgumentMode.register_BL:
                    if (type != ArgumentType.Register)
                    {
                        return false;
                    }
                    result = Register.BL;
                    return true;
                case ArgumentMode.register_CL:
                    if (type != ArgumentType.Register)
                    {
                        return false;
                    }
                    result = Register.CL;
                    return true;
                case ArgumentMode.register_DL:
                    if (type != ArgumentType.Register)
                    {
                        return false;
                    }
                    result = Register.DL;
                    return true;
                case ArgumentMode.register_A:
                    if (type != ArgumentType.Register)
                    {
                        return false;
                    }
                    result = Register.A;
                    return true;
                case ArgumentMode.register_B:
                    if (type != ArgumentType.Register)
                    {
                        return false;
                    }
                    result = Register.B;
                    return true;
                case ArgumentMode.register_C:
                    if (type != ArgumentType.Register)
                    {
                        return false;
                    }
                    result = Register.C;
                    return true;
                case ArgumentMode.register_D:
                    if (type != ArgumentType.Register)
                    {
                        return false;
                    }
                    result = Register.D;
                    return true;
                case ArgumentMode.register_L:
                    if (type != ArgumentType.Register)
                    {
                        return false;
                    }
                    result = Register.L;
                    return true;
                case ArgumentMode.register_H:
                    if (type != ArgumentType.Register)
                    {
                        return false;
                    }
                    result = Register.H;
                    return true;
                case ArgumentMode.register_AX:
                    if (type != ArgumentType.Register)
                    {
                        return false;
                    }
                    result = Register.AX;
                    return true;
                case ArgumentMode.register_BX:
                    if (type != ArgumentType.Register)
                    {
                        return false;
                    }
                    result = Register.BX;
                    return true;
                case ArgumentMode.register_CX:
                    if (type != ArgumentType.Register)
                    {
                        return false;
                    }
                    result = Register.CX;
                    return true;
                case ArgumentMode.register_DX:
                    if (type != ArgumentType.Register)
                    {
                        return false;
                    }
                    result = Register.DX;
                    return true;
                case ArgumentMode.register_AF:
                    if (type != ArgumentType.Register)
                    {
                        return false;
                    }
                    result = Register.AF;
                    return true;
                case ArgumentMode.register_BF:
                    if (type != ArgumentType.Register)
                    {
                        return false;
                    }
                    result = Register.BF;
                    return true;
                case ArgumentMode.register_CF:
                    if (type != ArgumentType.Register)
                    {
                        return false;
                    }
                    result = Register.CF;
                    return true;
                case ArgumentMode.register_DF:
                    if (type != ArgumentType.Register)
                    {
                        return false;
                    }
                    result = Register.DF;
                    return true;
                case ArgumentMode.register_address_HL:
                    if (type != ArgumentType.Address)
                    {
                        return false;
                    }
                    result = (Address)GetRegisterValue(Register.HL);
                    return true;

                case ArgumentMode.BP_rel_address_byte:
                    if (type != ArgumentType.Address)
                    {
                        return false;
                    }
                    reg = GetRegisterValue(Register.BP);
                    sbyte offset = (sbyte)FetchByte();
                    reg += offset + 1;
                    result = GetSegment(Register.SS, reg);
                    return true;
                case ArgumentMode.SP_rel_address_byte:
                    if (type != ArgumentType.Address)
                    {
                        return false;
                    }
                    reg = GetRegisterValue(Register.SP);
                    offset = (sbyte)FetchByte();
                    reg += offset;
                    result = GetSegment(Register.SS, reg);
                    return true;
                case ArgumentMode.SP_rel_address_short:
                    if (type != ArgumentType.Address)
                    {
                        return false;
                    }
                    reg = GetRegisterValue(Register.SP);
                    short offsetS = (short)FetchWord();
                    reg += offsetS;
                    result = GetSegment(Register.SS, reg);
                    return true;
                case ArgumentMode.BP_rel_address_short:
                    if (type != ArgumentType.Address)
                    {
                        return false;
                    }
                    reg = GetRegisterValue(Register.BP);
                    offsetS = (short)FetchWord();
                    reg += offsetS;
                    result = GetSegment(Register.SS, reg);
                    return true;
                case ArgumentMode.X_indexed_address:
                    if (type != ArgumentType.Address)
                    {
                        return false;
                    }

                    address = FetchWord();
                    address += m_X;
                    result = address;
                    return true;
                case ArgumentMode.Y_indexed_address:
                    if (type != ArgumentType.Address)
                    {
                        return false;
                    }

                    address = FetchWord();
                    address += m_Y;
                    result = address;
                    return true;
                case ArgumentMode.None:
                    break;
                default:
                    break;
            }

            result = default;
            return false;
        }
        bool getSource(ArgumentMode source, ArgumentType type, out object result)
        {
            Register Segment;
            Register Offset;
            result = default;
            int reg;
            switch (source)
            {
                case ArgumentMode.immediate_byte:
                case ArgumentMode.immediate_word:
                case ArgumentMode.immediate_tbyte:
                case ArgumentMode.immediate_dword:
                case ArgumentMode.immediate_qword:
                    if (type != ArgumentType.imm)
                    {
                        return false;
                    }

                    switch (source)
                    {
                        case ArgumentMode.immediate_byte:
                            result = FetchByte();
                            break;
                        case ArgumentMode.immediate_word:
                            result = FetchWord();
                            break;
                        case ArgumentMode.immediate_tbyte:
                            result = FetchTByte();
                            break;
                        case ArgumentMode.immediate_dword:
                            result = FetchDWord();
                            break;
                        case ArgumentMode.immediate_qword:
                            result = FetchQWord();
                            break;
                        default:
                            return false;
                    }

                    return true;

                case ArgumentMode.immediate_float:
                    if (type != ArgumentType.Float)
                    {
                        return false;
                    }

                    result = FetchFloat();
                    return true;

                case ArgumentMode.register:
                    if (type != ArgumentType.Register)
                    {
                        return false;
                    }
                    Register register = (Register)FetchByte();
                    result = register;
                    return true;
                case ArgumentMode.register_address:
                    if (type != ArgumentType.Address)
                    {
                        return false;
                    }
                    Register registerAddress = (Register)FetchByte();
                    result = (Address)GetRegisterValue(registerAddress);
                    return true;

                case ArgumentMode.near_address:
                    if (type != ArgumentType.Address)
                    {
                        return false;
                    }
                    Address addressNear = FetchByte();
                    result = addressNear;
                    return true;
                case ArgumentMode.short_address:
                    if (type != ArgumentType.Address)
                    {
                        return false;
                    }
                    Address address = FetchWord();
                    result = address;
                    return true;
                case ArgumentMode.long_address:
                    if (type != ArgumentType.Address)
                    {
                        return false;
                    }
                    Address addressLong = FetchTByte();
                    result = addressLong;
                    return true;
                case ArgumentMode.far_address:
                    if (type != ArgumentType.Address)
                    {
                        return false;
                    }
                    Address addressFar = FetchDWord();
                    result = addressFar;
                    return true;
                case ArgumentMode.relative_address:
                    if (type != ArgumentType.Address)
                    {
                        return false;
                    }
                    Address relativeAddress = FetchByte();
                    result = (Address)(m_PC + (relativeAddress - m_argumentOffset));
                    return true;

                case ArgumentMode.segment_address:
                    if (type != ArgumentType.SegmentAddress)
                    {
                        return false;
                    }

                    Segment = (Register)FetchByte();
                    Offset = (Register)FetchByte();

                    result = GetSegment(Segment, Offset);
                    return true;
                case ArgumentMode.segment_DS_register:
                    if (type != ArgumentType.SegmentAddress)
                    {
                        return false;
                    }

                    Offset = (Register)FetchByte();

                    result = GetSegment(Register.DS, Offset);
                    return true;
                case ArgumentMode.segment_DS_B:
                    if (type != ArgumentType.SegmentAddress)
                    {
                        return false;
                    }

                    result = GetSegment(Register.DS, Register.B);
                    return true;
                case ArgumentMode.segment_ES_register:
                    if (type != ArgumentType.SegmentAddress)
                    {
                        return false;
                    }

                    Offset = (Register)FetchByte();

                    result = GetSegment(Register.ES, Offset);
                    return true;
                case ArgumentMode.segment_ES_B:
                    if (type != ArgumentType.SegmentAddress)
                    {
                        return false;
                    }

                    result = GetSegment(Register.ES, Register.B);
                    return true;

                case ArgumentMode.register_AL:
                    if (type != ArgumentType.Register)
                    {
                        return false;
                    }
                    result = Register.AL;
                    return true;
                case ArgumentMode.register_BL:
                    if (type != ArgumentType.Register)
                    {
                        return false;
                    }
                    result = Register.BL;
                    return true;
                case ArgumentMode.register_CL:
                    if (type != ArgumentType.Register)
                    {
                        return false;
                    }
                    result = Register.CL;
                    return true;
                case ArgumentMode.register_DL:
                    if (type != ArgumentType.Register)
                    {
                        return false;
                    }
                    result = Register.DL;
                    return true;
                case ArgumentMode.register_A:
                    if (type != ArgumentType.Register)
                    {
                        return false;
                    }
                    result = Register.A;
                    return true;
                case ArgumentMode.register_B:
                    if (type != ArgumentType.Register)
                    {
                        return false;
                    }
                    result = Register.B;
                    return true;
                case ArgumentMode.register_C:
                    if (type != ArgumentType.Register)
                    {
                        return false;
                    }
                    result = Register.C;
                    return true;
                case ArgumentMode.register_D:
                    if (type != ArgumentType.Register)
                    {
                        return false;
                    }
                    result = Register.D;
                    return true;
                case ArgumentMode.register_L:
                    if (type != ArgumentType.Register)
                    {
                        return false;
                    }
                    result = Register.L;
                    return true;
                case ArgumentMode.register_H:
                    if (type != ArgumentType.Register)
                    {
                        return false;
                    }
                    result = Register.H;
                    return true;
                case ArgumentMode.register_AX:
                    if (type != ArgumentType.Register)
                    {
                        return false;
                    }
                    result = Register.AX;
                    return true;
                case ArgumentMode.register_BX:
                    if (type != ArgumentType.Register)
                    {
                        return false;
                    }
                    result = Register.BX;
                    return true;
                case ArgumentMode.register_CX:
                    if (type != ArgumentType.Register)
                    {
                        return false;
                    }
                    result = Register.CX;
                    return true;
                case ArgumentMode.register_DX:
                    if (type != ArgumentType.Register)
                    {
                        return false;
                    }
                    result = Register.DX;
                    return true;
                case ArgumentMode.register_AF:
                    if (type != ArgumentType.Register)
                    {
                        return false;
                    }
                    result = Register.AF;
                    return true;
                case ArgumentMode.register_BF:
                    if (type != ArgumentType.Register)
                    {
                        return false;
                    }
                    result = Register.BF;
                    return true;
                case ArgumentMode.register_CF:
                    if (type != ArgumentType.Register)
                    {
                        return false;
                    }
                    result = Register.CF;
                    return true;
                case ArgumentMode.register_DF:
                    if (type != ArgumentType.Register)
                    {
                        return false;
                    }
                    result = Register.DF;
                    return true;
                case ArgumentMode.register_address_HL:
                    if (type != ArgumentType.Address)
                    {
                        return false;
                    }
                    result = (Address)GetRegisterValue(Register.HL);
                    return true;

                case ArgumentMode.BP_rel_address_byte:
                    if (type != ArgumentType.Address)
                    {
                        return false;
                    }
                    reg = GetRegisterValue(Register.BP);
                    sbyte offset = (sbyte)FetchByte();
                    reg += offset + 1;
                    result = GetSegment(Register.SS, reg);
                    return true;
                case ArgumentMode.SP_rel_address_byte:
                    if (type != ArgumentType.Address)
                    {
                        return false;
                    }
                    reg = GetRegisterValue(Register.SP);
                    offset = (sbyte)FetchByte();
                    reg += offset;
                    result = GetSegment(Register.SS, reg);
                    return true;
                case ArgumentMode.SP_rel_address_short:
                    if (type != ArgumentType.Address)
                    {
                        return false;
                    }
                    reg = GetRegisterValue(Register.SP);
                    short offsetS = (short)FetchWord();
                    reg += offsetS;
                    result = GetSegment(Register.SS, reg);
                    return true;
                case ArgumentMode.BP_rel_address_short:
                    if (type != ArgumentType.Address)
                    {
                        return false;
                    }
                    reg = GetRegisterValue(Register.BP);
                    offsetS = (short)FetchWord();
                    reg += offsetS;
                    result = GetSegment(Register.SS, reg);
                    return true;
                case ArgumentMode.X_indexed_address:
                    if (type != ArgumentType.Address)
                    {
                        return false;
                    }

                    address = FetchWord();
                    address += m_X;
                    result = address;
                    return true;
                case ArgumentMode.Y_indexed_address:
                    if (type != ArgumentType.Address)
                    {
                        return false;
                    }

                    address = FetchWord();
                    address += m_Y;
                    result = address;
                    return true;
                default:
                    break;
            }

            result = default;
            return false;
        }
        bool getDestinationAll(ArgumentMode destination, Size InstructionSize, out Register? DestinationRegister, out Address? DestinationAddress)
        {
            object result;

            object DataDestination = null;
            DestinationRegister = null;
            DestinationAddress = null;

            if (getDestination(destination, ArgumentType.Register, out result))
            {
                Register register = (Register)result;
                DestinationRegister = register;
                DataDestination = register;
            }
            else if (getDestination(destination, ArgumentType.Address, out result))
            {
                Address address = (Address)result;
                DataDestination = result;
                DestinationAddress = (Address)address;
            }
            else if (getDestination(destination, ArgumentType.SegmentAddress, out result))
            {
                Address address = (Address)result;
                DataDestination = result;
                DestinationAddress = address;
            }


            return DataDestination == null;
        }
        bool getSourceAll(ArgumentMode source, Size InstructionSize, out int? ImmSource)
        {
            object result;

            ImmSource = null;
            object DataSource = null;

            if (getSource(source, ArgumentType.imm, out result))
            {
                DataSource = result;
                ImmSource = null;
                switch (source)
                {
                    case ArgumentMode.immediate_byte:
                        byte Bresult = (byte)result;
                        ImmSource = Bresult;
                        break;
                    case ArgumentMode.immediate_word:
                        ushort Wresult = (ushort)result;
                        ImmSource = Wresult;
                        break;
                    case ArgumentMode.immediate_tbyte:
                        uint Tresult = (uint)result & 0x00FF_FFFF;
                        ImmSource = (int)Tresult;
                        break;
                    case ArgumentMode.immediate_dword:
                        int Dresult = (int)result;
                        ImmSource = Dresult;
                        break;
                }
            }
            else if (getSource(source, ArgumentType.Address, out result))
            {
                Address address = (Address)result;
                DataSource = result;
                switch (InstructionSize)
                {
                    case Size._byte:
                        ImmSource = m_BUS.m_Memory.ReadByte(address);
                        break;
                    case Size._word:
                        ImmSource = m_BUS.m_Memory.ReadWord(address);
                        break;
                    case Size._tbyte:
                        ImmSource = (int)m_BUS.m_Memory.ReadTByte(address);
                        break;
                    case Size._dword:
                        ImmSource = (int)m_BUS.m_Memory.ReadDWord(address);
                        break;
                }
            }
            else if (getSource(source, ArgumentType.Register, out result))
            {
                Register register = (Register)result;
                DataSource = register;

                switch (InstructionSize)
                {
                    case Size._byte:
                        ImmSource = (byte)GetRegisterValue(register);
                        break;
                    case Size._word:
                        ImmSource = (ushort)GetRegisterValue(register);
                        break;
                    case Size._tbyte:
                        ImmSource = GetRegisterValue(register) & 0x00FF_FFFF;
                        break;
                    case Size._dword:
                        ImmSource = GetRegisterValue(register);
                        break;
                    default:
                        break;
                }
            }
            else if (getDestination(source, ArgumentType.SegmentAddress, out result))
            {
                Address address = (Address)result;
                DataSource = result;
                switch (InstructionSize)
                {
                    case Size._byte:
                        ImmSource = m_BUS.m_Memory.ReadByte(address);
                        break;
                    case Size._word:
                        ImmSource = m_BUS.m_Memory.ReadWord(address);
                        break;
                    case Size._tbyte:
                        ImmSource = (int)m_BUS.m_Memory.ReadTByte(address);
                        break;
                    case Size._dword:
                        ImmSource = (int)m_BUS.m_Memory.ReadDWord(address);
                        break;
                }
            }

            return DataSource == null;
        }
    }
}
