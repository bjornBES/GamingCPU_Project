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

            byte[] BinFileContents = File.ReadAllBytes(InputFile);

            bUS.Load(BinFileContents);

            while (bUS.m_CPU.GetFlag(BCG16CPU_Registers.FH) == false)
            {
                bUS.Tick();
            }

            WriteInfo(bUS.m_CPU);
        }

        void WriteInfo(BCG16CPU cpu)
        {
            Console.WriteLine($"A:\t0x{cpu.A.ToHex()}" + $" B:\t0x{cpu.B.ToHex()}");
            Console.WriteLine($"C:\t0x{cpu.C.ToHex()}" + $" D:\t0x{cpu.D.ToHex()}");
            Console.WriteLine($"");
            Console.WriteLine($"HL:\t0x{cpu.H.ToHex()}:{cpu.L.ToHex()}");
            Console.WriteLine($"");
            Console.WriteLine($"DS:\t0x{cpu.DS.ToHex()} ES:\t0x{cpu.ES.ToHex()}");
            Console.WriteLine($"SS:\t0x{cpu.SS.ToHex()} S:\t0x{cpu.S.ToHex()}");
            Console.WriteLine($"");
            Console.WriteLine($"PC:\t0x{cpu.PC.ToHex()}");
            Console.WriteLine($"");
            Console.WriteLine($"SP:\t0x{cpu.SP.ToHex()}" + $" BP:\t0x{cpu.BP.ToHex()}");
            Console.WriteLine($"");
            Console.WriteLine($"AF:\t{cpu.AF.m_Value}f".PadRight(11, ' ') + $"BF:\t{cpu.BF.m_Value}f");
            Console.WriteLine($"");
            Console.WriteLine($"R1:\t0x{cpu.R1.ToHex()}" + $" R2:\t0x{cpu.R2.ToHex()}");
            Console.WriteLine($"");
            Console.WriteLine($"MB:\t0x{cpu.MB.ToHex()}");
            Console.WriteLine($"");
            Console.WriteLine($"CR0:\t0b{cpu.CR0.ToBin()}" + $" CR1:\t0b{cpu.CR1.ToBin()}");
            Console.WriteLine($"");
            Console.WriteLine($"\t  F     F    FFFFFF");
            Console.WriteLine($"\t  H     I    LOCSEZ");
            Console.WriteLine($"\t  A     E    EFYIQO");
            Console.WriteLine($"F:\t0x{cpu.F.ToBin()}");
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
