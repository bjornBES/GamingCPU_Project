using System;
using System.Runtime.InteropServices;

namespace emulator.register
{
    [Serializable]
    [ComVisible(true)]
    public struct _16Bit_Register
    {
        public ushort m_Value;

        public _16Bit_Register(ushort value)
        {
            m_Value = value;
        }
        public _16Bit_Register(_8Bit_Register value, bool High)
        {
            m_Value = value;
        }
        public _16Bit_Register(int value)
        {
            m_Value = (ushort)value;
        }

        public _8Bit_Register this[int index]
        {
            get
            {
                if (index == 1)
                {
                    return (byte)(m_Value & 0x00FF);
                }
                else if (index == 2)
                {
                    return (byte)((m_Value & 0xFF00) >> 8);
                }

                return 0;
            }
            set
            {
                if (index == 1)
                {
                    m_Value &= 0xFF00;
                    m_Value |= value;
                }
                else if (index == 2)
                {
                    m_Value &= 0x00FF;
                    m_Value |= (ushort)(value << 8);
                }
            }
        }

        public _8Bit_Register this[bool high]
        {
            get
            {
                if (high == false)
                {
                    return (byte)(m_Value & 0x00FF);
                }
                else if (high == true)
                {
                    return (byte)((m_Value & 0xFF00) >> 8);
                }

                return 0;
            }
            set
            {
                if (high == false)
                {
                    m_Value &= 0xFF00;
                    m_Value |= value;
                }
                else if (high == true)
                {
                    m_Value &= 0x00FF;
                    m_Value |= (ushort)(value << 8);
                }
            }
        }

        public static bool operator !=(_16Bit_Register a, ushort b)
        {
            return a.m_Value != b;
        }
        public static bool operator ==(_16Bit_Register a, ushort b)
        {
            return a.m_Value == b;
        }

        public static ushort operator +(_16Bit_Register a, ushort b)
        {
            return (ushort)(a.m_Value + b);
        }
        public static ushort operator -(_16Bit_Register a, ushort b)
        {
            return (ushort)(a.m_Value - b);
        }
        public static ushort operator |(_16Bit_Register a, ushort b)
        {
            return (ushort)(a.m_Value | b);
        }
        public static ushort operator &(_16Bit_Register a, ushort b)
        {
            return (ushort)(a.m_Value & b);
        }

        public static _16Bit_Register operator ++(_16Bit_Register a)
        {
            a.m_Value++;
            return a;
        }
        public static _16Bit_Register operator --(_16Bit_Register a)
        {
            a.m_Value--;
            return a;
        }

        public static implicit operator _16Bit_Register(ushort value)
        {
            return new _16Bit_Register(value);
        }
        public static implicit operator _16Bit_Register(int value)
        {
            return new _16Bit_Register(value);
        }
        public static implicit operator _16Bit_Register(_8Bit_Register value)
        {
            return new _16Bit_Register(value, false);
        }

        public static implicit operator int(_16Bit_Register register)
        {
            return register.m_Value;
        }
        public static implicit operator ushort(_16Bit_Register register)
        {
            return register.m_Value;
        }
        public static implicit operator _8Bit_Register(_16Bit_Register register)
        {
            return register[false];
        }

        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}
