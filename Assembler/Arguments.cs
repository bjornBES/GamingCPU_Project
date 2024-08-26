namespace assembler.global
{
    public enum Arguments
    {
        none,

        AX,
        BX,
        CX,
        DX,
        MB,

        BP,

        FA,
        FB,

        HL,

        imm8,
        imm16,
        imm24,
        imm32,
        f_imm,
        address,
        long_address,
        reg8,
        reg16,
        reg32,
        f_reg,
        reg,
        reg_addr,
        segment_reg_imm,
        segment_imm_reg,
        segment_reg_reg,

        _string,
    }
}