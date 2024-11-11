using CommonBCGCPU;
using filesystem;
using System;
using System.Collections.Generic;

namespace _BFDCG12
{
    public class Program
    {
        static void Main(string[] args)
        {
            new Program();
        }

        public Program()
        {
            Disk disk1 = new Disk();
            disk1.m_DiskLetter = 'A';
            disk1.m_DiskPath = "D:/disk1.bin";
            disk1.m_DiskSize = DiskSize._F3MB;
            disk1.m_WriteEnable = 1;
            IPort port = new BFDCG12();
            port.Reset();
            ((BFDCG12)port).AddDisk(disk1, 1);

            port.Write(0x03, 0x104);
            port.Tick();
            port.Write(0x00, 0x104);
            port.Tick();
            port.Tick();

            port.Write(0x02, 0x104);
            port.Tick();
            port.Write(0x00, 0x104);                // drive + head
            port.Tick();
            port.Write(0x00, 0x104);                // track
            port.Tick();
            port.Write(0x02, 0x104);                // sector
            port.Tick();
            port.Write(0x02, 0x104);                // size
            port.Tick();
            port.Tick();

            port.Write(0x01, 0x104);
            port.Tick();
            port.Write(0x00, 0x104);                // drive + head
            port.Tick();
            port.Write(0x00, 0x104);                // track
            port.Tick();
            port.Write(0x02, 0x104);                // sector
            port.Tick();
            port.Write(0x02, 0x104);                // size
            port.Tick();
            port.Tick();

            //((BFDCG12)port).RemoveDisk(disk1);

            while ((ushort)(port.Read(out ushort _, 0x102) & 0x0200) != 0x0200)
            {
            }

            List<byte> buffer = new List<byte>();

            while ((ushort)(port.Read(out ushort _, 0x102) & 0x0200) == 0x0200)
            {
                port.Read(out byte result, 0x104);
                if ((ushort)(port.Read(out ushort _, 0x102) & 0x0200) != 0x0200)
                {
                    break;
                }
                buffer.Add(result);
            }

        }
    }
}
