using System.Collections.Generic;
using System.IO;

public enum OutputFormats
{
    obj,
}
public enum CPUType
{
    BCG8,
    BCG16,
    BCG1680,
    BCG1684,
    BCG1880,
    BCG1884,
}
public class AssemblerSettings
{
    public static string InputFile = "\0";
    public static string OutputFile = "a.o";

    public static List<FileInfo> Files = new List<FileInfo>();

    public static OutputFormats OutputFormat = OutputFormats.obj;

    public static CPUType CPUType;

    public static string TextSection = "TEXT";
    public static string DataSection = "DATA";
    public static string RDataSection = "RDATA";
    public static string BSSSection = "BSS";
}
