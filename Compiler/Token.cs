public struct Token
{
    public TokenType m_Type;
    public int m_Line;
    public string m_Value;
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
            case TokenType.minus:   return 3;
            case TokenType.plus:    return 3;
            case TokenType.star:    return 4;
            case TokenType.fslash:  return 4;

            case TokenType.neg:     return 10;
            default:
                break;
        }
        return null;
    }
}
