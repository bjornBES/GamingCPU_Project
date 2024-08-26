using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BCG16CPUEmulator
{
    public class Program : ArgumentFunctions
    {
        public Dictionary<string, VoidFunction> m_Arguments = new Dictionary<string, VoidFunction>()
        {
            {"-i", GetInputFile },
            // {"-o", GetOutputFile },
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

            InputFile = Path.GetFullPath(InputFile);
            
            BUS bUS = new BUS();

            bUS.Reset();

            string BinFileContents = File.ReadAllText(InputFile, Encoding.Default);

            bUS.Load(BinFileContents);

            bUS.Tick();

            int start = (int)(0x000F_FF00 - Memory.MemBankSize);
            int end = (int)(0x000F_FFFF - Memory.MemBankSize);

            byte[] array = bUS.m_Memory.Mem[start..end];
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
