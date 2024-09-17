using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum CPUType
{
    BCG8,
    BCG16,
    BCG1680,
    BCG1684,
    BCG1880,
    BCG1884,
}

public class CompilerSettings
{
    public static string InputFile = "\0";
    public static string OutputFile = "./CCLOutput.txt";

    public static bool DoEntry = true;

    public static int StartOffset = 0x10000;

    public static List<FileInfo> Files = new List<FileInfo>();

    public static CPUType CPUType;

    public const int BYTESIZE = 1;
    public const int WORDSIZE = 2;
    public const int TBYTESIZE = 2;
    public const int DWORDSIZE = 4;
    public const int NEARPOINTERSIZE = 1;
    public const int SHORTPOINTERSIZE = 2;
    public const int LONGPOINTERSIZE = 3;
    public const int FARPOINTERSIZE = 4;

    public const bool ISSIGNED = true;
    public const bool ISUNSIGNED = false;

    public enum AssignmentOperators
    {
        none,
        basic_assignment,
    }
}
