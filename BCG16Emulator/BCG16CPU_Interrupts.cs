using CommonBCGCPU.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace BCG16CPUEmulator
{
    public class BCG16CPU_Interrupts : BCG16CPU_HelperFunctions
    {
        public void Interrupt(byte routine)
        {
            switch (routine)
            {
                case 0x02:
                    InterruptRoutine02h();
                    break;
                case 0x04:
                    InterruptRoutine04h();
                    break;
                case 0x10:
                    InterruptRoutine0Fh();
                    break;
                default:
                    break;
            }
        }

        public void InterruptRoutine02h()
        {
            Address RoutineAddress = 0x0004;
            Address ServiceAddress = m_BUS.m_Memory.ReadDWord(RoutineAddress, 0x11);

            PushInterrupt();
            Jump(ServiceAddress);
        }

        public void InterruptRoutine04h()
        {
            int Routine = A[true];
            Address RoutineAddress = 0x0080 + (Routine * 0x4);
            Address ServiceAddress = m_BUS.m_Memory.ReadDWord(RoutineAddress, 0x11);

            PushInterrupt();
            Jump(ServiceAddress);
        }

        public void InterruptRoutine0Fh()
        {
            int Routine = A[true];
            Address RoutineAddress = 0x0400 + (Routine * 0x4);
            Address ServiceAddress = m_BUS.m_Memory.ReadDWord(RoutineAddress, 0x11);

            PushInterrupt();
            Jump(ServiceAddress);
        }
    }
}
