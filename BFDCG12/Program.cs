using CommonBCGCPU;
using filesystem;
using System;

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
            disk1.m_DiskPath = "./disk1.bin";
            disk1.m_DiskSize = DiskSize._F3MB;
            disk1.WriteEnable = 1;
            IPort port = new BFDCG12();
            port.Reset();
            ((BFDCG12)port).AddDisk(disk1, 1);

            port.Write(0x01, 0x104);
            port.Tick();
            port.Write(0x00, 0x104);
            port.Tick();
            port.Write(0x00, 0x104);
            port.Tick();
            port.Write(0x01, 0x104);
            port.Tick();
            port.Write(0x02, 0x104);
            port.Tick();
            port.Tick();

            ((BFDCG12)port).RemoveDisk(disk1);
        }
    }
}
