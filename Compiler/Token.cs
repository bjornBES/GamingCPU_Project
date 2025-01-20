public struct Token
{
    public TokenType m_Type { get; set; }
    public int m_Line { get; set; }
    public string m_Value { get; set; }
    public string m_File { get; set; }
    public string m_SrcLineNumbers { get; set; }
    public override string ToString()
    {
        switch (m_Type)
        {
            case TokenType.int_lit:
                return "int literal";
            case TokenType.ident:
                return "identifier";
            default:
                return m_Type.ToString();
        }
    }
    public int? BinProc()
    {
        switch (m_Type)
        {
            case TokenType.inc:     return 0;
            case TokenType.dec:     return 0;
            case TokenType.sub:     return 3;
            case TokenType.add:     return 3;
            case TokenType.mult:    return 4;
            case TokenType.div:     return 4;

            case TokenType.neg:     return 10;

            case TokenType.eq:      return 15;
            case TokenType.neq:     return 15;
            case TokenType.geq:     return 15;
            case TokenType.leq:     return 15;
            case TokenType.gt:      return 15;
            case TokenType.lt:      return 15;
            default:
                break;
        }
        return null;
    }
}
