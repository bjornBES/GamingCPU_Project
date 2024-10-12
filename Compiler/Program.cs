using System;

public class Program : ArgumentFunctions
{
    Dictionary<string, VoidFunction> m_arguments = new Dictionary<string, VoidFunction>()
    {
        {"-i", GetInputFile },

        {"-o", GetOutputFile },

        {"-e", DoNotEntry },

        {"-s", SetStartOffset },

        {"-cpu", SetCPUType },
    };

    public delegate void VoidFunction(string[] args, ref int i);
    public static void Main(string[] args)
    {
        new Program(args);
    }
    Program(string[] args)
    {
        decodeArguments(args);

        if (m_InputFile == "\0" || !File.Exists(m_InputFile))
        {
            Console.WriteLine("ERORR: invalid input file");
            Environment.Exit(1);
        }

        string FullSrc = "";

        for (int f = 0; f < m_Files.Count; f++)
        {
            string FileContents = File.ReadAllText(m_Files[f].FullName);

            FileContents = FileContents.Replace("\r\n", "\n");

            // pre compile

            //FullSrc += $"\n.newfile {Files[f].Name}\n" + FileContents;
            FullSrc += $"\n" + FileContents;
        }

        FullSrc = FullSrc.Replace("\r\n", "\n");

        CompilerCCL compiler = new CompilerCCL(FullSrc);
        File.WriteAllLines(m_OutputFile, compiler.m_Output);
    }

    static int m_i;
    void decodeArguments(string[] args)
    {
        for (m_i = 0; m_i < args.Length; m_i++)
        {
            if (m_arguments.ContainsKey(args[m_i]))
            {
                m_arguments[args[m_i]](args, ref m_i);
            }
            else
            {
                m_Files.Add(new FileInfo(Path.GetFullPath(args[m_i])));
            }
        }
    }
}
