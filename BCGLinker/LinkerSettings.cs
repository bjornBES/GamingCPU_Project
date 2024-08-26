using System.Collections.Generic;
using System.IO;

namespace BCGLinker
{
    public enum OutputFormats
    {
        fbin,
        bin,
        lib,
    }
    public class LinkerSettings
    {
        public static string InputFile = "\0";
        public static string OutputFile = "a.bin";

        public static OutputFormats OutputFormat = OutputFormats.bin;

        public static string HeaderSection = "_header";
        public static string TextSection = "_text";
        public static string DataSection = "_data";
        public static string RDataSection = "_rdata";
        public static string BSSSection = "_bss";

        public static List<FileInfo> Files = new List<FileInfo>();
    }
}