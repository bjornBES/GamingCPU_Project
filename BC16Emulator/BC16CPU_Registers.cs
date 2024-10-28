using CommonBCGCPU.Types;

namespace BCG16CPUEmulator
{
    public class BC16CPU_Registers
    {
        public _32Bit_Register m_AX = new _32Bit_Register();
        public _32Bit_Register m_BX = new _32Bit_Register();
        public _32Bit_Register m_CX = new _32Bit_Register();
        public _32Bit_Register m_DX = new _32Bit_Register();

        public _32Bit_Register m_HL = new _32Bit_Register();

        public _16Bit_Register m_DS = new _16Bit_Register();
        public _16Bit_Register m_ES = new _16Bit_Register();
        public _16Bit_Register m_FS = new _16Bit_Register();
        public _16Bit_Register m_SS = new _16Bit_Register();
        public _16Bit_Register m_CS = new _16Bit_Register();

        public _16Bit_Register m_X = new _16Bit_Register();
        public _16Bit_Register m_Y = new _16Bit_Register();

        public _24Bit_Register m_PC = new _24Bit_Register();
        public _24Bit_Register m_AddressSave = new _24Bit_Register();

        public _32BitFloatRegister m_AF = new _32BitFloatRegister();
        public _32BitFloatRegister m_BF = new _32BitFloatRegister();

        public _16Bit_Register m_BP = new _16Bit_Register();
        public _16Bit_Register m_SP = new _16Bit_Register();

        public _32Bit_Register m_R1 = new _32Bit_Register();
        public _32Bit_Register m_R2 = new _32Bit_Register();
        public _32Bit_Register m_R3 = new _32Bit_Register();
        public _32Bit_Register m_R4 = new _32Bit_Register();
        public _32Bit_Register m_R5 = new _32Bit_Register();
        public _32Bit_Register m_R6 = new _32Bit_Register();
        public _32Bit_Register m_R7 = new _32Bit_Register();
        public _32Bit_Register m_R8 = new _32Bit_Register();
        public _32Bit_Register m_R9 = new _32Bit_Register();
        public _32Bit_Register m_R10 = new _32Bit_Register();
        public _32Bit_Register m_R11 = new _32Bit_Register();
        public _32Bit_Register m_R12 = new _32Bit_Register();
        public _32Bit_Register m_R13 = new _32Bit_Register();
        public _32Bit_Register m_R14 = new _32Bit_Register();
        public _32Bit_Register m_R15 = new _32Bit_Register();
        public _32Bit_Register m_R16 = new _32Bit_Register();

        public _8Bit_Register m_CR0 = new _8Bit_Register();
        public _8Bit_Register m_CR1 = new _8Bit_Register();

        public _16Bit_Register m_F = new _16Bit_Register();

        public BUS m_BUS;

        public Instruction m_IR;

        /// <summary>
        /// Flag Zero
        /// </summary>
        public const int FL_Z = 0x0001;

        /// <summary>
        /// Flag equals
        /// </summary>
        public const int FL_E = 0x0002;

        /// <summary>
        /// Flag Signed
        /// </summary>
        public const int FL_S = 0x0004;

        /// <summary>
        /// Flag Carry
        /// </summary>
        public const int FL_C = 0x0008;

        /// <summary>
        /// Flag overflow
        /// </summary>
        public const int FL_O = 0x0010;

        /// <summary>
        /// flag less
        /// </summary>
        public const int FL_L = 0x0020;

        /// <summary>
        /// Flag interrupt
        /// </summary>
        public const int FL_I = 0x0040;

        /// <summary>
        /// flag Halted
        /// </summary>
        public const int FL_H = 0x0080;

        /// <summary>
        /// flag has Jumped
        /// </summary>
        public const int FL_J = 0x0100;

        /// <summary>
        /// Flag overflow
        /// </summary>
        public const int FL_U = 0x0200;

        /// <summary>
        /// shift flag
        /// </summary>
        public const int FL_SF = 0x0400;

        /// <summary>
        /// shift flag
        /// </summary>
        public const int FL_G = 0x0800;


        public const int CR0_BiosRom = 0x01;
        public const int CR0_FPUEnabel = 0x02;
        public const int CR0_LowPowerMode = 0x04;
        public const int CR0_EnableExtendedRegisters = 0x08;
        public const int CR0_EnableExtendedMode = 0x10;

    }
}
