
using static CompilerSettings;

public class GeneratorInstrctions : GeneratorVariabels
{
    protected void AddLine(string line, Section section = Section.text)
    {
        string[] segment = line.Split('\t');

        string result = "";

        for (int i = 0; i < segment.Length; i++)
        {
            result += segment[i].PadRight(result.Length + 4, ' ');
        }
        if (section == Section.text)
        {
            output.Add(result);
        }
        else
        {
            Console.WriteLine($"ERROR here {result} is not in TEXT");
        }
    }

    public void Comment(string msg)
    {
        AddLine($"; {msg}");
    }

    public void MOV(string dst, string src)
    {
        AddLine($"\tmov\t{dst},\t{src}");
        // status.Add($"on line {outputCode.Count} MOV with {dst} and {src}");
    }
    public void MOV(Regs dst, string src)
    {
        MOV(RegsFunctions.ToString(dst), src);
    }
    public void MOV(string dst, Regs src)
    {
        MOV(dst, RegsFunctions.ToString(src));
    }
    public void MOV(Regs dst, int src)
    {
        MOV(RegsFunctions.ToString(dst), $"0x{Utils.ToHex(src, 4)}");
    }
    public void MOV(Regs dst, Regs src)
    {
        MOV(RegsFunctions.ToString(dst), $"{RegsFunctions.ToString(src)}");
    }
    public void MOV(Regs dst, Regs src, bool useAsAddress)
    {
        if (useAsAddress)
        {
            MOV(RegsFunctions.ToString(dst), $"[{RegsFunctions.ToString(src)}]");
        }
        else
        {
            MOV(RegsFunctions.ToString(dst), $"{RegsFunctions.ToString(src)}");
        }
    }
    public void MOV(Regs dst, bool useAsAddress, Regs src)
    {
        if (useAsAddress)
        {
            MOV($"[{RegsFunctions.ToString(dst)}]", $"{RegsFunctions.ToString(src)}");
        }
        else
        {
            MOV(RegsFunctions.ToString(dst), $"{RegsFunctions.ToString(src)}");
        }
    }

    public void CMP(string dst, string src)
    {
        AddLine($"\tcmp\t{dst},\t{src}");
        // status.Add($"on line {outputCode.Count} MOV with {dst} and {src}");
    }
    public void CMP(Regs dst, string src)
    {
        CMP(RegsFunctions.ToString(dst), src);
    }
    public void CMP(string dst, Regs src)
    {
        CMP(dst, RegsFunctions.ToString(src));
    }
    public void CMP(Regs dst, int src)
    {
        CMP(RegsFunctions.ToString(dst), $"0x{Utils.ToHex(src, 4)}");
    }
    public void CMP(Regs dst, Regs src)
    {
        CMP(RegsFunctions.ToString(dst), $"{RegsFunctions.ToString(src)}");
    }

    public void PUSH(Regs src)
    {
        TypeSize typeSize = RegsFunctions.GetSize(src);

        // status.Add($"on line {outputCode.Count} PUSH {src} with type {typeSize}");

        switch (typeSize)
        {
            case TypeSize.CHAR:
                StackSize += 1;
                break;
            case TypeSize.SHORT:
            case TypeSize.SHORT_POINTER:
                StackSize += 2;
                break;
            case TypeSize.INT:
            case TypeSize.FLOAT:
            case TypeSize.FAR_POINTER:
                StackSize += 4;
                break;
            default:
                break;
        }
        AddLine($"\tpush\t{RegsFunctions.ToString(src)}");
    }

    public void POP(Regs src)
    {
        TypeSize typeSize = RegsFunctions.GetSize(src);

        // status.Add($"on line {outputCode.Count} POP {src} with type {typeSize}");

        switch (typeSize)
        {
            case TypeSize.CHAR:
                StackSize -= 1;
                break;
            case TypeSize.SHORT:
            case TypeSize.SHORT_POINTER:
                StackSize -= 2;
                break;
            case TypeSize.INT:
            case TypeSize.FLOAT:
            case TypeSize.FAR_POINTER:
                StackSize -= 4;
                break;
            default:
                break;
        }
        AddLine($"\tpop\t{RegsFunctions.ToString(src)}");
    }

    public void CALL(string name)
    {
        AddLine($"\tcall\t[_{name}]");
    }

    public void RET()
    {
        Function function = functions.Last();
        if (function.ArgumentSize == 0)
        {
            AddLine("\tretz");
            // status.Add($"on line {outputCode.Count} RET zero from {function.name}");
        }
        else
        {
            AddLine($"\tret\t0x{Utils.ToHex(function.ArgumentSize, 2)}");
            // status.Add($"on line {outputCode.Count} RET 0x{Utils.ToHex(function.ArgumentSize, 2)} from {function.name}");
            StackSize -= function.ArgumentSize;
        }
    }

    public void SEZ(string dst)
    {
        AddLine($"\tsez\t{dst}");
        // status.Add($"on line {outputCode.Count} SEZ with {dst}");
    }
    public void SEZ(Regs dst)
    {
        SEZ(RegsFunctions.ToString(dst));
    }

    public void LDA(string dst, string src)
    {
        AddLine($"\tlda\t{dst},\t{src}");
        // status.Add($"on line {outputCode.Count} LDA with {dst} and {src}");
    }
    public void LDA(Regs dst, string src)
    {
        LDA(RegsFunctions.ToString(dst), src);
    }
    public void LDA(string dst, Regs src)
    {
        LDA(dst, RegsFunctions.ToString(src));
    }
    public void LDA(Regs dst, int src)
    {
        LDA(RegsFunctions.ToString(dst), $"0x{Utils.ToHex(src, 4)}");
    }
    public void LDA(Regs dst, Regs src, bool useAsAddress)
    {
        if (useAsAddress)
        {
            LDA(RegsFunctions.ToString(dst), $"[{RegsFunctions.ToString(src)}]");
        }
        else
        {
            LDA(RegsFunctions.ToString(dst), $"{RegsFunctions.ToString(src)}");
        }
    }

    public void ADD(string dst, string src)
    {
        AddLine($"\tadd\t{dst},\t{src}");
        // status.Add($"on line {outputCode.Count} ADD {dst} and {src}");
    }
    public void ADD(Regs dst, string src)
    {
        ADD(RegsFunctions.ToString(dst), src);
    }
    public void ADD(Regs dst, int src)
    {
        ADD(RegsFunctions.ToString(dst), $"0x{Utils.ToHex(src, 4)}");
    }
    public void ADD(Regs dst, Regs src)
    {
        ADD(RegsFunctions.ToString(dst), RegsFunctions.ToString(src));
    }

    public void SUB(string dst, string src)
    {
        AddLine($"\tsub\t{dst},\t{src}");
        // status.Add($"on line {outputCode.Count} SUB {dst} and {src}");
    }
    public void SUB(Regs dst, string src)
    {
        SUB(RegsFunctions.ToString(dst), src);
    }
    public void SUB(Regs dst, int src)
    {
        SUB(RegsFunctions.ToString(dst), $"0x{Utils.ToHex(src, 4)}");
    }

    public void JMP(string label)
    {
        AddLine($"\tJMP\t[{label}]");
    }

    public void Pushr()
    {
        // status.Add($"on line {outputCode.Count} PUSHR");
        if (InProtectedMode)
        {
            StackSize += 5 * 4;         // AX BX CX DX HL
        }
        else
        {
            StackSize += 6 * 2;         // A B C D H L
        }
        AddLine("\tpushr");
    }

    public void Enter()
    {
        // status.Add($"on line {outputCode.Count} ENTER into {functions.Last().name}");
        StackSize += 2;
        AddLine("\tenter");
    }

    public void Leave()
    {
        // status.Add($"on line {outputCode.Count} LEAVE from {functions.Last().name}");
        StackSize -= 2;
        AddLine("\tleave");
    }

    public void Popr()
    {
        // status.Add($"on line {outputCode.Count} POPR");
        if (InProtectedMode)
        {
            StackSize -= 5 * 4;         // AX BX CX DX HL
        }
        else
        {
            StackSize -= 6 * 2;         // A B C D H L
        }
        AddLine("\tpopr");
    }

    protected void SetSection(Section section)
    {
        switch (section)
        {
            case Section.text:
                Text();
                break;
            case Section.data:
                Data();
                break;
        }
    }

    protected void Global(string name) => AddLine($".global {name}", Section.text);

    protected void FuncStart()
    {
        string name = functions.Last().FunctionLabelStart;

        Global(name);
        AddLine($"{name}:");
    }
    protected void FuncEnd()
    {
        string name = functions.Last().FunctionLabelEnd;

        AddLine($"{name}:");
        leaveScopeWithLabel();
        Popr();
        Leave();
        RET();
    }

    public void enterScope()
    {
        // status.Add($"on line {outputCode.Count} entering the scope of {functions.Last().name}");
        ScopeLables.Push($"Scope{ScopeCount}");
        AddLine($"Scope{ScopeCount}:");
        ScopeCount++;
        scopes.Push(variabels.Count);
    }
    public void leaveScopeWithLabel()
    {
        // status.Add($"on line {outputCode.Count} leaving the scope of {functions.Last().name}");
        AddLine($"{ScopeLables.Pop()}_END:");
        leaveScope();
    }
    public void leaveScope()
    {
        // status.Add($"on line {outputCode.Count} leaving the scope of {functions.Last().name}");
        int popCount = variabels.Count - scopes.Peek();
        StackSize -= popCount * VariabelSize;
        if (popCount > 0)
        {
            SUB(Regs.SP, popCount * VariabelSize);
        }
        for (int i = 0; i < popCount; i++)
        {
            variabels.Remove(variabels.Last());
        }
        scopes.Pop();
    }

    protected void Text()
    {
        if (currentSection == Section.text)
        {
            return;
        }

        AddLine($".section {TextSectionName.ToUpper()}");
        currentSection = Section.text;
    }
    protected void Data()
    {
        if (currentSection == Section.data)
        {
            return;
        }

        AddLine($".section {DataSectionName.ToUpper()}");
        currentSection = Section.data;
    }

    public void ExpandStack(int nBytes)
    {
        ADD(Regs.SP, nBytes);
        StackSize += nBytes;
    }
    public void ShrinkStack(int nBytes)
    {
        SUB(Regs.SP, nBytes);
        StackSize -= nBytes;
    }
    public string CreateLabel()
    {
        return $"Label{LableCount++}";
    }
    public void ConJUMP(CompersionOperators compersionOperators, string label)
    {
        switch (compersionOperators)
        {
            case CompersionOperators.eq:
                AddLine($"\tjne\t[{label}]");
                break;
            case CompersionOperators.neq:
                AddLine($"\tje\t[{label}]");
                break;
            case CompersionOperators.lt:
                AddLine($"\tjg\t[{label}]");
                break;
            case CompersionOperators.gt:
                AddLine($"\tjl\t[{label}]");
                break;
            case CompersionOperators.leq:
                AddLine($"\tjne\t[{label}]");
                AddLine($"\tjg\t[{label}]");
                break;
            case CompersionOperators.geq:
                AddLine($"\tjne\t[{label}]");
                AddLine($"\tjl\t[{label}]");
                break;
        }
    }
}