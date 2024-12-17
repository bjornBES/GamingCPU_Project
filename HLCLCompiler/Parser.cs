using HLCLCompiler.Tokenizer;
using HLCLCompiler.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLCLCompiler
{
    public class Parser
    {
        ExprType parseType()
        {
            ExprType result = null;
            bool IsSigned = true;
            bool IsCount = false;

            tryConsumeError(TokenKind.KEYWORD, out TokenKeyword KeywordToken);

            if (KeywordToken.Val == KeywordVal.CONST)
            {
                tryConsumeError(TokenKind.KEYWORD, out KeywordToken);
                IsCount = true;
            }

            switch (KeywordToken.Val)
            {
                case KeywordVal.FARPTR:
                    result = new FarPointerType(parseType(), IsCount);
                    return result;
                case KeywordVal.SHORTPTR:
                    result = new ShortPointerType(parseType(), IsCount);
                    return result;
                default:
                    break;
            }

            switch (KeywordToken.Val)
            {
                case KeywordVal.UNSIGNED:
                    tryConsumeError(TokenKind.KEYWORD, out KeywordToken);
                    IsSigned = false;
                    break;
                case KeywordVal.SIGNED:
                    tryConsumeError(TokenKind.KEYWORD, out KeywordToken);
                    IsSigned = true;
                    break;
                default:
                    break;
            }

            switch (KeywordToken.Val)
            {
                case KeywordVal.CHAR:
                case KeywordVal.BYTE:
                    if (IsSigned)
                    {
                        result = new CharType(IsCount);
                    }
                    else
                    {
                        result = new UCharType(IsCount);
                    }
                    break;
                case KeywordVal.SHORT:
                    if (IsSigned)
                    {
                        result = new ShortType(IsCount);
                    }
                    else
                    {
                        result = new UShortType(IsCount);
                    }
                    break;
                case KeywordVal.INT:
                    if (IsSigned)
                    {
                        result = new IntType(IsCount);
                    }
                    else
                    {
                        result = new UIntType(IsCount);
                    }
                    break;
                case KeywordVal.FLOAT:
                    result = new FloatType(IsCount);
                    break;
                default:
                    break;
            }

            if (tryConsume(TokenKind.OPERATOR, out TokenOperator tokenOperator))
            {
                if (tokenOperator != null && tokenOperator.m_Val == OperatorVal.LBRACKET)
                {
                    ExprType temp = result;

                    tryConsumeError(TokenKind.INT, out TokenInt tokenInt);
                    tryConsumeError(TokenKind.OPERATOR, out tokenOperator);
                    if (tokenOperator.m_Val != OperatorVal.RBRACKET)
                    {
                        CompilerErrors.Error_expected(peek(-2), "rigth bracket ]");
                    }

                    result = new ArrayType(temp, tokenInt.Val);
                }
                else
                {
                    m_index--;
                }
            }


            if (result == null)
            {
                throw new Exception("could not parse a type");
            }

            return result;
        }
        bool IsType()
        {
            int saveIndex = m_index;
            ExprType result = parseType();
            m_index = saveIndex;

            return result != null;
        }
        NodeTerm parseTerm()
        {
            NodeTerm term = new NodeTerm();

            if (tryConsume(TokenKind.INT, out TokenInt tokenInt))
            {
                term.term = new NodeTermIntLit() { value = tokenInt };
            }
            else if (tryConsume(TokenKind.IDENTIFIER, out TokenIdentifier tokenString))
            {
                term.term = new NodeTermIdent() { value = tokenString };
            }
            else if (tryConsume(TokenKind.OPERATOR, out TokenOperator tokenOperator))
            {
                if (tokenOperator.m_Val == OperatorVal.BITAND)
                {
                    if (tryConsume(TokenKind.IDENTIFIER, out tokenString))
                    {
                        term.term = new NodeTermIdent() { value = tokenString };
                    }
                }
            }

            return term;
        }
        NodeExpr parseExpr()
        {
            NodeExpr expr = new NodeExpr();

            if (tryConsume(TokenKind.KEYWORD, out TokenKeyword tokenKeyword))
            {
                if (tokenKeyword.Val == KeywordVal.SIZEOF)
                {
                    tryConsumeError(TokenKind.OPERATOR, out TokenOperator token);
                    if (token.m_Val != OperatorVal.LPAREN)
                    {
                        CompilerErrors.Error_expected(peek(-2), "left paren");
                    }
                    int size = 0;

                    if (IsType())
                    {
                        ExprType type = parseType();
                        switch (type.SizeOf)
                        {
                            case TypeSize.CHAR:
                                size = 1;
                                break;
                            case TypeSize.SHORT_POINTER:
                            case TypeSize.SHORT:
                                size = 2;
                                break;
                            case TypeSize.FAR_POINTER:
                            case TypeSize.FLOAT:
                            case TypeSize.INT:
                                size = 4;
                                break;
                        }
                    }
                    else
                    {

                    }

                    expr.expr = new NodeTerm() { term = new NodeTermIntLit() { value = new TokenInt(size, TokenInt.IntSuffix.U, "") } };

                    tryConsumeError(TokenKind.OPERATOR, out token);
                    if (token.m_Val != OperatorVal.RPAREN)
                    {
                        CompilerErrors.Error_expected(peek(-2), "left paren");
                    }
                }
                else if (tokenKeyword.Val == KeywordVal.CALL)
                {
                    expr.expr = new NodeTerm() { term = new NodeTermCall() { stmtCall = parseCall() } };
                    m_index--;
                }
            }
            else
            {
                expr.expr = parseTerm();
            }


            return expr;
        }
        string parseIdentifier()
        {
            string result = "";
            if (tryConsume(TokenKind.OPERATOR, out TokenOperator token) && token.m_Val == OperatorVal.MULT)
            {
                result = "*";
            }
            tryConsumeError(TokenKind.IDENTIFIER, out TokenIdentifier identifier);
            result += identifier.Val;
            if (peek<TokenOperator>().m_Val == OperatorVal.LBRACKET)
            {
                consume();
                tryConsumeError(TokenKind.INT, out TokenInt intVal);
                result += "[" + intVal.Val + "]";
                if (tryConsumeError(TokenKind.OPERATOR, out token) != null && token.m_Val == OperatorVal.RBRACKET)
                {
                }
            }
            return result;
        }

        NodeStmtDeclareFunction parseDeclareFunction()
        {
            NodeStmtDeclareFunction declareFunction = new NodeStmtDeclareFunction();

            string name = parseIdentifier();

            List<Argument> arguments = new List<Argument>();

            tryConsumeError(TokenKind.OPERATOR, out TokenOperator token);
            if (token.m_Val != OperatorVal.LPAREN)
            {
                CompilerErrors.Error_expected(peek(-2), "left paren");
            }

            while (true)
            {
                tryConsume(TokenKind.OPERATOR, out token);
                if (token == null)
                {

                }
                else if (token.m_Val == OperatorVal.RPAREN)
                {
                    break;
                }
                else
                {
                    m_index--;
                }
                if (peek() == null)
                {
                    Console.WriteLine("WIP Error");
                }

                Argument temp = new Argument();
                temp.ArgumentIndex = arguments.Count;
                temp.Type = parseType();
                temp.Name = parseIdentifier();
                arguments.Add(temp);

                tryConsumeError(TokenKind.OPERATOR, out token);
                if (token.m_Val != OperatorVal.RPAREN)
                {
                    CompilerErrors.Error_expected(peek(-2), "right paren");
                }
                else
                {
                    break;
                }
            }

            arguments.Reverse();

            for (int i = 0; i < arguments.Count; i++)
            {
                arguments[i].ArgumentIndex = i;
            }

            List<NodeStmt> nodeStmts = new List<NodeStmt>();
            tryConsumeError(TokenKind.OPERATOR, out token);
            if (token.m_Val != OperatorVal.LCURL)
            {
                CompilerErrors.Error_expected(peek(-2), "right paren");
            }

            while (true)
            {
                nodeStmts.Add(ParseStmt());

                tryConsume(TokenKind.OPERATOR, out token);
                if (token == null)
                {
                    continue;
                }
                if (token.m_Val == OperatorVal.RCURL)
                {
                    break;
                }
                else
                {
                    m_index--;
                }
            }

            declareFunction.stmts = nodeStmts;
            declareFunction.arguments = arguments.ToArray();
            declareFunction.name = name;
            declareFunction.LineNumber = curToken.m_Line;
            return declareFunction;
        }
        NodeStmtDeclareVariabel parseDeclareVariabel()
        {
            NodeStmtDeclareVariabel result = new NodeStmtDeclareVariabel();

            ExprType type = parseType();
            string name = parseIdentifier();
            result.name = name;
            result.type = type;

            tryConsumeError(TokenKind.OPERATOR, out TokenOperator token);

            if (token == null)
            {

            }
            else if (token.m_Val == OperatorVal.ASSIGN)
            {
                result.OperatorVal = token.m_Val;
                NodeExpr expr = parseExpr();
                result.expr = expr;
            }
            else
            {
                m_index--;
            }

            tryConsumeError(TokenKind.OPERATOR, out token);
            if (token.m_Val != OperatorVal.SEMICOLON)
            {
                CompilerErrors.Error_expected(peek(-1), "Semicolon");
            }

            return result;
        }
        NodeStmtAssignVariabel parseAssignVariabel()
        {
            NodeStmtAssignVariabel result = new NodeStmtAssignVariabel();

            result.name = parseIdentifier();
            tryConsumeError(TokenKind.OPERATOR, out TokenOperator token);
            if (token == null)
            {

            }
            else if (token.m_Val == OperatorVal.ASSIGN)
            {
                result.OperatorVal = token.m_Val;
                NodeExpr expr = parseExpr();
                result.expr = expr;
            }
            else
            {
                m_index--;
            }

            tryConsumeError(TokenKind.OPERATOR, out token);
            if (token.m_Val != OperatorVal.SEMICOLON)
            {
                CompilerErrors.Error_expected(peek(-1), "Semicolon");
            }

            return result;
        }

        NodeStmtCall parseCall()
        {
            NodeStmtCall result = new NodeStmtCall();

            result.name = parseIdentifier();
            tryConsumeError(TokenKind.OPERATOR, out TokenOperator token);
            if (token == null)
            {

            }

            List<NodeExpr> nodeStmts = new List<NodeExpr>();

            if (token.m_Val == OperatorVal.LPAREN)
            {
                while (true)
                {
                    tryConsume(TokenKind.OPERATOR, out token);
                    if (token == null)
                    {

                    }
                    else if (token.m_Val == OperatorVal.RPAREN)
                    {
                        break;
                    }
                    else
                    {
                        m_index--;
                    }
                    if (peek() == null)
                    {
                        Console.WriteLine("WIP Error");
                    }

                    nodeStmts.Add(parseExpr());

                    tryConsumeError(TokenKind.OPERATOR, out token);
                    if (token.m_Val != OperatorVal.RPAREN)
                    {
                        CompilerErrors.Error_expected(peek(-2), "right paren");
                    }
                    else
                    {
                        break;
                    }
                }
                result.args = nodeStmts;
            }

            tryConsumeError(TokenKind.OPERATOR, out token);
            if (token.m_Val != OperatorVal.SEMICOLON)
            {
                CompilerErrors.Error_expected(peek(-1), "Semicolon");
            }

            return result;
        }

        public NodeStmt ParseStmt()
        {
            if (peek() == null)
            {
                return null;
            }

            curToken = peek();
            NodeStmt result = new NodeStmt();

            if (curToken.GetType() == typeof(TokenKeyword))
            {
                TokenKeyword keyword = (TokenKeyword)curToken;
                switch (keyword.Val)
                {
                    case KeywordVal.FUNCTION:
                        consume();
                        result.stmt = parseDeclareFunction();
                        break;
                    case KeywordVal.CALL:
                        consume();
                        result.stmt = parseCall();
                        break;
                    default:
                        if (IsType())
                        {
                            result.stmt = parseDeclareVariabel();
                        }
                        break;
                }
            }
            else if (curToken.GetType() == typeof(TokenIdentifier))
            {
                result.stmt = parseAssignVariabel();
            }
            else if (curToken.GetType() == typeof(TokenOperator))
            {
                TokenOperator tokenOperator = (TokenOperator)curToken;
                if (tokenOperator.m_Val == OperatorVal.MULT)
                {
                    result.stmt = parseAssignVariabel();
                }
            }

            /*
            if (tryConsume(TokenType.function, out _))
            {
                result.stmt = parseDeclareFunction();
            }
            else
            {
                Console.WriteLine("WIP Error");
                Environment.Exit(1);
            }
             */

            return result;
        }

        public ProgNode Build(Token[] tokens)
        {
            m_tokens = tokens;
            m_index = 0;
            ProgNode result = new ProgNode();
            result.prog = new List<NodeStmt>();

            while (peek() != null)
            {
                NodeStmt stmt = ParseStmt();
                if (stmt == null)
                {
                    continue;
                }

                result.prog.Add(stmt);
            }

            return result;
        }

        T peek<T>(int offset = 0) where T : Token
        {
            if (m_index + offset >= m_tokens.Length)
                return null;
            Token token = m_tokens[m_index + offset];
            return token as T;
        }
        Token peek(int offset = 0)
        {
            if (m_index + offset >= m_tokens.Length)
                return null;
            Token token = m_tokens[m_index + offset];
            return token;
        }
        T consume<T>() where T : Token
        {
            Token token = m_tokens[m_index++];
            return token as T;
        }
        Token consume()
        {
            Token token = m_tokens[m_index++];
            return token;
        }
        bool tryConsume<T>(TokenKind kind, out T token) where T : Token
        {
            if (peek<T>() != null && peek<T>().Kind == kind && peek<T>().GetType() == typeof(T))
            {
                Token tok = consume<T>();
                token = tok as T;
                return true;
            }
            token = default;
            return false;
        }

        Token tryConsumeError<T>(TokenKind kind, out T token) where T : Token
        {
            if (tryConsume(kind, out token))
            {
                return token;
            }

            CompilerErrors.Error_expected(peek(-1), kind);

            throw new Exception("type not there");
        }
        Token tryConsumeError(TokenKind kind)
        {
            if (tryConsume(kind, out Token token))
            {
                return token;
            }

            CompilerErrors.Error_expected(peek(-1), kind);

            throw new Exception("type not there");
        }

        Token curToken;
        public Token[] m_tokens;
        public int m_index;
    }
}
