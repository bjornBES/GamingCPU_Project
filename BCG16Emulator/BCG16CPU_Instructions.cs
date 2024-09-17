using CommonBCGCPU.Types;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using System.ComponentModel;

namespace BCG16CPUEmulator
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
    public class BCG16CPU_Instructions : BCG16CPU_Interrupts
    {
        public void Move(Size InstructionSize, ArgumentMode destination, ArgumentMode source)
        {
            if (GetDestinationAll(destination, InstructionSize, out Register? DestinationRegister, out Address? DestinationAddress))
            {
                throw new NotImplementedException();
            }
            if (GetSourceAll(source, InstructionSize, out int? ImmSource))
            {
                throw new NotImplementedException();
            }

            if (DestinationRegister.HasValue)
            {
                if (ImmSource.HasValue)
                {
                    SetRegisterValue(DestinationRegister.Value, ImmSource.Value);
                }
                Compare(DestinationRegister.Value, DestinationRegister.Value);
            }
            else if (DestinationAddress.HasValue)
            {
                if (ImmSource.HasValue)
                {
                    switch (InstructionSize)
                    {
                        case Size._byte:
                            m_BUS.m_Memory.WriteByte(DestinationAddress.Value, (byte)ImmSource.Value);
                            break;
                        case Size._word:
                            m_BUS.m_Memory.WriteWord(DestinationAddress.Value, (ushort)ImmSource.Value);
                            break;
                        case Size._tbyte:
                            m_BUS.m_Memory.WriteTByte(DestinationAddress.Value, (uint)ImmSource.Value);
                            break;
                        case Size._dword:
                            m_BUS.m_Memory.WriteDWord(DestinationAddress.Value, (uint)ImmSource.Value);
                            break;
                        default:
                            break;
                    }
                }
            }


        }
        public void Move(Size InstructionSize, Register destination, ArgumentMode source)
        {
            if (GetSourceAll(source, InstructionSize, out int? ImmSource))
            {
                throw new NotImplementedException();
            }

            if (ImmSource.HasValue)
            {
                SetRegisterValue(destination, ImmSource.Value);
                Compare(Register.AL, Register.AL);
            }
        }
        public void Cmp(ArgumentMode operand1, ArgumentMode operand2)
        {
            if (GetSourceAll(operand1, Size._word, out int? ImmSource1))
            {
                throw new NotImplementedException();
            }
            if (GetSourceAll(operand2, Size._word, out int? ImmSource2))
            {
                throw new NotImplementedException();
            }

            if (ImmSource1.HasValue)
            {
                if (ImmSource2.HasValue)
                {
                    Compare(ImmSource1.Value, ImmSource2.Value);
                }
            }
        }
        public void Push(Size size, ArgumentMode source)
        {
            int? ImmSource = null;
            Register? DestinationRegister = null;
            if (!GetDestinationAll(source, size, out DestinationRegister, out _))
            {
            }
            else if (!GetSourceAll(source, size, out ImmSource))
            {
            }

            if (DestinationRegister.HasValue)
            {
                Push(DestinationRegister.Value);
            }

            if (ImmSource.HasValue)
            {
                Push(ImmSource.Value, source);
            }
        }
        public void Pop(Size InstructionSize, ArgumentMode destination)
        {
            if (GetDestinationAll(destination, Size._byte, out Register? DestinationRegister, out Address? DestinationAddress))
            {
                throw new NotImplementedException();
            }

            if (DestinationRegister.HasValue)
            {
                switch (InstructionSize)
                {
                    case Size._byte:
                        byte popResult = Pop(out byte _);
                        SetRegisterValue(DestinationRegister.Value, popResult);
                        break;
                    case Size._word:
                        ushort WpopResult = Pop(out ushort _);
                        SetRegisterValue(DestinationRegister.Value, WpopResult);
                        break;
                    case Size._tbyte:
                        break;
                    case Size._dword:
                        break;
                    default:
                        break;
                }
            }
        }
        public void Call(ArgumentMode destination)
        {
            if (GetDestinationAll(destination, Size._byte, out Register? _, out Address? DestinationAddress))
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
            if (GetSourceAll(source, Size._byte, out int? ImmSource))
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
            if (GetDestinationAll(destination, Size._byte, out Register? DestinationRegister, out Address? DestinationAddress))
            {
                throw new NotImplementedException();
            }

            if (DestinationRegister.HasValue)
            {
                SetRegisterValue(DestinationRegister.Value, 0);
            }
            else if (DestinationAddress.HasValue)
            {
                m_BUS.m_Memory.WriteByte(DestinationAddress.Value, 0);
            }
        }        
        public void Test(ArgumentMode destination)
        {
            if (GetDestinationAll(destination, Size._byte, out Register? DestinationRegister, out Address? DestinationAddress))
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
                Compare(b, b);
            }
        }
        public void Add(ArgumentMode destination, ArgumentMode source)
        {
            if (GetDestinationAll(destination, Size._byte, out Register? DestinationRegister, out Address? DestinationAddress))
            {
                throw new NotImplementedException();
            }
            if (GetSourceAll(source, Size._word, out int? ImmSource))
            {
                throw new NotImplementedException();
            }

            if (DestinationRegister.HasValue)
            {
                if (ImmSource.HasValue)
                {
                    SetRegisterValue(DestinationRegister.Value, ALU_Add(DestinationRegister.Value, ImmSource.Value, out _));
                }
            }
        }
        public void Or(ArgumentMode destination, ArgumentMode source)
        {
            if (GetDestinationAll(destination, Size._byte, out Register? DestinationRegister, out Address? DestinationAddress))
            {
                throw new NotImplementedException();
            }
            if (GetSourceAll(source, Size._word, out int? ImmSource))
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
        public void Inc(ArgumentMode destination)
        {
            if (GetDestinationAll(destination, Size._byte, out Register? DestinationRegister, out Address? DestinationAddress))
            {
                throw new NotImplementedException();
            }

            if (DestinationRegister.HasValue)
            {
                SetRegisterValue(DestinationRegister.Value, ALU_Add(DestinationRegister.Value, 1, out int _));
            }
        }
        public void Dec(ArgumentMode destination)
        {
            if (GetDestinationAll(destination, Size._byte, out Register? DestinationRegister, out Address? DestinationAddress))
            {
                throw new NotImplementedException();
            }

            if (DestinationRegister.HasValue)
            {
                SetRegisterValue(DestinationRegister.Value, ALU_Sub(DestinationRegister.Value, 1, out _));
            }
        }
        public void Outb(Size InstructionSize, ArgumentMode port, ArgumentMode source)
        {
            if (GetSourceAll(port, Size._word, out int? ImmPort))
            {
                throw new NotImplementedException();
            }
            if (GetSourceAll(source, Size._byte, out int? ImmSource))
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
                        case ArgumentMode.register_AL:
                        case ArgumentMode.register:
                            m_BUS.OutPort(ImmPort.Value, (byte)ImmSource.Value);
                            break;
                        case ArgumentMode.immediate_word:
                        case ArgumentMode.register_A:
                            m_BUS.OutPort(ImmPort.Value, (ushort)ImmSource.Value);
                            break;
                        case ArgumentMode.None:
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        public void Inb(Size InstructionSize, ArgumentMode port, ArgumentMode destination)
        {
            if (GetSourceAll(port, InstructionSize, out int? ImmPort))
            {
                throw new NotImplementedException();
            }
            if (GetDestinationAll(destination, InstructionSize, out Register? DestinationRegister, out Address? DestinationAddress))
            {
                throw new NotImplementedException();
            }

            if (DestinationRegister.HasValue)
            {
                if (ImmPort.HasValue)
                {
                    Size size = (Size)GetRegisterSize(DestinationRegister.Value);
                    int result = 0;
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
            if (GetDestinationAll(Address, Size._word, out Register? _, out Address? DestinationAddress))
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
            if (GetDestinationAll(Address, Size._word, out Register? _, out Address? DestinationAddress))
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
        public void Rti()
        {
            Pop(Register.F);

            Pop(Register.PC);

            Pop(Register.S);
            Pop(Register.SS);
            Pop(Register.ES);
            Pop(Register.DS);
        }
        public void Pushr()
        {
            Push(Register.A);
            Push(Register.B);
            Push(Register.C);
            Push(Register.D);
            Push(Register.H);
            Push(Register.L);
        }
        public void Popr()
        {
            Pop(Register.L);
            Pop(Register.H);
            Pop(Register.D);
            Pop(Register.C);
            Pop(Register.B);
            Pop(Register.A);
        }
        public void Int(ArgumentMode source)
        {
            if (GetSourceAll(source, Size._byte, out int? ImmSource))
            {
                throw new NotImplementedException();
            }

            if(ImmSource.HasValue)
            {
                Interrupt((byte)ImmSource.Value);
            }
        }

        bool GetDestination(ArgumentMode Destination, ArgumentType type, out object result)
        {
            Register Segment;
            Register Offset;
            result = default;
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
                    result = GetRegisterValue(registerAddress);
                    return true;
                case ArgumentMode.near_address:
                    if (type != ArgumentType.Address)
                    {
                        return false;
                    }
                    Address addressNear = FetchByte();
                    result = addressNear;
                    return true;
                case ArgumentMode.address:
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
                case ArgumentMode.relative_address:
                    if (type != ArgumentType.Address)
                    {
                        return false;
                    }
                    Address relativeAddress = FetchByte();
                    result = (Address)(PC + (relativeAddress - argumentOffset));
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
                case ArgumentMode.segment_address_immediate:
                    if (type != ArgumentType.SegmentAddress)
                    {
                        return false;
                    }

                    Segment = (Register)FetchByte();
                    Address AOffset = FetchWord();

                    result = GetSegment(Segment, AOffset);
                    return true;
                case ArgumentMode.segment_DS_B:
                    if (type != ArgumentType.SegmentAddress)
                    {
                        return false;
                    }

                    result = GetSegment(Register.DS, Register.B);
                    return true;
                case ArgumentMode.register_AL:
                    if (type != ArgumentType.Register)
                    {
                        return false;
                    }
                    result = Register.AL;
                    return true;
                case ArgumentMode.register_A:
                    if (type != ArgumentType.Register)
                    {
                        return false;
                    }
                    result = Register.A;
                    return true;
                case ArgumentMode.register_HL:
                    if (type != ArgumentType.Register)
                    {
                        return false;
                    }
                    result = Register.HL;
                    return true;
                case ArgumentMode.register_address_HL:
                    if (type != ArgumentType.Address)
                    {
                        return false;
                    }
                    result = (Address)GetRegisterValue(Register.HL);
                    return true;
                case ArgumentMode.BP_Offset_Address:
                    if (type != ArgumentType.Address)
                    {
                        return false;
                    }
                    int r = GetRegisterValue(Register.BP);
                    short offset = (short)FetchWord();
                    r += offset;
                    result = GetSegment(Register.SS, r);
                    return true;
                default:
                    break;
            }

            result = default;
            return false;
        }
        bool GetSource(ArgumentMode source, ArgumentType type, out object result)
        {
            Register Segment;
            Register Offset;
            result = default;
            switch (source)
            {
                case ArgumentMode.immediate_byte:
                case ArgumentMode.immediate_word:
                case ArgumentMode.immediate_tbyte:
                case ArgumentMode.immediate_dword:
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
                        default:
                            return false;
                    }

                    return true;
                case ArgumentMode.immediate_float:
                    break;
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
                case ArgumentMode.address:
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
                case ArgumentMode.relative_address:
                    if (type != ArgumentType.Address)
                    {
                        return false;
                    }
                    Address relativeAddress = FetchByte();
                    result = (Address)(PC + (relativeAddress - argumentOffset));
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
                case ArgumentMode.segment_address_immediate:
                    if (type != ArgumentType.SegmentAddress)
                    {
                        return false;
                    }

                    Segment = (Register)FetchByte();
                    Address AOffset = FetchWord();

                    result = GetSegment(Segment, AOffset);
                    return true;
                case ArgumentMode.segment_DS_B:
                    if (type != ArgumentType.SegmentAddress)
                    {
                        return false;
                    }

                    result = GetSegment(Register.DS, Register.B);
                    return true;
                case ArgumentMode.register_AL:
                    if (type != ArgumentType.Register)
                    {
                        return false;
                    }
                    result = Register.AL;
                    return true;
                case ArgumentMode.register_A:
                    if (type != ArgumentType.Register)
                    {
                        return false;
                    }
                    result = Register.A;
                    return true;
                case ArgumentMode.register_HL:
                    if (type != ArgumentType.Register)
                    {
                        return false;
                    }
                    result = Register.HL;
                    return true;
                case ArgumentMode.register_address_HL:
                    if (type != ArgumentType.Address)
                    {
                        return false;
                    }
                    result = (Address)GetRegisterValue(Register.HL);
                    return true;
                case ArgumentMode.BP_Offset_Address:
                    if (type != ArgumentType.Address)
                    {
                        return false;
                    }
                    int r = GetRegisterValue(Register.BP);
                    short offset = (short)FetchWord();
                    r += offset;
                    result = GetSegment(Register.SS, r);
                    return true;
                default:
                    break;
            }

            result = default;
            return false;
        }
        bool GetDestinationAll(ArgumentMode destination, Size InstructionSize, out Register? DestinationRegister, out Address? DestinationAddress)
        {
            object result;

            object DataDestination = null;
            DestinationRegister = null;
            DestinationAddress = null;

            if (GetDestination(destination, ArgumentType.Register, out result))
            {
                Register register = (Register)result;
                DestinationRegister = register;
                DataDestination = register;
            }
            else if (GetDestination(destination, ArgumentType.Address, out result))
            {
                Address address = (Address)result;
                DataDestination = result;
                DestinationAddress = (Address)address;
            }
            else if (GetDestination(destination, ArgumentType.SegmentAddress, out result))
            {
                Address address = (Address)result;
                DataDestination = result;
                DestinationAddress = address;
            }


            return DataDestination == null;
        }
        bool GetSourceAll(ArgumentMode source, Size InstructionSize, out int? ImmSource)
        {
            object result;

            ImmSource = null;
            object DataSource = null;

            if (GetSource(source, ArgumentType.imm, out result))
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
            else if (GetSource(source, ArgumentType.Address, out result))
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
            else if (GetSource(source, ArgumentType.Register, out result))
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
            else if (GetDestination(source, ArgumentType.SegmentAddress, out result))
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
