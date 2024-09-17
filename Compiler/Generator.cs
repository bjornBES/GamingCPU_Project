using Compiler.ParserNodes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using static CompilerSettings;
using static HexLibrary.HexConverter;

public class Generator
{
    public List<string> m_output = new List<string>();
    public List<string> m_output_rdata = new List<string>();
    public List<string> m_output_bss = new List<string>();
    public List<string> m_output_data = new List<string>();

    public List<string> m_Strings = new List<string>();

    List<Function> m_function = new List<Function>();

    public int m_stackSize = 0;

    List<Var> m_var = new List<Var>();

    Stack<int> m_scopes = new Stack<int>();

    const int RODataVariabelStartAddress = 0x000000;
    const int DataVariabelStartAddress = 0x000000;
    const int VariabelStartAddress = 0xEF8000;

    int m_RDataVariabelCount = 0;
    int m_DataVariabelCount = 0;
    int m_VariabelCount = VariabelStartAddress;

    int BinExprIndex = 0;
    bool m_DoAlign = false;
    bool m_WasInBinExpr = false;
    bool m_DontPushValue = false;

    NodeStmt LastStmt;

    private int m_SegmentRegister = 0;
    private int m_DataSegmentRegister = 0;

    object gen_expr(NodeExpr expr)
    {
        if (expr == null)
            return null;
        if (expr.var.Get<NodeTerm>() != null)
        {
            NodeTerm term = (NodeTerm)expr.var.Get<NodeTerm>();
            return gen_term(term);
        }
        /*
        else if (expr.var.Get<NodeBinExpr>() != null)
        {
            BinExprIndex++;
            NodeBinExpr nodeBinExpr = (NodeBinExpr)expr.var.Get<NodeBinExpr>();
            gen_binexpr(nodeBinExpr);
            m_WasInBinExpr = true;
            BinExprIndex--;
            return "ABX";
        }

        return null;
        */
        return null;
    }
    string gen_term(NodeTerm term)
    {
        if (term == null)
            return null;

        if (term.var.Get<NodeTermIntLit>() != null)
        {
            NodeTermIntLit nodeTermIntLit = (NodeTermIntLit)term.var.Get<NodeTermIntLit>();
            int value = Convert.ToInt32(nodeTermIntLit.int_lit.value);
            if (value >= 0 && value <= 0xFFFF)
            {
                if (BinExprIndex > 0)
                {
                    AddLine($"mov A, 0x{ToHexString(value)}");
                    Push("A");
                    return "A";
                }
                else if (BinExprIndex == 0)
                {
                    return $"0x{ToHexString(value)}";
                }

                AddLine($"mov AX, 0x{Convert.ToString(value, 16)}");
                if (!m_DontPushValue && BinExprIndex != 0)
                {
                    Push("AX");
                }
                else
                {
                    m_DontPushValue = false;
                }
            }
            else
            {
                if (BinExprIndex > 0)
                {
                    AddLine($"mov HL, 0x{ToHexString(value)}");
                    Push("HL");
                    return "HL";
                }
                else if (BinExprIndex == 0)
                {
                    return $"0x{ToHexString(value)}";
                }

                AddLine($"mov HL, 0x{Convert.ToString(value, 16)}");
                if (!m_DontPushValue && BinExprIndex != 0)
                {
                    Push("HL");
                }
                else
                {
                    m_DontPushValue = false;
                }
            }
            return nodeTermIntLit.int_lit.value;
        }
        /*
        else if (term.var.Get<NodeTermIdent>() != null)
        {
            NodeTermIdent nodeTermIdent = (NodeTermIdent)term.var.Get<NodeTermIdent>();

            if (GetVar($"_{nodeTermIdent.ident.value}", out Var var))
            {
                if (var.m_MemoryLoc == -1)
                {
                    AddLine($"mov AX, [{var.m_label}]");
                    Push("AX");
                    return $"[{var.m_label}]";
                }
                else
                {
                    _Size size = GetSize(var.m_Size);
                    switch (size)
                    {
                        case _Size._byte:
                            if (m_DoAlign)
                            {
                                AddLine("sez AX");
                                AddLine($"mov byte AL, [{getSetDataSegmentRegister(var.m_DSValue)}:{var.GetHexMemoryLoc()}]");
                                return "AX";
                            }
                            else
                            {
                                AddLine($"mov byte AL, [{getSetDataSegmentRegister(var.m_DSValue)}:{var.GetHexMemoryLoc()}]");
                                return "AL";
                            }
                        case _Size._short:
                            AddLine($"mov word AX, [{getSetDataSegmentRegister(var.m_DSValue)}:{var.GetHexMemoryLoc()}]");
                            return "AX";
                        case _Size._tbyte:
                            AddLine($"mov tbyte ABX, [{getSetDataSegmentRegister(var.m_DSValue)}:{var.GetHexMemoryLoc()}]");
                            return "ABX";
                        case _Size._int:
                            AddLine($"mov dword ABX, [{getSetDataSegmentRegister(var.m_DSValue)}:{var.GetHexMemoryLoc()}]");
                            return "ABX";
                        case (_Size)5:
                            AddLine($"mov tbyte ABX, [{getSetDataSegmentRegister(var.m_DSValue)}:{var.GetHexMemoryLoc()}]");
                            return "ABX";
                    }
                }
            }
            return ((NodeTermIdent)term.var.Get<NodeTermIdent>()).ident.value;
        }
        else if (term.var.Get<NodeTermString>() != null)
        {
            return ((NodeTermString)term.var.Get<NodeTermString>()).str.value;
        }
        else if (term.var.Get<NodeTermReference>() != null)
        {
            NodeTermReference reference = (NodeTermReference)term.var.Get<NodeTermReference>();
            if (GetVar($"_{reference.reference.value}", out Var var))
            {
                if (var.m_MemoryLoc == -1)
                {
                    return $"[{var.m_label}]";
                }
                else
                {
                    AddLine($"mov tbyte ABX, {var.GetFullAddressRaw()}");
                    return "ABX";
                }
            }
            return $"@_{reference.reference.value}";
        }
        else if (term.var.Get<NodeTermParen>() != null)
        {
            NodeTermParen nodeTermParen = (NodeTermParen)term.var.Get<NodeTermParen>();
            gen_expr(nodeTermParen.Expr);
            return "";
        }
        else if (term.var.Get<NodeTermCallFunction>() != null)
        {
            NodeTermCallFunction nodeTermCallFunction = (NodeTermCallFunction)term.var.Get<NodeTermCallFunction>();

            string funcName = $"_{nodeTermCallFunction.FunctionName.value}";
            if (!GetFunction($"{nodeTermCallFunction.FunctionName.value}", out Function result))
            {
                throw new Exception("NOPE");
            }

            if (result.m_NumOfArguments != nodeTermCallFunction.args.Length)
            {
                throw new Exception("NOPE");
            }

            for (int i = nodeTermCallFunction.args.Length - 1; i > 0; i--)
            {
                object termValue = gen_expr(nodeTermCallFunction.args[i]);

                string termHex;

                if (int.TryParse(termValue.ToString(), out _) || string.IsNullOrWhiteSpace(termValue.ToString()))
                {
                    Pop("AX");
                    termHex = "AX";
                }
                else
                {
                    termHex = termValue.ToString();
                }

                AddLine($"pusha {termHex}");
            }
            AddLine($"call [{funcName}]");
            AddLine($"mov AX, R1");
            //Push("AX");
            return "AX";
        }
        else if (term.var.Get(out NodeTermPointer nodeTermPointer) != null)
        {
            Var? var = GetVar($"_{nodeTermPointer.ident.value}");
            if (var.HasValue)
            {
                if (var.Value.m_MemoryLoc == -1)
                {
                    return $"[{var.Value.m_label}]";
                }
                else
                {
                    _Size size = GetSize(var.Value.m_Size);
                    switch (size)
                    {
                        case _Size._byte:
                            AddLine($"mov byte AL, [{getSetDataSegmentRegister(var.Value.m_DSValue)}:{var.Value.GetHexMemoryLoc()}]");
                            if (BinExprIndex > 0)
                            {
                                Push("tbyte AL");
                            }
                            return "AL";
                        case _Size._short:
                            AddLine($"mov word AX, [{getSetDataSegmentRegister(var.Value.m_DSValue)}:{var.Value.GetHexMemoryLoc()}]");
                            if (BinExprIndex > 0)
                            {
                                Push("tbyte AX");
                            }
                            return "AX";
                        case _Size._tbyte:
                            AddLine($"mov tbyte HL, [{getSetDataSegmentRegister(var.Value.m_DSValue)}:{var.Value.GetHexMemoryLoc()}]");
                            if (BinExprIndex > 0)
                            {
                                Push("HL");
                            }
                            return "HL";
                        case _Size._int:
                            AddLine($"mov dword HL, [{getSetDataSegmentRegister(var.Value.m_DSValue)}:{var.Value.GetHexMemoryLoc()}]");
                            return "HL";
                    }
                    return "NULL";
                }
            }
            if (int.TryParse(nodeTermPointer.ident.value, out int result))
            {
                return $"0x{Convert.ToString(result, 16)}";
            }
            else
            {
                return $"@_{nodeTermPointer.ident.value}";
            }
        }
        */

        return null;
    }
    void gen_binexpr(NodeBinExpr expr)
    {
        /*
        if (expr.var.Get<NodeBinExprAdd>() != null)
        {
            NodeBinExprAdd exprAdd = (NodeBinExprAdd)expr.var.Get<NodeBinExprAdd>();
            gen_expr(exprAdd.rhs);
            gen_expr(exprAdd.lhs);
        }
        else if (expr.var.Get<NodeBinExprSub>() != null)
        {
            NodeBinExprSub exprSub = (NodeBinExprSub)expr.var.Get<NodeBinExprSub>();
            gen_expr(exprSub.rhs);
            gen_expr(exprSub.lhs);
        }
        else if (expr.var.Get<NodeBinExprMulti>() != null)
        {
            NodeBinExprMulti exprMulti = (NodeBinExprMulti)expr.var.Get<NodeBinExprMulti>();
            gen_expr(exprMulti.rhs);
            gen_expr(exprMulti.lhs);
        }
        else if (expr.var.Get<NodeBinExprDiv>() != null)
        {
            NodeBinExprDiv exprDiv = (NodeBinExprDiv)expr.var.Get<NodeBinExprDiv>();
            gen_expr(exprDiv.rhs);
            gen_expr(exprDiv.lhs);
        }

        Pop("ABX");
        Pop("HL");

        if (expr.var.Get<NodeBinExprAdd>() != null)
        {
            AddLine("add ABX, HL");

        }
        else if (expr.var.Get<NodeBinExprSub>() != null)
        {
            AddLine("sub ABX, HL");
        }
        else if (expr.var.Get<NodeBinExprMulti>() != null)
        {
            AddLine("mul ABX, HL");
        }
        else if (expr.var.Get<NodeBinExprDiv>() != null)
        {
            AddLine("div ABX, HL");
        }
        Push("ABX");
        */
    }
    _Size GetSize(int size)
    {
        return (_Size)Enum.Parse(typeof(_Size), size.ToString());
    }
    string GetSizeStr(_Size _Size)
    {
        switch (_Size)
        {
            case _Size._byte:
                return "byte";
            case _Size._short:
                return "word";
            case _Size._tbyte:
                return "tbyte";
            case _Size._int:
                return "dword";
            default:
                return "";
        }
    }

    #region Generator Functions
    void gen_Scope(NodeScope scope)
    {
        begin_scope();

        for (int i = 0; i < scope.Stmts.Length; i++)
        {
            gen_stmt(scope.Stmts[i]);
        }
        AddLine($"", 0);
        end_scope();
    }
    void gen_DeclareFunction(NodeStmtDeclareFunction declareFunction)
    {
        string name = declareFunction.ident.value;
        int ArgumentSize = declareFunction.args.Length;
        int argSize = 0;
        for (int i = 0; i < ArgumentSize; i++)
        {
            NodeFunctionArgs nodeFunctionArgs = declareFunction.args[i];

            _Size size = GetSize(nodeFunctionArgs._Type.m_TypeSize);
            switch (size)
            {
                case _Size._byte:
                case _Size._short:
                argSize += 2;
                    break;
                case _Size._tbyte:
                case _Size._int:
                default:
                argSize += nodeFunctionArgs._Type.m_TypeSize;
                    break;
            }


            NewVariabelLocal(nodeFunctionArgs.name.value, nodeFunctionArgs._Type);
        }

        m_function.Add(new Function() { m_Name = name, m_NumOfArguments = ArgumentSize, m_size = argSize });

        if (declareFunction.IsPublic)
        {
            AddLine($".global _{name}:", 0);
        }
        else
        {
            AddLine($"_{name}:", 0);
        }

        Push("BP");
        AddLine($"mov BP, SP");
        Pushr();
        gen_Scope(declareFunction.Scope);
        Popr();
        Pop("BP");
        if (ArgumentSize == 0)
        {
            AddLine($"retz");
        }
        else
        {
            AddLine($"ret {ArgumentSize}");
        }
    }
    void gen_endFunction(NodeStmtEndFunction nodeStmtEndFunction)
    {
        /*
        if (m_function.Last().m_Name == nodeStmtEndFunction.ident.value)
        {
            if (LastStmt.var.Get<NodeStmtReturn>() != null)
            {
                AddLine($"mov R1, 0");
            }
            AddLine($"_END_{m_function.Last().m_Name}:");
            Popr();
            AddLine("pop bp");
            end_scope();
            AddLine($"ret {m_function.Last().m_size}");
        }
        else
        {
            throw new NotImplementedException();
        }
        */
    }
    void gen_newVariable(NodeStmtDeclareVariabel declareVariabel)
    {
        if (declareVariabel.IsConst)
        {
            NewVariabelRDATA(declareVariabel);

            return;
        }

        if (declareVariabel._operator == AssignmentOperators.none && declareVariabel.expr == null)
        {
            // BSS
            NewVariabelBss(declareVariabel);
            return;
        }
        else
        {
            NewVariabelData(declareVariabel);
        }
    }
    void gen_ReassignVariable(NodeStmtReAssingnVariabel declareVariabel)
    {
        /*
        string name = $"_{declareVariabel.ident.value}";
        NodeExpr expr = declareVariabel.expr;
        AssignmentOperators assignmentOperators = declareVariabel._operator;

        if (GetVar(name, out Var var))
        {
            if (var.m_IsConst)
            {
                CompilerErrors.error_VariableIsConst(LastStmt.Line, var);
                return;
            }

            string address;
            object term = gen_expr(expr);

            if (var.m_MemoryLoc == -1)
            {
                address = var.m_label;
            }
            else
            {
                address = $"{getSetDataSegmentRegister(var.m_DSValue)}:{var.GetHexMemoryLoc()}";
            }

            string termHex;

            if (int.TryParse(term.ToString(), out int result) || string.IsNullOrWhiteSpace(term.ToString()))
            {
                Pop("AX");
                termHex = "AX";
            }
            else
            {
                termHex = term.ToString();
            }

            _Size size = GetSize(var.m_Size);
            switch (size)
            {
                case _Size._byte:
                    if (assignmentOperators == AssignmentOperators.basic_assignment)
                    {
                        AddLine($"mov\tbyte [{address}], {termHex}");
                    }
                    break;
                case _Size._short:
                    if (assignmentOperators == AssignmentOperators.basic_assignment)
                    {
                        AddLine($"mov\tword [{address}], {termHex}");
                    }
                    break;
                case _Size._tbyte:
                    if (assignmentOperators == AssignmentOperators.basic_assignment)
                    {
                        AddLine($"mov\ttbyte [{address}], {termHex}");
                    }
                    break;
                case _Size._int:
                    if (assignmentOperators == AssignmentOperators.basic_assignment)
                    {
                        AddLine($"mov\tdword [{address}], {termHex}");
                    }
                    break;
                default:
                    break;
            }
        }
        */
    }
    void gen_return(NodeStmtReturn nodeStmtReturn)
    {
        /*
        object term = gen_expr(nodeStmtReturn.ReturnExpr);

        string termHex;

        if (int.TryParse(term.ToString(), out int result) || string.IsNullOrWhiteSpace(term.ToString()))
        {
            Pop("AX");
            termHex = "AX";
        }
        else
        {
            termHex = term.ToString();
        }

        AddLine($"mov R1, {termHex}");
        AddLine($"jmp [_END_{m_function.Last().m_Name}]");
        */
    }
    void gen_pointer(NodeStmtPointer nodeStmtPointer)
    {
        /*
        string name = $"_{nodeStmtPointer.name.value}";
        NodeExpr expr = nodeStmtPointer.Expr;
        AssignmentOperators assignmentOperators = nodeStmtPointer._operator;

        if (GetVar(name, out Var var))
        {
            bool DontPushValue = true;
            m_DontPushValue = true;
            object term = gen_expr(expr);

            string termHex;

            if (int.TryParse(term.ToString(), out _) || string.IsNullOrWhiteSpace(term.ToString()))
            {
                if (DontPushValue && m_DontPushValue)
                {
                    if (m_WasInBinExpr)
                    {
                        Pop("ABX");
                        termHex = "ABX";
                    }
                    else
                    {
                        Pop("AX");
                        termHex = "AX";
                    }
                }
                else
                {
                    termHex = "AX";
                }
            }
            else
            {
                termHex = term.ToString();
            }

            object address = gen_expr(nodeStmtPointer.ident);
            if (address == null)
            {
                if (var.m_MemoryLoc == -1)
                {
                    address = var.m_label;
                }
                else
                {
                    address = $"{getSetDataSegmentRegister(var.m_DSValue)}:{var.GetHexMemoryLoc()}";
                }
            }

            _Size size = GetSize(var.m_Size);
            string sizeAll = "";
            if (GetVar($"_{var.m_Reference}", out var))
            {
                sizeAll = GetSizeStr(GetSize(var.m_Size));
            }


            if (assignmentOperators == AssignmentOperators.basic_assignment)
            {
                if (address.ToString() == "HL")
                {
                    //AddLine($"mov {GetSizeStr(size)} HL, {address}");
                    AddLine($"mov {sizeAll} [{address}], {termHex}");
                }
                else if (address == null || string.IsNullOrEmpty(address.ToString()))
                {
                    Pop("tbyte HL");
                    //AddLine($"mov {GetSizeStr(size)} HL, [ABX]");
                    AddLine($"mov {sizeAll} [HL], {termHex}");
                }
                else
                {
                    AddLine($"mov {GetSizeStr(size)} HL, [{address}]");
                    AddLine($"mov {sizeAll} [HL], {termHex}");
                }
            }
        }
        else
        {
            bool DontPushValue = true;
            m_DontPushValue = true;
            object term = gen_expr(expr);

            string termHex;

            if (int.TryParse(term.ToString(), out int result) || string.IsNullOrWhiteSpace(term.ToString()))
            {
                if (DontPushValue && m_DontPushValue)
                {
                    if (m_WasInBinExpr)
                    {
                        Pop("ABX");
                        termHex = "ABX";
                    }
                    else
                    {
                        Pop("AX");
                        termHex = "AX";
                    }
                }
                else
                {
                    termHex = "AX";
                }
            }
            else
            {
                termHex = term.ToString();
            }

            object address = gen_expr(nodeStmtPointer.ident);
            if (assignmentOperators == AssignmentOperators.basic_assignment)
            {
                AddLine($"mov [{address}], {termHex}");
            }
        }
        */
    }
    void gen_callFunction(NodeStmtCallFunction nodeStmtCallFunction)
    {
        /*
        string funcName = $"_{nodeStmtCallFunction.FunctionName.value}";
        if (!GetFunction($"{nodeStmtCallFunction.FunctionName.value}", out Function result))
        {
            throw new Exception("NOPE");
        }

        if (result.m_NumOfArguments != nodeStmtCallFunction.args.Length)
        {
            throw new Exception("NOPE");
        }

        for (int i = nodeStmtCallFunction.args.Length - 1; i > -1; i--)
        {
            bool DontPushValue = true;
            m_DontPushValue = true;
            m_DoAlign = true;
            object termValue = gen_expr(nodeStmtCallFunction.args[i]);
            m_DoAlign = false;

            string termHex;


            if (int.TryParse(termValue.ToString(), out _) || string.IsNullOrWhiteSpace(termValue.ToString()))
            {
                if (DontPushValue && m_DontPushValue)
                {
                    if (m_WasInBinExpr)
                    {
                        Pop("ABX");
                        termHex = "ABX";
                    }
                    else
                    {
                        Pop("AX");
                        termHex = "AX";
                    }
                }
                else
                {
                    termHex = "AX";
                }
            }
            else
            {
                termHex = termValue.ToString();
            }

            AddLine($"pusha {termHex}");
        }
        AddLine($"call [{funcName}]");
        //AddLine($"mov AX, R1");
        */
    }
    #endregion
    void gen_stmt(NodeStmt nodeStmt)
    {
        AddLine($"", 0);
        if (nodeStmt.var.Get(out NodeStmtDeclareFunction nodeStmtDeclareFunction) != null)
        {
            gen_DeclareFunction(nodeStmtDeclareFunction);
        }
        else if (nodeStmt.var.Get(out NodeStmtReturn nodeStmtReturn) != null)
        {
            gen_return(nodeStmtReturn);
        }
        else if (nodeStmt.var.Get(out NodeStmtEndFunction nodeStmtEndFunction) != null)
        {
            gen_endFunction(nodeStmtEndFunction);
        }
        else if (nodeStmt.var.Get(out NodeStmtDeclareVariabel nodeStmtDeclareVariabel) != null)
        {
            gen_newVariable(nodeStmtDeclareVariabel);
        }
        else if (nodeStmt.var.Get(out NodeStmtReAssingnVariabel nodeStmtReAssingnVariabel) != null)
        {
            gen_ReassignVariable(nodeStmtReAssingnVariabel);
        }
        else if (nodeStmt.var.Get(out NodeStmtPointer stmtPointer) != null)
        {
            gen_pointer(stmtPointer);
        }
        else if (nodeStmt.var.Get(out NodeStmtCallFunction stmtCallFunction) != null)
        {
            gen_callFunction(stmtCallFunction);
        }
    }
    public void gen_prog(ProgNode progNode)
    {
        AddLine(".section TEXT", 0);
        if (DoEntry)
        {
            AddLine("_START_PROG:", 0);
            setDataSegmentRegister(0x0008);
            AddLine("; WIP");
            AddLine("mov SS, 0x0001");
            AddLine("sez SP");
            AddLine("sez BP");
            //setBank(0x0);
            AddLine("call [_main]");
            AddLine("rts");
        }

        for (int i = 0; i < progNode.stmts.Length; i++)
        {
            AddLine($"{Environment.NewLine}_{Convert.ToString(progNode.stmts[i].Line.line, 16).PadLeft(6, '0')}: ; {progNode.stmts[i].Line.value}", 0);
            gen_stmt(progNode.stmts[i]);
            LastStmt = progNode.stmts[i];
            m_WasInBinExpr = false;
        }

        AddLine($"{Environment.NewLine}.section DATA", 0);
        m_output.AddRange(m_output_data);

        AddLine($"{Environment.NewLine}.section RDATA", 0);
        m_output.AddRange(m_output_rdata);

        AddLine($"{Environment.NewLine}.section BSS", 0);
        m_output.AddRange(m_output_bss);
    }
    void AddLine(string line, int taps = 1, Section section = Section.text)
    {
        switch (section)
        {
            case Section.bss:
                m_output_bss.Add("".PadLeft(taps, '\t') + line);
                break;
            case Section.rdata:
                m_output_rdata.Add(line);
                break;
            case Section.data:
                m_output_data.Add(line.Replace(":|:", $":{Environment.NewLine}\t"));
                break;
            case Section.text:
                m_output.Add("".PadLeft(taps, '\t') + line);
                break;
            default:
                break;
        }
    }
    void CheckName(string name)
    {
        m_function.ForEach(func =>
        {
            if (func.m_Name == name)
            {
                // TODO Error
                Console.WriteLine("Error:Same name Error");
                Environment.Exit(1);
            }
        });

        m_var.ForEach(var =>
        {
            if (var.m_Name == name)
            {
                Console.WriteLine("Error:Same name Error");
                // TODO Error
                Environment.Exit(1);
            }
        });
    }
    void Push(string reg16)
    {
        AddLine($"push {reg16}");
        m_stackSize++;
    }
    void Pushr()
    {
        AddLine($"pushr");
        m_stackSize += 6;
    }
    void Pop(string reg16)
    {
        AddLine($"pop {reg16}");
        m_stackSize--;

    }
    void Popr()
    {
        AddLine($"popr");
        m_stackSize -= 6;
    }

    string getSetSegmentRegister(int value)
    {
        if (value == m_SegmentRegister)
            return $"S";
        if (value == m_DataSegmentRegister)
            return $"DS";
        else
        {
            setDataSegmentRegister(value);
            return "DS";
        }
    }
    string getSetDataSegmentRegister(int value)
    {
        if (value == m_DataSegmentRegister)
        {
            return $"DS";
        }
        else
        {
            setDataSegmentRegister(value);
            return "DS";
        }
    }
    void setSegmentRegister(int value)
    {
        if (m_SegmentRegister == value)
            return;
        if (m_SegmentRegister == value - 1)
        {
            AddLine($"inc S");
        }
        else if (m_SegmentRegister == value + 1)
        {
            AddLine($"dec S");
        }
        else
        {
            AddLine($"mov S, 0x{Convert.ToString(value, 16).PadLeft(4, '0')}");
        }
        m_SegmentRegister = value;
    }
    void setDataSegmentRegister(int value)
    {
        if (m_DataSegmentRegister == value)
        {

            return;
        }
        if (m_DataSegmentRegister == value - 1)
        {
            int addr = (ushort)(m_VariabelCount & 0x0000FFFF);
            m_VariabelCount = (value << 16) | addr;
            AddLine($"inc DS");
        }
        else if (m_DataSegmentRegister == value + 1)
        {
            int addr = (ushort)(m_VariabelCount & 0x0000FFFF);
            m_VariabelCount = (value << 16) | addr;
            AddLine($"dec DS");
        }
        else
        {
            int addr = (ushort)(m_VariabelCount & 0x0000FFFF);
            m_VariabelCount = (value << 16) | addr;
            AddLine($"mov DS, 0x{Convert.ToString(value, 16).PadLeft(4, '0')}");
        }
        m_DataSegmentRegister = value;
    }
    void begin_scope()
    {
        m_scopes.Push(m_var.Count);
    }
    void end_scope()
    {
        int popCount = m_var.Count - m_scopes.Peek();
        
        if (popCount != 0)
        {
            AddLine($"add SP, {popCount * 2}");
        }

        m_stackSize -= popCount;
        
        for (int i = m_scopes.First(); i < m_var.Count; i++)
        {
            m_var.RemoveAt(i);
        }

        m_scopes.Pop();
    }

    Var? GetVar(string name)
    {
        for (int i = 0; i < m_var.Count; i++)
        {
            if (name == m_var[i].m_Name)
            {
                return m_var[i];
            }
        }
        return null;
    }
    bool GetVar(string name, out Var result)
    {
        for (int i = 0; i < m_var.Count; i++)
        {
            if (name == m_var[i].m_Name)
            {
                result = m_var[i];
                return true;
            }
        }
        result = default;
        return false;
    }
    Function? GetFunction(string name)
    {
        for (int i = 0; i < m_function.Count; i++)
        {
            if (name == m_function[i].m_Name)
            {
                return m_function[i];
            }
        }
        return null;
    }
    bool GetFunction(string name, out Function result)
    {
        for (int i = 0; i < m_function.Count; i++)
        {
            if (name == m_function[i].m_Name)
            {
                result = m_function[i];
                return true;
            }
        }
        result = default;
        return false;
    }

    void NewVariabelBss(NodeStmtDeclareVariabel DeclareVariabel)
    {
        bool IsTopScope = m_scopes.Count == 0;
        /*
        _Size size = GetSize(type.TypeSize);

        switch (size)
        {
            case _Size._byte:
            case _Size._short:
            case _Size._tbyte:
            case _Size._int:
                if (IsTopScope)
                {
                    AddLine($"; _{name} with {type.TypeSize} bytes", 0, section: Section.bss);
                    AddLine($"_{name}:\t\t.res{GetSizeStr(size)}", 0, section: Section.bss);
                }
                else
                {
                    string DSReg = getSetDataSegmentRegister((ushort)((m_VariabelCount & 0xFFFF0000) >> 16));

                    string addr = $"{DSReg}:0x{Convert.ToString(m_VariabelCount & 0x0000FFFF, 16)}";

                    m_var.Add(new Var($"_{name}", type, (ushort)(m_VariabelCount & 0x0000FFFF), (ushort)((m_VariabelCount & 0xFFFF0000) >> 16), IsTopScope, DeclareVariabel));
                    m_VariabelCount += type.TypeSize;

                    AddLine($"; _{name} with {type.TypeSize} bytes at {addr}");
                    return;
                }
                break;
            default:
                if (type.TypeSize > 255)
                {
                    if (IsTopScope)
                    {
                        AddLine($"; _{name} with {type.TypeSize} bytes", 0, section: Section.Truebss);
                        AddLine($"_{name}:\t\t.resbyte {type.TypeSize}", 0, section: Section.Truebss);
                    }
                    else
                    {
                        string DSReg = getSetDataSegmentRegister((ushort)((m_VariabelCount & 0xFFFF0000) >> 16));

                        string addr = $"{DSReg}:0x{Convert.ToString(m_VariabelCount & 0x0000FFFF, 16)}";

                        m_var.Add(new Var($"_{name}", type, (ushort)(m_VariabelCount & 0x0000FFFF), (ushort)((m_VariabelCount & 0xFFFF0000) >> 16), IsTopScope, DeclareVariabel));
                        m_VariabelCount += type.TypeSize;

                        AddLine($"; _{name} with {type.TypeSize} bytes at {addr}");
                        return;
                    }
                }
                else
                {
                    if (IsTopScope)
                    {
                        AddLine($"; _{name} with {type.TypeSize} bytes", 0, section: Section.bss);
                        AddLine($"_{name}:\t\t.resbyte {type.TypeSize}", 0, section: Section.bss);
                    }
                    else
                    {
                        string DSReg = getSetDataSegmentRegister((ushort)((m_VariabelCount & 0xFFFF0000) >> 16));

                        string addr = $"{DSReg}:0x{Convert.ToString(m_VariabelCount & 0x0000FFFF, 16)}";

                        m_var.Add(new Var($"_{name}", type, (ushort)(m_VariabelCount & 0x0000FFFF), (ushort)((m_VariabelCount & 0xFFFF0000) >> 16), IsTopScope, DeclareVariabel));
                        m_VariabelCount += type.TypeSize;

                        AddLine($"; _{name} with {type.TypeSize} bytes at {addr}");
                        return;
                    }
                }
                break;
        }
        AddLine($"", 0, section: Section.bss);

        m_var.Add(new Var($"_{name}", type, $"_{name}", IsTopScope, DeclareVariabel));
        */
    }
    void NewVariabelData(NodeStmtDeclareVariabel DeclareVariabel)
    {
        string name = "_" + DeclareVariabel.ident.value;

        CheckName(name);

        bool IsTopScope = m_scopes.Count == 0;

        NodeExpr nodeExpr = DeclareVariabel.expr;
        
        object pre_term = gen_expr(nodeExpr);
        string registerTerm = "A";
        _Type type = DeclareVariabel.Type;
        _Size size = GetSize(type.m_TypeSize);

        AddLine($"; {name} with {type.m_TypeSize} bytes", section: Section.text);

        Address address = new Address();

        if (IsTopScope == false)
        {
            address.m_IsInStack = true;
            address.m_StackLoc = m_stackSize;

            AddLine($"mov {registerTerm}, {pre_term}");
            AddLine($"; SP = {m_stackSize * 2}");
            if (registerTerm == "HL")
            {
                Push("H");
                Push("L");
            }
            else
            {
                Push(registerTerm);
            }
            AddLine($"; SP = {m_stackSize * 2}");

        }
        m_var.Add(new Var(name, type, address));


        /*
        string termHex;

        if (long.TryParse(term.ToString(), out long result) || string.IsNullOrWhiteSpace(term.ToString()))
        {

            switch (size)
            {
                case _Size._byte:
                    if(result > 0xFF)
                    {
                        CompilerErrors.error_expected(LastStmt.Line, "");
                    }
                    break;
                case _Size._short:
                    if (result > 0xFFFF)
                    {
                        CompilerErrors.error_expected(LastStmt.Line, "");
                    }
                    break;
                case _Size._tbyte:
                    if (result > 0xFFFFFF)
                    {
                        CompilerErrors.error_expected(LastStmt.Line, "");
                    }
                    break;
                case _Size._int:
                    if (result > 0xFFFFFFFF)
                    {
                        CompilerErrors.error_expected(LastStmt.Line, "");
                    }
                    break;
                default:
                    break;
            }

            if (DontPushValue && m_DontPushValue)
            {
                if (m_WasInBinExpr)
                {
                    Pop("ABX");
                    termHex = "ABX";
                }
                else
                {
                    Pop("AX");
                    termHex = "AX";
                }
            }
            else
            {
                termHex = "AX";
            }
        }
        else
        {
            termHex = term.ToString();
        }

        string DSReg = getSetDataSegmentRegister((ushort)((m_VariabelCount & 0xFFFF0000) >> 16));

        string addr = $"{DSReg}:0x{Convert.ToString(m_VariabelCount & 0x0000FFFF, 16)}";

        AddLine($"mov\t{GetSizeStr(size)} [{addr}], {termHex}");

        if (reference == "")
        {
            m_var.Add(new Var($"_{name}", type, (ushort)(m_VariabelCount & 0x0000FFFF), (ushort)((m_VariabelCount & 0xFFFF0000) >> 16), IsTopScope, DeclareVariabel));
        }
        else
        {
            m_var.Add(new Var($"_{name}", type, (ushort)(m_VariabelCount & 0x0000FFFF), (ushort)((m_VariabelCount & 0xFFFF0000) >> 16), IsTopScope, DeclareVariabel, reference));
        }

        m_VariabelCount += type.TypeSize;
        */
    }
    private void NewVariabelRDATA(NodeStmtDeclareVariabel DeclareVariabel)
    {
        bool IsTopScope = m_scopes.Count == 0;
        /*
        object term = gen_expr(expr);
        string address = $"str_{Convert.ToString(m_Strings.Count, 16).PadLeft(4, '0')}";

        if (term == null)
        {
            NewVariabelBss(name, type, isTopScope, DeclareVariabel);
            return;
        }

        if (!m_Strings.Contains(term.ToString()))
        {
            AddLine($"_{address}:", section: Section.rdata);
            AddLine($".db \"{term}\",0", section: Section.rdata);
            m_Strings.Add(term.ToString());
        }
        else
        {
            int i = m_Strings.FindIndex(match: s => s == term.ToString());
            address = $"str_{Convert.ToString(i, 16).PadLeft(4, '0')}";
        }
        //Var var = new Var($"_{name}", type, $"_{address}", isTopScope);
        //var.m_IsString = true;
        //m_var.Add(var);

        NewVariabelData(name, type, new NodeExpr() { var = new NodeTerm() { var = new NodeTermReference() { reference = new Token() { value = address } } } }, isTopScope, DeclareVariabel);
        */
    }
    private void NewVariabelLocal(string name, _Type type)
    {
        /*
        AddLine($"; _{name} with {type.TypeSize} bytes", section: Section.text);

        m_var.Add(new Var($"_{name}", type, m_ArgsStackSize, false));
        m_ArgsStackSize += type.TypeSize;
        */
    }
}
public struct Var
{
    public string m_Name;
    public Address m_Address;
    public _Type m_Type;

    public int GetAddress()
    {
        if (m_Address.m_IsInStack)
        {
            return m_Address.m_StackLoc;
        }

        return 0;
    }

    public Var(string name, _Type _Type, Address address)
    {
        m_Name = name;
        m_Type = _Type;
        m_Address = address;
    }
}
public struct Address
{
    public bool m_IsInStack;
    public int m_StackLoc;
    
    public int m_MemoryBank;
}
struct Function
{
    public string m_Name;
    public int m_NumOfArguments;
    public int m_size;
}
enum Section
{
    bss,
    rdata,
    data,
    text,
    loacl,
}
enum _Size
{
    _byte = 1,
    _short = 2,
    _tbyte = 3,
    _int = 4,
}
