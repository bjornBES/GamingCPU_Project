using HLCLCompiler;
using HLCLCompiler.CodeGeneration;
using HLCLCompiler.Tokenizer;
using System.Text.Json;
using System.Text.RegularExpressions;

public class Program : ArgumentFunctions
{
    Dictionary<string, VoidFunction> m_arguments = new Dictionary<string, VoidFunction>()
    {
        {"-i", GetInputFile },

        {"-o", GetOutputFile },

        {"-e", DoNotEntry },

        {"-cpu", SetCPUType },

        {"-d", Debug },
    };

    public delegate void VoidFunction(string[] args);

    private static void Main(string[] args)
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

        m_InputFile = Path.GetFullPath(m_InputFile);

        string inputFileName = m_InputFile.Split(Path.DirectorySeparatorChar).Last();
        m_ProjectPath = m_InputFile.Replace($"{Path.DirectorySeparatorChar}{inputFileName}", "");

        string FullSrc = includeFiles(new FileInfo(m_InputFile));
        FullSrc = FullSrc.Replace("\r\n", "\n");

        File.WriteAllText("./src/HLCL/FullSrc.txt", FullSrc);

        Tokenizer tokenizer = new Tokenizer();
        Token[] tokens = tokenizer.Tokenize(FullSrc);
        string json = JsonSerializer.Serialize(tokens, new JsonSerializerOptions()
        {
            WriteIndented = true
        });
        File.WriteAllText("./src/HLCL/token.json", json);

        Parser parser = new Parser();
        ProgNode progNode = parser.Build(tokens);

        json = JsonSerializer.Serialize(progNode, new JsonSerializerOptions()
        {
            WriteIndented = true
        });
        File.WriteAllText("./src/HLCL/AST.json", json);

        Generator generator = new Generator();
        generator.gen_prog(progNode);

        File.WriteAllLines(m_OutputFile, generator.outputCode);
    }
    string includeFiles(FileInfo file)
    {
        string[] src;
        string outputSrc = "";

        src = File.ReadAllText(file.FullName).Split(Environment.NewLine);

        for (int a = 0; a < src.Length; a++)
        {
            if (src[a].StartsWith("#include"))
            {
                string line = src[a].Replace("#include ", "");

                if (line.StartsWith('\"'))
                {
                    line = line.Trim('\"');
                    string path = parsePath(line);
                    if (File.Exists(path))
                    {
                        FileInfo fileInfo = new FileInfo(path);
                        if (!m_Files.Contains(fileInfo))
                        {
                            // outputSrc += $"FILE \"{fileInfo.FullName}\"";
                            outputSrc += includeFiles(fileInfo);
                            // outputSrc += $"FILE \"{file.FullName}\"";
                            m_Files.Add(fileInfo);
                        }
                    }
                }
                else if (line.StartsWith('<'))
                {

                }
            }
            else
            {
                outputSrc += src[a] + Environment.NewLine;
            }
        }
        return outputSrc;
    }
    string parsePath(string path)
    {
        string result = "";

        for (int i = 0; i < path.Length;)
        {
            if (path[i] == '.')
            {
                i++;
                if (path[i] == '/')
                {
                    i++;
                    result += m_ProjectPath + Path.DirectorySeparatorChar;
                }
                else
                {
                    result += path[i - 1];
                }
            }
            else
            {
                result += path[i];
                i++;
            }
        }

        return result;
    }
    void decodeArguments(string[] args)
    {
        for (i = 0; i < args.Length; i++)
        {
            if (m_arguments.ContainsKey(args[i]))
            {
                m_arguments[args[i]](args);
            }
            else
            {
                m_Files.Add(new FileInfo(Path.GetFullPath(args[i])));
            }
        }
    }
}