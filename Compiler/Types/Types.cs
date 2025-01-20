public abstract class ExprType
{
    protected ExprType(bool isConst, bool isVolatile)
    {
        IsConst = isConst;
        IsVolatile = isVolatile;
    }

    public abstract ExprTypeKind Kind { get; }

    public virtual bool IsArith => false;

    public virtual bool IsIntegral => false;

    public virtual bool IsScalar => false;

    public virtual bool IsComplete => true;

    public abstract bool EqualType(ExprType other);

    public abstract TypeSize SizeOf { get; }
    public abstract TypeSize Alignment { get; }

    public readonly bool IsConst;
    public readonly bool IsVolatile;

    public string DumpQualifiers()
    {
        string str = "";
        if (IsConst)
        {
            str += "const ";
        }
        if (IsVolatile)
        {
            str += "volatile ";
        }
        return str;
    }

    public abstract ExprType GetQualifiedType(bool isConst, bool isVolatile);

}

public abstract class ScalarType : ExprType
{
    protected ScalarType(bool isConst, bool isVolatile)
        : base(isConst, isVolatile) { }
    public override bool IsScalar => true;
}

public abstract class ArithmeticType : ScalarType
{
    protected ArithmeticType(bool isConst, bool isVolatile)
        : base(isConst, isVolatile) { }
    public override bool IsArith => true;
    public override bool EqualType(ExprType other) => Kind == other.Kind;
}

public abstract class IntegralType : ArithmeticType
{
    protected IntegralType(bool isConst, bool isVolatile)
        : base(isConst, isVolatile) { }
    public override bool IsIntegral => true;
}
public partial class VoidType : ExprType
{
    public VoidType(bool isConst = false, bool isVolatile = false)
    : base(isConst, isVolatile)
    {
    }
    public override ExprTypeKind Kind => ExprTypeKind.VOID;

    public override TypeSize SizeOf => TypeSize.SHORT_POINTER;

    public override TypeSize Alignment => TypeSize.SHORT_POINTER;


    public override ExprType GetQualifiedType(bool isConst, bool isVolatile) =>
        new VoidType(isConst, isVolatile);

    public override bool EqualType(ExprType other) => other.Kind == ExprTypeKind.VOID;
}
public partial class CharType : IntegralType
{
    public CharType(bool isConst = false, bool isVolatile = false)
        : base(isConst, isVolatile) { }

    public override ExprTypeKind Kind => ExprTypeKind.CHAR;

    public override TypeSize SizeOf => TypeSize.CHAR;

    public override TypeSize Alignment => TypeSize.CHAR;

    public override ExprType GetQualifiedType(bool isConst, bool isVolatile) =>
        new CharType(isConst, isVolatile);
}

public partial class UCharType : IntegralType
{
    public UCharType(bool isConst = false, bool isVolatile = false)
        : base(isConst, isVolatile) { }

    public override ExprTypeKind Kind => ExprTypeKind.UCHAR;

    public override TypeSize SizeOf => TypeSize.CHAR;

    public override TypeSize Alignment => TypeSize.CHAR;

    public override ExprType GetQualifiedType(bool isConst, bool isVolatile) =>
        new UCharType(isConst, isVolatile);
}

public partial class ShortType : IntegralType
{
    public ShortType(bool isConst = false, bool isVolatile = false)
        : base(isConst, isVolatile) { }

    public override ExprTypeKind Kind => ExprTypeKind.SHORT;

    public override TypeSize SizeOf => TypeSize.SHORT;

    public override TypeSize Alignment => TypeSize.SHORT;

    public override ExprType GetQualifiedType(bool isConst, bool isVolatile) =>
        new ShortType(isConst, isVolatile);
}

public partial class UShortType : IntegralType
{
    public UShortType(bool isConst = false, bool isVolatile = false)
        : base(isConst, isVolatile) { }

    public override ExprTypeKind Kind => ExprTypeKind.USHORT;

    public override TypeSize SizeOf => TypeSize.SHORT;

    public override TypeSize Alignment => TypeSize.SHORT;

    public override ExprType GetQualifiedType(bool isConst, bool isVolatile) =>
        new UShortType(isConst, isVolatile);
}

public partial class IntType : IntegralType
{
    public IntType(bool isConst = false, bool isVolatile = false)
        : base(isConst, isVolatile) { }

    public override ExprTypeKind Kind => ExprTypeKind.INT;

    public override TypeSize SizeOf => TypeSize.INT;

    public override TypeSize Alignment => TypeSize.INT;

    public override ExprType GetQualifiedType(bool isConst, bool isVolatile)
    {
        return new IntType(isConst, isVolatile);
    }
}

public partial class UIntType : IntegralType
{
    public UIntType(bool isConst = false, bool isVolatile = false)
        : base(isConst, isVolatile) { }

    public override ExprTypeKind Kind => ExprTypeKind.UINT;

    public override TypeSize SizeOf => TypeSize.INT;

    public override TypeSize Alignment => TypeSize.INT;

    public override ExprType GetQualifiedType(bool isConst, bool isVolatile)
    {
        return new UIntType(isConst, isVolatile);
    }
}

public partial class FloatType : ArithmeticType
{
    public FloatType(bool isConst = false, bool isVolatile = false)
        : base(isConst, isVolatile) { }

    public override ExprTypeKind Kind => ExprTypeKind.FLOAT;

    public override TypeSize SizeOf => TypeSize.FLOAT;

    public override TypeSize Alignment => TypeSize.FLOAT;

    public override ExprType GetQualifiedType(bool isConst, bool isVolatile) =>
        new FloatType(isConst, isVolatile);
}

public partial class NearPointerType : ScalarType
{
    public NearPointerType(ExprType refType, bool isConst = false, bool isVolatile = false)
        : base(isConst, isVolatile)
    {
        RefType = refType;
    }

    public override ExprTypeKind Kind => ExprTypeKind.POINTER;

    public override TypeSize SizeOf => TypeSize.NEAR_POINTER;

    public override TypeSize Alignment => TypeSize.NEAR_POINTER;


    public ExprType RefType { get; set; }

    public override ExprType GetQualifiedType(bool isConst, bool isVolatile) =>
        new NearPointerType(RefType, isConst, isVolatile);

    public override bool EqualType(ExprType other) =>
        other.Kind == ExprTypeKind.POINTER && ((NearPointerType)other).RefType.EqualType(RefType);
}
public partial class ShortPointerType : ScalarType
{
    public ShortPointerType(ExprType refType, bool isConst = false, bool isVolatile = false)
        : base(isConst, isVolatile)
    {
        RefType = refType;
    }

    public override ExprTypeKind Kind => ExprTypeKind.POINTER;

    public override TypeSize SizeOf => TypeSize.FAR_POINTER;

    public override TypeSize Alignment => TypeSize.FAR_POINTER;


    public ExprType RefType { get; set; }

    public override ExprType GetQualifiedType(bool isConst, bool isVolatile) =>
        new ShortPointerType(RefType, isConst, isVolatile);

    public override bool EqualType(ExprType other) =>
        other.Kind == ExprTypeKind.POINTER && ((ShortPointerType)other).RefType.EqualType(RefType);
}
public partial class LongPointerType : ScalarType
{
    public LongPointerType(ExprType refType, bool isConst = false, bool isVolatile = false)
        : base(isConst, isVolatile)
    {
        RefType = refType;
    }

    public override ExprTypeKind Kind => ExprTypeKind.POINTER;

    public override TypeSize SizeOf => TypeSize.LONG_POINTER;

    public override TypeSize Alignment => TypeSize.FAR_POINTER;


    public ExprType RefType { get; set; }

    public override ExprType GetQualifiedType(bool isConst, bool isVolatile) =>
        new LongPointerType(RefType, isConst, isVolatile);

    public override bool EqualType(ExprType other) =>
        other.Kind == ExprTypeKind.POINTER && ((LongPointerType)other).RefType.EqualType(RefType);
}
public partial class FarPointerType : ScalarType
{
    public FarPointerType(ExprType refType, bool isConst = false, bool isVolatile = false)
        : base(isConst, isVolatile)
    {
        RefType = refType;
    }

    public override ExprTypeKind Kind => ExprTypeKind.POINTER;

    public override TypeSize SizeOf => TypeSize.FAR_POINTER;

    public override TypeSize Alignment => TypeSize.FAR_POINTER;


    public ExprType RefType { get; set; }

    public override ExprType GetQualifiedType(bool isConst, bool isVolatile) =>
        new FarPointerType(RefType, isConst, isVolatile);

    public override bool EqualType(ExprType other) =>
        other.Kind == ExprTypeKind.POINTER && ((FarPointerType)other).RefType.EqualType(RefType);
}

public partial class IncompleteArrayType : ExprType
{
    public IncompleteArrayType(ExprType elemType)
        : base(elemType.IsConst, elemType.IsVolatile)
    {
        ElemType = elemType;
    }

    public override ExprTypeKind Kind => ExprTypeKind.INCOMPLETE_ARRAY;

    public override TypeSize SizeOf
    {
        get
        {
            throw new InvalidOperationException("Incomplete array. Cannot get sizeof.");
        }
    }

    public override TypeSize Alignment => ElemType.Alignment;

    public override ExprType GetQualifiedType(bool isConst, bool isVolatile)
    {
        throw new InvalidOperationException("An array has the same cv qualifiers of its elems.");
    }

    public override bool EqualType(ExprType other) => false;

    public override bool IsComplete => false;

    public ExprType Complete(int numElems) => new ArrayType(ElemType, numElems);

    public ExprType ElemType { get; set; }
}

public partial class ArrayType : ExprType
{
    public ArrayType(ExprType elemType, int numElems)
        : base(elemType.IsConst, elemType.IsVolatile)
    {
        ElemType = elemType;
        NumElems = numElems;
    }

    public override ExprTypeKind Kind => ExprTypeKind.ARRAY;

    public int SizeOfArray() => (int)ElemType.SizeOf * NumElems;

    public override TypeSize Alignment => ElemType.Alignment;

    public override TypeSize SizeOf => (TypeSize)SizeOfArray();

    public readonly ExprType ElemType;

    public readonly int NumElems;

    public override ExprType GetQualifiedType(bool isConst, bool isVolatile)
    {
        throw new InvalidOperationException("An array has the same cv qualifiers of its elems.");
    }

    public override bool EqualType(ExprType other) =>
        other.Kind == ExprTypeKind.ARRAY && ((ArrayType)other).ElemType.EqualType(ElemType);
}