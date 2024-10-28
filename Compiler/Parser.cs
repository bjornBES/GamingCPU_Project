using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Compiler.Lib.Variants;
using static CompilerSettings;
using static CompilerErrors;
using System.Reflection.Metadata.Ecma335;
using Compiler;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics.Tracing;

namespace Compiler
{
    public class Parser
    {
        public int m_StackSize;
        public Stack<int> m_Scopes = new Stack<int>();

        Token[] m_tokens;
        int m_index = 0;
        List<Var> m_var = new List<Var>();
        List<Function> m_functions = new List<Function>();
        List<string> m_lineNumber = new List<string>();
        Token m_lastToken;
        List<string> m_lineNumbers = new List<string>();
        int m_labelCount;
        int m_globalVarIndex = 0;


        Section m_section;

        Dictionary<string, string> m_strings = new Dictionary<string, string>();

        public List<string> m_Output = new List<string>();
        public List<string> m_OutputBss = new List<string>();
        public List<string> m_OutputData = new List<string>();
        public List<string> m_OutputRodata = new List<string>();
        string parseTerm(out string register, TypeData type = null)
        {
                register = "";
            if (!peek().HasValue)
            {
                return null;
            }

            if (peek().Value.m_Type == TokenType.int_lit)
            {
                string term = consume().m_Value;

                uint data = Convert.ToUInt32(term);

                register = "AX";
                return $"0x{Convert.ToString(data, 16)}";
            }
            else if (peek().Value.m_Type == TokenType.ident)
            {
                string name = consume().m_Value;
                if (isVariabel(name, out Var var))
                {
                    if (var.m_Address.m_isGlobal)
                    {
                        register = "R1L";
                        return $"{var.m_Address.GetAddress(this)}";
                        //m_registerB = var.m_Address.m_B;
                    }
                    string address = $"{var.m_Address.GetAddress(this)}";
                    switch ((Size)var.m_TypeData.m_TypeSize)
                    {
                        case Size._byte:
                        case Size._short:
                        case Size._tbyte:
                        case Size._int:
                            register = "AX";
                            return $"{address}";
                        default:
                            break;
                    }
                }
                else if (m_strings.ContainsKey(name))
                {
                    int offset = 0;

                    if (try_consume(TokenType.open_square))
                    {
                        string strOffset = parseTerm(out _);
                        offset = Convert.ToInt32(strOffset, 16);
                        try_consume_error(TokenType.close_square);
                    }

                    string result = $"far @{name}";
                    if (offset != 0)
                    {
                        result += $" + 0x{Convert.ToString(offset, 16)}";
                    }
                    register = "HL";
                    return result;
                }
                else
                {

                }
            }
            else if (peek().Value.m_Type == TokenType.at)
            {
                try_consume_error(TokenType.at);
                register = try_consume_error(TokenType.ident).m_Value;
                return register;
            }
            else if (peek().Value.m_Type == TokenType.ampersand)
            {
                try_consume_error(TokenType.ampersand);

                return parseTerm(out register, type);
            }
            else if (peek().Value.m_Type == TokenType.star)
            {
                try_consume_error(TokenType.star);
                return parseTerm(out register, type);
            }

            return null;
        }
        string parseIdent(out string register)
        {
            register = "";
            if (!peek().HasValue)
            {
                return null;
            }

            if (peek().Value.m_Type == TokenType.ident)
            {
                string name = consume().m_Value;
                if (isVariabel(name, out Var var))
                {
                    string address = $"{var.m_Address.GetAddressRaw(this)}";
                    register = "HL";
                    return address;
                }
                else if (m_strings.ContainsKey(name))
                {
                    register = "HL";
                    return name;
                }
            }

            return null;
        }
        TypeData parseType()
        {
            TypeData result = new TypeData();

            result.m_IsPublic = m_Scopes.Count == 0;

            switch (peek().Value.m_Type)
            {
                case TokenType._const:
                    consume();
                    result.m_IsConst = true;
                    break;
                default:
                    result.m_IsConst = false;
                    break;
            }

            result.m_IsPointer = true;
            switch (peek().Value.m_Type)
            {
                case TokenType._nearPointer:
                    consume();
                    result.m_PointerSize = NEARPOINTERSIZE;
                    break;
                case TokenType._shortPointer:
                    consume();
                    result.m_PointerSize = SHORTPOINTERSIZE;
                    break;
                case TokenType._longPointer:
                    consume();
                    result.m_PointerSize = LONGPOINTERSIZE;
                    break;
                case TokenType._farPointer:
                    consume();
                    result.m_PointerSize = FARPOINTERSIZE;
                    break;
                default:
                    result.m_IsPointer = false;
                    result.m_PointerSize = 0;
                    break;
            }

            switch (peek().Value.m_Type)
            {
                case TokenType.uint8:
                    result.m_TypeSize = BYTESIZE;
                    result.m_IsSigned = ISUNSIGNED;
                    consume();
                    break;
                case TokenType.int8:
                    result.m_TypeSize = BYTESIZE;
                    result.m_IsSigned = ISSIGNED;
                    consume();
                    break;
                case TokenType.int16:
                    result.m_TypeSize = WORDSIZE;
                    result.m_IsSigned = ISSIGNED;
                    consume();
                    break;
                case TokenType._shortPointer:
                case TokenType.uint16:
                    result.m_TypeSize = WORDSIZE;
                    result.m_IsSigned = ISUNSIGNED;
                    consume();
                    break;
                case TokenType.int24:
                    result.m_TypeSize = TBYTESIZE;
                    result.m_IsSigned = ISSIGNED;
                    consume();
                    break;
                case TokenType.uint24:
                    result.m_TypeSize = TBYTESIZE;
                    result.m_IsSigned = ISUNSIGNED;
                    consume();
                    break;
                case TokenType.int32:
                    result.m_TypeSize = DWORDSIZE;
                    result.m_IsSigned = ISSIGNED;
                    consume();
                    break;
                case TokenType.uint32:
                    result.m_TypeSize = DWORDSIZE;
                    result.m_IsSigned = ISUNSIGNED;
                    consume();
                    break;
                case TokenType._void:
                    result.m_TypeSize = 0;
                    result.m_IsSigned = ISUNSIGNED;
                    consume();
                    break;
            }

            return result;
        }

        bool isVariabel(string name, out Var var)
        {
            for (int i = 0; i < m_var.Count; i++)
            {
                if (m_var[i].m_Name == name)
                {
                    var = m_var[i];
                    return true;
                }
            }
            var = default;
            return false;
        }

        bool isType()
        {
            switch (peek().Value.m_Type)
            {
                case TokenType._const:
                case TokenType._nearPointer:
                case TokenType._shortPointer:
                case TokenType._longPointer:
                case TokenType._farPointer:
                case TokenType.uint8:
                case TokenType.int8:
                case TokenType.int16:
                case TokenType.uint16:
                case TokenType.int24:
                case TokenType.uint24:
                case TokenType.int32:
                case TokenType.uint32:
                case TokenType._void:
                    return true;
            }
            return false;
        }

        void parseProgram()
        {
            try_consume_error(TokenType.program);
            string name = try_consume_error(TokenType.ident).m_Value;

            try_consume_error(TokenType.period);

            begin_scope();
            while (peek().HasValue && 
                peek().Value.m_Type != TokenType.end && peek(1).HasValue && peek().Value.m_Type != TokenType.program)
            {
                parse_Stmt();
            }
            try_consume_error(TokenType.end);
            try_consume_error(TokenType.program);
            try_consume_error(TokenType.period);
            end_scope();
        }
        void parseFunction()
        {
            try_consume_error(TokenType.function);
            string name = try_consume_error(TokenType.ident).m_Value;
            try_consume_error(TokenType.open_paren);
            List<Argument> args = new List<Argument>();
            addLine($"_{name}:", 0);
            byte BPoffset = 6;                      // 6 from the return address
            addLine("enter");
            pushr();
            int sizeOfArgument = 0;
            while (!try_consume(TokenType.close_paren))
            {
                TypeData type = parseType();
                string argName = try_consume_error(TokenType.ident).m_Value;

                if (type.m_IsPointer)
                {
                    BPoffset += (byte)type.m_PointerSize;
                    sizeOfArgument += type.m_PointerSize;
                }
                else
                {
                    BPoffset += (byte)type.m_TypeSize;
                    sizeOfArgument += type.m_TypeSize;
                }

                m_var.Add(new Var(argName, type, BPoffset, true));

                args.Add(new Argument()
                {
                    m_Name = argName,
                    m_Type = type,
                });
            }
            try_consume_error(TokenType.period);

            m_functions.Add(new Function()
            {
                m_ArgumentSize = sizeOfArgument,
                m_Name = name,
                m_Arguments = args.ToArray(),
            });

            begin_scope();

            if (name == "main")
            {
                addLine("mov\tR1H,\t0x0003");
            }

            while (peek().HasValue &&
                peek().Value.m_Type != TokenType.end && peek(1).HasValue && peek().Value.m_Type != TokenType.function)
            {
                parse_Stmt();
            }
            try_consume_error(TokenType.end);
            try_consume_error(TokenType.function);
            try_consume_error(TokenType.period);
            if (m_lastToken.m_Type != TokenType._return)
            {
                addLine($"mov\tR1,\t0");
            }
            addLine($"Exit_{name}:", 0);
            popr();
            end_scope();
            addLine($"leave");
            if (m_functions.Last().m_ArgumentSize == 0)
            {
                addLine("retz");
            }
            else
            {
                addLine($"ret\t{m_functions.Last().m_ArgumentSize}");
            }
        }
        void parseVariabel(TypeData type)
        {
            string name = try_consume_error(TokenType.ident).m_Value;
            if (try_consume(TokenType.eq))
            {
                string term = parseTerm(out string register, type);

                if (type.m_IsPointer)
                {
                    term = term.Replace("far ", "");
                    string pointerSize = "short";

                    switch (type.m_PointerSize)
                    {
                        case NEARPOINTERSIZE:   pointerSize = "near"; break; 
                        case SHORTPOINTERSIZE:  pointerSize = "short"; break; 
                        case LONGPOINTERSIZE:   pointerSize = "long"; break; 
                        case FARPOINTERSIZE:    pointerSize = "far"; break; 
                    }

                    term = pointerSize + " " + term;
                }

                declareVariabel(name, type, term, register);
            }
            else
            {
                declareVariabel(name, type);
            }
            try_consume_error(TokenType.period);
        }
        void parseReturn()
        {
            try_consume_error(TokenType._return);
            string term = parseTerm(out string register);

            if (register == "AX")
            {
                addLine($"mov\tAX,\t{term}");
            }
            else
            {
                addLine($"mov\t{register},\t{term}");
                addLine($"mov\tR1,\t{register}");
                addLine($"mov\tA,\tR1");
            }
            addLine($"jmp\t[Exit_{m_functions.Last().m_Name}]");
            try_consume_error(TokenType.period);
        }
        void parseReference()
        {
            try_consume_error(TokenType.star);

            string addressRegister = "";
            string register;

            if (peek().Value.m_Type == TokenType.ident)
            {
                string address = parseIdent(out register);
                addLine($"mov\tHL,\t{address}");
                addressRegister = register;
            }
            else if (peek().Value.m_Type == TokenType.int_lit)
            {
                string address = parseTerm(out register);
                addLine($"mov\tHL,\t{address}");
                addressRegister = register;
            }

            try_consume_error(TokenType.eq);
            string data = parseTerm(out register);

            if (register == "AX")
            {
                addLine($"mov\tAX,\t{data}");
            }
            else
            {
                addLine($"mov\t{register},\t{data}");
            }
            addLine($"mov\t[{addressRegister}]\t{register}");


            try_consume_error(TokenType.period);
        }
        void parseReassign()
        {
            string name = try_consume_error(TokenType.ident).m_Value;

            if (isVariabel(name, out Var var))
            {
                try_consume_error(TokenType.eq);
                string term = parseTerm(out string register, var.m_TypeData);
                string address;

                if (var.m_Address.m_isGlobal)
                {
                    addLine($"mov\tR1L,\t{var.m_Address.GetAddress(this)}");
                    //m_registerB = var.m_Address.m_B;
                    address = "[R1L]";
                }
                else
                {
                    address = var.m_Address.GetAddress(this);
                }

                if (register != term)
                {

                    if (register == "AX")
                    {
                        addLine($"mov\tAX,\t{term}");
                    }
                    else
                    {
                        addLine($"mov\t{register},\t{term}");
                    }
                }
                else
                {
                }

                addLine($"mov\t{address},\t{register}");
                addLine($"; _{name} = {term}");

                try_consume_error(TokenType.period);
            }
        }
        void parseReassignRegister()
        {
            try_consume_error(TokenType.at);
            string register = try_consume_error(TokenType.ident).m_Value;
            try_consume_error(TokenType.eq);
            string term = parseTerm(out string dataRegister);
            if (register == "AX")
            {
                addLine($"mov\tAX,\t{term}");
            }
            else
            {
                addLine($"mov\t{register},\t{term}");
            }
            try_consume_error(TokenType.period);
        }
        void parseInvoke()
        {
            try_consume_error(TokenType.invoke);
            try_consume_error(TokenType.open_paren);
            string Routine = parseTerm(out string routineRegister);
            try_consume_error(TokenType.comma);
            string Function = parseTerm(out string functionRegister);
            addLine($"mov\tAH,\t{Function}");
            addLine($"int\t{Routine}");
            try_consume_error(TokenType.close_paren);
            try_consume_error(TokenType.period);
        }
        void parseDisplay()
        {
            try_consume_error(TokenType.display);
            try_consume_error(TokenType.open_paren);

            string term = parseTerm(out string register);

            push32($"HL");
            push32($"AX");
            addLine($"mov\tHL,\t{term}");
            addLine($"mov\tAH,\t0x02");
            addLine($"int\t0x10");
            pop32("AX");
            pop32($"{register}");


            try_consume_error(TokenType.close_paren);
            try_consume_error(TokenType.period);
        }
        void parseIf()
        {
            Token LastSegment;
            LastSegment = try_consume_error(TokenType._if);
            string label = GetLabel();
            string exitLable = $"{label}_IfExit";
            string fExitLable = $"{label}_Exit";
            addLine("; if", 0);
            addLine(label + ":", 0);

            string term1 = parseTerm(out string RegisterTerm1);
            if (try_consume(TokenType.eq))
            {
                try_consume_error(TokenType.eq);
                string term2 = parseTerm(out string RegisterTerm2);
                if (RegisterTerm1 == "AX")
                {
                    addLine($"mov\tAX,\t{term1}");
                }
                else
                {
                    addLine($"mov\t{RegisterTerm1},\t{term1}");
                }
                addLine($"mov\tR19,\t{RegisterTerm1}");
                if (RegisterTerm2 == "AX")
                {
                    addLine($"mov\tAX,\t{term2}");
                }
                else
                {
                    addLine($"mov\t{RegisterTerm2},\t{term2}");
                }
                addLine($"mov\tR20,\t{RegisterTerm2}");

                addLine($"cmp\tR19,\tR20");
                addLine($"jne\t[{exitLable}]");
            }
            else
            {
                if (RegisterTerm1 == "AX")
                {
                    addLine($"mov\tAX,\t{term1}");
                }
                else
                {
                    addLine($"mov\t{RegisterTerm1},\t{term1}");
                }

                addLine($"jz\t[{exitLable}]");
            }

            try_consume_error(TokenType.period);
            try_consume_error(TokenType._then);
            try_consume_error(TokenType.period);
            begin_scope();
            while (peek().HasValue && peek(1).HasValue && 
                    peek().Value.m_Type != TokenType.end && peek(1).Value.m_Type != TokenType._if &&
                    peek().Value.m_Type != TokenType._elif &&
                    peek().Value.m_Type != TokenType._else)
            {
                parse_Stmt();
            }
            if (m_lastToken.m_Type != TokenType._return)
            {
                addLine($"jmp\t[{fExitLable}]");
            }

            addLine(exitLable + ":", 0);

            if (peek().Value.m_Type == TokenType._else)
            {
                LastSegment = try_consume_error(TokenType._else);
                addLine("; else", 0);
                try_consume_error(TokenType.period);
                try_consume_error(TokenType._then);
                try_consume_error(TokenType.period);
                begin_scope();
                while (peek().HasValue && peek(1).HasValue &&
                    peek().Value.m_Type != TokenType.end && peek(1).Value.m_Type != TokenType._if)
                {
                    parse_Stmt();
                }
                if (m_lastToken.m_Type != TokenType._return)
                {
                    addLine($"jmp\t[{fExitLable}]");
                }
            }

            if (try_consume(TokenType.end))
            {
                addLine("; end if", 0);
                try_consume_error(TokenType._if);
                try_consume_error(TokenType.period);
                    addLine(fExitLable + ":", 0);
                if (m_lastToken.m_Type == TokenType._return || LastSegment.m_Type == TokenType._else)
                {
                }
                else
                {
                    popr();
                    end_scope();
                }
            }
        }

        void parse_Stmt()
        {
            if (!peek().HasValue)
            {
                return;
            }

            if (try_consume(TokenType.Section))
            {
                try_consume_error(TokenType.colon);
                switch (peek().Value.m_Type)
                {
                    case TokenType.SectionText:
                        consume();
                        m_section = Section.text;
                        break;
                    case TokenType.SectionData:
                        consume();
                        m_section = Section.data;
                        break;
                    case TokenType.SectionString:
                        consume();
                        m_section = Section._string;
                        break;
                    default:
                        break;
                }
            }

            if (m_section == Section.text)
            {
                if (peek().Value.m_Type == TokenType.int_lit && peek(1).HasValue && peek(1).Value.m_Type == TokenType.colon)
                {
                    string linenumber = consume().m_Value;

                    if (m_lineNumber.Contains(linenumber))
                    {
                        Console.WriteLine($"Error: two lines can't have the same line number {linenumber}");
                    }

                    try_consume_error(TokenType.colon);
                    m_lineNumber.Add(linenumber);
                    addLine($"L{linenumber}:", 0);
                    m_lineNumbers.Add(linenumber);
                    return;
                }

                if (peek().Value.m_Type == TokenType.program && peek(1).HasValue && peek(1).Value.m_Type == TokenType.ident)
                {
                    m_lastToken = peek().Value;
                    parseProgram();
                    return;
                }
                else if (peek().Value.m_Type == TokenType.function && peek(1).HasValue && peek(1).Value.m_Type == TokenType.ident)
                {
                    m_lastToken = peek().Value;
                    parseFunction();
                    return;
                }
                else if (peek().Value.m_Type == TokenType._return)
                {
                    m_lastToken = peek().Value;
                    parseReturn();
                    return;
                }
                else if (isType())
                {
                    m_lastToken = peek().Value;
                    TypeData type = parseType();
                    if (peek().HasValue && peek().Value.m_Type == TokenType.ident)
                    {
                        parseVariabel(type);
                        return;
                    }
                    else
                    {
                        Error_expected(peek(-1).Value, m_lineNumbers.ToArray(), "name");
                    }
                }
                else if (peek().Value.m_Type == TokenType.ident && peek(1).HasValue && peek(1).Value.m_Type == TokenType.eq)
                {
                    m_lastToken = peek().Value;
                    parseReassign();
                    return;
                }
                else if (peek().Value.m_Type == TokenType.at && peek(1).HasValue && peek(1).Value.m_Type == TokenType.ident)
                {
                    m_lastToken = peek().Value;
                    parseReassignRegister();
                    return;
                }
                else if (peek().Value.m_Type == TokenType.invoke && peek(1).HasValue && peek(1).Value.m_Type == TokenType.open_paren)
                {
                    m_lastToken = peek().Value;
                    parseInvoke();
                    return;
                }
                else if (peek().Value.m_Type == TokenType.display && peek(1).HasValue && peek(1).Value.m_Type == TokenType.open_paren)
                {
                    if (!m_DoRaw)
                    {
                        m_lastToken = peek().Value;
                        parseDisplay();
                        return;
                    }
                }
                else if (peek().Value.m_Type == TokenType.star)
                {
                    parseReference();
                    return;
                }
                else if (peek().Value.m_Type == TokenType._if)
                {
                    parseIf();
                    return;
                }
            }
            else if (m_section == Section._string)
            {
                if (try_consume(TokenType.colon))
                {
                    string name = try_consume_error(TokenType.ident).m_Value;
                    try_consume_error(TokenType.quotation_mark);
                    string value = try_consume_error(TokenType.ident).m_Value;
                    try_consume_error(TokenType.quotation_mark);
                    m_strings.Add(name, value);
                    addLine($"{name}:", 0, AsmSection.rodata);
                    addLine($".db\t\"{value}\", 0", 0, AsmSection.rodata);
                    return;
                }
            }
            else if (m_section == Section.data) // BSS
            {
                if (try_consume(TokenType.colon))
                {
                    string name = try_consume_error(TokenType.ident).m_Value;
                    try_consume_error(TokenType._res);
                    string value = try_consume_error(TokenType.int_lit).m_Value;
                    addLine($"{name}:", 0, AsmSection.bss);
                    addLine($".res\t{value}", 0, AsmSection.bss);
                    return;
                }
            }

            Console.WriteLine($"-1 = {peek(-1).Value}");
            Console.WriteLine($"Need parsing for {peek().Value}");
            Console.WriteLine($"1 =  {peek(1).Value}");
        }

        public void Parse_Prog(Token[] tokens, string[] src)
        {
            addLine($".setcpu \"{m_CPUType}\"", 0);
            addLine($".section TEXT", 0);
            m_tokens = tokens;

            while (peek().HasValue)
            {
                parse_Stmt();
            }

            addLine($"{Environment.NewLine}.section BSS", 0);
            m_Output.AddRange(m_OutputBss);

            addLine($"{Environment.NewLine}.section DATA", 0);
            addLine($".local __DATASTRAT__", 0);
            addLine($"__DATASTRAT__:", 0);
            m_Output.AddRange(m_OutputData);
            addLine($"{Environment.NewLine}.section RODATA", 0);
            addLine($".local __RODATASTRAT__", 0);
            addLine($"__RODATASTRAT__:", 0);
            m_Output.AddRange(m_OutputRodata);

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
            Error_expected(peek(-1).Value, m_lineNumbers.ToArray(), type);
            throw new NotImplementedException("yes do it you lazy ass implement it now");
        }
        #endregion

        string GetLabel()
        {
            return $"_Label{m_labelCount++}";
        }

        void begin_scope()
        {
            m_Scopes.Push(m_var.Count);
        }
        void end_scope()
        {
            if (m_Scopes.Count > 1)
            {
                int popCount = m_var.Count - m_Scopes.Peek();

                if (popCount != 0)
                {
                    addLine($"sub\tSP,\t{popCount * 2}");
                }

                m_StackSize -= popCount;

                m_var.RemoveRange(m_Scopes.Peek(), popCount);
            }
            else if (m_Scopes.Count == 1)
            {
                int popCount = m_var.Count - m_Scopes.Peek();
                m_var.RemoveRange(m_Scopes.Peek(), popCount);
            }

            m_Scopes.Pop();
        }
        
        void push16(string reg16)
        {
            addLine($"push\t{reg16}");
            m_StackSize += 2;
        }
        void push32(string reg32)
        {
            addLine($"push\t{reg32}");
            m_StackSize += 4;
        }
        void pushr()
        {
            addLine($"pushr");
            m_StackSize += 20;
        }

        void pop16(string reg16)
        {
            addLine($"pop\t{reg16}");
            m_StackSize -= 2;
        }
        void pop32(string reg32)
        {
            addLine($"pop\t{reg32}");
            m_StackSize -= 4;
        }
        void popr()
        {
            addLine($"popr");
            m_StackSize -= 20;
        }

        void declareVariabel(string name, TypeData type, string term = "", string register = "")
        {
            if (m_Scopes.Count == 0)
            {
                addLine($"_{name}", section: AsmSection.bss);
                addLine($".res {type.m_TypeSize}", section: AsmSection.bss);
                if (term == "")
                {
                    if (register == "AX")
                    {
                        addLine($"mov\tAX,\t{term}");
                    }
                    else
                    {
                        addLine($"mov\t{register},\t{term}");
                    }
                    addLine($"mov\t[_{name}],\t{register}");
                }
                m_var.Add(new Var());
            }
            else if (m_Scopes.Count == 1)
            {
                addLine($"; _{name} is at 0x0003:{Convert.ToString(m_globalVarIndex)}", section: AsmSection.text);
                m_var.Add(new Var(name, type, m_globalVarIndex, 0))
                ;
                m_globalVarIndex += 4;
                //addLine($".res {type.m_TypeSize}", section: AsmSection.bss);
                /*
                if (term != "A")
                {
                    addLine($"mov\tA,\t{term}");
                }   
                addLine($"mov\t[_{name}],\tA");
                 */
            }
            else
            {
                m_var.Add(new Var(name, type, m_StackSize));

                if (register == "AX")
                {
                    addLine($"mov\tAX,\t{term}");
                }
                else
                {
                    addLine($"mov\t{register},\t{term}");
                }

                if (register == "AX")
                {
                    //addLine($"mov\tA,\t{term}");
                    push32("AX");
                }
                else if (register == "HL")
                {
                    push32("HL");
                }
                else
                {
                }
            }
        }

        void addLine(string line, int taps = 1, AsmSection section = AsmSection.text)
        {
            string[] splited = line.Split('\t');

            splited[0] = splited[0].PadRight(6, ' ');
            line = splited[0];

            for (int i = 1; i < splited.Length; i++)
            {
                line += splited[i].PadRight(25, ' ');
            }

            line = line.TrimEnd();


            switch (section)
            {
                case AsmSection.bss:
                    m_OutputBss.Add("".PadLeft(taps, '\t') + line);
                    break;
                case AsmSection.rodata:
                    m_OutputRodata.Add("".PadLeft(taps, '\t') + line);
                    //m_output_rdata.Add(line);
                    break;
                case AsmSection.data:
                    m_OutputData.Add("".PadLeft(taps, '\t') + line);
                    //m_output_data.Add(line.Replace(":|:", $":{Environment.NewLine}\t"));
                    break;
                case AsmSection.text:
                    m_Output.Add("".PadLeft(taps, '\t') + line);
                    break;
                default:
                    break;
            }
        }
    }
    enum AsmSection
    {
        bss,
        rodata,
        data,
        text,
        loacl,
    }
    enum Section
    {
        text,
        data,
        _string,
    }
    enum Size
    {
        _byte = 1,
        _short = 2,
        _tbyte = 3,
        _int = 4,
        pointer,
    }
}
public struct Var
{
    public string m_Name;
    public TypeData m_TypeData;
    public Address m_Address;

    public Var(string name, TypeData typeData, int stack)
    {
        m_Name = name;
        m_TypeData = typeData;
        m_Address.m_UseStack = true;
        m_Address.m_StackLoc = stack;
        m_Address.m_IsArg = false;
    }
    public Var(string name, TypeData typeData, int stack, bool Isarg)
    {
        m_Name = name;
        m_TypeData = typeData;
        m_Address.m_UseStack = true;
        m_Address.m_StackLoc = stack;
        m_Address.m_IsArg = Isarg;
    }
    public Var(string name, TypeData typeData, int address, int a)
    {
        m_Name = name;
        m_TypeData = typeData;
        m_Address.m_isGlobal = true;
        m_Address.m_address = address;
    }
}
public struct Address
{
    public bool m_UseStack;
    public bool m_IsArg;
    public int m_StackLoc;

    public bool m_isGlobal;
    public int m_address;

    public string GetAddress(Parser parser)
    {
        if (m_UseStack == true)
        {
            if (m_IsArg)
            {
                int address = m_StackLoc;
                if (address == 0)
                {
                    if (m_CPUType < CPUType.BC32)
                    {
                        return $"[BP]";
                    }
                    else
                    {
                        return $"[BPX]";
                    }
                }
                else
                {
                    if (m_CPUType < CPUType.BC32)
                    {
                        return $"[BP - {address}]";
                    }
                    else
                    {
                        return $"[BPX- {address}]";
                    }
                }
            }
            else
            {
                int address = parser.m_StackSize - m_StackLoc;
                if (address == 0)
                {
                    if (m_CPUType < CPUType.BC32)
                    {
                        return $"[SP]";
                    }
                    else
                    {
                        return $"[SPX]";
                    }
                }
                else
                {
                    if (m_CPUType < CPUType.BC32)
                    {
                        return $"[SP - {address}]";
                    }
                    else
                    {
                        return $"[SPX - {address}]";
                    }
                }
            }
        }
        else if (m_isGlobal)
        {
            string hex = Convert.ToString(m_address, 16).PadLeft(4, '0');
            return $"0x{hex}";
        }


        return "";
    }
    public string GetAddressRaw(Parser parser)
    {
        if (m_UseStack == true)
        {
            if (m_IsArg)
            {
                int address = m_StackLoc;
                if (address == 0)
                {
                    if (m_CPUType < CPUType.BC32)
                    {
                        return $"[BP]";
                    }
                    else
                    {
                        return $"[BPX]";
                    }
                }
                else
                {
                    if (m_CPUType < CPUType.BC32)
                    {
                        return $"[BP - {address}]";
                    }
                    else
                    {
                        return $"[BPX- {address}]";
                    }
                }
            }
            else
            {
                int address = parser.m_StackSize - m_StackLoc;
                if (address == 0)
                {
                    if (m_CPUType < CPUType.BC32)
                    {
                        return $"[SP]";
                    }
                    else
                    {
                        return $"[SPX]";
                    }
                }
                else
                {
                    if (m_CPUType < CPUType.BC32)
                    {
                        return $"[SP - {address}]";
                    }
                    else
                    {
                        return $"[SPX - {address}]";
                    }
                }
            }
        }
        else if (m_isGlobal)
        {
            return $"far @__DATASTRAT__ + {m_address}";
        }


        return "";
    }
}
public struct Function
{
    public string m_Name;
    public Argument[] m_Arguments;
    public int m_ArgumentSize;
}
public struct Argument
{
    public string m_Name;
    public TypeData m_Type;
}