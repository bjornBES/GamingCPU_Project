using CommonBCGCPU;
using CommonBCGCPU.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Text;

namespace BCG16CPUEmulator
{
    public class BC16CPU : BC16CPU_Instructions
    {
        Stack<byte> m_portsIRQ = new Stack<byte>(256);
        Stack<byte> m_portsNMI = new Stack<byte>(256);

        const ushort GotInterrupt = 0x7F55;
        
        Instruction[] m_instructions = new Instruction[3];
        ArgumentMode[] m_argumentModes1 = new ArgumentMode[3];
        ArgumentMode[] m_argumentModes2 = new ArgumentMode[3];

        public void IRQ(byte Port)
        {
            m_portsIRQ.Push(Port);
        }
        public void NMI(byte Port)
        {
            m_portsNMI.Push(Port);
        }

        public void ConnectBus(BUS bus)
        {
            m_BUS = bus;

            ResetCPU();
        }

        public void ResetCPU()
        {
            m_portsNMI.Clear();
            m_portsIRQ.Clear();

            m_instructions.Initialize();
            m_argumentModes1.Initialize();
            m_argumentModes2.Initialize();

            m_A = m_B = m_C = m_D = 0;
            m_H = m_L = 0;

            m_HL[true] = m_H;
            m_HL[false] = m_L;

            m_DS = 0;
            m_ES = 0;
            m_FS = 0;
            m_SS = 0;
            m_CS = 0;

            // stating point is 0x1200
            m_PC = 0x1200;

            m_BP = m_SP = 0;

            m_R1 = 0; 
            m_R2 = 0;
            m_R3 = 0;
            m_R4 = 0;
            m_R5 = 0;
            m_R6 = 0;

            m_MB = 0;

            m_CR0 = m_CR1 = 0xFF;

            m_F = 0;
        }

        uint m_stage1 = 0;
        uint m_stage2 = uint.MaxValue;
        uint m_stage3 = uint.MaxValue - 1;

        ArgumentMode m_argument1;
        ArgumentMode m_argument2;
        public void Tick()
        {
            m_HL[true] = m_H;
            m_HL[false] = m_L;

            m_AX[false] = m_A;
            m_BX[false] = m_B;
            m_CX[false] = m_C;
            m_DX[false] = m_D;

            if (GetFlag(FL_I) == true)
            {
                if (m_portsIRQ.TryPop(out byte result) == true)
                {
                    SetFlag(FL_I, true);
                    executeInstruction(m_instructions[0], m_argumentModes1[0], m_argumentModes2[0], ref m_stage1);
                    executeInstruction(m_instructions[1], m_argumentModes1[1], m_argumentModes2[1], ref m_stage2);
                    executeInstruction(m_instructions[2], m_argumentModes1[2], m_argumentModes2[2], ref m_stage3);

                    executionInterrupt(result);

                    tickOpertion(0);
                    tickOpertion(1);
                    tickOpertion(2);
                    return;
                }
            }
            if (m_portsNMI.TryPop(out byte NMIresult) == true)
            {
                executionInterrupt(NMIresult);
            }

            executeInstruction(m_instructions[0], m_argumentModes1[0], m_argumentModes2[0], ref m_stage1);
            executeInstruction(m_instructions[1], m_argumentModes1[1], m_argumentModes2[1], ref m_stage2);
            executeInstruction(m_instructions[2], m_argumentModes1[2], m_argumentModes2[2], ref m_stage3);

            if (GetFlag(FL_J) == true)
            {
                m_PC = m_AddressSave;
                SetFlag(FL_J, false);
            }

            tickOpertion(0);
            tickOpertion(1);
            tickOpertion(2);
        }

        void executionInterrupt(byte port)
        {
            m_BUS.INTA(port);

            PushInterrupt();

            Address InterruptAddress = (port * 4) + 0x0040;
            Address InterruptFunctionAddress = m_BUS.m_Memory.ReadDWord(InterruptAddress, 0x11);

            m_PC = InterruptFunctionAddress;
        }

        void tickOpertion(int Opertion)
        {

            if (GetFlag(FL_H) == true)
            {
                return;
            }
            switch (Opertion)
            {
                case 0:
                    tickStage(ref m_stage1,0);
                    break;
                case 1:
                    tickStage(ref m_stage2,1);
                    break;
                case 2:
                    tickStage(ref m_stage3,2);
                    break;
                default:
                    break;
            }

        }

        void tickStage(ref uint stage, int Opertion)
        {
            if (GetFlag(FL_J) == true)
            {
                return;
            }

            switch (stage)
            {
                case 0:
                    break;
                case 1:
                    m_IR = decodeInstruction(out m_argument1, out m_argument2);
                    m_instructions[Opertion] = m_IR;
                    m_argumentModes1[Opertion] = m_argument1;
                    m_argumentModes2[Opertion] = m_argument2;
                    break;
                case 2:
                    stage = 0;
                    return;
            }
            stage++;
        }

        private void executeInstruction(Instruction instruction, ArgumentMode Argument1, ArgumentMode Argument2, ref uint stage)
        {
            if (stage != 2)
            {
                return;
            }

            m_argumentOffset = 3;

            switch (instruction)
            {
                case Instruction.MOV:
                    Move(Size._byte, Argument1, Argument2);
                    return;
                case Instruction.MOVRAL:
                    Move(Size._byte, Register.AL, Argument1);
                    return;
                case Instruction.MOVRBL:
                    Move(Size._byte, Register.BL, Argument1);
                    return;
                case Instruction.MOVRCL:
                    Move(Size._byte, Register.CL, Argument1);
                    return;
                case Instruction.MOVRDL:
                    Move(Size._byte, Register.DL, Argument1);
                    return;
                case Instruction.MOVWRACR0:
                    Move(Register.AL, Register.CR0);
                    return;
                case Instruction.MOVWRCR0A:
                    Move(Register.CR0, Register.AL);
                    return;

                case Instruction.MOVW:
                    Move(Size._word, Argument1, Argument2);
                    return;
                case Instruction.MOVWRA:
                    Move(Size._word, Register.A, Argument1);
                    return;
                case Instruction.MOVWRB:
                    Move(Size._word, Register.B, Argument1);
                    return;
                case Instruction.MOVWRC:
                    Move(Size._word, Register.C, Argument1);
                    return;
                case Instruction.MOVWRD:
                    Move(Size._word, Register.D, Argument1);
                    return;

                case Instruction.MOVT:
                    Move(Size._tbyte, Argument1, Argument2);
                    return;

                case Instruction.MOVD:
                    Move(Size._dword, Argument1, Argument2);
                    return;
                case Instruction.MOVDRAX:
                    return;
                
                case Instruction.CMP:
                    Cmp(Argument1, Argument2);
                    return;

                case Instruction.PUSH:
                    Push(Size._byte, Argument1);
                    return;
                
                case Instruction.PUSHW:
                    Push(Size._word, Argument1);
                    return;
                
                case Instruction.PUSHT:
                    Push(Size._tbyte, Argument1);
                    return;
                
                case Instruction.PUSHD:
                    Push(Size._dword, Argument1);
                    return;
                
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
                case Instruction.CALLRHL:
                    Call(m_HL);
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
                case Instruction.SEZRAX:
                    SetRegisterValue(Register.AX, 0);
                    return;

                case Instruction.TEST:
                    Test(m_argument1);
                    return;
                case Instruction.TESTRAL:
                    Compare(Register.AL, Register.AL);
                    return;
                case Instruction.TESTRA:
                    Compare(Register.A, Register.A);
                    return;
                case Instruction.TESTRAX:
                    Compare(Register.AX, Register.AX);
                    return;

                case Instruction.JMP:
                    Jump(Argument1);
                    return;
                case Instruction.JZ:
                    JumpC(Argument1, FL_Z, 1);
                    return;
                case Instruction.JNZ:
                    JumpC(Argument1, FL_Z, 0);
                    return;
                case Instruction.JS:
                    JumpC(Argument1, m_FS, 1);
                    return;
                case Instruction.JNS:
                    JumpC(Argument1, m_FS, 0);
                    return;
                case Instruction.JE:
                    JumpC(Argument1, FL_E, 1);
                    return;
                case Instruction.JNE:
                    JumpC(Argument1, FL_E, 0);
                    return;
                case Instruction.JL:
                    JumpC(Argument1, FL_L, 1);
                    return;
                case Instruction.JG:
                    JumpC(Argument1, FL_L, 0);
                    return;
                case Instruction.JLE:
                    if (GetFlag(FL_L) == true || GetFlag(FL_E) == true)
                    {
                        Jump(Argument1);
                    }
                    return;
                case Instruction.JGE:
                    if (GetFlag(FL_L) == false || GetFlag(FL_E) == true)
                    {
                        Jump(Argument1);
                    }
                    return;
                case Instruction.JNV:
                    JumpC(Argument1, FL_O, 0);
                    return;

                case Instruction.INPB:
                    Inp(Size._byte, Argument1, Argument2);
                    return;
                case Instruction.INPW:
                    Inp(Size._word, Argument1, Argument2);
                    return;
                case Instruction.OUTB:
                    Out(Size._byte, Argument1, Argument2);
                    return;
                case Instruction.OUTW:
                    Out(Size._word, Argument1, Argument2);
                    return;

                case Instruction.SZE:
                    SetFlag(FL_Z, true);
                    return;
                case Instruction.SEE:
                    SetFlag(FL_E, true);
                    return;
                case Instruction.SES:
                    SetFlag(m_FS, true);
                    return;
                case Instruction.SEC:
                    SetFlag(FL_C, true);
                    return;
                case Instruction.SEL:
                    SetFlag(FL_L, true);
                    return;
                case Instruction.SEI:
                    SetFlag(FL_I, true);
                    return;
                case Instruction.SEH:
                    SetFlag(FL_H, true);
                    return;
                case Instruction.CZE:
                    SetFlag(FL_Z, false);
                    return;
                case Instruction.CLE:
                    SetFlag(FL_E, false);
                    return;
                case Instruction.CLS:
                    SetFlag(m_FS, false);
                    return;
                case Instruction.CLC:
                    SetFlag(FL_C, false);
                    return;
                case Instruction.CLL:
                    SetFlag(FL_L, false);
                    return;
                case Instruction.CLI:
                    SetFlag(FL_I, false);
                    return;
                case Instruction.CLH:
                    SetFlag(FL_H, false);
                    return;

                case Instruction.ADD:
                    Add(m_argument1, m_argument2);
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
                case Instruction.CMPL:
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
                    SetFlag(FL_H, true);
                    return;
                case Instruction.NOP:
                    return;
                case Instruction.CBTA:
                    break;
                case Instruction.ENTER:
                    Push(Register.BP);
                    SetRegisterValue(Register.BP, Register.SP);
                    Pushr();
                    return;
                case Instruction.LEAVE:
                    Popr();
                    Pop(Register.BP);
                    return;
                default:
                    break;
            }

            Console.WriteLine($"{instruction} {Argument1}, {Argument2}");
        }

        Instruction decodeInstruction(out ArgumentMode argument1, out ArgumentMode argument2)
        {
            argument1 = ArgumentMode.None;
            argument2 = ArgumentMode.None;

            byte arg1 = 0xFF;
            byte arg2 = 0xFF;
            ushort instrByte = FetchWord();
            if (Enum.TryParse(instrByte.ToString(), true, out Instruction result))
            {
                if (!InstructionArguments.m_Instructions.TryGetValue(result, out InstructionInfo instructionInfo))
                {

                }

                for (int i = 0; i < instructionInfo.m_OperandSize; i++)
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
