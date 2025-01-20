using System;
using System.Text.RegularExpressions;

public class Program : ArgumentFunctions
{
    Dictionary<string, VoidFunction> m_arguments = new Dictionary<string, VoidFunction>()
    {
        {"-h", ShowHelp },
        
        {"-i", GetInputFile },

        {"-o", GetOutputFile },

        {"-d", SetDebug },

        {"-cpu", SetCPUType },
    };

    public delegate void VoidFunction();
    public static void Main(string[] args)
    {
        new Program(args);
    }
    Program(string[] _args)
    {
        args = _args;

        Console.WriteLine($"Compiled CPU Language BCG 16-bit Compiler");
        Console.WriteLine($"Verison 1.0 {Environment.NewLine}");

        decodeArguments(_args);

        if (m_InputFile == "\0" || !File.Exists(m_InputFile))
        {
            Console.WriteLine($"ERORR: invalid input file");
            Environment.Exit(1);
        }

        string FullSrc = includeFiles(new FileInfo(m_InputFile));

        FullSrc = FullSrc.Replace("\r\n", "\n");

        CompilerCCL compiler = new CompilerCCL(FullSrc);
        if (m_DoWriteOut == true)
        {
            File.WriteAllLines(m_OutputFile, compiler.m_Output);
        }
        CompilerMessages.DisplayMessages();
        Console.WriteLine("");
    }
    string includeFiles(FileInfo file)
    {
        string text = "#       :inc";
        string[] src;
        string outputSrc = "";

        src = File.ReadAllText(file.FullName).Split(Environment.NewLine);
        outputSrc += $"FILE \"{file.FullName}\"{Environment.NewLine}";

        for (int a = 0; a < src.Length; a++)
        {
            if (src[a].StartsWith(text))
            {
                src[a] = src[a].Replace(text + " ", "");
                src[a] = src[a].Trim('\"');
                src[a] = src[a].Replace("./", "");
                string inputFile = m_InputFile.Split("/").Last();
                string currFileDir = m_InputFile.Replace(inputFile, "");
                string path = Path.Combine(currFileDir, src[a]);
                if (File.Exists(path))
                {
                    FileInfo fileInfo = new FileInfo(path);
                    if (!m_Files.Contains(fileInfo))
                    {
                        outputSrc += includeFiles(fileInfo);
                        outputSrc += $"FILE \"{file.FullName}\"";
                    }
                }
            }
            else
            {
                outputSrc += src[a] + Environment.NewLine;
            }
        }
        return outputSrc;
    }
    void decodeArguments(string[] args)
    {
        for (i = 0; i < args.Length; i++)
        {
            if (m_arguments.ContainsKey(args[i]))
            {
                m_arguments[args[i]]();
            }
            else
            {
                Console.WriteLine($"invalid argument {args[i]}");
            }
        }
    }
}
