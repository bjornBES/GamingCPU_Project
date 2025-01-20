using System.Text.Json;
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

        if (CompilerSettings.Debug)
        {
            CompilerSettings.Defines.Add(new Define() { m_Name = "debug", m_Value = 1});
        }
        else
        {
            CompilerSettings.Defines.Add(new Define() { m_Name = "debug", m_Value = 0});
        }

        string copySrc = src;
        string TokenFormat = "";
        
        for (var i = 0; i < CompilerSettings.Defines.Count; i++)
        {
            copySrc = $"#       :define {CompilerSettings.Defines[i].m_Name} = {CompilerSettings.Defines[i].m_Value}\n" + copySrc;
        }

        string[] lines = copySrc.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            string element = lines[i].Split("\\\\")[0];
            if (element.Length == 0)
            {
                m_Src += "\n";
                continue;
            }
            string lineNumber = element.Split(':').First();
            string line = element.Split(':', 2).Last();
            
            if (lineNumber.StartsWith("*"))
            {
                continue;
            }
            else if (lineNumber.StartsWith(" "))
            {

            }
            else if (lineNumber.StartsWith("file", StringComparison.OrdinalIgnoreCase))
            {
                m_Src += "#       :" + lineNumber + "\n";
                continue;
            }

            /*
            if (lineNumber == "SECTION")
            {
                m_Src += lineNumber + ":" + line + "\n";
                continue;
            }
            else if (element.StartsWith("#"))
            {
                m_Src += line + "\n";
                continue;
            }
            else if (line.StartsWith("extern"))
            {
                m_Src += element + "\n";
                continue;
            }
            */

            m_Src += lineNumber + ":" + line + "\n";
        }

        File.WriteAllText("./src/CCL/FullSrc.txt", m_Src);
        Tokenizer tokenizer = new Tokenizer();
        Token[] tokens = tokenizer.Tokenize(m_Src);

        TokenFormat = JsonSerializer.Serialize(tokens, new JsonSerializerOptions() {
            WriteIndented = true,
        });

        File.WriteAllText("./src/CCL/Tokens.json", TokenFormat);

        TokenFormat = "";
        for (int i = 0; i < tokens.Length; i++)
        {
            Token token = tokens[i];
            TokenFormat += $"{token.m_Type}".PadRight(20, ' ') + $"{token.m_Value}".PadRight(20, ' ') + $"{token.m_Line}" + Environment.NewLine;
        }
        File.WriteAllText("./src/CCL/Tokens.txt", TokenFormat);

        Parser parser = new Parser();
        parser.Parse_Prog(tokens);

        if(CompilerSettings.m_DoWriteOut == false)
        {
            Console.WriteLine("Exiting program found error");
            Environment.Exit(1);
        }

        TokenFormat = JsonSerializer.Serialize(parser.output, new JsonSerializerOptions() {
            WriteIndented = true,
        });
        File.WriteAllText("./src/CCL/AST.json", TokenFormat);
        
        Generator generator= new Generator();
        generator.genProgram(parser.output);

        if(CompilerSettings.m_DoWriteOut == false)
        {
            Console.WriteLine("Exiting program found error");
            Environment.Exit(1);
        }

        m_Output = generator.output;
        return;
    }
}
