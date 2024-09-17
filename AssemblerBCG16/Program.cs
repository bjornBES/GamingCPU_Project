﻿using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations;

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
        
        // {"-d", SetDebugMode },
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

    public Program(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("ERROR: Include arguments");
            Environment.Exit(1);
        }

        DecodeArguments(args);

        if (InputFile == "\0")
        {
            Console.WriteLine("Error: there is no input file");
            Environment.Exit(1);
        }

        InputFile = Path.GetFullPath(InputFile);
        OutputFile = Path.GetFullPath(OutputFile);

        int index = OutputFile.IndexOf(OutputFile.Split(Path.DirectorySeparatorChar).Last()) - 1;
        string path = OutputFile.Substring(0, index);

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(OutputFile.Substring(0, index)).Create();
        }

        if (!File.Exists(OutputFile))
        {
            File.Create(OutputFile).Close();
        }

        includeFiles();

        new Instructions();

        Assembler assembler = new Assembler();
        string FullSrc = "";
        List<List<string>> AssemblerOutput = new List<List<string>>();
        for (int i = 0; i < Files.Count; i++)
        {
            string filename = Path.GetFullPath(Files[i].FullName);

            string src = $"{Environment.NewLine}.newfile {filename}{Environment.NewLine}{File.ReadAllText(Files[i].FullName)}";
            src = src.Replace(".String", ".Str");
            if (Files[i].Extension == ".bin")
            {
                string TempSrc = ".byte ";
                byte[] bytes = File.ReadAllBytes(Files[i].FullName);
                for (int a = 0; a < bytes.Length; a++)
                {
                    TempSrc += "0x" + Convert.ToString(bytes[a], 16).PadLeft(2, '0') + ", ";
                }
                TempSrc = TempSrc.TrimEnd(' ').TrimEnd(',');
                src = $"{Environment.NewLine}.newfile {filename}{Environment.NewLine}{TempSrc}";
                src = src.Replace(".String", ".Str");
            }
            FullSrc += src;


            assembler.Build(src);
            if (Assembler.m_WriteOut)
            {
                AssemblerOutput.Add(assembler.m_output);
            }
        }

        if (Assembler.m_WriteOut)
        {
            for (int i = 0; i < Files.Count; i++)
            {
                string filename = Path.GetFullPath(Files[i].FullName);

                if (filename == InputFile)
                {
                    File.WriteAllLines(OutputFile, AssemblerOutput[i]);
                }
                else
                {
                    string OutputPath = GetPath(OutputFile);
                    File.WriteAllLines($"{OutputPath}{Path.DirectorySeparatorChar}{Files[i].Name.Split('.').First()}.o", AssemblerOutput[i]);
                }
            }
        }

        File.WriteAllText("./PreAssemblerSrc.txt", FullSrc);
    }

    void DecodeArguments(string[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            if (m_Arguments.ContainsKey(args[i]))
            {
                m_Arguments[args[i]](args, ref i);
            }
            else
            {
                Files.Add(new FileInfo(Path.GetFullPath(args[i])));
            }
        }
    }

    void includeFiles()
    {
        for (int f = 0; f < Files.Count; f++)
        {
            string FileContents = File.ReadAllText(Files[f].FullName);

            string[] src = FileContents.Split(Environment.NewLine);

            for (int i = 0; i < src.Length; i++)
            {
                if (src[i].StartsWith("; "))
                {
                    continue;
                }

                if (src[i].StartsWith(".includeil"))
                {
                    string BasePath = src[i].Split(".includeil ").Last().Trim('\"');
                    Files.Add(new FileInfo(FindPath(BasePath, f)));
                }
                else if (src[i].StartsWith(".include") && !src[i].StartsWith(".includeil"))
                {
                    string BasePath = src[i].Split(".include ").Last().Trim('\"');
                    Files.Add(new FileInfo(FindPath(BasePath, f)));
                }
            }
        }
    }
    string GetPath(string path)
    {
        string C_path = Path.GetFullPath(path);
        int index = C_path.IndexOf(C_path.Split(Path.DirectorySeparatorChar).Last()) - 1;
        return C_path.Substring(0, index);
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

        return NewBasePath;
    }
}
