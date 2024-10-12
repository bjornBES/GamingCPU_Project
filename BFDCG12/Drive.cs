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
        bool m_debug;
        bool m_use80Track;

        public bool m_Enable;
        public int m_Drive;

        public bool m_DiskSlotInUse = false;
        public FileSystemBase m_DiskSlot;

        ushort m_currentTrack = 0;

        // Input to FDC
        public bool m_Load;
        public bool m_MotorEnableA;
        public bool m_DriveSelB;
        public bool m_DriveSelA;
        public bool m_MotorEnableB;
        public bool m_Direction;
        public byte m_WriteData;
        public bool m_ReadEnable;
        public bool m_FloppyWriteEnable;
        public bool m_HeadSelect;

        // Output from FDC
        public bool m_Clk;
        public bool m_Index;
        public bool m_Track0;
        public bool m_WriteProtect;
        public byte[] m_ReadData;
        public bool m_DiskChange;

        public Drive(bool Use80Track, FileSystemType fileSystemType, int drive)
        {
            m_Drive = drive;
            m_use80Track = Use80Track;
            if (fileSystemType == FileSystemType.BFS01)
            {
                m_DiskSlot = new FileSystemBFS01();
            }
            else
            {
                m_DiskSlot = new FileSystemFAT12();
            }

            m_Enable = true;

            Thread thread = new Thread(new ThreadStart(Update));
            thread.Start();
        }

                byte m_sector = 1;
        int m_sampleIndex = 0;
        double[] m_sample = new double[30];
        public void Update()
        {
            while (m_Enable)
            {
                System.DateTime LastTime = System.DateTime.Now;
                m_Track0 = m_currentTrack == 0;
                if (m_DiskSlotInUse)
                {
                    m_WriteProtect = m_DiskSlot.m_Disk.m_WriteEnable == 0;
                }

                byte head = (byte)(m_HeadSelect ? 1 : 0);

                switch (motorEnabled())
                {
                    case true:
                        m_DiskChange = true;
                        m_Clk = true;
                        m_Index = false;
                        if (m_sector > 18)
                        {
                            m_Index = true;
                            m_sector = 1;
                            Thread.Sleep(7);
                        }

                        if (!motorEnabled())
                        {
                            break;
                        }
                        m_Clk = false;
                        m_DiskChange = false;
                        readSector(head);
                        
                        m_sector++;
                        break;
                    case false:
                        if (m_ReadEnable)
                        {
                            if (m_WriteData == 0xF0)
                            {
                                m_ReadData = new byte[]
                                {
                                    m_sector,
                                };
                                m_Clk = true;
                                while (m_ReadEnable == true)
                                {

                                }

                                m_Clk = false;
                            }
                        }
                        break;
                }
                

                m_ReadData = null;

                System.DateTime TimeNow = System.DateTime.Now;
                double time = (TimeNow - LastTime).TotalMilliseconds;
                if (time < 11)
                {
                    Thread.Sleep((int)((double)11f - time));
                }
                if (m_debug)
                {
                    m_sample[m_sampleIndex] = time;
                    m_sampleIndex++;
                    if (m_sampleIndex >= m_sample.Length)
                    {
                        m_sampleIndex = 0;
                    }
                    Console.SetCursorPosition(0, 0);
                    Console.WriteLine($"Time = {time}\r\nDid sleep = {time < 200}\r\nsample = {m_sample.Sum() / 11}");
                }
            }
        }

        void readSector(byte head)
        {
            if (m_ReadEnable == false)
            {
                return;
            }
            byte[] SectorData = m_DiskSlot.ReadDiskSector(head, m_currentTrack, m_sector);
            //Console.SetCursorPosition(1, 2);
                m_Clk = true;
                m_ReadData = SectorData;

            while (m_ReadEnable == true)
            {

            }

            m_Clk = false;
        }

        bool motorEnabled()
        {
            if (m_Load)
            {
                if (m_DriveSelA)
                {
                    if (m_Drive == 1)
                    {
                        return true;
                    }
                }
                if (m_DriveSelB)
                {
                    if (m_Drive == 2)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void Step()
        {
            if (!m_Direction)
            {
                if (m_use80Track)
                {
                    if (m_currentTrack < 80)
                    {
                        m_currentTrack++;
                    }
                }
                else
                {
                    if (m_currentTrack < 40)
                    {
                        m_currentTrack += 2;
                    }
                }
            }
            else if (m_Direction)
            {
                if (m_use80Track)
                {
                    if (m_currentTrack > 0)
                    {
                        m_currentTrack--;
                    }
                }
                else
                {
                    if (m_currentTrack > 0)
                    {
                        m_currentTrack -= 2;
                    }
                }
            }
        }
    }
}
