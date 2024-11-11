using System;
using System.Linq;
using static HexLibrary.HexConverter;

namespace BCGLinker
{
    public class Label
    {
        public string m_Name;
        public int m_Address;
        public bool m_IsGlobal;
        public bool m_IsLocal;
        public string m_File;
        public Section m_Section;

        public bool m_HaveStruct = false;
        public Struct m_Struct;

        public string[] GetAddress()
        {
            return SplitHexString(Convert.ToString(m_Address, 16), 2);
        }
        public string[] GetAddressLong()
        {
            return SplitHexString(Convert.ToString(m_Address, 16), 3);
        }
        public string[] GetAddressFar()
        {
            return SplitHexString(Convert.ToString(m_Address, 16), 4);
        }
        public string[] GetAddress(StructMembers structMembers)
        {
            return SplitHexString(Convert.ToString(m_Address + structMembers.m_offset, 16), 2);
        }
    }
}
