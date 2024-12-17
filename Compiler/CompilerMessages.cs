
public static class CompilerMessages
{
    public static List<Message> Messages = new List<Message>();

    public static void AddMessage(string msg, MessageType messageType, Token line)
    {
        Messages.Add (new Message(msg, messageType, line));
    }

    public static void DisplayMessages()
    {
        foreach (var message in Messages) 
        {
            if (CompilerSettings.m_DoPrintToSTDOut)
            {
                message.Display();
            }
            else
            {
                CompilerSettings.m_ErrorFileContents += message.ToString();
            }
        }
    }
}
