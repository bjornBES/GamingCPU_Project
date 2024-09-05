using CommonBCGCPU;
using CommonBCGCPU.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Text;

namespace BCG16CPUEmulator
{
    public class BCG16CPU : BCG16CPU_Instructions
    {
        Stack<IPort> PortsIRQ = new Stack<IPort>(256);
        bool IRQEnable;


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
            IRQEnable = true;
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

            IDTAddressRegister = 0;

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

            if (GetFlag(FI) == true)
            {
                if (PortsIRQ.TryPop(out IPort result) == true)
                {
                    ExecutionInterrupt(result);
                }
            }

            ExecuteInstruction(Instructions[0], ArgumentModes1[0], ArgumentModes2[0], ref stage1);
            ExecuteInstruction(Instructions[1], ArgumentModes1[1], ArgumentModes2[1], ref stage2);
            ExecuteInstruction(Instructions[2], ArgumentModes1[2], ArgumentModes2[2], ref stage3);

            TickOpertion(0);
            TickOpertion(1);
            TickOpertion(2);
        }

        void ExecutionInterrupt(IPort port)
        {
            PushInterrupt();
        }

        void TickOpertion(int Opertion)
        {
            if (GetFlag(FH) == true)
            {
                return;
            }
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

            argumentOffset = 3;

            switch (instruction)
            {
                case Instruction.MOV:
                    Move(Size._byte, argument1, argument2);
                    return;
                case Instruction.MOVW:
                    Move(Size._word, argument1, argument2);
                    return;
                case Instruction.MOVT:
                    Move(Size._tbyte, argument1, argument2);
                    return;
                case Instruction.MOVD:
                    Move(Size._dword, argument1, argument2);
                    return;
                case Instruction.CMP:
                    Cmp(argument1, argument2);
                    return;
                case Instruction.PUSH:
                    Push(argument1);
                    return;
                case Instruction.POP:
                    Pop(Size._byte, argument1);
                    return;
                case Instruction.POPW:
                    Pop(Size._word, argument1);
                    return;
                case Instruction.POPT:
                    Pop(Size._tbyte, argument1);
                    return;
                case Instruction.POPD:
                    Pop(Size._dword, argument1);
                    return;
                case Instruction.CALL:
                    Call(argument1);
                    return;
                case Instruction.RET:
                    Ret(argument1);
                    return;
                case Instruction.RETZ:
                    Ret(0);
                    return;
                case Instruction.SEZ:
                    Sez(argument1);
                    return;
                case Instruction.TEST:
                    break;
                case Instruction.JMP:
                    Jump(argument1);
                    return;
                case Instruction.JZ:
                    JumpC(argument1, FZ, 1);
                    return;
                case Instruction.JNZ:
                    JumpC(argument1, FZ, 0);
                    return;
                case Instruction.JS:
                    JumpC(argument1, FS, 1);
                    return;
                case Instruction.JNS:
                    JumpC(argument1, FS, 0);
                    return;
                case Instruction.JE:
                    JumpC(argument1, FE, 1);
                    return;
                case Instruction.JNE:
                    JumpC(argument1, FE, 0);
                    return;
                case Instruction.JL:
                    JumpC(argument1, FL, 1);
                    return;
                case Instruction.JG:
                    JumpC(argument1, FL, 0);
                    return;
                case Instruction.JLE:
                    if (GetFlag(FL) == true || GetFlag(FE) == true)
                    {
                        Jump(argument1);
                    }
                    return;
                case Instruction.JGE:
                    if (GetFlag(FL) == false || GetFlag(FE) == true)
                    {
                        Jump(argument1);
                    }
                    return;
                case Instruction.JNV:
                    JumpC(argument1, FO, 0);
                    return;
                case Instruction.JTZ:
                    Jump((Address)0);
                    return;
                case Instruction.INB:
                    Inb(argument1, argument2);
                    return;
                case Instruction.OUTB:
                    Outb(argument1, argument2);
                    return;
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
                    Or(argument1, argument2);
                    return;
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
                    Inc(argument1);
                    return;
                case Instruction.DEC:
                    Dec(argument1);
                    return;
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
                    Rti();
                    return;
                case Instruction.RISERR:
                    break;
                case Instruction.PUSHR:
                    Pushr();
                    return;
                case Instruction.POPR:
                    Popr();
                    return;
                case Instruction.INT:
                    Int(argument1);
                    return;
                case Instruction.BRK:
                    break;
                case Instruction.HALT:
                    SetFlag(FH, true);
                    return;
                case Instruction.NOP:
                    return;
                default:
                    break;
            }

            Console.WriteLine($"{instruction} {argument1}, {argument2}");
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
