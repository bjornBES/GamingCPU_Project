
using Compiler.Lib.Variants;
using static CompilerSettings;

namespace Compiler.ParserNodes
{
    public class NodeTermIntLit
    {
        public Token int_lit;
    };
    public class NodeTermIdent
    {
        public Token ident;
    };
    public class NodeTermString
    {
        public Token str;
    }
    public class NodeTermReference
    {
        public Token reference;
    }
    public class NodeTermParen
    {
        public NodeExpr Expr;
    }
    public class NodeTermCallFunction
    {
        public Token FunctionName;
        public NodeExpr[] args;
    }
    public class NodeTermPointer
    {
        public Token ident;
    }
    public class NodeTerm
    {
        public Variant<NodeTermPointer, NodeTermCallFunction, NodeTermParen, NodeTermReference, NodeTermString, NodeTermIdent, NodeTermIntLit> var;
    }
    public class NodeFunctionArgs
    {
        public Token name;
        public _Type _Type;
        public bool IsOut;
    }


    public class NodeBinExprAdd
    {
        public NodeExpr lhs;
        public NodeExpr rhs;
    }
    public class NodeBinExprMulti
    {
        public NodeExpr lhs;
        public NodeExpr rhs;
    }
    public class NodeBinExprSub
    {
        public NodeExpr lhs;
        public NodeExpr rhs;
    }
    public class NodeBinExprDiv
    {
        public NodeExpr lhs;
        public NodeExpr rhs;
    }
    public class NodeBinExpr
    {
        public Variant<NodeBinExprAdd, NodeBinExprMulti, NodeBinExprSub, NodeBinExprDiv> var;
    }

    public class NodeExpr
    {
        public Variant<NodeTerm, NodeBinExpr> var;
    }

    public class NodeStmtDeclareFunction
    {
        public Token ident;
        public bool IsPublic;
        public NodeFunctionArgs[] args;
	}
    public class NodeStmtEndFunction
    {
        public Token ident;
    }
    public class _Type
    {
        public int TypeSize;
        public bool IsSigned;
    }
    public class NodeStmtPointer
    {
        public Token name;
        public NodeExpr ident;
        public AssignmentOperators _operator;
        public NodeExpr Expr;
    }
    public class NodeStmtDeclareVariabel
    {
        public Token ident;
        public AssignmentOperators _operator;
        public NodeExpr expr;
        public _Type Type;
        public bool IsString;
        public bool IsConst;
        public bool IsPublic;
    }
    public class NodeStmtReAssingnVariabel
    {
        public Token ident;
        public AssignmentOperators _operator;
        public NodeExpr expr;
    }
    public class NodeStmtReturn
    {
        public NodeExpr ReturnExpr;
    }
    public class NodeStmtCallFunction
    {
        public Token FunctionName;
        public NodeExpr[] args;
    }
    public class NodeStmt
	{
        public Token Line;
        public Variant<NodeStmtCallFunction, NodeStmtPointer, NodeStmtReturn, NodeStmtReAssingnVariabel, NodeStmtDeclareVariabel, NodeStmtEndFunction, NodeStmtDeclareFunction> var;
    }
	public class ProgNode
	{
		public NodeStmt[] stmts;
    }
}
