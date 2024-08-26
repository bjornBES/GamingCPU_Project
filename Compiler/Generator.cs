using Compiler.ParserNodes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection.Metadata;
using static CompilerSettings;

public class Generator
{
    public List<string> m_output = new List<string>();
    public List<string> m_output_rdata = new List<string>();
    public List<string> m_output_bss = new List<string>();
    public List<string> m_output_Truebss = new List<string>();
    public List<string> m_output_data = new List<string>();

    public List<string> m_Strings = new List<string>();

    List<Function> m_function = new List<Function>();

    public int m_stackSize = 0;
    public int m_ArgsStackSize = 0;

    Token last_Line;

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
                    AddLine($"mov ABX, 0x{Convert.ToString(value, 16)}");
                    Push("tbyte ABX");
                    return "ABX";
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
                AddLine($"mov HL, 0x{Convert.ToString(value, 16)}");
                if (BinExprIndex > 0)
                {
                    Push("tbyte HL");
                    return nodeTermIntLit.int_lit.value;
                }

                if (!m_DontPushValue && BinExprIndex != 0)
                {
                    Push("tbyte HL");
                }
                else
                {
                    m_DontPushValue = false;
                }
                return "HL";
            }
            return nodeTermIntLit.int_lit.value;
        }
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

        return null;
    }
    void gen_binexpr(NodeBinExpr expr)
    {
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
    void gen_DeclareFunction(NodeStmtDeclareFunction declareFunction)
    {
        string name = declareFunction.ident.value;
        int ArgumentSize = declareFunction.args.Length;
        int argSize = 0;
        for (int i = 0; i < ArgumentSize; i++)
        {
            NodeFunctionArgs nodeFunctionArgs = declareFunction.args[i];

            _Size size = GetSize(nodeFunctionArgs._Type.TypeSize);
            switch (size)
            {
                case _Size._byte:
                case _Size._short:
                argSize += 2;
                    break;
                case _Size._tbyte:
                case _Size._int:
                default:
                argSize += nodeFunctionArgs._Type.TypeSize;
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

        AddLine($"push bp");
        Pushr();
        begin_scope();
    }
    void gen_endFunction(NodeStmtEndFunction nodeStmtEndFunction)
    {
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
    }
    void gen_newVariable(NodeStmtDeclareVariabel declareVariabel)
    {
        string name = declareVariabel.ident.value;
        NodeExpr expr = declareVariabel.expr;
        AssignmentOperators assignmentOperators = declareVariabel._operator;
        _Type _Type = declareVariabel.Type;

        bool IsTopScope = m_scopes.Count == 0;
        if (declareVariabel.IsString)
        {
            NewVariabelRDATA(name, _Type, expr, IsTopScope, declareVariabel);

            return;
        }

        if (assignmentOperators == AssignmentOperators.none && expr == null)
        {
            // BSS
            NewVariabelBss(name, _Type, IsTopScope, declareVariabel);
            return;
        }
        else
        {
            NewVariabelData(name, _Type, expr, IsTopScope, declareVariabel);
        }
    }
    void gen_ReassignVariable(NodeStmtReAssingnVariabel declareVariabel)
    {
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
    }
    void gen_return(NodeStmtReturn nodeStmtReturn)
    {
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
    }
    void gen_pointer(NodeStmtPointer nodeStmtPointer)
    {
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
    }
    void gen_callFunction(NodeStmtCallFunction nodeStmtCallFunction)
    {
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
    }
    /*
    void gen_assembly()
    {
        last_Line = try_consume_error(TokenType._asm);
        try_consume_error(TokenType.open_paren);
        try_consume_error(TokenType.quotation_mark);
        string asmCode = try_consume_error(TokenType.ident).value;
        if (asmCode.Contains("dec") && asmCode.Contains('S'))
        {
            m_segment_register -= 1;
        }
        else if (asmCode.Contains("inc") && asmCode.Contains('S'))
        {
            m_segment_register += 1;
        }
        if (asmCode.Contains("dec") && asmCode.Contains("SI"))
        {
            m_segment_index_register -= 1;
        }
        else if (asmCode.Contains("inc") && asmCode.Contains("SI"))
        {
            m_segment_index_register += 1;
        }
        try_consume_error(TokenType.quotation_mark);
        try_consume_error(TokenType.close_paren);
        AddLine($"{asmCode}");
    }
    */
    #endregion
    void gen_stmt(NodeStmt nodeStmt)
    {
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
        /*
        else if (peek() != null && peek().Type == TokenType.ident && peek(1) != null)
        {
            gen_newVariable();
        }
        else if (peek() != null && peek().Type == TokenType._asm && peek(1) != null && peek(1).Type == TokenType.open_paren)
        {
            gen_assembly();
        }
        else if (peek() != null && peek().Type == TokenType._return)
        {
            last_Line = try_consume_error(TokenType._return);
            object term = parse_term(out _);

            AddLine("popr");
            AddLine($"mov R1, {term}");
            end_scope();
            AddLine($"ret {m_function.Last().m_NumOfArguments}");
        }
        */
    }
    public void gen_prog(ProgNode progNode)
    {
        AddLine(".section TEXT", 0);
        AddLine($".org 0x{Convert.ToString(StartOffset, 16)}", 0);
        if (DoEntry)
        {
            AddLine("_START_PROG:", 0);
            setDataSegmentRegister(0x00EF);
            AddLine("; WIP");
            //setBank(0x0);
            AddLine("call [_main]");
            AddLine("rts");
        }

        for (int i = 0; i < progNode.stmts.Length; i++)
        {
            AddLine($"\n_{Convert.ToString(progNode.stmts[i].Line.line, 16).PadLeft(6, '0')}: ; {progNode.stmts[i].Line.value}", 0);
            gen_stmt(progNode.stmts[i]);
            LastStmt = progNode.stmts[i];
            m_WasInBinExpr = false;
        }

        AddLine("\n.section DATA", 0);
        m_output.AddRange(m_output_data);

        AddLine("\n.section RDATA", 0);
        m_output.AddRange(m_output_rdata);

        AddLine("\n.section BSS", 0);
        m_output.AddRange(m_output_Truebss);

        AddLine("\n.org 0x0", 0);
        m_output.AddRange(m_output_bss);
    }
    void AddLine(string line, int taps = 1, Section section = Section.text)
    {
        switch (section)
        {
            case Section.bss:
                m_output_bss.Add("".PadLeft(taps, '\t') + line);
                break;
            case Section.Truebss:
                m_output_Truebss.Add("".PadLeft(taps, '\t') + line);
                break;
            case Section.rdata:
                m_output_rdata.Add(line);
                break;
            case Section.data:
                m_output_data.Add(line.Replace(":|:", ":\n\t"));
                break;
            case Section.text:
                m_output.Add("".PadLeft(taps, '\t') + line);
                break;
            default:
                break;
        }
    }
    void Push(string reg16)
    {
        AddLine($"push {reg16}\n");

    }
    void Pushr()
    {
        AddLine($"pushr");
    }
    void Pop(string reg16)
    {
        AddLine($"pop {reg16}");

    }
    void Popr()
    {
        AddLine($"popr");
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

    /*
    #region Bank Functions
    void setBank(byte bank)
    {
        if (bank == m_bank_loc -1)
        {
            AddLine($"dec MB");
        }
        else if (bank == m_bank_loc + 1)
        {
            AddLine($"inc MB");
        }
        else if (bank != m_bank_loc)
        {
            AddLine($"mov MB, 0x{Convert.ToString(bank, 16)}");
        }
        else
        {
            return;
        }
        m_bank_loc = bank;
    }
    void pushBank()
    {
        AddLine($"push MB");
        m_bank_stack.Push(m_bank_loc);
    }
    void popBank()
    {
        AddLine($"pop MB");
        m_bank_loc = m_bank_stack.Pop();
    }
    #endregion
    */
    void begin_scope()
    {
        m_scopes.Push(m_var.Count);
    }
    void end_scope()
    {

        int added = 0;
        for (int i = m_scopes.First(); i < m_var.Count; i++)
        {
            string addr = m_var[i].GetFullAddress();
            AddLine($"; {m_var[i].m_Name} at {addr}");
            AddLine($"mov {GetSizeStr(GetSize(m_var[i].m_Size))} [{addr}], 0");
            added += m_var[i].m_Size;
            m_VariabelCount -= m_var[i].m_Size;
        }

        m_ArgsStackSize = 0;
        m_var.RemoveRange(m_scopes.First(), m_var.Count - m_scopes.First());
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

    void NewVariabelBss(string name, _Type type, bool IsTopScope, NodeStmtDeclareVariabel DeclareVariabel)
    {
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
    }
    void NewVariabelData(string name, _Type type, NodeExpr expr, bool IsTopScope, NodeStmtDeclareVariabel DeclareVariabel)
    {
        string reference = "";

        if (expr.var.Get(out NodeTerm __out) != null)
        {
            if (__out.var.Get(out NodeTermReference termReference) != null)
            {
                reference = termReference.reference.value;
            }
        }

        m_DontPushValue = true;
        bool DontPushValue = true;

        m_DoAlign = false;

        object term = gen_expr(expr);

        AddLine($"; _{name} with {type.TypeSize} bytes", section: Section.text);
        _Size size = GetSize(type.TypeSize);


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
    }
    private void NewVariabelRDATA(string name, _Type type, NodeExpr expr, bool isTopScope, NodeStmtDeclareVariabel DeclareVariabel)
    {
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
    }
    private void NewVariabelLocal(string name, _Type type)
    {
        AddLine($"; _{name} with {type.TypeSize} bytes", section: Section.text);

        m_var.Add(new Var($"_{name}", type, m_ArgsStackSize, false));
        m_ArgsStackSize += type.TypeSize;
    }
}
public struct Var
{
    public string m_Name;

    public int m_MemoryLoc;
    public int m_DSValue;
    public string m_label;

    public bool m_IsGlobal;

    public int m_MemoryBank;

    public int m_Size;
    public bool m_IsSigned;
    public bool m_IsString;

    public bool m_IsPublic;
    public bool m_IsConst;

    public string m_Reference;

    public int GetMemoryLoc()
    {
        return ((m_DSValue << 16) | m_MemoryLoc);
    }
    public string GetHexMemoryLoc()
    {
        string MemoryLoc = Convert.ToString(m_MemoryLoc, 16);
        return $"0x{MemoryLoc}";
    }
    public string GetFullAddress()
    {
        if (m_label != "\0")
        {
            return m_label;
        }
        else
        {
            if (m_DSValue != -1)
            {
                return $"DS:{GetHexMemoryLoc()}";
            }
            else
            {
                return $"{GetHexMemoryLoc()}";
            }
        }
    }
    public string GetFullAddressRaw()
    {
        if (m_label != "\0")
        {
            return m_label;
        }
        else
        {
            if (m_DSValue != -1)
            {
                return $"0x{Convert.ToString(GetMemoryLoc(), 16)}";
            }
            else
            {
                return $"{GetHexMemoryLoc()}";
            }
        }
    }

    public Var(string name, _Type type, ushort memory_loc, ushort DSvalue, bool IsTopScope, NodeStmtDeclareVariabel declareVariabel)
    {
        m_Name = name;
        m_Size = type.TypeSize;
        m_IsSigned = type.IsSigned;
        m_MemoryLoc = memory_loc;
        m_DSValue = DSvalue;
        m_IsGlobal = IsTopScope;
        m_label = "\0";
        m_Reference = "\0";
        m_IsPublic = declareVariabel.IsPublic;
        m_IsConst = declareVariabel.IsConst;
    }
    public Var(string name, _Type type, string label, bool IsTopScope, NodeStmtDeclareVariabel declareVariabel)
    {
        m_Name = name;
        m_Size = type.TypeSize;
        m_IsSigned = type.IsSigned;
        m_label = label;
        m_IsGlobal = IsTopScope;
        m_MemoryLoc = -1;
        m_DSValue = -1;
        m_Reference = "\0";
        m_IsPublic = declareVariabel.IsPublic;
        m_IsConst = declareVariabel.IsConst;
    }

    public Var(string name, _Type type, ushort memory_loc, ushort DSvalue, bool IsTopScope)
    {
        m_Name = name;
        m_Size = type.TypeSize;
        m_IsSigned = type.IsSigned;
        m_MemoryLoc = memory_loc;
        m_DSValue = DSvalue;
        m_IsGlobal = IsTopScope;
        m_label = "\0";
        m_Reference = "\0";
    }
    public Var(string name, _Type type, int ArgStackSize, bool IsTopScope)
    {
        m_Name = name;
        m_Size = type.TypeSize;
        m_IsSigned = type.IsSigned;
        m_label = $"(BP + {ArgStackSize})";
        m_IsGlobal = IsTopScope;
        m_MemoryLoc = -1;
        m_DSValue = -1;
        m_Reference = "\0";
    }
    public Var(string name, _Type type, string label, bool IsTopScope)
    {
        m_Name = name;
        m_Size = type.TypeSize;
        m_IsSigned = type.IsSigned;
        m_label = label;
        m_IsGlobal = IsTopScope;
        m_MemoryLoc = -1;
        m_DSValue = -1;
        m_Reference = "\0";
    }

    public Var(string name, _Type type, ushort memory_loc, ushort DSvalue, bool IsTopScope, NodeStmtDeclareVariabel declareVariabel, string reference) : this(name, type, memory_loc, DSvalue, IsTopScope, declareVariabel)
    {
        m_Reference = reference;
        m_Name = name;
        m_Size = type.TypeSize;
        m_IsSigned = type.IsSigned;
        m_MemoryLoc = memory_loc;
        m_DSValue = DSvalue;
        m_IsGlobal = IsTopScope;
        m_label = "\0";
        m_IsPublic = declareVariabel.IsPublic;
        m_IsConst = declareVariabel.IsConst;
    }
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
    Truebss,
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
