using System.Reflection;
using System.Text.RegularExpressions;

public class Tokenizer
{
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

            switch (m_buf.ToLower())
            {
                case "struct":
                    result.Add(addToken(TokenType._struct));
                    break;

                case "end":
                    result.Add(addToken(TokenType.end));
                    break;
                case "asm":
                    result.AddRange(aSMTokenize());
                    break;
                case "sizeof":
                    result.Add(addToken(TokenType._sizeof));
                    break;
                case "call":
                    result.Add(addToken(TokenType.call));
                    break;

                case "retf":
                case "ret":
                case "retu":
                case "return":
                    result.Add(addToken(TokenType._return));
                    break;

                case "file":
                    NewFile();
                    break;

                case "exit":
                    result.Add(addToken(TokenType._exit));
                    break;

                case "extern":
                    result.Add(addToken(TokenType._extern));
                    break;

                case "if":
                    result.Add(addToken(TokenType._if));
                    break;
                case "then":
                    result.Add(addToken(TokenType._then));
                    break;
                case "elseif":
                case "elif":
                    result.Add(addToken(TokenType._elif));
                    break;
                case "else":
                    result.Add(addToken(TokenType._else));
                    break;

                case "display":
                    result.Add(addToken(TokenType.display));
                    break;

                case "res":
                    result.Add(addToken(TokenType._res));
                    break;

                case "near":
                    result.Add(addToken(TokenType._nearPointer));
                    break;
                case "short":
                    result.Add(addToken(TokenType._shortPointer));
                    break;
                case "long":
                    result.Add(addToken(TokenType._longPointer));
                    break;
                case "far":
                    result.Add(addToken(TokenType._farPointer));
                    break;

                case "func":
                case "function":
                    result.Add(addToken(TokenType.function));
                    break;

                case "section":
                    result.Add(addToken(TokenType.Section));
                    break;
                case "invoke":
                    result.Add(addToken(TokenType.invoke));
                    break;

                case "text":
                    result.Add(addToken(TokenType.SectionText));
                    break;
                case "data":
                    result.Add(addToken(TokenType.SectionData));
                    break;
                case "string":
                    result.Add(addToken(TokenType.SectionString));
                    break;

                case "char":
                    result.Add(addToken(TokenType.uint8));
                    break;
                case "byte":
                    result.Add(addToken(TokenType.uint8));
                    break;
                case "sbyte":
                    result.Add(addToken(TokenType.int8));
                    break;

                case "ushort":
                    result.Add(addToken(TokenType.uint16));
                    break;
                case "word":
                    result.Add(addToken(TokenType.uint16));
                    break;
                case "sword":
                    result.Add(addToken(TokenType.int16));
                    break;

                case "dword":
                    result.Add(addToken(TokenType.uint32));
                    break;
                case "int":
                    result.Add(addToken(TokenType.int32));
                    break;
                case "uint":
                    result.Add(addToken(TokenType.uint32));
                    break;

                case "public":
                    result.Add(addToken(TokenType._public));
                    break;
                case "const":
                    result.Add(addToken(TokenType._const));
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
                case "enif":
                    result.Add(addToken(TokenType.end));
                    result.Add(addToken(TokenType._if));
                    break;

                case "shortbyte":
                    result.Add(addToken(TokenType._shortPointer));
                    result.Add(addToken(TokenType.uint8));
                    break;
                case "shortshort":
                    result.Add(addToken(TokenType._shortPointer));
                    result.Add(addToken(TokenType.uint16));
                    break;
                case "shortint":
                    result.Add(addToken(TokenType._shortPointer));
                    result.Add(addToken(TokenType.uint32));
                    break;

                case "longbyte":
                    result.Add(addToken(TokenType._longPointer));
                    result.Add(addToken(TokenType.uint8));
                    break;
                case "longshort":
                    result.Add(addToken(TokenType._longPointer));
                    result.Add(addToken(TokenType.uint16));
                    break;
                case "longint":
                    result.Add(addToken(TokenType._longPointer));
                    result.Add(addToken(TokenType.uint32));
                    break;

                default:
                    result.Add(addToken(TokenType.ident));
                    break;
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
        else
        {
            switch (peek().Value)
            {
                case ':':
                    consume();
                    result.Add(addToken(TokenType.colon));
                    break;
                case '=':
                    consume();
                    result.Add(addToken(TokenType.eq));
                    break;
                case '+':
                    consume();
                    if (peek().HasValue && peek().Value == '+')
                    {
                        consume();
                        result.Add(addToken(TokenType.inc));
                        break;
                    }
                    result.Add(addToken(TokenType.plus));
                    break;
                case '-':
                    consume();
                    if (peek().HasValue && peek().Value == '-')
                    {
                        consume();
                        result.Add(addToken(TokenType.dec));
                        break;
                    }
                    result.Add(addToken(TokenType.minus));
                    break;
                case '*':
                    consume();
                    result.Add(addToken(TokenType.star));
                    break;
                case '/':
                    consume();
                    result.Add(addToken(TokenType.fslash));
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
                case '&':
                    consume();
                    result.Add(addToken(TokenType.ampersand));
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
