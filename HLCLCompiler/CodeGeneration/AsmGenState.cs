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

        public void Text()
        {
            if (currentSection == Section.text)
            {
                return;
            }

            addLine(".section\tTEXT", 0);
            currentSection = Section.text;
        }
        public void DATA()
        {
            if (currentSection == Section.data)
            {
                return;
            }

            addLine(".section\tDATA", 0);
            currentSection = Section.data;
        }
        public void MOV(string dst, string src)
        {
            addLine($"mov\t{dst},\t{src}");
        }
        public void MOV(Regs dst, string src)
        {
            MOV(RegsFunctions.ToString(dst), src);
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

        public void PUSH(Regs src)
        {
            TypeSize typeSize = RegsFunctions.GetSize(src);

            addLine($"push\t{RegsFunctions.ToString(src)}");

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
        }

        public void RET()
        {
            Function function = functions.Last();
            if (function.arguments.Length == 0)
            {
                addLine("retz");
            }
            else
            {
                addLine($"ret\t0x{Utils.ToHex(function.ArgumentSize, 2)}");
            }
        }
        
        public void ADD(string dst, string src)
        {
            addLine($"add\t{dst},\t{src}");
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
            StackSize += 5 * 4;         // AX BX CX DX HL
            addLine("pushr");
        }

        public void Popr()
        {
            StackSize -= 5 * 4;         // AX BX CX DX HL
            addLine("popr");
        }
        
        public void Enter()
        {
            StackSize += 2;
            addLine("enter");
        }
        
        public void Leave()
        {
            StackSize -= 2;
            addLine("leave");
        }

        public void Global(string label) => addLine($".global {label}", 0);

        public void FuncStart()
        {
            string name = functions.Last().FunctionLabel;

            addLine($"{name}:", 0);
            Global(name);
            enterScope();
            Enter();
            ExpandStack(4);
            addLine("; return value is at [BP]");
            Pushr();
        }
        public void FuncEnd()
        {
            string name = functions.Last().ReturnLabel;

            addLine($"{name}:", 0);
            leaveScope();
            Popr();
            MOV(Regs.AX, Regs.BP, true);
            ShrinkStack(4);
            Leave();
            RET();
        }

        public void enterScope()
        {
            scopes.Push(StackSize);
        }
        public void leaveScope()
        {
            int popCount = variabels.Count - scopes.Peek();
            if (popCount != 0)
            {
                SUB(Regs.SP, popCount);
            }
            StackSize -= popCount;
            variabels.RemoveRange(variabels.Count - popCount, popCount);
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

        private void addLine(string line, int taps = 1)
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
