using System;
using System.Collections.Generic;
using System.Text;

public enum ArgumentMode
{
    immediate_byte = 0x00,
    immediate_word = 0x01,
    immediate_tbyte = 0x02,
    immediate_dword = 0x03,
    address = 0x04,
    register = 0x05,
    register_address = 0x06,

    near_address = 0x07,
    long_address = 0x08,
    far_address = 0x09,
    float_immediate = 0x0A,

    segment_address = 0x0C,
    segment_address_immediate = 0x0D,
    segment_immediate_address = 0x0E,
    segment_DS_register = 0x0F,

    immediate_qword = 0x1E,

    None = 0xFF
}
