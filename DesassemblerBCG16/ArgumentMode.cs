using System;
using System.Collections.Generic;
using System.Text;

public enum ArgumentMode
{
    immediate_byte =        0x00,
    immediate_word =        0x01,
    immediate_tbyte =       0x02,
    immediate_dword =       0x03,
    immediate_float =       0x04,
    register =              0x10,
    register_address =      0x11,
    register_A =            0x18,
    register_B =            0x19,
    register_C =            0x1A,
    register_D =            0x1B,
    register_H =            0x1C,
    register_L =            0x1D,
    register_address_HL =   0x1E,
    register_MB =           0x1F,
    register_AX =           0x30,
    register_BX =           0x31,
    register_CX =           0x32,
    register_DX =           0x33,
    relative_address =      0x50,
    near_address =          0x51,
    short_address =         0x52,
    long_address =          0x53,
    far_address =           0x54,
    SP_Offset_Address =     0x58,
    BP_Offset_Address =     0x59,
    segment_address =       0x5A,
    segment_DS_register =   0x5B,
    segment_DS_B =          0x5C,
    segment_ES_register =   0x5D,


    register_AL,
}
