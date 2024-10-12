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

namespace Compiler
{
    public class Parser
    {
        Token[] m_tokens;
        int m_index = 0;
        public Stack<int> m_Scopes = new Stack<int>();
        List<Var> m_var = new List<Var>();
        List<Function> m_functions = new List<Function>();
        public int m_StackSize;
        List<string> m_lineNumber = new List<string>();
        int _DS;
        int m_RegisterDS
        {
            get { return _DS; }
            set
            {
                GetSetRegiserValue(ref _DS, value, "DS");
            }
        }
        private int _B;
        int m_registerB
        {
            get { return _B; }
            set
            {
                GetSetRegiserValue(ref _B, value, "B");
            }
        }

        void GetSetRegiserValue(ref int register, int value, string registerName)
        {
            if (register == value)
            {
                return;
            }
            else if (register == value - 1)
            {
                addLine($"inc\t{registerName}");
            }
            else if (register == value + 1)
            {
                addLine($"dec\t{registerName}");
            }
            else if (value == 0)
            {
                addLine($"sez\t{registerName}");
            }
            else
            {
                addLine($"mov\t{registerName},\t0x{Convert.ToString(value, 16).PadLeft(4, '0')}");
            }
            register = value;
        }

        Section m_section;

        public List<string> m_Output = new List<string>();
        public List<string> m_OutputBss = new List<string>();
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

                if (data <= 0xFFFF)
                {
                    if (type != null)
                    {
                        switch ((Size)type.m_TypeSize)
                        {
                            case Size._byte:
                            case Size._short:
                                register = "A";
                                return term;
                            case Size._tbyte:
                            case Size._int:
                                register = "HL";
                                return term;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        register = "A";
                        return term;
                    }
                }
                else if (data <= 0xFFFFFFFF)
                {
                    register = "HL";
                    return term;
                }
            }
            else if (peek().Value.m_Type == TokenType.ident)
            {
                string name = consume().m_Value;
                if (isVariabel(name, out Var var))
                {
                    if (var.m_Address.m_UseDSB)
                    {
                        m_registerB = var.m_Address.m_B;
                        m_RegisterDS = var.m_Address.m_DS;
                    }
                    string address = $"{var.m_Address.GetAddress(this)}";
                    switch ((Size)var.m_TypeData.m_TypeSize)
                    {
                        case Size._byte:
                            register = "AL";
                            return $"{address}";
                        case Size._short:
                            register = "A";
                            return $"{address}";
                        case Size._int:
                            register = "HL";
                            return $"{address}";
                        default:
                            break;
                    }
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

            if (result.m_IsPointer)
            {
                try_consume_error(TokenType.star);
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
            byte BPoffset = 4;                      // 4 from the return address
            if (m_CPUType < CPUType.BC32)
            {
                push16("BP");
                addLine($"mov\tBP,\tSP");
                BPoffset += 2;
            }
            else
            {
                push32("BPX");
                addLine($"mov\tBPX,\tSPX");
                BPoffset += 4;
            }
            pushr();
            BPoffset += 20;                         // AX(4) + BX(4) + CX(4) + DX(4) + H(2) + L(2)
            int sizeOfArgument = 0;
            while (!try_consume(TokenType.close_paren))
            {
                TypeData type = parseType();
                string argName = try_consume_error(TokenType.ident).m_Value;

                m_var.Add(new Var(argName, type, BPoffset, true));

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
            m_RegisterDS = 0xB;
            if (m_registerB != 0)
            {
                m_registerB = 0;
            }
            while (peek().HasValue &&
                peek().Value.m_Type != TokenType.end && peek(1).HasValue && peek().Value.m_Type != TokenType.function)
            {
                parse_Stmt();
            }
            try_consume_error(TokenType.end);
            try_consume_error(TokenType.function);
            try_consume_error(TokenType.period);
            addLine($"mov\tR1,\t0");
            popr();
            if (m_CPUType < CPUType.BC32)
            {
                pop16("BP");
            }
            else
            {
                pop32("BPX");
            }
            addLine($"Exit_{name}:", 0);
            end_scope();
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

            addLine($"mov\t{register},\t{term}");
            addLine($"mov\tR1,\t{register}");
            pop16("BP");
            popr();
            addLine($"mov\tA,\tR1");
            addLine($"jmp\t[Exit_{m_functions.Last().m_Name}]");
            try_consume_error(TokenType.period);
        }
        void parseReassign()
        {
            string name = try_consume_error(TokenType.ident).m_Value;

            if (isVariabel(name, out Var var))
            {
                try_consume_error(TokenType.eq);
                string term = parseTerm(out string register, var.m_TypeData);

                if (var.m_Address.m_UseDSB)
                {
                    m_registerB = var.m_Address.m_B;
                    m_RegisterDS = var.m_Address.m_DS;
                }

                addLine($"mov\t{register},\t{term}");
                addLine($"mov\t{var.m_Address.GetAddress(this)},\t{register}");
                addLine($"; _{name} = {term}");

                try_consume_error(TokenType.period);
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
                    return;
                }

                if (peek().Value.m_Type == TokenType.program && peek(1).HasValue && peek(1).Value.m_Type == TokenType.ident)
                {
                    parseProgram();
                }
                else if (peek().Value.m_Type == TokenType.function && peek(1).HasValue && peek(1).Value.m_Type == TokenType.ident)
                {
                    parseFunction();
                }
                else if (peek().Value.m_Type == TokenType._return)
                {
                    parseReturn();
                }
                else if (isType())
                {
                    TypeData type = parseType();
                    if (peek().HasValue && peek().Value.m_Type == TokenType.ident)
                    {
                        parseVariabel(type);
                    }
                }
                else if (peek().Value.m_Type == TokenType.ident && peek(1).HasValue && peek(1).Value.m_Type == TokenType.eq)
                {
                    parseReassign();
                }
            }
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
            Error_expected(peek(-1).Value, type);
            throw new NotImplementedException("yes do it you lazy ass implement it now");
        }
        #endregion

        void begin_scope()
        {
            m_Scopes.Push(m_var.Count);
        }
        void end_scope()
        {
            int popCount = m_var.Count - m_Scopes.Peek();

            if (popCount != 0)
            {
                addLine($"add\tSP,\t{popCount * 2}");
            }

            m_StackSize -= popCount;

            for (int i = m_Scopes.First(); i < m_var.Count; i++)
            {
                m_var.RemoveAt(i);
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
            m_StackSize += 24;
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
            m_StackSize -= 24;
        }

        void declareVariabel(string name, TypeData type, string term = "", string register = "")
        {
            if (m_Scopes.Count == 0)
            {
                addLine($"_{name}", section: AsmSection.bss);
                addLine($".res {type.m_TypeSize}", section: AsmSection.bss);
                if (term == "")
                {
                    if (term != "A")
                    {
                        addLine($"mov\t{register},\t{term}");
                    }
                    addLine($"mov\t[_{name}],\t{register}");
                }
                m_var.Add(new Var());
            }
            else if (m_Scopes.Count == 1)
            {
                m_RegisterDS = 0xB;
                if (m_registerB != 0)
                {
                    m_registerB = 0;
                }

                addLine($"; _{name} is at DS:{m_registerB}", section: AsmSection.text);
                m_var.Add(new Var(name, type, m_RegisterDS, m_registerB));
                m_registerB += type.m_TypeSize;
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
                /*
                if (term != "A")
                {
                    addLine($"mov\tA,\t{term}");
                }
                else if (term != "HL")
                {
                    addLine($"mov\tHL,\t{term}");
                }
                 */

                m_var.Add(new Var(name, type, m_StackSize));

                addLine($"mov\t{register},\t{term}");

                if (register == "A")
                {
                    //addLine($"mov\tA,\t{term}");
                    push16("A");
                }
                else if (register == "HL")
                {
                    //addLine($"mov\tHL,\t{term}");
                    push16("H");
                    push16("L");
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
                case AsmSection.rdata:
                    //m_output_rdata.Add(line);
                    break;
                case AsmSection.data:
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
        rdata,
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
    public Var(string name, TypeData typeData, int DS, int B)
    {
        m_Name = name;
        m_TypeData = typeData;
        m_Address.m_UseDSB = true;
        m_Address.m_DS = DS;
        m_Address.m_B = B;
    }
}
public struct Address
{
    public bool m_UseStack;
    public bool m_IsArg;
    public int m_StackLoc;

    public bool m_UseDSB;
    public int m_DS;
    public int m_B;

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
        else if (m_UseDSB)
        {
            return "[DS:B]";
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
enum Section
    {
        bss,
        rdata,
        data,
        text,
        loacl,
    }
