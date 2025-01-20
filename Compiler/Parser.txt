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
using System.Diagnostics.CodeAnalysis;

namespace Compiler
{
    public class Parser : ParserCPUInstrctions
    {
        string parseTerm(out string register, TypeData type = null)
        {
            register = "";
            if (!peek().HasValue)
            {
                return null;
            }

            if (peek().Value.m_Type == TokenType.int_lit)
            {
                return parseTerm(consume(), out register, type);
            }
            else if (peek().Value.m_Type == TokenType.ident)
            {
                string name = consume().m_Value;
                if (m_strings.ContainsKey(name))
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
                    return parseTerm(peek(-1).Value, out register, type);
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
            else if (peek().Value.m_Type == TokenType.open_square)
            {
                try_consume_error(TokenType.open_square);
                string data = parseTerm(out register, type);
                try_consume_error(TokenType.close_square);
                return data;
            }

            return null;
        }
        string parseTerm(Token term, out string register, TypeData type = null)
        {
            register = "";
            if (!peek().HasValue)
            {
                return null;
            }

            if (term.m_Type == TokenType.int_lit)
            {
                string value = term.m_Value;

                uint data = Convert.ToUInt32(value);

                register = "AX";
                return $"0x{Convert.ToString(data, 16)}";
            }
            else if (term.m_Type == TokenType.ident)
            {
                string name = term.m_Value;
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
                else if (m_defines.ContainsKey(name))
                {
                    register = "AX";
                    return m_defines[name];
                }
                else if (m_bssDefines.ContainsKey(name))
                {
                    register = "HL";
                    return $"{m_bssDefines[name].m_offset}";
                }
                else if (m_functions.Exists(func => { return func.m_Name == name; }))
                {
                    register = "HL";
                    return $"@_{name}";
                }
                else
                {

                }
            }

            return null;
        }
        bool isParseTerm()
        {
            if (!peek().HasValue)
            {
                return false;
            }

            if (peek().Value.m_Type == TokenType.int_lit)
            {
                return true;
            }
            else if (peek().Value.m_Type == TokenType.ident)
            {
                string name = peek(0).Value.m_Value;
                if (isVariabel(name, out Var var))
                {
                    return true;
                }
                else if (m_strings.ContainsKey(name))
                {
                    return true;
                }
                else if (m_defines.ContainsKey(name))
                {
                    return true;
                }
                else if (m_bssDefines.ContainsKey(name))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (peek().Value.m_Type == TokenType.at)
            {
                return true;
            }
            else if (peek().Value.m_Type == TokenType.ampersand)
            {
                return true;
            }
            else if (peek().Value.m_Type == TokenType.open_square)
            {
                return true;
            }

            return false;
        }
        string parseExpr()
        {
            push(Regs.AX);
            push(Regs.BX);
            push(Regs.HL);


            pop32("HL");
            pop32("BX");
            pop32("AX");
            return "";
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

        void parseFunction()
        {
            try_consume_error(TokenType.function);
            string name = try_consume_error(TokenType.ident).m_Value;
            try_consume_error(TokenType.open_paren);
            List<Argument> args = new List<Argument>();
            byte BPoffset = 4;                      // 6 from the return address
            m_StackSize += 4;
            if (DoNotWrite == true)
            {
                addLine($"_{name}:", 0);
                addLine("enter");
                m_StackSize += 2;
                BPoffset += 2;
                pushr();
            }
            int sizeOfArgument = 0;
            while (!try_consume(TokenType.close_paren))
            {
                TypeData type = parseType();
                string argName = try_consume_error(TokenType.ident).m_Value;

                if (type.m_IsPointer)
                {
                    BPoffset += 4;
                    sizeOfArgument += type.m_PointerSize;
                }
                else
                {
                    BPoffset += 4;
                    sizeOfArgument += type.m_TypeSize;
                }

                if (DoNotWrite == true)
                {
                    m_var.Add(new Var(argName, type, BPoffset, true));

                }
                    args.Add(new Argument()
                    {
                        m_Name = argName,
                        m_Type = type,
                    });
                if (try_consume(TokenType.comma))
                {
                    continue;
                }
                else
                {
                    try_consume_error(TokenType.close_paren);
                    break;
                }
            }
            if (DoNotWrite == true)
            {
                try_consume_error(TokenType.period);
            }

            m_functions.Add(new Function()
            {
                m_ArgumentSize = sizeOfArgument,
                m_Name = name,
                m_Arguments = args.ToArray(),
            });

            if (DoNotWrite == true)
            {
                begin_scope();

                if (name == "main")
                {
                    addLine("mov\tR1H,\t0x0003");
                }

                while (peek().HasValue &&
                    peek().Value.m_Type != TokenType.end && peek(1).HasValue && peek().Value.m_Type != TokenType.function)
                {
                    parse_Stmt();
                    if (ExitNow)
                    {
                        return;
                    }
                }
                try_consume_error(TokenType.end);
                try_consume_error(TokenType.function);
                try_consume_error(TokenType.period);
                if (m_lastToken.m_Type != TokenType._return)
                {
                    addLine($"mov\tR16,\t0");
                }
                addLine($"Exit_{name}:", 0);
                end_scope();
                popr();
                addLine($"leave");
                m_StackSize -= 6;
                addLine($"mov\tAX,\tR16");
                if (m_functions.Last().m_ArgumentSize == 0)
                {
                    addLine("retz");
                }
                else
                {
                    addLine($"ret\t{m_functions.Last().m_ArgumentSize * 4}");
                }
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
                        case NEARPOINTERSIZE:
                            pointerSize = "near";
                            break;
                        case SHORTPOINTERSIZE:
                            pointerSize = "short";
                            break;
                        case LONGPOINTERSIZE:
                            pointerSize = "long";
                            break;
                        case FARPOINTERSIZE:
                            pointerSize = "far";
                            break;
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

            addLine($"mov\tR16,\t{term}");
            addLine($"jmp\t[Exit_{m_functions.Last().m_Name}]");
            try_consume_error(TokenType.period);
        }
        void parseReference()
        {
            string addressRegister = "";
            string register;
            
            try_consume_error(TokenType.open_square);
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
            try_consume_error(TokenType.close_square);

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
            if (peek().Value.m_Type == TokenType.open_square)
            {

            }
            string name = try_consume_error(TokenType.ident).m_Value;
            string address = "";
            string term = "";
            string register = "";
            if (isVariabel(name, out Var var))
            {
                try_consume_error(TokenType.eq);
                term = parseTerm(out register, var.m_TypeData);

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

            }
            else
            {
                m_index--;
                if (isParseTerm())
                {

                }
            }
            addLine($"mov\t{address},\t{register}");
            addLine($"; _{name} = {term}");

            try_consume_error(TokenType.period);
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
            addLine($"clc");
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
                    end_scope();
                    popr();
                }
            }
        }
        void parseArray()
        {
            string address = parseTerm(out string addressRegister);

            try_consume_error(TokenType.open_square);
            string index = parseTerm(out string indexRegister);
            try_consume_error(TokenType.close_square);
        }
        void parseCallFunction()
        {
            try_consume_error(TokenType.call);

            string name = try_consume_error(TokenType.ident).m_Value;

            Function function = m_functions.Find(func =>
            {
                return func.m_Name == name;
            });
            int argumentIndex = 0;

            List<(string, string)> arguments = new List<(string, string)>();

            try_consume_error(TokenType.open_paren);
            while (!try_consume(TokenType.close_paren))
            {
                string term = parseTerm(out string termRegister, function.m_Arguments[argumentIndex].m_Type);
                argumentIndex++;

                arguments.Add((term, termRegister));
                if (peek().Value.m_Type == TokenType.comma)
                {
                    consume();
                    continue;
                }
            }

            for (int i = arguments.Count - 1; i > -1; i--)
            {
                (string term, string termRegister) = arguments[i];

                addLine($"; arguemnt {i}");
                addLine($"mov\t{termRegister},\t{term}");
                addLine($"push\t{termRegister}");
            }

            addLine($"call\tfar [_{function.m_Name}]");

            try_consume_error(TokenType.period);
        }
        void parseAsm()
        {
            try_consume_error(TokenType._asm);
            try_consume_error(TokenType.open_paren);
            try_consume_error(TokenType.quotation_mark);
            string line = try_consume_error(TokenType.ident).m_Value;
            string asm = "";
            int registerIndex = 2;

            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '{')
                {
                    i++;
                    if (line[i] == '{')
                    {
                        asm += "{";
                        continue;
                    }
                    while (i < line.Length && line[i] != '}')
                    {
                        i++; 
                    }
                    string term = parseTerm(out string register);
                    addLine($"mov\tR{registerIndex},\t{term}");
                    asm += $"R{registerIndex}";
                    registerIndex++;
                }
                else if (line[i] == '\\')
                {
                    i++;
                    if (line[i] == 't')
                    {
                        asm += "\t";
                    }
                }
                else
                {
                    asm += line[i];
                }
            }
            try_consume_error(TokenType.quotation_mark);
            try_consume_error(TokenType.close_paren);
            try_consume_error(TokenType.period);

            addLine(asm);
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
                        return;
                    case TokenType.SectionData:
                        consume();
                        m_section = Section.data;
                        return;
                    case TokenType.SectionString:
                        consume();
                        m_section = Section._string;
                        return;
                    default:
                        break;
                }
            }

            if (try_consume(TokenType.numberSign))
            {
                try_consume_error(TokenType.define);
                string name = try_consume_error(TokenType.ident).m_Value;
                try_consume_error(TokenType.eq);
                string term = parseTerm(out _);
                m_defines.Add(name, term);
                return;
            }

            if (m_section == Section.text)
            {
                if (peek().Value.m_Type == TokenType.int_lit && peek(1).HasValue && peek(1).Value.m_Type == TokenType.colon)
                {
                    string linenumber = consume().m_Value;

                    if (m_lineNumber.Contains(linenumber))
                    {
                        Console.WriteLine($"Error: two lines can't have the same line number {peek().Value.m_File}:{linenumber}");
                    }

                    try_consume_error(TokenType.colon);
                    m_lineNumber.Add(linenumber);
                    //addLine($"L{linenumber}:", 0);
                    m_lineNumbers.Add(linenumber);
                    return;
                }

                if (peek().Value.m_Type == TokenType.function && peek(1).HasValue && peek(1).Value.m_Type == TokenType.ident)
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
                        Error_expected(peek(-1).Value, m_lineNumbers.Last(), "name");
                    }
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
                else if (isParseTerm() && isThereRange(TokenType.eq, 5))
                {
                    m_lastToken = peek().Value;
                    parseReassign();
                    return;
                }
                else if (peek().Value.m_Type == TokenType.open_square)
                {
                    parseReference();
                    return;
                }
                else if (peek().Value.m_Type == TokenType._if)
                {
                    parseIf();
                    return;
                }
                else if (peek(1).Value.m_Type == TokenType.open_square)
                {
                    parseArray();
                    return;
                }
                else if (peek().Value.m_Type == TokenType.call)
                {
                    parseCallFunction();
                }
                else if (peek().Value.m_Type == TokenType._asm && peek(1).HasValue && peek(1).Value.m_Type == TokenType.open_paren)
                {
                    parseAsm();
                }
                else
                {
                    errorStmtNotFound();
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
                }
                else
                {
                    errorStmtNotFound();
                    return;
                }
            }
            else if (m_section == Section.data) // BSS
            {
                if (try_consume(TokenType.colon))
                {
                    if (try_consume(TokenType._extern))
                    {
                        if (peek().Value.m_Type == TokenType.function)
                        {
                            DoNotWrite = false;
                            parseFunction();
                            addLine($".extern _{m_functions.Last().m_Name}", 0);
                            DoNotWrite = true;
                            return;
                        }
                    }

                    string name = try_consume_error(TokenType.ident).m_Value;
                    try_consume_error(TokenType._res);
                    string value = try_consume_error(TokenType.int_lit).m_Value;
                    addLine($"{name}:", 0, AsmSection.bss);
                    addLine($".res\t{value}", 0, AsmSection.bss);
                    m_bssDefines.Add(name, new BssEntry()
                    {
                        m_name = name,
                        m_size = Convert.ToInt32(value, 16),
                        m_offset = name
                    });
                }
                else
                {
                    errorStmtNotFound();
                    return;
                }
            }
            return;
        }

        void errorStmtNotFound()
        {

            Error_StmtNotFound(peek(-1).Value, m_lineNumbers.Last());
            ExitNow = true;

            // Environment.Exit(1);
        }
        bool ExitNow;
        public void Parse_Prog(Token[] tokens, string[] src)
        {
            ExitNow = false;
            addLine($".setcpu \"{m_CPUType}\"", 0);
            addLine($".section TEXT", 0);
            m_tokens = tokens;

            while (peek().HasValue)
            {
                parse_Stmt();

                if (ExitNow)
                {
                    break;
                }
            }

            addLine($".local __RODATASTRAT__", 0);
            addLine($"__RODATASTRAT__:", 0);
            m_Output.AddRange(m_OutputRodata);

            addLine($"{Environment.NewLine}.section BSS", 0);
            m_Output.AddRange(m_OutputBss);

            addLine($"{Environment.NewLine}.section DATA", 0);
            addLine($".local __DATASTRAT__", 0);
            addLine($"__DATASTRAT__:", 0);
            m_Output.AddRange(m_OutputData);

        }

        #region Token Functions
        Token? peek(int offset = 0)
        {
            if (m_index + offset >= m_tokens.Length)
                return null;
            return m_tokens[m_index + offset];
        }
        bool isThereRange(TokenType type, int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (peek(i).Value.m_Type == type)
                {
                    return true;
                }
            }
            return false;
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
            m_index++;
            Error_expected(peek(-1).Value, m_lineNumbers.Last(), type);
            return new Token();
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
                    addLine($"sub\tSP,\t{popCount * 4}");
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

        void declareVariabel(string name, TypeData type, string term = "", string register = "")
        {
            if (m_Scopes.Count == 0)
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
    }
}
public struct Function
{
    public string m_Name;
    public Argument[] m_Arguments;
    public int m_ArgumentSize;

    public override bool Equals(object obj)
    {
        if (obj is not string)
        {
            return false;
        }

        return (string)obj == m_Name;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
public struct Argument
{
    public string m_Name;
    public TypeData m_Type;
}