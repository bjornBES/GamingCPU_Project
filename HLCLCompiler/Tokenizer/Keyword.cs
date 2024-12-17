using System;
using System.Collections.Generic;

namespace HLCLCompiler.Tokenizer {
    public enum KeywordVal {
        STRUCT,
        BREAK,
        ELSE,
        SWITCH,
        CASE,
        ENUM,
        TYPEDEF,
        EXTERN,
        RETURN,
        CONST,
        CONTINUE,
        FOR,
        CALL,
        DEFAULT,
        GOTO,
        SIZEOF,
        VOLATILE,
        DO,
        IF,
        WHILE,
        FUNCTION,
        AS,

        CHAR,
        BYTE,
        SHORT,
        INT,
        FLOAT,
        UNSIGNED,
        SIGNED,

        FARPTR,
        SHORTPTR,
    }

    public class TokenKeyword : Token {
        public TokenKeyword(KeywordVal val) {
            Val = val;
        }

        public override TokenKind Kind { get; } = TokenKind.KEYWORD;
        public KeywordVal Val { get; }
        public static Dictionary<string, KeywordVal> Keywords { get; } = new Dictionary<string, KeywordVal>(StringComparer.InvariantCultureIgnoreCase) {
            { "CALL",       KeywordVal.CALL         },            
            { "INT",        KeywordVal.INT          },
            { "STRUCT",     KeywordVal.STRUCT       },
            { "BREAK",      KeywordVal.BREAK        },
            { "ELSE",       KeywordVal.ELSE         },
            { "SWITCH",     KeywordVal.SWITCH       },
            { "CASE",       KeywordVal.CASE         },
            { "ENUM",       KeywordVal.ENUM         },
            { "TYPEDEF",    KeywordVal.TYPEDEF      },
            { "CHAR",       KeywordVal.CHAR         },
            { "BYTE",       KeywordVal.BYTE         },
            { "EXTERN",     KeywordVal.EXTERN       },
            { "RETURN",     KeywordVal.RETURN       },
            { "CONST",      KeywordVal.CONST        },
            { "FLOAT",      KeywordVal.FLOAT        },
            { "SHORT",      KeywordVal.SHORT        },
            { "UNSIGNED",   KeywordVal.UNSIGNED     },
            { "CONTINUE",   KeywordVal.CONTINUE     },
            { "FOR",        KeywordVal.FOR          },
            { "SIGNED",     KeywordVal.SIGNED       },
            { "DEFAULT",    KeywordVal.DEFAULT      },
            { "GOTO",       KeywordVal.GOTO         },
            { "SIZEOF",     KeywordVal.SIZEOF       },
            { "VOLATILE",   KeywordVal.VOLATILE     },
            { "DO",         KeywordVal.DO           },
            { "IF",         KeywordVal.IF           },
            { "WHILE",      KeywordVal.WHILE        },
            
            { "FUNCTION",   KeywordVal.FUNCTION     },
            { "FUNC",       KeywordVal.FUNCTION     },

            { "__SHORT__",  KeywordVal.SHORTPTR     },
            { "__FAR__",    KeywordVal.FARPTR       },
        };

        public override string ToString() {
            return Kind + ": " + Val;
        }

    }
}