﻿public class Tokenizer
{
    string m_src;
    int m_index;
    string buf = "";
    int line_count = 0;
    List<Token> m_tokens = new List<Token>();

    public Tokenizer(string src)
    {
        m_src = src;
        m_index = 0;
    }

    public Token[] Tokenize()
    {
        while (peek().HasValue)
        {
            buf = "";
            if (peek().Value == '/' && peek(1).HasValue && peek(1).Value == '/')
            {
                while (peek().HasValue && peek().Value != '\n')
                {
                    consume();
                }
            }
            if (peek().Value == '/' && peek(1).HasValue && peek(1).Value == '*')
            {
                consume();
                consume();
                while (peek().HasValue)
                {
                    if (peek().Value == '*' && peek(1).HasValue && peek(1).Value == '/')
                    {
                        break;
                    }
                    consume();
                }
                consume();
                consume();
            }
            else if (peek().Value == '\'')
            {
                consume();
                buf = Convert.ToByte(consume()).ToString();
                if (!(peek().HasValue && peek().Value == '\''))
                {
                    throw new NotImplementedException();
                }
                addToken(TokenType.int_lit);
                consume();
            }
            if (peek().Value == '0' &&
                peek(1).HasValue && peek(1).Value == 'x')
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
                    buf += consume();
                }

                buf = Convert.ToString(Convert.ToInt32(buf, 16));

                addToken(TokenType.int_lit);

            }
            if (char.IsLetter(peek().Value) || peek().Value == '_')
            {
                buf += consume();
                while (peek().HasValue && (char.IsLetterOrDigit(peek().Value) || peek().Value == '_'))
                {
                    buf += consume();
                }

                switch (buf.ToLower())
                {
                    case "struct":  addToken(TokenType._struct);    break;
                    
                    case "end":     addToken(TokenType.end);        break;
                    case "asm":     addToken(TokenType._asm);       break;
                    case "sizeof":  addToken(TokenType._sizeof);    break;
                    case "call":    addToken(TokenType.call);       break;
                    
                    case "return":  addToken(TokenType._return);    break;

                    case "public":  addToken(TokenType._public);    break;

                    case "near":    addToken(TokenType._near);      break;
                    case "short":   addToken(TokenType._short);     break;
                    case "long":    addToken(TokenType._long);      break;
                    case "far":     addToken(TokenType._far);       break;
                    
                    case "void":    addToken(TokenType._void);      break;

                    case "char":    addToken(TokenType._char);      break;
                    case "uint8":   addToken(TokenType.uint8);      break;
                    case "int8":    addToken(TokenType.int8);       break;

                    case "uint16":  addToken(TokenType.uint16);     break;
                    case "int16":   addToken(TokenType.int16);      break;
                    
                    case "tbyte":   addToken(TokenType.uint24);     break;
                    case "uint24":  addToken(TokenType.uint24);     break;
                    case "sint24":  addToken(TokenType.int24);      break;
                    
                    case "uint32":  addToken(TokenType.uint32);     break;
                    case "sint32":  addToken(TokenType.int32);      break;

                    case "string":  addToken(TokenType._string);    break;

                    case "const":   addToken(TokenType._const);     break;

                    default:
                        addToken(TokenType.ident);
                        break;
                }
            }
            else if (new string("0123456789").Contains(peek().Value))
            {
                buf += consume();
                while (peek().HasValue && (char.IsDigit(peek().Value) || peek().Value == '.'))
                {
                    buf += consume();
                }
                addToken(TokenType.int_lit);
            }
            else if (char.IsSeparator(peek().Value))
            {
                consume();
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
                    case '&':
                        consume();
                        addToken(TokenType.ampersand);
                        break;
                    case ',':
                        consume();
                        addToken(TokenType.comma);
                        break;
                    case '\"':
                        consume();
                        addToken(TokenType.quotation_mark);
                        while (peek().HasValue && peek().Value != '\"')
                        {
                            buf += consume();
                        }
                        addToken(TokenType.ident);
                        buf = "";
                        consume();
                        addToken(TokenType.quotation_mark);
                        break;
                    case '\n':
                        consume();
                        line_count++;
                        break;
                    default: break;
                }
            }
        }
        return m_tokens.ToArray();
    }
    void addToken(TokenType type)
    {
        m_tokens.Add(new Token() { Type = type, value = buf, line = line_count });
    }
    char? peek(int offset = 0)
    {
        if (m_index + offset >= m_src.Length) return null;
        return m_src[m_index + offset];
    }
    char consume()
    {
        return m_src[m_index++];
    }
}
