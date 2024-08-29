using System;
using System.IO;

namespace BCG16CPUEmulator
{
    public class ArgumentFunctions : CPUSettings
    {
        static bool UsedInputFile = false;

        public static void GetInputFile(string[] args, ref int i)
        {
            if (UsedInputFile)
            {
                Console.WriteLine("ERROR: can't have 2 input files");
                Environment.Exit(1);
            }

            i++;
            InputFile = args[i];
            //UsedInputFile = true;
            Files.Add(new FileInfo(Path.GetFullPath(args[i])));
        }
    }
}