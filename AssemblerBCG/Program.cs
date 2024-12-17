using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations;
using AssemblerBCG;
using System.ComponentModel;

public class Program : ArgumentFunctions
{
    public Dictionary<string, VoidFunction> m_Arguments = new Dictionary<string, VoidFunction>()
    {
        {"-h", PrintHelp },
        {"-help", PrintHelp },

        {"-i", GetInputFile },
        {"-o", GetOutputFile },

        {"-f", SetFormatOutput },
        {"-fobj", SetFormatOBJ },

        // {"-mm", SetMemoryModel },

        {"-cpu", SetCPUType },

        {"-text-name",  SetTextSegmentName },
        {"-textname",   SetTextSegmentName },
        {"-tn",         SetTextSegmentName },

        {"-data-name",  SetDataSegmentName },
        {"-dataname",   SetDataSegmentName },
        {"-dn",         SetDataSegmentName },

        {"-rodata-name",SetRdataSegmentName },
        {"-rodataname", SetRdataSegmentName },
        {"-rn",         SetRdataSegmentName },

        {"-bss-name",   SetBSSSegmentName },
        {"-bssname",    SetBSSSegmentName },
        {"-bn",         SetBSSSegmentName },

        {"-d", SetDebugMode },
        {"-debug", SetDebugMode },
        // {"-p", SetProjectPath },
        
        {"-df", SetErrorFile },
        {"-fr", SetErrorFile },
        {"-fi", SetErrorFile },
    };
    public delegate void VoidFunction(string[] args, ref int i);
    static int Main(string[] args)
    {
        new Program(args);
        return 0;
    }

    public void ParseAddress(string term, SizeAlignment sizeAlignment, out OperandArgument[] operandArgument)
    {
        List<OperandArgument> operands = new List<OperandArgument>();

        // segments [RegS:DATA] / [{RegS:}IMMDATA]
        // address SpiltPoint(HIGH, LOW, none) length(near, short, long, far) DATA(R/K)

        if (term.StartsWith('[') && term.EndsWith(']'))
        {
            term = term.Trim('[', ']');
            if (term.Contains(':'))
            {
                string srgmentstr = term.Split(':')[0];
                string offsetstr = term.Split(':')[1];
                Register segment = Enum.Parse<Register>(srgmentstr, true);
                if (!Enum.TryParse(offsetstr, true, out Register offset))
                {
                    operandArgument = operands.ToArray();
                    return;
                }

                if (sizeAlignment == SizeAlignment._word)
                {
                    if (segment == Register.DS)
                    {
                        if (offset == Register.B)
                        {
                            operands.Add(new OperandArgument()
                            {
                                ArgumentMode = ArgumentMode.RM,
                                data = "00",
                                IsRawData = true,
                                Size = sizeAlignment
                            });
                            operands.Add(new OperandArgument()
                            {
                                ArgumentMode = ArgumentMode.MOD,
                                data = "0000",
                                IsRawData = true,
                                Size = sizeAlignment
                            });
                        }
                    }
                    else if (segment == Register.ES)
                    {
                        if (offset == Register.B)
                        {
                            operands.Add(new OperandArgument()
                            {
                                ArgumentMode = ArgumentMode.RM,
                                data = "00",
                                IsRawData = true,
                                Size = sizeAlignment
                            });
                            operands.Add(new OperandArgument()
                            {
                                ArgumentMode = ArgumentMode.MOD,
                                data = "0011",
                                IsRawData = true,
                                Size = sizeAlignment
                            });
                        }
                    }
                    else
                    {
                        operands.Add(new OperandArgument()
                        {
                            ArgumentMode = ArgumentMode.RM,
                            data = "00",
                            IsRawData = true,
                            Size = sizeAlignment
                        });
                        operands.Add(new OperandArgument()
                        {
                            ArgumentMode = ArgumentMode.MOD,
                            data = "0101",
                            IsRawData = true,
                            Size = sizeAlignment
                        });
                        operands.Add(new OperandArgument()
                        {
                            ArgumentMode = ArgumentMode.Sregister,
                            data = Convert.ToString((byte)Enum.Parse<SRegisterWord>(srgmentstr, true), 16).PadLeft(3, '0'),
                            IsRawData = true,
                            Size = sizeAlignment
                        });
                        operands.Add(new OperandArgument()
                        {
                            ArgumentMode = ArgumentMode.register,
                            data = Convert.ToString((byte)offset, 16).PadLeft(8, '0'),
                            IsRawData = true,
                            Size = sizeAlignment
                        });
                    }
                }
            }
            else
            {
                if (!Enum.TryParse(term, true, out Register offset))
                {

                }
            }
        }

        operandArgument = operands.ToArray();
        return;
    }

    public Program(string[] args)
    {
        /*
        ParseAddress("[DS:B]", SizeAlignment._word, out OperandArgument[] operandArgument);
        ParseAddress("[ES:B]", SizeAlignment._word, out operandArgument);
        ParseAddress("[FS:B]", SizeAlignment._word, out operandArgument);
        ParseAddress("[DS:0x7FFF]", SizeAlignment._word, out operandArgument);
        ParseAddress("[0x7FFF]", SizeAlignment._word, out operandArgument);
        ParseAddress("[B]", SizeAlignment._word, out operandArgument);

        new Instructions();

        InstructionArgumentInfo argumentInfo1 = new InstructionArgumentInfo(Instruction.MOV, ArgumentMode.GPregister, ArgumentMode.immediate);
        InstructionArgumentInfo argumentInfo2 = new InstructionArgumentInfo(Instruction.MOV, ArgumentMode.GPregister, ArgumentMode.register);
        InstructionArgumentInfo argumentInfo3 = new InstructionArgumentInfo(Instruction.MOV, ArgumentMode.GPregister, ArgumentMode.RM, ArgumentMode.MOD);
        InstructionInfo instructionInfo = Instructions.m_Instr[argumentInfo1];

        OperandArgument operand1 = new OperandArgument()
        {
            ArgumentMode = ArgumentMode.GPregister,
            data = "01",
            IsRawData = true,
            Size = SizeAlignment._dword,
        };
        OperandArgument operand2 = new OperandArgument()
        {
            ArgumentMode = ArgumentMode.immediate,
            data = "55AA55AA",
            IsRawData = true,
            Size = SizeAlignment._dword,
        };

        string InstrBin = instructionInfo.GenInstr(SizeAlignment._dword, operand1, operand2);
        string InstrHex = HexLibrary.HexConverter.ToHexString(InstrBin, 2);
        PrintInstr(argumentInfo1, InstrHex);

        instructionInfo = Instructions.m_Instr[argumentInfo2];
        operand1 = new OperandArgument()
        {
            ArgumentMode = ArgumentMode.GPregister,
            data = "01",
            Size = SizeAlignment._word,
        };
        operand2 = new OperandArgument()
        {
            ArgumentMode = ArgumentMode.register,
            data = Convert.ToString((byte)Register.B, 16).PadLeft(2, '0'),
            IsRawData = true,
            Size = SizeAlignment._word,
        };

        InstrBin = instructionInfo.GenInstr(SizeAlignment._word, operand1, operand2);
        InstrHex = HexLibrary.HexConverter.ToHexString(InstrBin, 2);
        PrintInstr(argumentInfo2, InstrHex);

        instructionInfo = Instructions.m_Instr[argumentInfo3];
        operand1 = new OperandArgument()
        {
            ArgumentMode = ArgumentMode.GPregister,
            data = "01",
            Size = SizeAlignment._word,
        };
        operand2 = new OperandArgument()
        {
            ArgumentMode = ArgumentMode.RM,
            data = "00",
            IsRawData = true,
            Size = SizeAlignment._word,
        };
        OperandArgument operand3 = new OperandArgument()
        {
            ArgumentMode = ArgumentMode.MOD,
            data = "0000",
            IsRawData = true,
            Size = SizeAlignment._word,
        };

        InstrBin = instructionInfo.GenInstr(SizeAlignment._word, operand1, operand2, operand3);
        InstrHex = HexLibrary.HexConverter.ToHexString(InstrBin, 2);
        PrintInstr(argumentInfo3, InstrHex);
         */
        if (args.Length == 0)
        {
            Console.WriteLine("ERROR: Include arguments");
            Environment.Exit(1);
        }

        decodeArguments(args);

        if (m_InputFile == "\0")
        {
            Console.WriteLine("Error: there is no input file");
            Environment.Exit(1);
        }

        Console.WriteLine($"BCGAssembler version {version}");

        m_InputFile = Path.GetFullPath(m_InputFile);
        m_OutputFile = Path.GetFullPath(m_OutputFile);

        int index = m_OutputFile.IndexOf(m_OutputFile.Split(Path.DirectorySeparatorChar).Last()) - 1;
        string path = m_OutputFile.Substring(0, index);

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(m_OutputFile.Substring(0, index)).Create();
        }

        if (!File.Exists(m_OutputFile))
        {
            File.Create(m_OutputFile).Close();
        }

        FileInfo startFile = new FileInfo(m_InputFile);
        string FullSrc = includeFiles(startFile, out FileInfo[] fileInfo);
        m_Files.AddRange(fileInfo);

        new OldInstructions();

        Assembler assembler = new Assembler();
        List<List<string>> AssemblerOutput = new List<List<string>>();
        for (int i = 0; i < m_Files.Count; i++)
        {
            if (fileInfo.Contains(m_Files[i]))
            {
                continue;
            }
            string filename = Path.GetFullPath(m_Files[i].FullName);


            assembler.Start(FullSrc, filename, i == m_Files.Count - 1);
            if (AssemblerVariabels.m_WriteOut)
            {
                AssemblerOutput.Add(assembler.m_Output);
            }
        }

        if (AssemblerVariabels.m_WriteOut)
        {
            for (int i = 0; i < m_Files.Count; i++)
            {
                if (fileInfo.Contains(m_Files[i]))
                {
                    continue;
                }
                string filename = Path.GetFullPath(m_Files[i].FullName);

                if (filename == m_InputFile)
                {
                    File.WriteAllLines(m_OutputFile, AssemblerOutput[i]);
                }
                else
                {
                    string OutputPath = getPath(m_OutputFile);
                    File.WriteAllLines($"{OutputPath}{Path.DirectorySeparatorChar}{m_Files[i].Name.Split('.').First()}.o", AssemblerOutput[i]);
                }
            }
        }

        File.WriteAllText("./PreAssemblerSrc.txt", FullSrc);
        Console.WriteLine("");
    }

    void PrintInstr(InstructionArgumentInfo instructionInfo, string hexinstr)
    {
        Console.WriteLine($"{instructionInfo}");
        Console.WriteLine($"{hexinstr}");
    }

    void decodeArguments(string[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            if (m_Arguments.ContainsKey(args[i]))
            {
                m_Arguments[args[i]](args, ref i);
            }
            else
            {
                m_Files.Add(new FileInfo(Path.GetFullPath(args[i])));
            }
        }
    }

    string includeFiles(FileInfo file, out FileInfo[] fileInfos)
    {
        List<FileInfo> files = new List<FileInfo>();
        string result = $"{Environment.NewLine}.newfile {file.FullName}{Environment.NewLine}";
        string FileContents = File.ReadAllText(file.FullName);

        string[] src = FileContents.Split(Environment.NewLine);

        if (file.Extension == ".bin")
        {
            string TempSrc = ".byte ";
            byte[] bytes = File.ReadAllBytes(file.FullName);
            for (int a = 0; a < bytes.Length; a++)
            {
                TempSrc += "0x" + Convert.ToString(bytes[a], 16).PadLeft(2, '0') + ", ";
            }
            TempSrc = TempSrc.TrimEnd(' ').TrimEnd(',');
            result += $"{TempSrc}";
            fileInfos = Array.Empty<FileInfo>();
            return result;
        }

        for (int i = 0; i < src.Length; i++)
        {
            result += src[i] + Environment.NewLine;
            if (src[i].StartsWith("; "))
            {
                continue;
            }

            if (src[i].StartsWith(".includeil"))
            {
                string BasePath = src[i].Split(".includeil ").Last().Trim('\"');
                Console.WriteLine($"including inlined {BasePath}");
                string path = findPath(BasePath);
                includeFiles(new FileInfo(path), out FileInfo[] newFiles);
                files.Add(new FileInfo(path));
                files.AddRange(newFiles);
                result += Environment.NewLine + File.ReadAllText(path) + Environment.NewLine;
            }
            else if (src[i].StartsWith(".include") && !src[i].StartsWith(".includeil"))
            {
                string BasePath = src[i].Split(".include ").Last().Trim('\"');
                Console.WriteLine($"including {BasePath}");
                m_Files.Add(new FileInfo(findPath(BasePath)));
            }
        }

        fileInfos = files.ToArray();
        return result;
    }
    string getPath(string path)
    {
        string C_path = Path.GetFullPath(path);
        int index = C_path.IndexOf(C_path.Split(Path.DirectorySeparatorChar).Last()) - 1;
        return C_path.Substring(0, index);
    }
    string findPath(string BasePath)
    {
        string NewBasePath = "";

        for (int c = 0; c < BasePath.Length; c++)
        {
            if (BasePath[c] == '.')
            {
                c++;
                if (BasePath[c] == Path.DirectorySeparatorChar)
                {
                    string projcetPath = getPath(m_InputFile);
                    NewBasePath += projcetPath;
                    NewBasePath += Path.DirectorySeparatorChar;
                }
                else
                {
                    c--;
                    NewBasePath += '.';
                }
            }
            else
            {
                NewBasePath += BasePath[c];
            }
        }

        return NewBasePath;
    }
}
