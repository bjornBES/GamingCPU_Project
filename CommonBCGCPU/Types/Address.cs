using System.Runtime.InteropServices;
using System;

namespace CommonBCGCPU.Types
{
    [Serializable]
    [ComVisible(true)]
    public struct Address
    {
        ulong m_Value;
        public const uint MaxValue = 0xFFFFFFFF;
        public const uint MinValue = 0x00000000;

        public Address(ulong value)
        {
            if (value > MaxValue) value = MaxValue;
            m_Value = value;
        }

        public static Address operator +(Address a, ulong b)
        {
            return (Address)(a.m_Value + b);
        }
        public static Address operator -(Address a, ulong b)
        {
            return (Address)(a.m_Value - b);
        }
        public static Address operator |(Address a, ulong b)
        {
            return (Address)(a.m_Value | b);
        }
        public static Address operator &(Address a, ulong b)
        {
            return (Address)(a.m_Value & b);
        }

        public static Address operator ++ (Address a)
        {
            a.m_Value++;
            return a;
        }

        public static implicit operator Address(ushort value)
        {
            return new Address(value);
        }
        public static implicit operator Address(uint value)
        {
            return new Address(value);
        }
        public static implicit operator Address(int value)
        {
            return new Address(Convert.ToUInt32(value));
        }
        public static implicit operator Address(_32Bit_Register value)
        {
            return new Address(value.m_Value);
        }
        public static implicit operator Address(_24Bit_Register value)
        {
            return new Address(value.m_Value);
        }
        public static implicit operator Address(_16Bit_Register value)
        {
            return new Address(value.m_Value);
        }
        public static implicit operator Address(_8Bit_Register value)
        {
            return new Address(value.m_Value);
        }

        public static implicit operator ushort(Address register)
        {
            return (ushort)register.m_Value;
        }
        public static implicit operator ulong(Address register)
        {
            return register.m_Value;
        }

        public static implicit operator int(Address register)
        {
            return Convert.ToInt32(register.m_Value);
        }
    }
}
