using System.Text.RegularExpressions;
using static CompilerSettings;

public class Generator : GeneratorInstrctions
{
    string genTerm(NodeTerm nodeTerm)
    {
        if (nodeTerm == null)
        {
            throw new ArgumentNullException(nameof(nodeTerm));
        }

        if (nodeTerm.IsType(out NodeTermIntlit termIntlit))
        {
            if (DoPushStack)
            {
                MOV(ReturnRegister, termIntlit.value);
                PUSH(ReturnRegister);
            }
            return termIntlit.value;
        }
        else if (nodeTerm.IsType(out NodeTermIdent termIdent))
        {
            if (GetDefine(termIdent.ident, out DefineEntry defineEntry))
            {
                return genExpr(defineEntry.value);
            }
            else if (GetVariabel(termIdent.ident, out Variabel variabel))
            {
                if (DoPushStack)
                {
                    LDA(AddressRegiser, variabel.Address.GetAddress(this));
                    MOV(ReturnRegister, AddressRegiser, true);
                    PUSH(ReturnRegister);
                }
                return variabel.Address.GetAddress(this);
            }
        }
        else if (nodeTerm.IsType(out NodeTermDereference nodeTermDereference))
        {
            if (GetVariabel(nodeTermDereference.ident.ident, out Variabel variabel))
            {
                if (DoPushStack)
                {
                    LDA(ReturnRegister, variabel.Address.GetAddress(this));
                    PUSH(ReturnRegister);
                }
                return variabel.Address.GetAddress(this);
            }
        }

        return null;
    }
    string genExpr(NodeExpr nodeExpr)
    {
        if (nodeExpr.IsType(out NodeTerm nodeTerm))
        {
            return genTerm(nodeTerm);
        }
        else if (nodeExpr.IsType(out NodeExprCompersion nodeCompersion))
        {
            genExpr(nodeCompersion.lhs);
            genExpr(nodeCompersion.rhs);
            POP(SecondRegister);
            POP(ReturnRegister);
            CMP(ReturnRegister, SecondRegister);
            otherResults = nodeCompersion.compersionOperators;
            return "";
        }
        return null;
    }

    bool GetVariabel(string name, out Variabel variabel)
    {
        for (int i = 0; i < variabels.Count; i++)
        {
            if (variabels[i].Name == name)
            {
                variabel = variabels[i];
                return true;
            }
        }
        variabel = new Variabel();
        return false;
    }
    bool GetDefine(string name, out DefineEntry defineEntry)
    {
        for (int i = 0; i < defines.Count; i++)
        {
            if (defines[i].name == name)
            {
                defineEntry = defines[i];
                return true;
            }
        }
        defineEntry = new DefineEntry();
        return false;
    }

    void genDeclareFunction(NodeStmtDeclareFunction nodeDeclareFunction)
    {
        if (nodeDeclareFunction == null)
        {
            throw new Exception("");
        }

        Text();

        Function function = new Function()
        {
            Name = nodeDeclareFunction.Name,
            Arguments = null,
            ArgumentSize = 0
        };
        functions.Add(function);

        FuncStart();
        Enter();
        Pushr();
        genScope(nodeDeclareFunction.Scope);
        FuncEnd();
    }
    void genDeclareVariabel(NodeStmtDeclareVariabel nodeDeclareVariabel)
    {
        if (nodeDeclareVariabel == null)
        {
            throw new Exception("");
        }

        Text();
        Address address = new Address();
        bool InScope = scopes.Count > 0;
        if (InScope)
        {
            address = AddressHelper.GetStackAddress(StackSize, ScopeRegister.ToString());
        }
        else
        {
        }

        genExpr(nodeDeclareVariabel.expr);

        Variabel variabel = new Variabel
        {
            Name = nodeDeclareVariabel.Name,
            TypeData = nodeDeclareVariabel.type,
            Address = address,
            IsInScope = InScope,
            IsGlobal = !InScope,
        };
        variabels.Add(variabel);
    }
    void genReassignVariabel(NodeStmtReassignVariabel nodeReassignVariabel)
    {
        if (nodeReassignVariabel == null)
        {
            throw new Exception();
        }

        Text();
        if (GetVariabel(nodeReassignVariabel.Name, out Variabel variabel))
        {
            genExpr(nodeReassignVariabel.expr);
            POP(ReturnRegister);

            switch (nodeReassignVariabel.assignmentOperator)
            {
                case AssignmentOperators.assign:
                    MOV(variabel.Address.GetAddress(this), ReturnRegister);
                    break;
                case AssignmentOperators.addassign:
                    MOV(SecondRegister, variabel.Address.GetAddress(this));
                    ADD(ReturnRegister, SecondRegister);
                    MOV(variabel.Address.GetAddress(this), ReturnRegister);
                    break;
            }
        }
    }
    void genWhileLoop(NodeStmtWhile nodeWhile)
    {
        if (nodeWhile == null)
        {
            throw new Exception();
        }

        Comment("Enter while loop");
        BreakLabels.Push($"{ScopeLables.Peek()}_END");
        string label = CreateLabel();
        AddLine(label + ":");
        genExpr(nodeWhile.Expr);
        ConJUMP((CompersionOperators)otherResults, label);

        genScope(nodeWhile.Scope);
        JMP(label);
        leaveScopeWithLabel();
        BreakLabels.Pop();
        Comment("Leave while loop");
    }
    void genIfPredElseIf(NodeIfPredElif nodeIfPredElseIf, string endLabel)
    {
        if (nodeIfPredElseIf == null)
        {
            throw new Exception();
        }

        Comment("Enter else if");
        string label = CreateLabel();
        genExpr(nodeIfPredElseIf.Expr);
        ConJUMP((CompersionOperators)otherResults, label);

        genScope(nodeIfPredElseIf.Scope);
        leaveScopeWithLabel();
        JMP(endLabel);
        Comment("Leave else if");

        if (nodeIfPredElseIf.pred != null)
        {
            AddLine(label + ":");
            if (nodeIfPredElseIf.IsType(out NodeIfPredElif nodeIfPredElif))
            {
                genIfPredElseIf(nodeIfPredElif, endLabel);
            }
            else if (nodeIfPredElseIf.IsType(out NodeIfPredElse nodeIfPredElse))
            {
                genIfPredElse(nodeIfPredElse);
            }
        }
        else
        {

        }

    }
    void genIfPredElse(NodeIfPredElse nodeIfPredElse)
    {
        if (nodeIfPredElse == null)
        {
            throw new Exception();
        }

        Comment("Enter else");
        genScope(nodeIfPredElse.Scope);
        leaveScopeWithLabel();
        Comment("Leave else");
    }
    void genIf(NodeStmtIf nodeIf)
    {
        if (nodeIf == null)
        {
            throw new Exception();
        }

        Comment("Enter if");
        string label = CreateLabel();
        genExpr(nodeIf.Expr);
        ConJUMP((CompersionOperators)otherResults, label);
        genScope(nodeIf.Scope);
        leaveScopeWithLabel();

        if (nodeIf.pred != null)
        {
            string endLabel = CreateLabel();
            JMP(endLabel);
            AddLine(label + ":");
            Comment("Leave if");
            if (nodeIf.IsType(out NodeIfPredElif nodeIfPredElif))
            {
                genIfPredElseIf(nodeIfPredElif, endLabel);
            }
            else if (nodeIf.IsType(out NodeIfPredElse nodeIfPredElse))
            {
                genIfPredElse(nodeIfPredElse);
            }
            AddLine(endLabel + ":");
        }
        else
        {
            AddLine(label + ":");
            Comment("Leave if");
        }
    }
    void genAsm(NodeStmtAsm nodeAsm)
    {
        Text();
        string line = nodeAsm.asmCode;

        line = Regex.Replace(line, @"\s+", "\t");
        if (nodeAsm.asmCode.Length > 0)
        {
            DoPushStack = false;
            for (var i = 0; i < nodeAsm.arguments.Length; i++)
            {
                string index = $"{{{i}}}";
                string value = genExpr(nodeAsm.arguments[i].value);
                line = line.Replace(index, value);
            }
            DoPushStack = true;
        }
        AddLine("\t" + line);
    }
    void genDefineIf(NodeStmtDefineIf nodeDefineIf)
    {
        if (nodeDefineIf == null)
        {
            throw new Exception();
        }

        if (GetDefine(nodeDefineIf.Define, out DefineEntry defineEntry))
        {
            DoPushStack = false;

            string value = genExpr(nodeDefineIf.expr);
            string DValue = genExpr(defineEntry.value);
            switch (nodeDefineIf.compersionOperators)
            {
                case CompersionOperators.eq:
                    if (value == DValue)
                    {
                        genScopeWithNoStart(nodeDefineIf.scope);
                    }
                    break;
            }

            DoPushStack = true;
        }
    }
    
    
    void genScope(NodeScope scope)
    {
        enterScope();
        NodeStmt[] stmts = scope.stmts;
        for (int i = 0; i < stmts.Length; i++)
        {
            genStmt(stmts[i]);
        }
    }
    void genScopeWithNoStart(NodeScope scope)
    {
        NodeStmt[] stmts = scope.stmts;
        for (int i = 0; i < stmts.Length; i++)
        {
            genStmt(stmts[i]);
        }
    }
    public void genStmt(NodeStmt stmt)
    {
        if (stmt ==null)
        {
            return;
        }
        Comment($"{stmt.lineNumber}");
        if (stmt.IsType(out NodeStmtDeclareFunction nodeDeclareFunction))
        {
            genDeclareFunction(nodeDeclareFunction);
        }
        else if (stmt.IsType(out NodeStmtDeclareVariabel nodeDeclareVariabel))
        {
            genDeclareVariabel(nodeDeclareVariabel);
        }
        else if (stmt.IsType(out NodeStmtReassignVariabel nodeReassignVariabel))
        {
            genReassignVariabel(nodeReassignVariabel);
        }
        else if (stmt.IsType(out NodeStmtWhile nodeWhile))
        {
            genWhileLoop(nodeWhile);
        }
        else if (stmt.IsType(out NodeStmtIf nodeIf))
        {
            genIf(nodeIf);
        }
        else if (stmt.IsType(out NodeStmtBreak nodeBreak))
        {
            if (BreakLabels.Count > 0)
            {
                int saveScopes = scopes.Peek();
                string saveLable = ScopeLables.Pop();
                leaveScope();
                JMP(BreakLabels.Peek());
                ScopeLables.Push(saveLable);
                scopes.Push(saveScopes);
            }
            else
            {
                int saveScopes = scopes.Peek();
                string saveLable = ScopeLables.Pop();
                leaveScope();
                JMP($"{ScopeLables.Peek()}_END");
                ScopeLables.Push(saveLable);
                scopes.Push(saveScopes);
            }
        }
        else if (stmt.IsType(out NodeStmtContinue nodeContinue))
        {
            AddLine($"\tjmp\t[{ScopeLables.Peek()}]");
        }
        else if (stmt.IsType(out NodeStmtAsm nodeAsm))
        {
            genAsm(nodeAsm);
        }
        else if (stmt.IsType(out NodeStmtDeclareDefine nodeDeclareDefine))
        {
            defines.Add(new DefineEntry() {name = nodeDeclareDefine.name, value = nodeDeclareDefine.expr});
        }
        else if (stmt.IsType(out NodeStmtDefineIf nodeDefineIf))
        {
            genDefineIf(nodeDefineIf);
        }
    }

    public void genProgram(ProgNode progNode)
    {
        output = new List<string>();

        Text();
        AddLine($".SETCPU {m_CPUType}");

        stmts = progNode.stmts;

        for (int i = 0; i < stmts.Length; i++)
        {
            genStmt(stmts[i]);
        }
    }
}
