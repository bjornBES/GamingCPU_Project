namespace assembler
{
    public struct Token
    {
#pragma warning disable IDE1006 // Naming Styles
        public TokenType type;
        public string ident;
        public long line;
        public long column;
        public string m_file;
#pragma warning restore IDE1006 // Naming Styles

        public override string ToString()
        {
            return $"{type}".PadRight(14, ' ') + $"\tat " + $"{line}".PadLeft(3, '0') + ":" + $"{column}".PadLeft(3, '0') + $",\t{ident}";
        }

        public int? bin_proc()
        {
            switch (type)
            {
                case TokenType.open_paren:
                    return 0;

                case TokenType.minus:
                    return 1;
                case TokenType.plus:
                    return 1;
                case TokenType.vpipe:
                    return 1;
                
                case TokenType.ampersand:
                    return 2;
                case TokenType.caret:
                    return 2;
                case TokenType.star:
                    return 2;
                case TokenType.fslash:
                    return 2;

                case TokenType.tilde:
                    return 10;
                case TokenType.neg:
                    return 10;
                case TokenType.pos:
                    return 10;
                default:
                    break;
            }
            return null;
        }
    }
}
