using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

public class Tokenizer
{
    public Dictionary<string, TokenType> symbols = new Dictionary<string, TokenType>()
    {
        { "func", TokenType.function },
        { "if", TokenType._if },
        { "then", TokenType._then },
        { "else", TokenType._else },
        { "struct", TokenType._struct },
        { "while", TokenType._while },
        { "end", TokenType.end },

        { "ret", TokenType._return }, { "return", TokenType._return },
        { "extern", TokenType._extern },
        { "public", TokenType._public },
        { "const", TokenType._const },
        { "call", TokenType.call },
        { "sizeof", TokenType._sizeof },
        { "void", TokenType._void },
        { "byte", TokenType.uint8 }, { "uint8", TokenType.uint8 }, { "char", TokenType.uint8 },
        { "sbyte", TokenType.int8 }, { "int8", TokenType.int8 },
        { "ushort", TokenType.uint16 }, { "uint16", TokenType.uint16 },
        { "short", TokenType.int16 }, { "int16", TokenType.int16 },
        { "uint", TokenType.uint32 }, { "uint32", TokenType.uint32 },
        { "int", TokenType.int32 }, { "int32", TokenType.int32 },
        { "near", TokenType._nearPointer },
        { "long", TokenType._longPointer },
        { "far", TokenType._farPointer },
        { "pointer", TokenType.pointer },
        { "break", TokenType._break},
        { "continue", TokenType._continue},

        { "as", TokenType.colon },
        { "eq", TokenType.assign },
    };

    public Dictionary<string, TokenType> operators = new Dictionary<string, TokenType>()
    {
        { "=", TokenType.assign},
        { "!", TokenType.not },
        { "==", TokenType.eq },
        { "!=", TokenType.neq },
        { "<", TokenType.lt },
        { ">", TokenType.gt },
        { "<=", TokenType.leq },
        { ">=", TokenType.geq },
        { "-", TokenType.sub },
        { "-=", TokenType.subassign },
        { "+", TokenType.add },
        { "+=", TokenType.addassign },
        { "&", TokenType.bitand },
        { "&&", TokenType.and },
        { "&=", TokenType.andassign },
        { "*", TokenType.mult },
        { "*=", TokenType.multassign },
        { "<<", TokenType.lshift },
        { "<<=", TokenType.lshiftassign },
        { ">>", TokenType.rshift },
        { ">>=", TokenType.rshiftassign },
        { "|", TokenType.bitor },
        { "||", TokenType.or },
        { "|=", TokenType.orassign },
        { "/", TokenType.div },
        { "/=", TokenType.divassign },
        { "%", TokenType.mod },
        { "%=", TokenType.modassign },
        { "^", TokenType.xor },
        { "^=", TokenType.xorassign },
        // { "++", TokenType.inc },
        // { "--", TokenType.dec },
    };

    string m_src;
    int m_index;
    string m_buf = "";
    int m_lineNumber = 0;
    int m_colNumber = 0;
    string m_file;
    List<Token> m_tokens = new List<Token>();

    public Tokenizer()
    {
        m_index = 0;
    }

    public Token[] Tokenize(string src)
    {
        m_src = src;
        m_colNumber = 1;
        m_lineNumber = 1;
        while (peek().HasValue)
        {
            m_tokens.AddRange(TokenizeToken());
        }

        return m_tokens.ToArray();
    }

    Token[] TokenizeToken()
    {
        List<Token> result = new List<Token>();
        m_buf = "";
        if (peek().Value == '\'')
        {
            consume();
            m_buf = Convert.ToByte(consume()).ToString();
            if (!(peek().HasValue && peek().Value == '\''))
            {
                throw new NotImplementedException();
            }
            result.Add(addToken(TokenType.int_lit));
            consume();
        }
        else if (char.IsAsciiHexDigit(peek().Value) && m_colNumber == 2)
        {
            m_buf += consume();
            while (peek().HasValue && char.IsAsciiHexDigit(peek().Value))
            {
                m_buf += consume();
            }
            m_buf = Convert.ToUInt32(m_buf, 16).ToString().PadLeft(7, '0');

            result.Add(addToken(TokenType.line_number));
        }
        else if (peek().Value == '0' && peek(1).HasValue && peek(1).Value == 'x')
        {
            consume();
            consume();
            while (peek().HasValue && (char.IsAsciiHexDigit(peek().Value) || peek() == '_'))
            {
                if (peek().Value == '_')
                {
                    consume();
                    continue;
                }
                m_buf += consume();
            }

            m_buf = Convert.ToString(Convert.ToInt32(m_buf, 16));

            result.Add(addToken(TokenType.int_lit));

        }
        else if (char.IsLetter(peek().Value) || peek().Value == '_')
        {
            m_buf += consume();
            while (peek().HasValue && (char.IsLetterOrDigit(peek().Value) || peek().Value == '_'))
            {
                m_buf += consume();
            }

            if (symbols.ContainsKey(m_buf))
            {
                result.Add(addToken(symbols[m_buf]));
            }
            else
            {
                switch (m_buf.ToLower())
                {
                    case "asm":
                        // result.AddRange(aSMTokenize());
                        result.Add(addToken(TokenType._asm));
                        break;
                    case "file":
                        NewFile();
                        break;

                    case "invoke":
                        result.Add(addToken(TokenType.invoke));
                        break;

                    case "define":
                        result.Add(addToken(TokenType.define));
                        break;

                    case "endfunc":
                    case "endfunction":
                        result.Add(addToken(TokenType.end));
                        result.Add(addToken(TokenType.function));
                        break;
                    case "endif":
                        result.Add(addToken(TokenType.end));
                        result.Add(addToken(TokenType._if));
                        break;
                    case "endwhile":
                        result.Add(addToken(TokenType.end));
                        result.Add(addToken(TokenType._while));
                        break;

                    case "elseif":
                    case "elif":
                        result.Add(addToken(TokenType._else));
                        result.Add(addToken(TokenType._if));
                        break;

                    default:
                        result.Add(addToken(TokenType.ident));
                        break;
                }
            }
        }
        else if (new string("0123456789").Contains(peek().Value))
        {
            m_buf += consume();
            while (peek().HasValue && char.IsDigit(peek().Value))
            {
                m_buf += consume();
            }
            result.Add(addToken(TokenType.int_lit));
        }
        else if (char.IsSeparator(peek().Value))
        {
            consume();
        }
        else if (peek().Value == '\\' && peek(1).HasValue && peek(1).Value == '\\')
        {
            consume();
            consume();
        }
        else if (peek().Value == '@' && m_colNumber == 1)
        {
            while (peek().HasValue && peek().Value != '\n')
            {
                consume();
            }
        }
        else if (peek().Value == '#' && m_colNumber == 1)
        {
            consume();
            while (peek().HasValue && char.IsSeparator(peek().Value))
            {
                consume();
            }
            consume();
            while (peek().HasValue && char.IsLetter(peek().Value))
            {
                m_buf += consume();
            }

            if (m_buf.StartsWith("FILE", StringComparison.OrdinalIgnoreCase))
            {
                m_buf = "";
                while (peek().HasValue && peek().Value != '\n')
                {
                    m_buf += consume();
                }
                string file = m_buf.Trim('\"');
                m_file = file;
            }
            else if (m_buf.StartsWith("DEFINE", StringComparison.OrdinalIgnoreCase))
            {
                result.Add(addToken(TokenType.defineD));
                /*
                m_buf = m_buf.Replace("DEFINE ", "", StringComparison.OrdinalIgnoreCase);
                string name = m_buf.Split(' ').First();
                if (m_buf.Split(' ').Length > 1)
                {
                    result.Add(addToken(TokenType.ident, name));
                    Tokenizer tokenizer = new Tokenizer();
                    m_buf = m_buf.Replace(name, "");
                    result.AddRange(tokenizer.Tokenize(m_buf));
                }
                else
                {
                    CompilerSettings.Defines.Add(new Define() { m_Name = name, m_Value = 0 });
                }
                */
            }
            else if (m_buf.StartsWith("IF", StringComparison.OrdinalIgnoreCase))
            {
                result.Add(addToken(TokenType.DIf));
            }
            else if (m_buf.StartsWith("ENDIF", StringComparison.OrdinalIgnoreCase))
            {
                result.Add(addToken(TokenType.DEndIf));
            }
        }
        else
        {
            switch (peek().Value)
            {
                case ':':
                    consume();
                    result.Add(addToken(TokenType.colon));
                    break;
                case '=':
                case '+':
                case '-':
                case '*':
                case '/':
                case '&':
                case '<':
                case '>':
                    result.AddRange(parseOperators());
                    break;
                case '(':
                    consume();
                    result.Add(addToken(TokenType.open_paren));
                    break;
                case ')':
                    consume();
                    result.Add(addToken(TokenType.close_paren));
                    break;
                case '{':
                    consume();
                    result.Add(addToken(TokenType.open_curly));
                    break;
                case '}':
                    consume();
                    result.Add(addToken(TokenType.close_curly));
                    break;
                case '[':
                    consume();
                    result.Add(addToken(TokenType.open_square));
                    break;
                case ']':
                    consume();
                    result.Add(addToken(TokenType.close_square));
                    break;
                case '@':
                    consume();
                    result.Add(addToken(TokenType.at));
                    break;
                case ',':
                    consume();
                    result.Add(addToken(TokenType.comma));
                    break;
                case '.':
                    consume();
                    result.Add(addToken(TokenType.period));
                    break;
                case '#':
                    consume();
                    result.Add(addToken(TokenType.numberSign));
                    break;
                case '\"':
                    consume();
                    result.Add(addToken(TokenType.quotation_mark));
                    while (peek().HasValue && peek().Value != '\"')
                    {
                        m_buf += consume();
                    }
                    result.Add(addToken(TokenType.ident));
                    m_buf = "";
                    consume();
                    result.Add(addToken(TokenType.quotation_mark));
                    break;
                case '\n':
                    consume();
                    m_colNumber = 1;
                    m_lineNumber++;
                    break;
                default:
                    break;
            }
        }
        return result.ToArray();
    }

    private void NewFile()
    {
        consume();
        m_tokens.RemoveRange(m_tokens.Count - 2, 2);
        if (peek().HasValue && peek().Value == '\"')
        {
            consume();
            m_file = "";
            while (peek().HasValue && peek().Value != '\"')
            {
                m_file += consume();
            }
            consume();
            m_colNumber = 1;
            m_lineNumber = 0;
        }
    }

    Token[] parseOperators()
    {
        List<Token> result = new List<Token>();

        string operatorBuffer = "";
        operatorBuffer += consume();

        if (!peek().HasValue)
        {
            Console.WriteLine("Not working");
            Environment.Exit(1);
        }
        if (peek().Value == '=')
        {
            operatorBuffer += consume();
        }
        else if (peek(1).Value == '=')
        {
            operatorBuffer += consume();
            operatorBuffer += consume();
        }
        else
        {
            if (operatorBuffer == "=")
            {

            }
            else if (operatorBuffer == "+" || operatorBuffer == "-")
            {
                operatorBuffer += consume();
            }
        }

        if (operators.ContainsKey(operatorBuffer))
        {
            result.Add(addToken(operators[operatorBuffer]));
        }
        else if (operatorBuffer == "++")
        {
            result.Add(addToken(TokenType.addassign));
            result.Add(addToken(TokenType.int_lit, "1"));
        }
        else if (operatorBuffer == "--")
        {
            result.Add(addToken(TokenType.subassign));
            result.Add(addToken(TokenType.int_lit, "1"));
        }

        return result.ToArray();
    }

    Token[] aSMTokenize()
    {
        List<Token> tokens = new List<Token>();

        if (!peek().HasValue || peek().Value != '(')
        {
            Console.WriteLine("Error: missing '('");
            Environment.Exit(1);
        }
        consume();
        if (!peek().HasValue || peek().Value != '"')
        {
            Console.WriteLine("Error: missing '\"'");
            Environment.Exit(1);
        }
        consume();
        tokens.Add(addToken(TokenType._asm));
        tokens.Add(addToken(TokenType.open_paren));
        tokens.Add(addToken(TokenType.quotation_mark));

        List<Token> variabels = new List<Token>();
        string buffer = "";
        while (peek().HasValue && peek().Value != '\"')
        {
            if (peek().Value == '{')
            {
                buffer += consume();
                if (peek().HasValue && peek().Value == '{')
                {
                    buffer += consume();
                    continue;
                }

                variabels.AddRange(TokenizeToken());
                buffer += m_buf;

                if (!peek().HasValue || peek().Value != '}')
                {
                    Console.WriteLine("Error: missing '}'");
                    Environment.Exit(1);
                }
                buffer += consume();
            }
            else if (peek().Value == '}')
            {
                consume();
                if (peek().HasValue && peek().Value == '}')
                {
                    buffer += consume();
                    continue;
                }
                else
                {
                    Console.WriteLine("Error: missing '}'");
                    Environment.Exit(1);
                }
            }
            else
            {
                buffer += consume();
            }
        }

        m_buf = buffer;
        tokens.Add(addToken(TokenType.ident));
        tokens.AddRange(variabels);

        if (!peek().HasValue || peek().Value != '"')
        {
            Console.WriteLine("Error: missing '\"'");
            Environment.Exit(1);
        }
        consume();
        if (!peek().HasValue || peek().Value != ')')
        {
            Console.WriteLine("Error: missing ')'");
            Environment.Exit(1);
        }
        consume();
        tokens.Add(addToken(TokenType.quotation_mark));
        tokens.Add(addToken(TokenType.close_paren));
        return tokens.ToArray();
    }
    Token addToken(TokenType type)
    {
        return addToken(type, m_buf);
    }
    Token addToken(TokenType type, string line)
    {
        Token result = new Token() { m_Type = type, m_Value = line, m_Line = m_lineNumber, m_File = m_file };
        return result;
    }
    char? peek(int offset = 0)
    {
        if (m_index + offset >= m_src.Length) return null;
        return m_src[m_index + offset];
    }
    char consume()
    {
        m_colNumber++;
        return m_src[m_index++];
    }
}
