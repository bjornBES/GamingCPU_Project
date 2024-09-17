using System;
using CommonBCGCPU.Types;

namespace CommonBCGCPU
{
    public interface IPort
    {
        public int m_PortIDStart { get; set; }
        public int m_PortIDEnd { get; set; }
        public byte m_InterruptIndex { get; set; }

        public Address m_Address { get; set; }
        public bool m_ReadRam { get; set; }
        public bool m_WriteRam { get; set; }

        public ushort m_Databus { get; set; }
        public ushort m_Outputbus { get; set; }

        public void Tick();
        public void Reset();

        public void Write(byte data, ushort Port);
        public void Write(ushort data, ushort Port);

        public byte Read(out byte data, ushort Port);
        public ushort Read(out ushort data, ushort Port);

        public void ConnectBus(CPUBus bus);
    }
}
