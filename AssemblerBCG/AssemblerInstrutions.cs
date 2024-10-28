using static HexLibrary.HexConverter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AssemblerBCG
{
    public class AssemblerInstrutions : AssemblerStructs
    {
        ArgumentMode argument1 = 0;
        ArgumentMode argument2 = 0;

        List<object> argumentData = new List<object>();

        public void AssembleInstrution()
        {
            List<string> InstructionBytes = new List<string>();

            string line = m_src[m_index];
            string instruction = line.Split(' ')[0];
            string[] arguments = line.Replace(instruction, "").Split(',');
            for (int i = 0; i < arguments.Length; i++)
            {
                arguments[i] = arguments[i].Trim();
            }

            if (!Enum.TryParse(instruction, true, out Instruction result))
            {
                Console.WriteLine($"Bad Instruction {instruction} {m_file}:{Linenumber}");
                m_WriteOut = true;
                return;
            }

            InstructionInfo instructionInfo = Instructions.m_Instr[result];

            if (!string.IsNullOrEmpty(arguments[0]))
            {
                if (arguments.Length != instructionInfo.m_NumberOfOperands)
                {
                    Console.WriteLine($"Invalid Instruction {instruction} {m_file}:{Linenumber}");
                    m_WriteOut = true;
                    return;
                }
            }

            parseInstrutionArguments(result, arguments, ref InstructionBytes, startingIndex: 0);

            m_Output.AddRange(InstructionBytes);
        }

        private void parseInstrutionArguments(Instruction instruction, string[] arguments, ref List<string> instructionBytes, int startingIndex = 0)
        {
            argument1 = ArgumentMode.none;
            argument2 = ArgumentMode.none;
            string argument1data = null;
            string argument2data = null;
            List<string> argumentBuffer = new List<string>();
            SizeAlignment sizeAlignment = SizeAlignment.none;

            switch (instruction)
            {
                case Instruction.MOV:
                case Instruction.MOVRAL:
                case Instruction.MOVRBL:
                case Instruction.MOVRCL:
                case Instruction.MOVRDL:
                case Instruction.OUTB:
                case Instruction.INP:
                    sizeAlignment = SizeAlignment._byte;
                    break;
                case Instruction.MOVW:
                case Instruction.MOVWRA:
                case Instruction.MOVWRB:
                case Instruction.MOVWRC:
                case Instruction.MOVWRD:
                case Instruction.MOVRALCR0:
                case Instruction.MOVRCR0AL:
                case Instruction.OUTW:
                case Instruction.INPW:
                    sizeAlignment = SizeAlignment._word;
                    break;
                case Instruction.MOVT:
                    sizeAlignment = SizeAlignment._tbyte;
                    break;
                case Instruction.MOVD:
                case Instruction.MOVDRAX:
                    sizeAlignment = SizeAlignment._dword;
                    break;
            }

            for (int i = startingIndex; i < arguments.Length; i++)
            {
                string argument = arguments[i];

                if (string.IsNullOrEmpty(argument))
                {
                    continue;
                }

                if (argument.Contains("byte", StringComparison.CurrentCultureIgnoreCase))
                {
                    argument = argument.Split(' ', 2).Last();
                    sizeAlignment = SizeAlignment._byte;
                }
                else if (argument.Contains("word", StringComparison.CurrentCultureIgnoreCase))
                {
                    argument = argument.Split(' ', 2).Last();
                    sizeAlignment = SizeAlignment._word;
                }
                else if (argument.Contains("tbyte", StringComparison.CurrentCultureIgnoreCase))
                {
                    argument = argument.Split(' ', 2).Last();
                    sizeAlignment = SizeAlignment._tbyte;
                }
                else if (argument.Contains("dword", StringComparison.CurrentCultureIgnoreCase))
                {
                    argument = argument.Split(' ', 2).Last();
                    sizeAlignment = SizeAlignment._dword;
                }
                else if (argument.Contains("qword", StringComparison.CurrentCultureIgnoreCase))
                {
                    argument = argument.Split(' ', 2).Last();
                    if (m_CPUType >= CPUType.BC32)
                    {
                        sizeAlignment = SizeAlignment._qword;
                    }
                }

                argumentBuffer.Add($"_NEWARG_ {argument}");
                if (!ParseTerm(argument, ref sizeAlignment, out ArgumentMode argumentMode, out string[] data))
                {

                }

                string result = "";

                if (data != null)
                {

                    for (int d = 0; d < data.Length; d++)
                    {
                        result += data[d];
                    }

                    if (argument1data == null)
                    {
                        argument1data = result;
                    }
                    else if (argument2data == null)
                    {
                        argument2data = result;
                    }
                }

                switch (argumentMode)
                {
                    case ArgumentMode.immediate_byte:
                    case ArgumentMode.immediate_word:
                    case ArgumentMode.immediate_tbyte:
                    case ArgumentMode.immediate_dword:
                    case ArgumentMode.immediate_qword:
                    case ArgumentMode.immediate_dqword:
                    case ArgumentMode.immediate_float:
                    case ArgumentMode.immediate_double:
                    case ArgumentMode.register:
                    case ArgumentMode.register_address:
                    case ArgumentMode.relative_address:
                    case ArgumentMode.near_address:
                    case ArgumentMode.short_address:
                    case ArgumentMode.long_address:
                    case ArgumentMode.far_address:
                    case ArgumentMode.X_indexed_address:
                    case ArgumentMode.Y_indexed_address:
                    case ArgumentMode.SP_rel_address_byte:
                    case ArgumentMode.BP_rel_address_byte:
                    case ArgumentMode.SPX_rel_address_word:
                    case ArgumentMode.BPX_rel_address_word:
                    case ArgumentMode.SP_rel_address_short:
                    case ArgumentMode.BP_rel_address_short:
                    case ArgumentMode.SPX_rel_address_tbyte:
                    case ArgumentMode.BPX_rel_address_tbyte:
                    case ArgumentMode.SPX_rel_address_int:
                    case ArgumentMode.BPX_rel_address_int:
                        if (argumentMode == ArgumentMode.register)
                        {
                            sizeAlignment = GetAlignmentFromRegister((Register)Convert.ToInt16(argument1data, 16)) + 1;
                        }
                        if (data != null)
                        {
                            argumentBuffer.AddRange(data);
                        }
                        SetArgumentMode(argumentMode);
                        break;
                    case ArgumentMode.segment_address:
                    case ArgumentMode.segment_DS_register:
                    case ArgumentMode.segment_ES_register:
                        Console.WriteLine($"data = {data[0]} {m_file}:{Linenumber}");
                        Register segment = Enum.Parse<Register>(data[0].Split(':')[0], true);
                        Register offset = Enum.Parse<Register>(data[0].Split(':')[1], true);

                        if (argumentMode == ArgumentMode.segment_address)
                        {
                            argumentBuffer.Add(Convert.ToString((byte)segment, 16));
                            argumentBuffer.Add(Convert.ToString((byte)offset, 16));
                        }
                        else
                        {
                            argumentBuffer.Add(Convert.ToString((byte)offset, 16));
                        }
                        SetArgumentMode(argumentMode);
                        break;
                    case ArgumentMode.segment_DS_B:
                    case ArgumentMode.segment_ES_B:
                    case ArgumentMode.register_AL:
                    case ArgumentMode.register_BL:
                    case ArgumentMode.register_CL:
                    case ArgumentMode.register_DL:
                    case ArgumentMode.register_A:
                    case ArgumentMode.register_B:
                    case ArgumentMode.register_C:
                    case ArgumentMode.register_D:
                    case ArgumentMode.register_H:
                    case ArgumentMode.register_L:
                    case ArgumentMode.register_address_HL:
                    case ArgumentMode.register_AX:
                    case ArgumentMode.register_BX:
                    case ArgumentMode.register_CX:
                    case ArgumentMode.register_DX:
                    case ArgumentMode.register_EX:
                    case ArgumentMode.register_FX:
                    case ArgumentMode.register_GX:
                    case ArgumentMode.register_HX:
                    case ArgumentMode.register_AF:
                    case ArgumentMode.register_BF:
                    case ArgumentMode.register_CF:
                    case ArgumentMode.register_DF:
                    case ArgumentMode.register_AD:
                    case ArgumentMode.register_BD:
                    case ArgumentMode.register_CD:
                    case ArgumentMode.register_DD:
                    case ArgumentMode.none:
                    default:
                        SetArgumentMode(argumentMode);
                        break;
                }

                switch (argumentMode)
                {
                    case ArgumentMode.register_AL:
                    case ArgumentMode.register_BL:
                    case ArgumentMode.register_CL:
                    case ArgumentMode.register_DL:
                        sizeAlignment = SizeAlignment._byte;
                        break;
                    case ArgumentMode.register_A:
                    case ArgumentMode.register_B:
                    case ArgumentMode.register_C:
                    case ArgumentMode.register_D:
                    case ArgumentMode.register_H:
                    case ArgumentMode.register_L:
                        sizeAlignment = SizeAlignment._word;
                        break;
                    case ArgumentMode.register_CX:
                    case ArgumentMode.register_DX:
                    case ArgumentMode.register_EX:
                    case ArgumentMode.register_FX:
                    case ArgumentMode.register_GX:
                    case ArgumentMode.register_HX:
                    case ArgumentMode.register_AX:
                    case ArgumentMode.register_BX:
                    case ArgumentMode.register_AF:
                    case ArgumentMode.register_BF:
                    case ArgumentMode.register_CF:
                    case ArgumentMode.register_DF:
                        sizeAlignment = SizeAlignment._dword;
                        break;
                    case ArgumentMode.register_AD:
                    case ArgumentMode.register_BD:
                    case ArgumentMode.register_CD:
                    case ArgumentMode.register_DD:
                        sizeAlignment = SizeAlignment._qword;
                        break;
                }
            }

            switch (instruction)
            {
                case Instruction.PUSH:
                case Instruction.POP:
                    switch (argument1)
                    {
                        case ArgumentMode.register:
                        case ArgumentMode.register_AL:
                        case ArgumentMode.register_BL:
                        case ArgumentMode.register_CL:
                        case ArgumentMode.register_DL:
                        case ArgumentMode.register_A:
                        case ArgumentMode.register_B:
                        case ArgumentMode.register_C:
                        case ArgumentMode.register_D:
                        case ArgumentMode.register_H:
                        case ArgumentMode.register_L:
                        case ArgumentMode.register_AX:
                        case ArgumentMode.register_BX:
                        case ArgumentMode.register_CX:
                        case ArgumentMode.register_DX:
                        case ArgumentMode.register_EX:
                        case ArgumentMode.register_FX:
                        case ArgumentMode.register_GX:
                        case ArgumentMode.register_HX:
                        case ArgumentMode.register_AF:
                        case ArgumentMode.register_BF:
                        case ArgumentMode.register_CF:
                        case ArgumentMode.register_DF:
                        case ArgumentMode.register_AD:
                        case ArgumentMode.register_BD:
                        case ArgumentMode.register_CD:
                        case ArgumentMode.register_DD:
                            break;
                        default:
                            Console.WriteLine($"Error: {instruction}".PadRight(6, ' ') + $" {m_file}:{Linenumber}");
                            m_WriteOut = true;
                            break;
                    }
                    break;
                default:
                    break;
            }

            if (sizeAlignment == SizeAlignment.none)
            {
                if (Debug)
                {
                    Console.WriteLine($"did not get an alignment {m_src[m_index]} {m_file}:{Linenumber}");
                }
            }

            InstructionInfo instructionInfo = Instructions.m_Instr[instruction];

            switch (sizeAlignment)
            {
                case SizeAlignment._byte:
                    if (Debug)
                    {
                        Console.Write($"{instruction}".PadRight(6, ' ') + "into ");
                    }
                    instruction = instructionInfo.GetByteVersion();
                    if (Debug)
                    {
                        Console.WriteLine($"{instruction}".PadRight(8, ' ') + $"{m_file}:{Linenumber}");
                    }
                    break;
                case SizeAlignment._word:
                    if (Debug)
                    {
                        Console.Write($"{instruction}".PadRight(6, ' ') + "into ");
                    }
                    instruction = instructionInfo.GetWordVersion();
                    if (Debug)
                    {
                        Console.WriteLine($"{instruction}".PadRight(8, ' ') + $"{m_file}:{Linenumber}");
                    }
                    break;
                case SizeAlignment._tbyte:
                    if (Debug)
                    {
                        Console.Write($"{instruction}".PadRight(6, ' ') + "into ");
                    }
                    instruction = instructionInfo.GetTbyteVersion();
                    if (Debug)
                    {
                        Console.WriteLine($"{instruction}".PadRight(8, ' ') + $"{m_file}:{Linenumber}");
                    }
                    break;
                case SizeAlignment._dword:
                    if (Debug)
                    {
                        Console.Write($"{instruction}".PadRight(6, ' ') + "into ");
                    }
                    instruction = instructionInfo.GetDwordVersion();
                    if (Debug)
                    {
                        Console.WriteLine($"{instruction}".PadRight(8, ' ') + $"{m_file}:{Linenumber}");
                    }
                    break;
                case SizeAlignment._qword:
                    break;
                default:
                    break;
            }

            switch (instruction)
            {
                case Instruction.MOV:
                    if (argument1 == ArgumentMode.register_AL)
                    {
                        argument1 = ArgumentMode.none;
                        if (m_CPUType < CPUType.BC32 && argument2 == ArgumentMode.register && Enum.TryParse(FromHexString(argument2data).ToString(), true, out Register result) && result == Register.CR0)
                        {
                            argument2 = ArgumentMode.none;
                            instruction = Instruction.MOVRALCR0;
                            argumentBuffer.Clear();
                            break;
                        }
                        instruction = Instruction.MOVRAL;
                        break;
                    }
                    else if (argument1 == ArgumentMode.register_BL)
                    {
                        argument1 = ArgumentMode.none;
                        instruction = Instruction.MOVRBL;
                        break;
                    }
                    else if (argument1 == ArgumentMode.register_CL)
                    {
                        argument1 = ArgumentMode.none;
                        instruction = Instruction.MOVRCL;
                        break;
                    }
                    else if (argument1 == ArgumentMode.register_DL)
                    {
                        argument1 = ArgumentMode.none;
                        instruction = Instruction.MOVRDL;
                        break;
                    }
                    else if (m_CPUType < CPUType.BC32 && argument1 == ArgumentMode.register && Enum.TryParse(FromHexString(argument1data).ToString(), true, out Register result) && result == Register.CR0)
                    {
                        if (argument2 == ArgumentMode.register_AL)
                        {
                            argumentBuffer.Clear();
                            argument1 = ArgumentMode.none;
                            argument2 = ArgumentMode.none;
                            instruction = Instruction.MOVRCR0AL;
                        }
                    }
                    break;
                case Instruction.MOVW:
                    if (argument1 == ArgumentMode.register_A)
                    {
                        argument1 = ArgumentMode.none;
                        if (m_CPUType >= CPUType.BC32 && argument2 == ArgumentMode.register && Enum.TryParse(FromHexString(argument2data).ToString(), true, out Register result) && result == Register.CR0)
                        {
                            argument2 = ArgumentMode.none;
                            instruction = Instruction.MOVWRACR0;
                            argumentBuffer.Clear();
                            break;
                        }
                        instruction = Instruction.MOVWRA;
                        break;
                    }
                    else if (argument1 == ArgumentMode.register_B)
                    {
                        argument1 = ArgumentMode.none;
                        instruction = Instruction.MOVWRB;
                        break;
                    }
                    else if (argument1 == ArgumentMode.register_C)
                    {
                        argument1 = ArgumentMode.none;
                        instruction = Instruction.MOVWRC;
                        break;
                    }
                    else if (argument1 == ArgumentMode.register_D)
                    {
                        argument1 = ArgumentMode.none;
                        instruction = Instruction.MOVWRD;
                        break;
                    }
                    else if (m_CPUType >= CPUType.BC32 && argument1 == ArgumentMode.register && Enum.TryParse(FromHexString(argument1data).ToString(), true, out Register result) && result == Register.CR0)
                    {
                        if (argument2 == ArgumentMode.register_A)
                        {
                            argumentBuffer.Clear();
                            argument1 = ArgumentMode.none;
                            argument2 = ArgumentMode.none;
                            instruction = Instruction.MOVWRCR0A;
                        }
                    }
                    break;
                case Instruction.MOVD:
                    if (argument1 == ArgumentMode.register_AX)
                    {
                        argument1 = ArgumentMode.none;
                        instruction = Instruction.MOVDRAX;
                        break;
                    }
                    else if (argument1 == ArgumentMode.register_BX)
                    {
                        argument1 = ArgumentMode.none;
                        instruction = Instruction.MOVDRBX;
                        break;
                    }
                    else if (argument1 == ArgumentMode.register_CX)
                    {
                        argument1 = ArgumentMode.none;
                        instruction = Instruction.MOVDRCX;
                        break;
                    }
                    else if (argument1 == ArgumentMode.register_DX)
                    {
                        argument1 = ArgumentMode.none;
                        instruction = Instruction.MOVDRDX;
                        break;
                    }
                    break;
                case Instruction.SEZ:
                    if (argument1 == ArgumentMode.register_AL)
                    {
                        argument1 = ArgumentMode.none;
                        instruction = Instruction.SEZRAL;
                        break;
                    }
                    else if (argument1 == ArgumentMode.register_BL)
                    {
                        argument1 = ArgumentMode.none;
                        instruction = Instruction.SEZRBL;
                        break;
                    }
                    else if (argument1 == ArgumentMode.register_CL)
                    {
                        argument1 = ArgumentMode.none;
                        instruction = Instruction.SEZRCL;
                        break;
                    }
                    else if (argument1 == ArgumentMode.register_DL)
                    {
                        argument1 = ArgumentMode.none;
                        instruction = Instruction.SEZRDL;
                        break;
                    }
                    else if (argument1 == ArgumentMode.register_A)
                    {
                        argument1 = ArgumentMode.none;
                        instruction = Instruction.SEZRA;
                        break;
                    }
                    else if (argument1 == ArgumentMode.register_B)
                    {
                        argument1 = ArgumentMode.none;
                        instruction = Instruction.SEZRB;
                        break;
                    }
                    else if (argument1 == ArgumentMode.register_C)
                    {
                        argument1 = ArgumentMode.none;
                        instruction = Instruction.SEZRC;
                        break;
                    }
                    else if (argument1 == ArgumentMode.register_D)
                    {
                        argument1 = ArgumentMode.none;
                        instruction = Instruction.SEZRD;
                        break;
                    }
                    else if (argument1 == ArgumentMode.register_AX)
                    {
                        argument1 = ArgumentMode.none;
                        instruction = Instruction.SEZRAX;
                        break;
                    }
                    else if (argument1 == ArgumentMode.register_BX)
                    {
                        argument1 = ArgumentMode.none;
                        instruction = Instruction.SEZRBX;
                        break;
                    }
                    else if (argument1 == ArgumentMode.register_CX)
                    {
                        argument1 = ArgumentMode.none;
                        instruction = Instruction.SEZRCX;
                        break;
                    }
                    else if (argument1 == ArgumentMode.register_DX)
                    {
                        argument1 = ArgumentMode.none;
                        instruction = Instruction.SEZRDX;
                        break;
                    }
                    break;
                case Instruction.TEST:
                    if (argument1 == ArgumentMode.register_AL)
                    {
                        argument1 = ArgumentMode.none;
                        instruction = Instruction.TESTRAL;
                        break;
                    }
                    else if (argument1 == ArgumentMode.register_BL)
                    {
                        argument1 = ArgumentMode.none;
                        instruction = Instruction.TESTRBL;
                        break;
                    }
                    else if (argument1 == ArgumentMode.register_CL)
                    {
                        argument1 = ArgumentMode.none;
                        instruction = Instruction.TESTRCL;
                        break;
                    }
                    else if (argument1 == ArgumentMode.register_DL)
                    {
                        argument1 = ArgumentMode.none;
                        instruction = Instruction.TESTRDL;
                        break;
                    }
                    else if (argument1 == ArgumentMode.register_A)
                    {
                        argument1 = ArgumentMode.none;
                        instruction = Instruction.TESTRA;
                        break;
                    }
                    else if (argument1 == ArgumentMode.register_B)
                    {
                        argument1 = ArgumentMode.none;
                        instruction = Instruction.TESTRB;
                        break;
                    }
                    else if (argument1 == ArgumentMode.register_C)
                    {
                        argument1 = ArgumentMode.none;
                        instruction = Instruction.TESTRC;
                        break;
                    }
                    else if (argument1 == ArgumentMode.register_D)
                    {
                        argument1 = ArgumentMode.none;
                        instruction = Instruction.TESTRD;
                        break;
                    }
                    else if (argument1 == ArgumentMode.register_AX)
                    {
                        argument1 = ArgumentMode.none;
                        instruction = Instruction.TESTRAX;
                        break;
                    }
                    else if (argument1 == ArgumentMode.register_BX)
                    {
                        argument1 = ArgumentMode.none;
                        instruction = Instruction.TESTRBX;
                        break;
                    }
                    else if (argument1 == ArgumentMode.register_CX)
                    {
                        argument1 = ArgumentMode.none;
                        instruction = Instruction.TESTRCX;
                        break;
                    }
                    else if (argument1 == ArgumentMode.register_DX)
                    {
                        argument1 = ArgumentMode.none;
                        instruction = Instruction.TESTRDX;
                        break;
                    }
                    break;

                case Instruction.CMP:
                    if (argument1 == ArgumentMode.register_A)
                    {
                        instruction = Instruction.CMPRA;
                        argument1 = ArgumentMode.none;
                        break;
                    }
                    else if (argument1 == ArgumentMode.register_AX)
                    {
                        instruction = Instruction.CMPRAX;
                        argument1 = ArgumentMode.none;
                        break;
                    }
                    else if (argument2 == ArgumentMode.immediate_byte && argument2data == "0")
                    {
                        argument2 = ArgumentMode.none;
                        instruction = Instruction.CMPZ;
                    }
                    break;

                case Instruction.ADD:
                case Instruction.ADC:
                case Instruction.SUB:
                case Instruction.SBB:
                case Instruction.MUL:
                case Instruction.DIV:
                case Instruction.AND:
                case Instruction.OR:
                case Instruction.NOR:
                case Instruction.XOR:
                case Instruction.NOT:
                    if (argument1 == ArgumentMode.register_A)
                    {
                        instruction = instruction + 1;
                        argument1 = ArgumentMode.none;
                        break;
                    }
                    else if (argument1 == ArgumentMode.register_AX)
                    {
                        instruction = instruction + 2;
                        argument1 = ArgumentMode.none;
                        break;
                    }
                    break;
                default:
                    break;
            }

            // Console.WriteLine($"instruction: {instruction} {argument1}, {argument2}");

            string instr = Convert.ToString((ushort)instruction, 16).PadLeft(4, '0');
            instructionBytes.AddRange(SplitHexString(instr));
            if (argument1 != ArgumentMode.none)
            {
                string arg1 = Convert.ToString((ushort)argument1, 16).PadLeft(2, '0');
                instructionBytes.Add(arg1);
            }
            if (argument2 != ArgumentMode.none)
            {
                string arg2 = Convert.ToString((ushort)argument2, 16).PadLeft(2, '0');
                instructionBytes.Add(arg2);
            }

            instructionBytes.AddRange(argumentBuffer);
        }

        void SetArgumentMode(ArgumentMode mode) 
        {
            switch (mode)
            {
                case ArgumentMode.immediate_byte:
                case ArgumentMode.immediate_word:
                case ArgumentMode.register:
                case ArgumentMode.register_AL:
                case ArgumentMode.register_BL:
                case ArgumentMode.register_CL:
                case ArgumentMode.register_DL:
                case ArgumentMode.register_H:
                case ArgumentMode.register_L:
                case ArgumentMode.register_address:
                case ArgumentMode.register_address_HL:
                case ArgumentMode.relative_address:
                case ArgumentMode.near_address:
                case ArgumentMode.short_address:
                case ArgumentMode.segment_address:
                case ArgumentMode.segment_DS_register:
                case ArgumentMode.segment_DS_B:
                case ArgumentMode.SP_rel_address_byte:
                case ArgumentMode.BP_rel_address_byte:
                    if (m_CPUType < CPUType.BC8)
                    {
                        E_InvalidCPUFeature(CPUType.BC8, mode);
                    }
                    break;
                case ArgumentMode.immediate_tbyte:
                case ArgumentMode.immediate_dword:
                case ArgumentMode.immediate_qword:
                case ArgumentMode.immediate_float:
                case ArgumentMode.register_A:
                case ArgumentMode.register_B:
                case ArgumentMode.register_C:
                case ArgumentMode.register_D:
                case ArgumentMode.register_AX:
                case ArgumentMode.register_BX:
                case ArgumentMode.register_CX:
                case ArgumentMode.register_DX:
                case ArgumentMode.long_address:
                case ArgumentMode.far_address:
                case ArgumentMode.X_indexed_address:
                case ArgumentMode.Y_indexed_address:
                case ArgumentMode.segment_ES_register:
                case ArgumentMode.segment_ES_B:
                case ArgumentMode.register_AF:
                case ArgumentMode.register_BF:
                case ArgumentMode.register_CF:
                case ArgumentMode.register_DF:
                case ArgumentMode.SP_rel_address_short:
                case ArgumentMode.BP_rel_address_short:
                    if (m_CPUType < CPUType.BC16)
                    {
                        E_InvalidCPUFeature(CPUType.BC16, mode);
                    }
                    break;
                case ArgumentMode.immediate_dqword:
                case ArgumentMode.immediate_double:
                case ArgumentMode.register_EX:
                case ArgumentMode.register_FX:
                case ArgumentMode.register_GX:
                case ArgumentMode.register_HX:
                case ArgumentMode.SPX_rel_address_word:
                case ArgumentMode.BPX_rel_address_word:
                case ArgumentMode.register_AD:
                case ArgumentMode.register_BD:
                case ArgumentMode.register_CD:
                case ArgumentMode.register_DD:
                case ArgumentMode.SPX_rel_address_tbyte:
                case ArgumentMode.BPX_rel_address_tbyte:
                case ArgumentMode.SPX_rel_address_int:
                case ArgumentMode.BPX_rel_address_int:
                    if (m_CPUType < CPUType.BC32)
                    {
                        E_InvalidCPUFeature(CPUType.BC32, mode);
                    }
                    break;
                case ArgumentMode.none:
                    break;
                default:
                    break;
            }

            if (argument1 == ArgumentMode.none)
            {
                argument1 = mode;
                return;
            }
            if (argument2 == ArgumentMode.none)
            {
                argument2 = mode;
            }
        }
    }
}
