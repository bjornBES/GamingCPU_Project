using System;
using System.Collections.Generic;
using System.Linq;
using static HexLibrary.SplitFunctions;
using static HexLibrary.HexConverter;
using static HexLibrary.StringFunctions;
using System.Text;
using System.Data;

namespace AssemblerBCG
{
    public class AssemblerVariabels : AssemblerSettings
    {
        public const StringComparison IgnoreCasing = StringComparison.OrdinalIgnoreCase;
        public string[] m_src;
        public int m_index;
        public Section m_Section = Section.Text;
        public static bool m_WriteOut = true;

        public string m_file;
        public List<string> m_doneFiles = new List<string>();
        public List<Variable> m_Variables = new List<Variable>();

        public int Linenumber
        {
            get
            {
                if (m_InputFile == m_file)
                {
                    return m_index - 1;
                }
                return m_index + 1;
            }
        }

        public List<string> m_Output = new List<string>();
        public List<string> m_OutputSymbols = new List<string>();
        public List<string> m_OutputSection = new List<string>();
        public List<string> m_OutputIncludedFiles = new List<string>();

        public List<Label> m_Label = new List<Label>();

        public void AddLine(string line)
        {
            AddLine(line, m_Section);
        }
        public void AddLine(IEnumerable<string> line)
        {
            if (m_WriteOut == false)
            {
                return;
            }

            for (int i = 0; i < line.Count(); i++)
            {
                AddLine(line.ElementAt(i), m_Section);
            }
        }
        public void AddLine(string line, Section section)
        {
            if (m_WriteOut == false)
            {
                return;
            }
            switch (section)
            {
                case Section.Text:
                case Section.Data:
                case Section.Rdata:
                case Section.Bss:
                default:
                    m_Output.Add(line);
                    break;
                case Section.Symbols:
                    m_OutputSymbols.Add(line);
                    break;
                case Section.Section:
                    m_OutputSection.Add(line);
                    break;
                case Section.IncludedFiles:
                    m_OutputIncludedFiles.Add(line);
                    break;
            }
        }

        bool m_maskLow = false;
        bool m_maskHigh = false;

        bool m_isNear = false;
        bool m_isLong = false;
        bool m_isFar = false;
        public bool ParseTerm(string term, SizeAlignment alignment, out ArgumentMode mode, out string[] _out)
        {
            SizeAlignment sizeAlignment = alignment;
            return ParseTerm(term, ref sizeAlignment, out mode, out _out);
        }
        public string ParseTerm(string term, SizeAlignment alignment, out ArgumentMode mode)
        {
            SizeAlignment sizeAlignment = alignment;
            string[] _out;
            if (ParseTerm(term, ref sizeAlignment, out mode, out _out) == false)
            {
                return null;
            }

            string result = "";

            for (int i = 0; i < _out.Length; i++)
            {
                result += _out[i];
            }

            return result;
        }
        public bool ParseTerm(string term, ref SizeAlignment alignment, out ArgumentMode mode, out string[] _out)
        {
            if (string.IsNullOrEmpty(term))
            {
                _out = null;
                mode = default;
                return false;
            }

            m_maskLow = false;
            m_maskHigh = false;

            if (term.StartsWith("LOW", IgnoreCasing))
            {
                m_maskLow = true;
                term = term.Split(' ', 2)[1];
            }
            else if (term.StartsWith("HIGH", IgnoreCasing))
            {
                m_maskHigh = true;
                term = term.Split(' ', 2)[1];
            }

            if (term.Contains("far", IgnoreCasing))
            {
                m_isFar = true;

                string _arg = term.Replace("far ", "", false, System.Globalization.CultureInfo.CurrentCulture);

                bool result = ParseTerm(_arg, ref alignment, out mode, out _out);

                m_isFar = false;

                return result;
            }
            else if (term.Contains("long", IgnoreCasing))
            {
                m_isLong = true;

                string _arg = term.Replace("long ", "", false, System.Globalization.CultureInfo.CurrentCulture);

                bool result = ParseTerm(_arg, ref alignment, out mode, out _out);

                m_isLong = false;

                return result;
            }
            else if (term.Contains("short", IgnoreCasing))
            {
                string _arg = term.Replace("short ", "", false, System.Globalization.CultureInfo.CurrentCulture);

                bool result = ParseTerm(_arg, ref alignment, out mode, out _out);

                return result;
            }
            else if (term.Contains("near", IgnoreCasing))
            {
                m_isNear = true;

                string _arg = term.Replace("near ", "", false, System.Globalization.CultureInfo.CurrentCulture);

                bool result = ParseTerm(_arg, ref alignment, out mode, out _out);
                m_isNear = false;
                return result;
            }

            if (ContainsOperators(term) && term.Length > 2)
            {
                bool wasBadExpr = BuildExpr(term, out mode, out _out);
                if (!wasBadExpr)
                {
                    return true;
                }
            }

            if (term.StartsWith('%') && GetVariable(term.TrimStart('%'), out Variable variable))
            {
                mode = variable.m_Mode;
                _out = variable.m_Value;
                return true;
            }
            else if (term.StartsWith("0x"))
            {
                string HexTerm = term.Substring(2).Replace("_", "");

                switch (alignment)
                {
                    case SizeAlignment._byte:
                        HexTerm = HexTerm.PadLeft(2, '0');
                        break;
                    case SizeAlignment._word:
                        HexTerm = HexTerm.PadLeft(4, '0');
                        break;
                    case SizeAlignment._tbyte:
                        HexTerm = HexTerm.PadLeft(6, '0');
                        break;
                    case SizeAlignment._dword:
                        HexTerm = HexTerm.PadLeft(8, '0');
                        break;
                    case SizeAlignment._qword:
                        HexTerm = HexTerm.PadLeft(16, '0');
                        break;
                    default:
                        break;
                }

                mode = getSize(ref HexTerm);
                _out = SplitHexString(HexTerm);
                return true;
            }
            else if (term.StartsWith("0b"))
            {
                string HexTerm = ToHexString(term.Substring(2).Replace("_", ""), 2);

                switch (alignment)
                {
                    case SizeAlignment._byte:
                        HexTerm = HexTerm.PadLeft(2, '0');
                        break;
                    case SizeAlignment._word:
                        HexTerm = HexTerm.PadLeft(4, '0');
                        break;
                    case SizeAlignment._tbyte:
                        HexTerm = HexTerm.PadLeft(6, '0');
                        break;
                    case SizeAlignment._dword:
                        HexTerm = HexTerm.PadLeft(8, '0');
                        break;
                    case SizeAlignment._qword:
                        HexTerm = HexTerm.PadLeft(16, '0');
                        break;
                    default:
                        break;
                }

                mode = getSize(ref HexTerm);
                _out = SplitHexString(HexTerm);
                return true;
            }
            else if (char.IsDigit(term[0]))
            {
                string HexTerm = ToHexString(term.Replace("_", ""), 10);

                switch (alignment)
                {
                    case SizeAlignment._byte:
                        HexTerm = HexTerm.PadLeft(2, '0');
                        break;
                    case SizeAlignment._word:
                        HexTerm = HexTerm.PadLeft(4, '0');
                        break;
                    case SizeAlignment._tbyte:
                        HexTerm = HexTerm.PadLeft(6, '0');
                        break;
                    case SizeAlignment._dword:
                        HexTerm = HexTerm.PadLeft(8, '0');
                        break;
                    case SizeAlignment._qword:
                        HexTerm = HexTerm.PadLeft(16, '0');
                        break;
                    default:
                        break;
                }

                mode = getSize(ref HexTerm);

                _out = SplitHexString(HexTerm);

                return true;
            }
            else if (isRegister(term, out Register register))
            {
                mode = GetArgumentFromRegister(register);
                alignment = GetAlignmentFromRegister(register);
                _out = new string[] { ToHexString((int)register).PadLeft(2, '0') };
                return true;
            }
            else if (doAddresses(term, ref alignment, out mode, out _out))
            {
                return true;
            }
            else if (term.StartsWith('\'') && term.EndsWith('\''))
            {
                mode = ArgumentMode.immediate_byte;
                term = term.Remove(0, 1);
                term = term.Remove(term.Length - 1, 1);
                if (term == "\\0")
                {
                    term = "00";
                }
                else if (term == "\\n")
                {
                    term = "0A";
                }   
                else
                {
                    term = Convert.ToString((byte)term[0], 16).PadLeft(2, '0');
                }
                _out = new string[] { term };
                return true;
            }
            else if (term.StartsWith('\"') && term.EndsWith('\"'))
            {
                string data = term.Trim('"');
                List<byte> bytes = new List<byte>();

                List<string> result = new List<string>();
                string c;
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i] == '\\')
                    {
                        i++;
                        c = ParseTerm($"\'\\{data[i]}\'", alignment, out _);
                    }
                    else
                    {
                        c = ParseTerm($"\'{data[i]}\'", alignment, out _);
                    }
                    result.Add(c.PadLeft(2, '0'));
                }
                _out = result.ToArray();
                mode = ArgumentMode.immediate_byte;
                return true;
            }
            else if (term.EndsWith('f'))
            {
                string fValue = term.Replace("f", "");
                string binaryString = "";
                if (float.TryParse(fValue, out float floatValueParsed))
                {
                    int floatToInt = BitConverter.ToInt32(BitConverter.GetBytes(floatValueParsed), 0);
                    binaryString = Convert.ToString(floatToInt, 16).PadLeft(8, '0');
                }
                else if (double.TryParse(fValue, out double doubleValueParsed))
                {
                    long floatToInt = BitConverter.ToInt64(BitConverter.GetBytes(doubleValueParsed), 0);
                    binaryString = Convert.ToString(floatToInt, 16).PadLeft(16, '0');
                }

                mode = ArgumentMode.immediate_float;
                _out = SplitHexString(binaryString);
                return true;
            }

            mode = default;
            _out = null;
            return false;
        }
        ArgumentMode GetArgumentFromRegister(Register register)
        {
            return register switch
            {
                Register.AL => ArgumentMode.register_AL,
                Register.A => ArgumentMode.register_A,
                Register.AX => ArgumentMode.register_AX,

                Register.BL => ArgumentMode.register_BL,
                Register.B => ArgumentMode.register_B,
                Register.BX => ArgumentMode.register_BX,
                
                Register.CL => ArgumentMode.register_CL,
                Register.C => ArgumentMode.register_C,
                Register.CX => ArgumentMode.register_CX,
                
                Register.DL => ArgumentMode.register_DL,
                Register.D => ArgumentMode.register_D,
                Register.DX => ArgumentMode.register_DX,

                Register.L => ArgumentMode.register_L,
                Register.H => ArgumentMode.register_H,

                Register.AF => ArgumentMode.register_AF,
                Register.BF => ArgumentMode.register_BF,
                Register.CF => ArgumentMode.register_CF,
                Register.DF => ArgumentMode.register_DF,

                Register.AD => ArgumentMode.register_AD,
                Register.BD => ArgumentMode.register_BD,
                Register.CD => ArgumentMode.register_CD,
                Register.DD => ArgumentMode.register_DD,

                Register.EX => ArgumentMode.register_EX,
                Register.FX => ArgumentMode.register_FX,
                Register.GX => ArgumentMode.register_GX,
                Register.HX => ArgumentMode.register_HX,

                Register.MB => ArgumentMode.register_MB,
                _ => ArgumentMode.register,
            };
        }
        public SizeAlignment GetAlignmentFromRegister(Register register)
        {
            RegisterInfo registerInfo = Registers.m_Regs[register];
            return (SizeAlignment)registerInfo.m_Size;
        }
        public ArgumentMode getSize(ref string HexNumber)
        {
            switch (HexNumber.Length)
            {
                case 1:
                case 2:
                    HexNumber = HexNumber.PadLeft(2, '0');
                    return ArgumentMode.immediate_byte;
                case 3:
                case 4:
                    HexNumber = HexNumber.PadLeft(4, '0');
                    return ArgumentMode.immediate_word;
                case 5:
                    HexNumber = HexNumber.PadLeft(5, '0');
                    return ArgumentMode.immediate_tbyte;
                case 6:
                    HexNumber = HexNumber.PadLeft(6, '0');
                    return ArgumentMode.immediate_dword;
                default:
                    HexNumber = HexNumber.Substring(HexNumber.Length - 6);
                    return ArgumentMode.immediate_qword;
            }
        }
        bool doAddresses(string addressTerm, ref SizeAlignment alignment, out ArgumentMode argumentMode, out string[] _out)
        {

            if (addressTerm.StartsWith("[BP ") && addressTerm.EndsWith(']') || addressTerm.StartsWith("[BPX ") && addressTerm.EndsWith(']'))
            {
                addressTerm = addressTerm.TrimEnd(']').TrimStart('[');
                if (!isRegister(addressTerm.Split(' ')[0], out Register register))
                {
                    Console.WriteLine("Error: 00010 Nope");
                }
                char Operator = addressTerm.Split(' ')[1][0];
                if (!ParseTerm(addressTerm.Split(" ")[2], ref alignment, out argumentMode, out string[] datas))
                {
                    Console.WriteLine("Error: 00011 Nope");
                }
                string data = "";
                int number = 0;

                for (int i = 0; i < datas.Length; i++)
                {
                    data += datas[i];
                }

                if (argumentMode == ArgumentMode.immediate_byte)
                {
                    number = Convert.ToByte(data, 16);
                    argumentMode = ArgumentMode.BP_rel_address_byte;
                }
                else if (argumentMode == ArgumentMode.immediate_word)
                {
                    number = Convert.ToUInt16(data, 16);
                    argumentMode = ArgumentMode.BPX_rel_address_word;
                }


                number = (Operator == '_' ? -number : number);
                _out = SplitHexString(ToHexString(number));
                return true;
            }
            else if (addressTerm.StartsWith("[SP ") && addressTerm.EndsWith(']') || addressTerm.StartsWith("[SPX ") && addressTerm.EndsWith(']'))
            {
                addressTerm = addressTerm.TrimEnd(']').TrimStart('[');
                if (!isRegister(addressTerm.Split(' ')[0], out Register register))
                {
                    Console.WriteLine("Error: 00010 Nope");
                }
                char Operator = addressTerm.Split(' ')[1][0];
                if (!ParseTerm(addressTerm.Split(" ")[2], ref alignment, out argumentMode, out string[] datas))
                {
                    Console.WriteLine("Error: 00011 Nope");
                }
                string data = "";
                int number = 0;

                for (int i = 0; i < datas.Length; i++)
                {
                    data += datas[i];
                }

                if (argumentMode == ArgumentMode.immediate_byte)
                {
                    number = Convert.ToByte(data, 16);
                    argumentMode = ArgumentMode.SP_rel_address_byte;
                }
                else if (argumentMode == ArgumentMode.immediate_word)
                {
                    number = Convert.ToUInt16(data, 16);
                    argumentMode = ArgumentMode.SPX_rel_address_word;
                }


                number = (Operator == '_' ? -number : number);
                _out = SplitHexString(ToHexString(number));
                return true;
            }
            else if (addressTerm.StartsWith('[') && addressTerm.EndsWith(']') && addressTerm.Contains(':'))
            {
                addressTerm = addressTerm.TrimEnd(']').TrimStart('[');
                if (!isRegister(addressTerm.Split(':')[0], out Register segment))
                {
                }
                if (!isRegister(addressTerm.Split(':')[1], out Register offset))
                {
                }

                if (segment == Register.DS)
                {
                    argumentMode = ArgumentMode.segment_DS_register;
                    if (offset == Register.B)
                    {
                        argumentMode = ArgumentMode.segment_DS_B;
                    }
                }
                else if (segment == Register.ES)
                {
                    argumentMode = ArgumentMode.segment_ES_register;
                    if (offset == Register.B)
                    {
                        argumentMode = ArgumentMode.segment_ES_B;
                    }
                }
                else
                {
                    argumentMode = ArgumentMode.segment_address;
                }

                _out = new string[]
                {
                    $"{segment.ToString().PadLeft(2, '0')}:{offset.ToString().PadLeft(2, '0')}"
                };
                return true;
            }
            else if (addressTerm.StartsWith('[') && addressTerm.EndsWith(']') && isRegister(addressTerm.Trim('[', ']'), out Register register))
            {
                argumentMode = ArgumentMode.register_address;
                if (register == Register.HL)
                {
                    argumentMode = ArgumentMode.register_address_HL;
                }
                _out = new string[] { ToHexString((int)register).PadLeft(2, '0') };
                return true;
            }
            else if (addressTerm.Contains('.') && !addressTerm.StartsWith('['))
            {
                string output;

                setPreSize(out output, out argumentMode);

                output += "L_";

                output += addressTerm.Replace('.', '_');

                if (m_maskHigh)
                {
                    output += ",MH";
                }
                else if (m_maskLow)
                {
                    output += ",ML";
                }

                _out = new string[] { output };
                return true;
            }
            else if (addressTerm.StartsWith('@') || IsLetterOrDidit(addressTerm))
            {
                string output;
                setPreSize(out output, out argumentMode);

                output += "L_";

                output += addressTerm.Trim('@');

                if (m_maskHigh)
                {
                    output += ",MH";
                }
                else if (m_maskLow)
                {
                    output += ",ML";
                }

                _out = new string[] { output };
                return true;
            }
            else if (addressTerm == "$")
            {
                string output;
                setPreSize(out output, out argumentMode);

                output += "CA_";

                if (m_maskHigh)
                {
                    output += ",MH";
                }
                else if (m_maskLow)
                {
                    output += ",ML";
                }
                _out = new string[] { output };
                return true;
            }
            else if (addressTerm == "$$")
            {
                string output;
                setPreSize(out output, out argumentMode);

                output += "CS_";

                if (m_maskHigh)
                {
                    output += ",MH";
                }
                else if (m_maskLow)
                {
                    output += ",ML";
                }
                _out = new string[] { output };
                return true;
            }
            else if (addressTerm.StartsWith('[') && addressTerm.EndsWith(']'))
            {
                addressTerm = addressTerm.TrimEnd(']').TrimStart('[');
                ParseTerm(addressTerm, ref alignment, out argumentMode, out _out);
                switch (argumentMode)
                {
                    case ArgumentMode.immediate_byte:
                        argumentMode = ArgumentMode.near_address;
                        break;
                    case ArgumentMode.immediate_word:
                        argumentMode = ArgumentMode.short_address;
                        break;
                    case ArgumentMode.immediate_tbyte:
                        argumentMode = ArgumentMode.long_address;
                        break;
                    case ArgumentMode.immediate_dword:
                        argumentMode = ArgumentMode.far_address;
                        break;
                    case ArgumentMode.immediate_qword:
                    default:
                        break;
                }

                return true;
            }
            else if (addressTerm.StartsWith("rel", IgnoreCasing))
            {
                addressTerm = addressTerm.Replace("rel ", "");
                string result = ParseTerm(addressTerm, alignment, out _);
                argumentMode = ArgumentMode.relative_address;

                _out = new string[] { $"REL_{result}" };

                //Console.WriteLine($"ERROR: {argumentMode} not supported by relative addressing {m_file}:{Linenumber}");
                return true;
            }

            argumentMode = default;
            _out = null;
            return false;
        }
        bool isRegister(string value, out Register register) => Enum.TryParse(value, true, out register);
        void setPreSize(out string _out, out ArgumentMode argumentMode)
        {
            if (m_isNear)
            {
                _out = "_N";
                argumentMode = ArgumentMode.immediate_byte;
            }
            else if (m_isLong)
            {
                _out = "_L";
                argumentMode = ArgumentMode.immediate_tbyte;
            }
            else if (m_isFar)
            {
                _out = "_F";
                argumentMode = ArgumentMode.immediate_dword;
            }
            else
            {
                _out = "_S";
                argumentMode = ArgumentMode.immediate_word;
            }
        }
        public bool LabelExists(string name, out Label label, out int index)
        {
            for (int i = 0; i < m_Label.Count; i++)
            {
                if (name == m_Label[i].m_Name)
                {
                    index = i;
                    label = m_Label[i];
                    return true;
                }
            }
            index = 0;
            label = null;
            return false;
        }
        public bool GetVariable(string name, out Variable variable)
        {
            for (int i = 0; i < m_Variables.Count; i++)
            {
                if (m_Variables[i].m_Name == name)
                {
                    variable = m_Variables[i];
                    return true;
                }
            }
            variable = null;
            return false;
        }



        List<string> tokens = new List<string>();
        string m_expr;
        int m_exprindex = 0;
        public bool BuildExpr(string expr, out ArgumentMode argumentMode, out string[] result)
        {
            m_exprindex = 0;
            tokens.Clear();
            m_expr = expr;
            Tokenize();
            result = ParseExpr(out argumentMode);
            if (badExpr == true)
            {
                return true;
            }
            return false;
        }
        public void Tokenize()
        {
            while (peek().HasValue)
            {
                string buf = "";
                if (char.IsLetterOrDigit(peek().Value))
                {
                    while (peek().HasValue && char.IsLetterOrDigit(peek().Value))
                    {
                        buf += consume();
                    }
                    tokens.Add(buf);
                }
                else if (char.IsSeparator(peek().Value))
                {
                    consume();
                }
                else if (peek().Value == '@')
                {
                    buf += consume();
                    if (char.IsLetter(peek().Value) || peek().Value == '_')
                    {
                        buf += consume();
                    }
                    while (peek().HasValue && (char.IsLetterOrDigit(peek().Value) || peek().Value == '_'))
                    {
                        buf += consume();
                    }
                    tokens.Add(buf);
                }
                else if (peek().Value == '$' && peek(1).HasValue && peek(1).Value == '$')
                {
                    consume();
                    consume();
                    tokens.Add("$$");
                }
                else
                {
                    tokens.Add(consume().ToString());
                }
            }
        }

        Stack<string> HoldingStack = new Stack<string>();
        Stack<string> OutputStack = new Stack<string>();
        bool badExpr = false;
        public string[] ParseExpr(out ArgumentMode argumentMode)
        {
            badExpr = false;
            HoldingStack.Clear();
            OutputStack.Clear();
            bool isAddress = false;
            bool doResult = true;
            string lastToken = "";
            List<ArgumentMode> argumentModes = new List<ArgumentMode>();

            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i] == "[")
                {
                    isAddress = true;
                    continue;
                }
                if (tokens[i] == "]")
                {
                    isAddress = true;
                    continue;
                }

                string term = tokens[i];

                if (ParseTerm(term, SizeAlignment._word, out ArgumentMode mode) != null)
                {
                    argumentModes.Add(mode);
                    string result = ParseTerm(term, SizeAlignment._word, out _);
                    switch (mode)
                    {
                        case ArgumentMode.immediate_byte:
                        case ArgumentMode.immediate_word:
                        case ArgumentMode.immediate_tbyte:
                        case ArgumentMode.immediate_dword:
                        case ArgumentMode.immediate_float:
                        case ArgumentMode.immediate_qword:
                        case ArgumentMode.immediate_double:
                            if (result.StartsWith("_"))
                            {
                                OutputStack.Push(result);
                                doResult = false;
                            }
                            else
                            {
                                OutputStack.Push("i_" + result);
                            }
                            break;
                        default:
                            badExpr = true;
                            argumentMode = ArgumentMode.none;
                            return null;
                    }

                    lastToken = "term";
                }
                else if (term == "(")
                {
                    lastToken = "(";
                    HoldingStack.Push(lastToken);
                }
                else if (term == ")")
                {
                    lastToken = ")";
                    string result;
                    while (HoldingStack.TryPeek(out result) && result != "(")
                    {
                        OutputStack.Push(HoldingStack.Pop());
                    }

                    if (!HoldingStack.TryPeek(out _))
                    {
                        throw new NotImplementedException();
                    }

                    if (HoldingStack.TryPeek(out result) && result == "(")
                    {
                        HoldingStack.Pop();
                    }
                }
                else if (getBinProc(term) != -1)
                {
                    string newToken = tokens[i];

                    if (newToken == "-")
                    {
                        if (lastToken == null || (lastToken != "term" && lastToken != ")"))
                        {
                            newToken = "n";
                        }
                    }
                    else if (newToken == "+")
                    {
                        if (lastToken == null || (lastToken != "term" && lastToken != ")"))
                        {
                            newToken = "p";
                        }
                    }

                    while (HoldingStack.TryPeek(out string result) && result != "(")
                    {
                        if (ContainsOperators(result))
                        {
                            int binproc = getBinProc(result);
                            if (binproc >= getBinProc(newToken))
                            {
                                OutputStack.Push(HoldingStack.Pop());
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    HoldingStack.Push(newToken.ToString());
                    lastToken = newToken.ToString();
                }
                else
                {

                }
            }

            while (HoldingStack.TryPeek(out _))
            {
                OutputStack.Push(HoldingStack.Pop());
            }

            string[] outputS = OutputStack.Reverse().ToArray();

            string[] outputResult;

            if (doResult)
            {
                Stack<int> resultStack = new Stack<int>();

                for (int i = 0; i < outputS.Length; i++)
                {
                    string result = outputS[i];
                    if (result.StartsWith("i_"))
                    {
                        result = result.Substring(2);
                        int number = Convert.ToInt32(result, 16);
                        resultStack.Push(number);
                    }
                    else
                    {
                        int rhs;
                        int lsh;
                        switch (result)
                        {
                            case "+":
                                rhs = resultStack.Pop();
                                lsh = resultStack.Pop();

                                resultStack.Push(lsh + rhs);
                                break;
                            case "-":
                                rhs = resultStack.Pop();
                                lsh = resultStack.Pop();

                                resultStack.Push(lsh - rhs);
                                break;
                            default:
                                break;
                        }
                    }
                }
                string hexResult = Convert.ToString(resultStack.Pop(), 16);
                argumentMode = getSize(ref hexResult);
                outputResult = SplitHexString(hexResult);
            }
            else
            {
                argumentMode = ArgumentMode.none;
                string result = "BINEXPR ";
                for (int i = 0; i < outputS.Length; i++)
                {
                    result += outputS[i] + " ";
                }
                result = result.Trim();
                outputResult = new string[] { result };
            }


            for (int i = 0; i < argumentModes.Count; i++)
            {
                if (argumentModes[i] == ArgumentMode.immediate_double && argumentMode < ArgumentMode.immediate_double)
                {
                    if (isAddress)
                    {
                        throw new NotImplementedException();
                    }
                    argumentMode = ArgumentMode.immediate_double;
                }
                else if (argumentModes[i] == ArgumentMode.immediate_float && argumentMode < ArgumentMode.immediate_float)
                {
                    if (isAddress)
                    {
                        throw new NotImplementedException();
                    }
                    argumentMode = ArgumentMode.immediate_float;
                }
                else if (argumentModes[i] == ArgumentMode.immediate_word && argumentMode < ArgumentMode.immediate_word)
                {
                    argumentMode = ArgumentMode.immediate_word;
                }
                else if (argumentModes[i] == ArgumentMode.immediate_tbyte && argumentMode < ArgumentMode.immediate_tbyte)
                {
                    argumentMode = ArgumentMode.immediate_tbyte;
                }
                else if (argumentModes[i] == ArgumentMode.immediate_dword && argumentMode < ArgumentMode.immediate_dword)
                {
                    argumentMode = ArgumentMode.immediate_dword;
                }
                else if (argumentModes[i] == ArgumentMode.immediate_dword && argumentMode < ArgumentMode.immediate_dword)
                {
                    if (m_CPUType >= CPUType.BC32)
                    {
                        argumentMode = ArgumentMode.immediate_qword;
                    }
                    else
                    {
                        argumentMode = ArgumentMode.immediate_dword;
                    }
                }
                else if (argumentMode == ArgumentMode.immediate_byte)
                {
                    continue;
                }
                else
                {

                }
            }

            if (isAddress)
            {
                switch (argumentMode)
                {
                    case ArgumentMode.immediate_byte:
                        argumentMode = ArgumentMode.near_address;
                        break;
                    case ArgumentMode.immediate_word:
                        argumentMode = ArgumentMode.short_address;
                        break;
                    case ArgumentMode.immediate_tbyte:
                        argumentMode = ArgumentMode.long_address;
                        break;
                    case ArgumentMode.immediate_dword:
                        argumentMode = ArgumentMode.far_address;
                        break;
                    case ArgumentMode.immediate_qword:
                    default:
                        throw new NotImplementedException();
                }
            }

            return outputResult;
        }

        char? peek(int offset = 0)
        {
            if (m_exprindex + offset >= m_expr.Length)
                return null;
            return m_expr[m_exprindex + offset];
        }
        char consume()
        {
            return m_expr[m_exprindex++];
        }
        int getBinProc(string c)
        {
            switch (c)
            {
                case "-":
                    return 1;
                case "+":
                    return 1;

                case "*":
                    return 3;
                case "/":
                    return 3;

                case "n":
                    return 10;
                case "p":
                    return 10;
                default:
                    break;
            }
            return -1;
        }

    }

    public enum Section
    {
        Text,
        Data,
        Rdata,
        Bss,
        Symbols,
        Section,
        IncludedFiles,
    }
}
