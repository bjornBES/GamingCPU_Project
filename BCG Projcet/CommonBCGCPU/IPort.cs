using System;
using CommonBCGCPU.Types;

namespace CommonBCGCPU
{
    public interface IPort
    {
        public int PortIDStart { get; set; }
        public int PortIDEnd { get; set; }
        public byte InterruptIndex { get; set; }

        public CPUBus BUS { get; set; }

        public void Tick();
        public void Reset();

        public void Write(byte data, ushort Port);
        public void Write(ushort data, ushort Port);

        public byte Read(out byte data, ushort Port);
        public ushort Read(out ushort data, ushort Port);

        public void ConnectBus(CPUBus bus);
        public void INTA();
    }
}
