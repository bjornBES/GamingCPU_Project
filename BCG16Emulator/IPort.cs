using BCG16CPUEmulator.Types;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Text;

namespace BCG16CPUEmulator
{
    public interface IPort
    {
        public ushort PortID { get; set; }
        public ushort InterruptIndex { get; set; }

        public Address Address { get; set; }
        public bool ReadRam { get; set; }
        public bool WriteRam { get; set; }

        public bool IRQEnable { get; set; }

        public void Tick(ushort Databus);
        public void Reset()
        {
            Address = 0;
            ReadRam = WriteRam = false;
            IRQEnable = false;

        }
    }
}
