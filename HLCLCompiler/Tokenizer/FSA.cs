using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLCLCompiler.Tokenizer
{
    public enum FSAStatus
    {
        NONE,
        END,
        RUNNING,
        ERROR
    }

    public abstract class FSA
    {
        public abstract FSAStatus GetStatus();
        public abstract void ReadChar(char ch);
        public abstract void Reset();
        public abstract void ReadEOF();
        public abstract Token RetrieveToken();
    }
}
