using BCG16CPUEmulator.Types;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using static HexLibrary.SplitFunctions;
using static HexLibrary.HexConverter;

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
        public void Jump(Address address)
        {
            PC = address;
        }

        public void Move(Size InstructionSize, ArgumentMode destination, ArgumentMode source)
        {
            object result;

            Register? DestinationRegister = null;

            int? ImmSource = null;

            object DataSource = null;
            object DataDestination = null;

            if (GetDestination(destination, ArgumentType.Register, out result))
            {
                Register register = (Register)result;
                DestinationRegister = register;
                DataDestination = register;
            }

            if (GetSource(source, ArgumentType.imm, out result))
            {
                DataSource = result;
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
                        break;
                    case ArgumentMode.immediate_dword:
                        break;
                }
            }


            if (DataSource == null)
            {
                throw new NotImplementedException();
            }
            if (DataDestination == null)
            {
                throw new NotImplementedException();
            }

            if (DestinationRegister.HasValue)
            {
                if (ImmSource.HasValue)
                {
                    SetRegisterValue(DestinationRegister.Value, ImmSource.Value);
                }
            }
        }
        public void Sez(ArgumentMode destination)
        {
            object result;

            Register? DestinationRegister = null;

            object DataDestination = null;

            if (GetDestination(destination, ArgumentType.Register, out result))
            {
                Register register = (Register)result;
                DestinationRegister = register;
                DataDestination = register;
            }

            if (DataDestination == null)
            {
                throw new NotImplementedException();
            }

            if (DestinationRegister.HasValue)
            {
                SetRegisterValue(DestinationRegister.Value, 0);
            }
        }

        public void Push(ArgumentMode source)
        {
            object result;

            int? ImmSource = null;

            object DataSource = null;

            if (GetSource(source, ArgumentType.imm, out result))
            {
                DataSource = result;
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
                        break;
                    case ArgumentMode.immediate_dword:
                        break;
                }
            }

            if (DataSource == null)
            {
                throw new NotImplementedException();
            }

            if (ImmSource.HasValue)
            {
                Push(ImmSource.Value, source);
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

        public uint GetFullStackAddress()
        {
            return (SS << 16) | SP;
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
                SS++;
                SP = 0;
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
                    break;
                case ArgumentMode.immediate_tbyte:
                    break;
                case ArgumentMode.immediate_dword:
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

        public byte FetchByte()
        {
            byte result = m_BUS.m_Memory.ReadByte(PC);
            PC++;
            return result;
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

        public int GetRegisterValue(Register register)
        {
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

                Register.ABX => ABX,
                Register.CDX => CDX,

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

                Register.IL => IL,

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
                    ABX = value;
                    break;
                case Register.CDX: 
                    CDX = value;
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
                case Register.IL:
                        IL = value;
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
            }

            switch (register)
            {
                case Register.A:
                case Register.AH:
                case Register.AL:
                case Register.B:
                case Register.BH:
                case Register.BL:
                case Register.C:
                case Register.CH:
                case Register.CL:
                case Register.D:
                case Register.DH:
                case Register.DL:
                case Register.H:
                case Register.L:
                    ABX[true] = A;
                    ABX[false] = B;

                    CDX[true] = C;
                    CDX[false] = D;

                    HL[true] = H;
                    HL[false] = L;
                    break;
                default:
                    A = ABX[true];
                    B = ABX[false];

                    C = CDX[true];
                    D = CDX[false];

                    H = HL[true];
                    L = HL[false];
                    break;
            }
        }
        public void SetRegisterValue(Register register, Register value)
        {
            SetRegisterValue(register, GetRegisterValue(value));
        }
    }
}
