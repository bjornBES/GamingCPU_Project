public class ArgumentFunctions : CompilerSettings
{
    public static void GetOutputFile(string[] args, ref int i)
    {
        i++;
        OutputFile = Path.GetFullPath(args[i]);
    }
    public static void GetInputFile(string[] args, ref int i)
    {
        i++;
        InputFile = args[i];
        Files.Add(new FileInfo(Path.GetFullPath(args[i])));
    }

    public static void DoNotEntry(string[] args, ref int i)
    {
        DoEntry = false;
    }
    public static void SetStartOffset(string[] args, ref int i)
    {
        i++;
        if (int.TryParse(args[i], out int result))
        {
            StartOffset = result;
        }
        else if (args[i].StartsWith("0x"))
        {
            StartOffset = Convert.ToInt32(args[i], 16);
        }
    }
    public static void SetCPUType(string[] args, ref int i)
    {
        i++;

        string CPUstr = args[i];

        if (Enum.TryParse(CPUstr, true, out CPUType result))
        {
            CPUType = result;
        }
        else
        {

        }
    }
}
