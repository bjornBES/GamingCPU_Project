using System;
using System.IO;
using System.Linq;
using System.Reflection;

public class ArgumentFunctions : AssmeblerErrors
{
    static bool m_usedInputFile;

    public static void GetInputFile(string[] args, ref int i)
    {
        if (m_usedInputFile)
        {
            Console.WriteLine("ERROR: can't have 2 input files");
            Environment.Exit(1);
        }

        i++;
        m_InputFile = args[i];
        m_usedInputFile = true;
        m_Files.Add(new FileInfo(Path.GetFullPath(args[i])));
    }

    public static void GetOutputFile(string[] args, ref int i)
    {
        i++;
        m_OutputFile = Path.GetFullPath(args[i]);
    }
    public static void SetFormatOutput(string[] args, ref int i)
    {
        i++;
        string type = args[i];
        m_OutputFormat = (OutputFormats)Enum.Parse(typeof(OutputFormats), type);
    }
    public static void SetFormatOBJ(string[] args, ref int i)
    {
        m_OutputFormat = OutputFormats.obj;
    }

    public static void SetCPUType(string[] args, ref int i)
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

    public static void PrintHelp(string[] args, ref int i)
    {
        string ExecutableName = Assembly.GetExecutingAssembly().Location.Split('.').First();

        Console.WriteLine($"Usage: {ExecutableName}.exe [options] file{Environment.NewLine}");
        Console.WriteLine("Options:");
        Console.WriteLine($"\t-help\t\t\tDisplay this usage message. (short form: -h or -?)");
        Console.WriteLine($"\t-output file\t\tName the output file. (short form: -o)");
        Console.WriteLine($"\t-fr file\t\tSet an error file.");
        Console.Write($"{Environment.NewLine}");
        Console.WriteLine($"\t-format format\t\tselect output file format. (short form: -f)");
        Console.WriteLine($"\t\tobj\t\tobject file");
        Console.Write($"{Environment.NewLine}");
        Console.WriteLine($"\t-text-name name\t\tSet the name of the TEXT section(short form: -tn)");
        Console.WriteLine($"\t-data-name name\t\tSet the name of the DATA section(short form: -dn)");
        Console.WriteLine($"\t-rodata-name name\tSet the name of the RODATA section(short form: -rn)");
        Console.WriteLine($"\t-bss-name name\t\tSet the name of the BSS section(short form: -bn)");
        Console.Write($"{Environment.NewLine}");
        Console.WriteLine($"\t-cpu-type type\tSet cpu type(short from:-cpu)");
        Console.WriteLine($"\t\t  BCG8\t\tthe 8 bit verison of the CPU");
        Console.WriteLine($"\t\t  BCG16\t\tthe 16 bit verison of the CPU");
        Console.WriteLine($"\t\t  BCG1680\tthe 16/32 bit verison of the CPU");
        Console.WriteLine($"\t\t  BCG1686\tthe 16/32 bit verison of the CPU");
        Environment.Exit(0);
    }

    public static void SetTextSegmentName(string[] args, ref int i)
    {
        i++;
        m_TextSection = args[i];
    }
    public static void SetDataSegmentName(string[] args, ref int i)
    {
        i++;
        m_DataSection = args[i];
    }
    public static void SetRdataSegmentName(string[] args, ref int i)
    {
        i++;
        m_RDataSection = args[i];
    }
    public static void SetBSSSegmentName(string[] args, ref int i)
    {
        i++;
        m_BSSSection = args[i];
    }

    public static void SetErrorFile(string[] args, ref int i)
    {

    }
}
