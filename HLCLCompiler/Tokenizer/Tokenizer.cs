using System.Collections.Immutable;
using System.Reflection;
using System.Text.RegularExpressions;

namespace HLCLCompiler.Tokenizer
{
    public class Tokenizer
    {
        string m_src;
        int m_index;
        int m_lineNumber = 0;
        int m_colNumber = 0;
        List<Token> m_tokens = new List<Token>();

        ImmutableList<FSA> FSAs;

        public Tokenizer()
        {
            FSAs = ImmutableList.Create<FSA>(
                new FSAFloat(),
                new FSAInt(),
                new FSAIdentifier(),
                new FSAOperator(),
                new FSASpace(),
                new FSANewLine(),
                new FSACharConst(),
                new FSAstring()
                );
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

            while (peek().HasValue)
            {
                if (peek().Value == '/' && peek(1).HasValue && peek(1).Value == '/')
                {
                    consume();
                    consume();
                    while (peek().HasValue && peek().Value != '\n')
                    {
                        consume();
                    }
                }

                char c = consume();
                FSAs.ForEach(fsa =>
                {
                    fsa.ReadChar(c);
                });

                int i = FSAs.FindIndex(fsa =>
                {
                    return fsa.GetStatus() == FSAStatus.RUNNING;
                });
                if (i == -1)
                {
                    int idx = FSAs.FindIndex(fsa => fsa.GetStatus() == FSAStatus.END);
                    if (idx != -1)
                    {
                        if (FSAs[idx].GetType() == typeof(FSANewLine))
                        {
                            m_colNumber = 1;
                            m_lineNumber++;
                        }

                        Token token = FSAs[idx].RetrieveToken();
                        token.m_Line = m_lineNumber;
                        if (token.Kind != TokenKind.NONE)
                        {
                            result.Add(token);
                        }
                        m_index--;
                        FSAs.ForEach(fsa => fsa.Reset());
                    }
                    else
                    {
                        Console.WriteLine("ERROR");
                    }
                }
            }

            FSAs.ForEach(fsa => fsa.ReadEOF());
            // find END
            int idx2 = FSAs.FindIndex(fsa => fsa.GetStatus() == FSAStatus.END);
            if (idx2 != -1)
            {
                Token token = FSAs[idx2].RetrieveToken();
                if (token.Kind != TokenKind.NONE)
                {
                    result.Add(token);
                }
            }
            else
            {
                Console.WriteLine("error");
            }

            return result.ToArray();
        }

        char? peek(int offset = 0)
        {
            if (m_index + offset >= m_src.Length)
                return null;
            return m_src[m_index + offset];
        }
        char consume()
        {
            if (!peek().HasValue)
            {
                return '\0';
            }

            m_colNumber++;
            return m_src[m_index++];
        }
    }
}
