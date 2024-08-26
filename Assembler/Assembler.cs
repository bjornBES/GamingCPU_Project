using System;
using assembler.global;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;

namespace assembler
{
    public class Assembler
    {

        public List<string> m_BinaryOutput = new List<string>();
        public string[] m_Src;
        int m_lineNumber = 0;

        int m_bank = 0;
        bool m_bankEnable = false;

        string m_file;

        public List<Struct> m_Structs = new List<Struct>();

        List<Variable> m_variables = new List<Variable>();

        List<Marcos> m_marcos = new List<Marcos>();

        const string IMMDIATEBYTE =     "00000000";
        const string IMMDIATEWORD =     "00000001";
        const string IMMDIATEDWORD =    "00000010";
        const string IMMDIATETBYTE =    "00000111";
        
        const string ADDRESS =          "00000011";
        
        const string REGISTER =         "00000100";
        const string REGISTERADDRESS =  "00000101";

        const string STACKADDRESS =     "00000110";

        const string LONGADDRESS =      "00001001";

        const string FLOATIMMDIATE =    "00001010";
        
        const string SEGMENT =          "00001100";
        const string SEGMENTIMMDIATE =  "00001101";

        const string NONE =             "11111111";

        public void BuildSrc(string src)
        {
            m_BinaryOutput.Clear();

            src = Regex.Replace(src, @";\n[^\n]*", "COM||");
            src = Regex.Replace(src, @"COM\|\|", "");
            src = Regex.Replace(src, @" +", " ");


            m_Src = src.Split('\n');

            buildMarcos();

            build();
        }

        void buildMarcos()
        {
            for (m_lineNumber = 0; m_lineNumber < m_Src.Length; m_lineNumber++)
            {
                if (m_lineNumber == 0) continue;
                m_Src[m_lineNumber] = m_Src[m_lineNumber].Trim('\n');

                if (string.IsNullOrEmpty(m_Src[m_lineNumber])) continue;

                // marcos
                if (m_Src[m_lineNumber].Contains('@'))
                {
                    MakeMarco();
                    continue;
                }
                else if (m_Src[m_lineNumber].StartsWith('$'))
                {
                    m_Src[m_lineNumber] = m_Src[m_lineNumber].TrimStart(' ');
                    string[] operatons = m_Src[m_lineNumber].TrimStart('$').Split(' ');
                    string[] data = Parse(operatons[2]);
                    string hexString = "";
                    for (int i = 0; i < data.Length; i++)
                    {
                        hexString += data[i];
                    }

                    if (operatons[1] == "=")
                    {
                        m_variables.Add(new Variable()
                        {
                            m_Name = operatons[0],
                            m_Value = hexString
                        });
                    }
                    m_Src[m_lineNumber] = "";
                }
            }
        }
        public bool IsVariabel(string name, out Variable variable)
        {
            for (int i = 0; i < m_variables.Count; i++)
            {
                if (m_variables[i].m_Name == name)
                {
                    variable = m_variables[i];
                    return true;
                }
            }
            variable = null;
            return false;
        }
        void build()
        {
            for (m_lineNumber = 0; m_lineNumber < m_Src.Length; m_lineNumber++)
            {
                if (m_lineNumber == 0) continue;
                m_BinaryOutput.Add("_NEWLINE_");
                CodeOptimization();
                m_Src[m_lineNumber] = m_Src[m_lineNumber].Trim('\n');
                m_Src[m_lineNumber] = m_Src[m_lineNumber].TrimStart(' ');

                if (string.IsNullOrEmpty(m_Src[m_lineNumber])) continue;

                if (m_Src[m_lineNumber].StartsWith('@'))
                {
                    for (int i = m_lineNumber + 1; !m_Src[i].StartsWith(".ENDM"); i++)
                    {
                        m_lineNumber++;
                    }
                    m_lineNumber++;
                    continue;
                }

                if (m_Src[m_lineNumber].EndsWith(':'))
                {
                    MakeLabel();
                    continue;
                }
                else if (m_Src[m_lineNumber].Contains(".section"))
                {
                    continue;
                }
                else if (m_Src[m_lineNumber].StartsWith('.'))
                {
                    BuildDirectives();
                    continue;
                }
                else
                {
                    string line = m_Src[m_lineNumber].Trim();
                    m_rev = 0;
                    BuildInstruction(line);
                    continue;
                }


            }
        }

        int m_rev = 0;
        void BuildInstruction(string line, int maxRev = -1)
        {
            // this is for marcos, so there can't be a marco in a marco to many times
            if (m_rev == maxRev)
            { return; }
            m_rev++;

            if (string.IsNullOrEmpty(m_Src[m_lineNumber].Trim()))
                return;
            
            List<string> InstrutionBytes = new List<string>();

            string instruction = line.Split(' ', 2)[0];
            string PreArguments = line.Split(' ', 2).Last();
            string[] Arguments = PreArguments.Split(", ");

            if (instruction.ToUpper() == "MOVB")
            {
                instruction = "mov";
            }

            //m_BinaryOutput.Add($"{Convert.ToString(m_lineNumber, 16).PadLeft(4, '0')}:\t" + m_src[m_lineNumber].TrimStart());

            if (!Enum.TryParse(instruction.ToUpper(), out Instruction result))
            {
                if (IsMarcos(instruction, out Marcos marcos))
                {
                    string[] lines = marcos.lines;
                    Dictionary<string, string> args = new Dictionary<string, string>();
                    for (int i = 0; i < marcos.arguments.Length; i++)
                    {
                        args.Add(marcos.arguments[i], Arguments[i]);
                    }
                    for (int i = 0; i < lines.Length; i++)
                    {
                        string marco_line = lines[i].Trim();
                        string[] marco_arguments = marco_line.Split(' ', 2).Last().Split(", ");
                        string ArgList = "";
                        for (int a = 0; a < marco_arguments.Length; a++)
                        {
                            if (args.TryGetValue(marco_arguments[a], out string value))
                            {
                                marco_arguments[a] = value;
                            }
                            ArgList += marco_arguments[a] + ", ";
                        }
                        marco_line = marco_line.Split(' ', 2).First().Trim() + " " + ArgList.TrimEnd(' ').TrimEnd(',');
                        BuildInstruction(marco_line, 5);
                        m_rev--;
                    }
                    return;
                }
                throw new NotImplementedException("Implement Error");
            }

            m_BinaryOutput.Add($"_DEL_{line}");
            if (result == Instruction.POP)
            {
                if (Arguments[0] == "byte")
                {
                    Arguments[0] = Arguments[0].Replace("byte ", "");
                }
                if (Arguments[0] == "word")
                {
                    result = Instruction.POPW;
                    Arguments[0] = Arguments[0].Replace("word ", "");
                }
                if (Arguments[0] == "dword")
                {
                    result = Instruction.POPD;
                    Arguments[0] = Arguments[0].Replace("dword ", "");
                }
            }

            m_limit = true;
            ParseInstruction(result, Arguments, ref InstrutionBytes);

        }

        bool m_limit = true;
        void ParseInstruction(Instruction instruction, string[] Arguments, ref List<string> InstrutionBytes, int startingIndex = 0, bool DoCheck = true)
        {
            if (DoCheck)
            {
                InstrutionBytes.Add(Convert.ToString((int)instruction, 16).PadLeft(2, '0'));
                InstrutionBytes.Add(NONE);
                InstrutionBytes.Add(NONE);
                string String_bank = m_bankEnable ? "8" : "0";
                InstrutionBytes.Add(Convert.ToString(m_bank, 16) + String_bank);

            }

            if (instruction.ToString() == Arguments.First().ToUpper())
            {
                startingIndex = Arguments.Length + 1;
            }
            for (int i = startingIndex; i < Arguments.Length; i++)
            {
                if (DoCheck)
                {
                    InstrutionBytes.Add("_NEWARG_ " + Arguments[i]);
                }

                // parse argument
                if (Arguments[i].StartsWith("long"))
                {
                    Arguments[i] = Arguments[i].Replace("long ", "");
                    string[] newArguments = { Arguments[i] };

                    int ArgIndex = GetArgumentIndex(InstrutionBytes);

                    m_limit = false;

                    List<string> argumentBytes = new List<string>()
                    {
                        "00","00","00"
                    };

                    ParseInstruction(instruction, newArguments, ref argumentBytes, Math.Clamp(-1, 0, 0xFF), false);

                    int MissingBytes = -((argumentBytes.Count - 3) - 3);

                    for (int b = 0; b < MissingBytes; b++)
                    {
                        InstrutionBytes.Add("?? ; missing bytes from ^");
                    }

                    InstrutionBytes.AddRange(argumentBytes.GetRange(3, argumentBytes.Count - 3));

                    AddArgumentMode(ref InstrutionBytes, LONGADDRESS, ArgIndex);
                }
                else if (Parse(Arguments[i].Trim('[', ']')) != null && Arguments[i].StartsWith('[') && Arguments[i].EndsWith(']') && Parse(Arguments[i].Trim('[', ']')).First() == "BINEXPRWR")
                {
                    string[] data = Arguments[i].Trim('[', ']').Split(' ');
                    for (int a = 0; a < data.Length; a++)
                    {
                        if (Enum.TryParse(data[a].ToUpper(), out Register result) && (result == Register.BP || result == Register.SP))
                        {
                            AddArgumentMode(ref InstrutionBytes, STACKADDRESS);
                            string[] expr = parse_expr(Arguments[i].Trim('[', ']'));
                            string stringExpr = "";
                            for (a = 0; a < expr.Length; a++)
                            {
                                stringExpr += expr[a] + " ";
                            }
                            InstrutionBytes.Add(stringExpr);
                            break;
                        }
                    }
                }
                else if (IsVariabel(Arguments[i].Trim('[', ']'), out Variable variable))
                {
                    string expr = variable.m_Value;
                    if (expr.StartsWith('[') && expr.EndsWith(']'))
                    {
                        if (m_limit == false)
                        {
                            AddArgumentMode(ref InstrutionBytes, LONGADDRESS);
                        }
                        else
                        {
                            AddArgumentMode(ref InstrutionBytes, ADDRESS);
                        }
                    }
                    else
                    {
                        if (expr.Length <= 2)
                        {
                            AddArgumentMode(ref InstrutionBytes, IMMDIATEBYTE);
                        }
                        else if (expr.Length > 2 && expr.Length <= 4)
                        {
                            AddArgumentMode(ref InstrutionBytes, IMMDIATEWORD);
                        }
                        else if (expr.Length > 4 && expr.Length <= 6)
                        {
                            AddArgumentMode(ref InstrutionBytes, IMMDIATEDWORD);
                        }
                    }
                    string[] hexStrings = Parse(Arguments[i].Trim('[', ']'));
                    for (int a = 0; a < hexStrings.Length; a++)
                    {
                        InstrutionBytes.Add(hexStrings[a]);
                    }
                }
                else if (Arguments[i].StartsWith(".struct"))
                {
                    string data = Parse(Arguments[i])[0];

                    InstrutionBytes.Add(data);

                    AddArgumentMode(ref InstrutionBytes, LONGADDRESS);
                }
                //bin expr
                else if (Parse(Arguments[i]) != null && Parse(Arguments[i]).First() == "BINEXPR")
                {
                    string[] data = Parse(Arguments[i].Trim('[', ']'));
                    string expr = "";
                    for (int a = 0; a < data.Length; a++)
                    {
                        expr += data[a] + " ";
                    }
                    InstrutionBytes.Add(expr);
                    if (Arguments[i].StartsWith('[') && Arguments[i].EndsWith(']'))
                    {
                        AddArgumentMode(ref InstrutionBytes, LONGADDRESS);
                    }
                    else
                    {
                        AddArgumentMode(ref InstrutionBytes, IMMDIATEDWORD);
                    }
                }
                else if (Arguments[i].StartsWith('[') && Arguments[i].EndsWith(']') && Parse(Arguments[i]) != null)
                {
                    if (Enum.TryParse(Arguments[i].Trim('[', ']'), out Register result))
                    {
                        AddArgumentMode(ref InstrutionBytes, REGISTERADDRESS);
                        InstrutionBytes.Add(Convert.ToString((int)result, 16).PadLeft(2, '0'));
                        continue;
                    }
                    string[] data = Parse(Arguments[i]);
                    AddArgumentMode(ref InstrutionBytes, ADDRESS);

                    int max = data.Length;
                    if (m_limit)
                    {
                        if (data.Length > 3)
                        {
                            max = 3;
                        }
                        else if (data.First().StartsWith("L_"))
                        {
                        }
                        else if (data.Length < 2)
                        {
                            InstrutionBytes.Add("00");
                        }
                    }
                    else
                    {
                        if (data.First().StartsWith("L_"))
                        {
                            data[0] = "L" + data[0];
                        }
                        if (data.Length > 2)
                        {
                            max = 2;
                        }
                    }

                    for (int a = 0; a < max; a++)
                    {
                        InstrutionBytes.Add(data[a]);
                    }
                    if (m_limit)
                    {
                        InstrutionBytes.Add("");
                    }
                    else
                    {
                        if (data.Length < 3)
                        {
                            for (int a = data.Length; a < (3 - data.Length) + 1; a++)
                            {
                                InstrutionBytes.Add("");
                            }
                        }
                    }
                }
                else if (Arguments[i].StartsWith('(') && Arguments[i].EndsWith(")f") && Parse(Arguments[i]) != null)
                {
                    string[] data = Parse(Arguments[i]);
                    AddArgumentMode(ref InstrutionBytes, FLOATIMMDIATE);
                    for (int a = 0; a < data.Length; a++)
                    {
                        InstrutionBytes.Add(data[a]);
                    }
                }
                else if (Arguments[i].StartsWith("byte", true, CultureInfo.CurrentCulture) && instruction == Instruction.MOV)
                {
                    if (instruction == Instruction.MOV)
                    {
                        instruction = Instruction.MOV;
                    }
                    AddArgumentMode(ref InstrutionBytes, IMMDIATEBYTE);
                    string[] data = Parse(Arguments[i]);
                    InstrutionBytes.Add(data[0]);
                }
                else if (Arguments[i].StartsWith("word", true, CultureInfo.CurrentCulture) && instruction == Instruction.MOV)
                {
                    if (instruction == Instruction.MOV)
                    {
                        instruction = Instruction.MOVW;
                    }
                    AddArgumentMode(ref InstrutionBytes, IMMDIATEDWORD);
                    string[] data = Parse(Arguments[i]);
                    InstrutionBytes.Add(data[0]);
                    InstrutionBytes.Add(data[1]);
                }
                else if (Arguments[i].StartsWith("dword", true, CultureInfo.CurrentCulture) && instruction == Instruction.MOV)
                {
                    if (instruction == Instruction.MOV)
                    {
                        instruction = Instruction.MOVD;
                    }
                    AddArgumentMode(ref InstrutionBytes, IMMDIATEDWORD);
                    string[] data = Parse(Arguments[i]);
                    InstrutionBytes.Add(data[0]);
                    InstrutionBytes.Add(data[1]);
                    InstrutionBytes.Add(data[2]);
                    InstrutionBytes.Add(data[3]);
                }
                else if (Parse(Arguments[i]) != null)
                {
                    string[] data = Parse(Arguments[i]);
                    int max = data.Length;
                    if (data.Length > 2)
                    {
                        max = 2;
                    }
                    if (data.Length == 1)
                    {
                        AddArgumentMode(ref InstrutionBytes, IMMDIATEBYTE);
                        max = 1;
                    }
                    else if (data.Length == 2)
                    {
                        AddArgumentMode(ref InstrutionBytes, IMMDIATEWORD);
                    }
                    else if (data.Length == 4)
                    {
                        AddArgumentMode(ref InstrutionBytes, IMMDIATEDWORD);
                    }
                    else if (data.Length > 2)
                    {
                        AddArgumentMode(ref InstrutionBytes, IMMDIATEWORD);
                        max = 2;
                    }
                    else if (data.Length > 4)
                    {
                        AddArgumentMode(ref InstrutionBytes, IMMDIATEDWORD);
                        max = 4;
                    }
                    for (int a = 0; a < max; a++)
                    {
                        InstrutionBytes.Add(data[a]);
                    }
                }
                else if (Enum.TryParse(Arguments[i].ToUpper(), out Register result))
                {
                    InstrutionBytes.Add(Convert.ToString((int)result, 16).PadLeft(2, '0'));
                    AddArgumentMode(ref InstrutionBytes, REGISTER);
                }
                else if (Arguments[i].Contains(':') && Arguments[i].StartsWith('[') && Arguments[i].EndsWith(']'))
                {
                    string segment = Arguments[i].Split(':').First().Trim('[');
                    string offset = Arguments[i].Split(':').Last().Trim(']');

                    if (Parse(segment) == null)
                    {
                        AddArgumentMode(ref InstrutionBytes, SEGMENT);
                        if (!Enum.TryParse(segment.ToUpper(), out Register result1))
                        {
                            throw new NotImplementedException();
                        }
                        if (!Enum.TryParse(offset.ToUpper(), out Register result2))
                        {
                            throw new NotImplementedException();
                        }

                        InstrutionBytes.Add(Convert.ToString((int)result1, 16).PadLeft(2, '0'));
                        InstrutionBytes.Add(Convert.ToString((int)result2, 16).PadLeft(2, '0'));
                    }
                    else
                    {
                        AddArgumentMode(ref InstrutionBytes, SEGMENTIMMDIATE);

                        string[] data = Parse(segment);

                        int max = 2;

                        if (data.Length < 2)
                        {
                            max = 1;
                            InstrutionBytes.Add("00");
                        }

                        if (!Enum.TryParse(offset.ToUpper(), out Register result2))
                        {
                            throw new NotImplementedException();
                        }

                        for (int a = 0; a < max; a++)
                        {
                            InstrutionBytes.Add(data[a]);
                        }

                        InstrutionBytes.Add(Convert.ToString((int)result2, 16).PadLeft(2, '0'));
                    }
                }
                else if (IsLetter(Arguments[i]))
                {
                    AddArgumentMode(ref InstrutionBytes, IMMDIATETBYTE);
                    InstrutionBytes.Add($"LL_{Arguments[i]}");
                    InstrutionBytes.Add($"");
                    InstrutionBytes.Add($"");
                }
                else
                {
                    throw new NotImplementedException(Arguments[i]);
                }
            }

            if (DoCheck)
            {
                InstrutionBytes[1] = Convert.ToString(Convert.ToUInt16(InstrutionBytes[1].PadRight(8, '1'), 2), 16).PadLeft(2, '0');
                InstrutionBytes[2] = Convert.ToString(Convert.ToUInt16(InstrutionBytes[2].PadRight(8, '1'), 2), 16).PadLeft(2, '0');
                m_BinaryOutput.AddRange(InstrutionBytes);
            }
        }
        void AddArgumentMode(ref List<string> InstrutionBytes, string ArgumentMode)
        {
            if (InstrutionBytes[1] == NONE)
            {
                InstrutionBytes[1] = ArgumentMode;
            }
            else
            {
                InstrutionBytes[2] = ArgumentMode;
            }
        }
        void AddArgumentMode(ref List<string> InstrutionBytes, string ArgumentMode, int index)
        {
            InstrutionBytes[index] = ArgumentMode;
        }
        int GetArgumentIndex(List<string> InstrutionBytes)
        {
            if (InstrutionBytes[1] == NONE)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
        int m_structSize = 0;
        bool m_getStructSize = false;
        Struct m_tempStruct = new Struct();
        void BuildDirectives()
        {
            string line = m_Src[m_lineNumber].Trim().TrimStart('.');
            //.DirectiveInstruction arguement1, arguement2 ...
            string Instruction = line.Split(' ', 2)[0].ToLower();
            string PreArguments = line.Split(' ', 2).Last();
            string[] Arguments = PreArguments.Split(", ");

            //m_BinaryOutput.Add($"{Convert.ToString(m_lineNumber, 16).PadLeft(4, '0')}:\t" + Instruction + " " + PreArguments + " with a length of " + Arguments.Length);

            string[] data;

            switch (Instruction)
            {
                case "newfile":
                    m_file = Arguments[0];
                    m_BinaryOutput.Add($"_FILE_ {Arguments[0]}");
                    break;
                case "db":
                case "byte":

                    if (m_getStructSize)
                    {
                        if (Arguments[0].Contains('[') && Arguments[0].EndsWith(']'))
                        {
                            int size = int.Parse(Arguments[0].Split('[')[1].TrimEnd(']')) * 1;
                            Arguments[0] = Arguments[0].Split('[')[0].TrimEnd(']');
                            m_tempStruct.m_Fields.Add(new DataFields() { m_Name = Arguments[0], m_Offset = m_structSize, m_Size = 1, m_IsArray = true });
                            m_structSize += size;
                            break;
                        }
                        m_tempStruct.m_Fields.Add(new DataFields() { m_Name = Arguments[0], m_Offset = m_structSize, m_Size = 1 });
                        m_structSize++;
                    }
                    else
                    {
                        for (int i = 0; i < Arguments.Length; i++)
                        {
                            data = Parse(Arguments[i]);
                            m_BinaryOutput.Add(data[0]);
                        }
                    }
                    break;
                case "dw":
                case "word":
                    if (m_getStructSize)
                    {
                        if (Arguments[0].StartsWith('[') && Arguments[0].EndsWith(']'))
                        {
                            int size = int.Parse(Arguments[0].Split('[')[1].TrimEnd(']')) * 2;
                            Arguments[0] = Arguments[0].Split('[')[0].TrimEnd(']');
                            m_tempStruct.m_Fields.Add(new DataFields() { m_Name = Arguments[0], m_Offset = m_structSize, m_Size = 1, m_IsArray = true });
                            m_structSize += size;
                            break;
                        }
                        m_tempStruct.m_Fields.Add(new DataFields() { m_Name = Arguments[0], m_Offset = m_structSize, m_Size = 2 });
                        m_structSize += 2;
                    }
                    else
                    {
                        for (int i = 0; i < Arguments.Length; i++)
                        {
                            data = Parse(Arguments[i]);
                            for (int a = 0; a < data.Length; a++)
                            {
                                m_BinaryOutput.Add(data[a]);
                            }
                        }
                    }
                    break;
                case "dd":
                case "dword":
                    if (m_getStructSize)
                    {
                        if (Arguments[0].StartsWith('[') && Arguments[0].EndsWith(']'))
                        {
                            int size = int.Parse(Arguments[0].Split('[')[1].TrimEnd(']')) * 4;
                            Arguments[0] = Arguments[0].Split('[')[0].TrimEnd(']');
                            m_tempStruct.m_Fields.Add(new DataFields() { m_Name = Arguments[0], m_Offset = m_structSize, m_Size = 1, m_IsArray = true });
                            m_structSize += size;
                            break;
                        }
                        m_tempStruct.m_Fields.Add(new DataFields() { m_Name = Arguments[0], m_Offset = m_structSize, m_Size = 4 });
                        m_structSize += 4;
                    }
                    else
                    {
                        for (int i = 0; i < Arguments.Length; i++)
                        {
                            data = Parse(Arguments[i]);
                            for (int a = 0; a < data.Length; a++)
                            {
                                m_BinaryOutput.Add(data[a]);
                            }
                        }
                    }
                    break;
                case "str":
                case "string":
                    for (int i = 0; i < Arguments.Length; i++)
                    {
                        data = Parse(Arguments[i]);
                        for (int a = 0; a < data.Length; a++)
                        {
                            m_BinaryOutput.Add(data[a]);
                        }
                    }
                    m_BinaryOutput.Add("00");
                    break;
                case "org":
                    data = Parse(Arguments[0]);
                    string hexString = "";
                    for (int i = 0; i < data.Length; i++)
                    {
                        hexString += data[i];
                    }
                    m_BinaryOutput.Add($"_OFF_ {hexString}");
                    break;
                case "includeil":
                    m_BinaryOutput.Add($"_INCIL_ {Arguments[0].Trim('"')}");
                    break;
                case "include":
                    m_BinaryOutput.Add($"_INC_ {Arguments[0].Trim('"')}");
                    break;
                case "struct":
                    m_structSize = 0;
                    m_tempStruct.m_Fields.Clear();

                    string name = Arguments[0];
                    m_tempStruct.m_Name = name;
                    m_getStructSize = true;
                    break;
                case "end":
                    if (Arguments[0] == "struct")
                    {
                        m_getStructSize = false;
                        m_tempStruct.m_Size = m_structSize;

                        m_Structs.Add(m_tempStruct);

                    }
                    break;
                case "make_struct":
                    for (int i = 0; i < m_Structs.Count; i++)
                    {
                        if (Arguments[0] == m_Structs[i].m_Name)
                        {
                            m_BinaryOutput.Add($"MS_{m_Structs[i].m_Name}");
                            for (int a = 0; a < m_Structs[i].m_Size - 1; a++)
                            {
                                m_BinaryOutput.Add($"00");
                            }
                        }
                    }
                    break;
                case "bank":
                    if (Arguments[0] == "enable")
                    {
                        m_bankEnable = true;
                    }
                    else if (Arguments[0] == "disable")
                    {
                        m_bankEnable = false;
                    }
                    else
                    {
                        m_bank = Convert.ToInt16(Parse(Arguments[0])[0], 16);
                    }
                    break;
                case "bankenable":
                    m_bankEnable = true;
                    break;
                case "bankdisable":
                    m_bankEnable = false;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
        void MakeMarco()
        {
            string name = m_Src[m_lineNumber].Trim('@').Split(' ', 2)[0];
            string[] arguments = m_Src[m_lineNumber].Split(' ', 2).First().Split(' ');
            if (name == arguments[0].Trim('@'))
            {
                arguments = Array.Empty<string>();
            }
            List<string> lines = new List<string>();
            for (int i = m_lineNumber + 1; !m_Src[i].StartsWith(".ENDM"); i++)
            {
                lines.Add(m_Src[i]);
                m_lineNumber++;
            }
            m_lineNumber++;
            m_marcos.Add(new Marcos() { Name = name, arguments =  arguments, lines = lines.ToArray() });
            return;
        }
        void MakeLabel()
        {
            bool IsGlobal = m_Src[m_lineNumber].ToLower().Contains(".global");

            string name = m_Src[m_lineNumber].Replace(".global ", "").Trim(':');

            m_BinaryOutput.Add($"_REF_ LABEL {name},{IsGlobal}");
            return;
        }
        void CodeOptimization()
        {
            if (m_lineNumber < 3) return;
            string line = m_Src[m_lineNumber].TrimStart(' ').ToLower();
            string LastLine = m_Src[m_lineNumber - 1].TrimStart(' ').ToLower();
            string LastLastLine = m_Src[m_lineNumber - 2].TrimStart(' ').ToLower();

            if (line.StartsWith('.')) return;
            string[] sections = line.Split(' ');
            string[] LastSections = LastLine.Split(' ');
            string[] LastLastSections = LastLastLine.Split(' ');

            if (sections[0] == "add" && sections[2].Trim(',') == "1")
            {
                m_Src[m_lineNumber] = $"inc {sections[1].Trim(',')}";
                //AssemblerWarnings.CodeOptimization(m_lineNumber, m_file, line, m_Src[m_lineNumber]);
            }
            else if (sections[0] == "sub" && sections[2].Trim(',') == "1")
            {
                m_Src[m_lineNumber] = $"dec {sections[1].Trim(',')}";
                //AssemblerWarnings.CodeOptimization(m_lineNumber, m_file, line, m_Src[m_lineNumber]);
            }
            else if (sections[0] == "mov" && sections[2].Trim(',') == "0")
            {
                m_Src[m_lineNumber] = $"sez {sections[1].Trim(',')}";
                //AssemblerWarnings.CodeOptimization(m_lineNumber, m_file, line, m_Src[m_lineNumber]);
            }
            else if (sections[0] == "halt" &&
                     ((LastSections[0] == "jmp" && LastSections[1].Trim(',') == "$") ||
                     (LastLastSections[0] == "jmp" && LastLastSections[1].Trim(',') == "$")))
            {
                AssemblerWarnings.RedundantCodeBecurseOfJump(m_lineNumber, m_file, line);
                m_Src[m_lineNumber] = $"";
            }
        }
        bool IsMarcos(string name, out Marcos marcos)
        {
            for (int i = 0; i < m_marcos.Count; i++)
            {
                if (m_marcos[i].Name == name)
                {
                    marcos = m_marcos[i];
                    return true;
                }
            }
            marcos = null;
            return false;
        }
        List<string> SplitHexString(string hexString)
        {
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
        string[] Parse(string expr)
        {
            if (expr.StartsWith(".struct"))
            {
                string structExpr = expr.Split(' ')[1];

                string structName = structExpr.Split('.', 2)[0];
                if (structExpr.Contains('.'))
                {

                    string structField = structExpr.Split('.', 2)[1];

                    return new string[] { $"SF_{structName}.F_{structField}" };
                }
                else
                {
                    return new string[] { $"SBA_{structName}" };
                }
            }
            else if (IsVariabel(expr, out Variable variable))
            {
                string data = variable.m_Value;
                if (expr.StartsWith('[') && expr.EndsWith(']'))
                {
                    if (m_limit == false)
                    {
                        data = data.PadLeft(6, '0');
                    }
                    else
                    {
                        data = data.PadLeft(4, '0');
                    }
                }
                else
                {
                    if (data.Length >= 2)
                    {
                        data = data.PadLeft(2, '0');
                    }
                    else if (data.Length < 2 && data.Length >= 4)
                    {
                        data = data.PadLeft(4, '0');
                    }
                    else if (data.Length < 4 && data.Length >= 6)
                    {
                        data = data.PadLeft(6, '0');
                    }
                }
                return SplitHexString(data).ToArray();
            }
            else if (expr == "$")
            {
                return new string[] { "_CA_", "" };
            }
            else if (expr.ToLower().StartsWith("byte"))
            {
                return new string[] { Parse(expr.Replace("byte ", ""))[0] };
            }
            else if (expr.ToLower().StartsWith("word"))
            {
                string data = expr.Replace("word ", "");
                string[] parsedData = Parse(data);
                return new string[] { parsedData[0], parsedData[1] };
            }
            else if (expr.ToLower().StartsWith("dword"))
            {
                string data = expr.Replace("dword ", "");
                string[] parsedData = Parse(data);
                return new string[] { parsedData[0], parsedData[1], parsedData[2], parsedData[3] };
            }
            else if (expr.StartsWith('(') && expr.EndsWith(")f"))
            {
                string data = expr.TrimEnd('f').Trim('(', ')');

                // Convert the string to float
                float floatValueParsed = float.Parse(data);

                // Get the binary representation of the float
                int floatToInt = BitConverter.ToInt32(BitConverter.GetBytes(floatValueParsed), 0);
                string binaryString = Convert.ToString(floatToInt, 2);

                // Ensure the binary string is 32 bits long (for single precision float)
                binaryString = binaryString.PadLeft(32, '0');

                return SplitHexString(Convert.ToString(Convert.ToInt32(binaryString, 2), 16)).ToArray();
            }
            else if (expr.StartsWith('-'))
            {
                string data = expr.TrimStart('-');

                short SignedInt = ConventToDec(data, out short _);

                if ((SignedInt & 0x8000) != 0x8000)
                    SignedInt = (short)-SignedInt;

                string hexString = Convert.ToString(SignedInt, 16);

                return SplitHexString(hexString).ToArray();
            }
            else if ((expr.StartsWith("0x") || expr.StartsWith('$')) && !expr.Contains(' '))
            {
                string data = expr.Replace("0x", "").TrimStart('$');
                int dec_data = Convert.ToInt32(data, 16);
                return SplitHexString(Convert.ToString(dec_data, 16)).ToArray();
            }
            else if (expr.StartsWith("0b"))
            {
                string data = expr.Replace("0b", "");
                int dec_data = Convert.ToInt32(data, 2);
                return SplitHexString(Convert.ToString(dec_data, 16)).ToArray();
            }
            else if (expr.StartsWith("\"") && expr.EndsWith("\""))
            {
                string data = expr.Trim('"');
                byte[] bytes = Encoding.ASCII.GetBytes(data);
                List<string> result = new List<string>();
                for (int i = 0; i < bytes.Length; i++)
                {
                    result.Add(Convert.ToString(bytes[i], 16));
                }
                return result.ToArray();
            }
            else if (expr.StartsWith("\'") && expr.EndsWith("\'"))
            {
                string data = expr.Trim('\'');
                byte[] bytes = Encoding.ASCII.GetBytes(data);
                List<string> result = new List<string>();
                result.Add(Convert.ToString(bytes[0], 16));
                return result.ToArray();
            }
            else if (IsDigit(expr))
            {
                string data = expr.Replace("0d", "");
                int dec_data = Convert.ToInt32(data);
                return SplitHexString(Convert.ToString(dec_data, 16)).ToArray();
            }
            else if (expr.StartsWith('[') && expr.EndsWith(']'))
            {
                string addr = expr.Trim('[', ']');
                if (IsLetter(addr))
                {
                    return new string[] { $"L_{addr}" };
                }
                else if (Parse(addr) != null)
                {
                    return Parse(addr);
                }

            }
            else if (ContainsBinProc(expr, out _))
            {
                return parse_expr(expr);
            }
            else if (Enum.TryParse(expr.ToUpper(), out Register result))
            {
                return null;
            }
            else
            {
                //Console.WriteLine(expr);
            }

            return null;
        }
        string[] parse_expr(string expr)
        {
            Stack<string> holdingStack = new Stack<string>();
            Stack<string> OutputStack = new Stack<string>();

            string buf;

            string lastToken = null;

            string ident = "BINEXPR";

            for (int i = 0; i < expr.Length; i++)
            {
                buf = "";
                if (char.IsLetterOrDigit(expr[i]))
                {
                    lastToken = "lit";
                    while (i < expr.Length && char.IsLetterOrDigit(expr[i]))
                    {
                        buf += expr[i];
                        i++;
                    }
                    i--;
                    if (IsLetter(buf) && Enum.TryParse(buf, true, out Register _))
                    {
                        OutputStack.Push("R_" + buf);
                        ident = "BINEXPRWR";
                        continue;
                    }
                    else if (IsLetter(buf) && IsVariabel(buf, out Variable variable))
                    {
                        OutputStack.Push($"I_{variable.m_Value}");
                        continue;
                    }
                    else if (IsLetter(buf))
                    {
                        OutputStack.Push("L_" + buf);
                        continue;
                    }

                    string[] buffer = Parse(buf);
                    buf = "";

                    for (int b = 0; b < buffer.Length; b++)
                    {
                        buf += buffer[b];
                    }

                    OutputStack.Push("I_" + buf);
                }
                else if (expr[i] == '$')
                {
                    OutputStack.Push("_CA_");
                }
                else if (expr[i] == '(')
                {
                    lastToken = "(";
                    holdingStack.Push("(");
                }
                else if (expr[i] == ')')
                {
                    lastToken = ")";
                    string result;
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
                else if (GetBinProc(expr[i]) != -1)
                {
                    char newToken = expr[i];
                    i++;

                    if (newToken == '-')
                    {
                        if (lastToken == null || 
                            (lastToken != "lit" && lastToken != ")"))
                        {
                            newToken = 'n';
                        }
                    }

                    while (holdingStack.TryPeek(out string result) && result != "(")
                    {
                        if (ContainsBinProc(result, out int binproc))
                        {
                            if (binproc >= GetBinProc(newToken))
                            {
                                OutputStack.Push(holdingStack.Pop());
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    holdingStack.Push(newToken.ToString());
                    lastToken = newToken.ToString();
                }
                else if (char.IsWhiteSpace(expr[i]))
                {
                    continue;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            while (holdingStack.TryPeek(out _))
            {
                OutputStack.Push(holdingStack.Pop());
            }

            OutputStack.Push(ident);

            return OutputStack.ToArray();
        }
        bool ContainsBinProc(string expr, out int binproc)
        {
            for (int i = 0; i < expr.Length; i++)
            {
                if (new string("+-*/").Contains(expr[i]))
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
            if (c == '-') return 1;
            else if (c == '+') return 2;
            else if (c == '*') return 3;
            else if (c == '/') return 4;
            else if (c == 'n') return 10;
            else
            {
                return -1;
            }
        }
        short ConventToDec(string str, out short dec)
        {
            string[] data = Parse(str);

            int max = data.Length;

            if (data.Length > 2) max = 2;

            string hexString = "";
            for (int i = 0; i < max; i++)
            {
                hexString += data[i];
            }

            int temp = Convert.ToInt16(hexString.PadLeft(4, '0'), 16);
            dec = (short)temp;
            return (short)temp;
        }

        bool IsDigit(string expr)
        {
            for (int i = 0; i < expr.Length; i++)
            {
                if (!char.IsDigit(expr[i])) return false;
            }
            return true;
        }
        bool IsLetter(string expr)
        {
            for (int i = 0; i < expr.Length; i++)
            {
                if (!char.IsLetter(expr[i]) && expr[i] != '_') return false;
            }
            return true;
        }
    }
}
