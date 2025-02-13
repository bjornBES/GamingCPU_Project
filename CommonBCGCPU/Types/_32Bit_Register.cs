﻿using System;
using System.Runtime.InteropServices;

namespace CommonBCGCPU.Types
{
    [Serializable]
    [ComVisible(true)]
    public struct _32Bit_Register
    {
        public uint m_Value;
        public const uint MinValue = 0;
        public const uint MaxValue = uint.MaxValue;

        public _32Bit_Register(uint value)
        {
            m_Value = value;
        }
        public _32Bit_Register(int value)
        {
            m_Value = (uint)value;
        }

        public _16Bit_Register this[int index]
        {
            get
            {
                if (index == 1)
                {
                    return (ushort)(m_Value & 0x0000FFFF);
                }
                else if (index == 2)
                {
                    return (ushort)(m_Value & 0xFFFF0000) >> 16;
                }
                else if (index == 3)
                {
                    return m_Value & 0x000000FF;
                }
                else if (index == 4)
                {
                    return (m_Value & 0x0000FF00) >> 8;
                }

                return 0;
            }
            set
            {
                if (index == 1)
                {
                    m_Value &= 0xFFFF0000;
                    m_Value |= value;
                }
                else if (index == 2)
                {
                    m_Value &= 0x0000FFFF;
                    m_Value |= (ushort)(value << 16);
                }
                else if (index == 3)
                {
                    m_Value &= 0xFFFFFF00;
                    m_Value |= (ushort)value;
                }
                else if (index == 4)
                {
                    m_Value &= 0xFFFF00FF;
                    m_Value |= (ushort)(value << 8);
                }
            }
        }
        public _16Bit_Register this[bool high]
        {
            get
            {
                if (high == false)
                {
                    return (ushort)(m_Value & 0x0000FFFF);
                }
                else if (high == true)
                {
                    return (ushort)((m_Value & 0xFFFF0000) >> 16);
                }

                return 0;
            }
            set
            {
                if (high == false)
                {
                    m_Value &= 0xFFFF0000;
                    m_Value |= value;
                }
                else
                {
                    m_Value &= 0x0000FFFF;
                    m_Value |= (uint)(value << 16);
                }
            }
        }

        public static bool operator !=(_32Bit_Register a, uint b)
        {
            return a.m_Value != b;
        }
        public static bool operator ==(_32Bit_Register a, uint b)
        {
            return a.m_Value == b;
        }

        public static uint operator +(_32Bit_Register a, uint b)
        {
            return (ushort)(a.m_Value + b);
        }
        public static uint operator -(_32Bit_Register a, uint b)
        {
            return (ushort)(a.m_Value - b);
        }
        public static uint operator |(_32Bit_Register a, uint b)
        {
            return (ushort)(a.m_Value | b);
        }
        public static uint operator &(_32Bit_Register a, uint b)
        {
            return (ushort)(a.m_Value & b);
        }

        public static _32Bit_Register operator ++(_32Bit_Register a)
        {
            a.m_Value++;
            return a;
        }
        public static _32Bit_Register operator --(_32Bit_Register a)
        {
            a.m_Value--;
            return a;
        }

        public static implicit operator _32Bit_Register(ulong value)
        {
            return new _32Bit_Register((uint)value);
        }
        public static implicit operator _32Bit_Register(uint value)
        {
            return new _32Bit_Register(value);
        }
        public static implicit operator _32Bit_Register(int value)
        {
            return new _32Bit_Register(value);
        }
        public static implicit operator _32Bit_Register(ushort value)
        {
            return new _32Bit_Register(value);
        }
        public static implicit operator _32Bit_Register(Address value)
        {
            return new _32Bit_Register(value);
        }

        public static implicit operator string(_32Bit_Register register)
        {
            return register.ToHex();
        }
        public static implicit operator uint(_32Bit_Register register)
        {
            return register.m_Value;
        }

        public static implicit operator int(_32Bit_Register register)
        {
            return (int)register.m_Value;
        }

        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public string ToHex()
        {
            return Convert.ToString(m_Value, 16).PadLeft(8, '0');
        }
    }
}
