public class GeneratorVariabels
{
    public List<string> output { get; set;}
    public NodeStmt[] stmts;
    
    protected Section currentSection = Section.none;

    public bool HasMain = false;

    protected List<Function> functions = new List<Function>();

    protected List<Variabel> variabels = new List<Variabel>();
    public int StackSize = 0;
    protected Stack<int> scopes = new Stack<int>();

    public const Regs ReturnRegister = Regs.A;
    public const Regs SecondRegister = Regs.B;
    public const Regs AddressRegiser = Regs.HL;
    public const Regs ScopeRegister = Regs.BP;

    protected Stack<string> ScopeLables = new Stack<string>();
    protected int ScopeCount = 0;
    protected int LableCount = 0;
    protected object otherResults = "";

    protected Stack<string> BreakLabels = new Stack<string>();
    protected bool DoPushStack = true;
    protected List<DefineEntry> defines = new List<DefineEntry>();
}

public struct DefineEntry
{
    public string name;
    public NodeExpr value;
}