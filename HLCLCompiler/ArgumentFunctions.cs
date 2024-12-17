public class ArgumentFunctions : CompilerSettings
{
    public static int i;
    public static void GetOutputFile(string[] args)
    {
        i++;
        m_OutputFile = Path.GetFullPath(args[i]);
    }
    public static void GetInputFile(string[] args)
    {
        i++;
        m_InputFile = args[i];
        m_Files.Add(new FileInfo(Path.GetFullPath(args[i])));
    }

    public static void DoNotEntry(string[] args)
    {
        m_DoEntry = false;
    }
    public static void Debug(string[] args)
    {
        m_Debug = true;
    }
    public static void SetCPUType(string[] args)
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
}
