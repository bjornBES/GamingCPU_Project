using Compiler;
/// <summary>
/// this is the Compiler for Compiler Cpu Language(CCL)
/// </summary>
public class CompilerCCL
{
    public string m_Src;
    public List<string> m_Output = new List<string>();
    public CompilerCCL(string src) 
    {
        string TokenFormat = "";
        

        string[] lines = src.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            string element = lines[i].Split("\\\\")[0];
            if (element.Length == 0)
            {
                m_Src += "\n";
                continue;
            }
            string lineNumber = element.Split(':').First();
            string line = element.Split(':').Last().PadRight(64, ' ');
            if (lineNumber == "SECTION")
            {
                m_Src += lineNumber + ":" + line + "\n";
                continue;
            }
            if (line.Length > 64)
            {
                Console.WriteLine("Error: Line needs to be 64 characters long with spaces");
            }
            m_Src += lineNumber + ":" + line + "\\\\\n";
        }

        Tokenizer tokenizer = new Tokenizer(m_Src);
        Token[] tokens = tokenizer.Tokenize();

        for (int i = 0; i < tokens.Length; i++)
        {
            TokenFormat += $"{tokens[i]}".PadRight(20, ' ') + $"line:" + $"{tokens[i].m_Line}".PadLeft(5, '0') + $"\t{tokens[i].m_Value}" + $"{Environment.NewLine}";
        }

        File.WriteAllText("./Tokens.txt", TokenFormat);
        //Environment.Exit(0);
        Parser parser = new Parser();
        parser.Parse_Prog(tokens, m_Src.Split('\n'));

        m_Output = parser.m_Output;
        return;
    }
}
