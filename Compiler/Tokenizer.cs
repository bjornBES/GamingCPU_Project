using System.Text.RegularExpressions;

public class Tokenizer
{
    string m_src;
    int m_index;
    string m_buf = "";
    int m_lineNumber = 0;
    int m_colNumber = 0;
    List<Token> m_tokens = new List<Token>();

    public Tokenizer(string src)
    {
        m_src = src;
        m_index = 0;
    }

    public Token[] Tokenize()
    {
        m_colNumber = 1;
        m_lineNumber = 1;
        while (peek().HasValue)
        {
            m_buf = "";
            if (peek().Value == '\'')
            {
                consume();
                m_buf = Convert.ToByte(consume()).ToString();
                if (!(peek().HasValue && peek().Value == '\''))
                {
                    throw new NotImplementedException();
                }
                addToken(TokenType.int_lit);
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

                addToken(TokenType.int_lit);

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
                    case "struct":  addToken(TokenType._struct);    break;

                    case "prog":
                    case "program": addToken(TokenType.program);    break;
                    
                    case "end":     addToken(TokenType.end);        break;
                    case "asm":     aSMTokenize();                  break;
                    case "sizeof":  addToken(TokenType._sizeof);    break;
                    case "call":    addToken(TokenType.call);       break;
                    
                    case "retf":
                    case "ret":
                    case "retu":
                    case "return":  addToken(TokenType._return);    break;

                    case "exit":    addToken(TokenType._exit);      break;

                    case "if":      addToken(TokenType._if);      break;
                    case "then":    addToken(TokenType._then);    break;
                    case "elseif":
                    case "elif":    addToken(TokenType._elif);      break;
                    case "else":    addToken(TokenType._else);      break;
                    
                    case "display": addToken(TokenType.display);    break;

                    case "res":     addToken(TokenType._res);       break;

                    case "near":    addToken(TokenType._nearPointer);      break;
                    case "short":   addToken(TokenType._shortPointer);     break;
                    case "long":    addToken(TokenType._longPointer);      break;
                    case "far":     addToken(TokenType._farPointer);       break;
                    
                    case "func":
                    case "function":addToken(TokenType.function);   break;

                    case "section": addToken(TokenType.Section);    break;
                    case "invoke":  addToken(TokenType.invoke);     break;

                    case "text":    addToken(TokenType.SectionText);    break;
                    case "data":    addToken(TokenType.SectionData);    break;
                    case "string":  addToken(TokenType.SectionString);  break;

                    case "char":    addToken(TokenType.uint8);      break;
                    case "byte":    addToken(TokenType.uint8);      break;
                    case "sbyte":   addToken(TokenType.int8);       break;

                    case "ushort":  addToken(TokenType.uint16);     break;
                    case "word":    addToken(TokenType.uint16);     break;
                    case "sword":   addToken(TokenType.int16);      break;
                    
                    case "dword":   addToken(TokenType.uint32);     break;
                    case "int":     addToken(TokenType.int32);      break;
                    case "uint":    addToken(TokenType.uint32);     break;

                    case "public":  addToken(TokenType._public);    break;
                    case "const":   addToken(TokenType._const);     break;

                    case "endfunc": 
                    case "endfunction": 
                        addToken(TokenType.end); addToken(TokenType.function); break;
                    case "endif":
                    case "enif":
                        addToken(TokenType.end); addToken(TokenType._if); break;
                    case "endprogram":
                    case "endprog":
                        addToken(TokenType.end); addToken(TokenType.program); break;

                    case "shortbyte":
                        addToken(TokenType._shortPointer);
                        addToken(TokenType.uint8);
                        break;
                    case "shortshort":
                        addToken(TokenType._shortPointer);
                        addToken(TokenType.uint16);
                        break;
                    case "shortint":
                        addToken(TokenType._shortPointer);
                        addToken(TokenType.uint32);
                        break;

                    case "longbyte":
                        addToken(TokenType._longPointer);
                        addToken(TokenType.uint8);
                        break;
                    case "longshort":
                        addToken(TokenType._longPointer);
                        addToken(TokenType.uint16);
                        break;
                    case "longint":
                        addToken(TokenType._longPointer);
                        addToken(TokenType.uint32);
                        break;

                    default:
                        addToken(TokenType.ident);
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
                addToken(TokenType.int_lit);
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
                        addToken(TokenType.colon);
                        break;
                    case '=':
                        consume();
                        addToken(TokenType.eq);
                        break;
                    case '+':
                        consume();
                        if (peek().HasValue && peek().Value == '+')
                        {
                            consume();
                            addToken(TokenType.inc);
                            break;
                        }
                        addToken(TokenType.plus);
                        break;
                    case '-':
                        consume();
                        if (peek().HasValue && peek().Value == '-')
                        {
                            consume();
                            addToken(TokenType.dec);
                            break;
                        }
                        addToken(TokenType.minus);
                        break;
                    case '*':
                        consume();
                        addToken(TokenType.star);
                        break;
                    case '/':
                        consume();
                        addToken(TokenType.fslash);
                        break;
                    case '(':
                        consume();
                        addToken(TokenType.open_paren);
                        break;
                    case ')':
                        consume();
                        addToken(TokenType.close_paren);
                        break;
                    case '{':
                        consume();
                        addToken(TokenType.open_curly);
                        break;
                    case '}':
                        consume();
                        addToken(TokenType.close_curly);
                        break;
                    case '[':
                        consume();
                        addToken(TokenType.open_square);
                        break;
                    case ']':
                        consume();
                        addToken(TokenType.close_square);
                        break;
                    case '@':
                        consume();
                        addToken(TokenType.at);
                        break;
                    case '&':
                        consume();
                        addToken(TokenType.ampersand);
                        break;
                    case ',':
                        consume();
                        addToken(TokenType.comma);
                        break;
                    case '.':
                        consume();
                        addToken(TokenType.period);
                        break;
                    case '\"':
                        consume();
                        addToken(TokenType.quotation_mark);
                        while (peek().HasValue && peek().Value != '\"')
                        {
                            m_buf += consume();
                        }
                        addToken(TokenType.ident);
                        m_buf = "";
                        consume();
                        addToken(TokenType.quotation_mark);
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
        }
        return m_tokens.ToArray();
    }

    void aSMTokenize()
    {
        string buffer = "";

        if (char.IsLetterOrDigit(peek().Value))
        {
            while (peek().HasValue && peek().Value != '\n')
            {
                buffer += consume();
            }

            buffer = buffer.Trim(' ');
            buffer = Regex.Replace(buffer, @" +", " ");
            addToken(TokenType._asm, buffer);
        }
    }
    void addToken(TokenType type)
    {
        addToken(type, m_buf);
    }
    void addToken(TokenType type, string line)
    {
        m_tokens.Add(new Token() { m_Type = type, m_Value = line, m_Line = m_lineNumber });
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
