using System;
using System.Collections.Generic;
using System.Text;
using static HexLibrary.SplitFunctions;
using static HexLibrary.HexConverter;
using static HexLibrary.StringFunctions;
using System.Data.Common;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO;

namespace AssemblerBCG
{
    public class Assembler : AssemblerInstrutions
    {
        public void Start(string src, string file, bool isLast)
        {

            Console.WriteLine($"Assembling {file.Split(Path.DirectorySeparatorChar).Last()}");
            if (m_doneFiles.Contains(file))
            {
                return;
            }

            m_WriteOut = true;

            Build(src);

            for (int i = 0; i < m_Label.Count; i++)
            {
                AddLine($"_REF_ LABEL {m_Label[i].m_Name},{m_Label[i].m_IsGlobal}.{m_Label[i].m_IsLocal} {m_Label[i].m_file}", Section.Symbols);
            }

            m_Output.Insert(0, "TEXT HEADER");
            m_OutputSymbols.Insert(0, "SYMBOLS HEADER");
            m_OutputSection.Insert(0, "SECTION HEADER");
            m_OutputIncludedFiles.Insert(0, "INCLUDE HEADER");



            m_OutputSymbols.Add(Environment.NewLine);
            m_OutputSection.Add(Environment.NewLine);
            m_OutputIncludedFiles.Add(Environment.NewLine);

            m_Output.InsertRange(0, m_OutputSymbols);
            m_Output.InsertRange(0, m_OutputSection);
            m_Output.InsertRange(0, m_OutputIncludedFiles);
        }

        public void Build(string src)
        {
            src = Regex.Replace(src, @";[^\r\n]*(\r?\n)", Environment.NewLine);
            src = Regex.Replace(src, @" +", " ");
            m_src = src.Split(Environment.NewLine);

            PreAssemble();

            Assemble();
        }

        public void PreAssemble()
        {
            for (m_index = 0; m_index < m_src.Length; m_index++)
            {
                if (m_src[m_index].StartsWith('$'))
                {
                    string[] line = m_src[m_index].Split(' ');
                    string name = line[0].TrimStart('$');
                    if (line[1] == "=")
                    {
                        SizeAlignment alignment = SizeAlignment._word;
                        if (!ParseTerm(line[2], ref alignment, out ArgumentModeOld mode, out string[] value))
                        {
                            Console.WriteLine($"Error: 00100 {m_file}:{Linenumber}");
                            Environment.Exit(1);
                        }

                        m_Variables.Add(new Variable()
                        {
                            m_Mode = mode,
                            m_Name = name,
                            m_Value = value
                        });
                    }
                }
            }
        }

        public void Assemble()
        {
            for (m_index = 0; m_index < m_src.Length; m_index++)
            {
                m_src[m_index] = Regex.Replace(m_src[m_index], @";[^.]*", Environment.NewLine);
                m_src[m_index] = Regex.Replace(m_src[m_index], @" +", " ");

                AddLine("_NEWLINE_");

                m_src[m_index] = m_src[m_index].Trim('\r', '\n');
                m_src[m_index] = m_src[m_index].TrimStart(' ');
                m_src[m_index] = m_src[m_index].Trim();

                if (string.IsNullOrEmpty(m_src[m_index]))
                {
                    continue;
                }


                if (m_src[m_index].StartsWith('.'))
                {
                    BuildDirectives();
                }
                else if (m_src[m_index].StartsWith('$'))
                {
                    continue;
                }
                else if (m_src[m_index].EndsWith(':'))
                {
                    MakeLabel();
                    continue;
                }
                else
                {
                    AddLine($"_DEL_ {m_src[m_index]}");
                    AssembleInstrution();
                }
            }
        }

        void MakeLabel()
        {
            string name = m_src[m_index].TrimEnd(':');

            AddLine($"_REF_ LABEL {name} {m_file}");
            if (LabelExists(name, out Label _, out int _))
            {
            }
            else
            {
                m_Label.Add(new Label()
                {
                    m_Name = name,
                    m_file = m_file
                });
            }
        }

        #region Assembler directives

        public void BuildDirectives()
        {
            string[] strings = m_src[m_index].Split(' ', 2);

            string instr = strings[0].TrimStart('.');
            string[] arguments = strings.Last().Split(',');
            BuildDirectives(instr, arguments);
        }

        void BuildDirectives(string instr, string[] arguments)
        {
            for (int i = 0; i < arguments.Length; i++)
            {
                arguments[i] = arguments[i].Trim();
            }

            if (instr.Contains("org", IgnoreCasing))
            {
                directiveOrg(arguments[0]);
            }
            else if (instr.Contains("newfile", IgnoreCasing))
            {
                directiveNewfile(arguments[0]);
            }
            else if (instr.Contains("section", IgnoreCasing))
            {
                directiveSection(arguments[0]);
                directiveSetSection(arguments[0]);
            }
            else if (instr.Contains("SETCPU", IgnoreCasing))
            {
                directiveSetCPU(arguments[0]);
            }
            else if (instr.Contains("includeil", IgnoreCasing))
            {
                directiveIncludeInlined(arguments[0]);
            }
            else if (instr.Contains("include", IgnoreCasing))
            {
                directiveInclude(arguments[0]);
            }
            else if (instr.Contains("global", IgnoreCasing))
            {
                string name = arguments[0];
                if (LabelExists(name, out Label _, out int index))
                {
                    m_Label[index].m_IsGlobal = true;
                }
                else
                {
                    m_Label.Add(new Label()
                    {
                        m_Name = name,
                        m_file = m_file
                    });
                }
            }
            else if (instr.Contains("extern", IgnoreCasing))
            {
                m_Label.Add(new Label()
                {
                    m_Name = arguments[0],
                    m_file = m_file
                });
            }
            else if (instr.Contains("local", IgnoreCasing))
            {
                string name = arguments[0];
                if (LabelExists(name, out Label _, out int index))
                {
                    m_Label[index].m_IsLocal = true;
                }
                else
                {
                    m_Label.Add(new Label()
                    {
                        m_Name = name,
                        m_file = m_file
                    });
                }
            }
            else if (instr.StartsWith("struct", IgnoreCasing))
            {
                arguments = arguments[0].Split(' ');
                string structName = arguments[0];
                if (arguments.Length > 1)
                {
                    string lableName = arguments[1].TrimEnd(':');
                    DeclareStruct(structName, lableName);
                }
                else
                {
                    MakeStruct(structName);
                }
            }
            else if (instr.StartsWith("times", IgnoreCasing))
            {
                arguments = arguments[0].Split(' ',2 );
                string times = arguments[0];
                string directive = arguments[1];
                directiveTimes(times, directive);
            }
            else
            {
                SizeAlignment sizeAlignment;
                switch (instr)
                {
                    case "db":
                    case "byte":
                        for (int i = 0; i < arguments.Length; i++)
                        {
                            sizeAlignment = SizeAlignment._byte;
                            if (ParseTerm(arguments[i], ref sizeAlignment, out ArgumentModeOld mode, out string[] result))
                            {
                                if (mode != ArgumentModeOld.immediate_byte)
                                {
                                    Console.WriteLine($"Error: {arguments[i]} is not a byte value");
                                    m_WriteOut = false;
                                }
                                for (int d = 0; d < result.Length; d++)
                                {
                                    AddLine(result[d]);
                                }
                            }
                        }
                        break;
                    case "dw":
                    case "word":
                        for (int i = 0; i < arguments.Length; i++)
                        {
                            sizeAlignment = SizeAlignment._word;
                            if (ParseTerm(arguments[i], ref sizeAlignment, out ArgumentModeOld mode, out string[] result))
                            {
                                if (mode != ArgumentModeOld.immediate_word)
                                {
                                    Console.WriteLine($"Error: {arguments[i]} is not a word value");
                                    m_WriteOut = false;
                                }
                                for (int d = 0; d < result.Length; d++)
                                {
                                    AddLine(result[d]);
                                }
                            }
                        }
                        break;
                    case "dt":
                    case "tbyte":
                        for (int i = 0; i < arguments.Length; i++)
                        {
                            sizeAlignment = SizeAlignment._tbyte;
                            if (ParseTerm(arguments[i], ref sizeAlignment, out ArgumentModeOld mode, out string[] result))
                            {
                                if (mode != ArgumentModeOld.immediate_tbyte)
                                {
                                    Console.WriteLine($"Error: {arguments[i]} is not a tbyte value");
                                    m_WriteOut = false;
                                }
                                for (int d = 0; d < result.Length; d++)
                                {
                                    AddLine(result[d]);
                                }
                            }
                        }
                        break;
                    case "dd":
                    case "dword":
                        for (int i = 0; i < arguments.Length; i++)
                        {
                            sizeAlignment = SizeAlignment._dword;
                            if (ParseTerm(arguments[i], ref sizeAlignment, out ArgumentModeOld mode, out string[] result))
                            {
                                if (mode != ArgumentModeOld.immediate_dword)
                                {
                                    Console.WriteLine($"Error: {arguments[i]} is not a dword value");
                                    m_WriteOut = false;
                                }
                                for (int d = 0; d < result.Length; d++)
                                {
                                    AddLine(result[d]);
                                }
                            }
                        }
                        break;
                    case "res":
                        string res = ParseTerm(arguments[0], SizeAlignment._word, out _);
                        AddLine($"_RES_ {res}");
                        break;
                    case "resb":
                        AddLine($"_RES_ 1");
                        break;
                    case "resw":
                        AddLine($"_RES_ 2");
                        break;
                    case "rest":
                        AddLine($"_RES_ 3");
                        break;
                    case "resd":
                        AddLine($"_RES_ 4");
                        break;
                    default:
                        Console.WriteLine($"Error: Undefined directives {m_src[m_index]}   {m_file}:{Linenumber}");
                        break;
                }
            }
        }

        void directiveTimes(string times, string directive)
        {
            string timesResult = ParseTerm(times, SizeAlignment._word, out _);
            string instr = directive.Split(' ', 2)[0].Trim('.');
            string argument = directive.Split(' ', 2)[1];

            string result = "";
            int size = 0;
            switch (instr)
            {
                case "db":
                case "byte":
                    size = 1;
                    result = ParseTerm(argument, SizeAlignment._byte, out _);
                    break;
                case "dw":
                case "word":
                    size = 2;
                    result = ParseTerm(argument, SizeAlignment._word, out _);
                    break;
                case "dt":
                case "tbyte":
                    size = 3;
                    result = ParseTerm(argument, SizeAlignment._tbyte, out _);
                    break;
                case "dd":
                case "dword":
                    size = 4;
                    result = ParseTerm(argument, SizeAlignment._dword, out _);
                    break;
                default:
                    Console.WriteLine($"Error: in Times error msg WIP");
                    Console.WriteLine($"{m_file}:{Linenumber}");
                    m_WriteOut = false;
                    break;
            }

            AddLine($"_TIMES_ {timesResult},{result}.{size}");
        }

        void directiveIncludeInlined(string _file)
        {
            string FileName = _file.Trim('\"').Split('\\', '/').Last();
            int FileIndex = m_Files.FindIndex(file =>
            {
                return file.Name == FileName;
            });
            string srcFile = m_Files[FileIndex].FullName;
            AddLine($"INCINIL {srcFile}", Section.IncludedFiles);

            string[] savem_src = new string[m_src.Length];
            m_src.CopyTo(savem_src, 0);
            int index = m_index;
            string file = m_file;
            Section section = m_Section;

            string src = File.ReadAllText($"{srcFile}");
            m_file = srcFile;
            AddLine($"_FILE_ \"{m_file}\"");

            Build(src);

            m_doneFiles.Add(m_file);

            m_Section = section;
            m_file = file;
            m_src = savem_src;
            m_index = index;
        }

        void directiveInclude(string file)
        {

        }
        void directiveSetSection(string section)
        {
            AddLine($"_{section} {m_file}", Section.Section);
            setSection(section);
        }
        private void setSection(string section)
        {
            if (section == m_TextSection)
            {
                m_Section = Section.Text;
            }
            else if (section == m_DataSection)
            {
                m_Section = Section.Data;
            }
            else if (section == m_BSSSection)
            {
                m_Section = Section.Bss;
            }
            else if (section == m_RDataSection)
            {
                m_Section = Section.Rdata;
            }
        }

        void directiveSetCPU(string CPU)
        {
            CPU = CPU.Trim('\"');
            if (Enum.TryParse(CPU, true, out CPUType result))
            {
                m_CPUType = result;
            }
            else
            {
                Console.WriteLine($"00000: {CPU} CPU type is defind {m_file}:{Linenumber}");
                m_WriteOut = false;
            }

            Register[] registers = Registers.m_Regs.Keys.ToArray();

            for (int i = 0; i < registers.Length; i++)
            {
                Registers.m_Regs[registers[i]].m_Size = Registers.m_Regs[registers[i]].GetSize();
            }
        }
        void directiveSection(string section)
        {
            AddLine($"_SECTION_ {section}");

            if (Enum.TryParse(section, true, out Section result))
            {
                m_Section = result;
            }
            else
            {
                m_Section = Section.Text;
            }
        }

        void directiveNewfile(string file)
        {
            m_file = file;
            AddLine($"_FILE_ \"{m_file}\"");
            m_doneFiles.Add(file);
        }

        void directiveOrg(string number)
        {
            if (ParseTerm(number, SizeAlignment._word, out ArgumentModeOld mode, out string[] data))
            {
                string address = "";
                for (int i = 0; i < data.Length; i++)
                {
                    address += data[i];
                }

                AddLine($"_OFF_ {address}");
            }
        }

        #endregion
    }
}
