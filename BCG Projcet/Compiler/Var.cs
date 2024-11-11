using Compiler;

public struct Var
{
    public string m_Name;
    public TypeData m_TypeData;
    public Address m_Address;

    public Var(string name, TypeData typeData, int stack)
    {
        m_Name = name;
        m_TypeData = typeData;
        m_Address.m_UseStack = true;
        m_Address.m_StackLoc = stack;
        m_Address.m_IsArg = false;
    }
    public Var(string name, TypeData typeData, int stack, bool Isarg)
    {
        m_Name = name;
        m_TypeData = typeData;
        m_Address.m_UseStack = true;
        m_Address.m_StackLoc = stack;
        m_Address.m_IsArg = Isarg;
    }
    public Var(string name, TypeData typeData, int address, int a)
    {
        m_Name = name;
        m_TypeData = typeData;
        m_Address.m_isGlobal = true;
        m_Address.m_address = address;
    }
}
