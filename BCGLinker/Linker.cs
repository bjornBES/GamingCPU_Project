using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.ExceptionServices;
using static HexLibrary.HexConverter;
using static HexLibrary.StringFunctions;

namespace BCGLinker
{
    public class Linker
    {
        string[] m_src;

        public List<string> m_DataOutput = new List<string>();
        public List<string> m_Output = new List<string>();
        public string m_OutputBin = "";
        List<Label> m_Labels = new List<Label>();
        List<Struct> m_Structs = new List<Struct>();
        List<Section> m_Sections = new List<Section>();
        int i;

        int m_BPX_ref = 0;
        int m_SPX_ref = 0;

        string m_CurrFile = "";
        int m_pc = 0;

        public void BuildSrc(string src)
        {
            i = 0;
            m_DataOutput.Clear();

            //m_Output.Add("_TEXT SECTION".PadRight(16, '\0'));

            m_src = src.Split(Environment.NewLine);

            m_pc = 0;

            //BuildRdata();
            
            //BuildData();
            
            //BuildBss();
            
            BuildText();

            m_pc = 0;
            i = 0;
            m_src = src.Split(Environment.NewLine);
            m_DataOutput.Clear();
            m_Output.Clear();
            m_BPX_ref = m_SPX_ref = 0;

            //BuildRdata();

            //BuildData();
            //BuildBss();

            m_pc = 0;

            BuildText();

            List<string> tempList = new List<string>();

            string[] m_Output_ARRAY = m_Output.ToArray();
            m_Output_ARRAY = SortHexStrings(m_Output_ARRAY);

            //string[] m_Output_BSS_ARRAY = m_Output_BSS.ToArray();F
            //m_Output_BSS_ARRAY = sortHexStrings(m_Output_BSS_ARRAY);

            string[] m_Output_DATA_ARRAY = m_DataOutput.ToArray();
            m_Output_DATA_ARRAY = SortHexStrings(m_Output_DATA_ARRAY);

            switch (LinkerSettings.OutputFormat)
            {
                case OutputFormats.lib:
                    DebugWriter.Writeline("Output Format lib");
                    tempList.Add("library ACL0.1\0");

                    tempList.Add("_TEXT SECTION".PadRight(16, '\0'));
                    tempList.AddRange(m_Output_ARRAY);

                    tempList.Add("_DATA SECTION".PadRight(16, '\0'));
                    tempList.AddRange(m_Output_DATA_ARRAY);

                    tempList.Add("_SYMBOLS".PadRight(16, '\0'));
                    break;
                case OutputFormats.fbin:
                case OutputFormats.bin:
                    DebugWriter.Writeline("Output Format Bin");
                    // tempList.Add("_TEXT SECTION".PadRight(16, '\0'));
                    tempList.AddRange(m_Output_ARRAY);

                    // tempList.Add("_DATA SECTION".PadRight(16, '\0'));
                    tempList.AddRange(m_Output_DATA_ARRAY);
                    break;
                default:
                    break;
            }
            //tempList.AddRange(m_Output_BSS_ARRAY);

            File.WriteAllLines("./PostLinker.txt", tempList.ToArray());

            string[] m_pre_Output = tempList.ToArray();
            m_Output.Clear();

            List<string> UsedAddress = new List<string>();
            for (int i = 0; i < m_pre_Output.Length; i++)
            {
                string line = m_pre_Output[i];

                if (line.StartsWith('_'))
                {
                    m_Output.Add(line);
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

                byte number = Convert.ToByte(code, 16);

                if (UsedAddress.Contains(Full_address))
                {
                    m_Output[^1] = ((char)number).ToString();
                    //m_Output[^1] = "  " + address + ":\t" + code + " DEBUG CODE";
                    continue;
                }
                else
                {
                    m_Output.Add(((char)number).ToString());
                    //write_out(code + " DEBUG CODE");
                }
                UsedAddress.Add(Full_address);
            }

            for (int i = 0; i < m_Output.Count; i++)
            {
                m_OutputBin += m_Output[i];
            }
            if (m_OutputBin.Length % 3 != 0)
            {
                m_OutputBin += "\0";
            }
        }

        public void BuildSectionSymbols(string src)
        {
            i = 0;
            m_src = src.Split(Environment.NewLine);

            BuildSymbols();
        }

        public void BuildSectionStruct(string src)
        {
            i = 0;
            m_src = src.Split(Environment.NewLine);

            BuildStruct();
        }

        public void BuildSectionSection(string src)
        {
            i = 0;
            m_src = src.Split(Environment.NewLine);

            BuildSections();
        }

        int m_InstrOffset = 0;
        int lineNumber = 0;
        void BuildText()
        {
            for (i = 0; i < m_src.Length; i++)
            {
                if (m_src[i] == "TEXT HEADER")
                {
                    i++;
                    for (; i < m_src.Length; i++)
                    {
                        string line = m_src[i];
                        if (string.IsNullOrEmpty(line))
                        {
                            continue;
                        }

                        ParseLine(line, out bool Exit);
                        if (Exit)
                        {
                            break;
                        }
                    }
                    break;
                }
            }
        }

        private void ParseLine(string line, out bool Exit)
        {
            Exit = false;
            if (line == "_NEWLINE_")
            {
                m_InstrOffset = 0;
                lineNumber++;
                return;
            }
            else if (line.StartsWith("_FILE_"))
            {
                string file = line.Split(' ').Last();
                m_CurrFile = file.Trim('"');
                return;
            }
            else if (line.StartsWith("_TIMES_"))
            {
                string Lineexpr = line.Split(" ", 2).Last();
                string NumberExpr = Lineexpr.Split(',')[0];
                string Instr = Lineexpr.Split(",")[1].Split('.')[0];

                string result;

                if (NumberExpr.StartsWith("BINEXPR"))
                {
                    string bin_type = line.Split(' ').First();
                    result = gen_expr(NumberExpr, bin_type);

                }
                else if(gen_term(NumberExpr, out string[] outputExpr))
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

                int times = Convert.ToInt32(result, 16) + 1;
                int size = int.Parse(line.Split('.').Last());

                string[] ResultStr = SplitHexString(Instr, size);

                for (int a = 0; a < times; a++)
                {
                    for (int i = 0; i < ResultStr.Length; i++)
                    {
                        AddString(ResultStr[i]);
                    }
                }
            }
            else if (line.StartsWith("_REF_"))
            {
                string[] s_line = m_src[i].Split(' ');
                string name;
                bool IsGolbal;
                string Hex_address;
                int address;
                string file;
                string[] Ident;
                if (s_line[0] == "_REF_")
                {
                    switch (s_line[1])
                    {
                        case "LABEL":
                            Ident = s_line[2].Split(',');
                            name = Ident[0];
                            IsGolbal = bool.Parse(Ident[1]);
                            Hex_address = s_line[3].Trim('[', ']').Replace("0x", "");
                            address = Convert.ToInt32(Hex_address, 16);
                            file = s_line[4];

                            int index = m_Labels.FindIndex(label =>
                            {
                                if (label.m_Name == name)
                                {
                                    return true;
                                }
                                return false;
                            });

                            m_Labels[index].m_Address = m_pc;

                            break;
                        default:
                            break;
                    }
                }
            }
            else if (line.StartsWith("_DEL_"))
            {
                m_InstrOffset = 0;
                return;
            }
            else if (line.StartsWith("_INC_FILE_"))
            {

            }
            else if (line.StartsWith("_OFF_"))
            {
                int offset = Convert.ToInt32(line.Replace("_OFF_ ", ""), 16);
                m_pc = offset;
            }
            else if (line.StartsWith("_SET_"))
            {
                line = line.Replace("_SET_ ", "");
                string register = line.Split(' ').First();
                int number = int.Parse(line.Split(" ").Last());

                switch (register)
                {
                    case "BPX":
                        m_BPX_ref = number;
                        break;
                    case "SPX":
                        m_SPX_ref = number;
                        break;

                    default:
                        break;
                }
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
                    AddString(hex[i]);
                }
            }
            else if (gen_term(line, out string[] expr))
            {
                for (int i = 0; i < expr.Length; i++)
                {
                    AddString(expr[i]);
                }
            }
            else
            {
                AddString(line);
            }
        }

        bool gen_term(string line, out string[] expr)
        {
            if (line.StartsWith("SL_") || line.StartsWith("LL_") || line.StartsWith("FL_"))
            {
                string name = line.Replace("FL_", "").Replace("LL_", "").Replace("SL_", "");
                if (!GetLabel(name, out Label label))
                {
                    Console.WriteLine($"LINKER ERROR: {name} not found as symbol");
                }

                string[] address;

                if (line.StartsWith("LL_"))
                {
                    address = label.GetAddressLong();
                }
                else if (line.StartsWith("FL_"))
                {
                    address = label.GetAddressFar();
                }
                else
                {
                    address = label.GetAddress();
                }

                expr = address; 
                return true;
            }
            else if (line.StartsWith("_SCA_") || line.StartsWith("_LCA_") || line.StartsWith("_FCA_"))
            {
                string[] address;

                int PCValue = m_pc - m_InstrOffset;

                if (line.StartsWith("LL_"))
                {
                    address = SplitHexString(Convert.ToString(PCValue, 16), 4);
                }
                else if (line.StartsWith("FL_"))
                {
                    address = SplitHexString(Convert.ToString(PCValue, 16), 3);
                }
                else
                {
                    address = SplitHexString(Convert.ToString(PCValue, 16), 2);
                }

                expr = address;
                return true;
            }
            else if (line.StartsWith("_SCS_") || line.StartsWith("_LCS_") || line.StartsWith("_FCS_"))
            {
                string[] address;

                int PCValue = (m_pc - m_InstrOffset) % 0x200;

                if (line.StartsWith("LL_"))
                {
                    address = SplitHexString(Convert.ToString(PCValue, 16), 4);
                }
                else if (line.StartsWith("FL_"))
                {
                    address = SplitHexString(Convert.ToString(PCValue, 16), 3);
                }
                else
                {
                    address = SplitHexString(Convert.ToString(PCValue, 16), 2);
                }

                expr = address;
                return true;
            }
            else if (line.StartsWith("I_"))
            {
                string data = line.Replace("I_", "");
                expr = SplitHexString(data);
                return true;
            }
            else if (IsHex(line))
            {
                expr = SplitHexString(line);
                return true;
            }
            else if (line.StartsWith("SLSM_"))
            {
                line = line.Replace("SLSM_", "");

                string[] layers = line.Split('.');

                string Labelname = layers[0];
                if (!GetLabel(Labelname, out Label label))
                {
                    Console.WriteLine($"LINKER ERROR: {Labelname} label not found as symbol");
                }

                Struct _struct = label.m_Struct;

                StructMembers[] structMembers = _struct.m_StructMembers.ToArray();

                for (int i = 0; i < structMembers.Length; i++)
                {
                    if (structMembers[i].m_Name == layers[1])
                    {
                        expr = label.GetAddress(structMembers[i]);
                        return true;
                    }
                }
                expr = null;
                return false;
            }
            else if (line.StartsWith("_SSO_"))
            {
                line = line.Replace("_SSO_", "");

                string[] layers = line.Split('.');

                string Structname = layers[0];
                if (!GetStruct(Structname, out Struct _struct))
                {
                    Console.WriteLine($"LINKER ERROR: {Structname} struct not found as symbol");
                }

                if (layers.Length > 1)
                {
                    StructMembers[] structMembers = _struct.m_StructMembers.ToArray();

                    for (int i = 0; i < structMembers.Length; i++)
                    {
                        if (structMembers[i].m_Name == layers[1])
                        {
                            expr = structMembers[i].GetSize();
                            return true;
                        }
                    }
                }
                else if (layers.Length <= 1)
                {
                    expr = _struct.GetSize();
                    return true;
                }
                expr = null;
                return false;
            }
            expr = null;
            return false;
        }
        string gen_expr(string binExpr, string binType)
        {
            string[] expr = binExpr.Split(' ', StringSplitOptions.RemoveEmptyEntries).Reverse().ToArray();


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

                    if (term.StartsWith("I_"))
                    {
                        SolverStack.Push(Convert.ToInt32(fullExpr));
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
                else if (term.StartsWith("R_"))
                {
                    string register = term.Replace("R_", "");
                    switch (register)
                    {
                        case "BPX":
                        case "BP":
                            SolverStack.Push(m_BPX_ref);
                            break;
                        default:
                            break;
                    }
                    //pushOut = true;
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
                                AddString(Convert.ToString(rsh, 16).PadLeft(2, '0'));
                                AddString(Convert.ToString(lsh, 16).PadLeft(2, '0'));
                                break;
                            }

                            SolverStack.Push(lsh + rsh);
                            break;
                        case "-":
                            rsh = SolverStack.Pop();
                            lsh = SolverStack.Pop();

                            if (pushOut)
                            {
                                AddString(Convert.ToString((byte)-rsh, 16).PadLeft(2, '0'));
                                AddString(Convert.ToString(lsh, 16).PadLeft(2, '0'));
                                //break;
                            }

                            SolverStack.Push(lsh - rsh);
                            break;
                        case "*":
                            rsh = SolverStack.Pop();
                            lsh = SolverStack.Pop();

                            if (pushOut)
                            {
                                AddString(Convert.ToString(rsh, 16).PadLeft(2, '0'));
                                AddString(Convert.ToString(lsh, 16).PadLeft(2, '0'));
                                break;
                            }

                            SolverStack.Push(lsh * lsh);
                            break;
                        case "/":
                            rsh = SolverStack.Pop();
                            lsh = SolverStack.Pop();

                            if (pushOut)
                            {
                                AddString(Convert.ToString(rsh, 16).PadLeft(2, '0'));
                                AddString(Convert.ToString(lsh, 16).PadLeft(2, '0'));
                                break;
                            }

                            SolverStack.Push(lsh / lsh);
                            break;
                        case "&":
                            rsh = SolverStack.Pop();
                            lsh = SolverStack.Pop();

                            if (pushOut)
                            {
                                AddString(Convert.ToString(rsh, 16).PadLeft(2, '0'));
                                AddString(Convert.ToString(lsh, 16).PadLeft(2, '0'));
                                break;
                            }

                            SolverStack.Push(lsh & lsh);
                            break;
                        case "~":
                            rsh = SolverStack.Pop();

                            if (pushOut)
                            {
                                AddString(Convert.ToString(rsh, 16).PadLeft(2, '0'));
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

        void AddString(string line)
        {
            for (int i = 0; i < m_Sections.Count; i++)
            {
                Section section = m_Sections[i];
                bool IsIn = section.InSection(m_pc);
                bool DoBreak = false;
                if (IsIn)
                {
                    switch (section.m_Name)
                    {
                        case "_text":
                            m_Output.Add($"{GetPCOffset(section)}\t{line}");
                            DoBreak = true;
                            m_pc++;
                            m_InstrOffset++;
                            break;
                        case "_data":
                            m_DataOutput.Add($"{GetPCOffset(section)}\t{line}");
                            DoBreak = true;
                            m_pc++;
                            break;
                        default:
                            break;
                    }
                    if (DoBreak)
                    {
                        break;
                    }
                }
            }
        }
        bool GetLabel(string name, out Label label)
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
        bool GetStruct(string name, out Struct result)
        {
            for (int i = 0; i < m_Structs.Count; i++)
            {
                if (m_Structs[i].m_Name == name)
                {
                    result = m_Structs[i];
                    return true;
                }
            }
            result = null;
            return false;
        }
        string GetPCOffset(Section section)
        {
            int offset = m_pc + section.m_offset;
            if (m_pc > section.m_offset - 1)
            {
                offset -= section.m_offset;
            }
            return $"{Convert.ToString(offset, 16).PadLeft(6, '0')}:";
        }

        void BuildSymbols()
        {
            for (i = 0; i < m_src.Length; i++)
            {
                if (m_src[i] == "SYMBOLS HEADER")
                {
                    i++;
                    for (; i < m_src.Length; i++)
                    {
                        if (string.IsNullOrEmpty(m_src[i]))
                        {
                            continue;
                        }

                        string[] line = m_src[i].Split(' ');
                        string name;
                        string labelName;
                        bool IsGolbal;
                        int size;
                        string Hex_address;
                        int address;
                        string file;
                        string[] Ident;
                        if (line[0] == "_REF_")
                        {
                            switch (line[1])
                            {
                                case "LABEL":
                                    Ident = line[2].Split(',');
                                    name = Ident[0];
                                    IsGolbal = bool.Parse(Ident[1]);
                                    Hex_address = line[3].Trim('[', ']').Replace("0x", "");
                                    address = Convert.ToInt32(Hex_address, 16);
                                    file = line[4];

                                    m_Labels.Add(
                                        new Label()
                                        {
                                            m_Name = name,
                                            m_IsGlobal = IsGolbal,
                                            m_Address = address,
                                            m_File = file
                                        });
                                    break;
                                case "STRUCT":
                                    Ident = line[2].Split(',');
                                    name = Ident[0];
                                    size = Convert.ToInt32(Ident[1], 16);
                                    labelName = line[3];

                                    if (GetLabel(labelName, out Label result))
                                    {
                                        int index = m_Labels.IndexOf(result);
                                        if (GetStruct(name, out Struct Structresult))
                                        {
                                            m_Labels[index].m_HaveStruct = true;
                                            m_Labels[index].m_Struct = Structresult;
                                        }
                                        else
                                        {
                                            Console.WriteLine($"LINKER ERROR: {name} struct not found as symbol");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine($"LINKER ERROR: {name} label not found as symbol");
                                    }

                                    break;
                                default:
                                    break;
                            }
                        }
                        else if (m_src[i].EndsWith("HEADER"))
                        {
                            break;
                        }
                    }
                    break;
                }
            }
        }
        void BuildSections()
        {
            for (i = 0; i < m_src.Length; i++)
            {
                if (m_src[i] == "SECTION HEADER")
                {
                    i++;
                    for (; i < m_src.Length; i++)
                    {
                        if (m_src[i].EndsWith("HEADER"))
                        {
                            break;
                        }

                        if (string.IsNullOrEmpty(m_src[i]))
                        {
                            continue;
                        }

                        string[] line = m_src[i].Split(' ');
                        string sectionName = FindName(line.First());
                        string Hex_offset = line[1].Trim('[', ']').Replace("0x", "");
                        string file = line[2];
                        int offset = Convert.ToInt32(Hex_offset, 16);

                        m_Sections.Add(new Section()
                        {
                            m_Name = sectionName,
                            m_offset = offset,
                        });
                    }

                    int size = 0;
                    List<Section> sections = new List<Section>(m_Sections);
                    m_Sections.Clear();
                    for (int i = 0; i < sections.Count; i++)
                    {
                        Section Section = sections[i];
                        int index = sections.IndexOf(Section);
                        if (sections.Count <= index)
                        {
                            m_Sections.Add(Section);
                            continue;
                        }
                        if (sections.Count <= index + 1)
                        {
                            m_Sections.Add(Section);
                            continue;
                        }

                        Section.m_size = sections[index + 1].m_offset - size;
                        size += Section.m_size;

                        m_Sections.Add(Section);
                    }
                }
            }
        }
        void BuildStruct()
        {
            for (i = 0; i < m_src.Length; i++)
            {
                if (m_src[i] == "STRUCT HEADER")
                {
                    i++;
                    int MemberAddress = 0;
                    for (; i < m_src.Length; i++)
                    {
                        if (m_src[i].EndsWith("HEADER"))
                        {
                            break;
                        }

                        if (string.IsNullOrEmpty(m_src[i]))
                        {
                            continue;
                        }

                        Struct TempStruct = new Struct();

                        string[] line = m_src[i].Split(' ', 4);
                        string StructName = line[0];
                        string ResBytes = line[2];
                        string[] StructMembers = line[3].Trim('{', '}').Split(",");

                        TempStruct.m_Name = StructName;
                        TempStruct.m_Size = Convert.ToInt32(ResBytes, 16);

                        for (int i = 0; i < StructMembers.Length; i++)
                        {
                            string[] member = StructMembers[i].Split(' ');
                            string MemberName = member[0];
                            string MemberResBytes = member[2];

                            int memberSize = Convert.ToInt32(MemberResBytes, 16);

                            TempStruct.m_StructMembers.Add(new StructMembers()
                            {
                                m_Name = MemberName,
                                m_Size = memberSize,
                                m_offset = MemberAddress
                            });
                            MemberAddress += memberSize;
                        }

                        m_Structs.Add(TempStruct);
                    }
                }
            }
        }

        string FindName(string name)
        {
            if (name.Contains(LinkerSettings.TextSection, StringComparison.OrdinalIgnoreCase))
            {
                return "_text";
            }
            else if (name.Contains(LinkerSettings.DataSection, StringComparison.OrdinalIgnoreCase))
            {
                return "_data";
            }
            else if (name.Contains(LinkerSettings.RDataSection, StringComparison.OrdinalIgnoreCase))
            {
                return "_rdata";
            }
            else if (name.Contains(LinkerSettings.BSSSection, StringComparison.OrdinalIgnoreCase))
            {
                return "_bss";
            }
            else if (name.Contains(LinkerSettings.HeaderSection, StringComparison.OrdinalIgnoreCase))
            {
                return "_header";
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        List<string> mapFile = new List<string>();
        public string[] GenerateMapFile()
        {

            mapFile.Add("Offset\t\t:\tsize\t\t\tName");
            mapFile.Add("");

            for (int i = 0; i < m_Sections.Count; i++)
            {
                Section currSection = m_Sections[i];

                string offset = Convert.ToString(currSection.m_offset, 16).PadLeft(8, '0');
                string size = Convert.ToString(currSection.m_size, 16).PadLeft(8, '0');

                mapFile.Add($"{offset}\t:\t{size}\t\t" + $"{currSection.m_Name}");
            }

            mapFile.Add("");
            mapFile.Add("Address\t:\toffset\t\t" + "Name".PadRight(25, ' ') + "File");
            mapFile.Add("");

            for (int i = 0; i < m_Labels.Count; i++)
            {
                string address = Convert.ToString(m_Labels[i].m_Address, 16).PadLeft(8, '0');

                string file = m_Labels[i].m_File.Replace(Environment.CurrentDirectory, "");

                string segment = address.Substring(0, 4);
                string offset = address.Substring(4);

                mapFile.Add($"{segment}\t:\t{offset}\t\t"+ $"{m_Labels[i].m_Name}".PadRight(25, ' ') + $"{file}");
            }

            return mapFile.ToArray();
        }
    }
}
