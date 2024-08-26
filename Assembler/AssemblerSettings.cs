using System.Collections.Generic;
using System.IO;

namespace assembler
{
    public static class AssemblerSettings
    {
        public static string InputFile = "\0";
        public static string OutputFile = "./a.out";
        public static string ProjectPath;

        public static List<FileInfo> Files = new List<FileInfo>();

        public static bool Debug = false;
        public static int DebugLevel = 0;

        public static string ErrorFile;
        public static bool PrintIntoErrorFile = false;

        public static string TEXTSectionName = "text";
        public static string DATASectionName = "data";
        public static string RDATASectionName = "rdata";
        public static string BSSSectionName = "bss";
    }
}