using static CompilerSettings;
using Compiler;

public struct Address
{
    public bool m_UseStack;
    public bool m_IsArg;
    public int m_StackLoc;

    public bool m_isGlobal;
    public int m_address;

    public string GetAddress(Parser parser)
    {
        if (m_UseStack == true)
        {
            if (m_IsArg)
            {
                int address = m_StackLoc;
                if (address == 0)
                {
                    if (m_CPUType < CPUType.BC32)
                    {
                        return $"[BP]";
                    }
                    else
                    {
                        return $"[BPX]";
                    }
                }
                else
                {
                    if (m_CPUType < CPUType.BC32)
                    {
                        return $"[BP - {address}]";
                    }
                    else
                    {
                        return $"[BPX- {address}]";
                    }
                }
            }
            else
            {
                int address = parser.m_StackSize - m_StackLoc;
                if (address == 0)
                {
                    if (m_CPUType < CPUType.BC32)
                    {
                        return $"[SP]";
                    }
                    else
                    {
                        return $"[SPX]";
                    }
                }
                else
                {
                    if (m_CPUType < CPUType.BC32)
                    {
                        return $"[SP - {address}]";
                    }
                    else
                    {
                        return $"[SPX - {address}]";
                    }
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
    public string GetAddressRaw(Parser parser)
    {
        if (m_UseStack == true)
        {
            if (m_IsArg)
            {
                int address = m_StackLoc;
                if (address == 0)
                {
                    if (m_CPUType < CPUType.BC32)
                    {
                        return $"[BP]";
                    }
                    else
                    {
                        return $"[BPX]";
                    }
                }
                else
                {
                    if (m_CPUType < CPUType.BC32)
                    {
                        return $"[BP - {address}]";
                    }
                    else
                    {
                        return $"[BPX- {address}]";
                    }
                }
            }
            else
            {
                int address = parser.m_StackSize - m_StackLoc;
                if (address == 0)
                {
                    if (m_CPUType < CPUType.BC32)
                    {
                        return $"[SP]";
                    }
                    else
                    {
                        return $"[SPX]";
                    }
                }
                else
                {
                    if (m_CPUType < CPUType.BC32)
                    {
                        return $"[SP - {address}]";
                    }
                    else
                    {
                        return $"[SPX - {address}]";
                    }
                }
            }
        }
        else if (m_isGlobal)
        {
            return $"far @__DATASTRAT__ + {m_address}";
        }


        return "";
    }
}
