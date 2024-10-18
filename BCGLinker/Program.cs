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
            {"-ls", SetLinkerScript },
            {"-m", SetMapFilePath }
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

            decodeArguments(args);

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

            LinkerScript linkerScript = new LinkerScript();
            linkerScript.m_Linker = linker;
            linkerScript.BuildLS(LinkerScriptPath);

            string FullSrc = "";
            List<string> src = File.ReadAllText(InputFile).Split(Environment.NewLine).ToList();
            for (int i = 0; i < Files.Count; i++)
            {
                string filename = Path.GetFullPath(Files[i].FullName);
                if (filename == InputFile)
                {
                    continue;
                }

                for (int l = 0; l < src.Count; l++)
                {
                    if (src[l].StartsWith("INCLUDE HEADER"))
                    {
                        string fileSrc = File.ReadAllText(filename);
                        string[] fileSrcLine = fileSrc.Split(Environment.NewLine);

                        int FILEIndex = 0;
                        for (int n = 0; n < fileSrcLine.Length; n++)
                        {
                            if (fileSrcLine[n].StartsWith("_FILE_"))
                            {
                                FILEIndex = n;
                                break;
                            }
                        }
                        string srcFileName = fileSrcLine[FILEIndex].Replace("_FILE_ ", "").Trim('"');

                        src.Insert(l + 1, $"INCINIL {srcFileName}{Environment.NewLine}");
                        
                        int fileTextIndex = fileSrc.IndexOf("TEXT HEADER");
                        int fileSymbolsIndex = fileSrc.IndexOf("SYMBOLS HEADER");
                        string textFileSrc = fileSrc.Substring(fileTextIndex + 12);
                        src.Add(Environment.NewLine + textFileSrc);
                    }
                    else if (src[l].StartsWith("SYMBOLS HEADER"))
                    {
                        string fileSrc = File.ReadAllText(filename);

                        int fileTextIndex = fileSrc.IndexOf("TEXT HEADER");
                        int fileSymbolsIndex = fileSrc.IndexOf("SYMBOLS HEADER");
                        string symbolsFileSrc = fileSrc.Substring(fileSymbolsIndex + 15, fileTextIndex - (fileSymbolsIndex + 15));
                        src.Insert(l + 1, symbolsFileSrc);
                    }
                }
                if (filename != InputFile)
                {
                    Files.RemoveAt(i);
                }

                /*
                linker.BuildSectionInclude(src);
                linker.BuildSectionSymbols(src);
                linker.BuildLabels(src);

                int indexOfText = src.IndexOf("TEXT HEADER");

                src = src.Substring(indexOfText);

                FullSrc += src;
                 */
            }

            for (int i = 0; i < src.Count; i++)
            {
                FullSrc += src[i] + Environment.NewLine;
            }

            FullSrc = FullSrc.Trim('\n');
            FullSrc = FullSrc.Trim('\r');


            for (int i = 0; i < Files.Count; i++)
            {
                linker.BuildSectionSymbols(FullSrc);
                linker.BuildSectionInclude(FullSrc);
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

            if(MapFilePath != "")
            {
                File.WriteAllLines(MapFilePath, linker.GenerateMapFile());
            }
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
                    Files.Add(new FileInfo(Path.GetFullPath(args[i])));
                }
            }
        }
    }
}
