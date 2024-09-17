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
        Stack<byte> PortsIRQ = new Stack<byte>(256);
        Stack<byte> PortsNMI = new Stack<byte>(256);

        const ushort GotInterrupt = 0x7F55;
        
        Instruction[] Instructions = new Instruction[3];
        ArgumentMode[] ArgumentModes1 = new ArgumentMode[3];
        ArgumentMode[] ArgumentModes2 = new ArgumentMode[3];

        public void IRQ(byte Port)
        {
            PortsIRQ.Push(Port);
        }
        public void NMI(byte Port)
        {
            PortsNMI.Push(Port);
        }

        public void ConnectBus(BUS bus)
        {
            m_BUS = bus;

            ResetCPU();
        }

        public void ResetCPU()
        {
            PortsNMI.Clear();
            PortsIRQ.Clear();

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
            HL[true] = H;
            HL[false] = L;

            if (GetFlag(FI) == true)
            {
                if (PortsIRQ.TryPop(out byte result) == true)
                {
                    ExecuteInstruction(Instructions[0], ArgumentModes1[0], ArgumentModes2[0], ref stage1);
                    ExecuteInstruction(Instructions[1], ArgumentModes1[1], ArgumentModes2[1], ref stage2);
                    ExecuteInstruction(Instructions[2], ArgumentModes1[2], ArgumentModes2[2], ref stage3);

                    ExecutionInterrupt(result);

                    TickOpertion(0);
                    TickOpertion(1);
                    TickOpertion(2);
                    return;
                }
            }
            if (PortsNMI.TryPop(out byte NMIresult) == true)
            {
                ExecutionInterrupt(NMIresult);
            }
            ExecuteInstruction(Instructions[0], ArgumentModes1[0], ArgumentModes2[0], ref stage1);
            ExecuteInstruction(Instructions[1], ArgumentModes1[1], ArgumentModes2[1], ref stage2);
            ExecuteInstruction(Instructions[2], ArgumentModes1[2], ArgumentModes2[2], ref stage3);

            TickOpertion(0);
            TickOpertion(1);
            TickOpertion(2);
        }

        void ExecutionInterrupt(byte port)
        {
            m_BUS.INTA(port);

            PushInterrupt();

            Address InterruptAddress = (port * 4) + 0x0064;
            Address InterruptFunctionAddress = m_BUS.m_Memory.ReadDWord(InterruptAddress, 0x11);

            PC = InterruptFunctionAddress;
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

        private void ExecuteInstruction(Instruction instruction, ArgumentMode Argument1, ArgumentMode Argument2, ref uint stage)
        {
            if (stage != 2)
            {
                return;
            }

            argumentOffset = 3;

            switch (instruction)
            {
                case Instruction.MOV:
                    Move(Size._byte, Argument1, Argument2);
                    return;
                case Instruction.MOVRAL:
                    Move(Size._byte, Register.AL, Argument1);
                    return;
                case Instruction.MOVW:
                    Move(Size._word, Argument1, Argument2);
                    return;
                case Instruction.MOVWRA:
                    Move(Size._word, Register.A, Argument1);
                    return;
                case Instruction.MOVT:
                    Move(Size._tbyte, Argument1, Argument2);
                    return;
                case Instruction.MOVD:
                    Move(Size._dword, Argument1, Argument2);
                    return;
                case Instruction.CMP:
                    Cmp(Argument1, Argument2);
                    return;
                case Instruction.PUSH:
                    Push(Size._byte, Argument1);
                    return;
                case Instruction.PUSHW:
                    Push(Size._word, Argument1);
                    break;
                case Instruction.PUSHT:
                    Push(Size._tbyte, Argument1);
                    break;
                case Instruction.PUSHD:
                    Push(Size._dword, Argument1);
                    break;
                case Instruction.POP:
                    Pop(Size._byte, Argument1);
                    return;
                case Instruction.POPW:
                    Pop(Size._word, Argument1);
                    return;
                case Instruction.POPT:
                    Pop(Size._tbyte, Argument1);
                    return;
                case Instruction.POPD:
                    Pop(Size._dword, Argument1);
                    return;
                case Instruction.CALL:
                    Call(Argument1);
                    return;
                case Instruction.RET:
                    Ret(Argument1);
                    return;
                case Instruction.RETZ:
                    Return(0);
                    return;
                case Instruction.SEZ:
                    Sez(Argument1);
                    return;
                case Instruction.SEZRAL:
                    SetRegisterValue(Register.AL, 0);
                    return;
                case Instruction.SEZRA:
                    SetRegisterValue(Register.A, 0);
                    return;
                case Instruction.TEST:
                    Test(argument1);
                    return;
                case Instruction.TESTRAL:
                    Compare(Register.AL, Register.AL);
                    return;
                case Instruction.TESTRA:
                    Compare(Register.A, Register.A);
                    return;
                case Instruction.JMP:
                    Jump(Argument1);
                    return;
                case Instruction.JZ:
                    JumpC(Argument1, FZ, 1);
                    return;
                case Instruction.JNZ:
                    JumpC(Argument1, FZ, 0);
                    return;
                case Instruction.JS:
                    JumpC(Argument1, FS, 1);
                    return;
                case Instruction.JNS:
                    JumpC(Argument1, FS, 0);
                    return;
                case Instruction.JE:
                    JumpC(Argument1, FE, 1);
                    return;
                case Instruction.JNE:
                    JumpC(Argument1, FE, 0);
                    return;
                case Instruction.JL:
                    JumpC(Argument1, FL, 1);
                    return;
                case Instruction.JG:
                    JumpC(Argument1, FL, 0);
                    return;
                case Instruction.JLE:
                    if (GetFlag(FL) == true || GetFlag(FE) == true)
                    {
                        Jump(Argument1);
                    }
                    return;
                case Instruction.JGE:
                    if (GetFlag(FL) == false || GetFlag(FE) == true)
                    {
                        Jump(Argument1);
                    }
                    return;
                case Instruction.JNV:
                    JumpC(Argument1, FO, 0);
                    return;
                case Instruction.JTZ:
                    Jump((Address)0);
                    return;

                case Instruction.INPB:
                    Inb(Size._byte, Argument1, Argument2);
                    return;
                case Instruction.INPW:
                    Inb(Size._word, Argument1, Argument2);
                    return;
                case Instruction.OUTB:
                    Outb(Size._byte, Argument1, Argument2);
                    return;
                case Instruction.OUTW:
                    Outb(Size._word, Argument1, Argument2);
                    return;

                case Instruction.SZE:
                    SetFlag(FZ, true);
                    return;
                case Instruction.SEE:
                    SetFlag(FE, true);
                    return;
                case Instruction.SES:
                    SetFlag(FS, true);
                    return;
                case Instruction.SEC:
                    SetFlag(FC, true);
                    return;
                case Instruction.SEL:
                    SetFlag(FL, true);
                    return;
                case Instruction.SEI:
                    SetFlag(FI, true);
                    return;
                case Instruction.SEH:
                    SetFlag(FH, true);
                    return;
                case Instruction.CZE:
                    SetFlag(FZ, false);
                    return;
                case Instruction.CLE:
                    SetFlag(FE, false);
                    return;
                case Instruction.CLS:
                    SetFlag(FS, false);
                    return;
                case Instruction.CLC:
                    SetFlag(FC, false);
                    return;
                case Instruction.CLL:
                    SetFlag(FL, false);
                    return;
                case Instruction.CLI:
                    SetFlag(FI, false);
                    return;
                case Instruction.CLH:
                    SetFlag(FH, false);
                    return;

                case Instruction.ADD:
                    Add(argument1, argument2);
                    return;
                case Instruction.SUB:
                    break;
                case Instruction.MUL:
                    break;
                case Instruction.DIV:
                    break;
                case Instruction.AND:
                    break;
                case Instruction.OR:
                    Or(Argument1, Argument2);
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
                    Inc(Argument1);
                    return;
                case Instruction.DEC:
                    Dec(Argument1);
                    return;
                case Instruction.NEG:
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
                case Instruction.CMPSTR:
                    break;
                case Instruction.MOVF:
                    break;
                case Instruction.RETI:
                    Rti();
                    return;
                case Instruction.PUSHR:
                    Pushr();
                    return;
                case Instruction.POPR:
                    Popr();
                    return;
                case Instruction.INT:
                    Int(Argument1);
                    return;
                case Instruction.BRK:
                    break;
                case Instruction.HALT:
                    SetFlag(FH, true);
                    return;
                case Instruction.NOP:
                    return;
                case Instruction.CBTA:
                    break;
                default:
                    break;
            }

            Console.WriteLine($"{instruction} {Argument1}, {Argument2}");
        }

        Instruction DecodeInstruction(out ArgumentMode argument1, out ArgumentMode argument2)
        {
            argument1 = ArgumentMode.None;
            argument2 = ArgumentMode.None;

            byte arg1 = 0xFF;
            byte arg2 = 0xFF;

            byte instrByte = FetchByte();
            if (Enum.TryParse(instrByte.ToString(), true, out Instruction result))
            {
                if (!InstructionArguments.instructions.TryGetValue(result, out InstructionInfo instructionInfo))
                {

                }

                for (int i = 0; i < instructionInfo.m_operandSize; i++)
                {
                    if (i == 0)
                    {
                        arg1 = FetchByte();
                    }
                    else if (i == 1)
                    {
                        arg2 = FetchByte();
                    }
                }
                if (Enum.TryParse(arg1.ToString(), true, out argument1)) { }
                if (Enum.TryParse(arg2.ToString(), true, out argument2)) { }

                return result;
            }



            throw new NotImplementedException();
        }
    }
}
