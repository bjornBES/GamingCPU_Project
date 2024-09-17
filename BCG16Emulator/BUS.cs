using CommonBCGCPU.Types;
using CommonBCGCPU;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using _BFDCG12;
using filesystem;

namespace BCG16CPUEmulator
{
    public class BUS
    {
        public Memory m_Memory;
        public BCG16CPU m_CPU;

        public CPUBus m_CPUBus;

        public List<IPort> m_ports = new List<IPort>(0x200);

        public BUS() 
        {
            new InstructionArguments();

            m_Memory = new Memory();
            m_Memory.ConnectBus(this);
            
            m_CPU = new BCG16CPU();
            m_CPU.ConnectBus(this);

            m_CPUBus = new CPUBus();

            m_CPUBus.NMI = NMI;
            m_CPUBus.IRQ = IRQ;

            m_CPUBus.ReadMemory = m_Memory.ReadByte;
            m_CPUBus.ReadBytes = m_Memory.ReadBytes;
            m_CPUBus.WriteMemory = m_Memory.WriteByte;

            TestPort testPort = new TestPort(0);
            testPort.m_PortIDStart = 0;
            testPort.m_PortIDEnd = 0;
            m_ports.Add(testPort);

            VideoPort videoPort = new VideoPort(0x02);
            m_ports.Add(videoPort);

            Disk TestDisk = new Disk();
            TestDisk.m_DiskSize = DiskSize._F3MB;
            TestDisk.m_DiskLetter = 'A';
            TestDisk.WriteEnable = 1;
            TestDisk.m_DiskPath = "D:/disk1.bin";
            TestDisk.FileSystemFormat = FileSystemType.BFS01;

            Disk Fat12TestDisk = new Disk();
            Fat12TestDisk.m_DiskSize = DiskSize._F3MB;
            Fat12TestDisk.m_DiskLetter = 'B';
            Fat12TestDisk.WriteEnable = 1;
            Fat12TestDisk.m_DiskPath = "D:/Fat12Disk.bin";
            Fat12TestDisk.FileSystemFormat = FileSystemType.FAT12;

            Disk Fat16TestDisk = new Disk();
            Fat16TestDisk.m_DiskSize = DiskSize._F3MB;
            Fat16TestDisk.m_DiskLetter = 'C';
            Fat16TestDisk.WriteEnable = 1;
            Fat16TestDisk.m_DiskPath = "D:/Fat16Disk.bin";
            Fat16TestDisk.FileSystemFormat = FileSystemType.FAT16;

            BFDCG12 bfdcg12 = new BFDCG12();
            bfdcg12.Reset();
            bfdcg12.AddDisk(TestDisk, 1);
            //bfdcg12.AddDisk(Fat12TestDisk, 2);
            //bfdcg12.AddDisk(Fat16TestDisk, 3);
            m_ports.Add(bfdcg12);
            Environment.Exit(0);
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
                p.ConnectBus(m_CPUBus);
            });

            m_CPU.ResetCPU();
        }

        void TICK()
        {
            m_CPU.Tick();
            
            m_ports.ForEach(p =>
            {
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

        public void IRQ(IPort port)
        {
            m_CPU.IRQ(port.m_InterruptIndex);
        }
        public void NMI(IPort port)
        {
            m_CPU.NMI(port.m_InterruptIndex);
        }
        public void INTA(byte port)
        {
            m_ports.ForEach(p =>
            {
                if (p.m_InterruptIndex == port)
                {
                    p.Write(0x7F55, 0);
                }
            });
        }
    }
}