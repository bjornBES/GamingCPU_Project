﻿using BCG16CPUEmulator.Types;
using System.Collections.Generic;
using System.Threading;

namespace BCG16CPUEmulator
{
    public class BUS
    {
        public Memory m_Memory;
        public BCG16CPU m_CPU;

        public List<IPort> m_ports = new List<IPort>(0x200);

        public BUS() 
        {
            m_Memory = new Memory();
            m_Memory.ConnectBus(this);
            
            m_CPU = new BCG16CPU();
            m_CPU.ConnectBus(this);

        }

        public void Load(string bin)
        {
            Address textSegmentAddress = 0x0000200;
            for (int i = 0; i < bin.Length; i++)
            {
                byte b = (byte)bin[i];
                m_Memory.WriteByte(textSegmentAddress + i, b);
            }
        }

        public void Tick()
        {
            Thread CpuTickThread = new Thread(new ThreadStart(TICK));
            CpuTickThread.Start();
        }

        public void Reset()
        {
            m_ports.ForEach(p =>
            {
                p.Reset();
            });

            m_CPU.ResetCPU();
        }

        void TICK()
        {
            m_ports.ForEach(p =>
            {
                if (p.IRQEnable)
                {
                    m_CPU.IRQ(p);
                }
            });

            m_CPU.Tick();
        }
    }
}