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

    public static void DoNotEntry(string[] args, ref int i)
    {
        m_DoEntry = false;
    }
    public static void SetStartOffset(string[] args, ref int i)
    {
        i++;
        if (int.TryParse(args[i], out int result))
        {
            m_StartOffset = result;
        }
        else if (args[i].StartsWith("0x"))
        {
            m_StartOffset = Convert.ToInt32(args[i], 16);
        }
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
