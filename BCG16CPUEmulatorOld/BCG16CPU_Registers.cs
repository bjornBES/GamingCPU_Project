using BCG16CPUEmulator.Types;

namespace BCG16CPUEmulator
{
    public class BCG16CPU_Registers
    {
        public _16Bit_Register A = new _16Bit_Register();
        public _16Bit_Register B = new _16Bit_Register();
        public _16Bit_Register C = new _16Bit_Register();
        public _16Bit_Register D = new _16Bit_Register();

        public _32Bit_Register EXAB = new _32Bit_Register();
        public _32Bit_Register EXCD = new _32Bit_Register();

        public _16Bit_Register H = new _16Bit_Register();
        public _16Bit_Register L = new _16Bit_Register();
        public _32Bit_Register HL = new _32Bit_Register();

        public _16Bit_Register CS = new _16Bit_Register();
        public _16Bit_Register DS = new _16Bit_Register();
        public _16Bit_Register SS = new _16Bit_Register();
        public _16Bit_Register S = new _16Bit_Register();

        public _8Bit_Register PCMB = new _8Bit_Register();
        public _32Bit_Register PC = new _32Bit_Register();

        public _32BitFloatRegister AF = new _32BitFloatRegister();
        public _32BitFloatRegister BF = new _32BitFloatRegister();

        public _16Bit_Register BPX = new _16Bit_Register();
        public _16Bit_Register SPX = new _16Bit_Register();

        public _8Bit_Register IL = new _8Bit_Register();

        public _16Bit_Register R1 = new _16Bit_Register();
        public _16Bit_Register R2 = new _16Bit_Register();

        public _16Bit_Register MB = new _16Bit_Register();

        public _8Bit_Register CR0 = new _8Bit_Register();
        public _8Bit_Register CR1 = new _8Bit_Register();

        public _16Bit_Register F = new _16Bit_Register();

        public BUS m_BUS;
    }
}
