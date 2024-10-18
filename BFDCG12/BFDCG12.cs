using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
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
            InterruptIndex = 0xF;            // IRQ15
            PortIDStart = 0x100;
            PortIDEnd = 0x107;
        }

        public void INTA()
        {

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

        ushort m_commandRegister;

        byte[] m_byteBuffer;

        List<ushort> m_arguments = new List<ushort>();
        public Address Address;

        public byte InterruptIndex { get; set; }
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
                        setFlag(ref m_MasterStatusRegister, 0x0002, false);
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
                        byte head = (byte)(m_arguments[0] & 0x00FF);
                        byte drive = (byte)((m_arguments[0] & 0xFF00) >> 8);
                        readDisk(drive, (Size)m_arguments[3], head, m_arguments[1], (byte)m_arguments[2]);

                        m_arguments.Clear();
                        m_commandRegister = 0;
                        setFlag(ref m_MasterStatusRegister, 0x0004, false);
                        m_DataRegister = 0;
                        return;
                    }
                    break;
                case 0x02:
                    if (m_arguments.Count == 4)
                    {
                        byte head = (byte)(m_arguments[0] & 0x00FF);
                        byte drive = (byte)((m_arguments[0] & 0xFF00) >> 8);
                        writeDisk(drive, (Size)m_arguments[3], head, m_arguments[1], (byte)m_arguments[2]);

                        m_arguments.Clear();
                        m_commandRegister = 0;
                        setFlag(ref m_MasterStatusRegister, 0x0004, false);
                        m_DataRegister = 0;
                        return;
                    }
                    break;
                default:
                    break;
            }
        }

        public void Reset()
        {
            m_bufferIndex = 0;
            m_commandRegister = 0;

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
            drive = new Drive(disk.m_FileSystemFormat, diskSlot);

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
                    if (getFlag(m_MasterStatusRegister, 0x0001) == true && getFlag(m_MasterStatusRegister, 0x0002) == false && m_commandRegister != 0)
                    {
                        m_arguments.Add(m_DataRegister);
                        return;
                    }
                    if (getFlag(m_MasterStatusRegister, 0x0001) && getFlag(m_MasterStatusRegister, 0x0002) && m_commandRegister == 0)
                    {
                        m_arguments.Add(data);
                    }
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
                    return result;
                case 0x104:
                    if (getFlag(m_MasterStatusRegister, 0x0001) && getFlag(m_MasterStatusRegister, 0x0002))
                    {
                        if (m_arguments.Count == 2)
                        {
                            List<byte> bytes = new List<byte>();
                            bytes.AddRange(BitConverter.GetBytes(m_arguments[1]));
                            bytes.AddRange(BitConverter.GetBytes(m_arguments[0]));
                            Address = BitConverter.ToUInt32(bytes.ToArray());

                            BUS.WriteBytes(Address, m_byteBuffer);

                            setFlag(ref m_MasterStatusRegister, 0x0001, false);
                            return 0x1100;
                        }
                        else
                        {
                            return 0x0;
                        }
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
        async void readDisk(byte driveIndex, Size Size, byte head, ushort track, byte sector)
        {
            await Task.Run(() =>
            {
                Drive drive = getDrive(driveIndex);
                drive.ReadSector(head, track, sector);

                int Bytesize = 0;

                switch (Size)
                {
                    case Size._128Bytes:
                        Bytesize = 128;
                        break;
                    case Size._256Bytes:
                        Bytesize = 256;
                        break;
                    case Size._512Bytes:
                        Bytesize = 512;
                        break;
                    case Size._1024Bytes:
                        Bytesize = 1024;
                        break;
                    case Size._2048Bytes:
                        Bytesize = 2048;
                        break;
                    case Size._4096Bytes:
                        Bytesize = 4096;
                        break;
                    case Size._8192Bytes:
                        Bytesize = 8192;
                        break;
                    default:
                        break;
                }
                m_byteBuffer = new byte[Bytesize];
                Array.Copy(drive.m_ReadData, m_byteBuffer, Bytesize);
                setFlag(ref m_MasterStatusRegister, 0x0002, true);
                setFlag(ref m_MasterStatusRegister, 0x0001, true);
            });
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
