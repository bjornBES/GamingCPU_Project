using HLCLCompiler.Types;

namespace HLCLCompiler.CodeGeneration
{
    public class AsmGenState
    {
        public List<string> outputCode = new List<string>();
        public List<Function> functions = new List<Function>();
        public List<Variabel> variabels = new List<Variabel>();
        public int StackSize = 0;
        public Stack<int> scopes = new Stack<int>();
        protected Section currentSection;
        protected List<string> status = new List<string>();

        public void Text()
        {
            if (currentSection == Section.text)
            {
                return;
            }

            addLine(".section\tTEXT", 0);
            status.Add($"on line {outputCode.Count} switch to TEXT section");
            currentSection = Section.text;
        }
        public void DATA()
        {
            if (currentSection == Section.data)
            {
                return;
            }

            addLine(".section\tDATA", 0);
            status.Add($"on line {outputCode.Count} switch to DATA section");
            currentSection = Section.data;
        }
        public void MOV(string dst, string src)
        {
            addLine($"mov\t{dst},\t{src}");
            status.Add($"on line {outputCode.Count} MOV with {dst} and {src}");
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

        public void PUSH(Regs src)
        {
            TypeSize typeSize = RegsFunctions.GetSize(src);

            status.Add($"on line {outputCode.Count} PUSH {src} with type {typeSize}");

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
            addLine($"push\t{RegsFunctions.ToString(src)}");
        }

        public void POP(Regs src)
        {
            TypeSize typeSize = RegsFunctions.GetSize(src);

            status.Add($"on line {outputCode.Count} POP {src} with type {typeSize}");

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
            addLine($"pop\t{RegsFunctions.ToString(src)}");
        }

        public void CALL(string name)
        {
            addLine($"call\t[_{name}]");
        }

        public void RET()
        {
            Function function = functions.Last();
            if (function.arguments.Length == 0)
            {
                addLine("retz");
                status.Add($"on line {outputCode.Count} RET zero from {function.name}");
            }
            else
            {
                addLine($"ret\t0x{Utils.ToHex(function.ArgumentSize, 2)}");
                status.Add($"on line {outputCode.Count} RET 0x{Utils.ToHex(function.ArgumentSize, 2)} from {function.name}");
                StackSize -= function.ArgumentSize;
            }
        }

        public void SEZ(string dst)
        {
            addLine($"sez\t{dst}");
            status.Add($"on line {outputCode.Count} SEZ with {dst}");
        }
        public void SEZ(Regs dst)
        {
            SEZ(RegsFunctions.ToString(dst));
        }

        public void LDA(string dst, string src)
        {
            addLine($"lda\t{dst},\t{src}");
            status.Add($"on line {outputCode.Count} LDA with {dst} and {src}");
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
            addLine($"add\t{dst},\t{src}");
            status.Add($"on line {outputCode.Count} ADD {dst} and {src}");
        }
        public void ADD(Regs dst, string src)
        {
            ADD(RegsFunctions.ToString(dst), src);
        }
        public void ADD(Regs dst, int src)
        {
            ADD(RegsFunctions.ToString(dst), $"0x{Utils.ToHex(src, 4)}");
        }

        public void SUB(string dst, string src)
        {
            addLine($"sub\t{dst},\t{src}");
            status.Add($"on line {outputCode.Count} SUB {dst} and {src}");
        }
        public void SUB(Regs dst, string src)
        {
            SUB(RegsFunctions.ToString(dst), src);
        }
        public void SUB(Regs dst, int src)
        {
            SUB(RegsFunctions.ToString(dst), $"0x{Utils.ToHex(src, 4)}");
        }

        public void Pushr()
        {
            status.Add($"on line {outputCode.Count} PUSHR");
            StackSize += 5 * 4;         // AX BX CX DX HL
            addLine("pushr");
        }

        public void Popr()
        {
            status.Add($"on line {outputCode.Count} POPR");
            StackSize -= 5 * 4;         // AX BX CX DX HL
            addLine("popr");
        }
        
        public void Enter()
        {
            status.Add($"on line {outputCode.Count} ENTER into {functions.Last().name}");
            StackSize += 2;
            addLine("enter");
        }
        
        public void Leave()
        {
            status.Add($"on line {outputCode.Count} LEAVE from {functions.Last().name}");
            StackSize -= 2;
            addLine("leave");
        }

        public void Global(string label) => addLine($".global {label}", 0);

        public void FuncStart()
        {
            string name = functions.Last().FunctionLabel;

            addLine($"{name}:", 0);
            Global(name);
        }
        public void FuncEnd()
        {
            string name = functions.Last().ReturnLabel;

            addLine($"{name}:", 0);
            leaveScope();
            Popr();
            POP(Regs.AX);
            Leave();
            RET();
        }

        public void enterScope()
        {
            status.Add($"on line {outputCode.Count} entering the scope of {functions.Last().name}");

            scopes.Push(variabels.Count);
        }
        public void leaveScope()
        {
            status.Add($"on line {outputCode.Count} leaving the scope of {functions.Last().name}");

            int popCount = variabels.Count - scopes.Peek();
            StackSize -= popCount * 4;
            SUB(Regs.SP, popCount * 4);
            for (int i = 0; i < popCount; i++)
            {
                variabels.Remove(variabels.Last());
            }
            scopes.Pop();
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

        protected void addLine(string line, int taps = 1)
        {
            string[] splited = line.Split('\t');

            splited[0] = splited[0].PadRight(10, ' ');
            line = splited[0];

            for (int i = 1; i < splited.Length; i++)
            {
                line += splited[i].PadRight(30, ' ');
            }

            line = line.TrimEnd();

            outputCode.Add("".PadLeft(taps, '\t') + line);
            status.Add($"on line {outputCode.Count} stackSize is {StackSize}");
        }
    }
    public class AddressHelper
    {
        public static Address GetStack(int StackPlacement, string stackRegister)
        {
            Address address = new Address();

            address.IsStack = true;
            address.StackRegister = stackRegister;
            address.StackPlacement = StackPlacement;

            return address;
        }
    }
}
