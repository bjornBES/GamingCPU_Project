public class ArgumentFunctions : CompilerSettings
{
    public static void GetOutputFile(string[] args, ref int i)
    {
        i++;
        m_OutputFile = Path.GetFullPath(args[i]);
    }
    public static void GetInputFile(string[] args, ref int i)
    {
        i++;
        m_InputFile = args[i];
        m_Files.Add(new FileInfo(Path.GetFullPath(args[i])));
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
}
