using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace filesystem
{
    public class FileSystemBFS01 : FileSystemBFSBase
    {
        public FileSystemBFS01()
        {
            EntrySize = 0x20;
            FatAddress = 0x600;
            m_RootAddress = 0x300;
            FiletypeLength = 5;
            SectorsPerPage = 2;
            NameLength = 0xF - FiletypeLength;
            m_formatFunction = FormatDiskNoFiles;
            m_loadFile = BFS01LoadFile;
            m_readSector = ReadSector;
        }

        public void FormatDiskNoFiles(Disk disk)
        {
            FormatDisk(disk);
        }
        public void FormatDisk(Disk disk)
        {
            m_EntryConut = 0;
            CreateDisk(disk);

            if (Program.m_Admin == false)
            {
                if (File.Exists(disk.m_DiskPath))
                {
                    LoadFile(disk);
                    return;
                }
            }

            ushort MaxNumberOfEntrys = (ushort)((m_BytesPerSector - (m_RootAddress - m_BytesPerSector)) / EntrySize);

            byte FatAllocation = (byte)Math.Clamp((int)MathF.Round((float)(m_DiskSize / 8) / m_BytesPerSector / m_BytesPerSector, 0, MidpointRounding.ToPositiveInfinity), 1, 255);
            FatSize = FatAllocation * m_BytesPerSector;

            int newOffset = 0x200;
            // Boot sector

            if (Program.m_Admin)
            {
                ushort rootDirectorySector = (ushort)(m_RootAddress / m_BytesPerSector);
                ushort FATSector = (ushort)(FatAddress / m_BytesPerSector);                
                newOffset = WriteToDisk("BFS01".PadRight(6, ' '), newOffset);
                newOffset = WriteToDisk(m_Heads, newOffset, inline: false);
                newOffset = WriteToDisk(m_TracksPerHead, newOffset, inline: false);
                newOffset = WriteToDisk(m_TotalSectors, newOffset, false);
                newOffset = WriteToDisk(m_SectorsPerTracks, newOffset, false);
                newOffset = WriteToDisk(m_BytesPerSector, newOffset, false);
                newOffset = WriteToDisk(rootDirectorySector, newOffset, inline: false);
                newOffset = WriteToDisk(FATSector, newOffset, inline: false);
                newOffset = WriteToDisk(MaxNumberOfEntrys, newOffset, false);
                newOffset = WriteToDisk(FatAllocation, newOffset, false);
                newOffset = WriteToDisk(FatAllocation, newOffset, false);
                newOffset = WriteToDisk(NameLength + FiletypeLength, newOffset, false);
                newOffset = WriteToDisk("".PadRight(m_RootAddress - newOffset, '\0'), newOffset);
            }
        }

        public void CreateDirectory(string path, bool IsProtected = false, bool IsHidden = false)
        {
            convertToFSPath(path, out string name, out string parentDirectory);
            if (m_Disk.m_FileSystemFormat == FileSystemType.BFS01)
            {
                if (name.Length > NameLength)
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
                if ((entry.m_Flags & IsDirectoryFlag) != IsDirectoryFlag)
                {
                    Console.WriteLine($"{parentDirectory} is not a directory");
                    return;
                }
                offsetDir = (entry.m_StartingPage + 1) * m_BytesPerSector;
                EntryConut = entry.m_EntryCount;
            }
            else
            {
                offsetDir = m_RootAddress;
                EntryConut = m_EntryConut;
            }

            createEntryBFS01(name, 1, offsetDir, ref EntryConut, true, IsProtected, IsHidden);

            if (!string.IsNullOrEmpty(parentDirectory))
            {
                Entry Newentry = entry;
                Newentry.m_EntryCount = (byte)EntryConut;
                updateEntry(ref entry, Newentry);
            }
            else
            {
                m_EntryConut = (ushort)EntryConut;
            }
        }
        public void CreateFile(string path, int size, bool IsProtected = false, bool IsHidden = false)
        {
            convertToFSPath(path, out string name, out string parentDirectory);
            if (m_Disk.m_FileSystemFormat == FileSystemType.BFS01)
            {
                if (name.Length > NameLength)
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
                if ((entry.m_Flags & IsDirectoryFlag) != IsDirectoryFlag)
                {
                    Console.WriteLine($"{parentDirectory} is not a directory");
                    return;
                }
                offsetDir = (entry.m_StartingPage + 1) * m_BytesPerSector;
                EntryConut = entry.m_EntryCount;
            }
            else
            {
                offsetDir = m_RootAddress;
                EntryConut = m_EntryConut;
            }

            createEntryBFS01(name, (ushort)size, offsetDir, ref EntryConut, false, IsProtected, IsHidden);
            if (!string.IsNullOrEmpty(parentDirectory))
            {
                ConventName(entry.m_Name, out string FileName, out string FileType);
                Entry Newentry = new Entry()
                {
                    m_Name = FileName,
                    m_Type = FileType,
                    m_DateTime = entry.m_DateTime,
                    m_EntryCount = entry.m_EntryCount,
                    m_FileSizeInPages = entry.m_FileSizeInPages,
                    m_Flags = entry.m_Flags,
                    m_StartingPage = entry.m_StartingPage,
                };
                Newentry.m_EntryCount = (byte)EntryConut;
                updateEntry(ref entry, Newentry);
            }
            else
            {
                m_EntryConut = (ushort)EntryConut;
            }
        }

        public void BFS01LoadFile(Disk disk)
        {
            if (disk == null)
            {
                return;
            }
            m_Disk = disk;
            m_EntryConut = 0;
            m_DiskBuffer = File.ReadAllBytes(disk.m_DiskPath);

            int offset = 0x206;

            ReadDisk(ref offset, out m_Heads, false);
            ReadDisk(ref offset, out m_TracksPerHead, false);
            ReadDisk(ref offset, out m_TotalSectors, false);
            ReadDisk(ref offset, out m_SectorsPerTracks, false);
            ReadDisk(ref offset, out m_BytesPerSector, false);

            m_TotalTracks = (ushort)(m_TracksPerHead * 2);

            m_DiskSize = m_TracksPerHead * 2 * m_Heads * m_SectorsPerTracks * m_BytesPerSector;
        }

        void updateEntry(ref Entry entry, Entry NewEntry)
        {
            entry.m_EntryCount = NewEntry.m_EntryCount;

            entry.m_DateTime = NewEntry.m_DateTime;

            int offset = getAddressOfEntry(entry) + 0x12;
            offset = WriteToDisk(NewEntry.m_EntryCount, offset, true);
            offset = WriteToDisk(NewEntry.m_DateTime.m_Day, offset);
            offset = WriteToDisk(NewEntry.m_DateTime.m_Month, offset);
            offset = WriteToDisk(NewEntry.m_DateTime.m_Year, offset);
            offset = WriteToDisk(NewEntry.m_DateTime.m_Hour, offset);
            WriteToDisk(NewEntry.m_DateTime.m_Minute, offset);
        }
        int getAddressOfEntry(Entry entry, int startAddress = 0x300)
        {
            int Address = startAddress;
            Entry[] entries = GetEntrysByAddress(Address);
            for (int i = 0; i < entries.Length; i++)
            {
                if (entries[i].m_Name == entry.m_Name && entries[i].m_Type == entry.m_Type && entries[i].m_StartingPage == entry.m_StartingPage)
                {
                    return Address + (i * EntrySize);
                }

                if ((entries[i].m_Flags & IsDirectoryFlag) == IsDirectoryFlag)
                {
                    int result = getAddressOfEntry(entry, convetToAddress(entries[i].m_StartingPage));
                    if (result == 0)
                    {
                        continue;
                    }
                    return result;
                }
            }
            return 0;
        }
        void createEntryBFS01(string name, ushort sizeInSectors, int offsetDir, ref int EntryConut, bool IsDirectory = false, bool IsProtected = false, bool IsHidden = false)
        {
            name = name.PadRight(0x10, '\0');
            Console.WriteLine($"Creating {name} entry");

            ushort page = BFS01findFreeSector(sizeInSectors);
            Console.WriteLine($"{name} got sector {page} with {sizeInSectors} in size");

            byte flag = 0;
            flag |= (byte)(IsDirectory ? IsDirectoryFlag : 0);
            flag |= (byte)(IsHidden ? IsHiddenFlag : 0);
            flag |= (byte)(IsProtected ? IsProtectedFlag : 0);

            int address = offsetDir + EntryConut * EntrySize;

            int offset = address;

            ConventName(name, out string FileName, out string FileType);

            if (FileType != "")
            {
                offset = WriteToDisk(FileName.PadRight(0xB, '\0'), offset);
                offset = WriteToDisk(FileType.PadRight(0x5, '\0'), offset);
            }
            else
            {
                offset = WriteToDisk(FileName.PadRight(0x10, '\0'), offset);
            }
            offset = WriteToDisk(page, offset, false);
            offset = WriteToDisk(sizeInSectors, offset, false);
            offset = WriteToDisk(flag, offset, false);

            // original datetime : 02 04 2024, 16 59 59
            offset = WriteToDisk(05, 10, 2024, 17, 20, (byte)(System.DateTime.Now.Second / 2), offset);
            offset = WriteToDisk((byte)0, offset, false);
            WriteToDisk("".PadRight(EntrySize - (offset - (address)), '\0'), offset);
            EntryConut++;
        }
        public Entry GetEntryByPath(string path)
        {
            path = path.Replace('\\', '/');
            string[] DividePath = path.Split('/');
            int PointedAddress = m_RootAddress;
            int e = 0;

            Console.WriteLine($"Geting Entry from {path}");

            Entry[] entries = null;
            for (int i = 0; i < DividePath.Length; i++)
            {
                entries = GetEntrysByAddress(PointedAddress);
                for (e = 0; e < entries.Length; e++)
                {
                    ConventName(DividePath[i], out string FileName, out string FileType);
                    if (string.IsNullOrEmpty(FileName) && string.IsNullOrEmpty(FileType))
                    {
                        continue;
                    }

                    if (entries[e].m_Name.Contains(FileName, StringComparison.OrdinalIgnoreCase) && entries[e].m_Type.Contains(FileType, StringComparison.OrdinalIgnoreCase))
                    {
                        PointedAddress = convetToAddress(entries[e].m_StartingPage);
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
            int PointedAddress = m_RootAddress;
            int e = 0;

            Entry[] entries = null;
            for (int i = 0; i < DividePath.Length; i++)
            {
                entries = GetEntrysByAddress(PointedAddress);
                for (e = 0; e < entries.Length; e++)
                {
                    if (entries[e].m_Name == DividePath[i])
                    {
                        PointedAddress = convetToAddress(entries[e].m_StartingPage);
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
                address += EntrySize;
            } while (m_DiskBuffer[address] != '\0');
            return entries.ToArray();
        }
        public Entry GetEntryByAddress(int address)
        {
            Entry result = new Entry();
            byte[] name = ReadDisk(ref address, 0xB);
            byte[] type = ReadDisk(ref address, 0x5);
            if (name[0] == 0)
            {
                return null;
            }
            result.m_Name = m_DefualtEncoder.GetString(name).TrimEnd('\0');
            result.m_Type = m_DefualtEncoder.GetString(type).TrimEnd('\0');

            ReadDisk(ref address, out result.m_StartingPage, false);
            ReadDisk(ref address, out result.m_FileSizeInPages, false);
            ReadDisk(ref address, out result.m_Flags, false);
            ReadDisk(ref address, out result.m_DateTime);
            ReadDisk(ref address, out result.m_EntryCount, false);

            return result;
        }

        public ushort BFS01findFreeSector(ushort sizeInSectors = 1)
        {
            int result = -1;
            byte[] buffer = m_DiskBuffer[FatAddress..(FatAddress + FatSize)];
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
            Array.Copy(buffer, 0, m_DiskBuffer, FatAddress, FatSize);
            return (ushort)(result + 2 + (FatSize / m_BytesPerSector));
        }

        public void WriteEntry(Entry entry, byte[] data)
        {
            if (data.Length != m_BytesPerSector)
            {
                Array.Resize(ref data, m_BytesPerSector);
            }

            int entryAddress = convetToAddress(entry.m_StartingPage);
            for (int i = entryAddress; i < entryAddress + m_BytesPerSector; i++)
            {
                m_DiskBuffer[i] = data[i - entryAddress];
            }
        }
        public void WriteEntry(string path, byte[] data)
        {
            convertToFSPath(path, out string file, out string dir);

            Entry entry = GetEntryByPath((dir == null ? "" : dir + "/") + file);

            WriteEntry(entry, data);
        }
        public void ReadEntrySector(Entry entry, out byte[] buffer)
        {
            if (entry == null)
            {
                throw new ArgumentNullException();
            }

            if ((entry.m_Flags & IsDirectoryFlag) == IsDirectoryFlag)
            {
                Console.WriteLine("Entry is a directory");
                buffer = null;
                return;
            }

            List<byte> data = new List<byte>(m_BytesPerSector);

            // FatAddress + (entryStartingPage * m_BytesPerSector)
            // 0x400 + ( * 0x200)

            int entryAddress = convetToAddress(entry.m_StartingPage);

            for (int i = 0; i < m_BytesPerSector; i++)
            {
                int address = entryAddress + i;
                data.Add(m_DiskBuffer[address]);
            }

            buffer = data.ToArray();
        }
        public void ReadEntrySector(Entry entry, int offset, out byte[] buffer)
        {
            if (entry == null)
            {
                throw new ArgumentNullException();
            }

            if ((entry.m_Flags & IsDirectoryFlag) == IsDirectoryFlag)
            {
                Console.WriteLine("Entry is a directory");
                buffer = null;
                return;
            }

            List<byte> data = new List<byte>(m_BytesPerSector);

            int entryAddress = convetToAddress(entry.m_StartingPage);

            for (int i = entryAddress; i < entryAddress + m_BytesPerSector; i++)
            {
                data.Add(m_DiskBuffer[i + offset]);
            }

            buffer = data.ToArray();
        }
        public void ReadEntryPage(string path, out byte[] buffer)
        {
            convertToFSPath(path, out string file, out string dir);

            Entry entry = GetEntryByPath((dir == null ? "" : dir + "/") + file);

            ReadEntrySector(entry, out buffer);
        }

        public class Entry
        {
            public string m_Name;
            public string m_Type;
            public ushort m_StartingPage;
            public ushort m_FileSizeInPages;
            public byte m_Flags;
            public byte m_EntryCount;
            public DateTime m_DateTime;
        }
    }
}
