using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace filesystem
{
    public class FileSystem
    {
        const int _144MB = 1_509_949;
        const int _20MB = 20_971_520;

        Encoding DefualtEncoder = Encoding.ASCII;
        ushort rootAddress = 0x300;
        const ushort entrySize = 0x20;
        
        int DiskSize;
        byte[] DiskBuffer;
        ushort BytesPerSector;
        byte heads;
        ushort TotalSectors;
        ushort SectorsPerTracks;
        ushort TotalTracks;
        ushort TracksPerHead;

        bool WriteEnable = false;

        ushort entryConut;

        const ushort IsDirectoryFlag = 0b0000_0000_0000_0001;
        const ushort IsHiddenFlag = 0b0000_0000_0000_0010;
        const ushort IsProtectedFlag = 0b0000_0000_0000_0100;

        const ushort IsReadOnlyFlag = 0b0000_0000_0001_0000;

        public void FormatDisk(Disk disk)
        {
            /*
             1.44 MB is 1,509,949.44 bytes.
20 MB is 20,971,520 bytes.
            */
            entryConut = 0;
            byte MaxNumberOfEntrys;

            BytesPerSector = 0x200;
            rootAddress = 0x300;
            switch (disk.m_DiskSize)
            {
                case filesystem.DiskSize._20MB:
                    heads = 4;
                    TotalTracks = 320;
                    DiskSize = _20MB; // 20 MB
                    TracksPerHead = (ushort)(TotalTracks / 2);
                    break;
                case filesystem.DiskSize._40MB:
                    break;
                case filesystem.DiskSize._60MB:
                    break;
                case filesystem.DiskSize._F5MB:
                    break;
                case filesystem.DiskSize._F3MB:
                    heads = 2;
                    TotalTracks = 80;
                    DiskSize = _144MB; // 1,44 MB
                    TracksPerHead = 40;
                    break;
                default:
                    break;
            }

            MaxNumberOfEntrys = (byte)((BytesPerSector - (rootAddress - BytesPerSector)) / entrySize);
            TotalSectors = (ushort)(DiskSize / BytesPerSector);
            SectorsPerTracks = (ushort)(TotalSectors / (TotalTracks * heads));

            DiskBuffer = new byte[DiskSize];
            DiskBuffer.Initialize();

            byte FatAllocation = (byte)Math.Clamp((int)MathF.Round((float)(DiskSize / 8) / BytesPerSector / BytesPerSector, 0, MidpointRounding.ToPositiveInfinity), 1, 255);

            int newOffset = 0x200;

            // Boot sector

            newOffset = WriteDisk("BFS01", newOffset);                          // header
            newOffset = WriteDisk((byte)disk.m_DiskLetter, newOffset);          // disk letter

            newOffset = WriteDisk(heads, newOffset, inline: false);             // Number of heads
            newOffset = WriteDisk(TracksPerHead, newOffset, inline: false);     // Number of Tracks per side
            newOffset = WriteDisk(TotalSectors, newOffset, false);              // Number of sectors
            newOffset = WriteDisk(SectorsPerTracks, newOffset, false);          // Sectors per Track
            newOffset = WriteDisk(BytesPerSector, newOffset, false);            // Bytes Per Sector

            // more matadata

            newOffset = WriteDisk(rootAddress, newOffset, inline: true);        // root directory address
            newOffset = WriteDisk(disk.WriteEnable, newOffset, false);          // write
            newOffset = WriteDisk(MaxNumberOfEntrys, newOffset, false);         // max number of root directory entrys 
            newOffset = WriteDisk(FatAllocation, newOffset, false);             // Allocated FAT
            newOffset = WriteDisk(entryConut, newOffset, false);                // root directory Entry Conut
            newOffset = WriteDisk("".PadRight(rootAddress - newOffset, '\0'), newOffset);

            string[] test = SplitAndMap(FatAllocation);

            CreateFile("fat.disk", FatAllocation, false, true);

            List<byte> result = new List<byte>();
            for (int i = 0; i < test.Length; i++)
            {
                byte b = Convert.ToByte(test[i], 2);
                result.Add(b);
            }

            WriteEntry("fat.disk", result.ToArray());
            CreateDirectory("Disk", false, true);

            CreateFile("Disk/Disk.inf", 1, false, true);
        }

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

        public void SaveFile(Disk disk)
        {
            int offset = 0x200;

            WriteDisk(entryConut, offset + 0x16, false);
            File.WriteAllBytes(disk.m_DiskPath, DiskBuffer);
        }
        public void LoadFile(Disk disk)
        {
            entryConut = 0;
            DiskBuffer = File.ReadAllBytes(disk.m_DiskPath);
            
            int offset = 0x200;

            string headerVersion = ReadStringDisk(ref offset, 5);
            char DiskIdent = (char)ReadDisk(ref offset, out byte _, false);
            ReadDisk(ref offset, out heads, false);
            ReadDisk(ref offset, out TracksPerHead, false);
            ReadDisk(ref offset, out TotalSectors, false);
            ReadDisk(ref offset, out SectorsPerTracks, false);
            ReadDisk(ref offset, out BytesPerSector, false);

            TotalTracks = (ushort)(TracksPerHead * 2);

            offset += 0x4;

            byte buffer = ReadDisk(ref offset, out byte _, false);
            WriteEnable = Convert.ToBoolean(buffer);

            offset += 1;
            
            offset += 1;

            ReadDisk(ref offset, out entryConut, false);

            DiskSize = (TracksPerHead * 2) * heads * SectorsPerTracks * BytesPerSector;
        }

        public void CreateDirectory(string path, bool IsProtected = false, bool IsHidden = false)
        {
            ConvertToFSPath(path, out string name, out string parentDirectory);
            if (name.Length > 0xA)
            {
                Console.WriteLine("entry name is to long");
                return;
            }
            int offsetDir;
            int EntryConut;
            Entry entry = null;
            if (!string.IsNullOrEmpty(parentDirectory))
            {
                entry = GetEntryByPath(parentDirectory);
                if ((entry.Flags & IsDirectoryFlag) != IsDirectoryFlag)
                {
                    Console.WriteLine($"{parentDirectory} is not a directory");
                    return;
                }
                offsetDir = (entry.StartingPage + 1) * BytesPerSector;
                EntryConut = entry.EntryCount;
            }
            else
            {
                offsetDir = rootAddress;
                EntryConut = entryConut;
            }

            CreateEntry(name, 1, offsetDir, ref EntryConut, true, IsProtected, IsHidden);

            if (!string.IsNullOrEmpty(parentDirectory))
            {
                Entry Newentry = entry;
                Newentry.EntryCount = (byte)EntryConut;
                UpdateEntry(ref entry, Newentry);
            }
            else
            {
                entryConut = (ushort)EntryConut;
            }
        }
        public void CreateFile(string path, int size, bool IsProtected = false, bool IsHidden = false)
        {
            ConvertToFSPath(path, out string name, out string parentDirectory);
            if(name.Length > 0xA)
            {
                Console.WriteLine("entry name is to long");
                return;
            }
            int offsetDir;
            int EntryConut;
            Entry entry = null;
            if (!string.IsNullOrEmpty(parentDirectory))
            {
                entry = GetEntryByPath(parentDirectory);
                if((entry.Flags & IsDirectoryFlag) != IsDirectoryFlag)
                {
                    Console.WriteLine($"{parentDirectory} is not a directory");
                    return;
                }
                offsetDir = (entry.StartingPage + 1) * BytesPerSector;
                EntryConut = entry.EntryCount;
            }
            else
            {
                offsetDir = rootAddress;
                EntryConut = entryConut;
            }

            CreateEntry(name, (ushort)size, offsetDir, ref EntryConut, false, IsProtected, IsHidden);
            if (!string.IsNullOrEmpty(parentDirectory))
            {
                Entry Newentry = new Entry()
                {
                    name = entry.name,
                    ContinueAddress = entry.ContinueAddress,
                    dateTime = entry.dateTime,
                    EntryCount = entry.EntryCount,
                    FileSizeInPages = entry.FileSizeInPages,
                    Flags = entry.Flags,
                    StartingPage = entry.StartingPage,
                };
                Newentry.EntryCount = (byte)EntryConut;
                UpdateEntry(ref entry, Newentry);
            }
            else
            {
                entryConut = (ushort)EntryConut;
            }
        }
        
        void UpdateEntry(ref Entry entry, Entry NewEntry)
        {
            entry.EntryCount = NewEntry.EntryCount;

            entry.dateTime = NewEntry.dateTime;

            int offset = (entry.ContinueAddress - 0x20) + 0x12;
            offset = WriteDisk(NewEntry.EntryCount, offset, true);
            offset = WriteDisk(NewEntry.dateTime.Day, offset);
            offset = WriteDisk(NewEntry.dateTime.Month, offset);
            offset = WriteDisk(NewEntry.dateTime.Year, offset);
            offset = WriteDisk(NewEntry.dateTime.Hour, offset);
            WriteDisk(NewEntry.dateTime.Minute, offset);
        }
        void CreateEntry(string name, ushort sizeInSectors, int offsetDir, ref int EntryConut, bool IsDirectory = false, bool IsProtected = false, bool IsHidden = false)
        {

            name = name.PadRight(0xA, '\0');

            ushort page = findFreeSector(name, sizeInSectors);

            ushort flag = 0;
            flag |= (ushort)(IsDirectory ? IsDirectoryFlag : 0);
            flag |= (ushort)(IsHidden ? IsHiddenFlag : 0);
            flag |= (ushort)(IsProtected ? IsProtectedFlag : 0);

            int address = offsetDir + EntryConut * entrySize;

            int offset = address;

            offset = WriteDisk(name.PadRight(0xA, '\0'), offset);
            offset = WriteDisk(page, offset, false);
            offset = WriteDisk(sizeInSectors, offset, false);
            offset = WriteDisk(flag, offset, false);
            if (IsDirectory)
            {
                offset = WriteDisk((byte)00, offset);
            }
            else
            {
                offset = WriteDisk((byte)0xFF, offset);
            }

            offset = WriteDisk(02, 04, 2024, 16, 59, offset);
            offset = WriteDisk((ushort)(address + entrySize), offset, false);
            WriteDisk("".PadRight(entrySize - (offset - (address)), '\0'), offset);
            EntryConut++;
        }
        public Entry GetEntryByPath(string path)
        {
            path = path.Replace('\\', '/');
            string[] DividePath = path.Split('/');
            int PointedAddress = rootAddress;
            int e = 0;

            Entry[] entries = null;
            for (int i = 0; i < DividePath.Length; i++)
            {
                entries = GetEntrysByAddress(PointedAddress);
                for (e = 0; e < entries.Length; e++)
                {
                    if (entries[e].name == DividePath[i])
                    {
                        PointedAddress = GetAddressFromEntry(entries[e]);
                        break;
                    }
                }
            }
            try
            {
                return entries[e];
            }
            catch (Exception)
            {

                throw;
            }
        }
        public Entry[] GetEntrysByPath(string path)
        {
            path = path.Replace('\\', '/');
            string[] DividePath = path.Split('/');
            int PointedAddress = rootAddress;
            int e = 0;

            Entry[] entries = null;
            for (int i = 0; i < DividePath.Length; i++)
            {
                entries = GetEntrysByAddress(PointedAddress);
                for (e = 0; e < entries.Length; e++)
                {
                    if (entries[e].name == DividePath[i])
                    {
                        PointedAddress = GetAddressFromEntry(entries[e]);
                        break;
                    }
                }
            }
            return entries;
        }
        public Entry[] GetEntrysByAddress(int address)
        {
            List<Entry> entries = new List<Entry>();
            do
            {
                Entry entry = GetEntryByAddress(address);
                if (entry == null) break;
                entries.Add(entry);
                address += entrySize; 
            } while (DiskBuffer[address] != '\0');
            return entries.ToArray();
        }
        public Entry GetEntryByAddress(int address)
        {
            Entry result = new Entry();
            byte[] data = ReadDisk(ref address, 0xA);
            if (data[0] == 0)
            {
                return null;
            }
            result.name = DefualtEncoder.GetString(data).TrimEnd('\0');

            ReadDisk(ref address, out result.StartingPage, false);

            ReadDisk(ref address, out result.FileSizeInPages, false);

            ReadDisk(ref address, out result.Flags, false);

            ReadDisk(ref address, out result.EntryCount, false);

            result.dateTime = ReadDisk(ref address);

            ReadDisk(ref address, out result.ContinueAddress, false);

            return result;
        }

        public ushort findFreeSector(string filename, ushort size = 1)
        {
                int result = -1;
            if (filename != "fat.disk".PadRight(0xA, '\0'))
            {
                ReadEntryPage("fat.disk", out byte[] buffer);
                int doneSize = 0;
                for (int i = 0; i < buffer.Length; i++)
                {
                    if (doneSize == size)
                    {
                        break;
                    }

                    if (buffer[i] == 0xFF)
                    {
                        continue;
                    }

                    for (int b = 0; b < 8; b++)
                    {
                        int Index = Convert.ToInt32(SplitAndMap(b + 1)[0], 2);

                        if ((buffer[i] & Index) != Index)
                        {
                            doneSize++;
                            buffer[i] |= (byte)Index;
                            if (result == -1)
                            {
                                result = (i * 8) + b;
                            }
                        }

                        if (doneSize == size)
                        {
                            break;
                        }
                    }
                }
                WriteEntry("fat.disk", buffer);
            }
            if (result == -1)
            {
                result = 0;
            }
            return (ushort)(result + 1);
        }
        int WriteDisk(byte day, byte month, ushort year, byte hour, byte minute, int offset = 0)
        {
            offset = WriteDisk(day, offset, false);
            offset = WriteDisk(month, offset, false);
            offset = WriteDisk(year, offset, false);
            offset = WriteDisk(hour, offset, false);
            offset = WriteDisk(minute, offset, false);
            return offset;
        }
        int WriteDisk(System.DateTime dateTime, int offset = 0)
        {
            offset = WriteDisk((byte)dateTime.Day, offset, false);
            offset = WriteDisk((byte)dateTime.Month, offset, false);
            offset = WriteDisk((ushort)dateTime.Year, offset, false);
            offset = WriteDisk((byte)dateTime.Hour, offset, false);
            offset = WriteDisk((byte)dateTime.Minute, offset, false);
            return offset;
        }
        int WriteDisk(ushort data, int offset = 0, bool inline = false)
        {
            string DatHex = Convert.ToString(data, 16).PadLeft(4, '0');
            if (inline)
            {
                return WriteDisk(DatHex, offset);
            }
            else
            {
                byte high = Convert.ToByte(DatHex.Substring(0, 2), 16);
                byte low = Convert.ToByte(DatHex.Substring(2, 2), 16);
                byte[] bytestr = BitConverter.GetBytes(data);
                return WriteDisk(bytestr, offset);
            }
        }
        int WriteDisk(int data, int offset = 0, bool inline = false)
        {
            string DatHex = Convert.ToString(data, 16).PadLeft(8, '0');
            if (inline)
            {
                return WriteDisk(DatHex, offset);
            }
            else
            {
                byte highhigh = Convert.ToByte(DatHex.Substring(0, 2), 16);
                byte highlow = Convert.ToByte(DatHex.Substring(2, 2), 16);
                byte lowhigh = Convert.ToByte(DatHex.Substring(4, 2), 16);
                byte lowlow = Convert.ToByte(DatHex.Substring(6, 2), 16);
                byte[] bytestr = { highhigh, highlow, lowhigh, lowlow };
                return WriteDisk(bytestr, offset);
            }
        }
        int WriteDisk(byte data, int offset = 0, bool inline = false)
        {
            string DatHex = Convert.ToString(data, 16).PadLeft(2, '0');
            if (inline)
            {
                return WriteDisk(DatHex, offset);
            }
            else
            {
                byte[] bytestr = { Convert.ToByte(DatHex, 16) };
                return WriteDisk(bytestr, offset);
            }
        }
        int WriteDisk(string data, int offset = 0)
        {
            byte[] buffer = DefualtEncoder.GetBytes(data);
            return WriteDisk(buffer, offset);
        }
        int WriteDisk(byte[] data, int offset = 0)
        {
            int i;
            for (i = 0; i < data.Length; i++)
            {
                DiskBuffer[i + offset] = data[i];
            }
            return i + offset;
        }

        DateTime ReadDisk(ref int offset)
        {
            DateTime dateTime = new DateTime();
            dateTime.Day = ReadDisk(ref offset, out byte _, false);
            dateTime.Month = ReadDisk(ref offset, out byte _, false);
            dateTime.Year = ReadDisk(ref offset, out byte _, false);
            dateTime.Hour = ReadDisk(ref offset, out byte _, false);
            dateTime.Minute = ReadDisk(ref offset, out byte _, false);
            return dateTime;
        }
        int ReadDisk(ref int offset, out int result, bool inline = false)
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
        ushort ReadDisk(ref int offset, out ushort result, bool inline = false)
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
        byte ReadDisk(ref int offset, out byte result, bool inline = false)
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
        string ReadDisk(ref int offset, int count, out string result)
        {
            byte[] bytestr = ReadDisk(ref offset, count);
            result = DefualtEncoder.GetString(bytestr);
            return result;
        }
        string ReadStringDisk(ref int offset, int count)
        {
            string buffer = DefualtEncoder.GetString(ReadDisk(ref offset, count));
            return buffer;
        }
        byte[] ReadDisk(ref int offset, int count)
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
        byte[] ReadDisk(int offset, int count)
        {
            int i;
            List<byte> buffer = new List<byte>();
            for (i = 0; i < count; i++)
            {
                buffer.Add(DiskBuffer[i + offset]);
            }
            return buffer.ToArray();
        }

        void ConvertToFSPath(string path, out string filename, out string parentDirectory)
        {
            string result = path.Replace("\\", "/").Replace("./", "");

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
        public void WriteEntry(Entry entry, byte[] data)
        {
            if (data.Length != BytesPerSector)
            {
                Array.Resize(ref data, BytesPerSector);
            }

            int entryAddress = GetAddressFromEntry(entry);
            for (int i = entryAddress; i < entryAddress + BytesPerSector; i++)
            {
                DiskBuffer[i] = data[i - entryAddress];
            }
        }
        public void WriteEntry(string path, byte[] data)
        {
            ConvertToFSPath(path, out string file, out string dir);

            Entry entry = GetEntryByPath((dir == null ? "" : dir + "/") + file);

            WriteEntry(entry, data);
        }
        public void ReadDisk(int offset, int length, out byte[] buffer)
        {
            List<byte> data = new List<byte>(length);

            for (int i = offset; i < offset + length; i++)
            {
                data.Add(DiskBuffer[i]);
            }

            buffer = data.ToArray();
        }
        public void ReadEntryPage(Entry entry, out byte[] buffer)
        {
            if (entry == null)
            {
                throw new ArgumentNullException();
            }

            if ((entry.Flags & IsDirectoryFlag) == IsDirectoryFlag)
            {
                Console.WriteLine("Entry is a directory");
                buffer = null;
                return;
            }

            List<byte> data = new List<byte>(BytesPerSector);

            int entryAddress = GetAddressFromEntry(entry);

            for (int i = entryAddress; i < entryAddress + BytesPerSector; i++)
            {
                data.Add(DiskBuffer[i]);
            }

            buffer = data.ToArray();
        }
        public void ReadEntryPage(string path, out byte[] buffer)
        {
            ConvertToFSPath(path, out string file, out string dir);

            Entry entry = GetEntryByPath((dir == null ? "" : dir + "/") + file);

            ReadEntryPage(entry, out buffer);
        }
        public void ReadSector(byte head, ushort track, ushort sector, out byte[] buffer)
        {
            int LBA = GetLBA(head, track, sector);
            int address = GetAddressFromLBA(LBA);

            buffer = ReadDisk(address, BytesPerSector);
        }

        int GetAddressFromLBA(int lba) => lba * BytesPerSector;
        int GetLBA(byte head, ushort track, ushort sector) => (track * TracksPerHead + head) * SectorsPerTracks + (sector - 1);
        int GetAddressFromEntry(Entry entry) => 0x400 + ((entry.StartingPage - 1) * BytesPerSector);
    }

    public class Entry
    {
        public string name;
        public ushort StartingPage;
        public ushort FileSizeInPages;
        public ushort Flags;
        public byte EntryCount;
        public DateTime dateTime;
        public ushort ContinueAddress;
    }
    public struct DateTime
    {
        public byte Day;
        public byte Month;
        public ushort Year;
        
        public byte Hour;
        public byte Minute;
    }
}
