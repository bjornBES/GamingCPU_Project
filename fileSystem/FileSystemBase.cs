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

        public Encoding DefualtEncoder = Encoding.ASCII;
        public ushort rootAddress = 0x280;

        public int DiskSize;
        public byte[] DiskBuffer;
        public ushort BytesPerSector;
        public byte heads;
        public ushort TotalSectors;
        public byte SectorsPerTracks;
        public ushort TotalTracks;
        public ushort TracksPerHead;

        public Disk m_Disk;


        public const ushort IsDirectoryFlag =   0b0000_0001;
        public const ushort IsHiddenFlag =      0b0000_0010;
        public const ushort IsProtectedFlag =   0b0000_0100;
        public const ushort IsReadOnlyFlag =    0b0001_0000;

        internal delegate void FormatFunction(Disk disk);
        internal delegate byte[] ReadSector(byte head, ushort track, ushort sector);
        internal delegate void WriteSector(byte head, ushort track, ushort sector, byte[] bytes);

        internal FormatFunction m_FormatFunction;
        internal FormatFunction m_LoadFile;
        internal ReadSector m_ReadSector;
        internal WriteSector m_WriteSector;

        public int GetAddressFromLBA(int lba) => lba * BytesPerSector;
        public int GetLBA(byte head, ushort track, ushort sector) => (track * TracksPerHead + head) * SectorsPerTracks + (sector - 1);

        public void Format(Disk disk)
        {
            m_FormatFunction(disk);
        }
        public void CreateDisk(Disk disk)
        {
            if (!File.Exists(disk.m_DiskPath))
            {
                File.Create(disk.m_DiskPath).Close();
                Thread.Sleep(100);
            }

            m_Disk = disk;

            BytesPerSector = 0x200;
            rootAddress = 0x400;
            switch (disk.m_DiskSize)
            {
                case filesystem.DiskSize._20MB:
                    heads = 4;
                    DiskSize = _20MB; // 20 MB
                    TracksPerHead = 80;
                    break;
                case filesystem.DiskSize._40MB:
                    break;
                case filesystem.DiskSize._60MB:
                    break;
                case filesystem.DiskSize._F5MB:
                    break;
                case filesystem.DiskSize._F3MB:
                    heads = 2;
                    DiskSize = _144MB; // 1,44 MB
                    TracksPerHead = 80;
                    break;
                default:
                    break;
            }

            TotalTracks = (ushort)(TracksPerHead * heads);
            TotalSectors = (ushort)(DiskSize / BytesPerSector);
            SectorsPerTracks = (byte)(TotalSectors / TotalTracks);

            DiskBuffer = new byte[DiskSize];
            DiskBuffer.Initialize();
        }
        public byte[] ReadDiskSector(byte head, ushort track, ushort sector)
        {
            return m_ReadSector(head, track, sector);
        }
        public void SaveFile()
        {
            File.WriteAllBytes(m_Disk.m_DiskPath, DiskBuffer);
        }
        public void LoadFile(Disk disk)
        {
            if (disk == null)
            {
                m_FormatFunction(disk);
            }

            m_Disk = disk;
            DiskBuffer = File.ReadAllBytes(disk.m_DiskPath);

            m_LoadFile(disk);
        }

        public int WriteToDisk(byte day, byte month, ushort year, byte hour, byte minute, int offset = 0)
        {
            offset = WriteToDisk(day, offset, false);
            offset = WriteToDisk(month, offset, false);
            offset = WriteToDisk(year, offset, false);
            offset = WriteToDisk(hour, offset, false);
            offset = WriteToDisk(minute, offset, false);
            return offset;
        }
        public int WriteToDisk(System.DateTime dateTime, int offset = 0)
        {
            offset = WriteToDisk((byte)dateTime.Day, offset, false);
            offset = WriteToDisk((byte)dateTime.Month, offset, false);
            offset = WriteToDisk((ushort)dateTime.Year, offset, false);
            offset = WriteToDisk((byte)dateTime.Hour, offset, false);
            offset = WriteToDisk((byte)dateTime.Minute, offset, false);
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
            byte[] buffer = DefualtEncoder.GetBytes(data);
            return WriteToDisk(buffer, offset);
        }
        public int WriteToDisk(byte[] data, int offset = 0)
        {
            int i;
            for (i = 0; i < data.Length; i++)
            {
                DiskBuffer[i + offset] = data[i];
            }
            return i + offset;
        }

        public DateTime ReadDisk(ref int offset)
        {
            DateTime dateTime = new DateTime();
            dateTime.Day = ReadDisk(ref offset, out byte _, false);
            dateTime.Month = ReadDisk(ref offset, out byte _, false);
            dateTime.Year = ReadDisk(ref offset, out byte _, false);
            dateTime.Hour = ReadDisk(ref offset, out byte _, false);
            dateTime.Minute = ReadDisk(ref offset, out byte _, false);
            return dateTime;
        }
        public int ReadDisk(ref int offset, out int result, bool inline = false)
        {
            byte[] bytestr = ReadDisk(ref offset, 4);
            if (inline)
            {
                result = Convert.ToInt32(DefualtEncoder.GetString(bytestr));
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
                result = Convert.ToUInt16(DefualtEncoder.GetString(bytestr));
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
                result = Convert.ToByte(DefualtEncoder.GetString(bytestr));
                return result;
            }
            else
            {
                result = bytestr.First();
                return result;
            }
        }
        public string ReadDisk(ref int offset, int count, out string result)
        {
            byte[] bytestr = ReadDisk(ref offset, count);
            result = DefualtEncoder.GetString(bytestr);
            return result;
        }
        public byte[] ReadDisk(ref int offset, int count)
        {
            int i;
            List<byte> buffer = new List<byte>();
            for (i = 0; i < count; i++)
            {
                buffer.Add(DiskBuffer[i + offset]);
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
                buffer.Add(DiskBuffer[i + offset]);
            }
            return buffer.ToArray();
        }


    }
}
