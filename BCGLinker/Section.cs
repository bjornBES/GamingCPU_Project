namespace BCGLinker
{
    public struct Section
    {
        public string m_Name;
        public int m_size;
        public int m_offset;

        public bool InSection(int i)
        {
            if (m_size == 0) return true;

            if (m_offset <= i && m_offset + m_size > i)
            {
                return true;
            }

            return false;
        }
    }
}
