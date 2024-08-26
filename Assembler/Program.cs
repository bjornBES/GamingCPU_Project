using static assembler.AssemblerSettings;
using assembler;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

class Program
{
    public Dictionary<string, VoidFunction> m_Arguments = new Dictionary<string, VoidFunction>()
    {
        {"-i", GetInputFile },
        {"-o", GetOutputFile },
        {"-d", SetDebugMode },
        {"-p", SetProjectPath },
        {"-df", SetErrorFile },
    };
    public delegate void VoidFunction(string[] args);
    public static int Main(string[] args)
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

        if (InputFile == "\0" || !File.Exists(InputFile))
        {
            Console.WriteLine("ERORR: invalid input file");
            Environment.Exit(1);
        }

        if (ProjectPath == null)
        {
            ProjectPath = Path.GetFullPath("./").Replace("\\", "/");
        }

        if (PrintIntoErrorFile)
        {
            File.WriteAllText(ErrorFile, "");
        }

        int index = OutputFile.IndexOf(OutputFile.Split('/', '\\').Last()) - 1;
        string path = OutputFile.Substring(0, index);

        if (!Directory.Exists(path))
        { 
            Directory.CreateDirectory(OutputFile.Substring(0, index)).Create();
        }

        File.Create(OutputFile).Close();

        for (int f = 0; f < Files.Count; f++)
        {
            string FileContents = File.ReadAllText(Files[f].FullName);

            FileContents = FileContents.Replace("\r\n", "\n");
            if (FileContents.Contains(".includeil"))
            {
                string[] splited = FileContents.Split('\n');
                for (int i = 0; i < splited.Length; i++)
                {
                    if (!splited[i].Contains(".includeil")) continue;
                    string FilePath = splited[i].Split(".includeil ").Last().Trim('\"').Replace('\\', '/');
                    Files.Add(new FileInfo(FilePath));
                }
            }
            if (FileContents.Contains(".include") && !FileContents.Contains(".includeil"))
            {
                string[] splited = FileContents.Split('\n');
                for (int i = 0; i < splited.Length; i++)
                {
                    if (!splited[i].Contains(".include")) continue;
                    string FilePath = splited[i].Split(".include ").Last().Trim('\"').Replace('\\', '/');
                    Files.Add(new FileInfo(FilePath));
                }
            }
        }

        string FullSrc = "";
        List<string[]> bin_output = new List<string[]>();
        List<string> bin_output_src = new List<string>();
        List<string> AssemblerTokens = new List<string>();
        Tokenizer tokenizer = new Tokenizer();
        Generator generator = new Generator();
        for (int i = 0; i < Files.Count; i++)
        {
            string src = "\n.newfile " + Files[i].FullName.Replace("\\", "/").Replace(ProjectPath, "./") + "\n" + File.ReadAllText(Files[i].FullName);
            src = src.Replace("\r\n", "\n");
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
                src = "\n.newfile " + Files[i].FullName.Replace("\\", "/").Replace(ProjectPath, "./") + "\n" + TempSrc;
                src = src.Replace("\r\n", "\n");
                src = src.Replace(".String", ".Str");
            }
            FullSrc += src;
            tokenizer.Build(src);
        }

        for (int i = 0; i < tokenizer.m_tokens.Count; i++)
        {
            AssemblerTokens.Add($"{i}".PadLeft(4, '0') + "\t" + tokenizer.m_tokens[i].ToString());
        }

        generator.gen_prog(tokenizer.m_tokens.ToArray(), FullSrc);

        for (int i = 0; i < generator.m_OutputBin.Count; i++)
        {
            bin_output_src.AddRange(generator.m_OutputBin[i]);
        }

        File.WriteAllLines("./AssemblerTokens.txt", AssemblerTokens);
        File.WriteAllText("./PreAssemblerSrc.txt", FullSrc);
        File.WriteAllLines("./PostAssemblerSrc.txt", bin_output_src);

        //Environment.Exit(0);


        Linker linker = new Linker();
        linker.BuildSrc(generator, generator.m_OutputBin);
        File.WriteAllLines(OutputFile, linker.m_Output);
    }

    static int s_i;
    void DecodeArguments(string[] args)
    {
        for (s_i = 0; s_i < args.Length; s_i++)
        {
            if (m_Arguments.ContainsKey(args[s_i]))
            {
                m_Arguments[args[s_i]](args);
            }
            else
            {
                Files.Add(new FileInfo(Path.GetFullPath(args[s_i])));
            }
        }
    }

    static void GetInputFile(string[] args)
    {
        s_i++;
        InputFile = args[s_i];
        Files.Add(new FileInfo(Path.GetFullPath(args[s_i])));
    }
    static void GetOutputFile(string[] args)
    {
        s_i++;
        OutputFile = Path.GetFullPath(args[s_i]);
    }
    static void SetDebugMode(string[] args)
    {
        Debug = true;
        if (args.Length > s_i + 1 && char.IsDigit(args[s_i + 1][0]))
        {
            s_i++;
            DebugLevel = int.Parse(args[s_i]);
            if (DebugLevel > 5)
            {
                DebugLevel = 5;
                Console.WriteLine("Debug Level can't be over 5");
            }
        }
        else
        {
            DebugLevel = 1;
        }
    }
    static void SetProjectPath(string[] args)
    {
        Debug = true;
        if (args.Length > s_i + 1)
        {
            s_i++;
            ProjectPath = args[s_i];
        }
    }
    static void SetErrorFile(string[] args)
    {
        s_i++;
        PrintIntoErrorFile = true;
        if (File.Exists(args[s_i]))
        {
            ErrorFile = args[s_i];
        }
        else
        {
            ErrorFile = args[s_i];
            File.Create(ErrorFile).Close();
        }
    }
    void BinaryToDateAndTime()
    {
        string input = "11110000001100000001100000000000101000001011000011101100";

        // Split input into date and time parts
        string datePart = input.Substring(0, 24);
        string timePart = input.Substring(24);

        //01234567 89012345 67890123 01234567 89012345 67890123 45678901
        //00000000 00111111 11112222 00000000 00111111 11112222 22222233
        //DDDDDUUU MMMMUUUU YYYYYYYY FFFFFFFF HHHHHUUU mmmmmUUU SSSSSSUU
        //11110000 00110000 00011000 00000000 10100000 10110000 11101100
        //11110000 00110000 00011000                                     datePart
        //                           00000000 10100000 10110000 11101100 timePart

        // Decode date
        string S_day = datePart.Substring(0, 5);
        int day = Convert.ToInt32(S_day, 2);
        string S_month = datePart.Substring(8, 4);
        int month = Convert.ToInt32(S_month, 2);
        string S_year = datePart.Substring(16, 8);
        int year = Convert.ToInt32(S_year, 2);

        // Decode time
        string S_flags = timePart.Substring(0, 8);
        int flags = Convert.ToInt32(S_flags, 2);
        string S_hour = timePart.Substring(8, 5);
        int hour = Convert.ToInt32(S_hour, 2);
        string S_minute = timePart.Substring(16, 5);
        int minute = Convert.ToInt32(S_minute, 2);
        string S_second = timePart.Substring(24, 6);
        int second = Convert.ToInt32(S_second, 2);

        // Create DateTime object
        DateTime dateTime = new DateTime(year, month, day, hour, minute, second);
        Console.WriteLine("Decoded Date and Time: " + dateTime);
    }
}
