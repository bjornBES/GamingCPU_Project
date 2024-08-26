using System;
using System.Runtime.InteropServices;

namespace BCG16CPUEmulator.Types
{
    [Serializable]
    [ComVisible(true)]
    public struct _8Bit_Register
    {
        public byte m_Value;

        public const byte MaxValue = byte.MaxValue;

        public _8Bit_Register(byte value)
        {
            m_Value = value;
        }
        public _8Bit_Register(int value)
        {
            m_Value = (byte)value;
        }

        public _8Bit_Register this[int index]
        {
            get
            {
                if (index == 1)
                {
                    return (byte)(m_Value & 0x0F);
                }
                else if (index == 2)
                {
                    return (byte)((m_Value & 0xF0) >> 4);
                }

                return 0;
            }
            set
            {
                if (index == 1)
                {
                    m_Value &= 0xF0;
                    m_Value |= (byte)value;
                }
                else if (index == 2)
                {
                    m_Value &= 0x0F;
                    m_Value |= (byte)(value << 4);
                }
            }
        }

        public _8Bit_Register this[bool high]
        {
            get
            {
                if (high == false)
                {
                    return (byte)(m_Value & 0x0F);
                }
                else if (high == true)
                {
                    return (byte)((m_Value & 0xF0) >> 4);
                }

                return 0;
            }
            set
            {
                if (high == false)
                {
                    m_Value &= 0xF0;
                    m_Value |= (byte)value;
                }
                else if (high == true)
                {
                    m_Value &= 0x0F;
                    m_Value |= (byte)(value << 4);
                }
            }
        }

        public static bool operator !=(_8Bit_Register a, byte b)
        {
            return a.m_Value != b;
        }
        public static bool operator ==(_8Bit_Register a, byte b)
        {
            return a.m_Value == b;
        }

        public static byte operator +(_8Bit_Register a, byte b)
        {
            return (byte)(a.m_Value + b);
        }
        public static byte operator -(_8Bit_Register a, byte b)
        {
            return (byte)(a.m_Value - b);
        }
        public static byte operator |(_8Bit_Register a, byte b)
        {
            return (byte)(a.m_Value | b);
        }
        public static byte operator &(_8Bit_Register a, byte b)
        {
            return (byte)(a.m_Value & b);
        }

        public static _8Bit_Register operator ++(_8Bit_Register a)
        {
            a.m_Value++;
            return a;
        }
        public static _8Bit_Register operator --(_8Bit_Register a)
        {
            a.m_Value--;
            return a;
        }

        public static implicit operator _8Bit_Register(byte value)
        {
            return new _8Bit_Register(value);
        }

        public static implicit operator _8Bit_Register(int value)
        {
            return new _8Bit_Register(value);
        }

        public static implicit operator byte(_8Bit_Register register)
        {
            return register.m_Value;
        }

        public override bool Equals(object obj)
        {
            if (obj.Equals(m_Value))
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}
