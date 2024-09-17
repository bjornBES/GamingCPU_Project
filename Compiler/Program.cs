using System;

public class Program : ArgumentFunctions
{
    Dictionary<string, VoidFunction> Arguments = new Dictionary<string, VoidFunction>()
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
        DecodeArguments(args);

        if (InputFile == "\0" || !File.Exists(InputFile))
        {
            Console.WriteLine("ERORR: invalid input file");
            Environment.Exit(1);
        }

        string FullSrc = "";

        for (int f = 0; f < Files.Count; f++)
        {
            string FileContents = File.ReadAllText(Files[f].FullName);

            FileContents = FileContents.Replace("\r\n", "\n");

            // pre compile

            //FullSrc += $"\n.newfile {Files[f].Name}\n" + FileContents;
            FullSrc += $"\n" + FileContents;
        }

        FullSrc = FullSrc.Replace("\r\n", "\n");

        CompilerCCL compiler = new CompilerCCL(FullSrc);
        File.WriteAllLines(OutputFile, compiler.m_output);
    }

    static int i;
    void DecodeArguments(string[] args)
    {
        for (i = 0; i < args.Length; i++)
        {
            if (Arguments.ContainsKey(args[i]))
            {
                Arguments[args[i]](args, ref i);
            }
            else
            {
                Files.Add(new FileInfo(Path.GetFullPath(args[i])));
            }
        }
    }
}
