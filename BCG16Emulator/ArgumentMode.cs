namespace BCG16CPUEmulator
{
    public enum ArgumentMode
    {
        immediate_byte = 0x00,
        immediate_word = 0x01,
        immediate_tbyte = 0x02,
        immediate_dword = 0x03,
        immediate_qword = 0x06,
        immediate_float = 0x07,

        register = 0x08,
        register_address = 0x09,

        near_address = 0x0B,
        address = 0x0C,
        long_address = 0x0D,
        far_address = 0x0E,
        relative_address = 0x0F,

        segment_address = 0x10,
        segment_address_immediate = 0x11,
        segment_DS_register = 0x12,
        segment_DS_B = 0x13,

        register_AL = 0x1A,
        register_A = 0x1B,
        register_AX = 0x1C,
        register_HL = 0x1D,
        register_address_HL = 0x1E,
        BP_Offset_Address = 0x1F,

        None = 0xFF,
    }
}