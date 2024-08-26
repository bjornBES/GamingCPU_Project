using assembler.global;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace emulator
{
    public class CPU : CPU_InstructionFunction
    {
        public CPU() 
        {
            return;
        }
        public void ConnectBus(BUS bus)
        {
            BUS = bus;
        }
        public void CPU_RESET()
        {
            MB = 0;
            AX = BX = CX = DX = 0;
            DS = S = 0;
            FDS = 0xF000;
            HL = 0;
            PC = 0;
            FA = FB = 0;
            SP = BP = 0;
            R1 = R2 = R3 = R4 = 0;
            F1 = F2 = F3 = F4 = 0;
            //      0x8421000000000000
            //      0x0000842100000000
            //      0x0000000084210000
            //      0x0000000000008421
            //      0bHU0000IE00LVCSQZ
            Flags = 0b0000001001000101;
        }
        public void CPU_InterruptRequest()
        {

        }
        public void CPU_NonMaskableInterrupt()
        {

        }
        public void CPU_TICK()
        {

            if (GetFlag(0x8000) == false)
            {
                InstructionInfo instruction = DecodeInstruction();
                ExecuteInstruction(instruction);
            }
        }
    }
}
