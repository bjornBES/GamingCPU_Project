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
            {"-d", SetDriveBinary },
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

            if (m_InputFile == "\0")
            {
                Console.WriteLine("Error: there is no input file");
                Environment.Exit(1);
            }

            m_InputFile = Path.GetFullPath(m_InputFile);
            
            BUS bUS = new BUS();

            bUS.Reset();

            byte[] BinFileContents = File.ReadAllBytes(m_InputFile);

            bUS.Load(BinFileContents);

            while (bUS.m_CPU.GetFlag(BC16CPU_Registers.FL_H) == false)
            {
                bUS.Tick();
            }

            writeInfo(bUS.m_CPU);
        }

        void writeInfo(BC16CPU cpu)
        {
            Console.WriteLine($"A:\t0x{cpu.m_A.ToHex()}" + $" B:\t0x{cpu.m_B.ToHex()}");
            Console.WriteLine($"C:\t0x{cpu.m_C.ToHex()}" + $" D:\t0x{cpu.m_D.ToHex()}");
            Console.WriteLine($"");
            Console.WriteLine($"HL:\t0x{cpu.m_H.ToHex()}:{cpu.m_L.ToHex()}");
            Console.WriteLine($"");
            Console.WriteLine($"DS:\t0x{cpu.m_DS.ToHex()} ES:\t0x{cpu.m_ES.ToHex()}");
            Console.WriteLine($"SS:\t0x{cpu.m_SS.ToHex()} S:\t0x{cpu.m_CS.ToHex()}");
            Console.WriteLine($"");
            Console.WriteLine($"PC:\t0x{cpu.m_PC.ToHex()}");
            Console.WriteLine($"");
            Console.WriteLine($"SP:\t0x{cpu.m_SP.ToHex()}" + $" BP:\t0x{cpu.m_BP.ToHex()}");
            Console.WriteLine($"");
            Console.WriteLine($"AF:\t{cpu.m_AF.m_Value}f".PadRight(11, ' ') + $"BF:\t{cpu.m_BF.m_Value}f");
            Console.WriteLine($"");
            Console.WriteLine($"R1:\t0x{cpu.m_R1.ToHex()}" + $" R2:\t0x{cpu.m_R2.ToHex()}");
            Console.WriteLine($"");
            Console.WriteLine($"MB:\t0x{cpu.m_MB.ToHex()}");
            Console.WriteLine($"");
            Console.WriteLine($"CR0:\t0b{cpu.m_CR0.ToBin()}" + $" CR1:\t0b{cpu.m_CR1.ToBin()}");
            Console.WriteLine($"");
            Console.WriteLine($"\t  F     F    FFFFFF");
            Console.WriteLine($"\t  H     I    LOCSEZ");
            Console.WriteLine($"\t  A     E    EFYIQO");
            Console.WriteLine($"F:\t0x{cpu.m_F.ToBin()}");
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
                    m_Files.Add(new FileInfo(Path.GetFullPath(args[i])));
                }
            }
        }
    }
}
