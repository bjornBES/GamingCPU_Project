using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
namespace assembler.global
{
    public class AllInstruction
    {
        public static List<InstructionInfo> s_instructionInfo = new List<InstructionInfo>()
        {
            #region General instructions
            #region MOV
            new InstructionInfo(Instruction.MOV, Arguments.reg8, Arguments.imm8,                "0000"),
            new InstructionInfo(Instruction.MOV, Arguments.reg8, Arguments.address,             "0001"),
            new InstructionInfo(Instruction.MOV, Arguments.reg8, Arguments.reg8,                "0002"),
            new InstructionInfo(Instruction.MOV, Arguments.reg8, Arguments.reg_addr,            "0003"),
            new InstructionInfo(Instruction.MOV, Arguments.reg8, Arguments.segment_imm_reg,     "0004"),
            new InstructionInfo(Instruction.MOV, Arguments.reg8, Arguments.segment_reg_imm,     "0005"),
            new InstructionInfo(Instruction.MOV, Arguments.reg8, Arguments.segment_reg_reg,     "0006"),

            new InstructionInfo(Instruction.MOV, Arguments.reg_addr, Arguments.imm8,            "0010"),
            new InstructionInfo(Instruction.MOV, Arguments.reg_addr, Arguments.address,         "0011"),
            new InstructionInfo(Instruction.MOV, Arguments.reg_addr, Arguments.reg8,            "0012"),
            new InstructionInfo(Instruction.MOV, Arguments.reg_addr, Arguments.reg_addr,        "0013"),

            new InstructionInfo(Instruction.MOV, Arguments.address, Arguments.imm8,             "0020"),
            new InstructionInfo(Instruction.MOV, Arguments.address, Arguments.address,          "0021"),
            new InstructionInfo(Instruction.MOV, Arguments.address, Arguments.reg8,             "0022"),
            new InstructionInfo(Instruction.MOV, Arguments.address, Arguments.reg_addr,         "0023"),

            new InstructionInfo(Instruction.MOV, Arguments.long_address, Arguments.imm8,        "0030"),
            new InstructionInfo(Instruction.MOV, Arguments.long_address, Arguments.address,     "0031"),
            new InstructionInfo(Instruction.MOV, Arguments.long_address, Arguments.reg8,        "0032"),
            new InstructionInfo(Instruction.MOV, Arguments.long_address, Arguments.reg_addr,    "0033"),

            new InstructionInfo(Instruction.MOV, Arguments.segment_imm_reg, Arguments.imm8,     "0040"),
            new InstructionInfo(Instruction.MOV, Arguments.segment_imm_reg, Arguments.address,  "0041"),
            new InstructionInfo(Instruction.MOV, Arguments.segment_imm_reg, Arguments.reg8,     "0042"),
            new InstructionInfo(Instruction.MOV, Arguments.segment_imm_reg, Arguments.reg_addr, "0043"),

            new InstructionInfo(Instruction.MOV, Arguments.segment_reg_imm, Arguments.imm8,     "0050"),
            new InstructionInfo(Instruction.MOV, Arguments.segment_reg_imm, Arguments.address,  "0051"),
            new InstructionInfo(Instruction.MOV, Arguments.segment_reg_imm, Arguments.reg8,     "0052"),
            new InstructionInfo(Instruction.MOV, Arguments.segment_reg_imm, Arguments.reg_addr, "0053"),

            new InstructionInfo(Instruction.MOV, Arguments.segment_reg_reg, Arguments.imm8,     "0060"),
            new InstructionInfo(Instruction.MOV, Arguments.segment_reg_reg, Arguments.address,  "0061"),
            new InstructionInfo(Instruction.MOV, Arguments.segment_reg_reg, Arguments.reg8,     "0062"),
            new InstructionInfo(Instruction.MOV, Arguments.segment_reg_reg, Arguments.reg_addr, "0063"),

            new InstructionInfo(Instruction.MOV, Arguments.MB, Arguments.imm8,                  "00F0"),


            new InstructionInfo(Instruction.MOVW, Arguments.reg16, Arguments.imm16,             "0100"),
            new InstructionInfo(Instruction.MOVW, Arguments.reg16, Arguments.address,           "0101"),
            new InstructionInfo(Instruction.MOVW, Arguments.reg16, Arguments.reg16,             "0102"),
            new InstructionInfo(Instruction.MOVW, Arguments.reg16, Arguments.reg_addr,          "0103"),
            new InstructionInfo(Instruction.MOVW, Arguments.reg16, Arguments.segment_imm_reg,   "0104"),
            new InstructionInfo(Instruction.MOVW, Arguments.reg16, Arguments.segment_reg_imm,   "0105"),
            new InstructionInfo(Instruction.MOVW, Arguments.reg16, Arguments.segment_reg_reg,   "0106"),

            new InstructionInfo(Instruction.MOVW, Arguments.reg_addr, Arguments.imm16,          "0110"),
            new InstructionInfo(Instruction.MOVW, Arguments.reg_addr, Arguments.address,        "0111"),
            new InstructionInfo(Instruction.MOVW, Arguments.reg_addr, Arguments.reg16,          "0112"),
            new InstructionInfo(Instruction.MOVW, Arguments.reg_addr, Arguments.reg_addr,       "0113"),

            new InstructionInfo(Instruction.MOVW, Arguments.address, Arguments.imm16,           "0120"),
            new InstructionInfo(Instruction.MOVW, Arguments.address, Arguments.address,         "0121"),
            new InstructionInfo(Instruction.MOVW, Arguments.address, Arguments.reg16,           "0122"),
            new InstructionInfo(Instruction.MOVW, Arguments.address, Arguments.reg_addr,        "0123"),

            new InstructionInfo(Instruction.MOVW, Arguments.long_address,Arguments.imm16,       "0130"),
            new InstructionInfo(Instruction.MOVW, Arguments.long_address,Arguments.address,     "0131"),
            new InstructionInfo(Instruction.MOVW, Arguments.long_address,Arguments.reg16,       "0132"),
            new InstructionInfo(Instruction.MOVW, Arguments.long_address,Arguments.reg_addr,    "0133"),

            new InstructionInfo(Instruction.MOVW, Arguments.segment_imm_reg, Arguments.imm16,   "0140"),
            new InstructionInfo(Instruction.MOVW, Arguments.segment_imm_reg, Arguments.address, "0141"),
            new InstructionInfo(Instruction.MOVW, Arguments.segment_imm_reg, Arguments.reg16,   "0142"),
            new InstructionInfo(Instruction.MOVW, Arguments.segment_imm_reg, Arguments.reg_addr,"0143"),

            new InstructionInfo(Instruction.MOVW, Arguments.segment_reg_imm, Arguments.imm16,   "0150"),
            new InstructionInfo(Instruction.MOVW, Arguments.segment_reg_imm, Arguments.address, "0151"),
            new InstructionInfo(Instruction.MOVW, Arguments.segment_reg_imm, Arguments.reg16,   "0152"),
            new InstructionInfo(Instruction.MOVW, Arguments.segment_reg_imm, Arguments.reg_addr,"0153"),

            new InstructionInfo(Instruction.MOVW, Arguments.segment_reg_reg, Arguments.imm16,   "0160"),
            new InstructionInfo(Instruction.MOVW, Arguments.segment_reg_reg, Arguments.address, "0161"),
            new InstructionInfo(Instruction.MOVW, Arguments.segment_reg_reg, Arguments.reg16,   "0162"),
            new InstructionInfo(Instruction.MOVW, Arguments.segment_reg_reg, Arguments.reg_addr,"0163"),

            new InstructionInfo(Instruction.MOVW, Arguments.AX, Arguments.imm16,                "01F0"),


            new InstructionInfo(Instruction.MOVT, Arguments.reg32, Arguments.imm32,             "0200"),
            new InstructionInfo(Instruction.MOVT, Arguments.reg32, Arguments.address,           "0201"),
            new InstructionInfo(Instruction.MOVT, Arguments.reg32, Arguments.reg_addr,          "0202"),
            new InstructionInfo(Instruction.MOVT, Arguments.reg32, Arguments.segment_imm_reg,   "0204"),
            new InstructionInfo(Instruction.MOVT, Arguments.reg32, Arguments.segment_reg_imm,   "0205"),
            new InstructionInfo(Instruction.MOVT, Arguments.reg32, Arguments.segment_reg_reg,   "0206"),

            new InstructionInfo(Instruction.MOVT, Arguments.reg_addr, Arguments.imm32,          "0210"),
            new InstructionInfo(Instruction.MOVT, Arguments.reg_addr, Arguments.address,        "0211"),
            new InstructionInfo(Instruction.MOVT, Arguments.reg_addr, Arguments.reg32,          "0212"),
            new InstructionInfo(Instruction.MOVT, Arguments.reg_addr, Arguments.reg_addr,       "0213"),

            new InstructionInfo(Instruction.MOVT, Arguments.address, Arguments.imm32,           "0220"),
            new InstructionInfo(Instruction.MOVT, Arguments.address, Arguments.address,         "0221"),
            new InstructionInfo(Instruction.MOVT, Arguments.address, Arguments.reg32,           "0222"),
            new InstructionInfo(Instruction.MOVT, Arguments.address, Arguments.reg_addr,        "0223"),

            new InstructionInfo(Instruction.MOVT, Arguments.long_address,Arguments.imm32,       "0230"),
            new InstructionInfo(Instruction.MOVT, Arguments.long_address,Arguments.address,     "0231"),
            new InstructionInfo(Instruction.MOVT, Arguments.long_address,Arguments.reg32,       "0232"),
            new InstructionInfo(Instruction.MOVT, Arguments.long_address,Arguments.reg_addr,    "0233"),

            new InstructionInfo(Instruction.MOVT, Arguments.segment_imm_reg, Arguments.imm24,   "0240"),
            new InstructionInfo(Instruction.MOVT, Arguments.segment_imm_reg, Arguments.address, "0241"),
            new InstructionInfo(Instruction.MOVT, Arguments.segment_imm_reg, Arguments.reg32,   "0242"),
            new InstructionInfo(Instruction.MOVT, Arguments.segment_imm_reg, Arguments.reg_addr,"0243"),

            new InstructionInfo(Instruction.MOVT, Arguments.segment_reg_imm, Arguments.imm24,   "0250"),
            new InstructionInfo(Instruction.MOVT, Arguments.segment_reg_imm, Arguments.address, "0251"),
            new InstructionInfo(Instruction.MOVT, Arguments.segment_reg_imm, Arguments.reg32,   "0252"),
            new InstructionInfo(Instruction.MOVT, Arguments.segment_reg_imm, Arguments.reg_addr,"0253"),

            new InstructionInfo(Instruction.MOVT, Arguments.segment_reg_reg, Arguments.imm24,   "0260"),
            new InstructionInfo(Instruction.MOVT, Arguments.segment_reg_reg, Arguments.address, "0261"),
            new InstructionInfo(Instruction.MOVT, Arguments.segment_reg_reg, Arguments.reg32,   "0262"),
            new InstructionInfo(Instruction.MOVT, Arguments.segment_reg_reg, Arguments.reg_addr,"0263"),


            new InstructionInfo(Instruction.MOVD, Arguments.reg32, Arguments.imm32,             "0300"),
            new InstructionInfo(Instruction.MOVD, Arguments.reg32, Arguments.address,           "0301"),
            new InstructionInfo(Instruction.MOVD, Arguments.reg32, Arguments.reg32,             "0302"),
            new InstructionInfo(Instruction.MOVD, Arguments.reg32, Arguments.reg_addr,          "0303"),
            new InstructionInfo(Instruction.MOVD, Arguments.reg32, Arguments.segment_imm_reg,   "0304"),
            new InstructionInfo(Instruction.MOVD, Arguments.reg32, Arguments.segment_reg_imm,   "0305"),
            new InstructionInfo(Instruction.MOVD, Arguments.reg32, Arguments.segment_reg_reg,   "0306"),

            new InstructionInfo(Instruction.MOVD, Arguments.reg_addr, Arguments.imm32,          "0310"),
            new InstructionInfo(Instruction.MOVD, Arguments.reg_addr, Arguments.address,        "0311"),
            new InstructionInfo(Instruction.MOVD, Arguments.reg_addr, Arguments.reg32,          "0312"),
            new InstructionInfo(Instruction.MOVD, Arguments.reg_addr, Arguments.reg_addr,       "0313"),

            new InstructionInfo(Instruction.MOVD, Arguments.address, Arguments.imm32,           "0320"),
            new InstructionInfo(Instruction.MOVD, Arguments.address, Arguments.address,         "0321"),
            new InstructionInfo(Instruction.MOVD, Arguments.address, Arguments.reg32,           "0322"),
            new InstructionInfo(Instruction.MOVD, Arguments.address, Arguments.reg_addr,        "0323"),

            new InstructionInfo(Instruction.MOVD, Arguments.long_address,Arguments.imm32,       "0330"),
            new InstructionInfo(Instruction.MOVD, Arguments.long_address,Arguments.address,     "0331"),
            new InstructionInfo(Instruction.MOVD, Arguments.long_address,Arguments.reg32,       "0332"),
            new InstructionInfo(Instruction.MOVD, Arguments.long_address,Arguments.reg_addr,    "0333"),

            new InstructionInfo(Instruction.MOVD, Arguments.segment_imm_reg, Arguments.imm32,   "0340"),
            new InstructionInfo(Instruction.MOVD, Arguments.segment_imm_reg, Arguments.address, "0341"),
            new InstructionInfo(Instruction.MOVD, Arguments.segment_imm_reg, Arguments.reg32,   "0342"),
            new InstructionInfo(Instruction.MOVD, Arguments.segment_imm_reg, Arguments.reg_addr,"0343"),

            new InstructionInfo(Instruction.MOVD, Arguments.segment_reg_imm, Arguments.imm32,   "0350"),
            new InstructionInfo(Instruction.MOVD, Arguments.segment_reg_imm, Arguments.address, "0351"),
            new InstructionInfo(Instruction.MOVD, Arguments.segment_reg_imm, Arguments.reg32,   "0352"),
            new InstructionInfo(Instruction.MOVD, Arguments.segment_reg_imm, Arguments.reg_addr,"0353"),

            new InstructionInfo(Instruction.MOVD, Arguments.segment_reg_reg, Arguments.imm32,   "0360"),
            new InstructionInfo(Instruction.MOVD, Arguments.segment_reg_reg, Arguments.address, "0361"),
            new InstructionInfo(Instruction.MOVD, Arguments.segment_reg_reg, Arguments.reg32,   "0362"),
            new InstructionInfo(Instruction.MOVD, Arguments.segment_reg_reg, Arguments.reg_addr,"0363"),

            new InstructionInfo(Instruction.MOVD, Arguments.HL, Arguments.imm32,                "03F0"),
            #endregion
            #region CMP
            new InstructionInfo(Instruction.CMP, Arguments.reg8, Arguments.imm8,                "0400"),
            new InstructionInfo(Instruction.CMP, Arguments.reg8, Arguments.reg8,                "0401"),

            new InstructionInfo(Instruction.CMP, Arguments.reg16, Arguments.imm16,              "0410"),
            new InstructionInfo(Instruction.CMP, Arguments.reg16, Arguments.reg16,              "0411"),

            new InstructionInfo(Instruction.CMP, Arguments.reg32, Arguments.imm24,              "0420"),

            new InstructionInfo(Instruction.CMP, Arguments.reg32, Arguments.imm32,              "0430"),
            new InstructionInfo(Instruction.CMP, Arguments.reg32, Arguments.reg32,              "0431"),
            #endregion
            #region PUSH
            new InstructionInfo(Instruction.PUSH, Arguments.imm8, Arguments.none,               "0500"),
            new InstructionInfo(Instruction.PUSH, Arguments.imm16, Arguments.none,              "0501"),
            new InstructionInfo(Instruction.PUSH, Arguments.imm24, Arguments.none,              "0502"),
            new InstructionInfo(Instruction.PUSH, Arguments.imm32, Arguments.none,              "0503"),

            new InstructionInfo(Instruction.PUSH, Arguments.reg8, Arguments.none,               "0510"),
            new InstructionInfo(Instruction.PUSH, Arguments.reg16, Arguments.none,              "0511"),
            new InstructionInfo(Instruction.PUSH, Arguments.reg32, Arguments.none,              "0512"),

            new InstructionInfo(Instruction.PUSH, Arguments.BP, Arguments.none,                 "05F0"),
            #endregion
            #region POP
            new InstructionInfo(Instruction.POP, Arguments.reg8, Arguments.none,                "0600"),
            new InstructionInfo(Instruction.POPW, Arguments.reg16, Arguments.none,              "0610"),
            new InstructionInfo(Instruction.POPT, Arguments.reg32, Arguments.none,              "0620"),
            new InstructionInfo(Instruction.POPD, Arguments.reg32, Arguments.none,              "0630"),
            #endregion
            
            new InstructionInfo(Instruction.CALL, Arguments.address, Arguments.none,            "0700"),
            new InstructionInfo(Instruction.CALL, Arguments.reg_addr, Arguments.none,           "0701"),
            new InstructionInfo(Instruction.CALL, Arguments.segment_reg_reg, Arguments.none,    "0702"),

            new InstructionInfo(Instruction.RET, Arguments.imm8, Arguments.none,                "0800"),
            new InstructionInfo(Instruction.RET, Arguments.none, Arguments.none,                "0801"),

            new InstructionInfo(Instruction.SEZ, Arguments.reg8, Arguments.none,                "0900"),
            new InstructionInfo(Instruction.SEZ, Arguments.reg16, Arguments.none,               "0910"),
            new InstructionInfo(Instruction.SEZ, Arguments.reg32, Arguments.none,               "0920"),

            #region TEST
            new InstructionInfo(Instruction.TEST, Arguments.reg8, Arguments.none,               "0A00"),
            new InstructionInfo(Instruction.TEST, Arguments.reg16, Arguments.none,              "0A10"),
            new InstructionInfo(Instruction.TEST, Arguments.reg32, Arguments.none,              "0A20"),
            new InstructionInfo(Instruction.TEST, Arguments.AX, Arguments.none,                 "0AF0"),
            #endregion

            #region PUSHA
            new InstructionInfo(Instruction.PUSHA, Arguments.imm8, Arguments.none,              "5000"),
            new InstructionInfo(Instruction.PUSHA, Arguments.imm16, Arguments.none,             "5001"),
            new InstructionInfo(Instruction.PUSHA, Arguments.imm24, Arguments.none,             "5002"),
            new InstructionInfo(Instruction.PUSHA, Arguments.imm32, Arguments.none,             "5003"),

            new InstructionInfo(Instruction.PUSHA, Arguments.reg8, Arguments.none,              "5010"),
            new InstructionInfo(Instruction.PUSHA, Arguments.reg16, Arguments.none,             "5011"),
            new InstructionInfo(Instruction.PUSHA, Arguments.reg32, Arguments.none,             "5012"),
            #endregion

            #endregion

            #region Conditional jumps
            new InstructionInfo(Instruction.JMP, Arguments.address, Arguments.none,             "0B00"),
            new InstructionInfo(Instruction.JMP, Arguments.long_address, Arguments.none,        "0B01"),
            new InstructionInfo(Instruction.JMP, Arguments.segment_imm_reg, Arguments.none,     "0B02"),
            new InstructionInfo(Instruction.JMP, Arguments.segment_reg_imm, Arguments.none,     "0B03"),
            new InstructionInfo(Instruction.JMP, Arguments.segment_reg_reg, Arguments.none,     "0B04"),

            new InstructionInfo(Instruction.JZ, Arguments.address, Arguments.none,              "0C00"),
            new InstructionInfo(Instruction.JZ, Arguments.long_address, Arguments.none,         "0C01"),
            new InstructionInfo(Instruction.JZ, Arguments.segment_imm_reg, Arguments.none,      "0C02"),
            new InstructionInfo(Instruction.JZ, Arguments.segment_reg_imm, Arguments.none,      "0C03"),
            new InstructionInfo(Instruction.JZ, Arguments.segment_reg_reg, Arguments.none,      "0C04"),

            new InstructionInfo(Instruction.JNZ, Arguments.address, Arguments.none,             "0D00"),
            new InstructionInfo(Instruction.JNZ, Arguments.long_address, Arguments.none,        "0D01"),
            new InstructionInfo(Instruction.JNZ, Arguments.segment_imm_reg, Arguments.none,     "0C02"),
            new InstructionInfo(Instruction.JNZ, Arguments.segment_reg_imm, Arguments.none,     "0C03"),
            new InstructionInfo(Instruction.JNZ, Arguments.segment_reg_reg, Arguments.none,     "0C04"),

            new InstructionInfo(Instruction.JS, Arguments.address, Arguments.none,              "0E00"),
            new InstructionInfo(Instruction.JS, Arguments.long_address, Arguments.none,         "0E01"),
            new InstructionInfo(Instruction.JS, Arguments.segment_imm_reg, Arguments.none,      "0E02"),
            new InstructionInfo(Instruction.JS, Arguments.segment_reg_imm, Arguments.none,      "0E03"),
            new InstructionInfo(Instruction.JS, Arguments.segment_reg_reg, Arguments.none,      "0E04"),

            new InstructionInfo(Instruction.JNS, Arguments.address, Arguments.none,             "0F00"),
            new InstructionInfo(Instruction.JNS, Arguments.long_address, Arguments.none,        "0F01"),
            new InstructionInfo(Instruction.JNS, Arguments.segment_imm_reg, Arguments.none,     "0F02"),
            new InstructionInfo(Instruction.JNS, Arguments.segment_reg_imm, Arguments.none,     "0F03"),
            new InstructionInfo(Instruction.JNS, Arguments.segment_reg_reg, Arguments.none,     "0F04"),

            new InstructionInfo(Instruction.JE, Arguments.address, Arguments.none,              "1000"),
            new InstructionInfo(Instruction.JE, Arguments.long_address, Arguments.none,         "1001"),
            new InstructionInfo(Instruction.JE, Arguments.segment_imm_reg, Arguments.none,      "1002"),
            new InstructionInfo(Instruction.JE, Arguments.segment_reg_imm, Arguments.none,      "1003"),
            new InstructionInfo(Instruction.JE, Arguments.segment_reg_reg, Arguments.none,      "1004"),


            new InstructionInfo(Instruction.JNE, Arguments.address, Arguments.none,             "1100"),
            new InstructionInfo(Instruction.JNE, Arguments.long_address, Arguments.none,        "1101"),
            new InstructionInfo(Instruction.JNE, Arguments.segment_imm_reg, Arguments.none,     "1102"),
            new InstructionInfo(Instruction.JNE, Arguments.segment_reg_imm, Arguments.none,     "1103"),
            new InstructionInfo(Instruction.JNE, Arguments.segment_reg_reg, Arguments.none,     "1104"),

            new InstructionInfo(Instruction.JL, Arguments.address, Arguments.none,              "1200"),
            new InstructionInfo(Instruction.JL, Arguments.long_address, Arguments.none,         "1201"),
            new InstructionInfo(Instruction.JL, Arguments.segment_imm_reg, Arguments.none,      "1202"),
            new InstructionInfo(Instruction.JL, Arguments.segment_reg_imm, Arguments.none,      "1203"),
            new InstructionInfo(Instruction.JL, Arguments.segment_reg_reg, Arguments.none,      "1204"),

            new InstructionInfo(Instruction.JG, Arguments.address, Arguments.none,              "1300"),
            new InstructionInfo(Instruction.JG, Arguments.long_address, Arguments.none,         "1301"),
            new InstructionInfo(Instruction.JG, Arguments.segment_imm_reg, Arguments.none,      "1302"),
            new InstructionInfo(Instruction.JG, Arguments.segment_reg_imm, Arguments.none,      "1303"),
            new InstructionInfo(Instruction.JG, Arguments.segment_reg_reg, Arguments.none,      "1304"),

            new InstructionInfo(Instruction.JLE, Arguments.address, Arguments.none,             "1400"),
            new InstructionInfo(Instruction.JLE, Arguments.long_address, Arguments.none,        "1401"),
            new InstructionInfo(Instruction.JLE, Arguments.segment_imm_reg, Arguments.none,     "1402"),
            new InstructionInfo(Instruction.JLE, Arguments.segment_reg_imm, Arguments.none,     "1403"),
            new InstructionInfo(Instruction.JLE, Arguments.segment_reg_reg, Arguments.none,     "1404"),

            new InstructionInfo(Instruction.JGE, Arguments.address, Arguments.none,             "1500"),
            new InstructionInfo(Instruction.JGE, Arguments.long_address, Arguments.none,        "1501"),
            new InstructionInfo(Instruction.JGE, Arguments.segment_imm_reg, Arguments.none,     "1502"),
            new InstructionInfo(Instruction.JGE, Arguments.segment_reg_imm, Arguments.none,     "1503"),
            new InstructionInfo(Instruction.JGE, Arguments.segment_reg_reg, Arguments.none,     "1504"),

            new InstructionInfo(Instruction.JNV, Arguments.address, Arguments.none,             "1600"),
            new InstructionInfo(Instruction.JNV, Arguments.long_address, Arguments.none,        "1601"),
            new InstructionInfo(Instruction.JNV, Arguments.segment_imm_reg, Arguments.none,     "1602"),
            new InstructionInfo(Instruction.JNV, Arguments.segment_reg_imm, Arguments.none,     "1603"),
            new InstructionInfo(Instruction.JNV, Arguments.segment_reg_reg, Arguments.none,     "1604"),
            #endregion
            
            #region IO instructions
            #region IN
            new InstructionInfo(Instruction.IN, Arguments.imm8, Arguments.reg,                  "1C00"),
            new InstructionInfo(Instruction.IN, Arguments.reg, Arguments.reg,                   "1C01"),
            #endregion

            #region OUT
            new InstructionInfo(Instruction.IN, Arguments.imm8, Arguments.reg,                  "1D00"),
            new InstructionInfo(Instruction.IN, Arguments.imm8, Arguments.imm8,                 "1D01"),
            new InstructionInfo(Instruction.IN, Arguments.reg, Arguments.imm8,                  "1D02"),
            new InstructionInfo(Instruction.IN, Arguments.reg, Arguments.reg,                   "1D03"),
            #endregion
            #endregion

            #region Arithmetic and logic operations
            new InstructionInfo(Instruction.SEF, Arguments.imm16, Arguments.none,               "1E00"),

            new InstructionInfo(Instruction.CLF, Arguments.imm16, Arguments.none,               "1F00"),

            #region ADD
            new InstructionInfo(Instruction.ADD, Arguments.reg8, Arguments.imm8,                "2000"),
            new InstructionInfo(Instruction.ADD, Arguments.reg8, Arguments.address,             "2001"),
            new InstructionInfo(Instruction.ADD, Arguments.reg8, Arguments.reg8,                "2002"),
            new InstructionInfo(Instruction.ADD, Arguments.reg8, Arguments.reg_addr,            "2003"),

            new InstructionInfo(Instruction.ADD, Arguments.reg16, Arguments.imm16,              "2010"),
            new InstructionInfo(Instruction.ADD, Arguments.reg16, Arguments.address,            "2011"),
            new InstructionInfo(Instruction.ADD, Arguments.reg16, Arguments.reg16,              "2012"),
            new InstructionInfo(Instruction.ADD, Arguments.reg16, Arguments.reg_addr,           "2013"),
            
            new InstructionInfo(Instruction.ADD, Arguments.reg32, Arguments.imm32,              "2020"),
            new InstructionInfo(Instruction.ADD, Arguments.reg32, Arguments.address,            "2021"),
            new InstructionInfo(Instruction.ADD, Arguments.reg32, Arguments.reg32,              "2022"),
            new InstructionInfo(Instruction.ADD, Arguments.reg32, Arguments.reg_addr,           "2023"),
            #endregion

            #region SUB
            new InstructionInfo(Instruction.SUB, Arguments.reg8, Arguments.imm8,                "2100"),
            new InstructionInfo(Instruction.SUB, Arguments.reg8, Arguments.address,             "2101"),
            new InstructionInfo(Instruction.SUB, Arguments.reg8, Arguments.reg8,                "2102"),
            new InstructionInfo(Instruction.SUB, Arguments.reg8, Arguments.reg_addr,            "2103"),

            new InstructionInfo(Instruction.SUB, Arguments.reg16, Arguments.imm16,              "2110"),
            new InstructionInfo(Instruction.SUB, Arguments.reg16, Arguments.address,            "2111"),
            new InstructionInfo(Instruction.SUB, Arguments.reg16, Arguments.reg16,              "2112"),
            new InstructionInfo(Instruction.SUB, Arguments.reg16, Arguments.reg_addr,           "2113"),
            
            new InstructionInfo(Instruction.SUB, Arguments.reg32, Arguments.imm32,              "2120"),
            new InstructionInfo(Instruction.SUB, Arguments.reg32, Arguments.address,            "2121"),
            new InstructionInfo(Instruction.SUB, Arguments.reg32, Arguments.reg32,              "2122"),
            new InstructionInfo(Instruction.SUB, Arguments.reg32, Arguments.reg_addr,           "2123"),
            #endregion

            #region MUL
            new InstructionInfo(Instruction.MUL, Arguments.reg8, Arguments.imm8,                "2200"),
            new InstructionInfo(Instruction.MUL, Arguments.reg8, Arguments.address,             "2201"),
            new InstructionInfo(Instruction.MUL, Arguments.reg8, Arguments.reg8,                "2202"),
            new InstructionInfo(Instruction.MUL, Arguments.reg8, Arguments.reg_addr,            "2203"),

            new InstructionInfo(Instruction.MUL, Arguments.reg16, Arguments.imm16,              "2210"),
            new InstructionInfo(Instruction.MUL, Arguments.reg16, Arguments.address,            "2211"),
            new InstructionInfo(Instruction.MUL, Arguments.reg16, Arguments.reg16,              "2212"),
            new InstructionInfo(Instruction.MUL, Arguments.reg16, Arguments.reg_addr,           "2213"),
            
            new InstructionInfo(Instruction.MUL, Arguments.reg32, Arguments.imm32,              "2220"),
            new InstructionInfo(Instruction.MUL, Arguments.reg32, Arguments.address,            "2221"),
            new InstructionInfo(Instruction.MUL, Arguments.reg32, Arguments.reg32,              "2222"),
            new InstructionInfo(Instruction.MUL, Arguments.reg32, Arguments.reg_addr,           "2223"),
            #endregion

            #region DIV
            new InstructionInfo(Instruction.DIV, Arguments.reg8, Arguments.imm8,                "2300"),
            new InstructionInfo(Instruction.DIV, Arguments.reg8, Arguments.address,             "2301"),
            new InstructionInfo(Instruction.DIV, Arguments.reg8, Arguments.reg8,                "2302"),
            new InstructionInfo(Instruction.DIV, Arguments.reg8, Arguments.reg_addr,            "2303"),

            new InstructionInfo(Instruction.DIV, Arguments.reg16, Arguments.imm16,              "2310"),
            new InstructionInfo(Instruction.DIV, Arguments.reg16, Arguments.address,            "2311"),
            new InstructionInfo(Instruction.DIV, Arguments.reg16, Arguments.reg16,              "2312"),
            new InstructionInfo(Instruction.DIV, Arguments.reg16, Arguments.reg_addr,           "2313"),
            
            new InstructionInfo(Instruction.DIV, Arguments.reg32, Arguments.imm32,              "2320"),
            new InstructionInfo(Instruction.DIV, Arguments.reg32, Arguments.address,            "2321"),
            new InstructionInfo(Instruction.DIV, Arguments.reg32, Arguments.reg32,              "2322"),
            new InstructionInfo(Instruction.DIV, Arguments.reg32, Arguments.reg_addr,           "2323"),
            #endregion

            #region AND
            new InstructionInfo(Instruction.AND, Arguments.reg8, Arguments.imm8,                "2400"),
            new InstructionInfo(Instruction.AND, Arguments.reg8, Arguments.address,             "2401"),
            new InstructionInfo(Instruction.AND, Arguments.reg8, Arguments.reg8,                "2402"),
            new InstructionInfo(Instruction.AND, Arguments.reg8, Arguments.reg_addr,            "2403"),

            new InstructionInfo(Instruction.AND, Arguments.reg16, Arguments.imm16,              "2410"),
            new InstructionInfo(Instruction.AND, Arguments.reg16, Arguments.address,            "2411"),
            new InstructionInfo(Instruction.AND, Arguments.reg16, Arguments.reg16,              "2412"),
            new InstructionInfo(Instruction.AND, Arguments.reg16, Arguments.reg_addr,           "2413"),
            
            new InstructionInfo(Instruction.AND, Arguments.reg32, Arguments.imm32,              "2420"),
            new InstructionInfo(Instruction.AND, Arguments.reg32, Arguments.address,            "2421"),
            new InstructionInfo(Instruction.AND, Arguments.reg32, Arguments.reg32,              "2422"),
            new InstructionInfo(Instruction.AND, Arguments.reg32, Arguments.reg_addr,           "2423"),
            #endregion

            #region OR
            new InstructionInfo(Instruction.OR, Arguments.reg8, Arguments.imm8,                 "2500"),
            new InstructionInfo(Instruction.OR, Arguments.reg8, Arguments.address,              "2501"),
            new InstructionInfo(Instruction.OR, Arguments.reg8, Arguments.reg8,                 "2502"),
            new InstructionInfo(Instruction.OR, Arguments.reg8, Arguments.reg_addr,             "2503"),

            new InstructionInfo(Instruction.OR, Arguments.reg16, Arguments.imm16,               "2510"),
            new InstructionInfo(Instruction.OR, Arguments.reg16, Arguments.address,             "2511"),
            new InstructionInfo(Instruction.OR, Arguments.reg16, Arguments.reg16,               "2512"),
            new InstructionInfo(Instruction.OR, Arguments.reg16, Arguments.reg_addr,            "2513"),
            
            new InstructionInfo(Instruction.OR, Arguments.reg32, Arguments.imm32,               "2520"),
            new InstructionInfo(Instruction.OR, Arguments.reg32, Arguments.address,             "2521"),
            new InstructionInfo(Instruction.OR, Arguments.reg32, Arguments.reg32,               "2522"),
            new InstructionInfo(Instruction.OR, Arguments.reg32, Arguments.reg_addr,            "2523"),
            #endregion

            #region NOR
            new InstructionInfo(Instruction.NOR, Arguments.reg8, Arguments.imm8,                "2600"),
            new InstructionInfo(Instruction.NOR, Arguments.reg8, Arguments.address,             "2601"),
            new InstructionInfo(Instruction.NOR, Arguments.reg8, Arguments.reg8,                "2602"),
            new InstructionInfo(Instruction.NOR, Arguments.reg8, Arguments.reg_addr,            "2603"),

            new InstructionInfo(Instruction.NOR, Arguments.reg16, Arguments.imm16,              "2610"),
            new InstructionInfo(Instruction.NOR, Arguments.reg16, Arguments.address,            "2611"),
            new InstructionInfo(Instruction.NOR, Arguments.reg16, Arguments.reg16,              "2612"),
            new InstructionInfo(Instruction.NOR, Arguments.reg16, Arguments.reg_addr,           "2613"),
            
            new InstructionInfo(Instruction.NOR, Arguments.reg32, Arguments.imm32,              "2620"),
            new InstructionInfo(Instruction.NOR, Arguments.reg32, Arguments.address,            "2621"),
            new InstructionInfo(Instruction.NOR, Arguments.reg32, Arguments.reg32,              "2622"),
            new InstructionInfo(Instruction.NOR, Arguments.reg32, Arguments.reg_addr,           "2623"),
            #endregion

            #region NOT
            new InstructionInfo(Instruction.NOT, Arguments.reg8,  Arguments.none,               "2700"),
            new InstructionInfo(Instruction.NOT, Arguments.reg16, Arguments.none,               "2710"),
            new InstructionInfo(Instruction.NOT, Arguments.reg32, Arguments.none,               "2720"),
            #endregion

            #region XOR
            new InstructionInfo(Instruction.XOR, Arguments.reg8, Arguments.imm8,                "2800"),
            new InstructionInfo(Instruction.XOR, Arguments.reg8, Arguments.address,             "2801"),
            new InstructionInfo(Instruction.XOR, Arguments.reg8, Arguments.reg8,                "2802"),
            new InstructionInfo(Instruction.XOR, Arguments.reg8, Arguments.reg_addr,            "2803"),

            new InstructionInfo(Instruction.XOR, Arguments.reg16, Arguments.imm16,              "2810"),
            new InstructionInfo(Instruction.XOR, Arguments.reg16, Arguments.address,            "2811"),
            new InstructionInfo(Instruction.XOR, Arguments.reg16, Arguments.reg16,              "2812"),
            new InstructionInfo(Instruction.XOR, Arguments.reg16, Arguments.reg_addr,           "2813"),
            
            new InstructionInfo(Instruction.XOR, Arguments.reg32, Arguments.imm32,              "2820"),
            new InstructionInfo(Instruction.XOR, Arguments.reg32, Arguments.address,            "2821"),
            new InstructionInfo(Instruction.XOR, Arguments.reg32, Arguments.reg32,              "2822"),
            new InstructionInfo(Instruction.XOR, Arguments.reg32, Arguments.reg_addr,           "2823"),
            #endregion

            #region SHL
            new InstructionInfo(Instruction.SHL, Arguments.reg8, Arguments.imm8,                "2900"),
            new InstructionInfo(Instruction.SHL, Arguments.reg16, Arguments.imm16,              "2901"),
            new InstructionInfo(Instruction.SHL, Arguments.reg32, Arguments.imm24,              "2902"),
            new InstructionInfo(Instruction.SHL, Arguments.reg32, Arguments.imm32,              "2903"),
            #endregion

            #region SHR
            new InstructionInfo(Instruction.SHR, Arguments.reg8, Arguments.imm8,                "2A00"),
            new InstructionInfo(Instruction.SHR, Arguments.reg16, Arguments.imm16,              "2A01"),
            new InstructionInfo(Instruction.SHR, Arguments.reg32, Arguments.imm24,              "2A02"),
            new InstructionInfo(Instruction.SHR, Arguments.reg32, Arguments.imm32,              "2A03"),
            #endregion

            #region ROL
            new InstructionInfo(Instruction.ROL, Arguments.reg8, Arguments.imm8,                "2B00"),
            new InstructionInfo(Instruction.ROL, Arguments.reg16, Arguments.imm16,              "2B01"),
            new InstructionInfo(Instruction.ROL, Arguments.reg32, Arguments.imm24,              "2B02"),
            new InstructionInfo(Instruction.ROL, Arguments.reg32, Arguments.imm32,              "2B03"),
            #endregion

            #region ROR
            new InstructionInfo(Instruction.ROR, Arguments.reg8, Arguments.imm8,                "2C00"),
            new InstructionInfo(Instruction.ROR, Arguments.reg16, Arguments.imm16,              "2C01"),
            new InstructionInfo(Instruction.ROR, Arguments.reg32, Arguments.imm24,              "2C02"),
            new InstructionInfo(Instruction.ROR, Arguments.reg32, Arguments.imm32,              "2C03"),
            #endregion

            #region INC
            new InstructionInfo(Instruction.INC, Arguments.reg8,  Arguments.none,               "2D00"),
            new InstructionInfo(Instruction.INC, Arguments.reg16, Arguments.none,               "2D10"),
            new InstructionInfo(Instruction.INC, Arguments.reg32, Arguments.none,               "2D20"),
            #endregion

            #region DEC
            new InstructionInfo(Instruction.DEC, Arguments.reg8,  Arguments.none,               "2E00"),
            new InstructionInfo(Instruction.DEC, Arguments.reg16, Arguments.none,               "2E10"),
            new InstructionInfo(Instruction.DEC, Arguments.reg32, Arguments.none,               "2E20"),
            #endregion

            #region NEG
            new InstructionInfo(Instruction.DEC, Arguments.reg8,  Arguments.none,               "2F00"),
            new InstructionInfo(Instruction.DEC, Arguments.reg16, Arguments.none,               "2F10"),
            new InstructionInfo(Instruction.DEC, Arguments.reg32, Arguments.none,               "2F20"),
            #endregion

            #region AVG
            new InstructionInfo(Instruction.AVG, Arguments.reg8, Arguments.imm8,                "3000"),
            new InstructionInfo(Instruction.AVG, Arguments.reg16, Arguments.imm16,              "3001"),
            new InstructionInfo(Instruction.AVG, Arguments.reg32, Arguments.imm32,              "3002"),
            #endregion

            #region EXP
            new InstructionInfo(Instruction.EXP, Arguments.reg8, Arguments.imm8,                "3100"),
            new InstructionInfo(Instruction.EXP, Arguments.reg16, Arguments.imm16,              "3101"),
            new InstructionInfo(Instruction.EXP, Arguments.reg32, Arguments.imm32,              "3102"),
            #endregion

            #region SQRT
            new InstructionInfo(Instruction.SQRT, Arguments.reg8,  Arguments.none,              "3200"),
            new InstructionInfo(Instruction.SQRT, Arguments.reg16, Arguments.none,              "3201"),
            new InstructionInfo(Instruction.SQRT, Arguments.reg32, Arguments.none,              "3202"),
            #endregion

            #region MOD
            new InstructionInfo(Instruction.MOD, Arguments.reg8, Arguments.imm8,                "3300"),
            new InstructionInfo(Instruction.MOD, Arguments.reg16, Arguments.imm16,              "3301"),
            new InstructionInfo(Instruction.MOD, Arguments.reg32, Arguments.imm32,              "3302"),
            #endregion

            #region SEB
            new InstructionInfo(Instruction.SEB, Arguments.reg8, Arguments.imm8,                "3400"),
            new InstructionInfo(Instruction.SEB, Arguments.reg16, Arguments.imm16,              "3401"),
            new InstructionInfo(Instruction.SEB, Arguments.reg32, Arguments.imm32,              "3402"),
            #endregion

            #region CLB
            new InstructionInfo(Instruction.CLB, Arguments.reg8, Arguments.imm8,                "3500"),
            new InstructionInfo(Instruction.CLB, Arguments.reg16, Arguments.imm16,              "3501"),
            new InstructionInfo(Instruction.CLB, Arguments.reg32, Arguments.imm32,              "3502"),
            #endregion

            #region TOB
            new InstructionInfo(Instruction.TOB, Arguments.reg8, Arguments.imm8,                "3600"),
            new InstructionInfo(Instruction.TOB, Arguments.reg16, Arguments.imm16,              "3601"),
            new InstructionInfo(Instruction.TOB, Arguments.reg32, Arguments.imm32,              "3602"),
            #endregion
            #endregion

            #region Float arithmetic operations
            #region MOVF
            new InstructionInfo(Instruction.MOVF, Arguments.f_reg, Arguments.f_imm,             "3700"),

            new InstructionInfo(Instruction.MOVF, Arguments.FA, Arguments.f_imm,                "37F0"),
            new InstructionInfo(Instruction.MOVF, Arguments.FB, Arguments.f_imm,                "37F1"),
            #endregion

            #region ADDF
            new InstructionInfo(Instruction.MOVF, Arguments.f_reg, Arguments.f_imm,             "3800"),
            new InstructionInfo(Instruction.MOVF, Arguments.f_reg, Arguments.f_reg,             "3801"),
            #endregion
            #endregion

            #region Memory Operations
            new InstructionInfo(Instruction.MBL, Arguments.address, Arguments.address,          "0900"),
            #endregion
        };
    }

}