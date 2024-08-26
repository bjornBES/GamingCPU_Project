public struct Token
{
    public TokenType Type;
    public int line;
    public string value;
    public override string ToString()
    {
        switch (Type)
        {
            case TokenType.int_lit:
                return "int literal";
            case TokenType.ident:
                return "identifier";
            default:
                return Type.ToString();
        }
    }
    public int? bin_proc()
    {
        switch (Type)
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
