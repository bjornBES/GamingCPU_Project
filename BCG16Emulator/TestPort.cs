using CommonBCGCPU.Types;
using CommonBCGCPU;
using System;
using System.Collections.Generic;
using System.Text;

namespace BCG16CPUEmulator
{
    public class TestPort : IPort
    {
        public TestPort(ushort portID) 
        {
            m_PortID = portID;
            m_InterruptIndex = 2;       // IRQ 2
        }

        public void ConnectBus(CPUBus bus)
        {
            m_BUS = bus;
        }
        public ushort m_PortID { get; set; }
        public byte m_InterruptIndex { get; set; }
        public Address m_Address { get; set; }
        public bool m_ReadRam { get; set; }
        public bool m_WriteRam { get; set; }
        public ushort m_Databus { get; set; }
        public ushort m_Outputbus { get; set; }
        public int m_PortIDStart { get; set; }
        public int m_PortIDEnd { get; set; }

        CPUBus m_BUS;

        public void Tick()
        {
            if (m_Databus == 0)
                return;

            if (m_Databus == 0x55)
            {
                Console.WriteLine("TEST");
                m_Outputbus = 0x01;
            }

            if (m_Databus == 0xAA)
            {
                Console.WriteLine("TEST 2");
                m_Outputbus = 0x02;

                m_BUS.IRQ(this);
            }

            m_Databus = 0;
        }
        public void Reset()
        {
        }

        public void Write(byte data, ushort Port)
        {
            m_Databus = data;
        }

        public void Write(ushort data, ushort Port)
        {
            m_Databus = data;
        }

        public byte Read(out byte data, ushort Port)
        {
            data = (byte)m_Outputbus;
            return data;
        }

        public ushort Read(out ushort data, ushort Port)
        {
            data = m_Outputbus;
            return data;
        }
    }
}
