﻿namespace HLCLCompiler.Tokenizer
{
    public abstract class Token
    {
        public abstract TokenKind Kind { get; }
        public int m_Line { get; set; }
        public string m_File;
        public override string ToString()
        {
            return "at " + $"{m_Line}".PadLeft(8, '0') + $":{Kind}".PadRight(30, ' ');
        }
    }

    public class EmptyToken : Token
    {
        public override TokenKind Kind { get; } = TokenKind.NONE;
    }

    public sealed class FSASpace : FSA
    {
        private enum State
        {
            START,
            END,
            ERROR,
            SPACE
        };

        private State _state;

        public FSASpace()
        {
            _state = State.START;
        }

        public override void Reset()
        {
            _state = State.START;
        }

        public override FSAStatus GetStatus()
        {
            if (_state == State.START)
            {
                return FSAStatus.NONE;
            }
            if (_state == State.END)
            {
                return FSAStatus.END;
            }
            if (_state == State.ERROR)
            {
                return FSAStatus.ERROR;
            }
            return FSAStatus.RUNNING;
        }

        public override Token RetrieveToken()
        {
            return new EmptyToken();
        }

        public override void ReadChar(char ch)
        {
            switch (_state)
            {
                case State.END:
                case State.ERROR:
                    _state = State.ERROR;
                    break;
                case State.START:
                    if (Utils.IsSpace(ch))
                    {
                        _state = State.SPACE;
                    }
                    else
                    {
                        _state = State.ERROR;
                    }
                    break;
                case State.SPACE:
                    if (Utils.IsSpace(ch))
                    {
                        _state = State.SPACE;
                    }
                    else
                    {
                        _state = State.END;
                    }
                    break;
            }
        }

        public override void ReadEOF()
        {
            switch (_state)
            {
                case State.SPACE:
                    _state = State.END;
                    break;
                default:
                    _state = State.ERROR;
                    break;
            }
        }
    }

    public sealed class FSANewLine : FSA
    {
        private enum State
        {
            START,
            END,
            ERROR,
            NEWLINE
        };

        private State _state;

        public FSANewLine()
        {
            _state = State.START;
        }

        public override void Reset()
        {
            _state = State.START;
        }

        public override FSAStatus GetStatus()
        {
            if (_state == State.START)
            {
                return FSAStatus.NONE;
            }
            if (_state == State.END)
            {
                return FSAStatus.END;
            }
            if (_state == State.ERROR)
            {
                return FSAStatus.ERROR;
            }
            return FSAStatus.RUNNING;
        }

        public override Token RetrieveToken()
        {
            return new EmptyToken();
        }

        public override void ReadChar(char ch)
        {
            switch (_state)
            {
                case State.END:
                case State.ERROR:
                    _state = State.ERROR;
                    break;
                case State.START:
                    if (ch == '\n')
                    {
                        _state = State.NEWLINE;
                    }
                    else
                    {
                        _state = State.ERROR;
                    }
                    break;
                case State.NEWLINE:
                    _state = State.END;
                    break;
            }
        }

        public override void ReadEOF()
        {
            switch (_state)
            {
                case State.NEWLINE:
                    _state = State.END;
                    break;
                default:
                    _state = State.ERROR;
                    break;
            }
        }
    }

}