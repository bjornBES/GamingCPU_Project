using static CompilerSettings;
using Compiler;

public struct Address
{
    public bool m_UseStack;
    public int StackLoc;
    public string StackRegister;
    
    public bool m_IsArg;

    public bool m_isGlobal;
    public int m_address;
    public string GetAddress(Generator generator)
    {
        if (m_UseStack == true)
        {
            if (m_IsArg)
            {
                int address = StackLoc;
                if (address == 0)
                {
                    return $"[{StackRegister}]";
                }
                else
                {
                    return $"[{StackRegister} - {address}]";
                }
            }
            else
            {
                int address = generator.StackSize - StackLoc;
                if (address == 0)
                {
                    return $"[{StackRegister}]";
                }
                else
                {
                    return $"[{StackRegister} - {address}]";
                }
            }
        }
        else if (m_isGlobal)
        {
            string hex = Convert.ToString(m_address, 16).PadLeft(4, '0');
            return $"0x{hex}";
        }


        return "";
    }
}
public static class AddressHelper
{
    public static Address GetStackAddress(int stackPlacement, string stackRegister)
    {
        Address address= new Address();
        address.StackRegister = stackRegister;
        address.StackLoc = stackPlacement;
        address.m_UseStack = true;
        return address;
    }
}
