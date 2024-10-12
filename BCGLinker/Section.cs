namespace BCGLinker
{
    public struct Section
    {
        public string m_Name;
        public int m_Size;
        public int m_Offset;

        public bool InSection(int i)
        {
            if (m_Size == 0) return true;

            if (m_Offset <= i && m_Offset + m_Size > i)
            {
                return true;
            }

            return false;
        }
    }
}
