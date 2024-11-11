using System.Collections.Generic;
using System.IO;

public enum OutputFormats
{
    obj,
}
public enum CPUType
{
    BC8,

    BC16,
    BC16C,
    BC16CE,
    BC1602C,
    BC16F,
    BC16CF,
    BC16CEF,
    BC1602CF,

    BC32,
    BC3203,
    BC32F,
    BC3203F,
    BC3203FD,
}
public class AssemblerSettings
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

    public static bool Debug = false;
}
