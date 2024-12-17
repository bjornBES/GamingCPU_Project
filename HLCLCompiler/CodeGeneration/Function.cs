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
            //
            // |------|----------|---------|
            // |SP - ?|first arg | 4 bytes | we can find the first argument at stackSize + ArgumentSize
            // |------|----------|---------|
            // |SP -10|last arg  | 4 bytes |
            // |------|----------|---------|
            // |SP - 6| return   | 4 bytes |
            // |------|----------|---------|
            // |SP - 2| Old BP   | 2 bytes |
            // |------|----------|---------|
            //

            int stackStart = 6;
            for (int i = 0; i < arguments.Length; i++)
            {
                stackStart += 4;
                Address address = AddressHelper.GetStack(stackStart, "BP");
                Variabel temp = new Variabel(arguments[i].Name, arguments[i].Type, address, this);
                result.Add(temp);
            }

            return result.ToArray();
        }
    }
}
