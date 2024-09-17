
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

    public class NodeScope
    {
        public NodeStmt[] Stmts;
    }

    public class NodeStmtDeclareFunction
    {
        public Token ident;
        public bool IsPublic;
        public NodeFunctionArgs[] args;
        public NodeScope Scope;
    }
    public class NodeStmtEndFunction
    {
        public Token ident;
    }
    public class _Type
    {
        public bool m_IsPointer;
        public int m_PointerSize;

        public int m_TypeSize;
        public bool m_IsSigned;

        public bool m_IsPublic;
        public bool m_IsGlobal;
        public bool m_IsLocal;
        public bool m_IsConst;
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
        public Variant<NodeStmtCallFunction, NodeStmtPointer, NodeStmtReturn, NodeStmtReAssingnVariabel, NodeStmtDeclareVariabel, NodeStmtEndFunction, NodeStmtDeclareFunction, NodeScope> var;
    }
	public class ProgNode
	{
		public NodeStmt[] stmts;
    }
}
