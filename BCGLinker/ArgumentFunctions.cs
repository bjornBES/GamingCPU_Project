using System;
using System.IO;

namespace BCGLinker
{
    public class ArgumentFunctions : LinkerSettings
    {
        static bool UsedInputFile;

        public static void GetInputFile(string[] args, ref int i)
        {
            if (UsedInputFile)
            {
                Console.WriteLine("ERROR: can't have 2 input files");
                Environment.Exit(1);
            }

            i++;
            InputFile = args[i];
            UsedInputFile = true;
            Files.Add(new FileInfo(Path.GetFullPath(args[i])));
        }
        public static void GetOutputFile(string[] args, ref int i)
        {
            i++;
            OutputFile = args[i];
        }
        public static void SetFormatType(string[] args, ref int i)
        {
            i++;
            string type = args[i];
            OutputFormat = (OutputFormats)Enum.Parse(typeof(OutputFormats), type);
        }
        public static void SetFormatBin(string[] args, ref int i)
        {
            OutputFormat = OutputFormats.bin;
        }
        public static void SetFormatFbin(string[] args, ref int i)
        {
            OutputFormat = OutputFormats.fbin;
        }
        public static void SetFormatLib(string[] args, ref int i)
        {
            OutputFormat = OutputFormats.lib;
        }
    }
}