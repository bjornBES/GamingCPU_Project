using CommonBCGCPU.Types;

namespace BCG16CPUEmulator
{
    public class BCG16CPU_Registers
    {
        public _16Bit_Register A = new _16Bit_Register();
        public _16Bit_Register B = new _16Bit_Register();
        public _16Bit_Register C = new _16Bit_Register();
        public _16Bit_Register D = new _16Bit_Register();

        public _16Bit_Register H = new _16Bit_Register();
        public _16Bit_Register L = new _16Bit_Register();
        public _32Bit_Register HL = new _32Bit_Register();

        public _16Bit_Register DS = new _16Bit_Register();
        public _16Bit_Register ES = new _16Bit_Register();
        public _16Bit_Register SS = new _16Bit_Register();
        public _16Bit_Register S = new _16Bit_Register();

        public _8Bit_Register PCMB = new _8Bit_Register();
        public _24Bit_Register PC = new _24Bit_Register();

        public _32BitFloatRegister AF = new _32BitFloatRegister();
        public _32BitFloatRegister BF = new _32BitFloatRegister();

        public _16Bit_Register BP = new _16Bit_Register();
        public _16Bit_Register SP = new _16Bit_Register();

        public _24Bit_Register IDTAddressRegister = new _24Bit_Register();

        public _16Bit_Register R1 = new _16Bit_Register();
        public _16Bit_Register R2 = new _16Bit_Register();

        public _8Bit_Register MB = new _8Bit_Register();

        public _8Bit_Register CR0 = new _8Bit_Register();
        public _8Bit_Register CR1 = new _8Bit_Register();

        public _16Bit_Register F = new _16Bit_Register();

        public BUS m_BUS;

        public Instruction IR;

        /// <summary>
        /// Flag Zero
        /// </summary>
        public const int FZ = 0x0001;

        /// <summary>
        /// Flag equals
        /// </summary>
        public const int FE = 0x0002;

        /// <summary>
        /// Flag Signed
        /// </summary>
        public const int FS = 0x0004;

        /// <summary>
        /// Flag Carry
        /// </summary>
        public const int FC = 0x0008;

        /// <summary>
        /// Flag overflow
        /// </summary>
        public const int FO = 0x0010;

        /// <summary>
        /// flag less
        /// </summary>
        public const int FL = 0x0020;

        /// <summary>
        /// Flag interrupt
        /// </summary>
        public const int FI = 0x0200;

        /// <summary>
        /// flag Halted
        /// </summary>
        public const int FH = 0x8000;

        public const int CR0_EnableA20 = 0x01;
        public const int CR0_UseExtendedRegisters = 0x04;
        public const int CR0_EnableA24 = 0x10;

        public const int CR1_BiosRom = 0x10;
    }
}
