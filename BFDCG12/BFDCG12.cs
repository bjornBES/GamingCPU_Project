using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CommonBCGCPU;
using CommonBCGCPU.Types;
using filesystem;

namespace _BFDCG12
{
    public class BFDCG12 : IPort
    {
        public BFDCG12()
        {
            m_commandRegister = 0;
            m_argumentCommandCount = 0;
            InterruptIndex = 0xF;            // IRQ15
            PortIDStart = 0x100;
            PortIDEnd = 0x107;
        }

        public void ConnectBus(CPUBus bus)
        {
            BUS = bus;
        }

        Drive m_drive1;
        Drive m_drive2;
        Drive m_drive3;
        Drive m_drive4;

        public ushort m_StatusRegister;
        public ushort m_MasterStatusRegister;
        public ushort m_DataRateSelectRegister;
        public ushort m_DataRegister;
        ushort m_bufferIndex;

        ushort m_trackRegister;

        ushort m_currentSector = 0;
        ushort m_currentTrack = 0;
        byte m_currentHead = 0;

        ushort m_commandRegister;
        ushort m_argumentCommandCount;

        bool m_enabled;
        byte[] m_byteBuffer;

        List<ushort> m_arguments = new List<ushort>();

        public byte InterruptIndex { get; set; }
        public Address Address { get => throw new Exception(); set => throw new Exception(); }
        public bool ReadRam { get => throw new Exception(); set => throw new Exception(); }
        public bool WriteRam { get => throw new Exception(); set => throw new Exception(); }
        public ushort Databus { get => throw new Exception(); set => throw new Exception(); }
        public ushort Outputbus { get => throw new Exception(); set => throw new Exception(); }
        public int PortIDStart { get; set; }
        public int PortIDEnd { get; set; }

        public CPUBus BUS { get; set; }

        public byte Read(out byte data, ushort Port)
        {
            data = (byte)readRegister(Port);
            return data;
        }
        public ushort Read(out ushort data, ushort Port)
        {
            data = readRegister(Port);
            return data;
        }
        public void Write(byte data, ushort Port)
        {
            writeRegister(Port, data);
        }
        public void Write(ushort data, ushort Port)
        {
            writeRegister(Port, data);
        }

        public void Tick()
        {
            if (m_commandRegister == 0)
            {
                switch (m_DataRegister)
                {
                    case 0:
                        return;
                    case 0x01:
                    case 0x02:
                    case 0x03:
                    case 0x04:
                        m_commandRegister = m_DataRegister;
                        setFlag(ref m_MasterStatusRegister, 0x0001, true);
                        return;
                    default:
                        break;
                }
            }

            switch (m_commandRegister)
            {
                case 0x01:
                    if (m_arguments.Count == 4)
                    {
                        byte head = (byte)(m_arguments[0] & 0x0F);
                        byte drive = (byte)((m_arguments[0] & 0xC0) >> 6);
                        readDisk(drive, (Size)m_arguments[3], head, m_arguments[1], (byte)m_arguments[2]);

                        m_arguments.Clear();
                        m_commandRegister = 0;
                        setFlag(ref m_MasterStatusRegister, 0x0001, false);
                        return;
                    }
                    break;
                case 0x02:
                    if (m_arguments.Count == 4)
                    {
                        byte head = (byte)(m_arguments[0] & 0x0F);
                        byte drive = (byte)((m_arguments[0] & 0xC0) >> 6);
                        writeDisk(drive, (Size)m_arguments[3], head, m_arguments[1], (byte)m_arguments[2]);

                        m_arguments.Clear();
                        m_commandRegister = 0;
                        setFlag(ref m_MasterStatusRegister, 0x0001, false);
                        return;
                    }
                    break;
                case 0x03:
                    if (m_arguments.Count == 1)
                    {
                        byte drive = (byte)m_arguments[0];
                        recalibrate(drive);
                        m_arguments.Clear();
                        m_commandRegister = 0;
                        setFlag(ref m_MasterStatusRegister, 0x0001, false);
                        return;
                    }
                    break;
                default:
                    break;
            }

            if (getFlag(m_MasterStatusRegister, 0x0001) == true)
            {
                m_arguments.Add(m_DataRegister);
            }
        }

        public void Reset()
        {
            m_bufferIndex = 0;
            m_commandRegister = 0;
            m_argumentCommandCount = 0;

            InterruptIndex = 0x00;            // IRQ0

            PortIDStart = 0x100;
            PortIDEnd = 0x107;
        }


        public void AddDisk(Disk disk, byte diskSlot)
        {
            switch (diskSlot)
            {
                case 1:
                    addDiskDrive(ref m_drive1, disk, diskSlot);
                    break;
                case 2:
                    addDiskDrive(ref m_drive2, disk, diskSlot);
                    break;
                case 3:
                    addDiskDrive(ref m_drive3, disk, diskSlot);
                    break;
                case 4:
                    addDiskDrive(ref m_drive4, disk, diskSlot);
                    break;
                default:
                    break;
            }
        }
        void addDiskDrive(ref Drive drive, Disk disk, int diskSlot)
        {
            drive = new Drive(disk.m_Is80Track, disk.m_FileSystemFormat, diskSlot);

            if (!drive.m_DiskSlotInUse)
            {
                if (drive.m_DiskSlot.m_Disk == null)
                {
                    drive.m_DiskSlot.Format(disk);
                    drive.m_DiskSlot.SaveFile();
                }
                drive.m_DiskSlotInUse = true;
                drive.m_DiskSlot.LoadFile(disk);
            }
        }

        public void RemoveDisk(Disk disk)
        {
            /*
            if (m_drive1.m_DiskSlotInUse && m_drive1.m_DiskSlot.m_Disk == disk)
            {
                RemoveDiskDrive(ref m_drive1);
            }
            else if (m_drive2.m_DiskSlotInUse && m_drive2.m_DiskSlot.m_Disk == disk)
            {
                RemoveDiskDrive(ref m_drive2);
            }
             */
        }
        void removeDiskDrive(ref Drive drive)
        {
            //drive.m_DiskSlot.SaveFile();
            //drive.m_DiskSlot = null;
            //drive.m_DiskSlotInUse = false;
        }

        void writeRegister(ushort index, byte data)
        {
            writeRegister(index, (ushort)data);
        }
        void writeRegister(ushort index, ushort data)
        {
            switch (index)
            {
                case 0x103:
                    m_DataRateSelectRegister = data;
                    break;
                case 0x104:
                    m_DataRegister = data;
                    setFlag(ref m_MasterStatusRegister, 0x0001, true);
                    break;
                default:
                    break;
            }
        }

        bool getFlag(ushort register, int flag)
        {
            return (register & flag) == flag;
        }
        void setFlag(ref ushort register, int flag, bool value)
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

        ushort readRegister(ushort index)
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
                    if (getFlag(m_MasterStatusRegister, 0x0100) == true)
                    {
                        setFlag(ref m_MasterStatusRegister, 0x0100, false);
                    }
                    return result;
                case 0x104:
                    if (getFlag(m_MasterStatusRegister, 0x0200))
                    {
                        if (m_byteBuffer.Length == m_bufferIndex)
                        {
                            setFlag(ref m_MasterStatusRegister, 0x0200, false);
                            return m_DataRegister;
                        }
                        return m_byteBuffer[m_bufferIndex++];
                    }
                    return m_DataRegister;
                default:
                    break;
            }
            return 0;
        }

        void writeDisk(byte driveIndex, Size Size, byte head, ushort track, byte sector)
        {

        }
        void readDisk(byte driveIndex, Size Size, byte head, ushort track, byte sector)
        {
            seek(track, driveIndex);

            while (getFlag(m_MasterStatusRegister, 0x0100) == false)
            {

            }

            setFlag(ref m_MasterStatusRegister, 0x0100, true);

            ushort sizeInBytes = 0;

            switch (Size)
            {
                case Size._128Bytes:
                    sizeInBytes = 128;
                    break;
                case Size._256Bytes:
                    sizeInBytes = 256;
                    break;
                case Size._512Bytes:
                    sizeInBytes = 512;
                    //ReadData(drive, 512, head, sector);
                    break;
                case Size._1024Bytes:
                    sizeInBytes = 1024;
                    break;
                default:
                    break;
            }

            Drive drive = getDrive(driveIndex);

            if (head == 0)
            {
                drive.m_HeadSelect = false;
            }
            else
            {
                drive.m_HeadSelect = true;
            }

            drive.m_DriveSelA = true;
            drive.m_Load = false;

            drive.m_ReadEnable = true;
            drive.m_WriteData = 0xF0;

            while(drive.m_Clk == false) { }

            byte currnetDriveSector = drive.m_ReadData[0];
        
            drive.m_ReadEnable = false;
            
            while (currnetDriveSector != sector)
            {
                if (sector == 1)
                {
                    break;
                }
                drive.m_Load = true;
                Thread.Sleep(11);
                while (drive.m_Clk == true && drive.m_DiskChange == true)
                { 
                }
                drive.m_Load = false;
                Thread.Sleep(11);
                drive.m_ReadEnable = true;
                drive.m_WriteData = 0xF0;
                while (drive.m_Clk == false)
                { 
                }
                currnetDriveSector = drive.m_ReadData[0];
                drive.m_ReadEnable = false;
            }
            drive.m_Load = false;

            readData(driveIndex, sizeInBytes);
        }

        void recalibrate(byte driveIndex)
        {
            Drive drive = getDrive(driveIndex);
            drive.m_Direction = true;
            while (!drive.m_Track0)
            {
                drive.Step();
            }
        }

        async void readData(byte driveIndex, ushort size)
        {
            Drive drive = getDrive(driveIndex);
            List<byte> bytes = new List<byte>();

            await Task.Run(() =>
            {
                drive.m_Load = true;
                drive.m_ReadEnable = true;

                while (bytes.Count < size)
                {
                    if (drive.m_Clk == true)
                    {
                        if (drive.m_ReadData == null)
                        {
                            continue;
                        }
                        bytes.AddRange(drive.m_ReadData);
                        drive.m_ReadEnable = false;
                        break;
                    }
                }
                return;
            });

            setFlag(ref m_MasterStatusRegister, 0x0100, true);
            setFlag(ref m_MasterStatusRegister, 0x0200, true);
            if (BUS != null)
            {
                BUS.IRQ(this);
            }
            m_bufferIndex = 0;
            m_byteBuffer = bytes.ToArray();

            return;
        }

        void seek(ushort Track, byte drive)
        {
            setFlag(ref m_MasterStatusRegister, 0x0100, false);

            Drive result = getDrive(drive);

            m_preTrack = Track;

            doSeek(result);
        }
        ushort m_preTrack;
        async void doSeek(Drive drive)
        {
            if (drive.m_Track0)
            {
                await Task.Run(() =>
                {
                    drive.m_Direction = false;
                    for (int i = 0; i < m_preTrack; i++)
                    {
                        m_trackRegister++;
                        drive.Step();
                    }
                });
            }
            else
            {
                // TrackRegister = Track right now
                // PreTrack = Track we need
                for (int i = 0; i < 80; i++)
                {
                    if (m_trackRegister == m_preTrack)
                    {
                        break;
                    }

                    if (m_trackRegister < m_preTrack)
                    {
                        drive.m_Direction = false;
                    }
                    else
                    {
                        drive.m_Direction = true;
                    }
                    drive.Step();
                }
            }

            setFlag(ref m_MasterStatusRegister, 0x0100, true);
        }
        Drive getDrive(int drive)
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
}
