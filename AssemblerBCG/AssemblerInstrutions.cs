using static HexLibrary.HexConverter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AssemblerBCG
{
    public class AssemblerInstrutions : AssemblerStructs
    {
        ArgumentModeOld argument1 = 0;
        ArgumentModeOld argument2 = 0;

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

            if (!Enum.TryParse(instruction, true, out OLdInstruction result))
            {
                Console.WriteLine($"Bad Instruction {instruction} {m_file}:{Linenumber}");
                m_WriteOut = false;
                return;
            }

            OldInstructionInfo instructionInfo = OldInstructions.m_Instr[result];


            if (!string.IsNullOrEmpty(arguments[0]))
            {
                if (arguments.Length != instructionInfo.m_NumberOfOperands && !line.Contains(','))
                {
                    Console.WriteLine($"Expected a \',\' between operands {m_file}:{Linenumber}");
                    m_WriteOut = false;
                    return;
                }
                if (arguments.Length != instructionInfo.m_NumberOfOperands)
                {
                    Console.WriteLine($"Invalid Instruction {instruction} {m_file}:{Linenumber}");
                    m_WriteOut = false;
                    return;
                }
            }

            parseInstrutionArguments(result, arguments, ref InstructionBytes, startingIndex: 0);

            m_Output.AddRange(InstructionBytes);
        }

        private void parseInstrutionArguments(OLdInstruction instruction, string[] arguments, ref List<string> instructionBytes, int startingIndex = 0)
        {
            argument1 = ArgumentModeOld.none;
            argument2 = ArgumentModeOld.none;
            string argument1data = null;
            string argument2data = null;
            List<string> argumentBuffer = new List<string>();
            SizeAlignment sizeAlignment = SizeAlignment.none;

            switch (instruction)
            {
                case OLdInstruction.MOV:
                case OLdInstruction.MOVRAL:
                case OLdInstruction.MOVRBL:
                case OLdInstruction.MOVRCL:
                case OLdInstruction.MOVRDL:
                case OLdInstruction.OUTB:
                case OLdInstruction.INP:
                case OLdInstruction.MOVRALCR0:
                case OLdInstruction.MOVRCR0AL:
                    sizeAlignment = SizeAlignment._byte;
                    break;
                case OLdInstruction.MOVW:
                case OLdInstruction.MOVWRA:
                case OLdInstruction.MOVWRB:
                case OLdInstruction.MOVWRC:
                case OLdInstruction.MOVWRD:
                case OLdInstruction.OUTW:
                case OLdInstruction.INPW:
                case OLdInstruction.MOVWRCR0A:
                case OLdInstruction.MOVWRACR0:
                    sizeAlignment = SizeAlignment._word;
                    break;
                case OLdInstruction.MOVT:
                    sizeAlignment = SizeAlignment._tbyte;
                    break;
                case OLdInstruction.MOVD:
                case OLdInstruction.MOVDRAX:
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
                if (!ParseTerm(argument, ref sizeAlignment, out ArgumentModeOld argumentMode, out string[] data))
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
                //Console.WriteLine($"argument {argument}, argumentMode {argumentMode} with {result}");

                switch (argumentMode)
                {
                    case ArgumentModeOld.immediate_byte:
                    case ArgumentModeOld.immediate_word:
                    case ArgumentModeOld.immediate_tbyte:
                    case ArgumentModeOld.immediate_dword:
                    case ArgumentModeOld.immediate_qword:
                    case ArgumentModeOld.immediate_dqword:
                    case ArgumentModeOld.immediate_float:
                    case ArgumentModeOld.immediate_double:
                    case ArgumentModeOld.register:
                    case ArgumentModeOld.register_address:
                    case ArgumentModeOld.relative_address:
                    case ArgumentModeOld.near_address:
                    case ArgumentModeOld.short_address:
                    case ArgumentModeOld.long_address:
                    case ArgumentModeOld.far_address:
                    case ArgumentModeOld.X_indexed_address:
                    case ArgumentModeOld.Y_indexed_address:
                    case ArgumentModeOld.SP_rel_address_byte:
                    case ArgumentModeOld.BP_rel_address_byte:
                    case ArgumentModeOld.SPX_rel_address_word:
                    case ArgumentModeOld.BPX_rel_address_word:
                    case ArgumentModeOld.SP_rel_address_short:
                    case ArgumentModeOld.BP_rel_address_short:
                    case ArgumentModeOld.SPX_rel_address_tbyte:
                    case ArgumentModeOld.BPX_rel_address_tbyte:
                    case ArgumentModeOld.SPX_rel_address_int:
                    case ArgumentModeOld.BPX_rel_address_int:
                        if (argumentMode == ArgumentModeOld.register)
                        {
                            sizeAlignment = GetAlignmentFromRegister((Register)Convert.ToInt16(result, 16)) + 1;
                        }
                        if (data != null)
                        {
                            argumentBuffer.AddRange(data);
                        }
                        SetArgumentMode(argumentMode);
                        break;
                    case ArgumentModeOld.segment_address:
                    case ArgumentModeOld.segment_DS_register:
                    case ArgumentModeOld.segment_ES_register:
                        //Console.WriteLine($"data = {data[0]} {m_file}:{Linenumber} as {argumentMode}");
                        Register segment = Enum.Parse<Register>(data[0].Split(':')[0], true);
                        Register offset = Enum.Parse<Register>(data[0].Split(':')[1], true);

                        if (argumentMode == ArgumentModeOld.segment_address)
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
                    case ArgumentModeOld.segment_DS_B:
                    case ArgumentModeOld.segment_ES_B:
                    case ArgumentModeOld.register_AL:
                    case ArgumentModeOld.register_BL:
                    case ArgumentModeOld.register_CL:
                    case ArgumentModeOld.register_DL:
                    case ArgumentModeOld.register_A:
                    case ArgumentModeOld.register_B:
                    case ArgumentModeOld.register_C:
                    case ArgumentModeOld.register_D:
                    case ArgumentModeOld.register_H:
                    case ArgumentModeOld.register_L:
                    case ArgumentModeOld.register_address_HL:
                    case ArgumentModeOld.register_AX:
                    case ArgumentModeOld.register_BX:
                    case ArgumentModeOld.register_CX:
                    case ArgumentModeOld.register_DX:
                    case ArgumentModeOld.register_EX:
                    case ArgumentModeOld.register_FX:
                    case ArgumentModeOld.register_GX:
                    case ArgumentModeOld.register_HX:
                    case ArgumentModeOld.register_AF:
                    case ArgumentModeOld.register_BF:
                    case ArgumentModeOld.register_CF:
                    case ArgumentModeOld.register_DF:
                    case ArgumentModeOld.register_AD:
                    case ArgumentModeOld.register_BD:
                    case ArgumentModeOld.register_CD:
                    case ArgumentModeOld.register_DD:
                    case ArgumentModeOld.none:
                    default:
                        SetArgumentMode(argumentMode);
                        break;
                }

                switch (argumentMode)
                {
                    case ArgumentModeOld.register_AL:
                    case ArgumentModeOld.register_BL:
                    case ArgumentModeOld.register_CL:
                    case ArgumentModeOld.register_DL:
                        sizeAlignment = SizeAlignment._byte;
                        break;
                    case ArgumentModeOld.register_A:
                    case ArgumentModeOld.register_B:
                    case ArgumentModeOld.register_C:
                    case ArgumentModeOld.register_D:
                    case ArgumentModeOld.register_H:
                    case ArgumentModeOld.register_L:
                        sizeAlignment = SizeAlignment._word;
                        break;
                    case ArgumentModeOld.register_CX:
                    case ArgumentModeOld.register_DX:
                    case ArgumentModeOld.register_EX:
                    case ArgumentModeOld.register_FX:
                    case ArgumentModeOld.register_GX:
                    case ArgumentModeOld.register_HX:
                    case ArgumentModeOld.register_AX:
                    case ArgumentModeOld.register_BX:
                    case ArgumentModeOld.register_AF:
                    case ArgumentModeOld.register_BF:
                    case ArgumentModeOld.register_CF:
                    case ArgumentModeOld.register_DF:
                        sizeAlignment = SizeAlignment._dword;
                        break;
                    case ArgumentModeOld.register_AD:
                    case ArgumentModeOld.register_BD:
                    case ArgumentModeOld.register_CD:
                    case ArgumentModeOld.register_DD:
                        sizeAlignment = SizeAlignment._qword;
                        break;
                }
            }

            switch (instruction)
            {
                case OLdInstruction.PUSH:
                case OLdInstruction.POP:
                    switch (argument1)
                    {
                        case ArgumentModeOld.register:
                        case ArgumentModeOld.register_AL:
                        case ArgumentModeOld.register_BL:
                        case ArgumentModeOld.register_CL:
                        case ArgumentModeOld.register_DL:
                        case ArgumentModeOld.register_A:
                        case ArgumentModeOld.register_B:
                        case ArgumentModeOld.register_C:
                        case ArgumentModeOld.register_D:
                        case ArgumentModeOld.register_H:
                        case ArgumentModeOld.register_L:
                        case ArgumentModeOld.register_AX:
                        case ArgumentModeOld.register_BX:
                        case ArgumentModeOld.register_CX:
                        case ArgumentModeOld.register_DX:
                        case ArgumentModeOld.register_EX:
                        case ArgumentModeOld.register_FX:
                        case ArgumentModeOld.register_GX:
                        case ArgumentModeOld.register_HX:
                        case ArgumentModeOld.register_AF:
                        case ArgumentModeOld.register_BF:
                        case ArgumentModeOld.register_CF:
                        case ArgumentModeOld.register_DF:
                        case ArgumentModeOld.register_AD:
                        case ArgumentModeOld.register_BD:
                        case ArgumentModeOld.register_CD:
                        case ArgumentModeOld.register_DD:
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

            OldInstructionInfo instructionInfo = OldInstructions.m_Instr[instruction];

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
                case OLdInstruction.MOV:
                    if (argument1 == ArgumentModeOld.register_AL)
                    {
                        argument1 = ArgumentModeOld.none;
                        if (m_CPUType < CPUType.BC32 && argument2 == ArgumentModeOld.register && Enum.TryParse(FromHexString(argument2data).ToString(), true, out Register result) && result == Register.CR0)
                        {
                            argument2 = ArgumentModeOld.none;
                            instruction = OLdInstruction.MOVRALCR0;
                            argumentBuffer.Clear();
                            break;
                        }
                        instruction = OLdInstruction.MOVRAL;
                        break;
                    }
                    else if (argument1 == ArgumentModeOld.register_BL)
                    {
                        argument1 = ArgumentModeOld.none;
                        instruction = OLdInstruction.MOVRBL;
                        break;
                    }
                    else if (argument1 == ArgumentModeOld.register_CL)
                    {
                        argument1 = ArgumentModeOld.none;
                        instruction = OLdInstruction.MOVRCL;
                        break;
                    }
                    else if (argument1 == ArgumentModeOld.register_DL)
                    {
                        argument1 = ArgumentModeOld.none;
                        instruction = OLdInstruction.MOVRDL;
                        break;
                    }
                    else if (m_CPUType < CPUType.BC32 && argument1 == ArgumentModeOld.register && Enum.TryParse(FromHexString(argument1data).ToString(), true, out Register result) && result == Register.CR0)
                    {
                        if (argument2 == ArgumentModeOld.register_AL)
                        {
                            argumentBuffer.Clear();
                            argument1 = ArgumentModeOld.none;
                            argument2 = ArgumentModeOld.none;
                            instruction = OLdInstruction.MOVRCR0AL;
                        }
                    }
                    break;
                case OLdInstruction.MOVW:
                    if (argument1 == ArgumentModeOld.register_A)
                    {
                        argument1 = ArgumentModeOld.none;
                        if (m_CPUType >= CPUType.BC32 && argument2 == ArgumentModeOld.register && Enum.TryParse(FromHexString(argument2data).ToString(), true, out Register result) && result == Register.CR0)
                        {
                            argument2 = ArgumentModeOld.none;
                            instruction = OLdInstruction.MOVWRACR0;
                            argumentBuffer.Clear();
                            break;
                        }
                        instruction = OLdInstruction.MOVWRA;
                        break;
                    }
                    else if (argument1 == ArgumentModeOld.register_B)
                    {
                        argument1 = ArgumentModeOld.none;
                        instruction = OLdInstruction.MOVWRB;
                        break;
                    }
                    else if (argument1 == ArgumentModeOld.register_C)
                    {
                        argument1 = ArgumentModeOld.none;
                        instruction = OLdInstruction.MOVWRC;
                        break;
                    }
                    else if (argument1 == ArgumentModeOld.register_D)
                    {
                        argument1 = ArgumentModeOld.none;
                        instruction = OLdInstruction.MOVWRD;
                        break;
                    }
                    else if (m_CPUType >= CPUType.BC32 && argument1 == ArgumentModeOld.register && Enum.TryParse(FromHexString(argument1data).ToString(), true, out Register result) && result == Register.CR0)
                    {
                        if (argument2 == ArgumentModeOld.register_A)
                        {
                            argumentBuffer.Clear();
                            argument1 = ArgumentModeOld.none;
                            argument2 = ArgumentModeOld.none;
                            instruction = OLdInstruction.MOVWRCR0A;
                        }
                    }
                    break;
                case OLdInstruction.MOVD:
                    if (argument1 == ArgumentModeOld.register_AX)
                    {
                        argument1 = ArgumentModeOld.none;
                        instruction = OLdInstruction.MOVDRAX;
                        break;
                    }
                    else if (argument1 == ArgumentModeOld.register_BX)
                    {
                        argument1 = ArgumentModeOld.none;
                        instruction = OLdInstruction.MOVDRBX;
                        break;
                    }
                    else if (argument1 == ArgumentModeOld.register_CX)
                    {
                        argument1 = ArgumentModeOld.none;
                        instruction = OLdInstruction.MOVDRCX;
                        break;
                    }
                    else if (argument1 == ArgumentModeOld.register_DX)
                    {
                        argument1 = ArgumentModeOld.none;
                        instruction = OLdInstruction.MOVDRDX;
                        break;
                    }
                    break;
                case OLdInstruction.SEZ:
                    if (argument1 == ArgumentModeOld.register_AL)
                    {
                        argument1 = ArgumentModeOld.none;
                        instruction = OLdInstruction.SEZRAL;
                        break;
                    }
                    else if (argument1 == ArgumentModeOld.register_BL)
                    {
                        argument1 = ArgumentModeOld.none;
                        instruction = OLdInstruction.SEZRBL;
                        break;
                    }
                    else if (argument1 == ArgumentModeOld.register_CL)
                    {
                        argument1 = ArgumentModeOld.none;
                        instruction = OLdInstruction.SEZRCL;
                        break;
                    }
                    else if (argument1 == ArgumentModeOld.register_DL)
                    {
                        argument1 = ArgumentModeOld.none;
                        instruction = OLdInstruction.SEZRDL;
                        break;
                    }
                    else if (argument1 == ArgumentModeOld.register_A)
                    {
                        argument1 = ArgumentModeOld.none;
                        instruction = OLdInstruction.SEZRA;
                        break;
                    }
                    else if (argument1 == ArgumentModeOld.register_B)
                    {
                        argument1 = ArgumentModeOld.none;
                        instruction = OLdInstruction.SEZRB;
                        break;
                    }
                    else if (argument1 == ArgumentModeOld.register_C)
                    {
                        argument1 = ArgumentModeOld.none;
                        instruction = OLdInstruction.SEZRC;
                        break;
                    }
                    else if (argument1 == ArgumentModeOld.register_D)
                    {
                        argument1 = ArgumentModeOld.none;
                        instruction = OLdInstruction.SEZRD;
                        break;
                    }
                    else if (argument1 == ArgumentModeOld.register_AX)
                    {
                        argument1 = ArgumentModeOld.none;
                        instruction = OLdInstruction.SEZRAX;
                        break;
                    }
                    else if (argument1 == ArgumentModeOld.register_BX)
                    {
                        argument1 = ArgumentModeOld.none;
                        instruction = OLdInstruction.SEZRBX;
                        break;
                    }
                    else if (argument1 == ArgumentModeOld.register_CX)
                    {
                        argument1 = ArgumentModeOld.none;
                        instruction = OLdInstruction.SEZRCX;
                        break;
                    }
                    else if (argument1 == ArgumentModeOld.register_DX)
                    {
                        argument1 = ArgumentModeOld.none;
                        instruction = OLdInstruction.SEZRDX;
                        break;
                    }
                    break;
                case OLdInstruction.TEST:
                    if (argument1 == ArgumentModeOld.register_AL)
                    {
                        argument1 = ArgumentModeOld.none;
                        instruction = OLdInstruction.TESTRAL;
                        break;
                    }
                    else if (argument1 == ArgumentModeOld.register_BL)
                    {
                        argument1 = ArgumentModeOld.none;
                        instruction = OLdInstruction.TESTRBL;
                        break;
                    }
                    else if (argument1 == ArgumentModeOld.register_CL)
                    {
                        argument1 = ArgumentModeOld.none;
                        instruction = OLdInstruction.TESTRCL;
                        break;
                    }
                    else if (argument1 == ArgumentModeOld.register_DL)
                    {
                        argument1 = ArgumentModeOld.none;
                        instruction = OLdInstruction.TESTRDL;
                        break;
                    }
                    else if (argument1 == ArgumentModeOld.register_A)
                    {
                        argument1 = ArgumentModeOld.none;
                        instruction = OLdInstruction.TESTRA;
                        break;
                    }
                    else if (argument1 == ArgumentModeOld.register_B)
                    {
                        argument1 = ArgumentModeOld.none;
                        instruction = OLdInstruction.TESTRB;
                        break;
                    }
                    else if (argument1 == ArgumentModeOld.register_C)
                    {
                        argument1 = ArgumentModeOld.none;
                        instruction = OLdInstruction.TESTRC;
                        break;
                    }
                    else if (argument1 == ArgumentModeOld.register_D)
                    {
                        argument1 = ArgumentModeOld.none;
                        instruction = OLdInstruction.TESTRD;
                        break;
                    }
                    else if (argument1 == ArgumentModeOld.register_AX)
                    {
                        argument1 = ArgumentModeOld.none;
                        instruction = OLdInstruction.TESTRAX;
                        break;
                    }
                    else if (argument1 == ArgumentModeOld.register_BX)
                    {
                        argument1 = ArgumentModeOld.none;
                        instruction = OLdInstruction.TESTRBX;
                        break;
                    }
                    else if (argument1 == ArgumentModeOld.register_CX)
                    {
                        argument1 = ArgumentModeOld.none;
                        instruction = OLdInstruction.TESTRCX;
                        break;
                    }
                    else if (argument1 == ArgumentModeOld.register_DX)
                    {
                        argument1 = ArgumentModeOld.none;
                        instruction = OLdInstruction.TESTRDX;
                        break;
                    }
                    break;

                case OLdInstruction.CMP:
                    if (argument1 == ArgumentModeOld.register_A)
                    {
                        instruction = OLdInstruction.CMPRA;
                        argument1 = ArgumentModeOld.none;
                        break;
                    }
                    else if (argument1 == ArgumentModeOld.register_AX)
                    {
                        instruction = OLdInstruction.CMPRAX;
                        argument1 = ArgumentModeOld.none;
                        break;
                    }
                    else if (argument2 == ArgumentModeOld.immediate_byte && argument2data == "0")
                    {
                        argument2 = ArgumentModeOld.none;
                        instruction = OLdInstruction.CMPZ;
                    }
                    break;

                case OLdInstruction.ADD:
                case OLdInstruction.ADC:
                case OLdInstruction.SUB:
                case OLdInstruction.SBB:
                case OLdInstruction.MUL:
                case OLdInstruction.DIV:
                case OLdInstruction.AND:
                case OLdInstruction.OR:
                case OLdInstruction.NOR:
                case OLdInstruction.XOR:
                case OLdInstruction.NOT:
                    if (argument1 == ArgumentModeOld.register_A)
                    {
                        instruction = instruction + 1;
                        argument1 = ArgumentModeOld.none;
                        break;
                    }
                    else if (argument1 == ArgumentModeOld.register_AX)
                    {
                        instruction = instruction + 2;
                        argument1 = ArgumentModeOld.none;
                        break;
                    }
                    break;
                default:
                    break;
            }

            // Console.WriteLine($"instruction: {instruction} {argument1}, {argument2}");

            string instr = Convert.ToString((ushort)instruction, 16).PadLeft(4, '0');
            instructionBytes.AddRange(SplitHexString(instr));
            if (argument1 != ArgumentModeOld.none)
            {
                string arg1 = Convert.ToString((ushort)argument1, 16).PadLeft(2, '0');
                instructionBytes.Add(arg1);
            }
            if (argument2 != ArgumentModeOld.none)
            {
                string arg2 = Convert.ToString((ushort)argument2, 16).PadLeft(2, '0');
                instructionBytes.Add(arg2);
            }

            instructionBytes.AddRange(argumentBuffer);
        }

        void SetArgumentMode(ArgumentModeOld mode) 
        {
            switch (mode)
            {
                case ArgumentModeOld.immediate_byte:
                case ArgumentModeOld.immediate_word:
                case ArgumentModeOld.register:
                case ArgumentModeOld.register_AL:
                case ArgumentModeOld.register_BL:
                case ArgumentModeOld.register_CL:
                case ArgumentModeOld.register_DL:
                case ArgumentModeOld.register_H:
                case ArgumentModeOld.register_L:
                case ArgumentModeOld.register_address:
                case ArgumentModeOld.register_address_HL:
                case ArgumentModeOld.relative_address:
                case ArgumentModeOld.near_address:
                case ArgumentModeOld.short_address:
                case ArgumentModeOld.segment_address:
                case ArgumentModeOld.segment_DS_register:
                case ArgumentModeOld.segment_DS_B:
                case ArgumentModeOld.SP_rel_address_byte:
                case ArgumentModeOld.BP_rel_address_byte:
                    if (m_CPUType < CPUType.BC8)
                    {
                        E_InvalidCPUFeature(CPUType.BC8, mode);
                    }
                    break;
                case ArgumentModeOld.immediate_tbyte:
                case ArgumentModeOld.immediate_dword:
                case ArgumentModeOld.immediate_qword:
                case ArgumentModeOld.immediate_float:
                case ArgumentModeOld.register_A:
                case ArgumentModeOld.register_B:
                case ArgumentModeOld.register_C:
                case ArgumentModeOld.register_D:
                case ArgumentModeOld.register_AX:
                case ArgumentModeOld.register_BX:
                case ArgumentModeOld.register_CX:
                case ArgumentModeOld.register_DX:
                case ArgumentModeOld.long_address:
                case ArgumentModeOld.far_address:
                case ArgumentModeOld.X_indexed_address:
                case ArgumentModeOld.Y_indexed_address:
                case ArgumentModeOld.segment_ES_register:
                case ArgumentModeOld.segment_ES_B:
                case ArgumentModeOld.register_AF:
                case ArgumentModeOld.register_BF:
                case ArgumentModeOld.register_CF:
                case ArgumentModeOld.register_DF:
                case ArgumentModeOld.SP_rel_address_short:
                case ArgumentModeOld.BP_rel_address_short:
                    if (m_CPUType < CPUType.BC16)
                    {
                        E_InvalidCPUFeature(CPUType.BC16, mode);
                    }
                    break;
                case ArgumentModeOld.immediate_dqword:
                case ArgumentModeOld.immediate_double:
                case ArgumentModeOld.register_EX:
                case ArgumentModeOld.register_FX:
                case ArgumentModeOld.register_GX:
                case ArgumentModeOld.register_HX:
                case ArgumentModeOld.SPX_rel_address_word:
                case ArgumentModeOld.BPX_rel_address_word:
                case ArgumentModeOld.register_AD:
                case ArgumentModeOld.register_BD:
                case ArgumentModeOld.register_CD:
                case ArgumentModeOld.register_DD:
                case ArgumentModeOld.SPX_rel_address_tbyte:
                case ArgumentModeOld.BPX_rel_address_tbyte:
                case ArgumentModeOld.SPX_rel_address_int:
                case ArgumentModeOld.BPX_rel_address_int:
                    if (m_CPUType < CPUType.BC32)
                    {
                        E_InvalidCPUFeature(CPUType.BC32, mode);
                    }
                    break;
                case ArgumentModeOld.none:
                    break;
                default:
                    break;
            }

            if (argument1 == ArgumentModeOld.none)
            {
                argument1 = mode;
                return;
            }
            if (argument2 == ArgumentModeOld.none)
            {
                argument2 = mode;
            }
        }
    }
}
