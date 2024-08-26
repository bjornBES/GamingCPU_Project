using System;
using System.Diagnostics.CodeAnalysis;

namespace assembler.global
{
    public struct InstructionInfo
    {
        public Instruction m_Instruction;
        public Arguments m_Argument1;
        public Arguments m_Argument2;
        public string m_Opcode;

        public int m_ArgumentSize;
        public InstructionInfo(Instruction instruction, Arguments arg1, Arguments arg2, string opcode)
        {
            m_ArgumentSize = 0;
            m_Instruction = instruction;

            m_Argument1 = arg1;
            m_Argument2 = arg2;

            m_ArgumentSize += arg1 != Arguments.none ? 1 : 0;
            m_ArgumentSize += arg2 != Arguments.none ? 1 : 0;

            m_Opcode = opcode;
        }
        public InstructionInfo(Instruction instruction)
        {
            m_ArgumentSize = 0;
            m_Argument1 = Arguments.none;
            m_Argument2 = Arguments.none;
            m_Instruction = instruction;
            m_Opcode = "";
        }

        public static implicit operator Instruction(InstructionInfo info)
        {
            return info.m_Instruction;
        }
        public static implicit operator string(InstructionInfo info)
        {
            return info.m_Instruction.ToString();
        }

        public static explicit operator InstructionInfo(string v)
        {
            if (Enum.TryParse(v.ToUpper(), out Instruction result))
            {
                return new InstructionInfo(result);
            }
            return new InstructionInfo(Instruction.none);
        }
        public static explicit operator InstructionInfo(Instruction v)
        {
            return new InstructionInfo(v);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException();

            if (obj.GetType() == typeof(Instruction))
            {
                Instruction instruction = (Instruction)obj;

                if (instruction == m_Instruction)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (obj.GetType() == typeof(InstructionInfo))
            {
                InstructionInfo instruction = (InstructionInfo)obj;
                if (m_Instruction == instruction)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (obj.GetType() == typeof(string) && ((string)obj).Length == 4)
            {
                string str = (string)obj;

                return str == m_Opcode;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(InstructionInfo left, InstructionInfo right)
        {
            return left.m_Instruction == right.m_Instruction;
        }

        public static bool operator !=(InstructionInfo left, InstructionInfo right)
        {
            return left.m_Instruction != right.m_Instruction || left.m_Opcode != right.m_Opcode;
        }
    }
}