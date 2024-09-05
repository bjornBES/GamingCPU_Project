using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CommonBCGCPU;
using CommonBCGCPU.Types;
using filesystem;

namespace BFDCG12
{
    public class BFDCG12 : IPort
    {
        Drive m_drive1;
        Drive m_drive2;
        Drive m_drive3;
        Drive m_drive4;

        public ushort m_StatusRegister;
        public ushort m_MasterStatusRegister;
        public ushort m_DataRateSelectRegister;
        public ushort m_DataRegister;

        ushort m_CurrentSector = 0;
        ushort m_CurrentTrack = 0;
        byte m_CurrentHead = 0;

        ushort CommandRegister;
        ushort ArgumentCommandCount;

        List<ushort> m_Arguments = new List<ushort>();

        public ushort m_InterruptIndex { get; set; }
        public Address m_Address { get => throw new Exception(); set => throw new Exception(); }
        public bool m_ReadRam { get => throw new Exception(); set => throw new Exception(); }
        public bool m_WriteRam { get => throw new Exception(); set => throw new Exception(); }
        public bool m_IRQEnable { get; set; }
        public ushort m_Databus { get => throw new Exception(); set => throw new Exception(); }
        public ushort m_Outputbus { get => throw new Exception(); set => throw new Exception(); }
        public int m_PortIDStart { get; set; }
        public int m_PortIDEnd { get; set; }

        public byte Read(out byte data, ushort Port)
        {
            data = (byte)ReadRegister(Port);
            return data;
        }
        public ushort Read(out ushort data, ushort Port)
        {
            data = ReadRegister(Port);
            return data;
        }
        public void Write(byte data, ushort Port)
        {
            WriteRegister(Port, data);
        }
        public void Write(ushort data, ushort Port)
        {
            WriteRegister(Port, data);
        }

        public void Tick()
        {
            if (CommandRegister == 0)
            {
                switch (m_DataRegister)
                {
                    case 0:
                        return;
                    case 0x01:
                        CommandRegister = m_DataRegister;
                        return;
                    default:
                        break;
                }
            }

            switch (CommandRegister)
            {
                case 0x01:
                    if (m_Arguments.Count == 4)
                    {
                        byte head = (byte)(m_Arguments[0] & 0x0F);
                        byte drive = (byte)((m_Arguments[0] & 0xC0) >> 6);
                        ReadDisk(drive, (Size)m_Arguments[3], head, m_Arguments[1], (byte)m_Arguments[2]);

                        m_Arguments.Clear();
                        CommandRegister = 0;
                        break;
                    }

                    if (GetFlag(m_MasterStatusRegister, 0x0001) == true)
                    {
                        m_Arguments.Add(m_DataRegister);
                        SetFlag(ref m_MasterStatusRegister, 0x0001, false);
                    }

                    break;
                default:
                    break;
            }
        }

        public void Reset()
        {
            CommandRegister = 0;
            ArgumentCommandCount = 0;

            m_InterruptIndex = 0x10;

            m_PortIDStart = 0x100;
            m_PortIDEnd = 0x107;

            m_drive1 = new Drive();
            m_drive2 = new Drive();
            m_drive3 = new Drive();
            m_drive4 = new Drive();
            
            Seek(0, 0, 1, 0);

            while (GetFlag(m_MasterStatusRegister, 0x0100) == false)
            {

            }

            SetFlag(ref m_MasterStatusRegister, 0x0100, false);
            Seek(0, 0, 1, 1);

            while (GetFlag(m_MasterStatusRegister, 0x0100) == false)
            {

            }

            SetFlag(ref m_MasterStatusRegister, 0x0100, false);
            Seek(0, 0, 1, 2);

            while (GetFlag(m_MasterStatusRegister, 0x0100) == false)
            {

            }

            SetFlag(ref m_MasterStatusRegister, 0x0100, false);
            Seek(0, 0, 1, 3);

            while (GetFlag(m_MasterStatusRegister, 0x0100) == false)
            {

            }

            SetFlag(ref m_MasterStatusRegister, 0x0100, false);
        }


        public void AddDisk(Disk disk, byte diskSlot)
        {
            switch (diskSlot)
            {
                case 1:
                    AddDiskDrive(ref m_drive1, disk);
                    break;
                case 2:
                    AddDiskDrive(ref m_drive2, disk);
                    break;
                case 3:
                    AddDiskDrive(ref m_drive3, disk);
                    break;
                case 4:
                    AddDiskDrive(ref m_drive4, disk);
                    break;
                default:
                    break;
            }
        }
        void AddDiskDrive(ref Drive drive, Disk disk)
        {
            if (!drive.m_DiskSlotInUse)
            {
                if (drive.m_DiskSlot.m_Disk == null)
                {
                    drive.m_DiskSlot.FormatDisk(disk);
                    drive.m_DiskSlot.SaveFile();
                }
                drive.m_DiskSlotInUse = true;
                drive.m_DiskSlot.LoadFile(disk);
            }
        }

        public void RemoveDisk(Disk disk)
        {
            if (m_drive1.m_DiskSlotInUse && m_drive1.m_DiskSlot.m_Disk == disk)
            {
                RemoveDiskDrive(ref m_drive1);
            }
            else if (m_drive2.m_DiskSlotInUse && m_drive2.m_DiskSlot.m_Disk == disk)
            {
                RemoveDiskDrive(ref m_drive2);
            }
        }
        void RemoveDiskDrive(ref Drive drive)
        {
            drive.m_DiskSlot.SaveFile();
            drive.m_DiskSlot = null;
            drive.m_DiskSlotInUse = false;
        }

        void WriteRegister(ushort index, byte data)
        {
            WriteRegister(index, (ushort)data);
        }
        void WriteRegister(ushort index, ushort data)
        {
            switch (index)
            {
                case 0x103:
                    m_DataRateSelectRegister = data;
                    break;
                case 0x104:
                    m_DataRegister = data;
                    SetFlag(ref m_MasterStatusRegister, 0x0001, true);
                    break;
                default:
                    break;
            }
        }

        ushort ReadRegister(ushort index)
        {
            ushort result;
            switch (index)
            {
                case 0x100:
                    result = m_StatusRegister;
                    return result;
                case 0x101:
                    break;
                case 0x102:
                    result = m_MasterStatusRegister;
                    if (GetFlag(m_MasterStatusRegister, 0x0100) == true)
                    {
                        SetFlag(ref m_MasterStatusRegister, 0x0100, false);
                    }
                    return result;
                case 0x104:
                    return m_DataRegister;
                default:
                    break;
            }
            return 0;
        }

        void ReadDisk(byte drive, Size Size, byte head, ushort track, byte sector)
        {
            Seek(head, track, sector, drive);

            while (GetFlag(m_MasterStatusRegister, 0x0100) == false)
            {

            }

            SetFlag(ref m_MasterStatusRegister, 0x0100, false);

            switch (Size)
            {
                case Size._128Bytes:
                    break;
                case Size._256Bytes:
                    break;
                case Size._512Bytes:
                    ReadData(m_CurrentHead, m_CurrentTrack, m_CurrentSector, drive, 512);
                    break;
                case Size._1024Bytes:
                    break;
                default:
                    break;
            }

            SetFlag(ref m_MasterStatusRegister, 0x0100, true);
        }

        void ReadData(byte Head, ushort Track, ushort Sector, byte driveIndex, ushort size)
        {
            Drive drive = GetDrive(driveIndex);
        }

        void StepTrack()
        {

        }

        void StepSector()
        {

        }

        void Seek(byte Head, ushort Track, ushort Sector, byte drive)
        {
            Drive result = GetDrive(drive);

            PreHead = Head;
            PreTrack = Track;
            PreSector = Sector;

            Thread thread = new Thread(new ParameterizedThreadStart(DoSeek));
            thread.Start(result);
        }
        byte PreHead;
        ushort PreTrack;
        ushort PreSector;
        async void DoSeek(object odrive)
        {
            Drive drive = (Drive)odrive;

            float time = 0;

            if (PreHead - m_CurrentHead < 0)
            {
                time += (m_CurrentHead - PreHead) * 0.01f;
            }
            else
            {
                time += (PreHead - m_CurrentHead) * 0.01f;
            }

            if (PreTrack - m_CurrentTrack < 0)
            {
                time += (m_CurrentTrack - PreTrack) * 0.01f;
            }
            else
            {
                time += (PreTrack - m_CurrentTrack) * 0.01f;
            }

            if ((PreSector - m_CurrentSector - 1) < 0)
            {
                time += (PreSector - m_CurrentSector - 1) * 0.001f;
            }
            else
            {
                time += (PreSector - m_CurrentSector - 1) * 0.001f;
            }

            await Task.Delay((int)time);
            drive.m_CurrentHead = PreHead;
            drive.m_CurrentTrack = PreTrack;
            drive.m_CurrentSector = PreSector;
            SetFlag(ref m_MasterStatusRegister, 0x0100, true);
        }

        Drive GetDrive(int drive)
        {
            switch (drive)
            {
                case 0:
                    return m_drive1;
                case 1:
                    return m_drive2;
                case 2:
                    return m_drive3;
                case 3:
                    return m_drive4;
                default:
                    break;
            }
            return null;
        }

        bool GetFlag(ushort register, int flag)
        {
            return (register & flag) == flag;
        }
        void SetFlag(ref ushort register, int flag, bool value)
        {
            if (value)
            {
                register |= (ushort)flag;
            }
            else
            {
                register &= (ushort)~(ushort)flag;
            }
        }
    }

    public enum Size
    {
        _128Bytes,
        _256Bytes,
        _512Bytes,
        _1024Bytes,
        _2048Bytes,
        _4096Bytes,
        _8192Bytes,
    }

    public class Drive
    {
        public bool m_DiskSlotInUse = false;
        public FileSystem m_DiskSlot;

        public ushort m_CurrentSector = 0;
        public ushort m_CurrentTrack = 0;
        public byte m_CurrentHead = 0;

        public Drive()
        {
            m_DiskSlot = new FileSystem();
        }
    }
}
