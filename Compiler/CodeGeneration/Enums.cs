public enum AsmSection
{
    bss,
    rodata,
    data,
    text,
    loacl,
}
public enum Section
{
    none,
    text,
    data,
    _string,
}
public enum Size
{
    _byte = 1,
    _short = 2,
    _tbyte = 3,
    _int = 4,
    pointer,
}

public enum Regs
{
    A,
    B,
    C,
    D,
    HL,
    SP,
    BP,
    H,
    L,
    R1,
}

public class RegsFunctions
{
    public static string ToString(Regs regs)
    {
        switch (regs)
        {
            case Regs.A:
                if (CompilerSettings.InProtectedMode)
                {
                    return "AX";
                }
                else
                {
                    return "A";
                }
            case Regs.B:
                if (CompilerSettings.InProtectedMode)
                {
                    return "BX";
                }
                else
                {
                    return "B";
                }
            case Regs.C:
                if (CompilerSettings.InProtectedMode)
                {
                    return "CX";
                }
                else
                {
                    return "C";
                }
            case Regs.D:
                if (CompilerSettings.InProtectedMode)
                {
                    return "DX";
                }
                else
                {
                    return "D";
                }
            case Regs.HL:
                return "HL";
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
            case Regs.A:
            case Regs.B:
            case Regs.C:
            case Regs.D:
                if (CompilerSettings.InProtectedMode)
                {
                    return TypeSize.INT;
                }
                else
                {
                    return TypeSize.SHORT;
                }
            case Regs.SP:
            case Regs.BP:
                return TypeSize.SHORT;
            default:
                break;
        }
        return 0;
    }
}