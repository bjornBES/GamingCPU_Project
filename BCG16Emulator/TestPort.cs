﻿using CommonBCGCPU.Types;
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
            m_InterruptIndex = 0;
        }

        public ushort m_PortID { get; set; }
        public ushort m_InterruptIndex { get; set; }
        public Address m_Address { get; set; }
        public bool m_ReadRam { get; set; }
        public bool m_WriteRam { get; set; }
        public bool m_IRQEnable { get; set; }
        public ushort m_Databus { get; set; }
        public ushort m_Outputbus { get; set; }

        public byte Read(out byte data)
        {
            data = (byte)m_Outputbus;
            return data;
        }

        public ushort Read(out ushort data)
        {
            data = m_Outputbus;
            return data;
        }

        public int Read(out int data)
        {
            data = (int)m_Outputbus;
            return data;
        }

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
            }
            m_Databus = 0;
        }
        public void Reset()
        {
        }

        public void Write(byte[] data)
        {
            if (data.Length == 1)
            {
                m_Databus = data[0];
            }
            else
            {
                m_Databus = BitConverter.ToUInt16(data);
            }
        }
    }
}