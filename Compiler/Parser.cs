using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Compiler.Lib.Variants;
using Compiler.ParserNodes;
using static CompilerSettings;
using static CompilerErrors;

namespace Compiler
{
    public class Parser
    {
        List<NodeStmt> m_Stmts = new List<NodeStmt>();
        Token[] m_tokens;
        int m_index = 0;

        const int BYTESIZE = 1;
        const int WORDSIZE = 2;
        const int POINTERSIZE = 3;
        const int DWORDSIZE = 4;

        int parse_size(out bool isSigned, out bool IsString, out int o_size)
        {
            switch (peek().Value.Type)
            {
                case TokenType._byte:
                    consume();
                    isSigned = ISUNSIGNED;
                    IsString = false;
                    o_size = BYTESIZE;
                    return BYTESIZE;
                case TokenType._sbyte:
                    consume();
                    isSigned = ISSIGNED;
                    IsString = false;
                    o_size = BYTESIZE;
                    return BYTESIZE;
                case TokenType._char:
                    consume();
                    isSigned = ISUNSIGNED;
                    IsString = false;
                    if (try_consume(TokenType.open_square))
                    {
                        int size = int.Parse(try_consume_error(TokenType.int_lit).value);
                        IsString = true;
                        try_consume_error(TokenType.close_square);
                        o_size = size + 1;
                        return size + 1;
                    }
                    o_size = BYTESIZE;
                    return BYTESIZE;

                case TokenType._ushort or TokenType.size_t:
                    consume();
                    isSigned = ISUNSIGNED;
                    IsString = false;
                    o_size = WORDSIZE;
                    return WORDSIZE;
                case TokenType._short:
                    consume();
                    isSigned = ISSIGNED;
                    IsString = false;
                    o_size = WORDSIZE;
                    return WORDSIZE;

                case TokenType.pointer:
                    consume();
                    isSigned = ISUNSIGNED;
                    IsString = false;
                    o_size = POINTERSIZE;
                    return POINTERSIZE;
                case TokenType.tbyte:
                    consume();
                    isSigned = ISUNSIGNED;
                    IsString = false;
                    o_size = POINTERSIZE;
                    return POINTERSIZE;

                case TokenType._int:
                    consume();
                    isSigned = ISUNSIGNED;
                    IsString = false;
                    o_size = DWORDSIZE;
                    return DWORDSIZE;

                case TokenType._string:
                    consume();
                    isSigned = ISUNSIGNED;
                    IsString = true;
                    o_size = -2;
                    return -2;

                default:
                    //error_expected(peek(-1).Value, "type");
                    isSigned = ISUNSIGNED;
                    IsString = false;
                    o_size = -1;
                    return -1;
            }
        }
        _Type parse_size(out bool IsString)
        {
            bool isSigned;
            int o_size;
            parse_size(out isSigned, out IsString, out o_size);

            return new _Type() { TypeSize = o_size, IsSigned = isSigned };
        }
        AssignmentOperators parse_assignmentOperator()
        {
            if(try_consume(TokenType.eq))
            {
                return AssignmentOperators.basic_assignment;
            }

            return AssignmentOperators.none;
        }
        NodeTerm parse_term()
        {
            if (!peek().HasValue)
            {
                throw new NotImplementedException("peek has no value");
            }

            if (peek().Value.Type == TokenType.int_lit)
            {
                return new NodeTerm() 
                { 
                    var = new NodeTermIntLit() 
                    { 
                        int_lit = consume() 
                    } 
                };
            }
            else if (peek().Value.Type == TokenType.ident &&
                     peek(1).HasValue && peek(1).Value.Type == TokenType.open_paren)
            {
                Token name = try_consume_error(TokenType.ident);
                try_consume_error(TokenType.open_paren);

                List<NodeExpr> args = new List<NodeExpr>();

                while (!try_consume(TokenType.close_paren))
                {
                    args.Add(new NodeExpr() { var = parse_term() });
                    if (try_consume(TokenType.comma))
                    {
                        continue;
                    }
                }

                return new NodeTerm()
                {
                    var = new NodeTermCallFunction()
                    {
                        FunctionName = name,
                        args = args.ToArray()
                    }
                };
            }
            else if (peek().Value.Type == TokenType.ident)
            {
                return new NodeTerm()
                {
                    var = new NodeTermIdent()
                    {
                        ident = consume()
                    }
                };
            }
            else if (peek().Value.Type == TokenType._sizeof)
            {
                /*
                try_consume_error(TokenType._sizeof);
                try_consume_error(TokenType.open_paren);
                object obj = parse_term(out int add);
                try_consume_error(TokenType.close_paren);
                if (obj != null && obj.GetType() == typeof(Var))
                {
                    Var var = (Var)obj;
                    return var.m_Size - 1;
                }
                return obj;
                 */
            }
            else if (peek().Value.Type == TokenType.quotation_mark)
            {
                try_consume_error(TokenType.quotation_mark);
                string string_ident = try_consume_error(TokenType.ident).value;
                try_consume_error(TokenType.quotation_mark);

                return new NodeTerm()
                {
                    var = new NodeTermString()
                    {
                        str = new Token()
                        {
                            Type = TokenType.ident,
                            value = string_ident,
                        }
                    }
                };
            }
            else if (peek().Value.Type == TokenType.ampersand &&
                     peek(1).HasValue && peek(1).Value.Type == TokenType.ident)
            {
                try_consume_error(TokenType.ampersand);
                string string_ident = try_consume_error(TokenType.ident).value;
                return new NodeTerm()
                {
                    var = new NodeTermReference()
                    {
                        reference = new Token()
                        {
                            Type = TokenType.ident,
                            value = string_ident,
                        }
                    }
                };
            }
            else if (peek().Value.Type == TokenType.open_paren)
            {
                try_consume_error(TokenType.open_paren);
                NodeExpr nodeExpr = parse_Expr();
                try_consume_error(TokenType.close_paren);
                if (nodeExpr == null)
                {
                    error_expected(peek(-1).Value, "expression");
                }

                return new NodeTerm() { var = new NodeTermParen() { Expr = nodeExpr } };
            }
            else if (peek().Value.Type == TokenType.star &&
                     peek(1).HasValue && peek(1).Value.Type == TokenType.ident)
            {
                try_consume_error(TokenType.star);
                string string_ident = try_consume_error(TokenType.ident).value;
                return new NodeTerm()
                {
                    var = new NodeTermPointer()
                    {
                        ident = new Token()
                        {
                            Type = TokenType.ident,
                            value = string_ident,
                        }
                    }
                };
            }
            else if (peek().Value.Type == TokenType.star &&
                     peek(1).HasValue && peek(1).Value.Type == TokenType.int_lit)
            {
                try_consume_error(TokenType.star);
                string string_ident = try_consume_error(TokenType.int_lit).value;
                return new NodeTerm()
                {
                    var = new NodeTermPointer()
                    {
                        ident = new Token()
                        {
                            Type = TokenType.ident,
                            value = string_ident,
                        }
                    }
                };
            }

            return null;

            //error_expected(peek(-1).Value, "expression");
            //throw new NotImplementedException();
        }
        NodeExpr parse_Expr(int min_prec = 0)
        {
            NodeTerm term_lhs = parse_term();
            if (term_lhs == null)
            {
                return null;
            }

            NodeExpr expr_lhs = new NodeExpr() { var = term_lhs };

            while (true)
            {
                Token? currtok = peek();
                int? prec;
                if (currtok.HasValue)
                {
                    prec = currtok.Value.bin_proc();
                    if (!prec.HasValue || prec.Value < min_prec)
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }

                TokenType type = peek().Value.Type;
                int line = peek().Value.line;
                string value = consume().value;

                int nextMinPrec = prec.Value + 1;
                NodeExpr expr_rhs = parse_Expr(nextMinPrec);
                if (expr_rhs == null)
                {
                    error_expected(peek(-1).Value, "expression");
                }

                NodeBinExpr expr = new NodeBinExpr();
                NodeExpr expr_lhs2 = new NodeExpr();

                if (type == TokenType.plus)
                {
                    expr_lhs2.var = expr_lhs.var;
                    NodeBinExprAdd nodeBinExprAdd = new NodeBinExprAdd() { lhs = expr_lhs2, rhs = expr_rhs };
                    expr.var = nodeBinExprAdd;
                }
                else if (type == TokenType.star)
                {
                    expr_lhs2.var = expr_lhs.var;
                    NodeBinExprMulti nodeBinExprMulti = new NodeBinExprMulti() { lhs = expr_lhs2, rhs = expr_rhs };
                    expr.var = nodeBinExprMulti;
                }
                else if (type == TokenType.minus)
                {
                    expr_lhs2.var = expr_lhs.var;
                    NodeBinExprSub nodeBinExprSub = new NodeBinExprSub() { lhs = expr_lhs2, rhs = expr_rhs };
                    expr.var = nodeBinExprSub;
                }
                else if (type == TokenType.fslash)
                {
                    expr_lhs2.var = expr_lhs.var;
                    NodeBinExprDiv nodeBinExprDiv = new NodeBinExprDiv() { lhs = expr_lhs2, rhs = expr_rhs };
                    expr.var = nodeBinExprDiv;
                }
                else
                {
                    Console.WriteLine("NOT WORK");
                    throw new NotImplementedException();
                }

                expr_lhs.var = expr;
            }
            return expr_lhs;
        }

        NodeStmtDeclareVariabel parse_DeclareCountVariabel()
        {
            try_consume_error(TokenType._const);
            parse_size(out bool IsSigned, out bool IsString, out int typesize);
            NodeStmtDeclareVariabel declareVariabel = parse_DeclareVariabel(IsSigned, IsString, typesize);

            declareVariabel.IsConst = true; 
            return declareVariabel;
        }
        NodeStmtDeclareVariabel parse_DeclareVariabel(bool IsSigned, bool IsString, int typesize)
        {
            Token name = try_consume_error(TokenType.ident);
            AssignmentOperators operators = parse_assignmentOperator();
            NodeExpr nodeExpr = null;
            bool IsPublic = false;

            if (operators == AssignmentOperators.basic_assignment)
            {
                nodeExpr = new NodeExpr() { var = parse_term() };
            }
            
            if (typesize == -1)
            {
                error_expected(peek(-1).Value, "type");
            }

            if (IsString)
            {
                if (typesize == -2)
                {
                    NodeTerm nodeTerm = (NodeTerm)(nodeExpr.var.Get<NodeTerm>());
                    NodeTermString nodeTermString = (NodeTermString)(nodeTerm.var.Get<NodeTermString>());
                    typesize = nodeTermString.str.value.Length;
                }
            }

            if (try_consume(TokenType.colon))
            {
                if (try_consume(TokenType._public))
                {
                    IsPublic = true;
                }
            }

            return new NodeStmtDeclareVariabel()
            {
                ident = name,
                _operator = operators,
                expr = nodeExpr,
                IsString = IsString,
                IsPublic = IsPublic,
                IsConst = false,
                Type = new _Type()
                {
                    IsSigned = IsSigned,
                    TypeSize = typesize,
                }
            };
        }
        NodeStmtReAssingnVariabel parse_ReassignVariabel()
        {
            Token name = try_consume_error(TokenType.ident);
            AssignmentOperators operators = parse_assignmentOperator();
            NodeExpr nodeExpr = new NodeExpr() { var = parse_term() };

            return new NodeStmtReAssingnVariabel()
            {
                ident = name,
                _operator = operators,
                expr = nodeExpr,
            };
        }
        NodeStmtEndFunction parse_EndFunction()
        {
            try_consume_error(TokenType.end);
            Token ident = try_consume_error(TokenType.ident);

            return new NodeStmtEndFunction()
            {
                ident = ident,
            };
        }
        NodeStmtDeclareFunction parse_DeclareFunction()
        {
            try_consume_error(TokenType._void);
            Token ident = try_consume_error(TokenType.ident);
            List<NodeFunctionArgs> args = new List<NodeFunctionArgs>();
            try_consume_error(TokenType.open_paren);
            while (!try_consume(TokenType.close_paren))
            {
                bool IsOut;
                
                if (try_consume(TokenType._IN_))
                {
                    IsOut = false;
                }
                else if (try_consume(TokenType._OUT_))
                {
                    IsOut = true;
                }
                else
                {
                    IsOut = false;
                }
                _Type _Type = parse_size(out bool IsString);
                if (IsString)
                {
                    throw new Exception("Argument can't be a string");
                }
                Token A_Name = try_consume_error(TokenType.ident);
                
                args.Add(new NodeFunctionArgs() { name = A_Name, _Type = _Type, IsOut = IsOut});
                if (try_consume(TokenType.comma))
                {
                    continue;
                }
            }

            bool IsPublic = false;
            if (try_consume(TokenType.colon))
            {
                if (try_consume(TokenType._public))
                {
                    IsPublic = true;
                }
            }

            return new NodeStmtDeclareFunction()
            {
                ident = ident,
                IsPublic = IsPublic,
                args = args.ToArray()
            };
        }
        NodeStmtReturn parse_Return()
        {
            try_consume_error(TokenType._return);
            return new NodeStmtReturn()
            {
                ReturnExpr = new NodeExpr() { var = parse_term() },
            };
        }
        NodeStmtPointer parse_Pointer(int offset = 0, TokenType AddressingType = TokenType.ident)
        {
            Token name;
            NodeTerm ident;
            AssignmentOperators operators;
            NodeTerm term;

            if (AddressingType == TokenType.ident)
            {
                name = peek(offset + 1).Value;
                ident = parse_term();
                operators = parse_assignmentOperator();
                term = parse_term();
            }
            else if (AddressingType == TokenType.int_lit)
            {
                name = default;
                ident = parse_term();
                operators = parse_assignmentOperator();
                term = parse_term();
            }
            else
            {
                return null;
            }


            return new NodeStmtPointer()
            {
                name = name,
                ident = new NodeExpr() { var = ident },
                _operator = operators,
                Expr = new NodeExpr() { var = term },
            };
        }
        NodeStmtCallFunction parse_CallFunction()
        {
            Token name = try_consume_error(TokenType.ident);
            try_consume_error(TokenType.open_paren);

            List<NodeExpr> args = new List<NodeExpr>();

            while (!try_consume(TokenType.close_paren))
            {
                args.Add(new NodeExpr() { var = parse_term() });
                if (try_consume(TokenType.comma))
                {
                    continue;
                }
            }

            return new NodeStmtCallFunction()
            {
                FunctionName = name,
                args = args.ToArray()
            };
        }

        void Parse_Stmt(string[] src)
        {
            NodeStmt nodeStmt = new NodeStmt();
            nodeStmt.Line = new Token() { line = peek().Value.line, value = src[peek().Value.line] };
            if (peek().Value.Type == TokenType._void &&
                peek(1).HasValue && peek(1).Value.Type == TokenType.ident &&
                peek(2).HasValue && peek(2).Value.Type == TokenType.open_paren)
            {
                nodeStmt.var = parse_DeclareFunction();
                m_Stmts.Add(nodeStmt);
                return;
            }
            else if (peek().Value.Type == TokenType.end &&
                peek(1).HasValue && peek(1).Value.Type == TokenType.ident)
            {
                nodeStmt.var = parse_EndFunction();
                m_Stmts.Add(nodeStmt);
                return;
            }
            else if (peek().Value.Type == TokenType.ident &&
                    (peek(1).HasValue && peek(1).Value.Type == TokenType.eq ||
                     peek(2).HasValue && peek(2).Value.Type == TokenType.eq))
            {
                nodeStmt.var = parse_ReassignVariabel();
                m_Stmts.Add(nodeStmt);
                return;
            }
            else if (parse_size(out bool IsSigned, out bool IsString, out int o_size) != -1 &&
                     peek().HasValue && peek().Value.Type == TokenType.ident)
            {
                nodeStmt.var = parse_DeclareVariabel(IsSigned, IsString, o_size);
                m_Stmts.Add(nodeStmt);
                return;
            }
            else if (peek().Value.Type == TokenType._return)
            {
                nodeStmt.var = parse_Return();
                m_Stmts.Add(nodeStmt);
                return;
            }
            else if (peek().Value.Type == TokenType._const &&
                     peek(2).HasValue && peek(2).Value.Type == TokenType.ident &&
                    (peek(3).HasValue && peek(3).Value.Type == TokenType.eq ||
                     peek(4).HasValue && peek(4).Value.Type == TokenType.eq))
            {
                nodeStmt.var = parse_DeclareCountVariabel();
                m_Stmts.Add(nodeStmt);
                return;
            }
            else if (peek().Value.Type == TokenType.star &&
                     peek(1).HasValue && peek(1).Value.Type == TokenType.ident &&
                    (peek(2).HasValue && peek(2).Value.Type == TokenType.eq ||
                     peek(3).HasValue && peek(3).Value.Type == TokenType.eq))
            {
                nodeStmt.var = parse_Pointer();
                m_Stmts.Add(nodeStmt);
                return;
            }
            else if (peek(1).HasValue && peek(1).Value.Type == TokenType.star &&
                     peek(2).HasValue && peek(2).Value.Type == TokenType.ident)
            {
                nodeStmt.var = parse_Pointer(1);
                m_Stmts.Add(nodeStmt);
                return;
            }
            else if (peek().Value.Type == TokenType.star &&
                     peek(1).HasValue && peek(1).Value.Type == TokenType.int_lit &&
                    (peek(2).HasValue && peek(2).Value.Type == TokenType.eq ||
                     peek(3).HasValue && peek(3).Value.Type == TokenType.eq))
            {
                nodeStmt.var = parse_Pointer(0, TokenType.int_lit);
                m_Stmts.Add(nodeStmt);
                return;
            }
            else if (peek(1).HasValue && peek(1).Value.Type == TokenType.star &&
                     peek(2).HasValue && peek(2).Value.Type == TokenType.int_lit)
            {
                nodeStmt.var = parse_Pointer(1, TokenType.int_lit);
                m_Stmts.Add(nodeStmt);
                return;
            }
            else if (peek().Value.Type == TokenType.ident &&
                     peek(1).HasValue && peek(1).Value.Type == TokenType.open_paren)
            {
                nodeStmt.var = parse_CallFunction();
                m_Stmts.Add(nodeStmt);
                return;
            }
        }

        public ProgNode Parse_Prog(Token[] tokens, string[] src)
        {
            m_tokens = tokens;

            while (peek().HasValue)
            {
                Parse_Stmt(src);
            }

            return new ProgNode() { stmts = m_Stmts.ToArray() };
        }

        #region Token Functions
        Token? peek(int offset = 0)
        {
            if (m_index + offset >= m_tokens.Length)
                return null;
            return m_tokens[m_index + offset];
        }
        Token consume()
        {
            m_index++;
            return m_tokens[m_index - 1];
        }
        bool try_consume(TokenType type)
        {
            if (peek().HasValue && peek().Value.Type == type)
            {
                consume();
                return true;
            }
            return false;
        }
        Token try_consume_error(TokenType type)
        {
            if (peek().HasValue && peek().Value.Type == type)
            {
                return consume();
            }
            error_expected(peek(-1).Value, type);
            throw new NotImplementedException("yes do it you lazy ass implement it now");
        }
        #endregion
    }
}
