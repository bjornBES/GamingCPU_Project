using AssemblerBCG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum ArgumentModeOld
{
    none = -1,
    immediate_byte              = 0x00,
    immediate_word              = 0x01,
    immediate_tbyte             = 0x02,
    immediate_dword             = 0x03,
    immediate_qword             = 0x04,
    immediate_dqword            = 0x05,
    immediate_float             = 0x08,
    immediate_double            = 0x09,
    register                    = 0x10,
    register_AL                 = 0x11,
    register_BL                 = 0x12,
    register_CL                 = 0x13,
    register_DL                 = 0x14,
    register_H                  = 0x15,
    register_L                  = 0x16,
    register_A                  = 0x17,
    register_B                  = 0x18,
    register_C                  = 0x19,
    register_D                  = 0x1A,
    register_AX                 = 0x1B,
    register_BX                 = 0x1C,
    register_CX                 = 0x1D,
    register_DX                 = 0x1E,
    register_EX                 = 0x24,
    register_FX                 = 0x25,
    register_GX                 = 0x26,
    register_HX                 = 0x27,
    register_address            = 0x20,
    register_address_HL         = 0x21,
    relative_address            = 0x30,
    near_address                = 0x31,
    short_address               = 0x32,
    long_address                = 0x33,
    far_address                 = 0x34,
    X_indexed_address           = 0x36,
    Y_indexed_address           = 0x37,
    segment_address             = 0x3A,
    segment_DS_register         = 0x3B,
    segment_DS_B                = 0x3C,
    segment_ES_register         = 0x3D,
    segment_ES_B                = 0x3E,
    SP_rel_address_byte         = 0x40,
    BP_rel_address_byte         = 0x41,
    SP_rel_address_short        = 0x42,
    BP_rel_address_short        = 0x43,
    SPX_rel_address_tbyte       = 0x44,
    BPX_rel_address_tbyte       = 0x45,
    SPX_rel_address_word        = 0x46,
    BPX_rel_address_word        = 0x47,
    SPX_rel_address_int         = 0x48,
    BPX_rel_address_int         = 0x49,
    register_AF                 = 0x60,
    register_BF                 = 0x61,
    register_CF                 = 0x62,
    register_DF                 = 0x63,
    register_AD                 = 0x70,
    register_BD                 = 0x71,
    register_CD                 = 0x72,
    register_DD                 = 0x73,


    /*
     */
}

public enum ArgumentMode
{
    none = -1,

    immediate,
    immediate_float,
    register,
    GPregister,
    Sregister,
    Rregister,
    RM,
    MOD,
    /*
    immediate_byte = 0x00,
    immediate_word = 0x01,
    immediate_dword = 0x03,
    immediate_qword = 0x04,
    immediate_dqword = 0x05,
    immediate_float = 0x08,
    immediate_double = 0x09,
    register = 0x10,
    register_AL = 0x11,
    register_BL = 0x12,
    register_CL = 0x13,
    register_DL = 0x14,
    register_H = 0x15,
    register_L = 0x16,
    register_A = 0x17,
    register_B = 0x18,
    register_C = 0x19,
    register_D = 0x1A,
    register_AX = 0x1B,
    register_BX = 0x1C,
    register_CX = 0x1D,
    register_DX = 0x1E,
    register_EX = 0x24,
    register_FX = 0x25,
    register_GX = 0x26,
    register_HX = 0x27,
    register_address = 0x20,
    register_address_HL = 0x21,
    relative_address = 0x30,
    near_address = 0x31,
    short_address = 0x32,
    long_address = 0x33,
    far_address = 0x34,
    X_indexed_address = 0x36,
    Y_indexed_address = 0x37,
    segment_address = 0x3A,
    segment_DS_register = 0x3B,
    segment_DS_B = 0x3C,
    segment_ES_register = 0x3D,
    segment_ES_B = 0x3E,
    SP_rel_address_byte = 0x40,
    BP_rel_address_byte = 0x41,
    SP_rel_address_short = 0x42,
    BP_rel_address_short = 0x43,
    SPX_rel_address_tbyte = 0x44,
    BPX_rel_address_tbyte = 0x45,
    SPX_rel_address_word = 0x46,
    BPX_rel_address_word = 0x47,
    SPX_rel_address_int = 0x48,
    BPX_rel_address_int = 0x49,
    register_AF = 0x60,
    register_BF = 0x61,
    register_CF = 0x62,
    register_DF = 0x63,
    register_AD = 0x70,
    register_BD = 0x71,
    register_CD = 0x72,
    register_DD = 0x73,
     */
}

public struct OperandArgument
{
    public string data;
    public bool IsRawData;
    public ArgumentMode ArgumentMode;
    public SizeAlignment Size;

    public string GetDataBin()
    {
        string register;
        switch (ArgumentMode)
        {
            case ArgumentMode.immediate:
                switch (Size)
                {
                    case SizeAlignment._byte:
                        if (IsRawData)
                        {
                            return HexLibrary.HexConverter.ToBinString(data, 16).PadLeft(8, '0');
                        }
                        else
                        {
                            return data;
                        }
                    case SizeAlignment._word:
                        if (IsRawData)
                        {
                            string word = data;
                            return HexLibrary.HexConverter.ToBinString(word, 16).PadLeft(16, '0');
                        }
                        else
                        {
                            return data;
                        }
                    case SizeAlignment._dword:
                        if (IsRawData)
                        {
                            string dword = data;
                            return HexLibrary.HexConverter.ToBinString(dword, 16).PadLeft(32, '0');
                        }
                        else
                        {
                            return data;
                        }
                }
                break;
            case ArgumentMode.immediate_float:
                break;
            case ArgumentMode.register:
                register = data;
                return HexLibrary.HexConverter.ToBinString(register, 16).PadLeft(8, '0');
            case ArgumentMode.GPregister:
            case ArgumentMode.Sregister:
            case ArgumentMode.Rregister:
                register = data;
                return HexLibrary.HexConverter.ToBinString(register, 16).Substring(4).PadLeft(4, '0');
            case ArgumentMode.RM:
                string RM = data;
                return HexLibrary.HexConverter.ToBinString(RM, 16).Substring(6).PadLeft(2, '0');
            case ArgumentMode.MOD:
                string MOD = data;
                return HexLibrary.HexConverter.ToBinString(MOD, 16).Substring(4).PadLeft(4, '0');
        }
        return "";
    }
}
