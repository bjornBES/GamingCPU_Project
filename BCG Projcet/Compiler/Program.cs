using System;
using System.Text.RegularExpressions;

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

        string FullSrc = includeFiles(new FileInfo(m_InputFile));

        /*
        for (int f = 0; f < m_Files.Count; f++)
        {
            string FileContents = File.ReadAllText(m_Files[f].FullName);

            // pre compile

            //FullSrc += $"\n.newfile {Files[f].Name}\n" + FileContents;
            FullSrc += $"{Environment.NewLine}" + FileContents;
        }
         */

        FullSrc = FullSrc.Replace("\r\n", "\n");

        CompilerCCL compiler = new CompilerCCL(FullSrc);
        File.WriteAllLines(m_OutputFile, compiler.m_Output);
    }
    string includeFiles(FileInfo file)
    {
        string[] src;
        string outputSrc = "";

        src = File.ReadAllText(file.FullName).Split(Environment.NewLine);
        outputSrc += $"FILE \"{file.FullName}\"";

        for (int a = 0; a < src.Length; a++)
        {
            if (src[a].StartsWith("#inc"))
            {
                string inputFile = m_InputFile.Split("/").Last();
                string currFileDir = m_InputFile.Replace(inputFile, "");
                src[a] = currFileDir + Regex.Replace(src[a], @" +", " ").Replace("#inc ", "").Trim('\"').Substring(2);
                if (File.Exists(src[a]))
                {
                    FileInfo fileInfo = new FileInfo(src[a]);
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
