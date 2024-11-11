using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Lib.Variants
{
#nullable enable
    public class Variant<T1, T2>
    {
        T2? m_V2;
        T1? m_V1;

        public Variant()
        {
        }
        public Variant(T1 value)
        {
            m_V1 = value;
        }
        public Variant(T2 value)
        {
            m_V2 = value;
        }

        // Implicit conversion from T1
        public static implicit operator Variant<T1, T2>(T1 value)
        {
            return new Variant<T1, T2>(value);
        }

        // Implicit conversion from T2
        public static implicit operator Variant<T1, T2>(T2 value)
        {
            return new Variant<T1, T2>(value);
        }

#nullable disable
        public object Get<R>()
        {
            object _out;
            if (CheckAndOut<R>(m_V1, out _out))
                return _out;
            else if (CheckAndOut<R>(m_V2, out _out))
                return _out;
            else
            {
                return null;
            }
        }
        public object Get<R>(out R __out)
        {
            object _out;
            if (CheckAndOut<R>(m_V1, out _out))
            {
                __out = (R)_out;
                return _out;
            }
            else if (CheckAndOut<R>(m_V2, out _out))
            {
                __out = (R)_out;
                return _out;
            }
            else
            {
                __out = default(R);
                return null;
            }
        }

        bool CheckAndOut<R>(object V, out object _out)
        {
            if (V != null)
            {
                if (V.GetType() == typeof(R))
                {
                    _out = V;
                    return true;
                }
            }
            _out = null;
            return false;
        }
    }
#nullable enable
    public class Variant<T1, T2, T3>
    {
        T3? m_V3;
        T2? m_V2;
        T1? m_V1;

        public Variant()
        {
        }
        public Variant(T1 value)
        {
            m_V1 = value;
        }
        public Variant(T2 value)
        {
            m_V2 = value;
        }
        public Variant(T3 value)
        {
            m_V3 = value;
        }

        // Implicit conversion from T1
        public static implicit operator Variant<T1, T2, T3>(T1 value)
        {
            return new Variant<T1, T2, T3>(value);
        }

        // Implicit conversion from T2
        public static implicit operator Variant<T1, T2, T3>(T2 value)
        {
            return new Variant<T1, T2, T3>(value);
        }

        // Implicit conversion from T3
        public static implicit operator Variant<T1, T2, T3>(T3 value)
        {
            return new Variant<T1, T2, T3>(value);
        }
#nullable disable
        public object Get<R>()
        {
            if (m_V1 != null)
            {
                if (typeof(R) == m_V1.GetType())
                {
                    return m_V1;
                }
            }
            else if (m_V2 != null)
            {
                if (typeof(R) == m_V2.GetType())
                {
                    return m_V2;
                }
            }
            else if (m_V3 != null)
            {
                if (typeof(R) == m_V3.GetType())
                {
                    return m_V3;
                }
            }
            else
            {
                return null;
            }
            return null;
        }
    }
#nullable enable
    public class Variant<T1, T2, T3, T4>
    {
        T4? m_V4;
        T3? m_V3;
        T2? m_V2;
        T1? m_V1;

        public Variant(T1 value) => m_V1 = value;
        public Variant(T2 value) => m_V2 = value;
        public Variant(T3 value) => m_V3 = value;
        public Variant(T4 value) => m_V4 = value;

        public static implicit operator Variant<T1, T2, T3, T4>(T1 value) => new(value);
        public static implicit operator Variant<T1, T2, T3, T4>(T2 value) => new(value);
        public static implicit operator Variant<T1, T2, T3, T4>(T3 value) => new(value);
        public static implicit operator Variant<T1, T2, T3, T4>(T4 value) => new(value);
#nullable disable
        public object Get<R>()
        {
            object _out;
            if (CheckAndOut<R>(m_V1, out _out))
                return _out;
            else if (CheckAndOut<R>(m_V2, out _out))
                return _out;
            else if (CheckAndOut<R>(m_V3, out _out))
                return _out;
            else if (CheckAndOut<R>(m_V4, out _out))
                return _out;
            else
            {
                return null;
            }
        }

        bool CheckAndOut<R>(object V, out object _out)
        {
            if (V != null)
            {
                if (V.GetType() == typeof(R))
                {
                    _out = V;
                    return true;
                }
            }
            _out = null;
            return false;
        }
    }
#nullable enable
    public class Variant<T1, T2, T3, T4, T5>
    {
        T5? m_V5;
        T4? m_V4;
        T3? m_V3;
        T2? m_V2;
        T1? m_V1;

        public Variant(T1 value) => m_V1 = value;
        public Variant(T2 value) => m_V2 = value;
        public Variant(T3 value) => m_V3 = value;
        public Variant(T4 value) => m_V4 = value;
        public Variant(T5 value) => m_V5 = value;

        public static implicit operator Variant<T1, T2, T3, T4, T5>(T1 value) => new(value);
        public static implicit operator Variant<T1, T2, T3, T4, T5>(T2 value) => new(value);
        public static implicit operator Variant<T1, T2, T3, T4, T5>(T3 value) => new(value);
        public static implicit operator Variant<T1, T2, T3, T4, T5>(T4 value) => new(value);
        public static implicit operator Variant<T1, T2, T3, T4, T5>(T5 value) => new(value);
#nullable disable
        public object Get<R>()
        {
            object _out;
            if (CheckAndOut<R>(m_V1, out _out))
                return _out;
            else if (CheckAndOut<R>(m_V2, out _out))
                return _out;
            else if (CheckAndOut<R>(m_V3, out _out))
                return _out;
            else if (CheckAndOut<R>(m_V4, out _out))
                return _out;
            else if (CheckAndOut<R>(m_V5, out _out))
                return _out;
            else
            {
                return null;
            }
        }

        bool CheckAndOut<R>(object V, out object _out)
        {
            if (V != null)
            {
                if (V.GetType() == typeof(R))
                {
                    _out = V;
                    return true;
                }
            }
            _out = null;
            return false;
        }
    }
#nullable enable
    public class Variant<T1, T2, T3, T4, T5, T6>
    {
        T6? m_V6;
        T5? m_V5;
        T4? m_V4;
        T3? m_V3;
        T2? m_V2;
        T1? m_V1;

        public Variant(T1 value) => m_V1 = value;
        public Variant(T2 value) => m_V2 = value;
        public Variant(T3 value) => m_V3 = value;
        public Variant(T4 value) => m_V4 = value;
        public Variant(T5 value) => m_V5 = value;
        public Variant(T6 value) => m_V6 = value;

        public static implicit operator Variant<T1, T2, T3, T4, T5, T6>(T1 value) => new(value);
        public static implicit operator Variant<T1, T2, T3, T4, T5, T6>(T2 value) => new(value);
        public static implicit operator Variant<T1, T2, T3, T4, T5, T6>(T3 value) => new(value);
        public static implicit operator Variant<T1, T2, T3, T4, T5, T6>(T4 value) => new(value);
        public static implicit operator Variant<T1, T2, T3, T4, T5, T6>(T5 value) => new(value);
        public static implicit operator Variant<T1, T2, T3, T4, T5, T6>(T6 value) => new(value);
#nullable disable
        public object Get<R>()
        {
            object _out;
            if (CheckAndOut<R>(m_V1, out _out))
                return _out;
            else if (CheckAndOut<R>(m_V2, out _out))
                return _out;
            else if (CheckAndOut<R>(m_V3, out _out))
                return _out;
            else if (CheckAndOut<R>(m_V4, out _out))
                return _out;
            else if (CheckAndOut<R>(m_V5, out _out))
                return _out;
            else if (CheckAndOut<R>(m_V6, out _out))
                return _out;
            else
            {
                return null;
            }
        }
        public object Get<R>(out R __out)
        {
            object _out;
            if (CheckAndOut<R>(m_V1, out _out))
            {
                __out = (R)_out;
                return _out;
            }
            else if (CheckAndOut<R>(m_V2, out _out))
            {
                __out = (R)_out;
                return _out;
            }
            else if (CheckAndOut<R>(m_V3, out _out))
            {
                __out = (R)_out;
                return _out;
            }
            else if (CheckAndOut<R>(m_V4, out _out))
            {
                __out = (R)_out;
                return _out;
            }
            else if (CheckAndOut<R>(m_V5, out _out))
            {
                __out = (R)_out;
                return _out;
            }
            else if (CheckAndOut<R>(m_V6, out _out))
            {
                __out = (R)_out;
                return _out;
            }
            else
            {
                __out = default(R);
                return null;
            }
        }

        bool CheckAndOut<R>(object V, out object _out)
        {
            if (V != null)
            {
                if (V.GetType() == typeof(R))
                {
                    _out = V;
                    return true;
                }
            }
            _out = null;
            return false;
        }
    }
#nullable enable
    public class Variant<T1, T2, T3, T4, T5, T6, T7>
    {
        T7? m_V7;
        T6? m_V6;
        T5? m_V5;
        T4? m_V4;
        T3? m_V3;
        T2? m_V2;
        T1? m_V1;

        public Variant(T1 value) => m_V1 = value;
        public Variant(T2 value) => m_V2 = value;
        public Variant(T3 value) => m_V3 = value;
        public Variant(T4 value) => m_V4 = value;
        public Variant(T5 value) => m_V5 = value;
        public Variant(T6 value) => m_V6 = value;
        public Variant(T7 value) => m_V7 = value;

        public static implicit operator Variant<T1, T2, T3, T4, T5, T6, T7>(T1 value) => new(value);
        public static implicit operator Variant<T1, T2, T3, T4, T5, T6, T7>(T2 value) => new(value);
        public static implicit operator Variant<T1, T2, T3, T4, T5, T6, T7>(T3 value) => new(value);
        public static implicit operator Variant<T1, T2, T3, T4, T5, T6, T7>(T4 value) => new(value);
        public static implicit operator Variant<T1, T2, T3, T4, T5, T6, T7>(T5 value) => new(value);
        public static implicit operator Variant<T1, T2, T3, T4, T5, T6, T7>(T6 value) => new(value);
        public static implicit operator Variant<T1, T2, T3, T4, T5, T6, T7>(T7 value) => new(value);
#nullable disable
        public object Get<R>()
        {
            object _out;
            if (CheckAndOut<R>(m_V1, out _out))
                return _out;
            else if (CheckAndOut<R>(m_V2, out _out))
                return _out;
            else if (CheckAndOut<R>(m_V3, out _out))
                return _out;
            else if (CheckAndOut<R>(m_V4, out _out))
                return _out;
            else if (CheckAndOut<R>(m_V5, out _out))
                return _out;
            else if (CheckAndOut<R>(m_V6, out _out))
                return _out;
            else if (CheckAndOut<R>(m_V7, out _out))
                return _out;
            else
            {
                return null;
            }
        }

        public object Get<R>(out R __out)
        {
            object _out;
            if (CheckAndOut<R>(m_V1, out _out))
            {
                __out = (R)_out;
                return _out;
            }
            else if (CheckAndOut<R>(m_V2, out _out))
            {
                __out = (R)_out;
                return _out;
            }
            else if (CheckAndOut<R>(m_V3, out _out))
            {
                __out = (R)_out;
                return _out;
            }
            else if (CheckAndOut<R>(m_V4, out _out))
            {
                __out = (R)_out;
                return _out;
            }
            else if (CheckAndOut<R>(m_V5, out _out))
            {
                __out = (R)_out;
                return _out;
            }
            else if (CheckAndOut<R>(m_V6, out _out))
            {
                __out = (R)_out;
                return _out;
            }
            else if (CheckAndOut<R>(m_V7, out _out))
            {
                __out = (R)_out;
                return _out;
            }
            else
            {
                __out = default(R);
                return null;
            }
        }

        bool CheckAndOut<R>(object V, out object _out)
        {
            if (V != null)
            {
                if (V.GetType() == typeof(R))
                {
                    _out = V;
                    return true;
                }
            }
            _out = null;
            return false;
        }
    }
#nullable enable
    public class Variant<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        T8? m_V8;
        T7? m_V7;
        T6? m_V6;
        T5? m_V5;
        T4? m_V4;
        T3? m_V3;
        T2? m_V2;
        T1? m_V1;

        public Variant(T1 value) => m_V1 = value;
        public Variant(T2 value) => m_V2 = value;
        public Variant(T3 value) => m_V3 = value;
        public Variant(T4 value) => m_V4 = value;
        public Variant(T5 value) => m_V5 = value;
        public Variant(T6 value) => m_V6 = value;
        public Variant(T7 value) => m_V7 = value;
        public Variant(T8 value) => m_V8 = value;

        public static implicit operator Variant<T1, T2, T3, T4, T5, T6, T7, T8>(T1 value) => new(value);
        public static implicit operator Variant<T1, T2, T3, T4, T5, T6, T7, T8>(T2 value) => new(value);
        public static implicit operator Variant<T1, T2, T3, T4, T5, T6, T7, T8>(T3 value) => new(value);
        public static implicit operator Variant<T1, T2, T3, T4, T5, T6, T7, T8>(T4 value) => new(value);
        public static implicit operator Variant<T1, T2, T3, T4, T5, T6, T7, T8>(T5 value) => new(value);
        public static implicit operator Variant<T1, T2, T3, T4, T5, T6, T7, T8>(T6 value) => new(value);
        public static implicit operator Variant<T1, T2, T3, T4, T5, T6, T7, T8>(T7 value) => new(value);
        public static implicit operator Variant<T1, T2, T3, T4, T5, T6, T7, T8>(T8 value) => new(value);
#nullable disable
        public object Get<R>()
        {
            object _out;
            if (CheckAndOut<R>(m_V1, out _out))
                return _out;
            else if (CheckAndOut<R>(m_V2, out _out))
                return _out;
            else if (CheckAndOut<R>(m_V3, out _out))
                return _out;
            else if (CheckAndOut<R>(m_V4, out _out))
                return _out;
            else if (CheckAndOut<R>(m_V5, out _out))
                return _out;
            else if (CheckAndOut<R>(m_V6, out _out))
                return _out;
            else if (CheckAndOut<R>(m_V7, out _out))
                return _out;
            else if (CheckAndOut<R>(m_V8, out _out))
                return _out;
            else
            {
                return null;
            }
        }

        public object Get<R>(out R __out)
        {
            object _out;
            if (CheckAndOut<R>(m_V1, out _out))
            {
                __out = (R)_out;
                return _out;
            }
            else if (CheckAndOut<R>(m_V2, out _out))
            {
                __out = (R)_out;
                return _out;
            }
            else if (CheckAndOut<R>(m_V3, out _out))
            {
                __out = (R)_out;
                return _out;
            }
            else if (CheckAndOut<R>(m_V4, out _out))
            {
                __out = (R)_out;
                return _out;
            }
            else if (CheckAndOut<R>(m_V5, out _out))
            {
                __out = (R)_out;
                return _out;
            }
            else if (CheckAndOut<R>(m_V6, out _out))
            {
                __out = (R)_out;
                return _out;
            }
            else if (CheckAndOut<R>(m_V7, out _out))
            {
                __out = (R)_out;
                return _out;
            }
            else if (CheckAndOut<R>(m_V8, out _out))
            {
                __out = (R)_out;
                return _out;
            }
            else
            {
                __out = default(R);
                return null;
            }
        }

        bool CheckAndOut<R>(object V, out object _out)
        {
            if (V != null)
            {
                if (V.GetType() == typeof(R))
                {
                    _out = V;
                    return true;
                }
            }
            _out = null;
            return false;
        }
    }
#nullable enable
    public class Variant<T1, T2, T3, T4, T5, T6, T7, T8, T9>
    {
        T9? m_V9;
        T8? m_V8;
        T7? m_V7;
        T6? m_V6;
        T5? m_V5;
        T4? m_V4;
        T3? m_V3;
        T2? m_V2;
        T1? m_V1;

        public Variant(T1 value) => m_V1 = value;
        public Variant(T2 value) => m_V2 = value;
        public Variant(T3 value) => m_V3 = value;
        public Variant(T4 value) => m_V4 = value;
        public Variant(T5 value) => m_V5 = value;
        public Variant(T6 value) => m_V6 = value;
        public Variant(T7 value) => m_V7 = value;
        public Variant(T8 value) => m_V8 = value;
        public Variant(T9 value) => m_V9 = value;

        public static implicit operator Variant<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 value) => new(value);
        public static implicit operator Variant<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T2 value) => new(value);
        public static implicit operator Variant<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T3 value) => new(value);
        public static implicit operator Variant<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T4 value) => new(value);
        public static implicit operator Variant<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T5 value) => new(value);
        public static implicit operator Variant<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T6 value) => new(value);
        public static implicit operator Variant<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T7 value) => new(value);
        public static implicit operator Variant<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T8 value) => new(value);
        public static implicit operator Variant<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T9 value) => new(value);
#nullable disable
        public object Get<R>()
        {
            object _out;
            if (CheckAndOut<R>(m_V1, out _out)) return _out;
            else if (CheckAndOut<R>(m_V2, out _out)) return _out;
            else if (CheckAndOut<R>(m_V3, out _out)) return _out;
            else if (CheckAndOut<R>(m_V4, out _out)) return _out;
            else if (CheckAndOut<R>(m_V5, out _out)) return _out;
            else if (CheckAndOut<R>(m_V6, out _out)) return _out;
            else if (CheckAndOut<R>(m_V7, out _out)) return _out;
            else if (CheckAndOut<R>(m_V8, out _out)) return _out;
            else if (CheckAndOut<R>(m_V9, out _out)) return _out;
            else
            {
                return null;
            }
        }

        bool CheckAndOut<R>(object V, out object _out)
        {
            if (V != null)
            {
                if (V.GetType() == typeof(R))
                {
                    _out = V;
                    return true;
                }
            }
            _out = null;
            return false;
        }
    }
}
