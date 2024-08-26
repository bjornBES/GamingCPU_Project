using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using assembler.global;

namespace assembler
{
    public class Generator
    {
        Token[] m_tokens;
        int m_index = 0;

        public string[] m_Src;
        string m_file;

        public List<Struct> m_Structs = new List<Struct>();

        public List<string[]> m_OutputBin = new List<string[]>();

        public List<string> m_FileBin = new List<string>();

        public ushort BP_ref = 0;

        object parse(out Arguments arguments, out bool Split)
        {
            Token _out;
            if (!peek().HasValue)
            {
                throw new NotImplementedException();
            }

            if (try_consume(TokenType.open_square))
            {
                object output = null;
                arguments = Arguments.none;
                Split = false;
                if (try_consume(TokenType.open_paren))
                {
                    arguments = Arguments.address;
                    if (peek().Value.type == TokenType.register)
                    {
                        arguments = Arguments.reg_addr;
                    }
                    else
                    {
                        Console.WriteLine("No argument found");
                    }
                    output = parse_expr(TokenType.close_paren);
                    Split = false;
                }
                else if (peek().Value.type == TokenType.register)
                {
                    Register register = (Register)Enum.Parse(typeof(Register), consume().ident.ToUpper());
                    RegisterInfo registerInfo = AllRegister.s_registerInfos.Find(reg => reg == register);

                    arguments = Arguments.reg_addr;

                    Split = true;
                    output = registerInfo.GetCode();

                    if (try_consume(TokenType.colon))
                    {
                        if (peek().HasValue && peek().Value.type == TokenType.register)
                        {
                            Register register2 = (Register)Enum.Parse(typeof(Register), consume().ident.ToUpper());
                            RegisterInfo registerInfo2 = AllRegister.s_registerInfos.Find(reg => reg == register2);

                            output += registerInfo2.GetCode();
                            arguments = Arguments.segment_reg_reg;
                        }
                        else if (peek().HasValue && peek().Value.type == TokenType.int_lit)
                        {
                            string lit = Convert.ToString(Convert.ToInt32(consume().ident), 16);

                            output += lit.PadLeft(4, '0');
                            arguments = Arguments.segment_reg_imm;
                        }
                    }
                }
                else if (peek().Value.type == TokenType.int_lit)
                {
                    string lit = Convert.ToString(Convert.ToInt32(consume().ident), 16);
                    lit = lit.PadLeft(4, '0');
                    arguments = Arguments.address;
                    Split = true;
                    output = lit;
                    if (try_consume(TokenType.colon))
                    {
                        if (peek().HasValue && peek().Value.type == TokenType.register)
                        {
                            Register register = (Register)Enum.Parse(typeof(Register), consume().ident.ToUpper());
                            RegisterInfo registerInfo = AllRegister.s_registerInfos.Find(reg => reg == register);

                            output += registerInfo.GetCode();
                            arguments = Arguments.segment_imm_reg;
                        }
                    }
                }
                else if (peek().Value.type == TokenType.address_label)
                {
                    arguments = Arguments.address;
                    string name = consume().ident;
                    Split = false;
                    output = $"L_{name}";
                }
                else if (peek().Value.type == TokenType.dollar_sign)
                {
                    consume();
                    arguments = Arguments.address;
                    Split = false;
                    try_consume_error(TokenType.close_square);
                    return $"_CA_";
                }
                else if (try_consume(TokenType.at))
                {
                    if (try_consume(TokenType.address_label, out _out))
                    {
                        Split = false;
                        arguments = Arguments.address;
                        output = $"R_{_out.ident}";
                        if (try_consume(TokenType.colon))
                        {
                            if (peek().HasValue && peek().Value.type == TokenType.register)
                            {
                                Register register = (Register)Enum.Parse(typeof(Register), consume().ident.ToUpper());
                                RegisterInfo registerInfo = AllRegister.s_registerInfos.Find(reg => reg == register);

                                output = $"_SIR{_out.ident}:{registerInfo.GetCode()}";
                                arguments = Arguments.segment_imm_reg;
                            }
                        }
                    }
                }
                try_consume_error(TokenType.close_square);
                return output;
            }
            else if (try_consume(TokenType._long))
            {
                try_consume(TokenType.open_square);

                if (peek().Value.type == TokenType.int_lit)
                {
                    string lit = Convert.ToString(Convert.ToInt32(peek().Value.ident), 16);
                    consume();
                    lit = lit.PadLeft(8, '0');
                    arguments = Arguments.long_address;
                    Split = true;
                    try_consume_error(TokenType.close_square);
                    return lit;
                }
                else if (peek().Value.type == TokenType.address_label)
                {
                    arguments = Arguments.long_address;
                    object a_output = consume().ident;
                    Split = false;
                    try_consume_error(TokenType.close_square);
                    return $"LL_{a_output}";
                }
                else if (peek().Value.type == TokenType.dollar_sign)
                {
                    consume();
                    arguments = Arguments.long_address;
                    Split = false;
                    try_consume_error(TokenType.close_square);
                    return $"_LCA_";
                }
                else if (try_consume(TokenType.at))
                {
                    if (try_consume(TokenType.address_label, out _out))
                    {
                        Split = false;
                        arguments = Arguments.imm24;
                        if (try_consume(TokenType.close_square))
                        {
                            arguments = Arguments.long_address;
                        }
                        return $"LR_{_out.ident}";
                    }
                }
            }
            else if (try_consume(TokenType.register, out _out))
            {
                Register register = (Register)Enum.Parse(typeof(Register), _out.ident.ToUpper());
                RegisterInfo registerInfo = AllRegister.s_registerInfos.Find(reg => reg == register);

                arguments = registerInfo.m_ArgumentStyle;
                Split = true;
                return registerInfo.GetCode();
            }
            else if (try_consume(TokenType.open_paren))
            {
                arguments = Arguments.imm24;
                if (peek().Value.type == TokenType.register)
                {
                    arguments = Arguments.reg;
                }
                else
                {
                    Console.WriteLine("No argument found");
                }

                Split = false;
                return parse_expr(TokenType.close_paren);
            }
            else if (try_consume(TokenType.int_lit, out _out))
            {
                string lit = Convert.ToString(Convert.ToInt32(_out.ident), 16);
                switch (lit.Length)
                {
                    case 1:
                    case 2:
                        lit = lit.PadLeft(2, '0');
                        arguments = Arguments.imm8;
                        break;
                    case 3:
                    case 4:
                        lit = lit.PadLeft(4, '0');
                        arguments = Arguments.imm16;
                        break;
                    case 5:
                    case 6:
                        lit = lit.PadLeft(6, '0');
                        arguments = Arguments.imm24;
                        break;
                    case 7:
                    case 8:
                        lit = lit.PadLeft(8, '0');
                        arguments = Arguments.imm32;
                        break;
                    default:
                        arguments = Arguments.none;
                        break;
                }
                Split = true;
                return lit;
            }
            else if (try_consume(TokenType.dollar_sign))
            {
                arguments = Arguments.imm16;
                Split = false;
                if (try_consume(TokenType.dollar_sign))
                {
                    return $"_CS_";
                }
                return $"_CA_";
            }
            else if (try_consume(TokenType.float_lit, out _out))
            {
                string data = _out.ident;

                // Convert the string to float
                float floatValueParsed = float.Parse(data);

                // Get the binary representation of the float
                int floatToInt = BitConverter.ToInt32(BitConverter.GetBytes(floatValueParsed), 0);
                string binaryString = Convert.ToString(floatToInt, 2);

                // Ensure the binary string is 32 bits long (for single precision float)
                binaryString = binaryString.PadLeft(32, '0');

                int binaryInt = Convert.ToInt32(binaryString, 2);

                string hexStrings = Convert.ToString(binaryInt, 16);

                Split = true;
                arguments = Arguments.f_imm;
                return hexStrings;
            }
            else if (try_consume(TokenType.at))
            {
                if (try_consume(TokenType.address_label, out _out))
                {
                    Split = false;
                    arguments = Arguments.imm16;
                    return $"R_{_out.ident}";
                }
            }
            else if (try_consume(TokenType.quotation_mark))
            {
                Token token = try_consume_error(TokenType.ident);
                try_consume_error(TokenType.quotation_mark);

                arguments = Arguments._string;
                Split = true;
                byte[] bytes = Encoding.ASCII.GetBytes(token.ident);
                string hexstring = "";
                for (int i = 0; i < bytes.Length; i++)
                {
                    hexstring += Convert.ToString(bytes[i], 16);
                }
                return hexstring;
            }

            arguments = Arguments.none;
            Split = false;
            return null;
        }

        Instruction GetInstruction()
        {
            Instruction instruction = Enum.Parse<Instruction>(consume().ident);
            if (peek().Value.type == TokenType._byte || peek().Value.type == TokenType._word ||
                peek().Value.type == TokenType._tbyte|| peek().Value.type == TokenType._dword ||
                peek().Value.type == TokenType._float)
            {
                if (peek().HasValue && peek().Value.type == TokenType._byte)
                {
                    switch (instruction)
                    {
                        case Instruction.MOV: 
                        case Instruction.MOVW: 
                        case Instruction.MOVT: 
                        case Instruction.MOVD: 
                            instruction = Instruction.MOV; break;
                        case Instruction.POP:
                        case Instruction.POPW:
                        case Instruction.POPT:
                        case Instruction.POPD:
                            instruction = Instruction.POP;
                            break;
                        default:
                            break;
                    }
                }
                else if (peek().HasValue && peek().Value.type == TokenType._word)
                {
                    switch (instruction)
                    {
                        case Instruction.MOV:
                        case Instruction.MOVW:
                        case Instruction.MOVT:
                        case Instruction.MOVD:
                            instruction = Instruction.MOVW;
                            break;
                        case Instruction.POP:
                        case Instruction.POPW:
                        case Instruction.POPT:
                        case Instruction.POPD:
                            instruction = Instruction.POPW;
                            break;
                        default:
                            break;
                    }
                }
                else if (peek().HasValue && peek().Value.type == TokenType._tbyte)
                {
                    switch (instruction)
                    {
                        case Instruction.MOV:
                        case Instruction.MOVW:
                        case Instruction.MOVT:
                        case Instruction.MOVD:
                            instruction = Instruction.MOVT;
                            break;
                        case Instruction.POP:
                        case Instruction.POPW:
                        case Instruction.POPT:
                        case Instruction.POPD:
                            instruction = Instruction.POPT;
                            break;
                        default:
                            break;
                    }
                }
                else if (peek().HasValue && peek().Value.type == TokenType._dword)
                {
                    switch (instruction)
                    {
                        case Instruction.MOV:
                        case Instruction.MOVW:
                        case Instruction.MOVT:
                        case Instruction.MOVD:
                            instruction = Instruction.MOVD;
                            break;
                        case Instruction.POP:
                        case Instruction.POPW:
                        case Instruction.POPT:
                        case Instruction.POPD:
                            instruction = Instruction.POPD;
                            break;
                        default:
                            break;
                    }
                }
                else if (peek().HasValue && peek().Value.type == TokenType._float)
                {
                    switch (instruction)
                    {
                        case Instruction.MOV:
                        case Instruction.MOVW:
                        case Instruction.MOVT:
                        case Instruction.MOVD:
                        case Instruction.MOVF:
                            instruction = Instruction.MOVF;
                            break;
                        default:
                            break;
                    }
                }
                consume();
            }

            return instruction;
        }

        public void build()
        {
            if(peek().Value.type == TokenType.instruction)
            {
                Instruction instruction = GetInstruction();
                InstructionInfo instructionInfo = AllInstruction.s_instructionInfo.Find(
                    instr => instr.m_Instruction == instruction);

                object arg1 = null;
                object arg2 = null;

                Arguments argument1 = Arguments.none;
                Arguments argument2 = Arguments.none;

                bool DoSplitArg1 = false;
                bool DoSplitArg2 = false;

                for (int i = 0; i < instructionInfo.m_ArgumentSize; i++)
                {
                    if (arg1 == null)
                    {
                        arg1 = parse(out argument1, out DoSplitArg1);
                    }
                    else
                    {
                        try_consume_error(TokenType.comma);
                        arg2 = parse(out argument2, out DoSplitArg2);
                    }
                }

                if (instruction == Instruction.PUSHA)
                {
                    switch (argument1)
                    {
                        case Arguments.reg8:
                        case Arguments.imm8:
                            BP_ref++;
                            break;
                        case Arguments.reg16:
                        case Arguments.imm16:
                            BP_ref += 2;
                            break;
                        case Arguments.imm24:
                            BP_ref += 3;
                            break;
                        case Arguments.reg32:
                        case Arguments.imm32:
                            BP_ref += 4;
                            break;
                    }
                }


                instructionInfo = FindInstruction(instruction, argument1, ref arg1, argument2, ref arg2);

                if (instructionInfo.m_Opcode == null)
                {
                    if (arg1 == null)
                    {
                        AssemblerError.InvalidOperand(peek(-1).Value, m_file, m_Src[peek(-1).Value.line + 1]);
                    }
                }

                if (argument2 == default)
                {
                    m_FileBin.Add($"_DEL_ {instructionInfo.m_Instruction} {arg1}");
                }
                else if (argument1 == default)
                {
                    m_FileBin.Add($"_DEL_ {instructionInfo.m_Instruction}");
                }
                else
                {
                    m_FileBin.Add($"_DEL_ {instructionInfo.m_Instruction} {arg1}, {arg2}");
                }

                if (instructionInfo.m_Opcode == null)
                {
                    AssemblerError.InvalidInstruction(peek(-1).Value, m_file, m_Src[peek(-1).Value.line + 1]);
                }

                m_FileBin.AddRange(SplitHexString(instructionInfo.m_Opcode));
                if (arg1 != null)
                {
                    m_FileBin.Add("_NEWARG_");
                    if (DoSplitArg1)
                    {
                        m_FileBin.AddRange(SplitHexString(arg1.ToString()));
                    }
                    else
                    {
                        m_FileBin.Add(arg1.ToString());
                    }
                }

                if (arg2 != null)
                {
                    m_FileBin.Add("_NEWARG_");
                    if (DoSplitArg2)
                    {
                        m_FileBin.AddRange(SplitHexString(arg2.ToString()));
                    }
                    else
                    {
                        m_FileBin.Add(arg2.ToString());
                    }
                }

            }
            else if (try_consume(TokenType.newfile))
            {
                m_file = peek().Value.ident;
                if (m_FileBin.Count != 0)
                {
                    m_OutputBin.Add(m_FileBin.ToArray());
                }
                m_FileBin.Clear();
                Thread.Sleep(250);
                m_FileBin.Add($"_FILE_ {peek().Value.ident}");
            }
            else if (try_consume(TokenType.newline))
            {
                m_FileBin.Add("_NEWLINE_");
            }
            else if (try_consume(TokenType._byte))
            {
                while (peek().HasValue)
                {

                    object obj = parse(out Arguments size, out bool split);

                    switch (size)
                    {
                        case Arguments._string:
                        case Arguments.imm8:
                            break;
                        case Arguments.imm16:
                        case Arguments.imm24:
                        case Arguments.imm32:
                        case Arguments.f_imm:
                            AssemblerWarnings.OperandToBig(peek(-1).Value, m_file, m_Src[peek(-1).Value.line + 1], 1);
                            break;
                        default:
                        case Arguments.address:
                        case Arguments.reg8:
                        case Arguments.reg16:
                        case Arguments.reg32:
                        case Arguments.reg:
                        case Arguments.reg_addr:
                            AssemblerError.InvalidOperand(peek(-1).Value, m_file, m_Src[peek(-1).Value.line + 1]);
                            break;
                    }

                    if (obj == null)
                    {
                        AssemblerError.ExpectedExpr(peek(-1).Value, m_file);
                    }

                    if (split)
                    {
                        m_FileBin.AddRange(SplitHexString(obj.ToString()).ToArray());
                    }
                    else
                    {
                        m_FileBin.Add(obj.ToString());
                    }

                    if (peek().HasValue && peek().Value.type != TokenType.comma)
                    {
                        break;
                    }
                    else if (peek().HasValue && peek().Value.type == TokenType.comma)
                    {
                        consume();
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else if (try_consume(TokenType._word))
            {
                while (peek().HasValue)
                {

                    object obj = parse(out Arguments size, out bool split);

                    switch (size)
                    {
                        case Arguments._string:
                            break;
                        case Arguments.imm8:
                        case Arguments.imm16:
                            obj = obj.ToString().PadLeft(4, '0');
                            break;
                        case Arguments.imm24:
                        case Arguments.imm32:
                        case Arguments.f_imm:
                            AssemblerWarnings.OperandToBig(peek(-1).Value, m_file, m_Src[peek(-1).Value.line + 1], 2);
                            break;
                        default:
                        case Arguments.address:
                        case Arguments.reg8:
                        case Arguments.reg16:
                        case Arguments.reg32:
                        case Arguments.reg:
                        case Arguments.reg_addr:
                            AssemblerError.InvalidOperand(peek(-1).Value, m_file, m_Src[peek(-1).Value.line + 1]);
                            break;
                    }

                    if (split)
                    {
                        m_FileBin.AddRange(SplitHexString(obj.ToString()).ToArray());
                    }
                    else
                    {
                        m_FileBin.Add(obj.ToString());
                    }

                    if (peek().HasValue && peek().Value.type != TokenType.comma)
                    {
                        break;
                    }
                    else if (peek().HasValue && peek().Value.type == TokenType.comma)
                    {
                        consume();
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else if (try_consume(TokenType._tbyte))
            {
                while (peek().HasValue)
                {

                    object obj = parse(out Arguments size, out bool split);

                    switch (size)
                    {
                        case Arguments._string:
                            break;
                        case Arguments.imm8:
                        case Arguments.imm16:
                        case Arguments.imm24:
                            obj = obj.ToString().PadLeft(6, '0');
                            break;
                        case Arguments.imm32:
                        case Arguments.f_imm:
                            AssemblerWarnings.OperandToBig(peek(-1).Value, m_file, m_Src[peek(-1).Value.line + 1], 3);
                            break;
                        default:
                        case Arguments.address:
                        case Arguments.reg8:
                        case Arguments.reg16:
                        case Arguments.reg32:
                        case Arguments.reg:
                        case Arguments.reg_addr:
                            AssemblerError.InvalidOperand(peek(-1).Value, m_file, m_Src[peek(-1).Value.line + 1]);
                            break;
                    }

                    if (split)
                    {
                        m_FileBin.AddRange(SplitHexString(obj.ToString()).ToArray());
                    }
                    else
                    {
                        m_FileBin.Add(obj.ToString());
                    }

                    if (peek().HasValue && peek().Value.type != TokenType.comma)
                    {
                        break;
                    }
                    else if (peek().HasValue && peek().Value.type == TokenType.comma)
                    {
                        consume();
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else if (try_consume(TokenType._dword))
            {
                while (peek().HasValue)
                {

                    object obj = parse(out Arguments size, out bool split);

                    switch (size)
                    {
                        case Arguments._string:
                            break;
                        case Arguments.imm8:
                        case Arguments.imm16:
                        case Arguments.imm24:
                        case Arguments.imm32:
                            if(split)
                            {
                                obj = obj.ToString().PadLeft(8, '0');
                            }
                            break;
                        case Arguments.f_imm:
                            AssemblerWarnings.OperandToBig(peek(-1).Value, m_file, m_Src[peek(-1).Value.line + 1], 3);
                            break;
                        default:
                        case Arguments.address:
                        case Arguments.reg8:
                        case Arguments.reg16:
                        case Arguments.reg32:
                        case Arguments.reg:
                        case Arguments.reg_addr:
                            AssemblerError.InvalidOperand(peek(-1).Value, m_file, m_Src[peek(-1).Value.line + 1]);
                            break;
                    }

                    if (split)
                    {
                        m_FileBin.AddRange(SplitHexString(obj.ToString()).ToArray());
                    }
                    else
                    {
                        m_FileBin.Add(obj.ToString());
                    }

                    if (peek().HasValue && peek().Value.type != TokenType.comma)
                    {
                        break;
                    }
                    else if (peek().HasValue && peek().Value.type == TokenType.comma)
                    {
                        consume();
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else if (try_consume(TokenType._float))
            {
                while (peek().HasValue)
                {

                    object obj = parse(out Arguments size, out bool split);

                    switch (size)
                    {
                        case Arguments._string:
                            break;
                        case Arguments.imm8:
                        case Arguments.imm16:
                        case Arguments.imm24:
                        case Arguments.imm32:
                        case Arguments.f_imm:
                            obj = obj.ToString().PadLeft(8, '0');
                            break;
                        default:
                        case Arguments.address:
                        case Arguments.reg8:
                        case Arguments.reg16:
                        case Arguments.reg32:
                        case Arguments.reg:
                        case Arguments.reg_addr:
                            AssemblerError.InvalidOperand(peek(-1).Value, m_file, m_Src[peek(-1).Value.line + 1]);
                            break;
                    }

                    if (split)
                    {
                        m_FileBin.AddRange(SplitHexString(obj.ToString()).ToArray());
                    }
                    else
                    {
                        m_FileBin.Add(obj.ToString());
                    }

                    if (peek().HasValue && peek().Value.type != TokenType.comma)
                    {
                        break;
                    }
                    else if (peek().HasValue && peek().Value.type == TokenType.comma)
                    {
                        consume();
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else if (try_consume(TokenType.org))
            {
                object expr = parse(out Arguments argument, out _);
                if (argument == Arguments.imm8 || argument == Arguments.imm16 ||
                    argument == Arguments.imm24 || argument == Arguments.imm32)
                {
                    m_FileBin.Add($"_OFF_ {expr}");
                }
                else
                {
                    // error
                    AssemblerError.InvalidOperand(peek(-1).Value, m_file, m_Src[peek(-1).Value.line + 1]);
                }
            }
            else if (try_consume(TokenType.label, out Token _out))
            {
                m_FileBin.Add($"_REF_ LABEL {_out.ident},false");
            }
            else if (try_consume(TokenType.global))
            {
                if (try_consume(TokenType.label, out _out))
                {
                    m_FileBin.Add($"_REF_ LABEL {_out.ident},true");
                }
            }
            else if (try_consume(TokenType.section))
            {
                if (try_consume(TokenType.text))
                {
                    m_FileBin.Add("_TEXT"); 
                }
                else if (try_consume(TokenType.data))
                {
                    m_FileBin.Add("_DATA");
                }
                else if (try_consume(TokenType.bss))
                {
                    m_FileBin.Add("_BSS");
                }
            }
            else if (try_consume(TokenType.rbyte))
            {
                m_FileBin.Add("_RBYTE_");
            }
            else if (try_consume(TokenType.rword))
            {
                m_FileBin.Add("_RWORD_");
            }
            else if (try_consume(TokenType.rtbyte))
            {
                m_FileBin.Add("_RTBYTE_");
            }
            else if (try_consume(TokenType.rdword))
            {
                m_FileBin.Add("_RDWORD_");
            }
        }

        private InstructionInfo FindInstruction(Instruction instruction, Arguments argument1, ref object arg1, Arguments argument2, ref object arg2)
        {
            ConvertInstruction(ref instruction, ref argument1, ref argument2);

            InstructionInfo result;

            RegisterInfo? Tempregister1 = ConvertToInfo(arg1);
            RegisterInfo register1 = (RegisterInfo)Register.none;

            if (Tempregister1.HasValue)
            {
                register1 = Tempregister1.Value;
            }

            if (instruction == Instruction.TEST && argument1 == Arguments.reg16 && register1.m_Register == Register.AX)
            {
                result = AllInstruction.s_instructionInfo.Find(instr => FindInstr(instr, instruction, Arguments.AX, Arguments.none));

                arg1 = null;
            }
            else if (instruction == Instruction.MOV && argument1 == Arguments.reg8 && register1.m_Register == Register.MB)
            {
                result = AllInstruction.s_instructionInfo.Find(instr => FindInstr(instr, instruction, Arguments.MB, Arguments.imm8));

                arg1 = null;
            }
            else if (instruction == Instruction.MOVW && argument1 == Arguments.reg16 && register1.m_Register == Register.AX && OrOperator(argument2, Arguments.imm8, Arguments.imm16))
            {
                result = AllInstruction.s_instructionInfo.Find(instr => FindInstr(instr, instruction, Arguments.AX, Arguments.imm16));

                arg1 = null;
                arg2 = arg2.ToString().PadLeft(4, '0');
            }
            else if (instruction == Instruction.POPW && argument1 == Arguments.reg16 && register1.m_Register == Register.BP)
            {
                result = AllInstruction.s_instructionInfo.Find(instr => FindInstr(instr, instruction, Arguments.BP, Arguments.none));

                arg1 = null;
            }
            else
            {
                result = AllInstruction.s_instructionInfo.Find(instr => FindInstr(instr, instruction, argument1, argument2));
            }
            return result;
        }

        private bool OrOperator(Arguments argument, params Arguments[] arguments)
        {
            for (int i = 0; i < arguments.Length; i++)
            {
                if (argument != arguments[i])
                {
                    return false;
                }
            }
            return true;
        }

        private void ConvertInstruction(ref Instruction instruction, ref Arguments argument1, ref Arguments argument2)
        {
            if (argument1 == Arguments.reg8)
            {
                if (argument2 == Arguments.imm16 || argument2 == Arguments.imm24 || argument2 == Arguments.imm32)
                {
                    argument2 = Arguments.imm8;
                }
            }
            else if (argument1 == Arguments.reg16)
            {
                if (argument2 == Arguments.imm8 || argument2 == Arguments.imm24 || argument2 == Arguments.imm32)
                {
                    argument2 = Arguments.imm16;
                }
            }
            else if (argument1 == Arguments.reg32)
            {
                if (argument2 == Arguments.imm8 || argument2 == Arguments.imm16 || argument2 == Arguments.imm24)
                {
                    argument2 = Arguments.imm32;
                }
            }

                switch (instruction)
            {
                case Instruction.MOV:
                    if (argument1 == Arguments.reg16)
                    {
                        //if (!OrOperator(argument2, Arguments.imm8, Arguments.imm16, Arguments.imm24, Arguments.imm32))
                        if (argument2 == Arguments.imm8 || argument2 == Arguments.imm24 || argument2 == Arguments.imm32)
                        {
                            argument2 = Arguments.imm16;
                        }
                        instruction = Instruction.MOVW;
                        break;
                    }
                    else if (argument1 == Arguments.reg32)
                    {
                        instruction = Instruction.MOVD;
                        if (!OrOperator(argument2, Arguments.imm8, Arguments.imm16, Arguments.imm24, Arguments.imm32))
                        {
                            argument2 = Arguments.imm32;
                        }
                        break;
                    }

                    if (argument2 == Arguments.reg16 || argument2 == Arguments.imm16)
                    {
                        instruction = Instruction.MOVW;
                        break;
                    }
                    else if (argument2 == Arguments.reg32 || argument2 == Arguments.imm32)
                    {
                        instruction = Instruction.MOVD;
                        break;
                    }
                    break;
                case Instruction.MOVW:
                    if (argument2 == Arguments.reg8 || argument2 == Arguments.imm8)
                    {
                        instruction = Instruction.MOV;
                        break;
                    }
                    else if (argument2 == Arguments.reg32 || argument2 == Arguments.imm32)
                    {
                        instruction = Instruction.MOVD;
                        break;
                    }
                    break;
                case Instruction.MOVT:
                    if (argument2 == Arguments.reg8 || argument2 == Arguments.imm8)
                    {
                        instruction = Instruction.MOV;
                        break;
                    }
                    else if (argument2 == Arguments.reg16 || argument2 == Arguments.imm16)
                    {
                        instruction = Instruction.MOVW;
                        break;
                    }
                    break;
                case Instruction.MOVD:
                    if (argument2 == Arguments.reg8 || argument2 == Arguments.imm8)
                    {
                        instruction = Instruction.MOV;
                        break;
                    }
                    else if (argument2 == Arguments.reg16 || argument2 == Arguments.imm16)
                    {
                        instruction = Instruction.MOVW;
                        break;
                    }
                    break;
                case Instruction.POP:
                    if (argument1 == Arguments.reg16)
                    {
                        instruction = Instruction.POPW;
                    }
                    else if (argument1 == Arguments.reg32)
                    {
                        instruction = Instruction.POPD;
                    }
                    break;
                case Instruction.POPW:
                    break;
                case Instruction.POPT:
                    break;
                case Instruction.POPD:
                    break;
                case Instruction.ADD:
                    break;
                case Instruction.SUB:
                    break;
                case Instruction.MUL:
                    break;
                case Instruction.DIV:
                    break;
                case Instruction.AND:
                    break;
                case Instruction.OR:
                    break;
                case Instruction.NOR:
                    break;
                case Instruction.XOR:
                    break;
                case Instruction.NOT:
                    break;
                case Instruction.FADD:
                    break;
                case Instruction.FSUB:
                    break;
                case Instruction.FMUL:
                    break;
                case Instruction.FDIV:
                    break;
                case Instruction.FAND:
                    break;
                case Instruction.FOR:
                    break;
                case Instruction.FNOR:
                    break;
                case Instruction.FXOR:
                    break;
                case Instruction.FNOT:
                    break;
                case Instruction.MOVS:
                    break;
                case Instruction.MOVF:
                    break;
                default:
                    break;
            }
            if (argument1 == Arguments.reg16)
            {
                if (instruction == Instruction.MOV)
                {
                    instruction = Instruction.MOVW;
                }
            }
        }

        bool FindInstr(InstructionInfo instr, Instruction instruction, Arguments argument1, Arguments argument2)
        {
            return instr.m_Instruction == instruction &&
                instr.m_Argument1 == argument1 &&
                instr.m_Argument2 == argument2;
        }

        RegisterInfo? ConvertToInfo(object v)
        {
            if (v == null) return null;
            return (RegisterInfo)(string)v;
        }

        public void gen_prog(Token[] tokens, string src)
        {
            m_Src = src.Split('\n');
            m_tokens = tokens;

            while (peek().HasValue)
            {
                build();
            }

            m_OutputBin.Add(m_FileBin.ToArray());
        }


        #region Token Functions
        Token? peek(int offset = 0)
        {
            if (m_index + offset >= m_tokens.Length)
                return null;
            return m_tokens[m_index + offset];
        }
        Token consume()
        {
            m_index++;
            return m_tokens[m_index - 1];
        }
        bool try_consume(TokenType type)
        {
            if (peek().HasValue && peek().Value.type == type)
            {
                consume();
                return true;
            }
            return false;
        }
        bool try_consume(TokenType type, out Token _out)
        {
            if (peek().HasValue && peek().Value.type == type)
            {
                _out = consume();
                return true;
            }
            _out = new Token();
            return false;
        }
        Token try_consume_error(TokenType type)
        {
            if (peek().HasValue && peek().Value.type == type)
            {
                return consume();
            }
            AssemblerError.ExpectedToken(peek(-1).Value, m_file, type);

            throw new NotImplementedException("yes now it's implemented now");
        }
        #endregion

        List<string> SplitHexString(string hexString)
        {
            if(hexString == null)
            {
                throw new ArgumentNullException();
            }
            List<string> result = new List<string>();

            if (hexString.Length == 1)
            {
                hexString = hexString.PadLeft(2, '0');
            }
            else if (hexString.Length % 2 != 0)
            {
                hexString = hexString.PadLeft(hexString.Length + (hexString.Length % 2), '0');
            }

            for (int i = 0; i < hexString.Length; i += 2)
            {
                string _byte = hexString.Substring(i, 2);
                result.Add(_byte);
            }

            return result;
        }
        string parse_expr(TokenType endType)
        {
            Stack<string> holdingStack = new Stack<string>();
            Stack<string> OutputStack = new Stack<string>();
            string result;

            int parens = 1;

            string buf;

            string lastToken = null;

            string ident = "BINEXPR";

            while (parens != 0)
            {
                buf = "";
                if (peek().Value.type == TokenType.int_lit)
                {
                    lastToken = "lit";

                    string[] buffer = SplitHexString(Convert.ToString(Convert.ToInt32(consume().ident), 16)).ToArray();
                    buf = "";

                    for (int b = 0; b < buffer.Length; b++)
                    {
                        buf += buffer[b];
                    }

                    OutputStack.Push("I_" + buf);
                }
                else if (peek().Value.type == TokenType.register)
                {
                    OutputStack.Push("R_" + consume().ident);
                    ident = "BINEXPRWR";
                    continue;
                }
                else if (peek().Value.type == TokenType.open_paren)
                {
                    parens++;
                    consume();
                    lastToken = "(";
                    holdingStack.Push("(");
                }
                else if (peek().Value.type == TokenType.close_paren)
                {
                    parens--;
                    consume();
                    if(parens == 0)
                    {
                        break;
                    }
                    lastToken = ")";
                    while (holdingStack.TryPeek(out result) && result != "(")
                    {
                        OutputStack.Push(holdingStack.Pop());
                    }

                    if (!holdingStack.TryPeek(out _))
                    {
                        throw new NotImplementedException();
                    }

                    if (holdingStack.TryPeek(out result) && result == "(")
                    {
                        holdingStack.Pop();
                    }
                }
                else if (peek().Value.bin_proc().HasValue && peek().Value.bin_proc().Value != -1)
                {
                    Token newToken = consume();

                    if (newToken.type == TokenType.minus)
                    {
                        if (lastToken == null ||
                            (lastToken != "lit" && lastToken != ")"))
                        {
                            newToken.type = TokenType.neg;
                        }
                    }

                    while (holdingStack.TryPeek(out result) && result != "(")
                    {
                        if (ContainsBinProc(result, out int binproc))
                        {
                            if (newToken.bin_proc().HasValue && binproc >= newToken.bin_proc().Value)
                            {
                                OutputStack.Push(holdingStack.Pop());
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    holdingStack.Push(((char)newToken.type).ToString());
                    lastToken = ((char)newToken.type).ToString();
                }
                else
                {
                    object value = parse(out _, out _);
                    if (value == null)
                    {
                        throw new NotImplementedException();
                    }

                    OutputStack.Push(value.ToString());
                }
            }

            while (holdingStack.TryPeek(out _))
            {
                OutputStack.Push(holdingStack.Pop());
            }

            string output = "";
            while (OutputStack.TryPeek(out result))
            {
                output += OutputStack.Pop() + " ";
            }
            output = ident + " " + output;

            return output;
        }
        bool ContainsBinProc(string expr, out int binproc)
        {
            for (int i = 0; i < expr.Length; i++)
            {
                if (new string("+-*/&%^~").Contains(expr[i]))
                {
                    binproc = GetBinProc(expr[i]);
                    return true;
                }
            }
            binproc = -1;
            return false;
        }
        int GetBinProc(char c)
        {
            if (c == '-')
                return 1;
            else if (c == '+')
                return 1;
            else if (c == '*')
                return 2;
            else if (c == '/')
                return 2;
            else if (c == 'n')
                return 10;
            else if (c == 'p')
                return 10;
            else
            {
                return -1;
            }
        }
    }
}
