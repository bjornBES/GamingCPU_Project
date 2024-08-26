using AssemblerBCG16;
using BCG16CPUEmulator.Types;
using System;
using System.Collections.Generic;
using System.Text;

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
        RegisterAddress,
        FarAddress,
        LongAddress,
        Float,
        SegmentAddress,
        SegmentAddressImmediate,
        SegmentImmediateAddress,
        SegmentDSRegister
    }
    public class BCG16CPU_Instructions : BCG16CPU_Registers
    {
        public void Jump(uint address)
        {
            if (address == 0)
            {
                CS = 0;
            }
            PC = address;
        }

        public void Move(Size InstructionSize, ArgumentMode destination, ArgumentMode source)
        {


            if (GetDestination(destination, ArgumentType.Register, out object result))
            {
                Register register = (Register)result;
            }


        }

        bool GetDestination(ArgumentMode Destination, ArgumentType type, out object result)
        {
            result = default;
            switch (Destination)
            {
                case ArgumentMode.address:
                    if (type != ArgumentType.Address)
                    {
                        return false;
                    }
                    Address address = FetchWord();
                    result = address;
                    return true;
                case ArgumentMode.register:
                    if(type != ArgumentType.Register)
                    {
                        return false;
                    }
                    Register register = (Register)FetchByte();
                    result = register;
                    return true;
                case ArgumentMode.register_address:
                    break;
                case ArgumentMode.far_address:
                    break;
                case ArgumentMode.long_address:
                    break;
                case ArgumentMode.float_immediate:
                    break;
                case ArgumentMode.segment_address:
                    break;
                case ArgumentMode.segment_address_immediate:
                    break;
                case ArgumentMode.segment_immediate_address:
                    break;
                case ArgumentMode.segment_DS_register:
                    break;
                default:
                    break;
            }

            result = default;
            return false;
        }

        bool GetSource(ArgumentMode source, ArgumentType type, out object result)
        {
            result = default;
            switch (source)
            {
                case ArgumentMode.immediate_byte:
                case ArgumentMode.immediate_word:
                case ArgumentMode.immediate_tbyte:
                case ArgumentMode.immediate_dword:
                    if (type == ArgumentType.imm)
                    {

                    }
                    break;
                case ArgumentMode.address:
                    if (type != ArgumentType.Address)
                    {
                        return false;
                    }
                    Address address = FetchWord();
                    result = address;
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
                    break;
                case ArgumentMode.far_address:
                    break;
                case ArgumentMode.long_address:
                    break;
                case ArgumentMode.float_immediate:
                    break;
                case ArgumentMode.segment_address:
                    break;
                case ArgumentMode.segment_address_immediate:
                    break;
                case ArgumentMode.segment_immediate_address:
                    break;
                case ArgumentMode.segment_DS_register:
                    break;
                default:
                    break;
            }

            result = default;
            return false;
        }

        public byte FetchByte()
        {
            return m_BUS.m_Memory.ReadByte(PC++);
        }
        public ushort FetchWord()
        {
            ushort result = m_BUS.m_Memory.ReadWord(PC);
            PC += 2;
            return result;
        }
        public uint FetchTByte()
        {
            uint result = m_BUS.m_Memory.ReadTByte(PC);
            PC += 3;
            return result;
        }
        public uint FetchDWord()
        {
            uint result = m_BUS.m_Memory.ReadDWord(PC);
            PC += 4;
            return result;
        }
    }
}
