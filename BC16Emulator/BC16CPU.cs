using CommonBCGCPU;
using CommonBCGCPU.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Text;
using System.Threading.Tasks;

namespace BCG16CPUEmulator
{
    public class BC16CPU : BC16CPU_Instructions
    {
        Stack<byte> m_portsIRQ = new Stack<byte>(256);
        Stack<byte> m_portsNMI = new Stack<byte>(256);

        Instruction[] m_instructions = new Instruction[5];
        ArgumentMode[] m_argumentModes1 = new ArgumentMode[5];
        ArgumentMode[] m_argumentModes2 = new ArgumentMode[5];



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

            m_AX = m_BX = m_CX = m_DX = 0;
            m_HL = 0;

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

            m_F = 0;
        }

        uint m_stage1 = 0;
        uint m_stage2 = uint.MaxValue;
        uint m_stage3 = uint.MaxValue - 1;

        ArgumentMode m_argument1;
        ArgumentMode m_argument2;
        public void Tick()
        {
            if (GetFlag(FL_I) == true)
            {
                if (m_portsIRQ.TryPop(out byte result) == true)
                {
                    SetFlag(FL_I, true);
                    executeInstruction(m_instructions[0], m_argumentModes1[0], m_argumentModes2[0], ref m_stage1);
                    executeInstruction(m_instructions[1], m_argumentModes1[1], m_argumentModes2[1], ref m_stage2);
                    executeInstruction(m_instructions[2], m_argumentModes1[2], m_argumentModes2[2], ref m_stage3);

                    executionInterrupt((byte)(result));

                    tickOpertion(0);
                    tickOpertion(1);
                    tickOpertion(2);
                    return;
                }
            }
            if (m_portsNMI.TryPop(out byte NMIresult) == true)
            {
                executionInterrupt((byte)(NMIresult));
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

        void executionInterrupt(byte interruptNumber)
        {
            SetFlag(FL_I, false);
            m_BUS.INTA(interruptNumber);

            PushInterrupt();

            Address InterruptAddress = (interruptNumber * 4);
            Address InterruptFunctionAddress = m_BUS.m_Memory.ReadDWord(InterruptAddress);

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
                    tickStage(ref m_stage1, 0);
                    break;
                case 1:
                    tickStage(ref m_stage2, 1);
                    break;
                case 2:
                    tickStage(ref m_stage3, 2);
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
                case Instruction.MOVRALCR0:
                    Move(Register.AL, Register.CR0);
                    return;
                case Instruction.MOVRCR0AL:
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
                    Move(Size._word, Register.AX, Argument1);
                    return;
                case Instruction.MOVDRBX:
                    Move(Size._word, Register.BX, Argument1);
                    return;
                case Instruction.MOVDRCX:
                    Move(Size._word, Register.CX, Argument1);
                    return;
                case Instruction.MOVDRDX:
                    Move(Size._word, Register.DX, Argument1);
                    return;

                case Instruction.CMP:
                    Cmp(Argument1, Argument2);
                    return;
                case Instruction.CMPZ:
                    break;
                case Instruction.CMPRA:
                    Cmp(Register.A, Argument1);
                    return;
                case Instruction.CMPRAX:
                    Cmp(Register.AX, Argument1);
                    return;

                case Instruction.PUSH:
                    Push(Size._byte, Argument1);
                    return;

                case Instruction.POP:
                    Pop(Size._byte, Argument1);
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
                case Instruction.RETL:
                    if ((m_CR0 & CR0_EnableExtendedMode) == CR0_EnableExtendedMode)
                    {
                        PopPC();
                        return;
                    }
                    return;

                case Instruction.SEZ:
                    Sez(Argument1);
                    return;
                case Instruction.SEZRAL:
                    Sez(Register.AL);
                    return;
                case Instruction.SEZRBL:
                    Sez(Register.BL);
                    return;
                case Instruction.SEZRCL:
                    Sez(Register.CL);
                    return;
                case Instruction.SEZRDL:
                    Sez(Register.DL);
                    return;
                case Instruction.SEZRA:
                    Sez(Register.A);
                    return;
                case Instruction.SEZRB:
                    Sez(Register.B);
                    return;
                case Instruction.SEZRC:
                    Sez(Register.C);
                    return;
                case Instruction.SEZRD:
                    Sez(Register.D);
                    return;
                case Instruction.SEZRAX:
                    Sez(Register.AX);
                    return;
                case Instruction.SEZRBX:
                    Sez(Register.BX);
                    return;
                case Instruction.SEZRCX:
                    Sez(Register.CX);
                    return;
                case Instruction.SEZRDX:
                    Sez(Register.DX);
                    return;

                case Instruction.TEST:
                    Test(m_argument1);
                    return;
                case Instruction.TESTRAL:
                    Test(Register.AL);
                    return;
                case Instruction.TESTRBL:
                    Test(Register.BL);
                    break;
                case Instruction.TESTRCL:
                    Test(Register.CL);
                    break;
                case Instruction.TESTRDL:
                    Test(Register.DL);
                    break;
                case Instruction.TESTRA:
                    Test(Register.A);
                    return;
                case Instruction.TESTRB:
                    Test(Register.B);
                    break;
                case Instruction.TESTRC:
                    Test(Register.C);
                    break;
                case Instruction.TESTRD:
                    Test(Register.D);
                    break;
                case Instruction.TESTRAX:
                    Test(Register.AX);
                    return;
                case Instruction.TESTRBX:
                    Test(Register.BX);
                    break;
                case Instruction.TESTRCX:
                    Test(Register.CX);
                    break;
                case Instruction.TESTRDX:
                    Test(Register.DX);
                    break;

                case Instruction.SWAP:
                    Swap(Argument1, Argument2);
                    break;

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
                    JumpC(Argument1, FL_G, 1);
                    return;
                case Instruction.JLE:
                    JumpC(Argument1, FL_L, FL_E, 1, 1);
                    return;
                case Instruction.JGE:
                    JumpC(Argument1, FL_G, FL_E, 1, 1);
                    return;
                case Instruction.JNV:
                    JumpC(Argument1, FL_O, 0);
                    return;
                case Instruction.JC:
                    JumpC(Argument1, FL_C, 1);
                    return;
                case Instruction.JNC:
                    JumpC(Argument1, FL_C, 0);
                    return;

                case Instruction.INB:
                    Inp(Size._byte, Argument1, Argument2);
                    return;
                case Instruction.INW:
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
                    Add(m_argument1, m_argument2, false);
                    return;
                case Instruction.ADDRA:
                    Add(Register.A, Argument1, false);
                    return;
                case Instruction.ADDRAX:
                    Add(Register.AX, Argument1, false);
                    return;
                case Instruction.ADC:
                    Add(m_argument1, m_argument2, true);
                    return;
                case Instruction.ADCRA:
                    Add(Register.A, Argument1, true);
                    return;
                case Instruction.ADCRAX:
                    Sub(Register.AX, Argument1, true);
                    return;
                case Instruction.SUB:
                    Sub(m_argument1, m_argument2, false);
                    return;
                case Instruction.SUBRA:
                    Sub(Register.A, Argument1, false);
                    return;
                case Instruction.SUBRAX:
                    Sub(Register.AX, Argument1, false);
                    return;
                case Instruction.SBB:
                    Sub(m_argument1, m_argument2, true);
                    return;
                case Instruction.SBBRA:
                    Sub(Register.A, Argument1, true);
                    return;
                case Instruction.SBBRAX:
                    Sub(Register.AX, Argument1, true);
                    return;
                case Instruction.MUL:
                    Mul(Argument1, Argument2);
                    return;
                case Instruction.MULRA:
                    Mul(Register.A, Argument1);
                    return;
                case Instruction.MULRAX:
                    Mul(Register.AX, Argument1);
                    return;
                case Instruction.DIV:
                    Div(Argument1, Argument2);
                    return;
                case Instruction.DIVRA:
                    Div(Register.A, Argument1);
                    return;
                case Instruction.DIVRAX:
                    Div(Register.AX, Argument1);
                    return;
                case Instruction.AND:
                    And(Argument1, Argument2);
                    return;
                case Instruction.ANDRA:
                    And(Register.A, Argument1);
                    return;
                case Instruction.ANDRAX:
                    And(Register.AX, Argument1);
                    return;
                case Instruction.OR:
                    Or(Argument1, Argument2);
                    return;
                case Instruction.ORRA:
                    Or(Register.A, Argument1);
                    return;
                case Instruction.ORRAX:
                    Or(Register.AX, Argument1);
                    return;
                case Instruction.NOR:
                    Nor(Argument1, Argument2);
                    return;
                case Instruction.NORRA:
                    Nor(Register.A, Argument1);
                    return;
                case Instruction.NORRAX:
                    Nor(Register.AX, Argument1);
                    return;
                case Instruction.XOR:
                    Xor(Argument1, Argument2);
                    return;
                case Instruction.XORRA:
                    Xor(Register.A, Argument1);
                    return;
                case Instruction.XORRAX:
                    Xor(Register.AX, Argument1);
                    return;
                case Instruction.NOT:
                    Not(Argument1);
                    return;
                case Instruction.NOTRA:
                    Not(Register.A);
                    return;
                case Instruction.NOTRAX:
                    Not(Register.AX);
                    return;
                case Instruction.SHL:
                    Shl(Argument1, Argument2);
                    break;
                case Instruction.SHR:
                    Shr(Argument1, Argument2);
                    break;
                case Instruction.ROL:
                    Rol(Argument1, Argument2);
                    break;
                case Instruction.ROR:
                    Ror(Argument1, Argument2);
                    break;
                case Instruction.INC:
                    Inc(Argument1);
                    return;
                case Instruction.DEC:
                    Dec(Argument1);
                    return;
                case Instruction.NEG:
                    Neg(Argument1);
                    return;
                case Instruction.EXP:
                    Exp(Argument1, Argument2);
                    break;
                case Instruction.SQRT:
                    Sqrt(Argument1);
                    break;
                case Instruction.RNG:
                    Rng(Argument1);
                    return;
                case Instruction.SEB:
                    break;
                case Instruction.CLB:
                    break;
                case Instruction.TOB:
                    break;
                case Instruction.MOD:
                    Mod(Argument1, Argument2);
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
                    Cmpl();
                    break;
                case Instruction.MOVF:
                    break;
                case Instruction.RETI:
                    Reti();
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
                    executionInterrupt(0x1);
                    return;
                case Instruction.HALT:
                    SetFlag(FL_H, true);
                    return;
                case Instruction.NOP:
                    return;
                case Instruction.CBTA:
                    CBTA(Argument1, Argument2);
                    return;
                case Instruction.ENTER:
                    Push(Register.BP);
                    SetRegisterValue(Register.BP, Register.SP);
                    return;
                case Instruction.LEAVE:
                    Pop(Register.BP);
                    return;
                case Instruction.CPUID:
                    m_AX[false] = 0b0;
                    break;
                default:
                    break;
            }

            Console.WriteLine($"{instruction} {Argument1}, {Argument2}");
        }

        Instruction decodeInstruction(out ArgumentMode argument1, out ArgumentMode argument2)
        {
            argument1 = ArgumentMode.None;
            argument2 = ArgumentMode.None;

            int orgPC = m_PC;

            byte arg1 = 0xFF;
            byte arg2 = 0xFF;
            ushort instrByte = FetchWord();
            if (Enum.TryParse(instrByte.ToString(), true, out Instruction result))
            {
                if (!InstructionArguments.m_Instructions.TryGetValue(result, out InstructionInfo instructionInfo))
                {
                    SetRegisterValue(Register.HL, orgPC);
                    executionInterrupt(0x6);
                    return decodeInstruction(out argument1, out argument2);
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
                if (!Enum.TryParse(arg1.ToString(), true, out argument1))
                {
                    SetRegisterValue(Register.HL, orgPC);
                    executionInterrupt(0x6);
                    return decodeInstruction(out argument1, out argument2);
                }
                if (!Enum.TryParse(arg2.ToString(), true, out argument2))
                {
                    SetRegisterValue(Register.HL, orgPC);
                    executionInterrupt(0x6);
                    return decodeInstruction(out argument1, out argument2);
                }

                return result;
            }
            else
            {
                executionInterrupt(0x6);
            }
            throw new NotImplementedException();
        }
    }
}
