using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CompilerSettings
{
    public static string InputFile = "\0";
    public static string OutputFile = "./CCLOutput.txt";

    public static bool DoEntry = true;

    public static int StartOffset = 0x10000;

    public static List<FileInfo> Files = new List<FileInfo>();

    public const int BYTESIZE = 1;
    public const int WORDSIZE = 2;
    public const int POINTERSIZE = 3;

    public const bool ISSIGNED = true;
    public const bool ISUNSIGNED = false;

    public enum AssignmentOperators
    {
        none,
        basic_assignment,
    }
}
