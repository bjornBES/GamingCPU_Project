public enum TokenType
{

    assign              = '=',
    // -=
    subassign,
    // +=
    addassign,
    // &=
    andassign,
    // *=
    multassign,
    // <<=
    lshiftassign,
    // >>=
    rshiftassign,
    // |=
    orassign,
    // /=
    divassign,
    // %=
    modassign,
    // ^=
    xorassign,

    // ==
    eq,
    // !=
    neq,
    // <=
    leq,
    // >=
    geq,
    
    // ++
    dec,
    // --
    inc,

    // &&
    and,
    // ||
    or,
    // <<
    lshift,
    // >>
    rshift,
    
    sub                 = '-',
    add                 = '+',
    bitand              = '&',
    mult                = '*',
    lt                  = '<',
    gt                  = '>',
    not                 = '!',
    div                 = '/',
    mod                 = '%',
    xor                 = '^',
    bitor               = '|',
    semicolon           = ';',

    quotation_mark      = '\"',
    open_paren          = '(',
    close_paren         = ')',
    plus                = '+',
    star                = '*',
    minus               = '-',
    fslash              = '/',
    bslash              = '\\',
    open_curly          = '{',
    close_curly         = '}',
    open_square         = '[',
    close_square        = ']',
    colon               = ':',
    period              = '.',
    ampersand           = '&',
    caret               = '^',
    tilde               = '~',
    dollar_sign         = '$',
    comma               = ',',
    at                  = '@',
    vpipe               = '|',
    exclamationmark     = '!',
    leftanglebracket    = '<',
    rightanglebracket   = '>',
    numberSign          = '#',
    question            = '?',

    none = 0x17F,

    int_lit,
    ident,
    line_number,

    _struct,
    rarrow,
    end,
    endfunc,
    _asm,
    _sizeof,
    call,

    _exit,
    _is,
    _res,

    _if,
    _else,
    _then,

    _return,

    _while,

    uint8,
    int8,
    
    int16,
    uint16,
    
    int24,
    uint24,
    
    int32,
    uint32,

    _const,
    _public,
    _volatile,

    invoke,

    _extern,

    array,

    _break,
    _continue,

    pointer,

    _nearPointer,
    _shortPointer,
    _longPointer,
    _farPointer,

    _void,
    function,
    program,

    define,
    defineD,

    DIf,
    DEndIf,
    neg,
    Section,
    SectionText,
    SectionData,
    SectionString,
    display,
}