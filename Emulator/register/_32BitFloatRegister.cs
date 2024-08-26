using System.Runtime.InteropServices;
using System;

namespace emulator.register
{
    [Serializable]
    [ComVisible(true)]
    public struct _32BitFloatRegister
    {
        public float m_Value;

        public _32BitFloatRegister(float value)
        {
            m_Value = value;
        }

        public static bool operator !=(_32BitFloatRegister a, float b)
        {
            return a.m_Value != b;
        }
        public static bool operator ==(_32BitFloatRegister a, float b)
        {
            return a.m_Value == b;
        }

        public static float operator +(_32BitFloatRegister a, float b)
        {
            return (float)(a + b);
        }
        public static float operator -(_32BitFloatRegister a, float b)
        {
            return (float)(a - b);
        }

        public static _32BitFloatRegister operator ++(_32BitFloatRegister a)
        {
            a.m_Value++;
            return a;
        }
        public static _32BitFloatRegister operator --(_32BitFloatRegister a)
        {
            a.m_Value--;
            return a;
        }

        public static implicit operator _32BitFloatRegister(uint value)
        {
            return new _32BitFloatRegister(value);
        }
        public static implicit operator _32BitFloatRegister(int value)
        {
            return new _32BitFloatRegister(Convert.ToUInt32(value));
        }

        public static implicit operator float(_32BitFloatRegister register)
        {
            return register.m_Value;
        }

        public static implicit operator int(_32BitFloatRegister register)
        {
            return Convert.ToInt32(register.m_Value);
        }

        public override string ToString()
        {
            int floatToInt = BitConverter.ToInt32(BitConverter.GetBytes(m_Value), 0);
            return Convert.ToString(floatToInt, 2).PadLeft(32, ' ');
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
