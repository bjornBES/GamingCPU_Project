using HLCLCompiler.Types;

namespace HLCLCompiler.CodeGeneration
{
    public enum Regs
    {
        AX,
        BX,
        CX,
        DX,

        SP,
        BP
    }
    public class RegsFunctions
    {
        public static string ToString(Regs regs)
        {
            switch (regs)
            {
                case Regs.AX:
                    return "AX";
                case Regs.BX:
                    return "BX";
                case Regs.CX:
                    return "CX";
                case Regs.DX:
                    return "DX";
                case Regs.SP:
                    return "SP";
                case Regs.BP:
                    return "BP";
                default:
                    break;
            }
            return "";
        }
        public static TypeSize GetSize(Regs regs)
        {
            switch (regs)
            {
                case Regs.AX:
                case Regs.BX:
                case Regs.CX:
                case Regs.DX:
                    return TypeSize.INT;
                case Regs.SP:
                case Regs.BP:
                    return TypeSize.SHORT;
                default:
                    break;
            }
            return 0;
        }
    }
}
