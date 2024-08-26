using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace emulator
{
#pragma warning disable IDE1006 // Naming Styles
    public interface IPS2_Device
    {
        public byte OutputBit { get; set; }

        public void Tick();
        public void Rev_Byte(byte Data);
    }

    public class PS2_Keyborad : IPS2_Device
    {
        Dictionary<byte, VoidFunction> Commands = new Dictionary<byte, VoidFunction>()
        {
            {0x00, Test }
        };

        delegate void VoidFunction();
        static void Test()
        {

        }

        public byte OutputBit { get; set; }
        byte Input_shift_Register = 0;

        public void Tick()
        {
            if(byte_index_in == 8)
            {

            }
        }

        int byte_index_out = 0;
        bool Send_Byte(byte data)
        {
            OutputBit = Next_Bit_Out(byte_index_out, data);
            if(byte_index_out == 8)
            {
                byte_index_out = 0;
                return true;
            }
            byte_index_out++;
            return false;
        }
        int byte_index_in = 0;
        public void Rev_Byte(byte Data)
        {
            byte bit = (byte)((Data >> byte_index_in++) & 1);
            Input_shift_Register = (byte)((Input_shift_Register << 1) | bit);
        }

        byte Next_Bit_Out(int offset, byte data)
        {
            return (byte)(0x01 & (data << offset));
        }
    }
#pragma warning restore IDE1006 // Naming Styles
}
