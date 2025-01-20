using System.Diagnostics;

public class ArgumentFunctions : CompilerSettings
{
    public static string[] args;
    public static int i;
    public static void GetOutputFile()
    {
        i++;
        m_OutputFile = Path.GetFullPath(args[i]);
    }
    public static void GetInputFile()
    {
        i++;
        m_InputFile = args[i];
        m_Files.Add(new FileInfo(Path.GetFullPath(args[i])));
    }

    public static void SetDebug()
    {
        Debug = true;
    }

    public static void SetCPUType()
    {
        i++;

        string CPUstr = args[i];

        if (Enum.TryParse(CPUstr, true, out CPUType result))
        {
            m_CPUType = result;
        }
        else
        {

        }
    }
    public static void ShowHelp()
    {
        string FileName = Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName);
        string helpText = 
        $""" 
        Usage: {FileName} [options] file...
        Options:
        \t-help\t\tDisplay this info.

        \t-version\tDisplay compiler version info.

        \t-s\t\tCompile only; do not assemble.
        \t-c\t\tCompile and assemble.
        \t-o <file>\tPlace the output into <file>.

        """;
        helpText = helpText.Replace("\\t", "\t");
        Console.WriteLine(helpText);
        Environment.Exit(0);
    }
}
