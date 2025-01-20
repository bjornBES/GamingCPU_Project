using Compiler;

public struct Variabel
{
    public string Name;
    public ExprType TypeData;
    public Address Address;

    public bool IsInScope;
    public bool IsGlobal;

    public Variabel(string name, ExprType typeData, int stack)
    {
        Name = name;
        TypeData = typeData;
        Address.m_UseStack = true;
        Address.StackLoc = stack;
        Address.m_IsArg = false;
    }
    public Variabel(string name, ExprType typeData, int stack, bool Isarg)
    {
        Name = name;
        TypeData = typeData;
        Address.m_UseStack = true;
        Address.StackLoc = stack;
        Address.m_IsArg = Isarg;
    }
    public Variabel(string name, ExprType typeData, int address, int a)
    {
        Name = name;
        TypeData = typeData;
        Address.m_isGlobal = true;
        Address.m_address = address;
    }

    public Variabel UseStack(int stack, string stackRegister)
    {
        Variabel result = this;
        result.Address.StackLoc = stack;
        result.Address.StackRegister = stackRegister;
        result.Address.m_UseStack = true;
        result.Address.m_IsArg = false;
        return result;
    }
    public Variabel UseArgumentStack(int stack, string stackRegister)
    {
        Variabel result = this;
        result.Address.StackLoc = stack;
        result.Address.StackRegister = stackRegister;
        result.Address.m_UseStack = true;
        result.Address.m_IsArg = true;
        return result;
    }
}
