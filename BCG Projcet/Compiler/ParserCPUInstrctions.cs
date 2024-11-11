namespace Compiler
{
    public class ParserCPUInstrctions : ParserVariabels
    {
        public void push(Regs regs)
        {
            Size size = GetRegsSize(regs);
            addLine($"push\t{RegsToString(regs)}");

            switch (size)
            {
                case Size._byte:
                    m_StackSize++;
                    break;
                case Size._short:
                    m_StackSize += 2;
                    break;
                case Size._tbyte:
                    m_StackSize += 3;
                    break;
                case Size._int:
                    m_StackSize += 4;
                    break;
                default:
                    break;
            }
        }
        public void push16(string reg16)
        {
            addLine($"push\t{reg16}");
            m_StackSize += 2;
        }
        public void push32(string reg32)
        {
            addLine($"push\t{reg32}");
            m_StackSize += 4;
        }
        public void pushr()
        {
            addLine($"pushr");
            m_StackSize += 5 * 4;
        }

        public void pop16(string reg16)
        {
            addLine($"pop\t{reg16}");
            m_StackSize -= 2;
        }
        public void pop32(string reg32)
        {
            addLine($"pop\t{reg32}");
            m_StackSize -= 4;
        }
        public void popr()
        {
            addLine($"popr");
            m_StackSize -= 5 * 4;
        }
        public void addLine(string line, int taps = 1, AsmSection section = AsmSection.text)
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
}