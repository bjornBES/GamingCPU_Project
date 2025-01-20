using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum CPUType
{
    BC8,
    BC816,
    BC810680,

    BC16,
    BC16C,
    BC16CE,
    BC1602C,
    BC16F,
    BC16CF,
    BC16CEF,
    BC1602CF,

    BC24,
    BC24F,

    BC32,
    BC3203,
    BC32F,
    BC3203F,
    BC3203FD,
}

public class CompilerSettings
{
    public static string m_InputFile = "\0";
    public static string m_OutputFile = "./CCLOutput.txt";
    public static string m_ErrorFile = "";

    public static bool m_DoWriteOut = true;
    public static string m_ErrorFileContents = "";
    public static List<FileInfo> m_Files = new List<FileInfo>();
    public static List<Define> Defines = new List<Define>();
    public static bool m_DoPrintToSTDOut = true;
    public static CPUType m_CPUType;
    public static bool InProtectedMode = false;
    public static bool Debug = false;

    public const int BYTESIZE = 1;
    public const int WORDSIZE = 2;
    public const int TBYTESIZE = 3;
    public const int DWORDSIZE = 4;
    public const int NEARPOINTERSIZE = 1;
    public const int SHORTPOINTERSIZE = 2;
    public const int LONGPOINTERSIZE = 3;
    public const int FARPOINTERSIZE = 4;

    public const bool ISSIGNED = true;
    public const bool ISUNSIGNED = false;

    public const int VariabelSize = WORDSIZE;

    public static bool m_DoRaw = false;

    public static string TextSectionName = "TEXT";
    public static string DataSectionName = "DATA";
    public static string BSSSectionName = "BSS";
}

public class Define
{
    public string m_Name;
    public object m_Value;
}
