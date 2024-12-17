using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace filesystem
{
    public class FileSystemBFSBase : FileSystemBase
    {
        public ushort m_EntryConut;
        public ushort EntrySize = 0x20;
        public int FatAddress = 0x600;
        public int FatSize = 0;
        public int NameLength = 0xf;
        public int FiletypeLength;
        public int SectorsPerPage = 0;

        public string[] SplitAndMap(int input)
        {
            // Step 1: Split the integer into 8s
            List<int> splitValues = new List<int>();
            while (input > 0)
            {
                int value = Math.Min(input, 8);
                splitValues.Add(value);
                input -= value;
            }

            // Step 2: Map the split values
            Dictionary<int, int> mapping = new Dictionary<int, int>
        {
            { 0x8, 0xFF },
            { 0x7, 0x7F },
            { 0x6, 0x3F },
            { 0x5, 0x1F },
            { 0x4, 0x0F },
            { 0x3, 0x07 },
            { 0x2, 0x03 },
            { 0x1, 0x01 },
            { 0x0, 0x00 },
        };

            List<string> result = new List<string>();

            foreach (var value in splitValues)
            {
                int mappedValue;
                if (mapping.TryGetValue(value, out mappedValue))
                {
                    result.Add(Convert.ToString(mappedValue, 2).PadLeft(8, '0'));
                }
                else
                {
                    throw new Exception("Unexpected value in mapping");
                }
            }

            // Step 3: Return the final result as a hex string array
            return result.ToArray();
        }
        public void ConventName(string name, out string FileName, out string FileType)
        {
            if (!name.Contains('.'))
            {
                FileName = name;
                FileType = "";
                return;
            }
            FileName = name.Split('.')[0].TrimEnd('\0');
            FileType = name.Split('.')[1].TrimEnd('\0');
        }
        public void convertToFSPath(string path, out string filename, out string parentDirectory)
        {
            string result = path.Replace(Path.DirectorySeparatorChar, '/').Replace("./", "");

            filename = result.Split('/').Last();
            if (result.Contains('/'))
            {
                parentDirectory = result.Replace($"/{filename}", "");
            }
            else
            {
                parentDirectory = null;
            }
        }
        public void ReadDisk(int offset, int length, out byte[] buffer)
        {
            List<byte> data = new List<byte>(length);

            for (int i = offset; i < offset + length; i++)
            {
                data.Add(m_DiskBuffer[i]);
            }

            buffer = data.ToArray();
        }
        public byte[] ReadSector(byte head, ushort track, ushort sector)
        {
            byte[] buffer = ReadBytes(head, track, sector, 0, m_BytesPerSector, out _);
            return buffer;
        }
        public byte ReadByte(byte head, ushort track, ushort sector, ushort addressOffset, out byte buffer)
        {
            int LBA = GetLBA(head, track, sector);
            int address = GetAddressFromLBA(LBA) + addressOffset;

            buffer = ReadDisk(address, 1)[0];
            return buffer;
        }
        public byte[] ReadBytes(byte head, ushort track, ushort sector, ushort addressOffset, int count, out byte[] buffer)
        {
            int LBA = GetLBA(head, track, sector);
            int address = GetAddressFromLBA(LBA) + addressOffset;

            buffer = ReadDisk(address, count);
            return buffer;
        }

        public int convetToAddress(int entryStartingPage) => FatAddress + (entryStartingPage * m_BytesPerSector);
    }
}
