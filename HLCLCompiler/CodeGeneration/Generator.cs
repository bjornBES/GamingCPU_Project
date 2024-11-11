using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLCLCompiler.CodeGeneration
{

    public class Generator : AsmGenState
    {

        public Generator()
        {
            currentSection = Section.text;
        }

        void gen_Term(NodeTerm nodeTerm)
        {
            if (nodeTerm.IsType(out NodeTermIntLit termIntLit))
            {

            }
        }

        void gen_Expr(NodeExpr nodeExpr)
        {

            if (nodeExpr.IsType(out NodeTerm term))
            {
                gen_Term(term);
            }
        }

        void gen_DeclareFunction(NodeStmtDeclareFunction declareFunctionNode)
        {
            Text();

            Function function = new Function(declareFunctionNode);
            functions.Add(function);

            FuncStart();

            function.GetArgumentVariabels(StackSize);

            for (int i = 0; i < declareFunctionNode.stmts.Count; i++)
            {
                gen_stmt(declareFunctionNode.stmts[i]);
            }

            FuncEnd();

        }

        void gen_DeclareVariabel(NodeStmtDeclareVariabel declareVariabelNode)
        {
            Text();

        }

        public void gen_stmt(NodeStmt stmt)
        {
            Text();

            if (stmt.IsType(out NodeStmtDeclareFunction declareFunctionNode))
            {
                gen_DeclareFunction(declareFunctionNode);
            }
            else if (stmt.IsType(out NodeStmtDeclareVariabel declareVariabelNode))
            {
                gen_DeclareVariabel(declareVariabelNode);
            }

        }

        public void gen_prog(ProgNode progNode)
        {

            for (int i = 0; i < progNode.prog.Count; i++)
            {
                gen_stmt(progNode.prog[i]);
            }

        }
    }
}
