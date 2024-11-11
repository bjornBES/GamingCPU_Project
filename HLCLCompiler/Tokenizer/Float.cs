﻿namespace HLCLCompiler.Tokenizer
{
    /// <summary>
    /// The token representing a floating number.
    /// It can either be a float or double.
    /// </summary>
    public sealed class TokenFloat : Token
    {

        public TokenFloat(float value, string source)
        {
            Value = value;
            Source = source;
        }

        public override TokenKind Kind { get; } = TokenKind.FLOAT;
        public float Value { get; }
        public string Source { get; }

        public override string ToString()
        {
            string str = Kind.ToString() + "(float)";
            return str + ": " + Value + " \"" + Source + "\"";
        }
    }

    /// <summary>
    /// The FSA for scanning a float.
    /// </summary>
    public sealed class FSAFloat : FSA
    {
        private enum State
        {
            START,
            END,
            ERROR,
            D,
            P,
            DP,
            PD,
            DE,
            DES,
            DED,
            PDF,
            DPL
        };

        private string _raw;
        private int _intPart;
        private int _fracPart;
        private int _fracCount;
        private int _expPart;
        private bool _expPos;
        private State _state;

        public FSAFloat()
        {
            _state = State.START;
            _intPart = 0;
            _fracPart = 0;
            _fracCount = 0;
            _expPart = 0;
            _expPos = true;
            _raw = "";
        }

        public override void Reset()
        {
            _state = State.START;
            _intPart = 0;
            _fracPart = 0;
            _fracCount = 0;
            _expPart = 0;
            _expPos = true;
            _raw = "";
        }

        public override FSAStatus GetStatus()
        {
            switch (_state)
            {
                case State.START:
                    return FSAStatus.NONE;
                case State.END:
                    return FSAStatus.END;
                case State.ERROR:
                    return FSAStatus.ERROR;
                default:
                    return FSAStatus.RUNNING;
            }
        }

        public override Token RetrieveToken()
        {
            float val;
            if (_expPos)
            {
                val = (float)((_intPart + _fracPart * Math.Pow(0.1, _fracCount)) * Math.Pow(10, _expPart));
            }
            else
            {
                val = (float)((_intPart + _fracPart * Math.Pow(0.1, _fracCount)) * Math.Pow(10, -_expPart));
            }
            return new TokenFloat(val, _raw.Substring(0, _raw.Length - 1));
        }

        public override void ReadChar(char ch)
        {
            _raw += ch;
            switch (_state)
            {
                case State.ERROR:
                case State.END:
                    _state = State.ERROR;
                    break;

                case State.START:
                    if (char.IsDigit(ch))
                    {
                        _intPart = ch - '0';
                        _state = State.D;
                    }
                    else if (ch == '.')
                    {
                        _state = State.P;
                    }
                    else
                    {
                        _state = State.ERROR;
                    }
                    break;

                case State.D:
                    if (char.IsDigit(ch))
                    {
                        _intPart *= 10;
                        _intPart += ch - '0';
                        _state = State.D;
                    }
                    else if (ch == 'e' || ch == 'E')
                    {
                        _state = State.DE;
                    }
                    else if (ch == '.')
                    {
                        _state = State.DP;
                    }
                    else
                    {
                        _state = State.ERROR;
                    }
                    break;

                case State.P:
                    if (char.IsDigit(ch))
                    {
                        _fracPart = ch - '0';
                        _fracCount = 1;
                        _state = State.PD;
                    }
                    else
                    {
                        _state = State.ERROR;
                    }
                    break;

                case State.DP:
                    if (char.IsDigit(ch))
                    {
                        _fracPart = ch - '0';
                        _fracCount = 1;
                        _state = State.PD;
                    }
                    else if (ch == 'e' || ch == 'E')
                    {
                        _state = State.DE;
                    }
                    else if (ch == 'f' || ch == 'F')
                    {
                        _state = State.PDF;
                    }
                    else if (ch == 'l' || ch == 'L')
                    {
                        _state = State.DPL;
                    }
                    else
                    {
                        _state = State.END;
                    }
                    break;

                case State.PD:
                    if (char.IsDigit(ch))
                    {
                        _fracPart *= 10;
                        _fracPart += ch - '0';
                        _fracCount++;
                        _state = State.PD;
                    }
                    else if (ch == 'e' || ch == 'E')
                    {
                        _state = State.DE;
                    }
                    else if (ch == 'f' || ch == 'F')
                    {
                        _state = State.PDF;
                    }
                    else if (ch == 'l' || ch == 'L')
                    {
                        _state = State.DPL;
                    }
                    else
                    {
                        _state = State.END;
                    }
                    break;

                case State.DE:
                    if (char.IsDigit(ch))
                    {
                        _expPart = ch - '0';
                        _state = State.DED;
                    }
                    else if (ch == '+' || ch == '-')
                    {
                        if (ch == '-')
                        {
                            _expPos = false;
                        }
                        _state = State.DES;
                    }
                    else
                    {
                        _state = State.ERROR;
                    }
                    break;

                case State.DES:
                    if (char.IsDigit(ch))
                    {
                        _expPart = ch - '0';
                        _state = State.DED;
                    }
                    else
                    {
                        _state = State.ERROR;
                    }
                    break;

                case State.DPL:
                    _state = State.END;
                    break;

                case State.DED:
                    if (char.IsDigit(ch))
                    {
                        _expPart *= 10;
                        _expPart += ch - '0';
                        _state = State.DED;
                    }
                    else if (ch == 'f' || ch == 'F')
                    {
                        _state = State.PDF;
                    }
                    else if (ch == 'l' || ch == 'L')
                    {
                        _state = State.DPL;
                    }
                    else
                    {
                        _state = State.END;
                    }
                    break;

                case State.PDF:
                    _state = State.END;
                    break;

                default:
                    _state = State.ERROR;
                    break;
            }

        }

        public override void ReadEOF()
        {
            switch (_state)
            {
                case State.DP:
                case State.PD:
                case State.DED:
                case State.PDF:
                case State.DPL:
                    _state = State.END;
                    break;
                default:
                    _state = State.ERROR;
                    break;
            }
        }

    }
}