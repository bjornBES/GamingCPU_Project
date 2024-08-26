using emulator.register;

namespace emulator
{
    public class CPUFields
    {
#pragma warning disable IDE1006 // Naming Styles
        public _16Bit_Register AX;
        public _16Bit_Register BX;
        public _16Bit_Register CX;
        public _16Bit_Register DX;

        public _32Bit_Register HL;

        public _16Bit_Register DS;
        public _16Bit_Register FDS;
        public _16Bit_Register S;

        public _32Bit_Register PC;

        public _32BitFloatRegister FA;
        public _32BitFloatRegister FB;

        public _16Bit_Register SP;
        public _16Bit_Register BP;

        public byte IL;

        public _16Bit_Register R1;
        public _16Bit_Register R2;
        public _16Bit_Register R3;
        public _16Bit_Register R4;

        public _32BitFloatRegister F1;
        public _32BitFloatRegister F2;
        public _32BitFloatRegister F3;
        public _32BitFloatRegister F4;

        public byte MB;
        
        public _16Bit_Register Flags;

        public const int StackOffset = 0xA00000;
        public BUS BUS;
#pragma warning restore IDE1006 // Naming Styles
    }
}
