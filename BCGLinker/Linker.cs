using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.ExceptionServices;
using System.Text;
using static HexLibrary.HexConverter;
using static HexLibrary.StringFunctions;

namespace BCGLinker
{
    public class Linker
    {
        string[] m_src;

        public List<string> m_Output = new List<string>();
        public List<byte> m_OutputBin = new List<byte>();
        public List<string> m_InlinedFiles = new List<string>();
        public List<Label> m_Labels = new List<Label>();
        public List<Section> m_Sections = new List<Section>();
        int m_i;

        bool m_hasAllLabel;

        int m_currentSectionIndex = 0;

        string m_currFile = "";
        int m_pc = 0;

        public void BuildLabels(string src)
        {
            m_i = 0;
            m_src = src.Split(Environment.NewLine);
            m_hasAllLabel = false;
            m_pc = 0;

            buildText();
        }

        public void BuildSrc(string src)
        {
            LinkerSettings.Debug = true;
            m_Output.Clear();
            m_i = 0;
            m_pc = 0;
            m_src = src.Split(Environment.NewLine);
            m_currentSectionIndex = 0;

            buildText();

            for (int i = 0; i < m_Sections.Count; i++)
            {
                m_Sections[i].m_PCOffset = m_Sections[i].m_Start;
            }

            m_Output.Clear();
            m_hasAllLabel = true;
            m_i = 0;
            m_src = src.Split(Environment.NewLine);
            m_pc = 0;

            buildText();

            List<string> tempList = new List<string>();

            string[] m_Output_ARRAY = m_Output.ToArray();
            m_Output_ARRAY = SortHexStrings(m_Output_ARRAY);

            switch (LinkerSettings.OutputFormat)
            {
                case OutputFormats.lib:
                    DebugWriter.Writeline("Output Format lib");
                    tempList.Add("library ACL0.1\0");

                    tempList.Add("_TEXT SECTION".PadRight(16, '\0'));
                    tempList.AddRange(m_Output_ARRAY);

                    tempList.Add("_SYMBOLS".PadRight(16, '\0'));
                    break;
                case OutputFormats.fbin:
                case OutputFormats.bin:
                    DebugWriter.Writeline("Output Format Bin");
                    // tempList.Add("_TEXT SECTION".PadRight(16, '\0'));
                    tempList.AddRange(m_Output_ARRAY);
                    break;
                default:
                    break;
            }
            //tempList.AddRange(m_Output_BSS_ARRAY);

            File.WriteAllLines("./PostLinker.txt", tempList.ToArray());

            string[] m_pre_Output = tempList.ToArray();
            m_Output.Clear();

            List<byte> OutputBytes = new List<byte>();

            List<string> UsedAddress = new List<string>();
            for (int i = 0; i < m_pre_Output.Length; i++)
            {
                string line = m_pre_Output[i];

                if (line.StartsWith('_'))
                {
                    m_Output.Add(line);
                    continue;
                }
                if (line.Contains("_DEL_"))
                {
                    continue;
                }


                if (!line.Contains(":\t"))
                {
                     continue;
                }
                string Full_address = m_pre_Output[i].Split(":\t")[0];
                string address = Full_address;
                string code = m_pre_Output[i].Split(":\t")[1];

                if (code.StartsWith("INCIN "))
                {
                    continue;
                }

                if (UsedAddress.Count > 0)
                {
                    int LastAddress = Convert.ToInt32(UsedAddress.Last(), 16);
                    int currAddress = Convert.ToInt32(Full_address, 16);
                    if (LastAddress + 1 < currAddress)
                    {
                        int count = currAddress - (LastAddress + 1);
                        for (int a = 0; a < count; a++)
                        {
                            OutputBytes.Add(0);
                        }
                    }
                }

                byte number = Convert.ToByte(code, 16);

                if (UsedAddress.Contains(Full_address))
                {
                    OutputBytes[^1] = number;
                    //m_Output[^1] = "  " + address + ":\t" + code + " DEBUG CODE";
                    continue;
                }
                else
                {
                    OutputBytes.Add(number);
                    //write_out(code + " DEBUG CODE");
                }
                UsedAddress.Add(Full_address);
            }

            for (int i = 0; i < OutputBytes.Count; i++)
            {
                m_OutputBin.Add(OutputBytes[i]);
            }
        }

        public void BuildSectionInclude(string src)
        {
            m_i = 0;
            m_src = src.Split(Environment.NewLine);

            buildInclude();
        }
        public void BuildSectionSymbols(string src)
        {
            m_i = 0;
            m_src = src.Split(Environment.NewLine);

            buildSymbols();
        }

        int m_instrOffset = 0;
        int m_lineNumber = 0;
        void buildText()
        {
            for (m_i = 0; m_i < m_src.Length; m_i++)
            {
                if (m_src[m_i] == "TEXT HEADER")
                {
                    m_i++;
                    for (; m_i < m_src.Length; m_i++)
                    {
                        string line = m_src[m_i].Trim('\n', '\r');
                        if (string.IsNullOrEmpty(line))
                        {
                            continue;
                        }

                        parseLine(line, out bool Exit);
                        if (Exit)
                        {
                            break;
                        }
                    }
                    break;
                }
            }
        }

        private void parseLine(string line, out bool Exit)
        {

            if (!IsHex(line))
            {
                if (LinkerSettings.Debug == true)
                {
                    if (m_hasAllLabel)
                    {
                        switch (line)
                        {
                            case "_NEWLINE_":
                                break;
                            default:
                            m_Output.Add($"{getPCHex(m_Sections[m_currentSectionIndex])}_DEL_:\t{line}");
                                break;
                        }
                    }
                }
            }

            Exit = false;
            if (line == "_NEWLINE_")
            {
                m_instrOffset = 0;
                m_lineNumber++;
                return;
            }
            else if (line.StartsWith("_FILE_"))
            {
                string file = line.Split(' ').Last();
                m_currFile = file.Trim('"');
                m_lineNumber = 0;
                return;
            }
            else if (line.StartsWith("_REF_"))
            {
                if (m_hasAllLabel)
                {
                    return;
                }
                string[] s_line = m_src[m_i].Split(' ');
                string name;
                string file;
                string[] Ident;
                if (s_line[0] == "_REF_")
                {
                    switch (s_line[1])
                    {
                        case "LABEL":
                            Ident = s_line[2].Split(',');
                            name = Ident[0];
                            file = s_line[3];

                            int index = m_Labels.FindIndex(label =>
                            {
                                if (label.m_Name == name)
                                {
                                    return true;
                                }
                                return false;
                            });

                            m_Labels[index].m_Address = m_pc;

                            m_Labels[index].m_Section = m_Sections[m_currentSectionIndex];

                            break;
                        default:
                            break;
                    }
                }
            }
            else if (line.StartsWith("_DEL_"))
            {
                m_instrOffset = 0;
            }
            else if (line.StartsWith("_NEWARG_"))
            {
                return;
            }
            else if (line.StartsWith("_SECTION_"))
            {
                string sectionName = line.Split(' ').Last();
                m_Sections[m_currentSectionIndex].m_PCOffset = m_pc;
                int index = m_Sections.FindIndex(section =>
                {
                    if (section.m_Name == sectionName)
                    {
                        return true;
                    }
                    return false;
                });

                if (index == -1)
                {
                    Console.WriteLine($"Linker error: Section {sectionName} is not defined in the linker script");
                    return;
                }

                m_currentSectionIndex = index;
                if (m_Sections[index].m_PCOffset == 0)
                {
                    m_Sections[index].m_PCOffset = m_Sections[index].m_Start;
                }
                m_pc = m_Sections[index].m_PCOffset;
            }
            else if (line.StartsWith("_OFF_"))
            {
                int offset = Convert.ToInt32(line.Replace("_OFF_ ", ""), 16);
                m_pc = offset;
            }
            else if (line.StartsWith("_RES_"))
            {
                string Lineoffset = line.Replace("_RES_ ", "");
                string bin_type = Lineoffset.Split(' ', 2).First();
                string expr = Lineoffset.Split(" ", 2).Last();
                string result = gen_expr(expr, bin_type);

                int offset = Convert.ToInt32(result, 16);
                m_pc += offset;
            }
            else if (line.EndsWith("HEADER"))
            {
                if (line.StartsWith("SECTION"))
                {
                    Exit = true;
                    return;
                }
                else if (line.StartsWith("SYMBOLS"))
                {
                    Exit = true;
                    return;
                }
            }
            else if (line.StartsWith("BINEXPR"))
            {
                string bin_type = line.Split(' ', 2).First();
                string expr = line.Split(" ", 2).Last();
                string result = gen_expr(expr, bin_type);

                string[] hex = SplitHexString(result);

                for (int i = 0; i < hex.Length; i++)
                {
                    addString(hex[i]);
                }
            }
            else if (gen_term(line, out string[] expr))
            {
                for (int i = 0; i < expr.Length; i++)
                {
                    addString(expr[i]);
                }

                if (IsHex(line))
                {
                    return;
                }
            }
            else if (line.StartsWith("_TIMES_"))
            {
                string Lineexpr = line.Split(" ", 2).Last();
                string NumberExpr = Lineexpr.Split(',')[0];
                string Instr = Lineexpr.Split(",")[1].Split('.')[0];

                string result;

                if (NumberExpr.StartsWith("BINEXPR"))
                {
                    string bin_type = Lineexpr.Split(' ').First();
                    result = gen_expr(NumberExpr.Split(' ', 2)[1], bin_type);

                }
                else if (gen_term(NumberExpr, out string[] outputExpr))
                {
                    result = "";

                    for (int i = 0; i < outputExpr.Length; i++)
                    {
                        result += outputExpr[i];
                    }
                }
                else
                {
                    result = "";
                }

                int times = Convert.ToInt32(result, 16);
                int size = int.Parse(line.Split('.').Last());

                string[] ResultStr = SplitHexString(Instr, size);

                for (int a = 0; a < times; a++)
                {
                    for (int i = 0; i < ResultStr.Length; i++)
                    {
                        addString(ResultStr[i]);
                    }
                }
            }
            else
            {
                addString(line);
                return;
            }
        }

        bool gen_term(string line, out string[] expr)
        {
            if (line.StartsWith("REL_"))
            {
                expr = doAddress(line.Replace("REL_", ""), true);
                return true;
            }
            else if (line.StartsWith("_N") || line.StartsWith("_S") || line.StartsWith("_L") || line.StartsWith("_F"))
            {
                expr = doAddress(line);
                return true;
            }
            else if (line.StartsWith("i_"))
            {
                string data = line.Replace("I_", "", StringComparison.OrdinalIgnoreCase);
                expr = SplitHexString(data);
                return true;
            }
            else if (IsHex(line))
            {
                expr = SplitHexString(line);
                return true;
            }
            expr = null;
            return false;
        }

        string[] doAddress(string line, bool rel = false)
        {
            string[] Outputaddress;
            int address = 0;

            string Mask = line.Split(',').Last();
            line = line.Split(',').First();

            if (m_hasAllLabel)
            {
                if (line.StartsWith("_SCS_") || line.StartsWith("_LCS_") || line.StartsWith("_FCS_"))
                {
                    address = (m_pc - m_instrOffset) % 0x200;
                }
                else if (line.StartsWith("_SCA_") || line.StartsWith("_LCA_") || line.StartsWith("_FCA_"))
                {
                    address = m_pc - m_instrOffset;
                }
                else if (line.StartsWith("_SL_") || line.StartsWith("_LL_") || line.StartsWith("_FL_"))
                {
                    string name = line.Replace("_FL_", "").Replace("_LL_", "").Replace("_SL_", "");
                    if (!getLabel(name, out Label label))
                    {
                        Console.WriteLine($"LINKER ERROR: {name} not found as symbol");
                        Console.WriteLine($"{m_currFile}:{m_lineNumber + 1}");
                        Environment.Exit(-1);
                    }

                    if (label.m_IsGlobal)
                    {
                        address = label.m_Address;
                    }
                    else if (label.m_IsLocal)
                    {
                        if (label.m_File == m_currFile)
                        {
                            address = label.m_Address;
                        }
                    }
                    else
                    {
                        if (m_hasAllLabel)
                        {
                            string inputFileName = LinkerSettings.InputFile.Split(Path.DirectorySeparatorChar).Last().Split('.')[0];
                            string labelFileName = label.m_File.Split(Path.DirectorySeparatorChar).Last().Split('.')[0];
                            if (labelFileName.ToLower() == inputFileName.ToLower() || m_InlinedFiles.Contains(label.m_File) || label.m_File == "%O")
                            {
                                address = label.m_Address;
                            }
                            else
                            {
                                Console.WriteLine($"LINKER ERROR: file {label.m_File} is not include inlined with .includeil");
                                Console.WriteLine($"{m_currFile}:{m_lineNumber}");
                                Environment.Exit(1);
                            }
                        }
                        else
                        {
                            address = label.m_Address;
                        }
                    }
                }
            }

            if (Mask == "MH")
            {
                address = (int)(address & 0xFFFF0000) >> 16;
            }

            if (Mask == "ML")
            {
                address = address & 0x0000FFFF;
            }

            if (rel == true)
            {
                if (m_hasAllLabel)
                {
                    int tryAddress = address - m_pc;
                    if (tryAddress > 0xFF)
                    {
                        Console.WriteLine($"{line} is not a relative address it's over 0xFF in size {m_currFile}:{m_lineNumber}");
                        //Environment.Exit(1);
                    }
                    else
                    {
                        address -= m_pc;
                    }
                }
                Outputaddress = SplitHexString(Convert.ToString(address, 16), 1);
            }
            else if (line.StartsWith("_N"))
            {
                Outputaddress = SplitHexString(Convert.ToString(address, 16), 1);
            }
            else if (line.StartsWith("_S"))
            {
                Outputaddress = SplitHexString(Convert.ToString(address, 16), 2);
            }
            else if (line.StartsWith("_L"))
            {
                Outputaddress = SplitHexString(Convert.ToString(address, 16), 3);
            }
            else if (line.StartsWith("_F"))
            {
                Outputaddress = SplitHexString(Convert.ToString(address, 16), 4);
            }
            else
            {
                throw new NotImplementedException();
            }


            return Outputaddress;
        }

        string gen_expr(string binExpr, string binType)
        {
            string[] expr = binExpr.Split(' ', StringSplitOptions.RemoveEmptyEntries);


            Stack<int> SolverStack = new Stack<int>();

            bool pushOut = false;

            bool IsLong = binType == "BINEXPRL";
            bool IsFar = binType == "BINEXPRF";

            for (int i = 0; i < expr.Length; i++)
            {
                string term = expr[i];
                if (gen_term(term, out string[] data))
                {
                    string fullExpr = "";

                    for (int d = 0; d < data.Length; d++)
                    {
                        fullExpr += data[d];
                    }

                    if (term.StartsWith("i_"))
                    {
                        SolverStack.Push(Convert.ToInt32(fullExpr, 16));
                    }
                    else if (term.StartsWith("SL_") || term.StartsWith("LL_") || term.StartsWith("FL_"))
                    {
                        SolverStack.Push(Convert.ToInt32(fullExpr, 16));
                    }
                    else
                    {
                        SolverStack.Push(Convert.ToInt32(fullExpr, 16));
                    }
                }
                else
                {
                    int rsh;
                    int lsh;
                    switch (term)
                    {
                        case "+":
                            rsh = SolverStack.Pop();
                            lsh = SolverStack.Pop();

                            if (pushOut)
                            {
                                addString(Convert.ToString(rsh, 16).PadLeft(2, '0'));
                                addString(Convert.ToString(lsh, 16).PadLeft(2, '0'));
                                break;
                            }

                            SolverStack.Push(lsh + rsh);
                            break;
                        case "-":
                            rsh = SolverStack.Pop();
                            lsh = SolverStack.Pop();

                            if (pushOut)
                            {
                                addString(Convert.ToString((byte)-rsh, 16).PadLeft(2, '0'));
                                addString(Convert.ToString(lsh, 16).PadLeft(2, '0'));
                                //break;
                            }

                            SolverStack.Push(lsh - rsh);
                            break;
                        case "*":
                            rsh = SolverStack.Pop();
                            lsh = SolverStack.Pop();

                            if (pushOut)
                            {
                                addString(Convert.ToString(rsh, 16).PadLeft(2, '0'));
                                addString(Convert.ToString(lsh, 16).PadLeft(2, '0'));
                                break;
                            }

                            SolverStack.Push(lsh * lsh);
                            break;
                        case "/":
                            rsh = SolverStack.Pop();
                            lsh = SolverStack.Pop();

                            if (pushOut)
                            {
                                addString(Convert.ToString(rsh, 16).PadLeft(2, '0'));
                                addString(Convert.ToString(lsh, 16).PadLeft(2, '0'));
                                break;
                            }

                            SolverStack.Push(lsh / lsh);
                            break;
                        case "&":
                            rsh = SolverStack.Pop();
                            lsh = SolverStack.Pop();

                            if (pushOut)
                            {
                                addString(Convert.ToString(rsh, 16).PadLeft(2, '0'));
                                addString(Convert.ToString(lsh, 16).PadLeft(2, '0'));
                                break;
                            }

                            SolverStack.Push(lsh & lsh);
                            break;
                        case "~":
                            rsh = SolverStack.Pop();

                            if (pushOut)
                            {
                                addString(Convert.ToString(rsh, 16).PadLeft(2, '0'));
                                break;
                            }

                            SolverStack.Push(~rsh);
                            break;
                        default:
                            break;
                    }
                }
            }
            if (SolverStack.Count != 1)
                throw new NotImplementedException();

            int result = SolverStack.Pop();

            if (IsLong == false && IsFar == false)
            {
                if (result > 0xFFFF)
                {
                    result = (ushort)result;
                }
            }
            else if (IsLong == true && IsFar == false)
            {
                if (result > 0xFFFFF)
                {
                    result = result & 0xFFFFF;
                }
            }
            else if (IsLong == false && IsFar == true)
            {
                if (result > 0xFFFFFF)
                {
                    result = result & 0xFFFFFF;
                }
            }

            string hexString = Convert.ToString(result, 16).PadLeft(4, '0');

            return hexString;
        }

        string getPCHex(Section section)
        {
            uint address = (uint)m_pc;
            return Convert.ToString(address, 16).PadLeft(8, '0');
        }

        void addString(string line)
        {
            for (int i = 0; i < m_Sections.Count; i++)
            {
                if (m_Sections[i].InSection(m_pc))
                {
                    m_Output.Add($"{getPCHex(m_Sections[i])}:\t{line}");
                    m_pc++;
                    m_instrOffset++;
                }
            }
        }
        bool getLabel(string name, out Label label)
        {
            for (int i = 0; i < m_Labels.Count; i++)
            {
                if (m_Labels[i].m_Name == name)
                {
                    label = m_Labels[i];
                    return true;
                }
            }
            label = new Label();
            return false;
        }

        void buildInclude()
        {
            for (m_i = 0; m_i < m_src.Length; m_i++)
            {
                if (m_src[m_i] == "INCLUDE HEADER")
                {
                    m_i++;
                    for (; m_i < m_src.Length; m_i++)
                    {
                        if (string.IsNullOrEmpty(m_src[m_i]))
                        {
                            continue;
                        }

                        string[] line = m_src[m_i].Split(' ');
                        if (line[0].StartsWith("INCINIL"))
                        {
                            m_InlinedFiles.Add(line[1]);
                        }

                        if (m_src[m_i].EndsWith("HEADER"))
                        {
                            break;
                        }
                    }
                    break;
                }
            }
        }
        void buildSymbols()
        {
            for (m_i = 0; m_i < m_src.Length; m_i++)
            {
                if (m_src[m_i] == "SYMBOLS HEADER")
                {
                    m_i++;
                    for (; m_i < m_src.Length; m_i++)
                    {
                        if (string.IsNullOrEmpty(m_src[m_i]))
                        {
                            continue;
                        }


                        string[] line = m_src[m_i].Trim('\n', '\r').Split(' ');
                        string name;
                        bool IsGolbal;
                        bool IsLocal;
                        string file;
                        string[] Ident;
                        string[] Flags;
                        bool Con = false;

                        if (line[0] == "_REF_")
                        {
                            switch (line[1])
                            {
                                case "LABEL":
                                    Ident = line[2].Split(',');
                                    name = Ident[0];
                                    Flags = Ident[1].Split('.');
                                    IsGolbal = bool.Parse(Flags[0]);
                                    IsLocal = bool.Parse(Flags[1]);
                                    file = line[3];

                                    m_Labels.ForEach(label =>
                                    {
                                        if (label.m_Name == name)
                                        {
                                            Con = true;
                                        }
                                    });

                                    if (Con)
                                    {
                                        continue;
                                    }

                                    m_Labels.Add(
                                        new Label()
                                        {
                                            m_Name = name,
                                            m_IsGlobal = IsGolbal,
                                            m_IsLocal = IsLocal,
                                            m_File = file
                                        });
                                    break;
                                default:
                                    break;
                            }
                        }
                        else if (m_src[m_i].EndsWith("HEADER"))
                        {
                            break;
                        }
                    }
                    break;
                }
            }
        }

        List<string> m_mapFile = new List<string>();
        public string[] GenerateMapFile()
        {

            m_mapFile.Add("Sections:");
            m_mapFile.Add("Start\tSize\t\t" + "Name".PadRight(25, ' ') + "File");
            m_mapFile.Add("");

            for (int i = 0; i < m_Sections.Count; i++)
            {
                Section section = m_Sections[i];
                string start = ToHexString(section.m_Start).PadLeft(4, '0');
                string size = ToHexString(section.m_Size).PadLeft(4, '0');

                m_mapFile.Add($"{start}\t{size}\t\t"+ $"{section.m_Name}".PadRight(25, ' '));
            }

            m_mapFile.Add("");
            m_mapFile.Add("Name".PadRight(45, ' ') + "Address\t" + "Section".PadRight(20, ' ') + "File");
            m_mapFile.Add("");

            for (int i = 0; i < m_Labels.Count; i++)
            {
                string address = Convert.ToString(m_Labels[i].m_Address, 16).PadLeft(8, '0');

                string file = m_Labels[i].m_File.Replace(Environment.CurrentDirectory, "");

                m_mapFile.Add($"{m_Labels[i].m_Name}".PadRight(45, ' ') + $"{address}\t" + $"{m_Labels[i].m_Section.m_Name}".PadRight(20, ' ') + $"{file}");
            }

            return m_mapFile.ToArray();
        }
    }
}
