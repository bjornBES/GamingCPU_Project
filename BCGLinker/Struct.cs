using HexLibrary;
using System;
using System.Collections.Generic;
using System.Text;

namespace BCGLinker
{
    public class Struct
    {
        public string m_Name;
        public int m_Size;
        public List<StructMembers> m_StructMembers = new List<StructMembers>();

        public string[] GetSize()
        {
            return HexConverter.SplitHexString(Convert.ToString(m_Size, 16));
        }
    }

    public struct StructMembers
    {
        public string m_Name;
        public int m_Size;
        public int m_offset;

        public string[] GetSize()
        {
            return HexConverter.SplitHexString(Convert.ToString(m_Size, 16));
        }
    }
}
