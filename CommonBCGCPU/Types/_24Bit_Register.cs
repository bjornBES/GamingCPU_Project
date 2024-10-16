﻿using System.Runtime.InteropServices;
using System;

namespace CommonBCGCPU.Types
{
    [Serializable]
    [ComVisible(true)]
    public struct _24Bit_Register
    {
        public uint m_Value;
        public const uint MaxValue = 0x00FFFFFF;
        public const uint MinValue = 0x00000000;

        public _24Bit_Register(uint value)
        {
            m_Value = value;
        }
        public _24Bit_Register(int value)
        {
            m_Value = (uint)value;
        }

        public _24Bit_Register(long value)
        {
            m_Value = (uint)value;
        }

        public _24Bit_Register this[int index]
        {
            get
            {
                if (index == 1)
                {
                    return m_Value & 0x0000FFFF;
                }
                else if (index == 2)
                {
                    return (m_Value & 0xFFFF0000) >> 8;
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
            }
        }

        public _32Bit_Register this[bool high]
        {
            get
            {
                if (high == false)
                {
                    return m_Value & 0x0000FFFF;
                }
                else if (high == true)
                {
                    return (m_Value & 0xFFFF0000) >> 16;
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
                else if (high == true)
                {
                    m_Value &= 0x0000FFFF;
                    m_Value |= (uint)(value << 16);
                }
            }
        }

        public static bool operator !=(_24Bit_Register a, ushort b)
        {
            return a.m_Value != b;
        }
        public static bool operator ==(_24Bit_Register a, ushort b)
        {
            return a.m_Value == b;
        }

        public static _24Bit_Register operator +(_24Bit_Register a, ushort b)
        {
            _24Bit_Register result = new _24Bit_Register(a.m_Value + b);
            return result;
        }
        public static _24Bit_Register operator -(_24Bit_Register a, ushort b)
        {
            _24Bit_Register result = new _24Bit_Register(a.m_Value - b);
            return result;
        }
        public static _24Bit_Register operator |(_24Bit_Register a, ushort b)
        {
            _24Bit_Register result = new _24Bit_Register(a.m_Value | b);
            return result;
        }
        public static _24Bit_Register operator &(_24Bit_Register a, ushort b)
        {
            _24Bit_Register result = new _24Bit_Register(a.m_Value & b);
            return result;
        }

        public static _24Bit_Register operator +(_24Bit_Register a, int b)
        {
            _24Bit_Register result = new _24Bit_Register(a.m_Value + b);
            return result;
        }
        public static _24Bit_Register operator -(_24Bit_Register a, int b)
        {
            _24Bit_Register result = new _24Bit_Register(a.m_Value - b);
            return result;
        }
        public static _24Bit_Register operator |(_24Bit_Register a, int b)
        {
            _24Bit_Register result = new _24Bit_Register(a.m_Value | (uint)b);
            return result;
        }
        public static _24Bit_Register operator &(_24Bit_Register a, int b)
        {
            _24Bit_Register result = new _24Bit_Register(a.m_Value & b);
            return result;
        }

        public static _24Bit_Register operator +(_24Bit_Register a, _24Bit_Register b)
        {
            _24Bit_Register result = new _24Bit_Register(a.m_Value + b.m_Value);
            return result;
        }
        public static _24Bit_Register operator -(_24Bit_Register a, _24Bit_Register b)
        {
            _24Bit_Register result = new _24Bit_Register(a.m_Value - b.m_Value);
            return result;
        }
        public static _24Bit_Register operator |(_24Bit_Register a, _24Bit_Register b)
        {
            _24Bit_Register result = new _24Bit_Register(a.m_Value | b.m_Value);
            return result;
        }
        public static _24Bit_Register operator &(_24Bit_Register a, _24Bit_Register b)
        {
            _24Bit_Register result = new _24Bit_Register(a.m_Value & b.m_Value);
            return result;
        }

        public static _24Bit_Register operator ++(_24Bit_Register a)
        {
            a.m_Value++;
            return a;
        }
        public static _24Bit_Register operator --(_24Bit_Register a)
        {
            a.m_Value--;
            return a;
        }

        public static implicit operator _24Bit_Register(uint value)
        {
            return new _24Bit_Register(value);
        }
        public static implicit operator _24Bit_Register(int value)
        {
            return new _24Bit_Register(Convert.ToUInt32(value));
        }

        public static implicit operator string(_24Bit_Register register)
        {
            return register.ToHex();
        }

        public static implicit operator int(_24Bit_Register register)
        {
            return (int)register.m_Value;
        }

        public static implicit operator uint(_24Bit_Register register)
        {
            return register.m_Value;
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
            return Convert.ToString(m_Value, 16).PadLeft(6, '0');
        }
    }
}
