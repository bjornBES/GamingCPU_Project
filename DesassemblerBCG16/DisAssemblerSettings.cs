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
public class DisAssemblerSettings
{
    public static string m_InputFile = "\0";
    public static string m_OutputFile = "a.o";

    public static List<FileInfo> m_Files = new List<FileInfo>();

    public static OutputFormats m_OutputFormat = OutputFormats.obj;

    public static CPUType m_CPUType;

    public static string m_TextSection = "TEXT";
    public static string m_DataSection = "DATA";
    public static string m_RDataSection = "RDATA";
    public static string m_BSSSection = "BSS";
}
