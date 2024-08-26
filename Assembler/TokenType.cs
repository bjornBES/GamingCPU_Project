namespace assembler
{
    public enum TokenType
    {
        int_lit,
        address_lit,
        float_lit,
        ident,
        newline,

        instruction,
        register,
        reg_addr,
        address_label,
        _long,

        _struct,

        _byte,
        _word,
        _tbyte,
        _dword,
        _string,
        _float,
        org,

        rbyte,
        rword,
        rtbyte,
        rdword,

        text,
        data,
        rdata,
        bss,

        newfile,

        label,
        global,
        section,

        neg,
        pos,

        ampersand = '&',
        caret = '^',
        tilde = '~',
        dollar_sign = '$',
        quotation_mark = '\"',
        open_paren = '(',
        close_paren = ')',
        semi = ';',
        eq = '=',
        plus = '+',
        star = '*',
        minus = '-',
        fslash = '/',
        bslash = '\\',
        open_curly = '{',
        close_curly = '}',
        open_square = '[',
        close_square = ']',
        colon = ':',
        period = '.',
        comma = ',',
        at = '@',
        vpipe = '|',
        exclamationmark = '!',
        leftanglebracket = '<',
        rightanglebracket = '>',
    }
}
