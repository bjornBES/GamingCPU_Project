using System;
using System.Collections.Generic;
using System.Text;

public enum ArgumentMode
{
    immediate_byte =        0x00,
    immediate_word =        0x01,
    immediate_tbyte =       0x02,
    immediate_dword =       0x03,
    register =              0x04,
    register_address =      0x05,
    near_address =          0x06,
    address =               0x07,
    long_address =          0x08,
    relative_address =      0x09,
    segment_address =       0x0A,
    segment_DS_register =   0x0B,
    segment_DS_B =          0x0C,
    SP_Offset_Address =     0x0D,
    BP_Offset_Address =     0x0E,
    immediate_float =       0x0F,
    far_address =           0x10,


    register_AL,
    register_A,
    register_AX,
    register_address_HL,
    register_HL,
}
