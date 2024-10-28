using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace filesystem
{
    public class FileSystemBase
    {
        public const int _144MB = 2880 * 512;
        public const int _20MB = (1024 * 1024) * 20;

        public Encoding m_DefualtEncoder = Encoding.ASCII;
        public ushort m_RootAddress = 0x300;

        public int m_DiskSize;
        public byte[] m_DiskBuffer;
        public ushort m_BytesPerSector;
        public byte m_Heads;
        public ushort m_TotalSectors;
        public byte m_SectorsPerTracks;
        public ushort m_TotalTracks;
        public ushort m_TracksPerHead;
        public ushort m_HeadsPerCylinder;

        public Disk m_Disk;


        public const ushort IsDirectoryFlag =   0b0000_0001;
        public const ushort IsHiddenFlag =      0b0000_0010;
        public const ushort IsProtectedFlag =   0b0000_0100;
        public const ushort IsReadOnlyFlag =    0b0001_0000;

        internal delegate void FormatFunction(Disk disk);
        internal delegate byte[] ReadSector(byte head, ushort track, ushort sector);
        internal delegate void WriteSector(byte head, ushort track, ushort sector, byte[] bytes);

        internal FormatFunction m_formatFunction;
        internal FormatFunction m_loadFile;
        internal ReadSector m_readSector;
        internal WriteSector m_writeSector;

        public int GetAddressFromLBA(int lba) => lba * m_BytesPerSector;
        public int GetLBA(byte head, ushort track, ushort sector) => (track * m_HeadsPerCylinder + head) * m_SectorsPerTracks + (sector - 1);

        public void Format(Disk disk)
        {
            m_formatFunction(disk);
        }
        public void CreateDisk(Disk disk)
        {
            if (!File.Exists(disk.m_DiskPath))
            {
                File.Create(disk.m_DiskPath).Close();
                Thread.Sleep(100);
            }

            m_Disk = disk;

            m_BytesPerSector = 0x200;
            m_RootAddress = 0x300;
            m_HeadsPerCylinder = 2;
            switch (disk.m_DiskSize)
            {
                case filesystem.DiskSize._20MB:
                    m_Heads = 4;
                    m_DiskSize = _20MB; // 20 MB
                    m_TracksPerHead = 80;
                    break;
                case filesystem.DiskSize._40MB:
                    break;
                case filesystem.DiskSize._60MB:
                    break;
                case filesystem.DiskSize._F5MB:
                    break;
                case filesystem.DiskSize._F3MB:
                    m_Heads = 2;
                    m_DiskSize = _144MB; // 1,44 MB
                    m_TracksPerHead = 80;
                    break;
                default:
                    break;
            }

            m_TotalTracks = (ushort)(m_TracksPerHead * m_Heads);
            m_TotalSectors = (ushort)(m_DiskSize / m_BytesPerSector);
            m_SectorsPerTracks = (byte)(m_TotalSectors / m_TotalTracks);

            m_DiskBuffer = new byte[m_DiskSize];
            m_DiskBuffer.Initialize();
        }
        public byte[] ReadDiskSector(byte head, ushort track, ushort sector)
        {
            return m_readSector(head, track, sector);
        }
        public void WriteDiskSector(byte head, ushort track, ushort sector, byte[] data)
        {
            m_writeSector(head, track, sector, data);
        }
        public void SaveFile()
        {
            File.WriteAllBytes(m_Disk.m_DiskPath, m_DiskBuffer);
        }
        public void LoadFile(Disk disk)
        {
            if (disk == null)
            {
                m_formatFunction(disk);
            }

            m_Disk = disk;
            m_DiskBuffer = File.ReadAllBytes(disk.m_DiskPath);

            m_loadFile(disk);
        }

        public int WriteToDisk(byte day, byte month, ushort year, byte hour, byte minute, byte seconds, int offset = 0)
        {
            ushort time;
            ushort date;

            time = (ushort)((hour & 0b1_1111) << 11);
            time |= (ushort)(minute << 5);
            time |= (ushort)(seconds / 2);

            date = (ushort)((year & 0b1111111) << 9);
            date |= (ushort)(month << 5);
            date |= day;

            offset = WriteToDisk(date, offset, false);
            offset = WriteToDisk(time, offset, false);
            return offset;
        }
        public int WriteToDisk(System.DateTime dateTime, int offset = 0)
        {
            offset = WriteToDisk((byte)dateTime.Day, (byte)dateTime.Month, (ushort)(dateTime.Year - 2024), (byte)dateTime.Hour, (byte)dateTime.Minute, (byte)dateTime.Second, offset);
            return offset;
        }
        public int WriteToDisk(ushort data, int offset = 0, bool inline = false)
        {
            string DatHex = Convert.ToString(data, 16).PadLeft(4, '0');
            if (inline)
            {
                return WriteToDisk(DatHex, offset);
            }
            else
            {
                byte high = Convert.ToByte(DatHex.Substring(0, 2), 16);
                byte low = Convert.ToByte(DatHex.Substring(2, 2), 16);
                byte[] bytestr = BitConverter.GetBytes(data);
                return WriteToDisk(bytestr, offset);
            }
        }
        public int WriteToDisk(int data, int offset = 0, bool inline = false)
        {
            string DatHex = Convert.ToString(data, 16).PadLeft(8, '0');
            if (inline)
            {
                return WriteToDisk(DatHex, offset);
            }
            else
            {
                byte highhigh = Convert.ToByte(DatHex.Substring(0, 2), 16);
                byte highlow = Convert.ToByte(DatHex.Substring(2, 2), 16);
                byte lowhigh = Convert.ToByte(DatHex.Substring(4, 2), 16);
                byte lowlow = Convert.ToByte(DatHex.Substring(6, 2), 16);
                byte[] bytestr = { highhigh, highlow, lowhigh, lowlow };
                return WriteToDisk(bytestr, offset);
            }
        }
        public int WriteToDisk(byte data, int offset = 0, bool inline = false)
        {
            string DatHex = Convert.ToString(data, 16).PadLeft(2, '0');
            if (inline)
            {
                return WriteToDisk(DatHex, offset);
            }
            else
            {
                byte[] bytestr = { Convert.ToByte(DatHex, 16) };
                return WriteToDisk(bytestr, offset);
            }
        }
        public int WriteToDisk(string data, int offset = 0)
        {
            byte[] buffer = m_DefualtEncoder.GetBytes(data);
            return WriteToDisk(buffer, offset);
        }
        public int WriteToDisk(byte[] data, int offset = 0)
        {
            int i;
            for (i = 0; i < data.Length; i++)
            {
                m_DiskBuffer[i + offset] = data[i];
            }
            return i + offset;
        }

        public DateTime ReadDisk(ref int offset)
        {
            DateTime dateTime = new DateTime();

            ushort date = ReadDisk(ref offset, out ushort _, false);
            ushort time = ReadDisk(ref offset, out ushort _, false);

            // 17 20 ??

            // 0bYYYYYMMM_MMMSSSSS
            dateTime.m_Hour = (byte)((time   & 0b11111000_00000000) >> 11);
            dateTime.m_Minute = (byte)((time & 0b00000111_11100000) >> 5);
            dateTime.m_Second = (byte)((time & 0b00000000_00011111));

            // 05 10 0000

            dateTime.m_Year = (ushort)((date >> 11) & 0b1111111);
            dateTime.m_Month = (byte)((date >> 5));
            dateTime.m_Day = (byte)((date &     0b00000000_00001111));

            /*
            time = (ushort)((hour & 0b1_1111) << 11);
            time |= (ushort)(minute << 5);
            time |= (ushort)(seconds / 2);

            date = (ushort)((year & 0b1111111) << 9);
            date |= (ushort)(month << 5);
            date |= day;
             */

            return dateTime;
        }
        public int ReadDisk(ref int offset, out int result, bool inline = false)
        {
            byte[] bytestr = ReadDisk(ref offset, 4);
            if (inline)
            {
                result = Convert.ToInt32(m_DefualtEncoder.GetString(bytestr));
                return result;
            }
            else
            {
                result = BitConverter.ToInt32(bytestr, 0);
                return result;
            }
        }
        public ushort ReadDisk(ref int offset, out ushort result, bool inline = false)
        {
            byte[] bytestr = ReadDisk(ref offset, 2);
            if (inline)
            {
                result = Convert.ToUInt16(m_DefualtEncoder.GetString(bytestr));
                return result;
            }
            else
            {
                result = BitConverter.ToUInt16(bytestr.ToArray(), 0);
                return result;
            }
        }
        public byte ReadDisk(ref int offset, out byte result, bool inline = false)
        {
            byte[] bytestr = ReadDisk(ref offset, 1);
            if (inline)
            {
                result = Convert.ToByte(m_DefualtEncoder.GetString(bytestr));
                return result;
            }
            else
            {
                result = bytestr.First();
                return result;
            }
        }
        public void ReadDisk(ref int offset, out DateTime result)
        {
            result = ReadDisk(ref offset);
        }
        public string ReadDisk(ref int offset, int count, out string result)
        {
            byte[] bytestr = ReadDisk(ref offset, count);
            result = m_DefualtEncoder.GetString(bytestr);
            return result;
        }
        public byte[] ReadDisk(ref int offset, int count)
        {
            int i;
            List<byte> buffer = new List<byte>();
            for (i = 0; i < count; i++)
            {
                buffer.Add(m_DiskBuffer[i + offset]);
            }
            offset += count;
            return buffer.ToArray();
        }
        public byte[] ReadDisk(int offset, int count)
        {
            int i;
            List<byte> buffer = new List<byte>();
            for (i = 0; i < count; i++)
            {
                buffer.Add(m_DiskBuffer[i + offset]);
            }
            return buffer.ToArray();
        }

    }
}
