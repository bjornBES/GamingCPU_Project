using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace filesystem
{
    public class FileSystemBFS01 : FileSystemBase
    {
        public FileSystemBFS01()
        {
            m_FormatFunction = FormatDisk;
            m_LoadFile = BFS01LoadFile;
            m_ReadSector = _ReadSector;
        }

        public ushort entryConut;
        public const ushort entrySize = 0x20;

        public void FormatDisk(Disk disk)
        {
            entryConut = 0;
            CreateDisk(disk);
            ushort MaxNumberOfEntrys = (ushort)((BytesPerSector - (rootAddress - BytesPerSector)) / entrySize);

            byte FatAllocation = (byte)Math.Clamp((int)MathF.Round((float)(DiskSize / 8) / BytesPerSector / BytesPerSector, 0, MidpointRounding.ToPositiveInfinity), 1, 255);

            int newOffset = 0x200;
            // Boot sector

            newOffset = WriteToDisk("BFS01".PadRight(6, ' '), newOffset);                 // header
            newOffset = WriteToDisk((byte)disk.m_DiskLetter, newOffset);                  // disk letter
            newOffset = WriteToDisk(heads, newOffset, inline: false);                     // Number of heads
            newOffset = WriteToDisk(TracksPerHead, newOffset, inline: false);             // Number of Tracks per side
            newOffset = WriteToDisk(TotalSectors, newOffset, false);                      // Number of sectors
            newOffset = WriteToDisk(SectorsPerTracks, newOffset, false);                  // Sectors per Track
            newOffset = WriteToDisk(BytesPerSector, newOffset, false);                    // Bytes Per Sector
            newOffset = WriteToDisk(rootAddress, newOffset, inline: true);                // root directory address
            newOffset = WriteToDisk(MaxNumberOfEntrys, newOffset, false);                 // max number of root directory entrys 
            newOffset = WriteToDisk(FatAllocation, newOffset, false);                     // Allocated FAT in sectors
            newOffset = WriteToDisk("Hello world".PadLeft(16, ' '), newOffset);           // volume label
            newOffset = WriteToDisk("".PadRight(rootAddress - newOffset, '\0'), newOffset);

            if (Program.Admin)
            {
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

        public void CreateDirectory(string path, bool IsProtected = false, bool IsHidden = false)
        {
            ConvertToFSPath(path, out string name, out string parentDirectory);
            if (m_Disk.FileSystemFormat == FileSystemType.BFS01)
            {
                if (name.Length > 0xF)
                {
                    Console.WriteLine("entry name is to long for BFS01");
                    return;
                }
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

            CreateEntryBFS01(name, 1, offsetDir, ref EntryConut, true, IsProtected, IsHidden);

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
            if (m_Disk.FileSystemFormat == FileSystemType.BFS01)
            {
                if (name.Length > 0xF)
                {
                    Console.WriteLine("entry name is to long for BFS01");
                    return;
                }
            }
            else if (m_Disk.FileSystemFormat == FileSystemType.FAT12)
            {
                if (name.Length > 0xB)
                {
                    Console.WriteLine("entry name is to long for FAT12");
                    return;
                }
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

            CreateEntryBFS01(name, (ushort)size, offsetDir, ref EntryConut, false, IsProtected, IsHidden);
            if (!string.IsNullOrEmpty(parentDirectory))
            {
                ConventName(entry.name, out string FileName, out string FileType);
                Entry Newentry = new Entry()
                {
                    name = FileName,
                    type = FileType,
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

        public void BFS01LoadFile(Disk disk)
        {
            if (m_Disk == null)
            {
                FormatDisk(disk);
            }

            m_Disk = disk;
            entryConut = 0;
            DiskBuffer = File.ReadAllBytes(disk.m_DiskPath);

            int offset = 0x200;

            char DiskIdent = (char)ReadDisk(ref offset, out byte _, false);
            ReadDisk(ref offset, out heads, false);
            ReadDisk(ref offset, out TracksPerHead, false);
            ReadDisk(ref offset, out TotalSectors, false);
            ReadDisk(ref offset, out SectorsPerTracks, false);
            ReadDisk(ref offset, out BytesPerSector, false);

            TotalTracks = (ushort)(TracksPerHead * 2);

            DiskSize = (TracksPerHead * 2) * heads * SectorsPerTracks * BytesPerSector;
        }

        void UpdateEntry(ref Entry entry, Entry NewEntry)
        {
            entry.EntryCount = NewEntry.EntryCount;

            entry.dateTime = NewEntry.dateTime;

            int offset = (entry.ContinueAddress - 0x20) + 0x12;
            offset = WriteToDisk(NewEntry.EntryCount, offset, true);
            offset = WriteToDisk(NewEntry.dateTime.Day, offset);
            offset = WriteToDisk(NewEntry.dateTime.Month, offset);
            offset = WriteToDisk(NewEntry.dateTime.Year, offset);
            offset = WriteToDisk(NewEntry.dateTime.Hour, offset);
            WriteToDisk(NewEntry.dateTime.Minute, offset);
        }
        void CreateEntryBFS01(string name, ushort sizeInSectors, int offsetDir, ref int EntryConut, bool IsDirectory = false, bool IsProtected = false, bool IsHidden = false)
        {
            name = name.PadRight(0xF, '\0');

            ushort page = BFS01findFreeSector(name, sizeInSectors);

            ushort flag = 0;
            flag |= (ushort)(IsDirectory ? IsDirectoryFlag : 0);
            flag |= (ushort)(IsHidden ? IsHiddenFlag : 0);
            flag |= (ushort)(IsProtected ? IsProtectedFlag : 0);

            int address = offsetDir + EntryConut * entrySize;

            int offset = address;

            ConventName(name, out string FileName, out string FileType);

            if (FileType != "")
            {
                offset = WriteToDisk(FileName.PadRight(0xA, '\0'), offset);
                offset = WriteToDisk(FileType.PadRight(0x5, '\0'), offset);
            }
            else
            {
                offset = WriteToDisk(FileName.PadRight(0xF, '\0'), offset);
            }
            offset = WriteToDisk(page, offset, false);
            offset = WriteToDisk(sizeInSectors, offset, false);
            offset = WriteToDisk(flag, offset, false);
            if (IsDirectory)
            {
                offset = WriteToDisk((byte)00, offset);
            }
            else
            {
                offset = WriteToDisk((byte)0xFF, offset);
            }

            offset = WriteToDisk(02, 04, 2024, 16, 59, offset);
            offset = WriteToDisk((ushort)(address + entrySize), offset, false);
            WriteToDisk("".PadRight(entrySize - (offset - (address)), '\0'), offset);
            EntryConut++;
        }
        void ConventName(string name, out string FileName, out string FileType)
        {
            if (!name.Contains('.'))
            {
                FileName = name;
                FileType = "";
                return;
            }
            FileName = name.Split('.')[0];
            FileType = name.Split('.')[0];
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
                    ConventName(DividePath[i], out string FileName, out string FileType);
                    if (entries[e].name == FileName && entries[e].type == FileType)
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
                if (entry == null)
                    break;
                entries.Add(entry);
                address += entrySize;
            } while (DiskBuffer[address] != '\0');
            return entries.ToArray();
        }
        public Entry GetEntryByAddress(int address)
        {
            Entry result = new Entry();
            byte[] name = ReadDisk(ref address, 0xA);
            byte[] type = ReadDisk(ref address, 0x5);
            if (name[0] == 0)
            {
                return null;
            }
            result.name = DefualtEncoder.GetString(name).TrimEnd('\0');
            result.type = DefualtEncoder.GetString(type).TrimEnd('\0');

            ReadDisk(ref address, out result.StartingPage, false);

            ReadDisk(ref address, out result.FileSizeInPages, false);

            ReadDisk(ref address, out result.Flags, false);

            ReadDisk(ref address, out result.EntryCount, false);

            result.dateTime = ReadDisk(ref address);

            ReadDisk(ref address, out result.ContinueAddress, false);

            return result;
        }

        public ushort BFS01findFreeSector(string filename, ushort sizeInSectors = 1)
        {
            int result = -1;
            if (filename != "fat.disk".PadRight(0xF, '\0'))
            {
                ReadEntryPage("fat.disk", out byte[] buffer);
                int doneSize = 0;
                for (int i = 0; i < buffer.Length; i++)
                {
                    if (doneSize == sizeInSectors)
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

                        if (doneSize == sizeInSectors)
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
        public byte[] _ReadSector(byte head, ushort track, ushort sector)
        {
            byte[] buffer = new byte[BytesPerSector];

            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = ReadByte(head, track, sector, (ushort)i, out _);
            }

            return buffer;
        }
        public byte ReadByte(byte head, ushort track, ushort sector, ushort a, out byte buffer)
        {
            int LBA = GetLBA(head, track, sector);
            int address = GetAddressFromLBA(LBA) + a;

            buffer = ReadDisk(address, 1)[0];
            return buffer;
        }

        int GetAddressFromEntry(Entry entry) => 0x400 + ((entry.StartingPage - 1) * BytesPerSector);

        public class Entry
        {
            public string name;
            public string type;
            public ushort StartingPage;
            public ushort FileSizeInPages;
            public ushort Flags;
            public byte EntryCount;
            public DateTime dateTime;
            public ushort ContinueAddress;
        }
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
