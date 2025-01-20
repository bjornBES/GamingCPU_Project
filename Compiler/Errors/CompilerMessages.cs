
public static class CompilerMessages
{
    public static List<string> Messages = new List<string>();

    public static void AddMessage(string errorMessage)
    {
        if (CompilerSettings.m_DoPrintToSTDOut == false)
        {
            Messages.Add (errorMessage);
        }
    }

    public static void DisplayMessages()
    {
        foreach (var message in Messages) 
        {
            if (CompilerSettings.m_DoPrintToSTDOut == false)
            {
                CompilerSettings.m_ErrorFileContents += message.ToString();
            }
        }
    }
}
