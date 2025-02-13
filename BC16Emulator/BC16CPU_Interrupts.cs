﻿using CommonBCGCPU.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BC16CPUEmulator
{
    public class BC16CPU_Interrupts : BC16_ALU
    {
        public void Interrupt(byte routine)
        {
            switch (routine)
            {
                case 0x02:
                    InterruptRoutine02h();
                    break;
                case 0x03:
                    InterruptRoutine03h();
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
            Address ServiceAddress = m_BUS.m_Memory.ReadDWord(RoutineAddress);

            PushInterrupt();
            Jump(ServiceAddress);
        }

        public void InterruptRoutine03h()
        {
            if ((m_CR0 & CR0_EnableProtectedMode) == CR0_EnableProtectedMode)
            {
                int Routine = m_AX[4];
                switch (Routine)
                {
                    case 0x00:
                        m_BUS.m_MMU.Allocate((byte)m_AX[3]); // AL is the ProgramID
                        break;
                    case 0x01:
                        m_BUS.m_MMU.AllocatedBlocks((byte)m_AX[3]); // AL is the ProgramID
                        break;
                    case 0x02:
                        m_BUS.m_MMU.RequestMoreMemory((byte)m_AX[3]); // AL is the ProgramID
                        break;
                    case 0x04:
                        m_BUS.m_MMU.Deallocate((byte)m_AX[3]); // AL is the ProgramID
                        break;
                }
            }
        }

        public void InterruptRoutine04h()
        {
            int Routine = m_AX[4];
            Address RoutineAddress = 0x0080 + (Routine * 0x4);
            Address ServiceAddress = m_BUS.m_Memory.ReadDWord(RoutineAddress);

            PushInterrupt();
            Jump(ServiceAddress);
        }

        public void InterruptRoutine0Fh()
        {
            Address RoutineAddress = 0x0090;
            Address ServiceAddress = m_BUS.m_Memory.ReadDWord(RoutineAddress);

            PushInterrupt();
            Jump(ServiceAddress);
        }
    }
}
