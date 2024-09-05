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
                case 0x00:
                    InterruptRoutine00h();
                    break;
                case 0x10:
                    InterruptRoutine10h();
                    break;
                default:
                    break;
            }
        }

        public void InterruptRoutine00h()
        {
            switch (A[true])
            {
                case 0x00: // Load interrupt descriptor table
                    if ((CR1 & CR1_BiosRom) > 0x0 && A[false] == 0)
                    {
                        break;
                    }
                    for (int i = 0; i < Memory.InterruptDescriptorTableSize; i++)
                    {
                        Address address = HL + i;
                        byte b = m_BUS.m_Memory.ReadByte(address, MB);
                        Address IDTAddress = i + (A[false] * 1024);
                        m_BUS.m_Memory.WriteByte(IDTAddress, 0x11, b);
                    }
                    break;
                default:
                    break;
            }
        }
        public void InterruptRoutine10h()
        {
            int Routine = A[true];
            Address RoutineAddress = 0 + (Routine * 0x4);
            Address ServiceAddress = m_BUS.m_Memory.ReadDWord(RoutineAddress, 0x11);

            PushInterrupt();
            Jump(ServiceAddress);
        }
    }
}
