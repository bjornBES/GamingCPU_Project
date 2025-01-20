using static CompilerSettings;
using static CompilerErrors;
using Compiler;
using Microsoft.VisualBasic;
using System.Net.Http.Headers;

namespace Compiler
{
    public class Parser : ParserVariabels
    {
        NodeTerm parseTerm()
        {
            if (!peek().HasValue)
            {
                return null;
            }
            NodeTerm nodeTerm = null;
            if (peek().Value.m_Type == TokenType.int_lit)
            {
                nodeTerm = parseTerm(consume());
            }
            else if (peek().Value.m_Type == TokenType.bitand)
            {
                consume();
                nodeTerm = new NodeTerm();
                string name = try_consume_error(TokenType.ident).m_Value;
                NodeTermIdent termIdent = new NodeTermIdent() { ident = name };
                nodeTerm.term = new NodeTermDereference() { ident = termIdent };
            }
            else if (peek().Value.m_Type == TokenType.ident)
            {
                nodeTerm = parseTerm(consume());
            }

            return nodeTerm;
        }
        NodeTerm parseTerm(Token term)
        {
            NodeTerm nodeTerm = new NodeTerm();
            if (term.m_Type == TokenType.int_lit)
            {
                string lit = term.m_Value;
                nodeTerm.term = new NodeTermIntlit() { value = lit };
                return nodeTerm;
            }
            else if (term.m_Type == TokenType.ident)
            {
                nodeTerm.term = new NodeTermIdent() { ident = term.m_Value };
                return nodeTerm;
            }

            return null;
        }
        NodeExpr parseExpr(int min_prec = 0)
        {
            NodeTerm term_lhs = parseTerm();
            if (term_lhs == null)
            {
                return null;
            }
            NodeExpr expr_lhs = new NodeExpr() { expr = term_lhs };

            while (true)
            {
                Token? curToken = peek();
                int? prec;
                if (curToken.HasValue)
                {
                    prec = curToken.Value.BinProc();
                    if (!prec.HasValue || prec.Value < min_prec)
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
                TokenType type = consume().m_Type;
                int nextMinPrec = prec.Value + 1;
                NodeExpr expr_rhs = parseExpr(nextMinPrec);
                if (expr_rhs == null)
                {
                    throw new Exception("Bad");
                }
                NodeBinExpr expr = new NodeBinExpr();
                NodeExpr expr_lhs2 = new NodeExpr();
                if (type == TokenType.add)
                {
                    expr_lhs2.expr = expr_lhs.expr;
                    NodeBinExprAdd add = new NodeBinExprAdd();
                    add.lhs = expr_lhs2;
                    add.rhs = expr_rhs;
                    expr.value = add;
                }
                else if (type == TokenType.sub)
                {
                    expr_lhs2.expr = expr_lhs.expr;
                    NodeBinExprSub sub = new NodeBinExprSub();
                    sub.lhs = expr_lhs2;
                    sub.rhs = expr_rhs;
                    expr.value = sub;
                }
                else if (type == TokenType.mult)
                {
                    expr_lhs2.expr = expr_lhs.expr;
                    NodeBinExprMul mul = new NodeBinExprMul();
                    mul.lhs = expr_lhs2;
                    mul.rhs = expr_rhs;
                    expr.value = mul;
                }
                else if (type == TokenType.div)
                {
                    expr_lhs2.expr = expr_lhs.expr;
                    NodeBinExprDiv div = new NodeBinExprDiv();
                    div.lhs = expr_lhs2;
                    div.rhs = expr_rhs;
                    expr.value = div;
                }
                else
                {
                    switch (type)
                    {
                        case TokenType.eq:
                        case TokenType.neq:
                        case TokenType.leq:
                        case TokenType.geq:
                        case TokenType.lt:
                        case TokenType.gt:
                            expr_lhs2.expr = expr_lhs.expr;
                            NodeExprCompersion nodeExprCompersion = new NodeExprCompersion();
                            nodeExprCompersion.lhs = expr_lhs2;
                            nodeExprCompersion.rhs = expr_rhs;
                            nodeExprCompersion.compersionOperators = parseCompersionOperators(type);
                            expr_lhs.expr = nodeExprCompersion;
                            return expr_lhs;
                        default:
                            Console.WriteLine("Some thing's not good");
                            break;
                    }
                }
                expr_lhs.expr = expr;
            }

            return expr_lhs;
        }

        AssignmentOperators parseAssignmentOperator()
        {
            Token _operator = consume();
            return _operator.m_Type switch
            {
                TokenType.assign => AssignmentOperators.assign,
                TokenType.subassign => AssignmentOperators.subassign,
                TokenType.addassign => AssignmentOperators.addassign,
                TokenType.andassign => AssignmentOperators.andassign,
                TokenType.multassign => AssignmentOperators.multassign,
                TokenType.lshiftassign => AssignmentOperators.lshiftassign,
                TokenType.rshiftassign => AssignmentOperators.rshiftassign,
                TokenType.orassign => AssignmentOperators.orassign,
                TokenType.divassign => AssignmentOperators.divassign,
                TokenType.modassign => AssignmentOperators.modassign,
                TokenType.xorassign => AssignmentOperators.xorassign,
                TokenType.inc => AssignmentOperators.inc,
                TokenType.dec => AssignmentOperators.dec,
                _ => AssignmentOperators.none,
            };
        }
        CompersionOperators parseCompersionOperators(TokenType type)
        {
            return type switch
            {
                TokenType.eq => CompersionOperators.eq,
                TokenType.neq => CompersionOperators.neq,
                TokenType.leq => CompersionOperators.leq,
                TokenType.geq => CompersionOperators.geq,
                TokenType.lt => CompersionOperators.lt,
                TokenType.gt => CompersionOperators.gt,
                _ => CompersionOperators.none,
            };
        }
        ExprType parseType()
        {
            ExprType result;

            bool isConst = false;
            bool isVolatile = false;

            if (try_consume(TokenType._const))
            {
                isConst = true;
            }
            if (try_consume(TokenType._volatile))
            {
                if (isConst)
                {
                    throw new Exception("");
                }
                isVolatile = true;
            }

            if (isConst)
            {
                if (isVolatile)
                {
                    throw new Exception();
                }
            }

            if (try_consume(TokenType.pointer))
            {
                Token token = consume();
                switch (token.m_Type)
                {
                    case TokenType._nearPointer:
                        result = new NearPointerType(parseType(), isConst, isVolatile);
                        break;
                    default:
                    case TokenType.uint16:
                        result = new ShortPointerType(parseType(), isConst, isVolatile);
                        break;
                    case TokenType._longPointer:
                        result = new LongPointerType(parseType(), isConst, isVolatile);
                        break;
                    case TokenType._farPointer:
                        result = new FarPointerType(parseType(), isConst, isVolatile);
                        break;
                    case TokenType._void:
                        result = new VoidType(isConst, isVolatile);
                        break;
                }
            }
            else
            {
                result = parseBaseTypes(isConst, isVolatile);
            }

            return result;
        }
        ExprType parseBaseTypes(bool isConst, bool isVolatile)
        {
            int save = index;

            Token token = consume();
            switch (token.m_Type)
            {
                case TokenType.uint8:
                    return new UCharType(isConst, isVolatile);
                case TokenType.int8:
                    return new CharType(isConst, isVolatile);
                case TokenType.uint16:
                    return new UShortType(isConst, isVolatile);
                case TokenType.int16:
                    return new ShortType(isConst, isVolatile);
                case TokenType.uint32:
                    if (InProtectedMode)
                    {
                        return new UIntType(isConst, isVolatile);
                    }
                    goto case TokenType.uint16;
                case TokenType.int32:
                    if (InProtectedMode)
                    {
                        return new IntType(isConst, isVolatile);
                    }
                    goto case TokenType.int16;
                case TokenType._void:
                    Error_UseVoid(peek(-1).Value, this);
                    index = save;
                    return null;
                default:
                    Error_expected(peek(-1).Value, "type", this);
                    index = save;
                    return null;
            }

        }

        NodeScope parseScope(TokenType endType)
        {
            NodeScope result = new NodeScope();
            List<NodeStmt> stmts = new List<NodeStmt>();
            while (true)
            {
                NodeStmt stmt = parse_Stmt();
                stmts.Add(stmt);
                if (peek().HasValue && peek().Value.m_Type == TokenType.end)
                {
                    if (peek(1).HasValue && peek(1).Value.m_Type == endType)
                    {
                        consume();
                        consume();
                        break;
                    }
                }
            }
            result.stmts = stmts.ToArray();
            return result;
        }
        NodeScope parseDIfScope()
        {
            NodeScope result = new NodeScope();
            List<NodeStmt> stmts = new List<NodeStmt>();
            while (true)
            {
                NodeStmt stmt = parse_Stmt();
                stmts.Add(stmt);
                if (peek().HasValue && peek().Value.m_Type == TokenType.DEndIf)
                {
                    consume();
                    break;
                }
            }
            result.stmts = stmts.ToArray();
            return result;
        }
        NodeScope parseIfScope()
        {
            NodeScope result = new NodeScope();
            List<NodeStmt> stmts = new List<NodeStmt>();
            while (true)
            {
                inFunction = true;
                NodeStmt stmt = parse_Stmt();
                if (peek().HasValue && peek().Value.m_Type == TokenType.end)
                {
                    if (peek(1).HasValue && peek(1).Value.m_Type == TokenType._if)
                    {
                        consume();
                        consume();
                        break;
                    }
                }
                if (peek().HasValue && peek().Value.m_Type == TokenType._else)
                {
                    break;
                }
                stmts.Add(stmt);
            }
            result.stmts = stmts.ToArray();
            return result;
        }
        NodeStmtDeclareFunction parseDeclareFunction()
        {
            NodeStmtDeclareFunction result = new NodeStmtDeclareFunction();

            result.Name = try_consume_error(TokenType.ident).m_Value;

            functionNames.Add(result.Name);
            inFunction = true;
            NodeScope nodeScope = parseScope(TokenType.function);
            inFunction = false;
            result.Scope = nodeScope;

            return result;
        }
        NodeStmtDeclareVariabel parseDeclareVariabel()
        {
            NodeStmtDeclareVariabel result = new NodeStmtDeclareVariabel();

            result.Name = try_consume_error(TokenType.ident).m_Value;
            try_consume_error(TokenType.colon);
            result.type = parseType();
            AssignmentOperators operators = parseAssignmentOperator();
            if (operators == AssignmentOperators.none)
            {
                index--;
            }
            else
            {
                if (operators != AssignmentOperators.assign)
                {
                    Error_InvalidOperatorInUse(peek(-1).Value, operators, this);
                }
                result.assignmentOperator = operators;
                result.expr = parseExpr();
            }
            return result;
        }
        NodeStmtReassignVariabel parseReassignVariabel()
        {
            NodeStmtReassignVariabel result = new NodeStmtReassignVariabel();

            result.Name = try_consume_error(TokenType.ident).m_Value;
            AssignmentOperators operators = parseAssignmentOperator();
            if (operators == AssignmentOperators.none)
            {
                Error_expected(peek(-1).Value, "Assignment operator", this);
            }
            result.assignmentOperator = operators;
            result.expr = parseExpr();
            return result;
        }
        NodeStmtWhile parseWhile()
        {
            NodeStmtWhile result = new NodeStmtWhile();
            NodeExpr expr = parseExpr();
            try_consume_error(TokenType._then);
            NodeScope nodeScope = parseScope(TokenType._while);

            result.Expr = expr;
            result.Scope = nodeScope;

            return result;
        }
        NodeStmtIf parseIf()
        {
            NodeStmtIf result = new NodeStmtIf();
            result.Expr = parseExpr();
            parseLineNumber();
            try_consume_error(TokenType._then);
            result.Scope = parseIfScope();

            result.pred = parsePred();

            return result;
        }
        IStmtIf parsePred()
        {
            parseLineNumber();
            if (try_consume(TokenType._else))
            {
                parseLineNumber();
                if (try_consume(TokenType._if))
                {
                    NodeIfPredElif result = new NodeIfPredElif();
                    result.Expr = parseExpr();
                    parseLineNumber();
                    try_consume_error(TokenType._then);
                    result.Scope = parseIfScope();
                    if (peek(-1).Value.m_Type == TokenType._if && peek(-2).Value.m_Type == TokenType.end)
                    {
                        return result;
                    }
                    result.pred = parsePred();
                    return result;
                }
                else
                {
                    parseLineNumber();
                    try_consume_error(TokenType._then);
                    NodeIfPredElse result = new NodeIfPredElse();
                    result.Scope = parseIfScope();
                    return result;
                }
            }
            else
            {
                Error_expected(peek(-1).Value, TokenType._else, this);
            }

            return null;
        }
        NodeStmtBreak parseBreak()
        {
            return new NodeStmtBreak();
        }
        NodeStmtContinue parseContinue()
        {
            return new NodeStmtContinue();
        }
        NodeStmtAsm parseAsm()
        {
            List<AsmArgument> arguments = new List<AsmArgument>();
            NodeStmtAsm result = new NodeStmtAsm();
            try_consume_error(TokenType.open_paren);
            try_consume_error(TokenType.quotation_mark);
            string line = try_consume_error(TokenType.ident).m_Value;
            try_consume_error(TokenType.quotation_mark);

            int index = 0;
            while (peek().HasValue && peek().Value.m_Type == TokenType.comma)
            {
                try_consume_error(TokenType.comma);
                NodeExpr expr = parseExpr();
                arguments.Add(new AsmArgument() { placement = index, value = expr });
            }

            result.asmCode = line;
            result.arguments = arguments.ToArray();

            try_consume_error(TokenType.close_paren);
            return result;
        }
        NodeStmtDeclareDefine parseDeclareDefine()
        {
            NodeStmtDeclareDefine result = new NodeStmtDeclareDefine();
            result.name = try_consume_error(TokenType.ident).m_Value;
            result.expr = null;
            if (try_consume(TokenType.assign))
            {
                result.expr = parseExpr();
            }
            return result;
        }
        NodeStmtDefineIf parseDefineIf()
        {
            NodeStmtDefineIf result = new NodeStmtDefineIf();
            string name = try_consume_error(TokenType.ident).m_Value;
            CompersionOperators compersionOperator = parseCompersionOperators(consume().m_Type);
            NodeExpr expr = parseExpr();
            NodeScope scope = parseDIfScope();

            result.Define = name;
            result.compersionOperators = compersionOperator;
            result.expr = expr;
            result.scope = scope;
            return result;
        }
        void parseLineNumber()
        {
            if (peek().HasValue && peek().Value.m_Type == TokenType.line_number)
            {
                string lineNumber = try_consume_error(TokenType.line_number).m_Value;
                LineNumbers.Add(lineNumber);
                try_consume_error(TokenType.colon);
            }
        }
        NodeStmt parse_Stmt()
        {
            if (!peek().HasValue)
            {
                ExitNow = true;
                return null;
            }

            NodeStmt result = new NodeStmt();
            if (peek().HasValue && peek().Value.m_Type == TokenType.line_number)
            {
                string lineNumber = try_consume_error(TokenType.line_number).m_Value;
                LineNumbers.Add(lineNumber);
                try_consume_error(TokenType.colon);
            }

            if (try_consume(TokenType.defineD))
            {
                result.stmt = parseDeclareDefine();
                return result;
            }
            else
            {
                result.lineNumberInSrc = peek().Value.m_Line;
                result.lineNumber = LineNumbers.Last();
                if (try_consume(TokenType.function))
                {
                    result.stmt = parseDeclareFunction();
                }
                else if (peek().Value.m_Type == TokenType.ident)
                {
                    if (peek(1).HasValue && peek(1).Value.m_Type == TokenType.colon)
                    {
                        result.stmt = parseDeclareVariabel();
                    }
                    else
                    {
                        result.stmt = parseReassignVariabel();
                    }
                }
                else if (try_consume(TokenType._while))
                {
                    result.stmt = parseWhile();
                }
                else if (try_consume(TokenType._if))
                {
                    result.stmt = parseIf();
                }
                else if (try_consume(TokenType._break))
                {
                    result.stmt = parseBreak();
                }
                else if (try_consume(TokenType._continue))
                {
                    result.stmt = parseContinue();
                }
                else if (try_consume(TokenType._asm))
                {
                    result.stmt = parseAsm();
                }
                else if (try_consume(TokenType.DIf))
                {
                    result.stmt = parseDefineIf();
                }

                return result;
            }
            // return null;
        }
        public void Parse_Prog(Token[] tokens)
        {
            output = new ProgNode();
            ExitNow = false;
            Tokens = tokens;
            List<NodeStmt> stmts = new List<NodeStmt>();

            while (peek().HasValue)
            {
                NodeStmt temp = parse_Stmt();
                if (ExitNow)
                {
                    break;
                }
                if (temp == null)
                {
                    continue;
                }
                stmts.Add(temp);

            }

            output.stmts = stmts.ToArray();
        }

        #region Token Functions
        Token? peek(int offset = 0)
        {
            if (index + offset >= Tokens.Length)
                return null;
            return Tokens[index + offset];
        }
        Token consume()
        {
            index++;
            return Tokens[index - 1];
        }
        bool try_consume(TokenType type)
        {
            if (peek().HasValue && peek().Value.m_Type == type)
            {
                consume();
                return true;
            }
            return false;
        }
        Token try_consume_error(TokenType type)
        {
            if (peek().HasValue && peek().Value.m_Type == type)
            {
                return consume();
            }
            index++;
            Error_expected(peek(-1).Value, type, this);
            return new Token();
        }
        #endregion
        bool ExitNow;
    }
}