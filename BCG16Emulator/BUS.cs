using CommonBCGCPU.Types;
using CommonBCGCPU;
using System;
using System.Collections.Generic;
using System.Text;
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


            m_ports.Add(new TestPort(0));
        }

        public void Load(byte[] bin)
        {
            Array.Copy(bin, 0, m_Memory.Mem, 0x200, bin.Length);
        }

        public void Tick()
        {
            Thread CpuTickThread = new Thread(new ThreadStart(TICK));
            CpuTickThread.Start();

            while (CpuTickThread.ThreadState == ThreadState.Running)
            {

            }
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
            m_CPU.Tick();
            
            m_ports.ForEach(p =>
            {
                if (p.m_IRQEnable)
                {
                    m_CPU.IRQ(p);
                }
                p.Tick();
            });
        }

        public void Write(Address address, int data)
        {

            if (address >= 0 && address <= 0x200 - 1)
            {
                return;
            }
            if ((m_CPU.CR0 & 0x11) == 0x00)
            {
                address = address & 0x00FFFF;
            }
            else if ((m_CPU.CR0 & 0x01) == 0x01)
            {
                address = address & 0x0FFFFF;
            }
            else if ((m_CPU.CR0 & 0x11) == 0x11)
            {
                address = address & 0xFFFFFF;
            }
            else if ((m_CPU.CR0 & 0x10) == 0x10)
            {
                address = address & 0xF0FFFF;
            }

            if (data >= byte.MinValue && data <= byte.MaxValue)
            {
                m_Memory.WriteByte(address, (byte)data);
            }
        }

        public void OutPort(int Port, byte Data)
        {
            if (!(Port >= 0 && Port <= 0x200 - 1))
            {
                return;
            }

            m_ports.ForEach(port =>
            {
                if (port.m_PortIDStart <= Port && port.m_PortIDEnd >= Port)
                {
                    port.Write(Data, (ushort)Port);
                }
            });
        }
        public void OutPort(int Port, ushort Data)
        {
            if (!(Port >= 0 && Port <= 0x200 - 1))
            {
                return;
            }

            m_ports.ForEach(port =>
            {
                if (port.m_PortIDStart <= Port && port.m_PortIDEnd >= Port)
                {
                    port.Write(Data, (ushort)Port);
                }
            });
        }

        public byte InPort(int Port, out byte Data)
        {
            if (!(Port >= 0 && Port <= 0x200 - 1))
            {
                Data = 0;
                return 0;
            }

            byte result = 0;

            m_ports.ForEach(port =>
            {
                if (port.m_PortIDStart <= Port && port.m_PortIDEnd >= Port)
                {
                    port.Read(out result, (ushort)Port);
                }
            });

            Data = result;
            return result;
        }
        public ushort InPort(int Port, out ushort Data)
        {
            if (!(Port >= 0 && Port <= 0x200 - 1))
            {
                Data = 0;
                return 0;
            }

            ushort result = 0;

            m_ports.ForEach(port =>
            {
                if (port.m_PortIDStart <= Port && port.m_PortIDEnd >= Port)
                {
                    port.Read(out result, (ushort)Port);
                }
            });

            Data = result;
            return result;
        }
    }
}