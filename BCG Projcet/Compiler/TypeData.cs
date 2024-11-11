using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public class TypeData
    {
        public bool m_IsPointer;
        public int m_PointerSize;

        public int m_TypeSize;
        public bool m_IsSigned;

        public bool m_IsPublic;
        public bool m_IsGlobal;
        public bool m_IsLocal;

        public bool m_IsConst;
    }
}
