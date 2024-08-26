using System;
using System.Collections.Generic;
using System.Text;

public class Struct
{
    public string m_Name;
    public int m_Size;
    public List<StructMembers> m_StructMembers = new List<StructMembers>();
}

public struct StructMembers
{
    public string m_Name;
    public int m_Size;
}
