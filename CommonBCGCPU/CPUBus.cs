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
        public delegate void WriteMemoryFunction(Address address, byte data);
        public delegate byte ReadMemoryFunction(Address address);
        public delegate byte[] ReadMemoryBytesFunction(Address address, int count);
        public delegate void WriteMemoryBytesFunction(Address address, byte[] data);

        public funcIRQ IRQ;
        public funcNMI NMI;

        public WriteMemoryFunction WriteMemory;
        public ReadMemoryFunction ReadMemory;
        public ReadMemoryBytesFunction ReadBytes;
        public WriteMemoryBytesFunction WriteBytes;
    }
}
