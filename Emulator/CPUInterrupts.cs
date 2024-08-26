using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace emulator
{
    public class CPUInterrupts : CPU_UtilsFunctions
    {
        // |`0xC70000 - 0xC77FFF`|INTERRUPT MEMORY RESERVED  | 32 kb       |unused
        public const int INT_VARIABEL_CURSOR =  0xC70000; // dword


        public void INT(byte InterruptsRoutine)
        {

        }

        public void INT10h()
        {
            byte AH = AX[true];
            byte AL = AX[false];

            byte BH = BX[true];
            byte BL = BX[false];

            byte CH = CX[true];
            byte CL = CX[false];

            byte DH = DX[true];
            byte DL = DX[false];

            switch (AH)
            {
                // PRINT CHAR
                // AL = char to be printed,
                // BL = RED,
                // BH = GREEN,
                // CL = BLUE
                // prints the char in AL
                case 0x0:
                    uint Cursor = BUS.ReadDWord(INT_VARIABEL_CURSOR, 0) + 0xA3FFFF;

                    BUS.Write((int)Cursor, BL);
                    Cursor++;
                    BUS.Write((int)Cursor, BH);
                    Cursor++;
                    BUS.Write((int)Cursor, CL);
                    Cursor++;

                    BUS.Write((int)Cursor, 0b10000000);
                    Cursor++;

                    BUS.Write((int)Cursor, AL);
                    Cursor++;

                    BUS.WriteDWord(INT_VARIABEL_CURSOR, Cursor - 0xA3FFFF, 0);

                    break;
                default:
                    break;
            }
        }
    }
}
