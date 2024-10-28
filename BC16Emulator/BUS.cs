using CommonBCGCPU.Types;
using CommonBCGCPU;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using _BFDCG12;
using filesystem;
using BC16CPUEmulator;

namespace BCG16CPUEmulator
{
    public class BUS
    {
        public Memory m_Memory;
        public BC16CPU m_CPU;

        public CPUBus m_CPUBus;

        public List<IPort> m_Ports = new List<IPort>(0x200);

        public BUS() 
        {
            new InstructionArguments();

            m_Memory = new Memory();
            m_Memory.ConnectBus(this);
            m_Memory.Reset();

            m_CPU = new BC16CPU();
            m_CPU.ConnectBus(this);

            m_CPUBus = new CPUBus();

            m_CPUBus.NMI = NMI;
            m_CPUBus.IRQ = IRQ;

            m_CPUBus.ReadMemory = m_Memory.ReadByte;
            m_CPUBus.ReadBytes = m_Memory.ReadBytes;
            m_CPUBus.ReadVRAM = m_Memory.ReadVRAM;
            m_CPUBus.WriteMemory = m_Memory.WriteByte;
            m_CPUBus.WriteBytes = m_Memory.WriteBytes;

            VideoPort videoPort = new VideoPort();
            videoPort.ConnectBus(m_CPUBus);
            m_Ports.Add(videoPort);

            KeyBoard keyBoard = new KeyBoard();
            keyBoard.m_Scrren = videoPort.m_Scrren;
            keyBoard.ConnectBus(m_CPUBus);
            m_Ports.Add(keyBoard);

            BFDCG12 bfdcg12 = new BFDCG12();
            bfdcg12.Reset();

            Dictionary<int, Disk> paths = CPUSettings.m_DiskPaths;
            for (int i = 1; i < 4; i++)
            {
                if (!paths.ContainsKey(i))
                {
                    continue;
                }

                Disk disk = paths[i];

                bfdcg12.AddDisk(disk, (byte)i);
            }

            m_Ports.Add(bfdcg12);

            //Environment.Exit(0);
        }

        public void Load(byte[] bin)
        {
            Array.Copy(bin, 0, m_Memory.m_Mem, 0x1200, bin.Length);
        }

        public void Tick()
        {
            Thread thread = new Thread(new ThreadStart(tick));
            thread.Start();

            m_Ports.ForEach(p =>
            {
                p.Tick();
            });

            while (thread.ThreadState == ThreadState.Running)
            {

            }
        }

        public void Reset()
        {
            m_Ports.ForEach(p =>
            {
                p.Reset();
                p.ConnectBus(m_CPUBus);
            });

            m_CPU.ResetCPU();
        }

        void tick()
        {
            m_CPU.Tick();
        }

        public void Write(Address address, int data)
        {

            if (address >= 0 && address <= 0x200 - 1)
            {
                return;
            }
            if ((m_CPU.m_CR0 & 0x11) == 0x00)
            {
                address = address & 0x00FFFF;
            }
            else if ((m_CPU.m_CR0 & 0x01) == 0x01)
            {
                address = address & 0x0FFFFF;
            }
            else if ((m_CPU.m_CR0 & 0x11) == 0x11)
            {
                address = address & 0xFFFFFF;
            }
            else if ((m_CPU.m_CR0 & 0x10) == 0x10)
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

            m_Ports.ForEach(port =>
            {
                if (port.PortIDStart <= Port && port.PortIDEnd >= Port)
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

            m_Ports.ForEach(port =>
            {
                if (port.PortIDStart <= Port && port.PortIDEnd >= Port)
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

            m_Ports.ForEach(port =>
            {
                if (port.PortIDStart <= Port && port.PortIDEnd >= Port)
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

            m_Ports.ForEach(port =>
            {
                if (port.PortIDStart <= Port && port.PortIDEnd >= Port)
                {
                    port.Read(out result, (ushort)Port);
                }
            });

            Data = result;
            return result;
        }

        public void IRQ(IPort port)
        {
            m_CPU.IRQ(port.InterruptIndex);
        }
        public void NMI(IPort port)
        {
            m_CPU.NMI(port.InterruptIndex);
        }
        public void INTA(byte port)
        {
            m_Ports.ForEach(p =>
            {
                if (p.InterruptIndex == port)
                {
                    p.INTA();
                }
            });
        }
    }
}