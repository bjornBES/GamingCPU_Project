using Compiler.Lib.Variants;

public class NodeTermIntlit : ITerm
{
    public string value { get; set; }
}
public class NodeTermIdent : ITerm
{
    public string ident { get; set; }
}
public class NodeTermDereference : ITerm
{
    public NodeTermIdent ident{ get; set; }
}
public interface ITerm
{

}
public class NodeBinExprAdd : IBinExpr
{
    public NodeExpr lhs { get; set; }
    public NodeExpr rhs { get; set;}
}
public class NodeBinExprSub : IBinExpr
{
    public NodeExpr lhs { get; set; }
    public NodeExpr rhs { get; set;}
}
public class NodeBinExprMul : IBinExpr
{
    public NodeExpr lhs { get; set; }
    public NodeExpr rhs { get; set;}
}
public class NodeBinExprDiv : IBinExpr
{
    public NodeExpr lhs { get; set; }
    public NodeExpr rhs { get; set;}
}

public interface IBinExpr
{

}
public class NodeBinExpr : IExpr
{
    public object value { get; set; }
    public bool IsType<T>(out T result) where T : IBinExpr
    {
        if (value.GetType() == typeof(T))
        {
            result = (T)value;
            return true;
        }
        result = default;
        return false;
    }
}
public class NodeExprCompersion : IExpr
{
    public NodeExpr lhs {get; set;}
    public NodeExpr rhs {get; set;}
    public CompersionOperators compersionOperators {get; set;}
}
public interface IExpr
{

}
public class NodeExpr
{
    public object expr { get; set; }
    public bool IsType<T>(out T result) where T : IExpr
    {
        if (expr.GetType() == typeof(T))
        {
            result = (T)expr;
            return true;
        }
        result = default;
        return false;
    }
}
public class NodeTerm : IExpr
{
    public object term { get; set; }
    public bool IsType<T>(out T result) where T : ITerm
    {
        if (term.GetType() == typeof(T))
        {
            result = (T)term;
            return true;
        }
        result = default;
        return false;
    }
}

public class NodeStmtDeclareFunction : IStmt
{
    public string Name { get; set; }
    public NodeScope Scope { get; set; }
}
public class NodeStmtDeclareVariabel : IStmt
{
    public string Name { get; set; }
    public ExprType type { get; set; }
    public NodeExpr expr { get; set; }
    public AssignmentOperators assignmentOperator { get; set; }
}
public class NodeStmtReassignVariabel : IStmt
{
    public string Name { get; set; }
    public NodeExpr expr { get; set; }
    public AssignmentOperators assignmentOperator { get; set; }
}
public class NodeStmtWhile : IStmt
{
    public NodeExpr Expr{ get; set; }
    public NodeScope Scope{ get; set; }
}
public class NodeIfPredElif : IStmtIf
{
    public NodeExpr Expr{ get; set; }
    public NodeScope Scope { get; set; }
    public object pred { get; set; } = null;
    public bool IsType<T>(out T result) where T : IStmtIf
    {
        if (pred == null)
        {
            result = default;
            return false;
        }
        if (pred.GetType() == typeof(T))
        {
            result = (T)pred;
            return true;
        }
        result = default;
        return false;
    }
}
public class NodeIfPredElse : IStmtIf
{
    public NodeScope Scope { get; set; }
}
public interface IStmtIf
{

}
public class NodeStmtIf : IStmt
{
    public NodeExpr Expr{ get; set; }
    public NodeScope Scope { get; set; }
    public object pred { get; set; } = null;
    public bool IsType<T>(out T result) where T : IStmtIf
    {
        if (pred.GetType() == typeof(T))
        {
            result = (T)pred;
            return true;
        }
        result = default;
        return false;
    }
}
public class NodeStmtBreak : IStmt
{

}
public class NodeStmtContinue : IStmt
{

}
public class NodeStmtAsm : IStmt
{
    public string asmCode { get; set; }
    public AsmArgument[] arguments { get; set; }
}
public class NodeStmtDeclareDefine : IStmt
{
    public string name { get; set; }
    public NodeExpr expr{ get; set; }
}
public class NodeStmtDefineIf : IStmt
{
    public string Define { get; set; }
    public CompersionOperators compersionOperators{ get; set; }
    public NodeExpr expr { get; set; }
    public NodeScope scope { get; set; }
}
public class NodeStmtDereferenceVariabel : IStmt
{
    public string Name { get; set; }
    public NodeExpr expr { get; set; }
    public AssignmentOperators assignmentOperator { get; set; }
}

public struct AsmArgument
{
    public int placement { get; set; }
    public NodeExpr value { get; set; }
}

public class NodeScope
{
    public NodeStmt[] stmts { get; set; }
}

public interface IStmt
{

}
public class NodeStmt
{
    public int lineNumberInSrc { get; set; }
    public string lineNumber { get; set; }
    public object stmt { get; set; }
    public T GetType<T>() where T : IStmt
    {
        if (stmt.GetType() == typeof(T))
        {
            return (T)stmt;
        }
        return default;
    }
    public bool IsType<T>(out T result) where T : IStmt
    {
        if (stmt == null)
        {
            result = default;
            return false;
        }
        if (stmt.GetType() == typeof(T))
        {
            result = (T)stmt;
            return true;
        }
        result = default;
        return false;
    }
}

public class ProgNode
{
    public NodeStmt[] stmts { get; set; }
}