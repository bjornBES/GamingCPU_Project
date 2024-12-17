using HLCLCompiler.Tokenizer;
using HLCLCompiler.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLCLCompiler
{
    public class NodeTermIntLit
    {
        public TokenInt value { get; set; }
    }
    public class NodeTermIdent
    {
        public TokenIdentifier value { get; set; }
    }
    public class NodeTermReference
    {
        public TokenIdentifier value { get; set; }
    }
    public class NodeTermCall
    {
        public NodeStmtCall stmtCall{ get; set; }
    }
    public class NodeExpr
    {
        public object expr { get; set; }
        public bool IsType<T>(out T result)
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
    public class NodeTerm
    {
        public object term { get; set; }
        public bool IsType<T>(out T result)
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

    public interface IStmt
    {

    }
    public class NodeStmtCall : IStmt
    {
        public string name { get; set; }
        public List<NodeExpr> args { get; set; }
    }
    public class NodeStmtAssignVariabel : IStmt
    {
        public string name { get; set; }
        public NodeExpr expr{ get; set; }
        public OperatorVal OperatorVal{ get; set; }
    }
    public class NodeStmtDeclareVariabel : IStmt
    {
        public string name { get; set; }
        public ExprType type { get; set; }
        public NodeExpr expr { get; set; }
        public OperatorVal OperatorVal { get; set; }
    }
    public class NodeStmtDeclareFunction : IStmt
    {
        public string name { get; set; }
        public Argument[] arguments { get; set; }
        public int LineNumber { get; set; }
        public List<NodeStmt> stmts { get; set; } = new List<NodeStmt>();
    }

    public class NodeStmt
    {
        public object stmt { get; set; }
        public bool IsType<T>(out T result) where T : IStmt
        {
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
        public List<NodeStmt> prog { get; set; } = new List<NodeStmt>();
    }

    public class Argument
    {
        public string Name { get; set; }
        public ExprType Type { get; set; }
        public int ArgumentIndex { get; set; }
    }
}
