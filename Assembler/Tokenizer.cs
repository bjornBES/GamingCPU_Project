using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using assembler.global;

namespace assembler
{
    public class Tokenizer
    {
#pragma warning disable IDE1006 // Naming Styles
        public List<Token> m_tokens = new List<Token>();
        public string m_src;

        public int m_index = 0;

        string m_file;
        string buf = "";
        long line = 0;
        long col = 0;
#pragma warning restore IDE1006 // Naming Styles
        
        public static string tokenToString(TokenType type)
        {
            switch (type)
            {
                case TokenType.int_lit:
                    break;
                case TokenType.address_lit:
                    break;
                case TokenType.float_lit:
                    break;
                case TokenType.ident:
                    return "Identifier";
                case TokenType._long:
                case TokenType._struct:
                case TokenType._byte:
                case TokenType._word:
                case TokenType._tbyte:
                case TokenType._dword:
                case TokenType._string:
                case TokenType._float:
                case TokenType.org:
                case TokenType.label:
                case TokenType.global:
                    return type.ToString().Replace("_", "");
                case TokenType.dollar_sign:
                case TokenType.quotation_mark:
                case TokenType.open_paren:
                case TokenType.close_paren:
                case TokenType.semi:
                case TokenType.eq:
                case TokenType.plus:
                case TokenType.star:
                case TokenType.minus:
                case TokenType.fslash:
                case TokenType.bslash:
                case TokenType.open_curly:
                case TokenType.close_curly:
                case TokenType.open_square:
                case TokenType.close_square:
                case TokenType.colon:
                case TokenType.period:
                case TokenType.comma:
                case TokenType.at:
                    return ((char)type).ToString();
            }
            Console.WriteLine("FUCK");
            return "FUCK";
        }

        public void Build(string src)
        {
            m_src = src;

            Tokenize();
        }

        void Tokenize()
        {
            m_index = 0;
            line = -1;
            col = 1;
            while (peek().HasValue)
            {
                buf = "";
                if (peek().Value == '0' &&
                    peek(1).HasValue && peek(1).Value == 'x')
                {
                    consume();
                    consume();

                    while (peek().HasValue && (IsAsciiHexDigit(peek().Value) || peek().Value == '_'))
                    {
                        if (peek().Value == '_')
                        {
                            consume();
                            continue;
                        }
                        buf += consume();
                    }

                    int dec = Convert.ToInt32(buf, 16);
                    buf = dec.ToString();
                        addToken(TokenType.int_lit);
                }
                else if (peek().Value == ';')
                {
                    while (peek().HasValue && peek().Value != '\n')
                    {
                        consume();
                    }
                }
                else if (peek().Value == '"')
                {
                    consume();
                    addToken(TokenType.quotation_mark);
                    while (peek().HasValue && peek().Value != '\"')
                    {
                        buf += consume();
                    }
                    addToken(TokenType.ident);
                    buf = "";
                    if (peek().Value != '\"')
                    {
                        AssemblerError.ExpectedChar('"', m_file, line, col);
                    }
                    consume();
                    addToken(TokenType.quotation_mark);

                }
                else if (char.IsLetter(peek().Value) || peek().Value == '_')
                {
                    char tokenBefore = peek(-1).Value;
                    while (peek().HasValue && (char.IsLetterOrDigit(peek().Value) || peek().Value == '_'))
                    {
                        buf += consume();
                    }

                    switch (buf)
                    {
                        case "byte":
                            addToken(TokenType._byte);
                            break;
                        case "word":
                            addToken(TokenType._word);
                            break;
                        case "tbyte":
                            addToken(TokenType._tbyte);
                            break;
                        case "dword":
                            addToken(TokenType._dword);
                            break;
                        case "long":
                            addToken(TokenType._long);
                            break;
                        case "float":
                            addToken(TokenType._float);
                            break;
                        default:
                            if (((RegisterInfo)buf).m_Register != Register.none && AllRegister.s_registerInfos.Contains((RegisterInfo)buf))
                            {
                                buf = buf.ToUpper();
                                addToken(TokenType.register);
                            }
                            else if (tokenBefore == '@')
                            {
                                addToken(TokenType.address_label);
                            }
                            else if (peek().HasValue && peek().Value == ':')
                            {
                                consume();
                                addToken(TokenType.label);
                            }
                            else if (tokenBefore == '[')
                            {
                                addToken(TokenType.address_label);
                            }
                            else if (((InstructionInfo)buf).m_Instruction != Instruction.none && AllInstruction.s_instructionInfo.Contains((InstructionInfo)buf))
                            {
                                buf = buf.ToUpper();
                                addToken(TokenType.instruction);
                            }
                            break;
                    }
                }
                else if (peek().Value == '.')
                {
                    if (peek(1).HasValue && char.IsDigit(peek(1).Value))
                    {
                        while (peek().HasValue && (char.IsDigit(peek().Value) || peek().Value == '.'))
                        {
                            buf += consume();
                        }

                        if (peek().HasValue && peek().Value == 'f')
                        {
                            consume();
                        }
                        else
                        {
                            AssemblerError.ExpectedChar('f', m_file, line, col);
                        }

                        addToken(TokenType.float_lit);
                        
                        continue;
                    }
                    consume();
                    while (peek().HasValue && (char.IsLetter(peek().Value) || peek().Value == '_'))
                    {
                        buf += consume();
                    }

                    switch (buf)
                    {
                        case "newfile":
                            buf = "";
                            while (peek().HasValue && peek().Value != '\n')
                            {
                                buf += consume();
                            }
                            m_file = buf;
                            addToken(TokenType.newfile);
                            break;
                        case "byte":
                        case "db":
                            addToken(TokenType._byte);
                            break;
                        case "word":
                        case "dw":
                            addToken(TokenType._word);
                            break;
                        case "tbyte":
                        case "dt":
                            addToken(TokenType._tbyte);
                            break;
                        case "dword":
                        case "dd":
                            addToken(TokenType._dword);
                            break;
                        case "rb":
                        case "resb":
                        case "rbyte":
                        case "resbyte":
                            addToken(TokenType.rbyte);
                            break;
                        case "rw":
                        case "resw":
                        case "rword":
                        case "resword":
                            addToken(TokenType.rword);
                            break;
                        case "rt":
                        case "rest":
                        case "rtbyte":
                        case "restbyte":
                            addToken(TokenType.rtbyte);
                            break;
                        case "rd":
                        case "resd":
                        case "rdword":
                        case "resdword":
                            addToken(TokenType.rdword);
                            break;
                        case "float":
                        case "df":
                            addToken(TokenType._float);
                            break;
                        case "org":
                            addToken(TokenType.org);
                            break;
                        case "global":
                            addToken(TokenType.global);
                            break;
                        case "section":
                            addToken(TokenType.section);
                            buf = "";

                            if (peek().HasValue && peek().Value == ' ')
                            {
                                consume();
                            }

                            while (peek().HasValue && (char.IsLetter(peek().Value) || peek().Value == '_'))
                            {
                                buf += consume();
                            }

                            if (buf == AssemblerSettings.TEXTSectionName)
                            {
                                addToken(TokenType.text);
                            }
                            else if (buf == AssemblerSettings.DATASectionName)
                            {
                                addToken(TokenType.data);
                            }
                            else if (buf == AssemblerSettings.BSSSectionName)
                            {
                                addToken(TokenType.bss);
                            }
                            else if (buf == AssemblerSettings.RDATASectionName)
                            {
                                addToken(TokenType.rdata);
                            }

                            break;
                        case "include":
                        case "includeil":
                            while (peek().HasValue && peek().Value != '\n')
                            {
                                consume();
                            }
                            break;
                        default:
                            Console.WriteLine("001:Tokenizer: buf =" + buf);
                            Environment.Exit(1);
                            break;
                    }
                }
                else if (char.IsDigit(peek().Value) || peek().Value == '.')
                {
                    bool isFloat = false;
                    while (peek().HasValue && (char.IsDigit(peek().Value) || peek().Value == '.'))
                    {
                        if (peek().Value == '.')
                        {
                            isFloat = true;
                        }
                        buf += consume();
                    }

                    if (isFloat)
                    {
                        if (peek().HasValue && peek().Value == 'f')
                        {
                            consume();
                            addToken(TokenType.float_lit);
                        }
                        else
                        {
                            AssemblerError.ExpectedChar('f', m_file, line, col);
                        }
                    }
                    else
                    {
                        addToken(TokenType.int_lit);
                    }
                }
                else if (char.IsSeparator(peek().Value))
                {
                    consume();
                }
                else if (peek().Value == '\n')
                {
                    line++;
                    col = 1;
                    consume();
                    addToken(TokenType.newline);
                }
                else if (char.IsWhiteSpace(peek().Value))
                {
                    consume();
                }
                else
                {
                    switch (peek().Value)
                    {
                        case ',':
                            consume();
                            addToken(TokenType.comma);
                            break;
                        case '\n':
                            line++;
                            col = 1;
                            consume();
                            addToken(TokenType.newline);
                            break;
                        case '$':
                            consume();
                            addToken(TokenType.dollar_sign);
                            break;
                        case '[':
                            consume();
                            addToken(TokenType.open_square);
                            break;
                        case ']':
                            consume();
                            addToken(TokenType.close_square);
                            break;
                        case '(':
                            consume();
                            addToken(TokenType.open_paren);
                            break;
                        case ')':
                            consume();
                            addToken(TokenType.close_paren);
                            break;
                        case '+':
                            consume();
                            addToken(TokenType.plus);
                            break;
                        case '-':
                            consume();
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
                        case ':':
                            consume();
                            addToken(TokenType.colon);
                            break;
                        case '@':
                            consume();
                            addToken(TokenType.at);
                            break;
                        case '~':
                            consume();
                            addToken(TokenType.tilde);
                            break;
                        case '&':
                            consume();
                            addToken(TokenType.ampersand);
                            break;
                        default:
                            Console.WriteLine("002:Tokenizer: peek() = " + peek());
                            Environment.Exit(1);
                            break;
                    }
                }
            }
        }

        void addToken(TokenType type)
        {
            m_tokens.Add(new Token() { type = type, ident = buf, line = line, column = col, m_file = m_file });
        }
        char? peek(int offset = 0)
        {
            if (m_index + offset >= m_src.Length)
                return null;
            return m_src[m_index + offset];
        }
        char consume()
        {
            col++;
            return m_src[m_index++];
        }
        bool IsAsciiHexDigit(char c)
        {
            return new string("abcdefABCDEF1234567890").Contains(c);
        }
    }
}
