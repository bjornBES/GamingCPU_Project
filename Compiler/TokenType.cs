public enum TokenType
{
    quotation_mark      = '\"',
    open_paren          = '(',
    close_paren         = ')',
    semi                = ';',
    eq                  = '=',
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

    none = 0x17F,

    int_lit,
    ident,

    _struct,

    end,
    _asm,
    _sizeof,
    call,

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

    _nearPointer,
    _shortPointer,
    _longPointer,
    _farPointer,

    _void,
    function,
    program,

    neg,
    dec,
    inc,
    Section,
    SectionText,
    SectionData,
    SectionString,
}