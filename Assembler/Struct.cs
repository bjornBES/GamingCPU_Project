using System.Collections.Generic;

public class Struct
{
    public string m_Name;
    public Label m_BaseLabel;
    public int m_BaseAddress;
    public int m_Size;
    public List<DataFields> m_Fields = new List<DataFields>();
}

public class DataFields
{
    public string m_Name;
    public int m_Offset;
    public int m_Size;
    public bool m_IsArray = false;
}
