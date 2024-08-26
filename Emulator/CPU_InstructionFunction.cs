using assembler.global;
using System;

namespace emulator
{
    public class CPU_InstructionFunction : CPUInterrupts
    {

        public InstructionInfo DecodeInstruction()
        {
            string instructionByte = Convert.ToString(FetchWordFromPC(), 16).PadLeft(4, '0');
            InstructionInfo instruction = GetInstructionInfo(instructionByte);
            if (instruction != default)
            {
                return instruction;
            }

            return GetInstructionInfo(Instruction.NOP);
        }
        public void ExecuteInstruction(InstructionInfo instruction)
        {
            switch (instruction.m_Instruction)
            {
                case Instruction.MOV:
                    Instr_MOV(instruction);
                    break;
                case Instruction.CMP:
                    break;
                case Instruction.PUSH:
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
                case Instruction.SEZ:
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
                case Instruction.IN:
                    break;
                case Instruction.OUT:
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
                case Instruction.MOVW:
                    break;
                case Instruction.MOVD:
                    break;
                case Instruction.MOVT:
                    break;
                case Instruction.MOVS:
                    break;
                case Instruction.MOVF:
                    break;
                case Instruction.CMPSTR:
                    break;
                case Instruction.DATE:
                    break;
                case Instruction.DELAY:
                    break;
                case Instruction.TIME:
                    break;
                case Instruction.CTA:
                    break;
                case Instruction.CTH:
                    break;
                case Instruction.SSF:
                    break;
                case Instruction.SMBR:
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

    }
}
