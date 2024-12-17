using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HLCLCompiler.Types;

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
                if (termIntLit.value.Val == 0)
                {
                    SEZ(Regs.AX);
                }
                else
                {
                    MOV(Regs.AX, termIntLit.value.Val);
                }
                PUSH(Regs.AX);
                return;
            }
            else if (nodeTerm.IsType(out NodeTermIdent termIdent))
            {
                if (variabels.Find(var => var.Name == termIdent.value.Val) != null)
                {
                    int index = variabels.FindIndex(var => var.Name == termIdent.value.Val);
                    Variabel var = variabels[index];
                    MOV(Regs.AX, var.GetAddress(this));
                    PUSH(Regs.AX);
                }
                else
                {
                    addLine(" ; Ident term not used");
                }
            }
            else if (nodeTerm.IsType(out NodeTermReference termReference))
            {
                if (variabels.Find(var => var.Name == termReference.value.Val) != null)
                {
                    int index = variabels.FindIndex(var => var.Name == termReference.value.Val);
                    Variabel var = variabels[index];
                    LDA(Regs.AX, var.GetAddress(this));
                    PUSH(Regs.AX);
                }
                else
                {
                    addLine(" ; Ident term not used");
                }
            }
            else if (nodeTerm.IsType(out NodeTermCall termCall))
            {
                gen_CallFunction(termCall.stmtCall);
                PUSH(Regs.AX);
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

            Variabel[] arguments = function.GetArgumentVariabels(StackSize);
            for (int i = 0; i < arguments.Length; i++)
            {
                variabels.Add(arguments[i]);
                StackSize += 4;
            }
            FuncStart();
            Enter();
            variabels.Add(new Variabel("RetResult", new IntType(), AddressHelper.GetStack(0, "BP"), functions.Last()));
            ExpandStack(4);
            addLine("; return value is at [BP]");
            Pushr();
            enterScope();

            for (int i = 0; i < declareFunctionNode.stmts.Count; i++)
            {
                gen_stmt(declareFunctionNode.stmts[i]);
            }

            FuncEnd();

        }

        void gen_DeclareVariabel(NodeStmtDeclareVariabel declareVariabelNode)
        {
            Text();

            Address address = AddressHelper.GetStack(StackSize, "SP");
            gen_Expr(declareVariabelNode.expr);

            variabels.Add(new Variabel(declareVariabelNode.name, declareVariabelNode.type,address, functions.Last()));
        } 

        void gen_AssignVariabel(NodeStmtAssignVariabel assignVariabelNode)
        {
            string name = assignVariabelNode.name;
            if (!isVariabel(name, out Variabel result))
            {
                Console.WriteLine($"Error variabel {name} not Declared");
                return;
            }

            string address = result.GetAddress(this);
            gen_Expr(assignVariabelNode.expr);
            POP(Regs.AX);
            if (assignVariabelNode.OperatorVal == Tokenizer.OperatorVal.ASSIGN)
            {
                if (name.StartsWith("*") || name.EndsWith("]"))
                {
                    int offset = 0;
                    if (name.EndsWith("]"))
                    {
                        offset = int.Parse(name.Split('[').Last().TrimEnd(']'));
                    }
                    LDA(Regs.HL, address);
                    if (offset != 0)
                    {
                        ADD(Regs.HL, offset);
                    }
                    MOV(Regs.HL, true, Regs.AX);
                }
                else
                {
                    MOV(address, Regs.AX);
                }
            }
        }

        void gen_CallFunction(NodeStmtCall functionCallNode)
        {
            for (int i = 0; i < functionCallNode.args.Count; i++)
            {
                gen_Expr(functionCallNode.args[i]);
            }
            CALL(functionCallNode.name);
        }

        public void gen_stmt(NodeStmt stmt)
        {
            status.Add($"on line {outputCode.Count} stackSize is {StackSize}");
            if (stmt.stmt == null)
            {
                return;
            }

            Text();

            if (stmt.IsType(out NodeStmtDeclareFunction declareFunctionNode))
            {
                gen_DeclareFunction(declareFunctionNode);
            }
            else if (stmt.IsType(out NodeStmtDeclareVariabel declareVariabelNode))
            {
                gen_DeclareVariabel(declareVariabelNode);
            }
            else if (stmt.IsType(out NodeStmtAssignVariabel assignVariabelNode))
            {
                gen_AssignVariabel(assignVariabelNode);
            }
            else if (stmt.IsType(out NodeStmtCall functionCallNode))
            {
                gen_CallFunction(functionCallNode);
            }

            if (CompilerSettings.m_Debug == true)
            {
                status.Add($"on line {outputCode.Count} stackSize is {StackSize}");
                File.WriteAllLines("./Debug.txt", status);
            }
        }

        public void gen_prog(ProgNode progNode)
        {

            for (int i = 0; i < progNode.prog.Count; i++)
            {
                gen_stmt(progNode.prog[i]);
            }

        }

        bool isVariabel(string name, out Variabel result)
        {
            name = name.TrimStart('*');
            int index = name.IndexOf('[');
            if (index != -1)
            {
                name = name.Substring(0, index);
            }
            
            for (int i = 0; i < variabels.Count; i++)
            {
                if (variabels[i].Name == name)
                {
                    result = variabels[i];
                    return true;
                }
            }
            result = null;
            return false;
        }
    }
}
