using System;

public class Program : ArgumentFunctions
{
    Dictionary<string, VoidFunction> Arguments = new Dictionary<string, VoidFunction>()
    {
        {"-i", GetInputFile },
        {"-o", GetOutputFile },
        {"-e", DoNotEntry },
        {"-s", SetStartOffset }
    };

    public delegate void VoidFunction(string[] args, ref int i);
    public static void Main(string[] args)
    {
        new Program(args);
    }
    Program(string[] args)
    {
        DecodeArguments(args);

        if (CompilerSettings.InputFile == "\0" || !File.Exists(CompilerSettings.InputFile))
        {
            Console.WriteLine("ERORR: invalid input file");
            Environment.Exit(1);
        }

        string FullSrc = "";

        for (int f = 0; f < CompilerSettings.Files.Count; f++)
        {
            string FileContents = File.ReadAllText(CompilerSettings.Files[f].FullName);

            FileContents = FileContents.Replace("\r\n", "\n");

            // pre compile

            //FullSrc += $"\n.newfile {CompilerSettings.Files[f].Name}\n" + FileContents;
            FullSrc += $"\n" + FileContents;
        }

        FullSrc = FullSrc.Replace("\r\n", "\n");

        CompilerCCL compiler = new CompilerCCL(FullSrc);
        File.WriteAllLines(CompilerSettings.OutputFile, compiler.m_output);
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
                CompilerSettings.Files.Add(new FileInfo(Path.GetFullPath(args[i])));
            }
        }
    }
}
