﻿public enum TokenType
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
    numberSign          = '#',

    none = 0x17F,

    int_lit,
    ident,

    _struct,

    end,
    _asm,
    _sizeof,
    call,

    _exit,

    _res,

    _if,
    _elif,
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

    invoke,

    _extern,

    _nearPointer,
    _shortPointer,
    _longPointer,
    _farPointer,

    _void,
    function,
    program,

    define,

    neg,
    dec,
    inc,
    Section,
    SectionText,
    SectionData,
    SectionString,
    display,
}