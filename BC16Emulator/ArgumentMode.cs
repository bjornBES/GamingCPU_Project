﻿namespace BCG16CPUEmulator
{
    public enum ArgumentMode
    {
        immediate_byte                  = 0x00,
        immediate_word                  = 0x01,
        immediate_tbyte                 = 0x02,
        immediate_dword                 = 0x03,
        immediate_float                 = 0x04,
        register                        = 0x0A,
        register_address                = 0x0B,
        register_AL                     = 0x0C,
        register_BL                     = 0x0D,
        register_CL                     = 0x0E,
        register_DL                     = 0x0F,
        register_A                      = 0x10,
        register_B                      = 0x11,
        register_C                      = 0x12,
        register_D                      = 0x13,
        register_H                      = 0x14,
        register_L                      = 0x15,
        register_address_HL             = 0x16,
        register_MB                     = 0x17,
        register_AX                     = 0x20,
        register_BX                     = 0x21,
        register_CX                     = 0x22,
        register_DX                     = 0x23,
        relative_address                = 0x50,
        near_address                    = 0x51,
        short_address                   = 0x52,
        long_address                    = 0x53,
        X_indexed_address               = 0x55,
        Y_indexed_address               = 0x56,
        SP_rel_address_byte             = 0x60,
        BP_rel_address_byte             = 0x61,
        segment_address                 = 0x62,
        segment_DS_register             = 0x63,
        segment_DS_B                    = 0x64,
        segment_ES_register             = 0x65,
        segment_ES_B                    = 0x66,
        register_AF                     = 0x70,
        register_BF                     = 0x71,

        None,
    }
}