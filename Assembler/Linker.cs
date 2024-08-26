using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using assembler.global;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Concurrent;

namespace assembler
{
    public class Linker
    {
        string[] m_pre_Output = new string[0];
        public List<string> m_Output = new List<string>();
        public List<string> m_Output_DATA = new List<string>();
        public List<string> m_Output_BSS = new List<string>();
        List<Label> m_labels = new List<Label>();
        List<string> m_src = new List<string>();
        List<string[]> m_binary = new List<string[]>();
        string m_file;
        int m_fileIndex;

        int m_pc = 0x000000;
        int m_datapc = 0x000000;
        int m_bsspc = 0xF6C000;

        section CurrentSection;
        
        List<Struct> m_structs = new List<Struct>();    

        bool m_doLabels = true;

        List<int> m_procssedFiles = new List<int>();

        public void BuildSrc(Generator generator, List<string[]> binary)
        {
            m_structs = generator.m_Structs;

            m_binary = binary;
            for (m_fileIndex = 0; m_fileIndex < binary.Count; m_fileIndex++)
            {
                if (m_procssedFiles.Contains(m_fileIndex)) continue;
                m_src = binary[m_fileIndex].ToList();

                BuildFile(binary[m_fileIndex]);

                m_procssedFiles.Add(m_fileIndex);
            }
            
            m_procssedFiles.Clear();
            m_Output.Clear();
            m_Output_DATA.Clear();
            m_Output_BSS.Clear();
            m_pc = 0x000000;
            m_datapc = 0x000000;
            m_bsspc = 0xF6C000;

            m_doLabels = false;
            for (m_fileIndex = 0; m_fileIndex < binary.Count; m_fileIndex++)
            {
                if (m_procssedFiles.Contains(m_fileIndex)) continue;
                m_src = binary[m_fileIndex].ToList();

                BuildFile(binary[m_fileIndex]);

                m_procssedFiles.Add(m_fileIndex);
            }


            List<string> tempList = new List<string>();

            string[] m_Output_ARRAY = m_Output.ToArray();
            m_Output_ARRAY = sortHexStrings(m_Output_ARRAY);

            string[] m_Output_BSS_ARRAY = m_Output_BSS.ToArray();
            m_Output_BSS_ARRAY = sortHexStrings(m_Output_BSS_ARRAY);

            string[] m_Output_DATA_ARRAY = m_Output_DATA.ToArray();
            m_Output_DATA_ARRAY = sortHexStrings(m_Output_DATA_ARRAY);

            tempList.AddRange(m_Output_ARRAY);
            tempList.AddRange(m_Output_DATA_ARRAY);
            tempList.AddRange(m_Output_BSS_ARRAY);

            File.WriteAllLines("./PostLinker.txt", tempList.ToArray());

            m_pre_Output = tempList.ToArray();
            m_Output.Clear();

            List<string> UsedAddress = new List<string>();
            for (int i = 0; i < m_pre_Output.Length; i++)
            {
                string line = m_pre_Output[i];

                if (line.Contains("DEBUG LINE"))
                {
                    continue;
                }

                string Full_address = m_pre_Output[i].Split(":\t")[0];
                string bank = Full_address.Split('_').Last();
                string address = Full_address.Split('_').First();
                string code = m_pre_Output[i].Split(":\t")[1];
                if (UsedAddress.Contains(Full_address))
                {
                    m_Output[^1] = $"{Convert.ToString(Convert.ToByte(bank), 16)}:" + code;
                    //m_Output[^1] = "  " + address + ":\t" + code + " DEBUG CODE";
                    continue;
                }
                else
                {
                    m_Output.Add($"{Convert.ToString(Convert.ToByte(bank), 16)}:" + code);
                    //write_out(code + " DEBUG CODE");
                }
                UsedAddress.Add(Full_address);
            }
        }
        void BuildFile(string[] File)
        {
            int lineNumber = 0;
            for (int i = 0; i < File.Length; i++)
            {
                string line = File[i];
                if (line.Contains("_DEL_"))
                {
                    if (!AssemblerSettings.Debug)
                    {
                        continue;
                    }
                }
                else if (line.Contains("_NEWARG_"))
                {
                    if (!AssemblerSettings.Debug)
                    {
                        continue;
                    }
                }
                else if (line.Contains("_FILE_"))
                {
                    m_file = line.Replace("_FILE_ ", "");
                    if (!AssemblerSettings.Debug)
                    {
                        continue;
                    }
                }
                else if (line.Contains("_NEWLINE_"))
                {
                    lineNumber++;
                    if (!AssemblerSettings.Debug)
                    {
                        continue;
                    }
                }
                else if (line == "_TEXT")
                {
                    CurrentSection = section.TEXT;
                    if (!AssemblerSettings.Debug)
                    {
                        continue;
                    }
                }
                else if (line == "_DATA")
                {
                    CurrentSection = section.DATA;
                    if (!AssemblerSettings.Debug)
                    {
                        continue;
                    }
                }
                else if (line == "_BSS")
                {
                    CurrentSection = section.BSS;
                    if (!AssemblerSettings.Debug)
                    {
                        continue;
                    }
                }
                else if (line.Contains("_INCIL_"))
                {
                    string path = line.Replace("_INCIL_ ", "");
                    int fileindex = GetFileIndex(path);
                    string saveFile = m_file;
                    BuildFile(m_binary[fileindex]);
                    m_file = saveFile;
                    m_procssedFiles.Add(fileindex);
                    if (!AssemblerSettings.Debug)
                    {
                        continue;
                    }
                }
                else if (line.StartsWith("??"))
                {
                    if (File[i - 1] == line)
                    {
                        if (!AssemblerSettings.Debug)
                        {
                            continue;
                        }
                    }
                    LinkerWarnings.MissingByteFromAddress(lineNumber, m_file);
                    if (!AssemblerSettings.Debug)
                    {
                        continue;
                    }
                }
                else if (line.Contains("BINEXPR") || line.Contains("BINEXPRWR"))
                {

                    string hexString = Parse(line).PadLeft(4, '0');

                    string[] hexStringArray = SplitHexString(hexString).ToArray();

                    for (int a = 0; a < hexStringArray.Length; a++)
                    {
                        write_out(hexStringArray[a], true);
                    }

                    if (!AssemblerSettings.Debug)
                    {
                        continue;
                    }
                }
                else if (line.Contains("_INC_"))
                {
                    if (!AssemblerSettings.Debug)
                    {
                        continue;
                    }
                }
                else if (line.Contains("_OFF_"))
                {
                    int address = Convert.ToInt32(Parse(line.Split(' ').Last()), 16);
                    m_pc = address;
                    if (!AssemblerSettings.Debug)
                    {
                        continue;
                    }
                }
                else if (line.Contains("_REF_"))
                {
                    string Ref = line.Replace("_REF_ ", "");
                    if (Ref.Contains("LABEL") && m_doLabels)
                    {
                        string data = Ref.Replace("LABEL ", "");
                        string name = data.Split(',').First();
                        bool global = bool.Parse(data.Split(',').Last());
                        int address = 0;

                        switch (CurrentSection)
                        {
                            case section.TEXT:
                                address = m_pc;
                                break;
                            case section.DATA:
                                address = m_datapc;
                                break;
                            case section.BSS:
                                address = m_bsspc;
                                break;
                            default:
                                break;
                        }

                        m_labels.Add(new Label()
                        {
                            Address = address,
                            Name = name,
                            IsGlobal = global,
                            File = m_file,
                        });
                    }
                    if (!AssemblerSettings.Debug)
                    {
                        continue;
                    }
                }
                else if (!m_doLabels && line.Contains("L_"))
                {
                    string name = line.Replace("LL_", "").Replace("L_", "");
                    Label label = GetLabel(name);
                    if (label == null)
                    {
                        LinkerErrors.MissingLabel(name, lineNumber, m_file);
                    }
                    string address = Convert.ToString(label.Address, 16).PadLeft(4, '0');
                    if (line.StartsWith("LL_"))
                    {
                        address = address.PadLeft(8, '0');
                    }
                    string[] hexStrings = SplitHexString(address).ToArray();

                    for (int a = 0; a < hexStrings.Length; a++)
                    {
                        write_out(hexStrings[a], true);
                    }
                    if (!AssemblerSettings.Debug)
                    {
                        continue;
                    }
                }
                else if (!m_doLabels && (line.Contains("SF_") || line.Contains("SBA_")))
                {
                    string name = line.Replace("SF_", "").Replace("SBA_", "");
                    string First = line.Replace(name, "");

                    string LabelStructName = name.Split('.')[0];
                    Struct _Struct = null;
                    for (int s = 0; s < m_structs.Count; s++)
                    {
                        if (m_structs[s].m_BaseLabel.Name == LabelStructName)
                        {
                            _Struct = m_structs[s];
                        }
                    }

                    if (First == "SF_")
                    {
                        string StructField = name.Split('.').Last().Replace("F_", "");
                        int ArrayIndexOffset = 0;
                        if (StructField.Contains('[') && StructField.EndsWith(']'))
                        {
                            ArrayIndexOffset = int.Parse(StructField.Split('[')[1].Trim(']'));
                            StructField = StructField.Split('[')[0].Trim(']');
                        }
                        for (int f = 0; f < _Struct.m_Fields.Count; f++)
                        {
                            if (_Struct.m_Fields[f].m_Name == StructField)
                            {
                                DataFields dataFields = _Struct.m_Fields[f];
                                int Address = _Struct.m_BaseAddress + dataFields.m_Offset + ArrayIndexOffset;
                                string HexString = Convert.ToString(Address, 16);
                                string[] HexStrings = SplitHexString(HexString).ToArray();

                                for (int a = 0; a < HexStrings.Length; a++)
                                {
                                    write_out(HexStrings[a], true);
                                }
                                break;
                            }
                        }
                    }
                    else if (First == "SBA_")
                    {
                        int Address = _Struct.m_BaseAddress;
                        string HexString = Convert.ToString(Address, 16);
                        string[] HexStrings = SplitHexString(HexString).ToArray();

                        for (int a = 0; a < HexStrings.Length; a++)
                        {
                            write_out(HexStrings[a], true);
                        }
                        if (!AssemblerSettings.Debug)
                        {
                            continue;
                        }
                    }
                }
                else if (line.StartsWith("_CA_"))
                {
                    string address = Convert.ToString(m_pc, 16).PadLeft(6, '0').Substring(2, 4);
                    string[] hexStrings = SplitHexString(address).ToArray();

                    for (int a = 0; a < hexStrings.Length; a++)
                    {
                        write_out(hexStrings[a], true);
                    }
                    //i--;
                    if (!AssemblerSettings.Debug)
                    {
                        continue;
                    }
                }
                else if (line.StartsWith("_CS_"))
                {
                    string address = Convert.ToString(m_pc % 0x200, 16).PadLeft(6, '0').Substring(2, 4);
                    string[] hexStrings = SplitHexString(address).ToArray();

                    for (int a = 0; a < hexStrings.Length; a++)
                    {
                        write_out(hexStrings[a], true);
                    }

                    if (!AssemblerSettings.Debug)
                    {
                        continue;
                    }
                }
                else if (line.StartsWith("_LCA_"))
                {
                    string address = Convert.ToString(m_pc + 1, 16).PadLeft(8, '0');
                    string[] hexStrings = SplitHexString(address).ToArray();

                    for (int a = 0; a < hexStrings.Length; a++)
                    {
                        write_out(hexStrings[a], true);
                    }
                    i--;
                    if (!AssemblerSettings.Debug)
                    {
                        continue;
                    }
                }
                else if (line.StartsWith("MS_"))
                {
                    string name = line.Replace("MS_", "");
                    for (int s = 0; s < m_structs.Count; s++)
                    {
                        if (name == m_structs[s].m_Name)
                        {
                            m_structs[s].m_BaseLabel = m_labels[^1];
                            m_structs[s].m_BaseAddress = m_labels[^1].Address;
                            break;
                        }
                    }
                }
                else if (!m_doLabels && (line.StartsWith("R_") || line.StartsWith("LR_")))
                {
                    string name = line.Replace("LR_", "").Replace("R_", "");
                    Label label = GetLabel(name);
                    if (label == null)
                    {
                        LinkerErrors.MissingLabel(name, lineNumber, m_file);
                    }
                    string address = Convert.ToString(label.Address + 1, 16).PadLeft(4, '0');
                    if (line.StartsWith("LR_"))
                    {
                        address = Convert.ToString(label.Address, 16).PadLeft(8, '0');
                    }
                    string[] hexStrings = SplitHexString(address).ToArray();

                    for (int a = 0; a < hexStrings.Length; a++)
                    {
                        write_out(hexStrings[a], true);
                    }
                    if (!AssemblerSettings.Debug)
                    {
                        continue;
                    }

                }
                else if (line.StartsWith("_SIR"))
                {
                    string ident = line.Replace("_SIR", "");

                    string label_name = ident.Split(':').First();
                    string register = ident.Split(':').Last();
                    Label label = GetLabel(label_name);
                    if (label == null)
                    {
                        LinkerErrors.MissingLabel(label_name, lineNumber, m_file);
                    }
                    string address = Convert.ToString(label.Address, 16).PadLeft(4, '0');
                    if (line.StartsWith("LR_"))
                    {
                        address = address.PadLeft(8, '0');
                    }
                    List<string> hexStrings = SplitHexString(address);
                    hexStrings.Add(register);

                    for (int a = 0; a < hexStrings.Count; a++)
                    {
                        write_out(hexStrings[a], true);
                    }
                    if (!AssemblerSettings.Debug)
                    {
                        continue;
                    }
                }
                else if (line.StartsWith("_RBYTE_"))
                {
                    incPC();
                    if (!AssemblerSettings.Debug)
                    {
                        continue;
                    }
                }
                else if (line.StartsWith("_RWORD_"))
                {
                    incPC();
                    incPC();
                    if (!AssemblerSettings.Debug)
                    {
                        continue;
                    }
                }
                else if (line.StartsWith("_RTBYTE_"))
                {
                    incPC();
                    incPC();
                    incPC();
                    if (!AssemblerSettings.Debug)
                    {
                        continue;
                    }
                }
                else if (line.StartsWith("_RDWORD_"))
                {
                    incPC();
                    incPC();
                    incPC();
                    incPC();
                    if (!AssemblerSettings.Debug)
                    {
                        continue;
                    }
                }
                else
                {
                    write_out(line, true);
                    if (!AssemblerSettings.Debug)
                    {
                        continue;
                    }
                }

                if (AssemblerSettings.DebugLevel > 3)
                {
                    write_out(line + " DEBUG LINE", false);
                }
            }
        }
        void incPC()
        {
            switch (CurrentSection)
            {
                case section.TEXT:
                    m_pc++;
                    break;
                case section.DATA:
                    m_datapc++;
                    break;
                case section.BSS:
                    m_bsspc++;
                    break;
                default:
                    break;
            }
        }
        string Parse(string line)
        {
            if (line.Contains("BINEXPR") || line.Contains("BINEXPRWR"))
            {
                string[] expr;
                if (line.Contains("BINEXPR"))
                {
                    expr = line["BINEXPR ".Length..^1].Split(' '); //ORG: expr = line.Substring("BINEXPR ".Length, line.Length - 1 - "BINEXPR ".Length).Split(' ');
                }
                else
                {
                    expr = line["BINEXPRWR ".Length..^1].Split(' '); //ORG: expr = line.Substring("BINEXPRWR ".Length, line.Length - 1 - "BINEXPRWR ".Length).Split(' ');
                }

                expr = expr.Reverse().ToArray();

                Stack<int> SolverStack = new Stack<int>();

                bool pushOut = false;

                for (int a = 0; a < expr.Length; a++)
                {
                    if (expr[a].StartsWith("L_"))
                    {
                        string name = expr[a].Replace("LL_", "").Replace("L_", "");
                        Label label = GetLabel(name);
                        if (label == null)
                        {
                            break;
                        }
                        SolverStack.Push(label.Address);
                    }
                    else if (expr[a].StartsWith("I_"))
                    {
                        int data = Convert.ToInt32(expr[a].Replace("I_", ""), 16);
                        SolverStack.Push(data);
                    }
                    else if (expr[a].StartsWith("R_"))
                    {
                        pushOut = true;
                        SolverStack.Push((int)Enum.Parse(typeof(Register), expr[a].Replace("R_", "")));
                    }
                    else if (expr[a] == "_CA_")
                    {
                        int address = Convert.ToInt32(m_pc + 1);
                        SolverStack.Push(address);
                    }
                    else if (expr[a] == "_CS_")
                    {
                        int address = m_pc % 0x200;
                        SolverStack.Push(address);
                    }
                    else if (expr[a] == "_LCA_")
                    {
                        int address = Convert.ToInt32(m_pc + 1);
                        SolverStack.Push(address);
                    }
                    else
                    {
                        int rsh;
                        int lsh;
                        switch (expr[a])
                        {
                            case "+":
                                rsh = SolverStack.Pop();
                                lsh = SolverStack.Pop();

                                if (pushOut)
                                {
                                    write_out(Convert.ToString(rsh, 16).PadLeft(2, '0'), false);
                                    write_out(Convert.ToString(lsh, 16).PadLeft(2, '0'), false);
                                    break;
                                }

                                SolverStack.Push(rsh + lsh);
                                break;
                            case "-":
                                rsh = SolverStack.Pop();
                                lsh = SolverStack.Pop();

                                if (pushOut)
                                {
                                    write_out(Convert.ToString((byte)-rsh, 16).PadLeft(2, '0'), false);
                                    write_out(Convert.ToString(lsh, 16).PadLeft(2, '0'), false);
                                    break;
                                }

                                SolverStack.Push(rsh - lsh);
                                break;
                            case "*":
                                rsh = SolverStack.Pop();
                                lsh = SolverStack.Pop();

                                if (pushOut)
                                {
                                    write_out(Convert.ToString(rsh, 16).PadLeft(2, '0'), false);
                                    write_out(Convert.ToString(lsh, 16).PadLeft(2, '0'), false);
                                    break;
                                }

                                SolverStack.Push(rsh * lsh);
                                break;
                            case "/":
                                rsh = SolverStack.Pop();
                                lsh = SolverStack.Pop();

                                if (pushOut)
                                {
                                    write_out(Convert.ToString(rsh, 16).PadLeft(2, '0'), false);
                                    write_out(Convert.ToString(lsh, 16).PadLeft(2, '0'), false);
                                    break;
                                }

                                SolverStack.Push(rsh / lsh);
                                break;
                            case "&":
                                rsh = SolverStack.Pop();
                                lsh = SolverStack.Pop();

                                if (pushOut)
                                {
                                    write_out(Convert.ToString(rsh, 16).PadLeft(2, '0'), false);
                                    write_out(Convert.ToString(lsh, 16).PadLeft(2, '0'), false);
                                    break;
                                }

                                SolverStack.Push(rsh & lsh);
                                break;
                            case "~":
                                rsh = SolverStack.Pop();

                                if (pushOut)
                                {
                                    write_out(Convert.ToString(rsh, 16).PadLeft(2, '0'), false);
                                    break;
                                }

                                SolverStack.Push(~rsh);
                                break;
                            default:
                                break;
                        }
                    }
                    if (pushOut)
                        break;
                }

                if (SolverStack.Count == 0)
                    return "";

                string hexString = Convert.ToString(SolverStack.Pop(), 16).PadLeft(4, '0');
                return hexString;
            }
            else
            {
                return line;
            }
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
        Label GetLabel(string label)
        {
            for (int i = 0; i < m_labels.Count; i++)
            {
                if (m_labels[i].Name == label)
                {
                    return m_labels[i];
                }
            }
            return null;
        }
        int GetFileIndex(string path)
        {
            for (int i = 0; i < m_binary.Count; i++)
            {
                for (int f = 0; f < 10; f++)
                {
                    if (m_binary[i][f].Contains($"_FILE_ {path}"))
                    {
                        return i;
                    }
                }
            }
            return 0;
        }
        void write_out(string str, bool INC)
        {
            switch (CurrentSection)
            {
                case section.TEXT:
                    m_Output.Add($"{Convert.ToString(m_pc, 16).PadLeft(6, '0')}_15:\t" + str);
                    break;
                case section.DATA:
                    m_Output_DATA.Add($"{Convert.ToString(m_datapc, 16).PadLeft(6, '0')}_0:\t" + str);
                    break;
                case section.BSS:
                    m_Output_BSS.Add($"{Convert.ToString(m_bsspc, 16).PadLeft(6, '0')}_0:\t" + str);
                    break;
                default:
                    break;
            }
            if (INC)
            {
                incPC();
            }
        }
        string[] sortHexStrings(string[] hexNumbers)
        {
            string[] sortedItems = hexNumbers
                        .Select(item => new
                        {
                            Original = item,
                            HexPart = item.Substring(0, 6),
                            NumericHex = Convert.ToInt32(item.Substring(0, 6), 16)
                        })
                        .OrderBy(x => x.NumericHex)
                        .Select(x => x.Original)
                        .ToArray();
            return sortedItems;
        }
    }
    public enum section
    {
        TEXT,
        DATA,
        BSS
    }
}
