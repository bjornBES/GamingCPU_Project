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

    none,

    int_lit,
    hex_lit,
    bin_lit,
    ident,

    _struct,

    end,
    _asm,
    _sizeof,
    call,

    _return,

    _public,

    _char,
    _byte,
    _sbyte,
    _short,
    _ushort,
    tbyte,
    pointer,
    _int,
    size_t,
    _string,
    _const,

    _void,

    neg,
    dec,
    inc,

    _IN_,
    _OUT_
};
