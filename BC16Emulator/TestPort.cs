using CommonBCGCPU.Types;
using CommonBCGCPU;
using System;
using System.Collections.Generic;
using System.Text;

namespace BCG16CPUEmulator
{
    public class TestPort : IPort
    {
        public TestPort() 
        {
            InterruptIndex = 2;       // IRQ 2
        }

        public void ConnectBus(CPUBus bus)
        {
            BUS = bus;
        }
        public byte InterruptIndex { get; set; }
        public Address Address { get; set; }
        public bool ReadRam { get; set; }
        public bool WriteRam { get; set; }
        public ushort Databus { get; set; }
        public ushort Outputbus { get; set; }
        public int PortIDStart { get; set; }
        public int PortIDEnd { get; set; }

        public CPUBus BUS { get; set; }

        public void Tick()
        {
            if (Databus == 0)
                return;

            if (Databus == 0x55)
            {
                Console.WriteLine("TEST");
                Outputbus = 0x01;
            }

            if (Databus == 0xAA)
            {
                Console.WriteLine("TEST 2");
                Outputbus = 0x02;

                BUS.IRQ(this);
            }

            Databus = 0;
        }
        public void Reset()
        {
        }

        public void Write(byte data, ushort Port)
        {
            Databus = data;
        }

        public void Write(ushort data, ushort Port)
        {
            Databus = data;
        }

        public byte Read(out byte data, ushort Port)
        {
            data = (byte)Outputbus;
            return data;
        }

        public ushort Read(out ushort data, ushort Port)
        {
            data = Outputbus;
            return data;
        }
    }
}
