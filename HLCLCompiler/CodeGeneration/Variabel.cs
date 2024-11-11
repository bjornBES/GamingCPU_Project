using HLCLCompiler.Types;

namespace HLCLCompiler.CodeGeneration
{
    public class Variabel
    {
        public string Name;
        public ExprType Type;
        public Address Address;
        public Function Function;

        public Variabel(string name, ExprType type, Address address, Function function)
        {
            Name = name;
            Type = type;
            Address = address;
            Function = function;
        }

        public string GetAddress(Generator generator)
        {
            if (Function == null || generator.functions.Last() != Function)
            {
                if (Address.IsStack)
                {
                    return $"[{Address.StackRegister} - {generator.StackSize - Address.StackPlacement}]";
                }
            }
            else
            {

            }

            return "";
        }
    }
}
