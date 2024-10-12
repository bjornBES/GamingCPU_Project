using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesassemblerBCG16
{
    public enum Size
    {
        _byte,
        _word,
        _tbyte,
        _dword,
    }
    public class DisAssembler
    {
        public List<string> m_Output = new List<string>();
        byte[] m_src;
        int m_index;
        public void Build(byte[] src)
        {
            m_src = src;
            for (m_index = 0; m_index < src.Length; )
            {
                if (m_index + 2 <= src.Length)
                {
                    ushort instr = BitConverter.ToUInt16(src[m_index..(m_index + 2)].Reverse().ToArray());

                    if (Enum.TryParse(instr.ToString(), out Instruction result) && Enum.IsDefined(typeof(Instruction), result))
                    {
                        ArgumentMode argumentMode1 = (ArgumentMode)0xFF;
                        ArgumentMode argumentMode2 = (ArgumentMode)0xFF;

                        string argument1Data = "";
                        string argument2Data = "";

                        switch (result)
                        {
                            case Instruction.MOVWRACR0:
                            case Instruction.MOVWRCR0A:
                            case Instruction.CALLRHL:
                            case Instruction.RETZ:
                            case Instruction.SEZRAL:
                            case Instruction.SEZRA:
                            case Instruction.SEZRAX:
                            case Instruction.TESTRAL:
                            case Instruction.TESTRA:
                            case Instruction.TESTRAX:
                            case Instruction.SZE:
                            case Instruction.SEE:
                            case Instruction.SES:
                            case Instruction.SEC:
                            case Instruction.SEL:
                            case Instruction.SEI:
                            case Instruction.SEH:
                            case Instruction.CZE:
                            case Instruction.CLE:
                            case Instruction.CLS:
                            case Instruction.CLC:
                            case Instruction.CLL:
                            case Instruction.CLI:
                            case Instruction.CLH:
                            case Instruction.CMPL:
                            case Instruction.RETI:
                            case Instruction.NOP:
                            case Instruction.PUSHR:
                            case Instruction.POPR:
                            case Instruction.BRK:
                            case Instruction.ENTER:
                            case Instruction.LEAVE:
                            case Instruction.HALT:
                                m_index += 2;
                                argumentMode1 = (ArgumentMode)0xFF;
                                argumentMode2 = (ArgumentMode)0xFF;
                                break;
                            case Instruction.MOVRAL:
                            case Instruction.MOVRBL:
                            case Instruction.MOVRCL:
                            case Instruction.MOVRDL:
                            case Instruction.MOVWRA:
                            case Instruction.MOVWRB:
                            case Instruction.MOVWRC:
                            case Instruction.MOVWRD:
                            case Instruction.MOVDRAX:
                            case Instruction.PUSH:
                            case Instruction.PUSHW:
                            case Instruction.PUSHT:
                            case Instruction.PUSHD:
                            case Instruction.POP:
                            case Instruction.POPW:
                            case Instruction.POPT:
                            case Instruction.POPD:
                            case Instruction.CALL:
                            case Instruction.RET:
                            case Instruction.SEZ:
                            case Instruction.TEST:
                            case Instruction.NOT:
                            case Instruction.INC:
                            case Instruction.DEC:
                            case Instruction.NEG:
                            case Instruction.SQRT:
                            case Instruction.RNG:
                            case Instruction.FNOT:
                            case Instruction.JMP:
                            case Instruction.JZ:
                            case Instruction.JNZ:
                            case Instruction.JS:
                            case Instruction.JNS:
                            case Instruction.JE:
                            case Instruction.JNE:
                            case Instruction.JL:
                            case Instruction.JG:
                            case Instruction.JLE:
                            case Instruction.JGE:
                            case Instruction.JNV:
                            case Instruction.INT:
                                m_index += 2;
                                argumentMode1 = (ArgumentMode)src[m_index];
                                m_index++;
                                break;
                            case Instruction.MOV:
                            case Instruction.MOVW:
                            case Instruction.MOVT:
                            case Instruction.MOVD:
                            case Instruction.CMP:
                            case Instruction.OUTB:
                            case Instruction.OUTW:
                            case Instruction.INPB:
                            case Instruction.INPW:
                            case Instruction.EXP:
                            case Instruction.SEB:
                            case Instruction.CLB:
                            case Instruction.TOB:
                            case Instruction.MOD:
                            case Instruction.FADD:
                            case Instruction.FSUB:
                            case Instruction.FMUL:
                            case Instruction.FDIV:
                            case Instruction.FAND:
                            case Instruction.FOR:
                            case Instruction.FNOR:
                            case Instruction.FXOR:
                            case Instruction.ADD:
                            case Instruction.SUB:
                            case Instruction.MUL:
                            case Instruction.DIV:
                            case Instruction.AND:
                            case Instruction.OR:
                            case Instruction.NOR:
                            case Instruction.XOR:
                            case Instruction.SHL:
                            case Instruction.SHR:
                            case Instruction.ROL:
                            case Instruction.ROR:
                            case Instruction.CBTA:
                            case Instruction.MOVF:
                                m_index += 2;
                                argumentMode1 = (ArgumentMode)src[m_index];
                                m_index++;
                                argumentMode2 = (ArgumentMode)src[m_index];
                                m_index++;
                                break;
                            default:
                                break;
                        }

                        DecodeArgument(argumentMode1, out argument1Data);
                        DecodeArgument(argumentMode2, out argument2Data);

                        m_Output.Add($"{result}".PadRight(8, ' ') + $"\t\t" + $"{argument1Data}".PadRight(20, ' ') + $"\t\t" + $"{argument2Data}");
                        //return;
                    }
                    else
                    {
                        m_Output.Add($".byte {FetchHex(Size._byte)}");
                        m_index++;
                    }
                }
                else
                {
                    m_Output.Add($".byte {FetchHex(Size._byte)}");
                    m_index++;
                }
            }
        }

        public void DecodeArgument(ArgumentMode argumentMode, out string data)
        {
            data = "";
            sbyte sdata;
            switch (argumentMode)
            {
                case ArgumentMode.immediate_byte:
                    data = FetchHex(Size._byte);
                    break;
                case ArgumentMode.immediate_word:
                    data = FetchHex(Size._word);
                    break;
                case ArgumentMode.immediate_tbyte:
                    data = FetchHex(Size._tbyte);
                    break;
                case ArgumentMode.immediate_dword:
                    data = FetchHex(Size._dword);
                    break;
                case ArgumentMode.immediate_float:
                    break;
                case ArgumentMode.register:
                case ArgumentMode.register_address:
                    Register reg = (Register)FetchByte();
                    data = reg.ToString();
                    if (argumentMode == ArgumentMode.register_address)
                    {
                        data = $"[{data}]";
                    }
                    break;
                case ArgumentMode.register_A:
                    data = "A";
                    break;
                case ArgumentMode.register_B:
                    data = "B";
                    break;
                case ArgumentMode.register_C:
                    data = "C";
                    break;
                case ArgumentMode.register_D:
                    data = "D";
                    break;
                case ArgumentMode.register_H:
                    data = "H";
                    break;
                case ArgumentMode.register_L:
                    data = "L";
                    break;
                case ArgumentMode.register_address_HL:
                    data = "[HL]";
                    break;
                case ArgumentMode.register_MB:
                    data = "MB";
                    break;
                case ArgumentMode.register_AX:
                    data = "AX";
                    break;
                case ArgumentMode.register_BX:
                    data = "BX";
                    break;
                case ArgumentMode.register_CX:
                    data = "CX";
                    break;
                case ArgumentMode.register_DX:
                    data = "DX";
                    break;
                case ArgumentMode.relative_address:
                    break;
                case ArgumentMode.near_address:
                    data = $"[{FetchHex(Size._byte)}]";
                    break;
                case ArgumentMode.short_address:
                    data = $"[{FetchHex(Size._word)}]";
                    break;
                case ArgumentMode.long_address:
                    data = $"[{FetchHex(Size._tbyte)}]";
                    break;
                case ArgumentMode.far_address:
                    data = $"[{FetchHex(Size._dword)}]";
                    break;
                case ArgumentMode.SP_Offset_Address:
                    sdata = (sbyte)FetchByte();
                    data = $"[SP {sdata}]";
                    break;
                case ArgumentMode.BP_Offset_Address:
                    sdata = (sbyte)FetchByte();
                    data = $"[SP {sdata}]";
                    break;
                case ArgumentMode.segment_address:
                    Register segment = (Register)FetchByte();
                    Register offset = (Register)FetchByte();
                    data = $"[{segment}:{offset}]";
                    break;
                case ArgumentMode.segment_DS_register:
                    offset = (Register)FetchByte();
                    data = $"[DS:{offset}]";
                    break;
                case ArgumentMode.segment_DS_B:
                    data = "[DS:B]";
                    break;
                case ArgumentMode.segment_ES_register:
                    offset = (Register)FetchByte();
                    data = $"[ES:{offset}]";
                    break;
                case ArgumentMode.register_AL:
                    data = "AL";
                    break;
                default:
                    break;
            }
        }

        public byte FetchByte()
        {
            byte result = m_src[m_index];
            m_index++;
            return result;
        }
        public ushort FetchWord()
        {
            ushort result = BitConverter.ToUInt16(m_src[m_index..(m_index + 2)].Reverse().ToArray(), 0);
            m_index += 2;
            return result;
        }
        public uint FetchTByte()
        {
            List<byte> bytes = m_src[m_index..(m_index + 3)].Reverse().ToList();
            bytes.Insert(3, 0);
            int result = BitConverter.ToInt32(bytes.ToArray(), 0);
            m_index += 3;
            return (uint)result;
        }
        public int FetchDWord()
        {
            int result = BitConverter.ToInt32(m_src[m_index..(m_index + 4)].Reverse().ToArray(), 0);
            m_index += 4;
            return result;
        }
        public float FetchFloat()
        {
            uint intValue = (uint)FetchDWord();
            byte[] floatBytes = BitConverter.GetBytes(intValue);
            float result = BitConverter.ToSingle(floatBytes, 0);
            return result;
        }

        public string FetchHex(Size size)
        {
            switch (size)
            {
                case Size._byte:
                    return $"0x{Convert.ToString(FetchByte(), 16).PadLeft(2, '0')}";
                case Size._word:
                    return $"0x{Convert.ToString(FetchWord(), 16).PadLeft(4, '0')}";
                case Size._tbyte:
                    return $"0x{Convert.ToString(FetchTByte(), 16).PadLeft(6, '0')}";
                case Size._dword:
                    return $"0x{Convert.ToString(FetchDWord(), 16).PadLeft(8, '0')}";
                default:
                    break;
            }
            return "";
        }
    }
}
