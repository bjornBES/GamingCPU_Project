namespace HLCLCompiler.CodeGeneration
{
    public class Function
    {
        public Function(NodeStmtDeclareFunction nodeStmtDeclareFunction)
        {
            name = nodeStmtDeclareFunction.name;
            arguments = nodeStmtDeclareFunction.arguments;
        }

        public string name;
        public Argument[] arguments;
        public string ReturnLabel
        {
            get
            {
                return $"_{name}Return";
            }
        }
        public string FunctionLabel
        {
            get
            {
                return $"_{name}";
            }
        }
        public int ArgumentSize
        {
            get
            {
                int result = 0;
                for (int i = 0; i < arguments.Length; i++)
                {
                    result += 4;
                }
                return result;
            }
        }
        public Variabel[] GetArgumentVariabels(int stackSize)
        {
            List<Variabel> result = new List<Variabel>();

            // first argument at stackSize - (ArgumentSize - 6)
            // last argument ? bytes
            // return address 4 bytes
            // old BP 2 bytes

            int stackStart = stackSize - (ArgumentSize - 6);
            for (int i = 0; i < arguments.Length; i++)
            {
                Address address = AddressHelper.GetStack(stackStart, "BP");
                Variabel temp = new Variabel(arguments[i].Name, arguments[i].Type, address, this);
                result.Add(temp);
                stackStart += 4;
            }

            return result.ToArray();
        }
    }
}
