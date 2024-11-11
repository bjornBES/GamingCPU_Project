namespace BCGLinker
{
    public class Section
    {
        public string m_Name;
        public int m_Size;
        public int m_Start;
        public Memory m_Memory;
        public Type m_Type;
        public int m_PCOffset;
        public string OutputFile;

        public bool InSection(int i)
        {
            if (m_Size == 0) return true;

            if (m_Start <= i && m_Start + m_Size > i)
            {
                return true;
            }

            return false;
        }
    }
    public struct Memory
    {
        public string m_Name;
        public int m_Size;
        public int m_Start;
        public Type m_Type;
    }

    public enum Type
    {
        ReadOnly,
        WriteOnly,
        ReadWrite,
    }
}
