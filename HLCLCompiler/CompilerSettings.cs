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
    public static string m_ProjectPath;

    public static bool m_DoEntry = true;

    public static List<FileInfo> m_Files = new List<FileInfo>();

    public static CPUType m_CPUType;

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

    public static bool m_DoRaw = false;
}
