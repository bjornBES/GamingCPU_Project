using HexLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using static HexLibrary.HexConverter;
using static HexLibrary.StringFunctions;

public class Assembler : AssmeblerErrors
{
    public string[] m_Src;
    public string[] m_SrcRaw;
    public string m_SrcPreprocedCode;

    List<Variable> m_variables = new List<Variable>();
    List<Label> m_labels = new List<Label>();

    public List<string> m_output = new List<string>();
    public List<string> m_output_symbols = new List<string>();
    public List<string> m_output_section = new List<string>();
    public List<string> m_output_structs = new List<string>();
    public List<string> m_output_includedFiles = new List<string>();

    bool IsNear = false;
    bool IsLong = false;
    bool IsFar = false;
    public string m_file;

    Section m_section = Section.Text;

    Stack<int> m_BPX_stack = new Stack<int>();

    int BPX_ref = 0;
    int m_oldBPX = 0;
    int SPX_ref = 0;

    List<Struct> m_structs = new List<Struct>();

    List<string> m_DoneFiles = new List<string>();

    bool Going = false;

    int m_pc = 0;
    public int lineNumber = 0;

    public static bool m_WriteOut = true;

    public void Build(string src)
    {
        if (!Going)
        {
            Going = !(m_pc == 0);
        }

        if (!Going)
        {
            m_output.Clear();
            m_output_symbols.Clear();
            m_output_section.Clear();
            m_output_structs.Clear();
            m_output_includedFiles.Clear();
            Going = true;
        }
        else
        {
            Going = false;
        }

        m_SrcRaw = src.Split(Environment.NewLine);

        src = Regex.Replace(src, @";[^\r\n]*(\r?\n)", Environment.NewLine);
        src = Regex.Replace(src, @" +", " ");

        m_SrcPreprocedCode += src;

        m_Src = src.Split(Environment.NewLine);

        buildMarcos();

        Assemble();

        if (Going)
        {
            m_output.Insert(0, "TEXT HEADER");
            m_output_symbols.Insert(0, "SYMBOLS HEADER");
            m_output_section.Insert(0, "SECTION HEADER");
            m_output_structs.Insert(0, "STRUCT HEADER");
            m_output_includedFiles.Insert(0, "INCLUDE HEADER");

            for (int i = 0; i < m_structs.Count; i++)
            {
                Struct currstruct = m_structs[i];

                string name = currstruct.m_Name;
                List<StructMembers> members = currstruct.m_StructMembers;

                string format = $"{name} res {currstruct.m_Size} {{";
                for (int c = 0; c < members.Count; c++)
                {
                    StructMembers currMember = members[c];
                    format += $"{currMember.m_Name} res {currMember.m_Size}";
                    if (c != members.Count - 1)
                    {
                        format += ",";
                    }
                }
                format += "}";
                m_output_structs.Add(format);
            }

            m_output_symbols.Add(Environment.NewLine);
            m_output_section.Add(Environment.NewLine);
            m_output_structs.Add(Environment.NewLine);
            m_output_includedFiles.Add(Environment.NewLine);

            m_output.InsertRange(0, m_output_symbols);
            m_output.InsertRange(0, m_output_structs);
            m_output.InsertRange(0, m_output_section);
            m_output.InsertRange(0, m_output_includedFiles);
        }

        Going = false;
    }

    void buildMarcos()
    {
        for (lineNumber = 0; lineNumber < m_Src.Length; lineNumber++)
        {

            m_Src[lineNumber] = Regex.Replace(m_Src[lineNumber], @";[^.]*", Environment.NewLine);
            m_Src[lineNumber] = Regex.Replace(m_Src[lineNumber], @" +", " ");

            m_Src[lineNumber] = m_Src[lineNumber].Trim('\r', '\n');

            if (string.IsNullOrEmpty(m_Src[lineNumber]))
            {
                continue;
            }

            //struct

            if (m_Src[lineNumber].StartsWith(".struct"))
            {
                BuildDirectives();
            }
            else if (m_Src[lineNumber].StartsWith('@'))
            {

            }
            else if (m_Src[lineNumber].StartsWith(".SETCPU"))
            {
                string CPU = m_Src[lineNumber].Split(' ')[1].Trim('\"');

                CPUType type = (CPUType)Enum.Parse(typeof(CPUType), CPU);

                CPUType = type;

                Register[] registers = Registers.regs.Keys.ToArray();

                for (int i = 0; i < registers.Length; i++)
                {
                    Registers.regs[registers[i]].m_size = Registers.regs[registers[i]].GetSize();
                }
            }
            else if (m_Src[lineNumber].StartsWith('$'))
            {
                string[] line = m_Src[lineNumber].Split(' ');

                string name = line[0].Substring(1);
                if (line[1] == "=")
                {
                    string SizeAlignment = "word";
                    string value = parseTerm(line[2], ref SizeAlignment, out _, out ArgumentMode mode);

                    m_variables.Add(new Variable()
                    {
                        m_Name = name,
                        m_Value = value,
                        m_Mode = mode,
                    });
                }
            }

            //AddLine("BM:\t" + m_Src[i]);
        }
    }
    void Assemble()
    {
        for (lineNumber = 0; lineNumber < m_Src.Length; lineNumber++)
        {
            m_assembler = this;
            if (lineNumber == 0)
                continue;

            m_Src[lineNumber] = Regex.Replace(m_Src[lineNumber], @";[^.]*", Environment.NewLine);
            m_Src[lineNumber] = Regex.Replace(m_Src[lineNumber], @" +", " ");

            AddLine("_NEWLINE_");

            m_Src[lineNumber] = m_Src[lineNumber].Trim('\r', '\n');
            m_Src[lineNumber] = m_Src[lineNumber].TrimStart(' ');
            m_Src[lineNumber] = m_Src[lineNumber].Trim();

            if (string.IsNullOrEmpty(m_Src[lineNumber]))
            {
                continue;
            }

            if (m_Src[lineNumber].EndsWith(':'))
            {
                MakeLabel();
            }
            else if (m_Src[lineNumber].Contains(".section"))
            {
                string section = m_Src[lineNumber].Replace(".section ", "");
                AddLine($"_{section} {HexPC()} {m_file}", Section.Section);
                SetSection(section);
            }
            else if (m_Src[lineNumber].StartsWith('.'))
            {
                if (m_Src[lineNumber].StartsWith(".struct"))
                {
                    while (m_Src[lineNumber].StartsWith(".EndStruct") == false)
                    {
                        lineNumber++;
                    }
                    lineNumber--;
                    continue;
                }
                BuildDirectives();
            }
            else if (m_Src[lineNumber].StartsWith('$'))
            {
                continue;
            }
            else
            {
                string line = m_Src[lineNumber].Trim();
                //m_rev = 0;
                ParseInstruction(line);
            }
            //AddLine(m_Src[i]);
        }
    }

    private void SetSection(string section)
    {
        if (section == TextSection)
        {
            m_section = Section.Text;
        }
        else if (section == DataSection)
        {
            m_section = Section.Data;
        }
        else if (section == BSSSection)
        {
            m_section = Section.Bss;
        }
        else if (section == RDataSection)
        {
            m_section = Section.Rdata;
        }
    }

    bool m_getStructSize = false;
    int m_structSize = 0;

    private void BuildDirectives()
    {
        string line = m_Src[lineNumber].Trim().TrimStart('.');
        //.DirectiveInstruction arguement1, arguement2 ...
        string Instruction = line.Split(' ', 2)[0].ToLower();
        string PreArguments = line.Split(' ', 2).Last();
        string[] Arguments = PreArguments.Split(", ");

        string[] output = BuildDirectives(Instruction, Arguments, out int size);

        if (output == null)
        {
            return;
        }    

        for (int d = 0; d < output.Length; d++)
        {
            AddLine(output[d]);
            m_pc += size;
        }
    }
    private string[] BuildDirectives(string Instruction, string[] Arguments, out int size)
    {
        string data;
        string SizeAlignment = "";

        switch (Instruction)
        {
            case "newfile":
                m_file = Arguments[0];
                if (m_DoneFiles.Contains(m_file))
                {
                    lineNumber = m_Src.Length + 1;
                    break;
                }
                AddLine($"_FILE_ \"{Arguments[0]}\"");
                m_DoneFiles.Add(m_file);
                break;
            case "times":

                string line = Arguments[0].Split(".").Last();

                string instr = line.Split(' ').First();
                string numberS = Arguments[0].Replace(" ." + line, "");

                string[] args = line.Split(' ', 2).Last().Split(',');

                string[] Strresult = BuildDirectives(instr, args, out int _size);
                string Hexresult = "";

                for (int i = 0; i < Strresult.Length; i++)
                {
                    Hexresult += Strresult[i];
                }

                string timesHex = parseTerm(numberS, "word", out bool split, out _);
                AddLine($"_TIMES_ {timesHex},{Hexresult}.{_size}");
                break;
            case "org":
                string HexOffset = parseTerm(Arguments[0], ref SizeAlignment, out _, out _);
                int offset = Convert.ToInt32(HexOffset, 16);

                m_pc = offset;
                AddLine($"_OFF_ {HexOffset}");
                break;
            case "includeil":
                string FileName = Arguments[0].Trim('\"').Split('\\', '/').Last();
                string OutputPath = GetPath(AssemblerSettings.OutputFile);
                int FileIndex = Files.FindIndex(file =>
                {
                    return file.Name == FileName;
                });
                string OutputFile = $"{OutputPath}{Path.DirectorySeparatorChar}{FileName.Split('.').First()}.o";
                AddLine($"INCIN {FindPath(OutputFile, FileIndex)}", Section.IncludedFiles);

                string[] savem_src = new string[m_Src.Length];
                m_Src.CopyTo(savem_src, 0);

                int save_I = lineNumber;
                string src = File.ReadAllText($"{FindPath($"./{FileName}", FileIndex)}");

                string Savem_file = m_file;
                m_file = FindPath($"./{FileName}", FileIndex);
                AddLine($"_FILE_ \"{m_file}\"");
                bool SaveGoing = Going;
                Going = true;

                Build(src);

                m_DoneFiles.Add(m_file);
                m_file = Savem_file;

                m_Src = savem_src;
                lineNumber = save_I;
                Going = SaveGoing;
                break;

            case "res":

                string outputExpr;
                if (m_getStructSize)
                {
                    outputExpr = parseTerm(Arguments[0], "byte", out _, out _);
                    int number = Convert.ToInt16(outputExpr, 16);
                    StructSize(number);
                    break;
                }
                else
                {
                    if (IsStruct(Arguments[0], out Struct result))
                    {
                        string hexSize = Convert.ToString(result.m_Size, 16);
                        AddLine($"_REF_ STRUCT {result.m_Name},{hexSize} {m_labels.Last().m_Name}", Section.Symbols);

                        m_labels[^1].m_HaveStruct = true;
                        m_labels[^1].m_Struct = result;
                        
                        m_pc += result.m_Size;
                    }
                    else if (parseTerm(Arguments[0], "word", out split, out _, out outputExpr))
                    {
                        int res = Convert.ToInt16(outputExpr, 16);
                        m_pc += res;

                        AddLine($"_OFF_ {Convert.ToString(m_pc, 16)}");
                    }
                }
                break;
            case "reschar":
            case "resc":
            case "resb":
            case "resbyte":
                if (m_getStructSize)
                {
                    StructSize(1);
                    break;
                }
                else
                {
                    AddLine($"_OFF_ {Convert.ToString(m_pc + 1, 16)}");
                    m_pc++;
                }
                break;

            case "resushort":
            case "resw":
            case "resword":
                if (m_getStructSize)
                {
                    StructSize(2);
                    break;
                }
                else
                {
                    AddLine($"_OFF_ {Convert.ToString(m_pc + 2, 16)}");
                    m_pc += 2;
                }
                break;

            case "rest":
            case "restbyte":
                if (m_getStructSize)
                {
                    StructSize(3);
                    break;
                }
                else
                {
                    AddLine($"_OFF_ {Convert.ToString(m_pc + 3, 16)}");
                    m_pc += 3;
                }
                break;

            case "resint":
            case "resd":
            case "resdword":
                if (m_getStructSize)
                {
                    StructSize(4);
                    break;
                }
                else
                {
                    AddLine($"_OFF_ {Convert.ToString(m_pc + 4, 16)}");
                    m_pc += 4;
                }
                break;

            case "reslong":
            case "resq":
            case "resqword":
                if (m_getStructSize)
                {
                    StructSize(8);
                    break;
                }
                else
                {
                    AddLine($"_OFF_ {Convert.ToString(m_pc + 8, 16)}");
                    m_pc += 8;
                }
                break;

            case "db":
            case "char":
            case "byte":
                for (int i = 0; i < Arguments.Length; i++)
                {
                    if (parseTerm(Arguments[i], "byte", out bool DoSplit, out ArgumentMode mode, out data))
                    {
                        string[] output;

                        if (DoSplit)
                        {
                            output = SplitHexString(data);
                        }
                        else
                        {
                            output = new string[]
                            {
                                data
                            };
                        }

                        size = 1;
                        return output;
                    }
                }
                break;

            case "dw":
            case "ushort":
            case "word":

                for (int i = 0; i < Arguments.Length; i++)
                {
                    if (parseTerm(Arguments[i], "word", out bool DoSplit, out ArgumentMode mode, out data))
                    {
                        string[] output;

                        if (DoSplit)
                        {
                            output = SplitHexString(data, 2);
                        }
                        else
                        {
                            output = new string[]
                            {
                                data
                            };
                        }

                        size = 2;
                        return output;
                    }
                }
                break;

            case "dt":
            case "tbyte":
                for (int i = 0; i < Arguments.Length; i++)
                {
                    if (parseTerm(Arguments[i], "tbyte", out bool DoSplit, out ArgumentMode mode, out data))
                    {
                        string[] output;

                        if (DoSplit)
                        {
                            output = SplitHexString(data, 3);
                        }
                        else
                        {
                            output = new string[]
                            {
                                data
                            };
                        }

                        size = 3;
                        return output;
                    }
                }
                break;

            case "dd":
            case "int":
            case "dword":
                for (int i = 0; i < Arguments.Length; i++)
                {
                    if (parseTerm(Arguments[i], "dword", out bool DoSplit, out ArgumentMode mode, out data))
                    {
                        string[] output;

                        if (DoSplit)
                        {
                            output = SplitHexString(data, 4);
                        }
                        else
                        {
                            output = new string[]
                            {
                                data,
                                "",
                                "",
                                "",
                            };
                        }

                        size = 4;
                        return output;
                    }
                }
                break;
            case "dq":
            case "long":
            case "qword":
                for (int i = 0; i < Arguments.Length; i++)
                {
                    if (parseTerm(Arguments[i], "qword", out bool DoSplit, out ArgumentMode mode, out data))
                    {
                        string[] output;

                        if (DoSplit)
                        {
                            output = SplitHexString(data, 8);
                        }
                        else
                        {
                            output = new string[]
                            {
                                data
                            };
                        }

                        size = 8;
                        return output;
                    }
                }
                break;

            case "struct":
                string name = Arguments[0];
                DoStruct(name);
                break;
            case "EndStruct":
                break;
            default:
                break;
        }
        size = 0;
        return null;
    }

    bool IsStruct(string name, out Struct result)
    {
        for (int i = 0; i < m_structs.Count; i++)
        {
            if (name == m_structs[i].m_Name)
            {
                result = m_structs[i];
                return true;
            }
        }
        result = null;
        return false;
    }

    void StructSize(int BytesRes)
    {
        m_structSize += BytesRes;
    }

    void DoStruct(string name)
    {
        m_structSize = 0;
        Struct TempStruct = new Struct();
        TempStruct.m_Name = name;

        lineNumber++;

        m_getStructSize = true;

        for (; lineNumber < m_Src.Length; lineNumber++)
        {
            string currLine = m_Src[lineNumber].Trim();

            if (currLine == ".EndStruct")
            {
                break;
            }
            else
            {
                int currSize = m_structSize;

                string MemberName = currLine.Split(' ', 2).First();
                string instr = currLine.Split(' ', 2).Last().TrimStart('.');

                string[] argument = instr.Split(' ', 2).Last().Split(' ');
                instr = instr.Split(' ').First();

                BuildDirectives(instr, argument, out _);

                int memberSize = m_structSize - currSize;

                TempStruct.m_StructMembers.Add(new StructMembers()
                {
                    m_Name = MemberName,
                    m_Size = memberSize,
                });
            }
        }
        m_getStructSize = false;

        TempStruct.m_Size = m_structSize;
        m_structs.Add(TempStruct);
    }
    string FindPath(string BasePath, int f)
    {
        string NewBasePath = "";

        for (int c = 0; c < BasePath.Length; c++)
        {
            char currChar = BasePath[c];

            if (currChar == '.')
            {
                c++;
                currChar = BasePath[c];
                if (currChar == '/')
                {
                    string FileName = Files[f].FullName;
                    int index = FileName.IndexOf(FileName.Split(Path.DirectorySeparatorChar).Last()) - 1;
                    FileName = FileName.Substring(0, index);

                    NewBasePath += FileName + Path.DirectorySeparatorChar;
                }
                else if (char.IsLetter(currChar))
                {
                    NewBasePath += '.';
                    NewBasePath += currChar;
                }
            }
            else
            {
                NewBasePath += currChar;
            }
        }

        return NewBasePath.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
    }
    string GetPath(string path)
    {
        string C_path = Path.GetFullPath(path);
        int index = C_path.IndexOf(C_path.Split(Path.DirectorySeparatorChar).Last()) - 1;
        return C_path.Substring(0, index);
    }

    void MakeLabel()
    {
        bool IsGlobal = m_Src[lineNumber].ToLower().Contains(".global");

        string name = m_Src[lineNumber].Replace(".global ", "").Trim(':');
        m_labels.ForEach(label =>
        {
            if (label.m_Name == name)
            {
                return;
            }
        });

        AddLine($"_REF_ LABEL {name},{IsGlobal} [{HexPC()}] {m_file}", Section.Symbols);
        AddLine($"_REF_ LABEL {name},{IsGlobal} [{HexPC()}] {m_file}");

        m_labels.Add(new Label()
        {
            m_Address = m_pc,
            m_Name = name
        });

        //AddLine($"_REF_ LABEL {name},{IsGlobal}");
        return;
    }
    bool IsLabel(string name, out Label result)
    {
        for (int i = 0; i < m_labels.Count; i++)
        {
            if (name == m_labels[i].m_Name)
            {
                result = m_labels[i];
                return true;
            }
        }
        result = null;
        return false;
    }
    bool IsVariabel(string name, out Variable result)
    {
        for (int i = 0; i < m_variables.Count; i++)
        {
            if (name == m_variables[i].m_Name)
            {
                result = m_variables[i];
                return true;
            }
        }
        result = null;
        return false;
    }

    private string HexPC() => $"0x{Convert.ToString(m_pc, 16)}";

    void ParseInstruction(string line)
    {
        List<string> InstrutionBytes = new List<string>();

        string instruction = line.Split(' ', 2)[0];
        string PreArguments = line.Split(' ', 2).Last();
        string[] Arguments = PreArguments.Split(", ");

        if (!Enum.TryParse(instruction.ToUpper(), out Instruction result))
        {


            return;
        }

        InstructionInfo instructionInfo = Instructions.instr[result];

        if (Arguments[0] != instruction)
        {
            if (instructionInfo.m_NumberOfOperands != Arguments.Length)
            {
                E_InvalidInstruction(instruction);
                return;
            }
        }

        AddLine($"_DEL_{line}");
        if (Arguments[0] != instruction)
        {
            ParseArgument(ref result, Arguments, ref InstrutionBytes);
        }
        else
        {
            ParseArgument(ref result, Arguments, ref InstrutionBytes, startingIndex: Arguments.Length);
        }

        AddLine($"_SET_ BPX {BPX_ref}");
        AddLine($"_SET_ SPX {SPX_ref}");

        ArgumentMode mode;
        switch (result)
        {
            case Instruction.POP:
                SPX_ref -= 1;
                break;
            case Instruction.POPW:
                SPX_ref -= 2;
                if (Arguments.Contains("BPX"))
                {
                    m_oldBPX = BPX_ref;
                    BPX_ref = m_BPX_stack.Pop();
                }
                break;
            case Instruction.POPT:
                SPX_ref -= 3;
                break;
            case Instruction.POPD:
                SPX_ref -= 4;
                break;
            case Instruction.CALL:
                SPX_ref += 2;
                break;
            case Instruction.RET:
                SPX_ref -= 2;
                string SizeAlignment = "byte";
                string expr = parseTerm(Arguments[0], ref SizeAlignment, out _, out mode);
                if (IsNumber(expr))
                {
                    int number = Convert.ToInt32(expr, 16);
                    SPX_ref -= number;
                }
                else
                {
                    SPX_ref -= m_oldBPX - BPX_ref;
                }

                break;
            case Instruction.RETZ:
                SPX_ref -= 2;
                break;

            case Instruction.PUSH:
                parseTerm(Arguments[0], "byte", out _, out mode);
                switch (mode)
                {
                    case ArgumentMode.immediate_byte:
                        SPX_ref += 1;
                        break;
                    case ArgumentMode.immediate_word:
                        SPX_ref += 2;
                        break;
                    case ArgumentMode.immediate_tbyte:
                        SPX_ref += 3;
                        break;
                    case ArgumentMode.immediate_dword:
                        SPX_ref += 4;
                        break;
                    case ArgumentMode.immediate_qword:
                        SPX_ref += 8;
                        break;
                    case ArgumentMode.immediate_float:
                        SPX_ref += 4;
                        break;
                    default:
                        break;
                }
                break;

            default:
                break;
        }

        AddLine(InstrutionBytes);

        AddLine($"_SET_ BPX {BPX_ref}");
        AddLine($"_SET_ SPX {SPX_ref}");
    }

    private void ParseArgument(ref Instruction instruction, string[] Arguments, ref List<string> InstrutionBytes, int startingIndex = 0, bool DoWrite = true)
    {
        string Argument1 = "FF";
        string Argument2 = "FF";
        List<string> argumrntBuffer = new List<string>();

        string SizeAlignment = "";
        Register reg_result;

        Register register1 = Register.none;
        Register register2 = Register.none;

        string segment;
        string offset;

        for (int i = startingIndex; i < Arguments.Length; i++)
        {
            string arg = Arguments[i];

            if (arg.Contains("byte", StringComparison.CurrentCultureIgnoreCase))
            {
                Arguments[i] = Arguments[i].Split(' ', 2).Last();
                if (SizeAlignment == "")
                {
                    SizeAlignment = "byte";
                }
                arg = Arguments[i];
                instruction = Instructions.instr[instruction].GetByteVersion();
            }
            else if (arg.Contains("word", StringComparison.CurrentCultureIgnoreCase))
            {
                Arguments[i] = Arguments[i].Split(' ', 2).Last();
                if (SizeAlignment == "")
                {
                    SizeAlignment = "word";
                }
                arg = Arguments[i];
                instruction = Instructions.instr[instruction].GetByteVersion();
            }

            string expr = parseTerm(arg, ref SizeAlignment, out bool DoSplit, out ArgumentMode mode);

            switch (mode)
            {
                case ArgumentMode.immediate_byte:
                case ArgumentMode.immediate_word:
                case ArgumentMode.immediate_tbyte:
                case ArgumentMode.immediate_dword:
                case ArgumentMode.immediate_qword:
                    if (DoSplit == true)
                    {
                        argumrntBuffer.AddRange(SplitHexString(expr));
                        m_pc += SplitHexString(expr).Length;
                        SetArgumentMode(ref Argument1, ref Argument2, mode);
                    }
                    else
                    {
                        SetArgumentMode(ref Argument1, ref Argument2, mode);
                        argumrntBuffer.Add(expr);
                    }
                    break;
                case ArgumentMode.register:
                case ArgumentMode.register_address:
                    if (IsRegister(expr, out reg_result))
                    {
                        if (i == 0)
                        {
                            RegisterInfo registerInfo = Registers.regs[reg_result];
                            if (registerInfo.m_size == 1)
                            {
                                SizeAlignment = "byte";
                                if (instruction == Instruction.PUSH)
                                {
                                    SPX_ref++;
                                }
                                reg_result = registerInfo.GetByteVersion();
                            }
                            else if (registerInfo.m_size == 2)
                            {
                                SizeAlignment = "word";
                                if (instruction == Instruction.PUSH)
                                {
                                    SPX_ref += 2;
                                    if (reg_result == Register.BPX)
                                    {
                                        m_BPX_stack.Push(BPX_ref);
                                    }
                                }
                            }
                            else if (registerInfo.m_size == 3)
                            {
                                SizeAlignment = "tbyte";
                                if (instruction == Instruction.PUSH)
                                {
                                    SPX_ref += 3;
                                }
                            }
                            else if (registerInfo.m_size == 4)
                            {
                                SizeAlignment = "dword";
                                if (instruction == Instruction.PUSH)
                                {
                                    SPX_ref += 4;
                                }
                            }
                            else if (registerInfo.m_size == 8)
                            {
                                SizeAlignment = "qword";
                                if (instruction == Instruction.PUSH)
                                {
                                    SPX_ref += 8;
                                }
                            }
                            else if (registerInfo.m_size == -1)
                            {
                                E_InvalidRegister(registerInfo);
                            }
                        }

                        if (register1 == Register.none)
                        {
                            register1 = reg_result;
                        }
                        else
                        {
                            register2 = reg_result;
                        }

                        switch (reg_result)
                        {
                            case Register.AL:
                                SetArgumentMode(ref Argument1, ref Argument2, ArgumentMode.register_AL);
                                break;
                            case Register.A:
                                SetArgumentMode(ref Argument1, ref Argument2, ArgumentMode.register_A);
                                break;
                            case Register.AX:
                                SetArgumentMode(ref Argument1, ref Argument2, ArgumentMode.register_AX);
                                break;
                            case Register.HL:
                                if(mode == ArgumentMode.register_address)
                                {
                                    SetArgumentMode(ref Argument1, ref Argument2, ArgumentMode.register_address_HL);
                                    break;
                                }
                                SetArgumentMode(ref Argument1, ref Argument2, ArgumentMode.register_HL);
                                break;
                            case Register.none:
                                break;
                            default:
                                m_pc++;
                                SetArgumentMode(ref Argument1, ref Argument2, mode);
                                argumrntBuffer.Add(GetRegisterHex(reg_result));
                                break;
                        }
                    }
                    break;
                case ArgumentMode.near_address:
                case ArgumentMode.address:
                case ArgumentMode.far_address:
                case ArgumentMode.long_address:
                case ArgumentMode.relative_address:
                    if (DoSplit)
                    {
                        argumrntBuffer.AddRange(SplitHexString(expr));
                        SetArgumentMode(ref Argument1, ref Argument2, mode);
                    }
                    else
                    {
                        SetArgumentMode(ref Argument1, ref Argument2, mode);
                        argumrntBuffer.Add(expr);
                    }
                    break;
                case ArgumentMode.immediate_float:
                    break;
                case ArgumentMode.segment_DS_register:
                case ArgumentMode.segment_address:
                    segment = expr.Split(':')[0];
                    offset = expr.Split(':')[1];

                    if (!IsRegister(segment, out register1))
                    {
                        throw new NotImplementedException();
                    }

                    if (!IsRegister(offset, out register2))
                    {
                        throw new NotImplementedException();
                    }

                    m_pc++;

                    if (mode != ArgumentMode.segment_DS_register)
                    {
                        m_pc++;
                        argumrntBuffer.Add(GetRegisterHex(register1));
                    }

                    argumrntBuffer.Add(GetRegisterHex(register2));

                    SetArgumentMode(ref Argument1, ref Argument2, mode);
                    break;
                case ArgumentMode.segment_DS_B:
                    segment = expr.Split(':')[0];
                    offset = expr.Split(':')[1];

                    if (!IsRegister(segment, out register1))
                    {
                        throw new NotImplementedException();
                    }

                    if (!IsRegister(offset, out register2))
                    {
                        throw new NotImplementedException();
                    }

                    SetArgumentMode(ref Argument1, ref Argument2, mode);
                    break;
                case ArgumentMode.segment_address_immediate:
                    segment = expr.Split(':')[0];
                    offset = expr.Split(':')[1];

                    if (!IsRegister(segment, out register1))
                    {
                        throw new NotImplementedException();
                    }

                    m_pc += 1;
                    m_pc += 2;

                    argumrntBuffer.Add(GetRegisterHex(register1));
                    argumrntBuffer.AddRange(SplitHexString(offset));

                    SetArgumentMode(ref Argument1, ref Argument2, mode);
                    break;
                case ArgumentMode.None:
                    break;
                case ArgumentMode.BP_Offset_Address:
                    argumrntBuffer.AddRange(SplitHexString(expr));
                    SetArgumentMode(ref Argument1, ref Argument2, mode);
                    break;
                default:
                    break;
            }
        }

        if (DoWrite)
        {
            switch (SizeAlignment)
            {
                case "byte":
                    instruction = Instructions.instr[instruction].GetByteVersion();
                    break;
                case "word":
                    instruction = Instructions.instr[instruction].GetWordVersion();
                    break;
                case "tbyte":
                    instruction = Instructions.instr[instruction].GetTbyteVersion();
                    break;
                case "dword":
                    instruction = Instructions.instr[instruction].GetDwordVersion();
                    break;
                default:
                    break;
            }

            switch (instruction)
            {
                case Instruction.MOV:
                case Instruction.MOVW:
                case Instruction.MOVT:
                case Instruction.MOVD:
                    if ((register2 == Register.SPX || register2 == Register.SP) && (register1 == Register.BPX || register1 == Register.BP))
                    {
                        BPX_ref = SPX_ref;
                    }
                    else if ((register1 == Register.SPX || register1 == Register.SP) && (register2 == Register.BPX || register2 == Register.BP))
                    {
                        SPX_ref = BPX_ref;
                    }
                    break;
            }

            InstrutionBytes.Add(Convert.ToString((byte)instruction, 16).PadLeft(2, '0'));
            m_pc++;
            InstrutionBytes.Add(Argument1);
            m_pc++;
            InstrutionBytes.Add(Argument2);
            m_pc++;
            InstrutionBytes.AddRange(argumrntBuffer);
        }
    }

    bool parseTerm(string arg, ref string SizeAlignment, out bool split, out ArgumentMode mode, out string expr)
    {
        expr = parseTerm(arg, ref SizeAlignment, out split, out mode);
        return expr != null;
    }
    string parseTerm(string arg, string SizeAlignment, out bool split, out ArgumentMode mode)
    {
        return parseTerm(arg, ref SizeAlignment, out split, out mode);
    }
    bool parseTerm(string arg, string SizeAlignment, out bool split, out ArgumentMode mode, out string expr)
    {
        expr = parseTerm(arg, ref SizeAlignment, out split, out mode);
        return expr != null;
    }
    string parseTerm(string arg, ref string SizeAlignment, out bool split, out ArgumentMode mode)
    {
        Register reg_result;
        string expr;

        if (arg.StartsWith("[BP ") && ContainsOperators(arg) && arg.EndsWith(']'))
        {
            string[] line = arg.Trim('[', ']').Split(' ', 3);
            char Operator = line[1][0];
            expr = parseTerm(line[2], "byte", out bool s, out ArgumentMode m);

            if (m > ArgumentMode.immediate_dword)
            {

            }

            mode = ArgumentMode.BP_Offset_Address;

            short number = Convert.ToInt16(expr, 16);

            number = (short)(Operator == '-' ? -number : number);

            split = true;
            string hexString = ToHexString(number);
            return hexString;
        }
        else if (Parse(arg, out mode, out expr) && expr.StartsWith("BINEXPR"))
        {
            split = false;
            return expr;
        }
        else if (arg.Contains("far", StringComparison.OrdinalIgnoreCase))
        {
            IsFar = true;

            string _arg = arg.Replace("far ", "", false, System.Globalization.CultureInfo.CurrentCulture);

            expr = parseTerm(_arg, ref SizeAlignment, out split, out mode);

            IsFar = false;

            return expr;
        }
        else if (arg.Contains("long", StringComparison.OrdinalIgnoreCase))
        {
            IsLong = true;

            string _arg = arg.Replace("long ", "", false, System.Globalization.CultureInfo.CurrentCulture);

            expr = parseTerm(_arg, ref SizeAlignment, out split, out mode);

            IsLong = false;

            return expr;
        }
        else if (arg.Contains("near", StringComparison.OrdinalIgnoreCase))
        {
            IsNear = true;

            string _arg = arg.Replace("near ", "", false, System.Globalization.CultureInfo.CurrentCulture);

            expr = parseTerm(_arg, ref SizeAlignment, out split, out mode);

            IsNear = false;

            return expr;
        }
        else if (arg.StartsWith('\"') && arg.EndsWith('\"'))
        {
            string data = arg.Trim('"');
            byte[] bytes = Encoding.ASCII.GetBytes(data);

            expr = "";

            int PaddingSize = 0;

            switch (SizeAlignment)
            {
                case "byte":
                    PaddingSize = 2;
                    break;
                case "word":
                    PaddingSize = 4;
                    break;
                case "tbyte":
                    PaddingSize = 6;
                    break;
                case "dword":
                    PaddingSize = 8;
                    break;
                case "qword":
                    PaddingSize = 16;
                    break;
            }

            for (int i = 0; i < bytes.Length; i++)
            {
                expr += Convert.ToString(bytes[i], 16).PadLeft(PaddingSize, '0');
            }
            mode = ArgumentMode.immediate_byte;
            split = true;
            return expr;
        }
        else if (Parse(arg, out mode, out expr))
        {
            AdjustImmediateValue(ref mode, SizeAlignment, ref expr);
            split = true;
            return expr;
        }
        else if (IsRegister(arg, out reg_result))
        {
            mode = ArgumentMode.register;
            split = false;
            return arg;
        }
        else if (arg.Contains('.'))
        {
            string result = ParseStructArgument(arg, out mode, out split);
            return result;
        }
        else if (arg.StartsWith('[') && arg.EndsWith("]") && arg.Contains(':'))
        {
            expr = arg.Trim('[', ']');
            string[] S_SegOff = expr.Split(':');
            string segment = S_SegOff[0];
            string offset = S_SegOff[1];

            split = true;

            return ParseSegmentAddressing(segment, offset, out mode);
        }
        else if (arg.StartsWith('%') && IsVariabel(arg.TrimStart('%'), out Variable variable))
        {
            split = true;
            mode = variable.m_Mode;
            return variable.m_Value;
        }
        else if (arg.StartsWith('@'))
        {
            split = false;

            if (IsFar)
            {
                mode = ArgumentMode.immediate_dword;
                string label = arg.Substring(1);
                return $"FL_{label}";
            }
            else if (IsNear)
            {
                mode = ArgumentMode.immediate_byte;
                string label = arg.Substring(1);
                return $"NL_{label}";
            }
            else if (IsLong)
            {
                mode = ArgumentMode.immediate_tbyte;
                string label = arg.Substring(1);
                return $"LL_{label}";
            }
            else
            {
                mode = ArgumentMode.immediate_word;
                string label = arg.Substring(1);
                return $"SL_{label}";
            }
        }
        else if (IsLetter(arg))
        {
            split = false;
            if (IsLong)
            {
                mode = ArgumentMode.long_address;
                return $"LL_{arg}";
            }
            else if (IsNear)
            {
                mode = ArgumentMode.near_address;
                return $"_NL_";
            }
            else if (IsFar)
            {
                mode = ArgumentMode.far_address;
                return $"FL_{arg}";
            }
            else
            {
                mode = ArgumentMode.address;
                return $"SL_{arg}";
            }
        }
        else if (arg == "$")
        {
            split = false;
            if (IsLong)
            {
                mode = ArgumentMode.long_address;
                return $"_LCA_";
            }
            else if (IsNear)
            {
                mode = ArgumentMode.near_address;
                return $"_NCA_";
            }
            else if (IsFar)
            {
                mode = ArgumentMode.far_address;
                return $"_FCA_";
            }
            else
            {
                mode = ArgumentMode.address;
                return $"_SCA_";
            }
        }
        else if (arg == "$$")
        {
            split = false;
            if (IsLong)
            {
                mode = ArgumentMode.long_address;
                return $"_LCS_";
            }
            else if (IsNear)
            {
                mode = ArgumentMode.near_address;
                return $"_NCS_";
            }
            else if (IsFar)
            {
                mode = ArgumentMode.far_address;
                return $"_FCS_";
            }
            else
            {
                mode = ArgumentMode.address;
                return $"_SCS_";
            }
        }
        else if (arg.StartsWith('[') && arg.EndsWith("]"))
        {
            arg = arg.TrimStart('[').TrimEnd(']');
            if (IsLong)
            {

            }
            else if (IsFar)
            {

            }
            else if (IsNear)
            {

            }
            else
            {
                if (Parse(arg, out mode, out expr))
                {
                    bool done = false;
                    while (done == false)
                    {
                        switch (mode)
                        {
                            case ArgumentMode.immediate_byte:
                                mode = ArgumentMode.relative_address;
                                expr = expr.PadLeft(2, '0');
                                split = true;
                                return expr;
                            case ArgumentMode.immediate_word:
                                expr = expr.PadLeft(4, '0');
                                done = true;
                                mode = ArgumentMode.address;
                                break;
                            case ArgumentMode.immediate_tbyte:
                                expr = expr.PadLeft(6, '0');
                                mode = ArgumentMode.immediate_word;
                                expr = expr.Substring(expr.Length - 4);
                                break;
                            case ArgumentMode.immediate_dword:
                                expr = expr.PadLeft(8, '0');
                                mode = ArgumentMode.immediate_word;
                                expr = expr.Substring(expr.Length - 4);
                                break;
                            case ArgumentMode.None:
                                throw new Exception("What the fuck");
                            default:
                                break;
                        }
                    }
                    split = true;
                    return expr;
                }
                else if (IsRegister(arg, out reg_result))
                {
                    mode = ArgumentMode.register_address;
                    split = true;
                    return arg;
                }
                else if (arg == "$")
                {
                    split = false;
                    if (IsLong)
                    {
                        mode = ArgumentMode.long_address;
                        return $"_LCA_";
                    }
                    else if (IsFar)
                    {
                        mode = ArgumentMode.far_address;
                        return $"_FCA_";
                    }
                    else if (IsNear)
                    {
                        mode = ArgumentMode.near_address;
                        return $"_NCA_";
                    }
                    else
                    {
                        mode = ArgumentMode.address;
                        return $"_SCA_";
                    }
                }
                else
                {
                    mode = ArgumentMode.address;
                    split = false;
                    return $"SL_{arg}";
                }
            }
        }
        split = false;
        mode = ArgumentMode.None;
        return null;
    }

    private string ParseSegmentAddressing(string segment, string offset, out ArgumentMode mode)
    {
        string SegmentOutput;
        string OffsetOutput;

        // check segment

        SegmentOutput = parseTerm(segment, "word", out _, out ArgumentMode SegmentMode);
        OffsetOutput = parseTerm(offset, "word", out _, out ArgumentMode OffsetMode);

        if (SegmentMode == ArgumentMode.register && OffsetMode == ArgumentMode.register)
        {
            if (SegmentOutput == "DS" && OffsetOutput == "B")
            {
                mode = ArgumentMode.segment_DS_B;
            }
            else if (SegmentOutput == "DS")
            {
                mode = ArgumentMode.segment_DS_register;
            }
            else
            {
                mode = ArgumentMode.segment_address;
            }
        }
        else if (SegmentMode == ArgumentMode.register && OffsetMode <= ArgumentMode.immediate_dword)
        {
            AdjustImmediateValue(ref OffsetMode, "word", ref OffsetOutput);

            mode = ArgumentMode.segment_address_immediate;
        }
        else
        {
            throw new NotImplementedException();
        }

        return $"{SegmentOutput}:{OffsetOutput}";
    }

    void SetArgumentMode(ref string arg1, ref string arg2, ArgumentMode mode)
    {
        switch (mode)
        {
            case ArgumentMode.immediate_byte:
            case ArgumentMode.immediate_word:
            case ArgumentMode.immediate_tbyte:
            case ArgumentMode.immediate_dword:
            case ArgumentMode.register:
            case ArgumentMode.register_address:
            case ArgumentMode.near_address:
            case ArgumentMode.address:
            case ArgumentMode.relative_address:
            case ArgumentMode.register_AL:
            case ArgumentMode.register_HL:
            case ArgumentMode.register_address_HL:

            case ArgumentMode.segment_address:
            case ArgumentMode.segment_address_immediate:
            case ArgumentMode.segment_DS_register:
            case ArgumentMode.segment_DS_B:
                break;

            case ArgumentMode.long_address:
            case ArgumentMode.immediate_float:
                if (CPUType < CPUType.BCG16)
                {
                    E_InvalidCPUFeature(CPUType.BCG16);
                }
                break;
            case ArgumentMode.far_address:
            case ArgumentMode.immediate_qword:
            case ArgumentMode.register_AX:
                if (CPUType < CPUType.BCG1684)
                {
                    E_InvalidCPUFeature(CPUType.BCG1684);
                }
                break;
            case ArgumentMode.register_A:
                if (CPUType < CPUType.BCG16)
                {
                    E_InvalidCPUFeature(CPUType.BCG16);
                }
                break;
            default:
                break;
        }

        if (arg1 == "FF")
        {
            arg1 = GetArgumentMode(mode);
        }
        else if (arg2 == "FF")
        {
            arg2 = GetArgumentMode(mode);
        }
    }
    bool IsRegister(string value, out Register register) => Enum.TryParse(value, true, out register);
    string ParseStructArgument(string expr, out ArgumentMode mode, out bool split)
    {
        char StartC = expr[0];
        char EndC = expr[^1];
        expr = expr.Trim('[', ']', '@');
        string[] layers = expr.Split('.');
        int layerIndex = 0;

        string LabelName = layers[0];
        layerIndex++;

        if (IsLabel(LabelName, out Label result))
        {
            if (layers[layerIndex] == "sizeof")
            {
                if (result.m_Struct.m_Size >= 0xff)
                {
                    mode = ArgumentMode.immediate_byte;
                }
                else
                {
                    mode = ArgumentMode.immediate_word;
                }

                if (StartC == '[')
                {
                    mode = ArgumentMode.address;
                }

                split = true;

                return $"{Convert.ToString(result.m_Struct.m_Size, 16)}";
            }
            else if (IsMember(layers[layerIndex], result.m_Struct, out StructMembers member))
            {
                if (layerIndex < layers.Length - 1)
                {
                    layerIndex++;
                    if (layers[layerIndex] == "sizeof")
                    {
                        if (result.m_Struct.m_Size >= 0xff)
                        {
                            mode = ArgumentMode.immediate_byte;
                        }
                        else
                        {
                            mode = ArgumentMode.immediate_word;
                        }

                        if (StartC == '[')
                        {
                            mode = ArgumentMode.address;
                        }

                        split = true;

                        return $"{Convert.ToString(member.m_Size, 16)}";
                    }
                }
                else if (StartC == '@')
                {
                    split = false;
                    mode = ArgumentMode.immediate_word;
                    return $"SLSM_{result.m_Name}.{layers[layerIndex]}";
                }
                else if (StartC == '[')
                {
                    split = false;
                    mode = ArgumentMode.address;
                    return $"SLSM_{result.m_Name}.{layers[layerIndex]}";
                }
                else
                {
                    split = false;
                    mode = ArgumentMode.address;
                    return $"SLSM_{result.m_Name}.{layers[layerIndex]}";
                }
            }
            else if (!IsMember(layers[layerIndex], result.m_Struct, out _))
            {
                if (StartC == '@')
                {
                    split = false;
                    mode = ArgumentMode.immediate_word;
                    return $"SLSM_{result.m_Name}.{layers[layerIndex]}";
                }
                else if (StartC == '[')
                {
                    split = false;
                    mode = ArgumentMode.address;
                    return $"SLSM_{result.m_Name}.{layers[layerIndex]}";
                }
                else
                {
                    split = false;
                    mode = ArgumentMode.address;
                    return $"SLSM_{result.m_Name}.{layers[layerIndex]}";
                }
            }
        }
        else
        {
            if (layers[layerIndex] == "sizeof")
            {
                mode = ArgumentMode.immediate_byte;

                if (StartC == '[')
                {
                    mode = ArgumentMode.address;
                }

                split = false;

                return $"_SSO_{layers[0]}";
            }
            else if (!IsMember(layers[layerIndex], null, out _))
            {
                if (StartC == '@')
                {
                    split = false;
                    mode = ArgumentMode.immediate_word;
                    return $"SLSM_{layers[0]}.{layers[layerIndex]}";
                }
                else if (StartC == '[')
                {
                    split = false;
                    mode = ArgumentMode.address;
                    return $"SLSM_{layers[0]}.{layers[layerIndex]}";
                }
                else
                {
                    split = false;
                    mode = ArgumentMode.address;
                    return $"SLSM_{layers[0]}.{layers[layerIndex]}";
                }
            }
        }

        split = false;
        mode = ArgumentMode.None;
        return "";
    }
    bool IsMember(string name, Struct _struct, out StructMembers result)
    {
        if (_struct == null)
        {
            result = default;
            return false;
        }

        for (int i = 0; i < _struct.m_StructMembers.Count; i++)
        {
            if (_struct.m_StructMembers[i].m_Name == name)
            {
                result = _struct.m_StructMembers[i];
                return true;
            }
        }
        result = new StructMembers();
        return false;
    }

    string GetRegisterHex(Register register) => Convert.ToString((byte)register, 16).PadLeft(2, '0');
    string GetArgumentMode(ArgumentMode mode) => Convert.ToString((byte)mode, 16).PadLeft(2, '0');
    void AdjustImmediateValue(ref ArgumentMode mode, string SizeAlignment, ref string expr)
    {
        bool Done = false;

        if (string.IsNullOrEmpty(SizeAlignment))
        {
            if (expr.Length >= 10)
            {
                SizeAlignment = "qword";
            }
            else if (expr.Length >= 6)
            {
                SizeAlignment = "dword";
            }
            else if (expr.Length >= 5)
            {
                SizeAlignment = "tbyte";
            }
            else if (expr.Length >= 4)
            {
                SizeAlignment = "word";
            }
            else
            {
                SizeAlignment = "byte";
            }
        }
        while (Done == false)
        {
            switch (mode)
            {
                case ArgumentMode.immediate_byte:
                    AdjustSize(SizeAlignment, ref expr, ref Done, ref mode, ArgumentMode.immediate_word, "byte", 2);
                    if (Done)
                    {
                        break;
                    }
                    else
                    {
                        continue;
                    }
                case ArgumentMode.immediate_word:
                    AdjustSize(SizeAlignment, ref expr, ref Done, ref mode, ArgumentMode.immediate_tbyte, "word", 4);
                    if (Done)
                    {
                        break;
                    }
                    else
                    {
                        continue;
                    }
                case ArgumentMode.immediate_tbyte:
                    AdjustSize(SizeAlignment, ref expr, ref Done, ref mode, ArgumentMode.immediate_dword, "tbyte", 6);
                    if (Done)
                    {
                        break;
                    }
                    else
                    {
                        continue;
                    }
                case ArgumentMode.immediate_dword:
                    AdjustSize(SizeAlignment, ref expr, ref Done, ref mode, ArgumentMode.immediate_qword, "dword", 8);
                    if (Done)
                    {
                        break;
                    }
                    else
                    {
                        continue;
                    }
                case ArgumentMode.immediate_qword:
                    if (!(CPUType < CPUType.BCG1884))
                    {
                        throw new Exception("What the fuck");
                    }
                    AdjustSize(SizeAlignment, ref expr, ref Done, ref mode, ArgumentMode.None, "qword", 16);
                    if (Done)
                    {
                        break;
                    }
                    else
                    {
                        throw new Exception("What the fuck");
                    }
                case ArgumentMode.immediate_float:
                    break;
                case ArgumentMode.None:
                    throw new Exception("What the fuck");
                default:
                    break;
            }
        }
    }
    void AdjustSize(string SizeAlignment, ref string expr, ref bool Done, ref ArgumentMode mode, ArgumentMode nextMode, string TestAlignment, int padsize)
    {
        if (SizeAlignment == TestAlignment)
        {
            if (expr.Length <= padsize)
            {
                expr = expr.PadLeft(padsize, '0');
            }
            else
            {
                expr = expr.Substring(expr.Length - padsize);
            }
            Done = true;
        }
        else
        {
            mode = nextMode;
        }
    }

    bool InBinExpr = false;
    bool Parse(string value, out ArgumentMode mode, out string expr)
    {
        if (value == null)
        {
            mode = ArgumentMode.None;
            expr = null;
            return false;
        }

        if (ContainsOperators(value) && InBinExpr == false && value.StartsWith('[') && value.EndsWith(']'))
        {
            string bin_expr = value.Trim('[', ']');
            string result = parse_expr(bin_expr, out ArgumentMode _mode);
            mode = _mode;
            switch (_mode)
            {
                case ArgumentMode.immediate_byte:
                case ArgumentMode.immediate_word:
                    mode = ArgumentMode.address;
                    break;
                case ArgumentMode.immediate_tbyte:
                    mode = ArgumentMode.long_address;
                    break;
                case ArgumentMode.immediate_dword:
                    mode = ArgumentMode.far_address;
                    break;
                case ArgumentMode.address:
                    break;
                case ArgumentMode.register:
                    break;
                case ArgumentMode.far_address:
                    break;
                case ArgumentMode.long_address:
                    break;
                case ArgumentMode.immediate_float:
                    break;
                case ArgumentMode.None:
                    break;
                default:
                    break;
            }
            expr = result;
            return true;
        }
        else if (ContainsOperators(value) && InBinExpr == false)
        {
            string result = parse_expr(value, out ArgumentMode _mode);
            mode = ArgumentMode.immediate_word;
            switch (_mode)
            {
                case ArgumentMode.immediate_byte:
                case ArgumentMode.immediate_word:
                    mode = _mode;
                    break;
                case ArgumentMode.immediate_tbyte:
                case ArgumentMode.immediate_dword:
                    mode = ArgumentMode.immediate_word;
                    break;
                case ArgumentMode.address:
                    break;
                case ArgumentMode.register:
                    break;
                case ArgumentMode.far_address:
                    break;
                case ArgumentMode.long_address:
                    break;
                case ArgumentMode.immediate_float:
                    break;
                case ArgumentMode.None:
                    break;
                default:
                    break;
            }
            expr = result;
            return true;
        }
        else if (value.StartsWith("0x"))
        {
            string HexTerm = value.Substring(2).Replace("_", "");
            int size = GetSize(ref HexTerm);

            expr = HexTerm;

            switch (size)
            {
                case 1:
                    mode = ArgumentMode.immediate_byte;
                    break;
                case 2:
                    mode = ArgumentMode.immediate_word;
                    break;
                case 3:
                    mode = ArgumentMode.immediate_tbyte;
                    break;
                case 4:
                    mode = ArgumentMode.immediate_dword;
                    break;
                case 5:
                    mode = ArgumentMode.immediate_qword;
                    break;
                default:
                    mode = ArgumentMode.None;
                    break;
            }

            return true;
        }
        else if (value.StartsWith("0b"))
        {
            string HexTerm = ToHexString(value.Substring(2).Replace("_", ""), 2);
            int size = GetSize(ref HexTerm);

            expr = HexTerm;

            switch (size)
            {
                case 1:
                    mode = ArgumentMode.immediate_byte;
                    break;
                case 2:
                    mode = ArgumentMode.immediate_word;
                    break;
                case 3:
                    mode = ArgumentMode.immediate_tbyte;
                    break;
                case 4:
                    mode = ArgumentMode.immediate_dword;
                    break;
                default:
                    mode = ArgumentMode.None;
                    break;
            }

            return true;
        }
        else if (char.IsDigit(value[0]))
        {
            string HexTerm = ToHexString(value.Replace("_", ""), 10);

            int size = GetSize(ref HexTerm);

            expr = HexTerm;

            switch (size)
            {
                case 1:
                    mode = ArgumentMode.immediate_byte;
                    break;
                case 2:
                    mode = ArgumentMode.immediate_word;
                    break;
                case 3:
                    mode = ArgumentMode.immediate_tbyte;
                    break;
                case 4:
                    mode = ArgumentMode.immediate_dword;
                    break;
                default:
                    mode = ArgumentMode.None;
                    break;
            }

            return true;
        }
        else if (value.StartsWith('\'') && value.EndsWith('\''))
        {
            value = value.Trim('\'');
            mode = ArgumentMode.immediate_byte;
            expr = Convert.ToString((byte)value[0], 16);
            return true;
        }
        else if (value.StartsWith('\"') && value.EndsWith('\"'))
        {
            string data = value.Trim('"');
            byte[] bytes = Encoding.ASCII.GetBytes(data);

            expr = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                expr += Convert.ToString(bytes[i], 16);
            }
            mode = ArgumentMode.immediate_byte;
            return true;
        }

        mode = ArgumentMode.None;
        expr = null;
        return false;
    }

    int GetSize(ref string HexNumber)
    {
        switch (HexNumber.Length)
        {
            case 1:
            case 2:
                HexNumber = HexNumber.PadLeft(2, '0');
                return 1;
            case 3:
            case 4:
                HexNumber = HexNumber.PadLeft(4, '0');
                return 2;
            case 5:
                HexNumber = HexNumber.PadLeft(5, '0');
                return 3;
            case 6:
                HexNumber = HexNumber.PadLeft(6, '0');
                return 4;
            default:
                HexNumber = HexNumber.Substring(HexNumber.Length - 6);
                return 4;
        }
    }

    string parse_expr(string value, out ArgumentMode _mode)
    {
        Stack<string> holdingStack = new Stack<string>();
        Stack<string> OutputStack = new Stack<string>();

        InBinExpr = true;

        string lastToken = null;

        string ident = "BINEXPR";

        string[] expr = value.Split(' ');
        string SizeAlignment = "word";

        _mode = ArgumentMode.None;

        List<string> tokens = new List<string>();
        for (int i = 0; i < value.Length;)
        {
            string buf = "";
            if (value[i] == '0' && i + 1 < value.Length && value[i + 1] == 'x')
            {
                i += 2;
                while (i < value.Length && IsHex(value[i].ToString()))
                {
                    buf += value[i];
                    i++;
                }

                tokens.Add(Convert.ToString(Convert.ToUInt32(buf, 16)));
            }
            else if (char.IsDigit(value[i]))
            {
                while (i < value.Length && char.IsDigit(value[i]))
                {
                    buf += value[i];
                    i++;
                }

                tokens.Add(buf);
            }
            else if (IsLetter(value[i].ToString()))
            {
                while (i < value.Length && IsLetter(value[i].ToString()))
                {
                    buf += value[i];
                    i++;
                }

                tokens.Add(buf);
            }
            else if (GetBinProc(value[i].ToString()) != -1)
            {
                tokens.Add(value[i].ToString());
                i++;
            }
            else if (value[i] == '@')
            {
                buf += value[i++];

                if (IsLetter(value[i].ToString()))
                {
                    while (i < value.Length && IsLetter(value[i].ToString()))
                    {
                        buf += value[i];
                        i++;
                    }

                    tokens.Add(buf);
                }
            }
            else if (value[i] == '(')
            {
                i++;
                tokens.Add("(");
            }
            else if (value[i] == ')')
            {
                i++;
                tokens.Add(")");
            }
            else if (value[i] == '$' && i + 1 < value.Length && value[i + 1] == '$')
            {
                i++;
                i++;
                tokens.Add("$$");
            }
            else if (value[i] == '$')
            {
                i++;
                tokens.Add("$");
            }
            else if (char.IsWhiteSpace(value[i]))
            {
                i++;
            }
        }

        expr = tokens.ToArray();

        for (int i = 0; i < expr.Length; i++)
        {
            ArgumentMode mode;
            string _exprOut;
            if (Parse(expr[i], out mode, out _exprOut))
            {
                lastToken = "lit";

                string result = "";

                switch (mode)
                {
                    case ArgumentMode.immediate_byte:
                        result = Convert.ToByte(_exprOut, 16).ToString();
                        break;
                    case ArgumentMode.immediate_word:
                        result = Convert.ToUInt16(_exprOut, 16).ToString();
                        break;
                    case ArgumentMode.immediate_tbyte:
                    case ArgumentMode.immediate_dword:
                        result = Convert.ToUInt32(_exprOut, 16).ToString();
                        break;
                }

                if (_mode == ArgumentMode.None)
                {
                    _mode = mode;
                }

                OutputStack.Push("I_" + result);
            }
            else if (parseTerm(expr[i], ref SizeAlignment, out bool DoSplit, out mode, out _exprOut))
            {
                lastToken = "lit";
                Register reg_result;
                switch (mode)
                {
                    case ArgumentMode.register:
                        if (IsRegister(_exprOut, out reg_result))
                        {
                            if (i == 0)
                            {
                                RegisterInfo registerInfo = Registers.regs[reg_result];
                                if (registerInfo.m_size == 1)
                                {
                                    SizeAlignment = "byte";
                                }
                                else if (registerInfo.m_size == 2)
                                {
                                    SizeAlignment = "word";
                                }
                                else if (registerInfo.m_size == 3)
                                {
                                    SizeAlignment = "tbyte";
                                }
                                else if (registerInfo.m_size == 4)
                                {
                                    SizeAlignment = "dword";
                                }
                                else if (registerInfo.m_size == 8)
                                {
                                    SizeAlignment = "qword";
                                }
                                else if (registerInfo.m_size == -1)
                                {
                                    throw new NotImplementedException();
                                }
                            }

                            OutputStack.Push("R_" + _exprOut);
                            ident = "BINEXPRWR";
                        }

                        if (_mode != ArgumentMode.register || _mode != ArgumentMode.register_address)
                        {
                            _mode = ArgumentMode.register;
                        }
                        break;
                    case ArgumentMode.near_address:
                    case ArgumentMode.address:
                    case ArgumentMode.long_address:
                    case ArgumentMode.far_address:
                        if (_mode == ArgumentMode.long_address)
                        {
                            ident = "BINEXPRL";
                        }
                        else if (_mode == ArgumentMode.far_address)
                        {
                            ident = "BINEXPRF";
                        }
                        else if (_mode == ArgumentMode.near_address)
                        {
                            ident = "BINEXPRN";
                        }
                        if (_mode != ArgumentMode.register)
                        {
                            _mode = mode;
                        }
                        if (DoSplit == false)
                        {
                            OutputStack.Push(_exprOut);
                        }
                        break;
                    case ArgumentMode.register_address:
                        break;
                    case ArgumentMode.segment_address:
                        break;
                    case ArgumentMode.segment_address_immediate:
                        break;
                    case ArgumentMode.segment_DS_register:
                        break;
                    case ArgumentMode.None:
                        break;
                    case ArgumentMode.immediate_byte:
                    case ArgumentMode.immediate_word:
                    case ArgumentMode.immediate_tbyte:
                    case ArgumentMode.immediate_dword:
                    case ArgumentMode.immediate_qword:
                        if (!DoSplit)
                        {
                            OutputStack.Push(_exprOut);
                        }
                        break;
                    default:
                        break;
                }
            }
            else if (expr[i] == "(")
            {
                lastToken = "(";
                holdingStack.Push("(");
            }
            else if (expr[i] == ")")
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
                string newToken = expr[i];

                if (newToken == "-")
                {
                    if (lastToken == null || (lastToken != "lit" && lastToken != ")"))
                    {
                        newToken = "n";
                    }
                }
                else if (newToken == "+")
                {
                    if (lastToken == null || (lastToken != "lit" && lastToken != ")"))
                    {
                        newToken = "p";
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
            else if (string.IsNullOrEmpty(expr[i]))
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

        InBinExpr = false;

        string output = "";

        while (OutputStack.TryPeek(out _))
        {
            output += OutputStack.Pop() + " ";
        }

        output = output.Trim();

        return output;
    }
    bool ContainsBinProc(string expr, out int binproc)
    {
        for (int i = 0; i < expr.Length; i++)
        {
            if (ContainsOperators(expr))
            {
                binproc = GetBinProc(expr);
                return true;
            }
        }
        binproc = -1;
        return false;
    }
    int GetBinProc(string c)
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
    public void AddLine(string line)
    {
        AddLine(line, m_section);
    }
    public void AddLine(IEnumerable<string> line)
    {
        if (m_WriteOut == false)
        {
            return;
        }

        for (int i = 0; i < line.Count(); i++)
        {
            AddLine(line.ElementAt(i), m_section);
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
                m_output.Add(line);
                break;
            case Section.Symbols:
                m_output_symbols.Add(line);
                break;
            case Section.Section:
                m_output_section.Add(line);
                break;
            case Section.Structs:
                m_output_structs.Add(line);
                break;
            case Section.IncludedFiles:
                m_output_includedFiles.Add(line);
                break;
            default:
                break;
        }
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
    Structs,
    IncludedFiles,
}
