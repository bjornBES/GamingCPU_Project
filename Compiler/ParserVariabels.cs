namespace Compiler
{
    public class ParserVariabels
    {
        public int m_StackSize;
        public Stack<int> m_Scopes = new Stack<int>();

        public Token[] m_tokens;
        public int m_index = 0;
        public List<Var> m_var = new List<Var>();
        public List<Function> m_functions = new List<Function>();
        public List<string> m_lineNumber = new List<string>();
        public Token m_lastToken;
        public List<string> m_lineNumbers = new List<string>();
        public int m_labelCount;
        public int m_globalVarIndex = 0;

        public bool DoNotWrite = true;

        public Section m_section;

        public Dictionary<string, string> m_strings = new Dictionary<string, string>();
        public Dictionary<string, string> m_defines = new Dictionary<string, string>();
        public Dictionary<string, BssEntry> m_bssDefines = new Dictionary<string, BssEntry>();

        public List<string> m_OutputTop = new List<string>();
        public List<string> m_Output = new List<string>();
        public List<string> m_OutputBss = new List<string>();
        public List<string> m_OutputData = new List<string>();
        public List<string> m_OutputRodata = new List<string>();

        public string RegsToString(Regs regs)
        {
            switch (regs)
            {
                case Regs.AX:
                    return "AX";
                case Regs.BX:
                    return "BX";
                case Regs.CX:
                    return "CX";
                case Regs.DX:
                    return "DX";
                case Regs.HL:
                    return "HL";
                case Regs.H:
                    return "H";
                case Regs.L:
                    return "L";
                default:
                    break;
            }
            return "";
        }
        public Size GetRegsSize(Regs regs)
        {
            switch (regs)
            {
                case Regs.AX:
                case Regs.BX:
                case Regs.CX:
                case Regs.DX:
                case Regs.HL:
                    return Size._int;
                case Regs.H:
                case Regs.L:
                    return Size._short;
                default:
                    break;
            }
            return Size.pointer;
        }
    }
}

public struct BssEntry
{
    public string m_name;
    public int m_size;
    public string m_offset;
}