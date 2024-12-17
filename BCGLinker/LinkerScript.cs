using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace BCGLinker
{
    public class LinkerScript
    {
        int offset = 0;
        List<Memory> memories = new List<Memory>();
        List<Section> sections = new List<Section>();

        string[] m_src;

        public Linker m_Linker;

        public void BuildLS(string linkerScrPath)
        {
            if (linkerScrPath == "")
            {
                return;
            }
            m_src = File.ReadAllText(linkerScrPath).Split(Environment.NewLine);

            build();
        }

        int oldOffset;
        void build()
        {
            for (int i = 0; i < m_src.Length; i++)
            {
                string line = m_src[i];

                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                if (line.StartsWith("address", StringComparison.OrdinalIgnoreCase))
                {
                    string address;
                    if (line.Contains("="))
                    {
                        address = line.Split(' ').Last();
                    }
                    else
                    {
                        //TODO
                    }
                }
                else if (line.Contains("MEMORY", StringComparison.OrdinalIgnoreCase))
                {
                    i++;
                    for (; i < m_src.Length; i++)
                    {
                        line = m_src[i];
                        if (string.IsNullOrEmpty(line))
                        {
                            continue;
                        }

                        if (line.StartsWith('{'))
                        {
                            continue;
                        }

                        if (line.StartsWith('}'))
                        {
                            break;
                        }
                        else
                        {
                            line = Regex.Replace(line, @" +", " ").Trim();
                            line = line.Replace(", ", ",").Trim();
                            line = line.Replace(" ,", ",").Trim();
                            string name = line.Split(':')[0];
                            string[] lines = line.Split(':')[1].Trim().Split(',');

                            int start = offset;
                            int size = 0x100;
                            Type type = Type.ReadWrite;

                            for (int e = 0; e < lines.Length; e++)
                            {
                                if (lines[e].StartsWith("start", StringComparison.OrdinalIgnoreCase))
                                {
                                    string[] segments = lines[e].Split(" ");
                                    if (segments[1] == "=")
                                    {
                                        start = parseTerm(segments[2]);
                                        offset = start;
                                    }
                                }
                                else if (lines[e].StartsWith("size", StringComparison.OrdinalIgnoreCase))
                                {
                                    string[] segments = lines[e].Split(" ");
                                    if (segments[1] == "=")
                                    {
                                        size = parseTerm(segments[2]);
                                        offset += size;
                                    }
                                }
                                else if (lines[e].StartsWith("type", StringComparison.OrdinalIgnoreCase))
                                {
                                    string[] segments = lines[e].Split(" ");
                                    if (segments[1] == "=")
                                    {
                                        if (segments[2].Contains("RW", StringComparison.OrdinalIgnoreCase))
                                        {
                                            type = Type.ReadWrite;
                                        }
                                    }
                                }
                            }

                            memories.Add(new Memory()
                            {
                                m_Name = name,
                                m_Size = size,
                                m_Start = start,
                                m_Type = type
                            });
                        }
                    }
                }
                else if (line.Contains("SECTIONS", StringComparison.OrdinalIgnoreCase))
                {
                    i++;
                    for (; i < m_src.Length; i++)
                    {
                        line = m_src[i];
                        if (string.IsNullOrEmpty(line))
                        {
                            continue;
                        }

                        if (line.StartsWith('{'))
                        {
                            continue;
                        }

                        if (line.StartsWith('}'))
                        {
                            break;
                        }
                        else
                        {
                            line = Regex.Replace(line, @" +", " ").Trim();
                            line = line.Replace(", ", ",").Trim();
                            line = line.Replace(" ,", ",").Trim();
                            string name = line.Split(':')[0];
                            string[] lines = line.Split(':')[1].Trim().Split(',');

                            Memory memory = new Memory();
                            int start = offset;
                            int size = 0x100;
                            string startSymbol = "";
                            string endSymbol = "";
                            string file = "";

                            for (int e = 0; e < lines.Length; e++)
                            {
                                if (lines[e].StartsWith("load", StringComparison.OrdinalIgnoreCase))
                                {
                                    string[] segments = lines[e].Split(" ");
                                    if (segments[1] == "=")
                                    {
                                        string memoryName = segments[2];
                                        memories.ForEach(_memory =>
                                        {
                                            if (memoryName.Contains(_memory.m_Name, StringComparison.OrdinalIgnoreCase))
                                            {
                                                offset = _memory.m_Start + _memory.m_Size;
                                                if (oldOffset == 0)
                                                {
                                                    oldOffset = _memory.m_Start;
                                                }
                                                oldOffset += memory.m_Size;
                                                start = _memory.m_Start;
                                                size = _memory.m_Size;
                                                memory = _memory;
                                            }
                                        });
                                    }
                                }
                                else if (lines[e].StartsWith("start-symbol", StringComparison.OrdinalIgnoreCase))
                                {
                                    string[] segments = lines[e].Split(" ");
                                    if (segments[1] == "=")
                                    {
                                        startSymbol = segments[2].Trim();
                                    }
                                }
                                else if (lines[e].StartsWith("end-symbol", StringComparison.OrdinalIgnoreCase))
                                {
                                    string[] segments = lines[e].Split(" ");
                                    if (segments[1] == "=")
                                    {
                                        endSymbol = segments[2].Trim();
                                    }
                                }
                                else if (lines[e].StartsWith("start", StringComparison.OrdinalIgnoreCase))
                                {
                                    string[] segments = lines[e].Split(" ");
                                    if (segments[1] == "=")
                                    {
                                        if (segments[2] == ".")
                                        {
                                            start = offset;
                                            size = 0;
                                        }
                                    }
                                }
                                else if (lines[e].StartsWith("file", StringComparison.OrdinalIgnoreCase))
                                {
                                    string[] segments = lines[e].Split(" ");
                                    if (segments[1] == "=")
                                    {
                                        file = segments[2].Trim().Replace("\"", "");
                                    }
                                }
                            }

                            sections.Add(new Section()
                            {
                                OutputFile = file,
                                m_Name = name,
                                m_Size = size,
                                m_Start = start,
                                m_Memory = memory,
                            });

                            if (startSymbol != "")
                            {
                                m_Linker.m_Labels.Add(new Label()
                                {
                                    m_Address = start,
                                    m_IsGlobal = true,
                                    m_Section = sections.Last(),
                                    m_Name = startSymbol,
                                    m_File = file
                                });
                            }
                            else
                            {
                                m_Linker.m_Labels.Add(new Label()
                                {
                                    m_Address = start,
                                    m_IsGlobal = true,
                                    m_Section = sections.Last(),
                                    m_Name = $"__START_{name}__".ToUpper(),
                                    m_File = file
                                });
                            }
                            if (endSymbol != "")
                            {
                                m_Linker.m_Labels.Add(new Label()
                                {
                                    m_Address = offset,
                                    m_Section = sections.Last(),
                                    m_IsGlobal = true,
                                    m_Name = endSymbol,
                                    m_File = file
                                });
                            }
                            else
                            {
                                m_Linker.m_Labels.Add(new Label()
                                {
                                    m_Address = offset + start,
                                    m_IsGlobal = true,
                                    m_Section = sections.Last(),
                                    m_Name = $"__END_{name}__".ToUpper(),
                                    m_File = file
                                });
                            }
                            m_Linker.m_Labels.Add(new Label()
                            {
                                m_Address = offset,
                                m_IsGlobal = true,
                                m_Section = sections.Last(),
                                m_Name = $"__SIZE_{name}__".ToUpper(),
                                m_File = file
                            });
                        }
                    }
                }
                else if (line.Contains("OUTPUTFORMAT", StringComparison.OrdinalIgnoreCase))
                {
                    string[] elements = line.Split("\"");
                    string format = elements[1];
                    if (format.StartsWith("binary", StringComparison.OrdinalIgnoreCase))
                    {
                        LinkerSettings.OutputFormat = OutputFormats.bin;
                    }
                    if (format.StartsWith("fbinary", StringComparison.OrdinalIgnoreCase))
                    {
                        LinkerSettings.OutputFormat = OutputFormats.fbin;
                    }
                }
                else if (line.Contains("MAP", StringComparison.OrdinalIgnoreCase))
                {
                    string[] elements = line.Split("\"");
                    LinkerSettings.MapFilePath = elements[1];
                }
            }

            m_Linker.m_Sections = sections;
        }
        int parseTerm(string term)
        {
            if (term.StartsWith("0x"))
            {
                string hexTerm = term.Substring(2);
                int result = Convert.ToInt32(hexTerm, 16);
                return result;
            }

            return 0;
        }
    }
}
