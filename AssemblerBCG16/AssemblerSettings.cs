using System.Collections.Generic;
using System.IO;

public enum OutputFormats
{
    obj,
}
public enum CPUType
{
    BC8,
    BC816,
    BC80016,
    BC81680,

    BC16,
    BC16C0,
    BC16C0G,
    BC16C0GX,

    BC24,

    BC32,
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
