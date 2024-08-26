using assembler;
using assembler.global;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace emulator
{
    public class CPU_UtilsFunctions : CPUFields
    {
        public void IncrementPC()
        {
            PC++;
        }
        public ushort FetchWord(byte bank = 0x10)
        {
            if (bank == 0x10)
            {
                return (ushort)((BUS.Read(PC, MB) << 8) & BUS.Read(PC, MB));
            }
            return (ushort)((BUS.Read(PC, bank) << 8) & BUS.Read(PC, bank));
        }
        public byte FetchByte(byte bank = 0x10)
        {
            if (bank == 0x10)
            {
                return BUS.Read(PC, MB);
            }
            return BUS.Read(PC, bank);
        }
        public int FetchBytes(int size)
        {
            List<byte> bytes = new List<byte>();
            for (int i = 0; i < size; i++)
            {
                bytes.Add(FetchByteFromPC());
            }
            return Combine(bytes.ToArray());
        }
        public byte FetchByteFromPC()
        {
            byte data = BUS.Read(PC, 0xF);
            IncrementPC();
            return data;
        }
        public ushort FetchWordFromPC()
        {
            ushort data = (ushort)(BUS.Read(PC, 0xF) << 8);
            IncrementPC();
            data |= BUS.Read(PC, 0xF);
            IncrementPC();
            return data;
        }
        public bool GetFlag(ushort flag)
        {
            return flag == (ushort)(Flags & flag);
        }
        public void SetFlag(ushort flag, bool status)
        {
            if (status)
            {
                Flags |= flag;
            }
            else
            {
                Flags &= (ushort)~flag;
            }
        }
        public int Combine(byte[] data, bool LittleEndian = false)
        {
            string hexString = "";
            if (LittleEndian)
            {
                if (data.Length != 1)
                {
                    // low + high
                    for (int i = 0; i < data.Length; i++)
                    {
                        hexString += Convert.ToString(data[i], 16);
                    }
                }
                else
                {
                    hexString = Convert.ToString(data[0], 16);
                }
            }
            else
            {
                if (data.Length != 1)
                {
                    // high + low
                    for (int i = data.Length - 1; i > 0; i--)
                    {
                        hexString += Convert.ToString(data[i], 16);
                    }
                }
                else
                {
                    hexString = Convert.ToString(data[0], 16);
                }
            }

            return Convert.ToInt32(Convert.ToString(data[0], 16), 16);
        }
        public byte[] SplitToBytes(ushort data, bool LittleEndian = false)
        {
            string HexData = Convert.ToString(data, 16).PadLeft(4, '0');
            if (LittleEndian)
            {
                return new byte[] { Convert.ToByte(HexData.Substring(2, 2), 16), Convert.ToByte(HexData.Substring(0, 2), 16) };
            }
            else
            {
                return new byte[] { Convert.ToByte(HexData.Substring(0, 2), 16), Convert.ToByte(HexData.Substring(2, 2), 16) };
            }
        }
        public byte[] SplitToBytes(int data, bool LittleEndian = false)
        {
            string HexData = Convert.ToString(data, 16).PadLeft(8, '0');

            List<byte> result = new List<byte>();
            for (int i = 0; i < HexData.Length; i += 2)
            {
                result.Add(Convert.ToByte(HexData.Substring(i, 2), 16));
            }
            if (!LittleEndian)
            {
                result.Reverse();
            }

            return result.ToArray();
        }
        public byte[] SplitToBytes(short data, bool LittleEndian = false)
        {
            string HexData = Convert.ToString(data, 16).PadLeft(4, '0');
            if (LittleEndian)
            {
                return new byte[] { Convert.ToByte(HexData.Substring(2, 2), 16), Convert.ToByte(HexData.Substring(0, 2), 16) };
            }
            else
            {
                return new byte[] { Convert.ToByte(HexData.Substring(0, 2), 16), Convert.ToByte(HexData.Substring(2, 2), 16) };
            }
        }
        public int ParseSource(Arguments argument)
        {
            int value = 0;
            Register Reg_result;
            switch (argument)
            {
                case Arguments.imm8:
                    value = FetchByteFromPC();
                    break;
                case Arguments.imm16:
                    value = FetchWordFromPC();
                    break;
                case Arguments.imm24:
                    value = FetchWordFromPC() << 8;
                    value |= FetchWordFromPC();
                    break;
                case Arguments.imm32:
                    value = FetchWordFromPC() << 8;
                    value |= FetchWordFromPC();
                    break;
                case Arguments.address:
                    value = BUS.Read(FetchWordFromPC(), MB);
                    break;
                case Arguments.reg8:
                case Arguments.reg16:
                case Arguments.reg32:
                case Arguments.reg:
                    value = FetchByteFromPC();
                    if (Enum.TryParse(value.ToString(), out Reg_result))
                    {
                        value = GetRegisterValue(Reg_result);
                        break;
                    }
                    break;
                case Arguments.reg_addr:
                    value = FetchByteFromPC();
                    if (Enum.TryParse(value.ToString(), out Reg_result))
                    {
                        value = BUS.Read(GetRegisterValue(Reg_result), MB);
                        break;
                    }
                    break;
                case Arguments._string:
                    value = 0;
                    break;
                /*

case ArgumentMode.LongAddress:
value = FetchByteFromPC() << 16;
value |= FetchWordFromPC();
break;
case ArgumentMode.FloatImmediate:
value = FetchWordFromPC() << 16;
value |= FetchWordFromPC();
break;
case ArgumentMode.SegmentAddress:
value = FetchByteFromPC();
if (!Enum.TryParse(value.ToString(), out Register Segment))
{
throw new Exception($"something is wrong i can feel it {value}");
}
value = FetchByteFromPC();
if (!Enum.TryParse(value.ToString(), out Reg_result))
{
throw new Exception($"something is wrong i can feel it {value}");
}

value = GetRegisterValue(Segment) << 16;
value |= GetRegisterValue(Reg_result);

break;
case ArgumentMode.SegmentAddressImmediate:
offset = FetchWordFromPC();
value = FetchByteFromPC();
if (!Enum.TryParse(value.ToString(), out Reg_result))
{
throw new Exception($"something is wrong i can feel it {value}");
}

value = offset << 16;
value |= GetRegisterValue(Reg_result);
break;
*/
                default:
                    return -1;
            }
            return value;
        }
        public void UpdateFlags(int UpdatedRegister, int Register)
        {
        }
        public void push(byte source)
        {
            BUS.Write(SP, source, 0);
            SP++;
        }
        public void push(Register register)
        {
            int value = GetRegisterValue(register);
            byte[] bytes = SplitToBytes(value);
            for (int i = 0; i < bytes.Length; i++)
            {
                push(bytes[i]);
            }
        }
        public void push(ushort source)
        {
            byte[] bytes = SplitToBytes(source);
            for (int i = 0; i < bytes.Length; i++)
            {
                push(bytes[i]);
            }
        }
        public void pop(Register register)
        {
            int size = GetRegisterSize(register);
            List<byte> bytes = new List<byte>();
            for (int i = 0; i < size; i++)
            {
                SP--;
                bytes.Add(BUS.Read(SP, 0));
            }

            int data = Combine(bytes.ToArray());

            SetRegister(register, data);
        }
        public void Jump(ushort address, ushort flag = 0x0200, bool to = false)
        {
            if (flag == 0x0200)
            {
                PC = address;
            }
            else if (GetFlag(flag) == to)
            {
                PC = address;
            }
        }
        public void Jump(int address, ushort flag = 0x0200, bool to = false)
        {
            if (flag == 0x0200)
            {
                PC = address;   
            }
            else if (GetFlag(flag) == to)
            {
                PC = address;   
            }
        }
        public int GetRegisterValue(Register value)
        {
            return value switch
            {
                Register.AX => (int)AX,
                Register.AL => AX[false],
                Register.AH => AX[true],
                Register.BX => (int)BX,
                Register.BL => BX[false],
                Register.BH => BX[true],
                Register.CX => (int)CX,
                Register.CL => CX[false],
                Register.CH => CX[true],
                Register.DX => (int)DX,
                Register.DL => DX[false],
                Register.DH => DX[true],
                Register.HL => HL,
                Register.H => (int)HL[true],
                Register.L => (int)HL[false],
                Register.PC => PC,
                Register.PCH => (int)PC[false],
                Register.PCL => (int)PC[true],
                Register.F => (int)Flags,
                Register.AF => 0,
                Register.BF => 0,
                Register.SP => (int)SP,
                Register.BP => (int)BP,
                Register.R1 => (int)R1,
                Register.R2 => (int)R2,
                Register.R3 => (int)R3,
                Register.R4 => (int)R4,
                Register.MB => MB,
                Register.IL => IL,
                Register.F1 => F1,
                Register.F2 => F2,
                Register.F3 => F3,
                Register.F4 => F4,
                Register.S => S,
                Register.DS => DS,
                Register.FDS => FDS,
                _ => 0,
            };
        }
        public Register GetRegister(int value)
        {
            Register result;

            result = ((RegisterInfo)value).m_Register;

            return result;
        }
        public void SetRegister(Register register, int value)
        {
            switch (register)
            {
                case Register.AX:
                    AX = value;
                    break;
                case Register.AL:
                    AX[1] = value;
                    break;
                case Register.AH:
                    AX[2] = value;
                    break;
                case Register.BX:
                    BX = value;
                    break;
                case Register.BL:
                    BX[1] = value;
                    break;
                case Register.BH:
                    BX[2] = value;
                    break;
                case Register.CX:
                    CX = value;
                    break;
                case Register.CL:
                    CX[1] = value;
                    break;
                case Register.CH:
                    CX[2] = value;
                    break;
                case Register.DX:
                    DX = value;
                    break;
                case Register.DL:
                    DX[1] = value;
                    break;
                case Register.DH:
                    DX[2] = value;
                    break;
                case Register.HL:
                    HL = value;
                    break;
                case Register.H:
                    HL[true] = value;
                    break;
                case Register.L:
                    HL[false] = value;
                    break;
                case Register.PC:
                    Jump((ushort)value);
                    break;
                case Register.PCH:
                    PC[false] = value;
                    break;
                case Register.PCL:
                    PC[true] = value;
                    break;
                case Register.F:
                    Flags = value;
                    break;
                case Register.IL:
                    IL = (byte)value;
                    break;
                case Register.AF:
                    FA = value;
                    break;
                case Register.BF:
                    FB = value;
                    break;
                case Register.SP:
                    SP = value;
                    break;
                case Register.BP:
                    BP = value;
                    break;
                case Register.R1:
                    R1 = value;
                    break;
                case Register.R2:
                    R2 = value;
                    break;
                case Register.R3:
                    R3 = value;
                    break;
                case Register.R4:
                    R4 = value;
                    break;
                case Register.F1:
                    F1 = value;
                    break;
                case Register.F2:
                    F2 = value;
                    break;
                case Register.F3:
                    F3 = value;
                    break;
                case Register.F4:
                    F4 = value;
                    break;
                case Register.S:
                    S = value;
                    break;
                case Register.DS:
                    DS = value;
                    break;
                case Register.FDS:
                    FDS = value;
                    break;
                case Register.MB:
                    MB = (byte)value;
                    break;
            }
        }
        public int GetRegisterSize(Register register)
        {
            return register switch
            {
                Register.AX => 2,
                Register.AL => 1,
                Register.AH => 1,
                Register.BX => 2,
                Register.BL => 1,
                Register.BH => 1,
                Register.CX => 2,
                Register.CL => 1,
                Register.CH => 1,
                Register.DX => 2,
                Register.DL => 1,
                Register.DH => 1,
                Register.HL => 4,
                Register.H => 2,
                Register.L => 2,
                Register.PC => 4,
                Register.PCH => 2,
                Register.PCL => 2,
                Register.F => 2,
                Register.AF => 4,
                Register.BF => 4,
                Register.SP => 2,
                Register.BP => 2,
                Register.R1 => 2,
                Register.R2 => 2,
                Register.R3 => 2,
                Register.R4 => 2,
                Register.MB => 1,
                Register.IL => 1,
                Register.F1 => 4,
                Register.F2 => 4,
                Register.F3 => 4,
                Register.F4 => 4,
                Register.S => 2,
                Register.DS => 2,
                Register.FDS => 2,
                _ => 0,
            };
        }
        public int GetArgumentSize(Arguments argument)
        {
            return argument switch
            {
                Arguments.imm8 => 1, Arguments.reg8 => 1,
                Arguments.MB => 1,
                Arguments.imm16 => 2, Arguments.reg16 => 2,
                Arguments.AX => 2,
                Arguments.BX => 2,
                Arguments.CX => 2,
                Arguments.DX => 2,
                Arguments.imm24 => 3,
                Arguments.reg32 => 4,
                Arguments.imm32 => 4,
                Arguments.address => 2,
                Arguments.reg_addr => 2,
                _ => 0,
            };
        }
        public InstructionInfo GetInstructionInfo(string opcode)
        {
            InstructionInfo instructionInfo = AllInstruction.s_instructionInfo.Find(i => i.Equals(opcode));
            return instructionInfo;
        }
        public InstructionInfo GetInstructionInfo(Instruction instr)
        {
            InstructionInfo instructionInfo = AllInstruction.s_instructionInfo.Find(i => i.m_Instruction == instr);
            return instructionInfo;
        }

        public void Instr_MOV(InstructionInfo instruction)
        {
            int arg1 = FetchBytes(GetArgumentSize(instruction.m_Argument1));

            int soruce = ParseSource(instruction.m_Argument2);
            
            switch (instruction.m_Argument1)
            {
                case Arguments.address:
                    BUS.Write(arg1, SplitToBytes(soruce), MB);
                    break;
                case Arguments.reg:
                case Arguments.reg8:
                case Arguments.reg16:
                case Arguments.reg32:
                    Register register = GetRegister(arg1);
                    SetRegister(register, soruce);
                    break;
                case Arguments.reg_addr:
                    Register addrregister = GetRegister(arg1);
                    int addr = GetRegisterValue(addrregister);
                    BUS.Write(addr, SplitToBytes(soruce), MB);
                    break;
                case Arguments._string:
                    break;
                default:
                    break;
            }
        }
    }
}
