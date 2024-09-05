using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace BCGLinker
{
    public class Program : ArgumentFunctions
    {
        public Dictionary<string, VoidFunction> m_Arguments = new Dictionary<string, VoidFunction>()
        {
            {"-i", GetInputFile },
            {"-o", GetOutputFile },
            {"-f", SetFormatType },
            {"-ffbin", SetFormatFbin },
            {"-fbin", SetFormatBin },
            {"-flib", SetFormatLib },
            // {"-d", SetDebugMode },
            // {"-p", SetProjectPath },
            // {"-df", SetErrorFile },
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

            switch (OutputFormat)
            {
                case OutputFormats.fbin:
                case OutputFormats.bin:
                    int Outindex = OutputFile.IndexOf('.', 1);
                    OutputFile = OutputFile.Remove(Outindex) + ".bin";
                    break;
                default:
                    break;
            }

            InputFile = Path.GetFullPath(InputFile);
            OutputFile = Path.GetFullPath(OutputFile);

            int index = InputFile.IndexOf(InputFile.Split(Path.DirectorySeparatorChar).Last()) - 1;
            string path = InputFile.Substring(0, index);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(InputFile.Substring(0, index)).Create();
            }

            index = OutputFile.IndexOf(OutputFile.Split(Path.DirectorySeparatorChar).Last()) - 1;
            path = OutputFile.Substring(0, index);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(OutputFile.Substring(0, index)).Create();
            }

            if (!File.Exists(OutputFile))
            {
                File.Create(OutputFile).Close();
            }

            Linker linker = new Linker();
            string FullSrc = "";
            for (int i = 0; i < Files.Count; i++)
            {
                string filename = Path.GetFullPath(Files[i].FullName);

                string src = $"{Environment.NewLine}{File.ReadAllText(filename)}";
                /*
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
                 */
                FullSrc += src;
                linker.BuildSectionStruct(src);
                linker.BuildSectionSymbols(src);
                linker.BuildSectionSection(src);
            }

            File.WriteAllText("./PreLinkerSrc.txt", FullSrc);

            linker.BuildSrc(FullSrc);

            byte[] copyOld = File.ReadAllBytes(OutputFile);

            Thread.Sleep(250);

            File.WriteAllBytes(OutputFile, linker.m_OutputBin.ToArray());
            byte[] contents = File.ReadAllBytes(OutputFile);

            if (Equals(contents, linker.m_OutputBin.ToArray()))
            {
                File.WriteAllBytes(OutputFile, copyOld);
                Console.WriteLine("Error: values does not match");
                Environment.Exit(1);
            }

            File.WriteAllLines("./Map.map", linker.GenerateMapFile());
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
    }
}
