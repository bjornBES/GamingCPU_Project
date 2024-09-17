using CommonBCGCPU.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonBCGCPU
{
    public class CPUBus
    {
        public delegate void funcIRQ(IPort port);
        public delegate void funcNMI(IPort port);
        public delegate void WriteMemoryFunction(Address address, byte bank, byte data);
        public delegate byte ReadMemoryFunction(Address address, byte bank);
        public delegate byte[] ReadMemoryBytesFunction(Address address, byte bank, int count);

        public funcIRQ IRQ;
        public funcNMI NMI;

        public WriteMemoryFunction WriteMemory;
        public ReadMemoryFunction ReadMemory;
        public ReadMemoryBytesFunction ReadBytes;
    }
}
