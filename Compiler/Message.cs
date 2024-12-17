
public class Message
{
    public string Msg { get; set;}
    public MessageType MessageType { get; set;}
    public Token Line{ get; set;}

    public Message(string msg, MessageType messageType, Token line)
    {
        Msg = msg;
        MessageType = messageType;
        Line = line;
    }

    public void Display()
    {
        if (MessageType == MessageType.Error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Error");
            Console.ResetColor();
            Console.WriteLine($" at {Path.GetFileName(Line.m_File)} on line {Line.m_SrcLineNumbers}");
            Console.WriteLine(Msg);
            Console.WriteLine($"{Line.m_File}");
            Console.WriteLine();
        }
        else if (MessageType == MessageType.Warning)
        {
        }
        else
        {
        }
    }
}
