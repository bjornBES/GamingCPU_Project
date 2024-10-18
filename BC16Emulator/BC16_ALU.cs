using System;
using System.Collections.Generic;
using System.Text;

namespace BCG16CPUEmulator
{
    public class BC16_ALU : BC16CPU_HelperFunctions
    {
        public void ALU_Sef(int flag)
        {
            SetFlag(flag, true);
        }

        public void ALU_Clf(int flag)
        {
            SetFlag(flag, false);
        }

        public int ALU_Add(Register destination, int source, bool carry, out int result)
        {
            int V = GetRegisterValue(destination);
            ALU_Add(V, source, carry, out result);
            return result;
        }
        public int ALU_Add(int operand1, int operand2, bool carry, out int result)
        {
            result = operand1 + operand2 + (carry ? 1 : 0);
            Compare(operand1, operand2 + (carry ? 1 : 0), FL_S, FL_O, FL_Z, FL_C);
            return result;
        }

        public int ALU_Sub(Register destination, int source, bool carry, out int result)
        {
            int V = GetRegisterValue(destination);
            ALU_Sub(V, source, carry, out result);
            return result;
        }
        public int ALU_Sub(int operand1, int operand2, bool carry, out int result)
        {
            result = (operand1 - operand2) - (carry ? 1 : 0);
            Compare(operand1, operand2 - (carry ? 1 : 0), FL_S, FL_U, FL_Z, FL_C);
            return result;
        }

        public int ALU_Mul(Register destination, int source, out int result)
        {
            int V = GetRegisterValue(destination);
            result = V * source;
            Compare(V, source, FL_S, FL_Z, FL_C);
            return result;
        }

        public int ALU_Div(Register destination, int source, out int result)
        {
            int V = GetRegisterValue(destination);
            result = V / source;
            Compare(V, source, FL_S, FL_Z, FL_C);
            return result;
        }

        public int ALU_And(Register destination, int source, out int result)
        {
            int V = GetRegisterValue(destination);
            result = V & source;
            Compare(V, source, FL_S, FL_Z, FL_C);
            return result;
        }

        public int ALU_Or(Register destination, int source, out int result)
        {
            int V = GetRegisterValue(destination);
            result = V | source;
            Compare(V, source, FL_S, FL_Z, FL_C);
            return result;
        }

        public int ALU_Nor(Register destination, int source, out int result)
        {
            int V = GetRegisterValue(destination);
            result = ~(V | source);
            Compare(V, source, FL_S, FL_Z, FL_C);
            return result;
        }

        public int ALU_Xor(Register destination, int source, out int result)
        {
            int V = GetRegisterValue(destination);
            result = V ^ source;
            Compare(V, source, FL_S, FL_Z, FL_C);
            return result;
        }

        public int ALU_Not(Register destination, out int result)
        {
            int V = GetRegisterValue(destination);
            result = ~V;
            Compare(V, result, FL_S, FL_Z, FL_C);
            return result;
        }

        public int ALU_Neg(Register destination, out int result)
        {
            int V = GetRegisterValue(destination);
            result = -V;
            Compare(V, result, FL_S, FL_Z, FL_C);
            return result;
        }

        public int ALU_RNG()
        {
            int V = new Random().Next(0, 0x10000);
            return V;
        }
    }
}
