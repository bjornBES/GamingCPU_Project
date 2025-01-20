using Compiler;

public struct Function
{
    public string Name;
    public Argument[] Arguments;
    public int ArgumentSize;

    public string FunctionLabelEnd 
    { 
        get
        {
            return $"_Exit{FunctionLabelStart}";
        } 
    }
    public string FunctionLabelStart
    { 
        get
        {
            return $"_{Name}";
        } 
    }

    public override bool Equals(object obj)
    {
        if (obj is not string)
        {
            return false;
        }

        return (string)obj == Name;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

public struct Argument
{
    public string m_Name;
    public ExprType m_Type;
}
