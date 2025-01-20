namespace Compiler
{
    public class ParserVariabels
    {
        public Token[] Tokens;
        public int index = 0;
        public List<string> LineNumbers = new List<string>();

        public bool DoNotWrite = true;

        public ProgNode output;

        public List<string> functionNames = new List<string>();
        public bool inFunction = false;

        public Dictionary<string, string> strings = new Dictionary<string, string>();
        public Dictionary<string, BssEntry> bssDefines = new Dictionary<string, BssEntry>();
    }
}

public struct BssEntry
{
    public string m_name;
    public int m_size;
    public string m_offset;
}