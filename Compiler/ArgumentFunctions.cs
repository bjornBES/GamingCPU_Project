public class ArgumentFunctions
{
    public static void GetOutputFile(string[] args, ref int i)
    {
        i++;
        CompilerSettings.OutputFile = Path.GetFullPath(args[i]);
    }
    public static void GetInputFile(string[] args, ref int i)
    {
        i++;
        CompilerSettings.InputFile = args[i];
        CompilerSettings.Files.Add(new FileInfo(Path.GetFullPath(args[i])));
    }

    public static void DoNotEntry(string[] args, ref int i)
    {
        CompilerSettings.DoEntry = false;
    }
    public static void SetStartOffset(string[] args, ref int i)
    {
        i++;
        if (int.TryParse(args[i], out int result))
        {
            CompilerSettings.StartOffset = result;
        }
        else if (args[i].StartsWith("0x"))
        {
            CompilerSettings.StartOffset = Convert.ToInt32(args[i], 16);
        }
    }
}
