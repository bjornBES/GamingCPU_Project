using Compiler;
using Compiler.ParserNodes;
/// <summary>
/// this is the Compiler for Compiler Cpu Language(CCL)
/// </summary>
public class CompilerCCL
{
    public string m_src;
    public List<string> m_output = new List<string>();
    public CompilerCCL(string src) 
    {
        m_src = src;
        Tokenizer tokenizer = new Tokenizer(m_src);
        Token[] tokens = tokenizer.Tokenize();
        
        Parser parser = new Parser();
        ProgNode progNode = parser.Parse_Prog(tokens, m_src.Split('\n'));

        Generator generator = new Generator();
        generator.gen_prog(progNode);
        
        m_output = generator.m_output;
        return;
    }
}
