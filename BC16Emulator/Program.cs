using BC16CPUEmulator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace BCG16CPUEmulator
{
    public class Program : ArgumentFunctions
    {
        public Dictionary<string, VoidFunction> m_Arguments = new Dictionary<string, VoidFunction>()
        {
            {"-ibin", GetInputFile },
            {"-i", GetInputFile },
            // {"-o", GetOutputFile },
            {"-d", SetDriveBinary },
            // {"-p", SetProjectPath },
            // {"-df", SetErrorFile },
        };
        public delegate void VoidFunction(string[] args, ref int i);
        static int Main(string[] args)
        {
            Console.WriteLine("CPU");
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

            //Screen screen = new Screen();
            //screen.Instantiate();

            //Environment.Exit(1);

            writeInfo(bUS.m_CPU);

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
            Console.WriteLine($"AX:\t0x{cpu.m_AX.ToHex()}" + $" BX:\t0x{cpu.m_BX.ToHex()}");
            Console.WriteLine($"CX:\t0x{cpu.m_CX.ToHex()}" + $" DX:\t0x{cpu.m_DX.ToHex()}");
            Console.WriteLine($"");
            Console.WriteLine($"DS:\t0x{cpu.m_DS.ToHex()} ES:\t0x{cpu.m_ES.ToHex()}");
            Console.WriteLine($"SS:\t0x{cpu.m_SS.ToHex()} CS:\t0x{cpu.m_CS.ToHex()}");
            Console.WriteLine($"");
            Console.WriteLine($"PC:\t0x{cpu.m_PC.ToHex()}");
            Console.WriteLine($"");
            Console.WriteLine($"HL:\t0x{cpu.m_HL.ToHex()}");
            Console.WriteLine($"");
            Console.WriteLine($"SP:\t0x{cpu.m_SP.ToHex()}" + $" BP:\t0x{cpu.m_BP.ToHex()}");
            Console.WriteLine($"");
            Console.WriteLine($"AF:\t{cpu.m_AF.m_Value}f");
            Console.WriteLine($"BF:\t{cpu.m_BF.m_Value}f");
            Console.WriteLine($"");
            Console.WriteLine($"R1:\t0x{cpu.m_R1.ToHex()}" + $" R2:\t0x{cpu.m_R2.ToHex()}");
            Console.WriteLine($"R3:\t0x{cpu.m_R3.ToHex()}" + $" R4:\t0x{cpu.m_R4.ToHex()}");
            Console.WriteLine($"R5:\t0x{cpu.m_R5.ToHex()}" + $" R6:\t0x{cpu.m_R6.ToHex()}");
            Console.WriteLine($"");
            Console.WriteLine($"X:\t0x{cpu.m_X.ToHex()}" + $" Y:\t0x{cpu.m_Y.ToHex()}");
            Console.WriteLine($"");
            Console.WriteLine($"CR0:\t0b{cpu.m_CR0.ToBin()}" + $" CR1:\t0b{cpu.m_CR1.ToBin()}");
            Console.WriteLine($"");
            Console.WriteLine($"\t       FFF:FFFFFFFF");
            Console.WriteLine($"\t       EUR:HILOCSEZ");
            Console.WriteLine($"\t       8FJ:AEEFYIQO");
            Console.WriteLine($"\t  76543210:76543210");
            Console.WriteLine($"F:\t0b{cpu.m_F.ToBin()}");
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
