using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Text;

namespace BCG16CPUEmulator
{
    public class BCG16CPU : BCG16CPU_Instructions
    {
        Stack<IPort> PortsIRQ = new Stack<IPort>(256);
        bool IRQEnable = false;


        Instruction[] Instructions = new Instruction[3];
        ArgumentMode[] ArgumentModes1 = new ArgumentMode[3];
        ArgumentMode[] ArgumentModes2 = new ArgumentMode[3];

        public void IRQ(IPort Port)
        {
            PortsIRQ.Push(Port);
            IRQEnable = true;
        }

        public void ConnectBus(BUS bus)
        {
            m_BUS = bus;

            ResetCPU();
        }

        public void ResetCPU()
        {
            Instructions.Initialize();
            ArgumentModes1.Initialize();
            ArgumentModes2.Initialize();

            A = B = C = D = 0;
            H = L = 0;

            HL[true] = H;
            HL[false] = L;

            DS = 0;
            ES = 0;
            SS = 0;
            S = 0;

            // stating point is 0x00_0200
            PC = 0x200;
            PCMB = 0x0;

            BP = SP = 0;

            IL = 0;

            R1 = R2 = 0;

            MB = 0;

            CR0 = CR1 = 0;

            F = 0;
        }

        uint stage1 = 0;
        uint stage2 = uint.MaxValue;
        uint stage3 = uint.MaxValue - 1;

        ArgumentMode argument1;
        ArgumentMode argument2;
        public void Tick()
        {
            ABX[true] = A;
            ABX[false] = B;

            CDX[true] = C;
            CDX[false] = D;

            HL[true] = H;
            HL[false] = L;

            while (PortsIRQ.TryPop(out IPort result) == true)
            {

            }

            ExecuteInstruction(Instructions[0], ArgumentModes1[0], ArgumentModes2[0], ref stage1);
            ExecuteInstruction(Instructions[1], ArgumentModes1[1], ArgumentModes2[1], ref stage2);
            ExecuteInstruction(Instructions[2], ArgumentModes1[2], ArgumentModes2[2], ref stage3);

            TickOpertion(0);
            TickOpertion(1);
            TickOpertion(2);
        }

        void TickOpertion(int Opertion)
        {
            switch (Opertion)
            {
                case 0:
                    TickStage(ref stage1,0);
                    break;
                case 1:
                    TickStage(ref stage2,1);
                    break;
                case 2:
                    TickStage(ref stage3,2);
                    break;
                default:
                    break;
            }
        }

        void TickStage(ref uint stage, int Opertion)
        {
            switch (stage)
            {
                case 0:
                    break;
                case 1:
                    IR = DecodeInstruction(out argument1, out argument2);
                    Instructions[Opertion] = IR;
                    ArgumentModes1[Opertion] = argument1;
                    ArgumentModes2[Opertion] = argument2;
                    break;
                case 2:
                    stage = 0;
                    return;
            }
            stage++;
        }

        private void ExecuteInstruction(Instruction instruction, ArgumentMode argument1, ArgumentMode argument2, ref uint stage)
        {
            if (stage != 2)
            {
                return;
            }

            switch (instruction)
            {
                case Instruction.MOV:
                    Move(Size._byte, argument1, argument2);
                    break;
                case Instruction.MOVW:
                    Move(Size._word, argument1, argument2);
                    break;
                case Instruction.MOVT:
                    Move(Size._tbyte, argument1, argument2);
                    break;
                case Instruction.MOVD:
                    Move(Size._dword, argument1, argument2);
                    break;
                case Instruction.CMP:
                    break;
                case Instruction.PUSH:
                    Push(argument1);
                    break;
                case Instruction.POP:
                    break;
                case Instruction.POPW:
                    break;
                case Instruction.POPT:
                    break;
                case Instruction.POPD:
                    break;
                case Instruction.CALL:
                    break;
                case Instruction.RET:
                    break;
                case Instruction.RETZ:
                    break;
                case Instruction.SEZ:
                    Sez(argument1);
                    break;
                case Instruction.TEST:
                    break;
                case Instruction.JMP:
                    break;
                case Instruction.JZ:
                    break;
                case Instruction.JNZ:
                    break;
                case Instruction.JS:
                    break;
                case Instruction.JNS:
                    break;
                case Instruction.JE:
                    break;
                case Instruction.JNE:
                    break;
                case Instruction.JL:
                    break;
                case Instruction.JG:
                    break;
                case Instruction.JLE:
                    break;
                case Instruction.JGE:
                    break;
                case Instruction.JNV:
                    break;
                case Instruction.JTZ:
                    Jump(0);
                    break;
                case Instruction.JBA:
                    break;
                case Instruction.INB:
                    break;
                case Instruction.OUTB:
                    break;
                case Instruction.SEF:
                    break;
                case Instruction.CLF:
                    break;
                case Instruction.ADD:
                    break;
                case Instruction.SUB:
                    break;
                case Instruction.MUL:
                    break;
                case Instruction.DIV:
                    break;
                case Instruction.AND:
                    break;
                case Instruction.OR:
                    break;
                case Instruction.NOR:
                    break;
                case Instruction.XOR:
                    break;
                case Instruction.NOT:
                    break;
                case Instruction.SHL:
                    break;
                case Instruction.SHR:
                    break;
                case Instruction.ROL:
                    break;
                case Instruction.ROR:
                    break;
                case Instruction.INC:
                    break;
                case Instruction.DEC:
                    break;
                case Instruction.NEG:
                    break;
                case Instruction.AVG:
                    break;
                case Instruction.EXP:
                    break;
                case Instruction.SQRT:
                    break;
                case Instruction.RNG:
                    break;
                case Instruction.SEB:
                    break;
                case Instruction.CLB:
                    break;
                case Instruction.TOB:
                    break;
                case Instruction.MOD:
                    break;
                case Instruction.FADD:
                    break;
                case Instruction.FSUB:
                    break;
                case Instruction.FMUL:
                    break;
                case Instruction.FDIV:
                    break;
                case Instruction.FAND:
                    break;
                case Instruction.FOR:
                    break;
                case Instruction.FNOR:
                    break;
                case Instruction.FXOR:
                    break;
                case Instruction.FNOT:
                    break;
                case Instruction.MOVS:
                    break;
                case Instruction.CMPSTR:
                    break;
                case Instruction.MOVF:
                    break;
                case Instruction.DATE:
                    break;
                case Instruction.DELAY:
                    break;
                case Instruction.TIME:
                    break;
                case Instruction.RTI:
                    break;
                case Instruction.NOP:
                    break;
                case Instruction.RISERR:
                    break;
                case Instruction.PUSHR:
                    break;
                case Instruction.POPR:
                    break;
                case Instruction.INT:
                    break;
                case Instruction.BRK:
                    break;
                case Instruction.HALT:
                    break;
                default:
                    break;
            }
        }

        Instruction DecodeInstruction(out ArgumentMode argument1, out ArgumentMode argument2)
        {
            argument1 = ArgumentMode.None;
            argument2 = ArgumentMode.None;

            byte instrByte = FetchByte();
            byte arg1 = FetchByte();
            byte arg2 = FetchByte();

            if (Enum.TryParse(arg1.ToString(), true, out argument1)){}
            if (Enum.TryParse(arg2.ToString(), true, out argument2)){}

            if (Enum.TryParse(instrByte.ToString(), true, out Instruction result))
            {
                return result;
            }

            throw new NotImplementedException();
        }
    }
}
