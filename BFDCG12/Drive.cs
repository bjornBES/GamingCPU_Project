using System.Threading;
using System.Threading.Tasks;
using filesystem;
using System;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using System.ComponentModel.Design;

namespace _BFDCG12
{
    public class Drive
    {
        public int m_Drive;

        public bool m_DiskSlotInUse = false;
        public FileSystemBase m_DiskSlot;

        // Input to FDC
        public byte[] m_WriteData;

        // Output from FDC
        public byte[] m_ReadData;

        public Drive(FileSystemType fileSystemType, int drive)
        {
            m_Drive = drive;
            if (fileSystemType == FileSystemType.BFS01)
            {
                m_DiskSlot = new FileSystemBFS01();
            }
            else
            {
                m_DiskSlot = new FileSystemFAT12();
            }
        }

        public void ReadSector(byte head, ushort track, ushort sector)
        {
            m_ReadData = m_DiskSlot.ReadDiskSector(head, track, sector);
        }

        public void WriteSector(byte head, ushort track, ushort sector)
        {
            m_DiskSlot.WriteDiskSector(head, track, sector, m_WriteData);
        }
    }
}
