using System;
using System.IO;
using System.Linq;

namespace filesystem
{
    public class FileSystemFAT12 : FileSystemBase
    {
        public FileSystemFAT12()
        {
            m_formatFunction = FormatDisk;
            m_loadFile = FAT12Loadfile;
        }

        public const ushort entrySize =     32;
        public const byte Attribute_ReadOnly =     0x01;
        public const byte Attribute_Hidden =       0x02;
        public const byte Attribute_System =       0x04;
        public const byte Attribute_VolumeLabel =  0x08;
        public const byte Attribute_Subdirectory = 0x10;
        public const byte Attribute_Archive =      0x20;
        public const byte Attribute_Device =       0x40;

        public void FormatDisk(Disk disk)
        {
            CreateDisk(disk);
            if (Program.m_Admin)
            {
                int newOffset = 0;

                newOffset = 3;
                newOffset = WriteToDisk("MSWIN4.1".PadRight(8, ' '), newOffset);              // OEM Name
                newOffset = WriteToDisk(m_BytesPerSector, newOffset);                           // Bytes pre sector
                newOffset = WriteToDisk((byte)2, newOffset);                                  // Sectors per cluster
                newOffset = WriteToDisk((ushort)1, newOffset);                                // Reserved sectors count
                newOffset = WriteToDisk((byte)2, newOffset);                                  // Number of FAT
                newOffset = WriteToDisk((ushort)2, newOffset);                                // Maximum number of root directory entries
                newOffset = WriteToDisk(m_TotalSectors, newOffset);                             // Number of sectors
                newOffset = WriteToDisk((byte)0xF0, newOffset);                               // Media descriptor
                newOffset = WriteToDisk((ushort)0x0009, newOffset);                           // Sectors per FAT
                newOffset = WriteToDisk((ushort)m_SectorsPerTracks, newOffset);                 // Sectors per track
                newOffset = WriteToDisk((ushort)m_Heads, newOffset);                            // Number of heads
                newOffset = WriteToDisk(0x00000000, newOffset);                               // Hidden sectors
                newOffset = WriteToDisk(0x00000000, newOffset);                               // Total sectors

                CreateFile("\\test.txt", 1);
            }
        }

        public void FAT12Loadfile(Disk disk)
        {

        }

        public void CreateDirectory(string path, bool IsProtected = false, bool IsHidden = false)
        {
            SplitPath(path, out string _, out string _, out string parentDirectory);
        }
        public void CreateFile(string path, int size, bool IsProtected = false, bool IsHidden = false)
        {
            SplitPath(path, out string FileName, out string FileType, out string parentDirectory);

            if (FileName.Length > 8)
            {
                throw new NotImplementedException();
            }

            byte Attribute = 0;
            int sectorOffset = 0;

            if (parentDirectory != null)
            {
                throw new NotImplementedException();
            }
            else
            {
                sectorOffset = GetFirstRootDirectorySector();
            }

            CreateEntryFAT12(FileName, FileType, size, sectorOffset, Attribute);
        }

        public void CreateEntryFAT12(string name, string type, int sizeInSectors, int sectorOffset, byte Attribute)
        {
            name = name.PadRight(0xF, '\0');

            ushort FATIndex = FAT12findFreeFAT(sizeInSectors);
        }

        public ushort FAT12findFreeFAT(int sizeInSectors = 1)
        {
            if (m_Disk.m_FileSystemFormat == FileSystemType.FAT12)
            {
                byte SectorsPerCluster = m_DiskBuffer[0x0D];
                int ClusterSize = SectorsPerCluster * m_BytesPerSector;
                int FileSizeInBytes = sizeInSectors * m_BytesPerSector;
                int FileSizeInClusters = (int)MathF.Round((float)FileSizeInBytes / ClusterSize, MidpointRounding.ToPositiveInfinity);

                int FATIndex = FAT12ReadFAT(FileSizeInClusters);
                FAT12WriteFAT(FATIndex, FileSizeInClusters);
                return (ushort)FATIndex;
            }

            return 0;
        }
        // Function to read the FAT and find a sequence of free clusters
        public int FAT12ReadFAT(int numClustersToFind)
        {
            ushort SectorsPerFAT = BitConverter.ToUInt16(m_DiskBuffer[0x16..0x18]);
            byte FATCount = m_DiskBuffer[0x10];
            int FATAddress = m_BytesPerSector * SectorsPerFAT;
            int FATSize = (FATCount * SectorsPerFAT) * m_BytesPerSector;
            int FATEnd = FATAddress + FATSize;
            byte[] buffer = m_DiskBuffer[FATAddress..FATEnd];

            int freeClusterCount = 0;
            int startClusterIndex = -1;

            for (int i = 2; i < buffer.Length * 2 / 3; i++) // FAT12 starts from cluster 2
            {
                int fatEntry = FAT12ReadFATEntry(i + FATAddress);

                if (fatEntry == 0) // 0 indicates a free cluster
                {
                    if (startClusterIndex == -1)
                    {
                        startClusterIndex = i;
                    }

                    freeClusterCount++;

                    if (freeClusterCount == numClustersToFind)
                    {
                        return startClusterIndex; // Found the required number of free clusters
                    }
                }
                else
                {
                    freeClusterCount = 0;
                    startClusterIndex = -1; // Reset if we don't find consecutive free clusters
                }
            }

            return -1; // No sequence of free clusters found
        }

        // Function to write a sequence of clusters to the FAT
        public void FAT12WriteFAT(int startCluster, int numClustersToWrite)
        {
            int previousCluster = startCluster;

            for (int i = 1; i <= numClustersToWrite; i++)
            {
                int nextCluster = (i == numClustersToWrite) ? 0xFFF : startCluster + i; // EOF marker for the last cluster
                FAT12WriteFATEntry(previousCluster, nextCluster);
                previousCluster = nextCluster;
            }
        }

        // Function to read a 12-bit FAT entry from the FAT table
        private int FAT12ReadFATEntry(int clusterIndex)
        {
            int byteOffset = (clusterIndex * 3) / 2;
            int entry;

            if (clusterIndex % 2 == 0)
            {
                entry = m_DiskBuffer[byteOffset] | ((m_DiskBuffer[byteOffset + 1] & 0x0F) << 8);
            }
            else
            {
                entry = ((m_DiskBuffer[byteOffset] & 0xF0) >> 4) | (m_DiskBuffer[byteOffset + 1] << 4);
            }

            return entry;
        }

        // Function to write a 12-bit FAT entry to the FAT table
        private void FAT12WriteFATEntry(int clusterIndex, int value)
        {
            ushort SectorsPerFAT = BitConverter.ToUInt16(m_DiskBuffer[0x16..0x18]);
            byte FATCount = m_DiskBuffer[0x10];
            int FATAddress = m_BytesPerSector * SectorsPerFAT;
            int FATSize = (FATCount * SectorsPerFAT) * m_BytesPerSector;
            int FATEnd = FATAddress + FATSize;
            byte[] buffer = m_DiskBuffer[FATAddress..FATEnd];

            int byteOffset = (clusterIndex * 3) / 2;

            if (clusterIndex % 2 == 0)
            {
                // Write the lower 12 bits
                buffer[byteOffset] = (byte)(value & 0xFF); // Lower 8 bits
                buffer[byteOffset + 1] = (byte)((buffer[byteOffset + 1] & 0xF0) | ((value >> 8) & 0x0F)); // Upper 4 bits
            }
            else
            {
                // Write the upper 12 bits
                buffer[byteOffset] = (byte)((buffer[byteOffset] & 0x0F) | ((value << 4) & 0xF0)); // Lower 4 bits
                buffer[byteOffset + 1] = (byte)((value >> 4) & 0xFF); // Upper 8 bits
            }
            Array.Copy(buffer, 0, m_DiskBuffer, FATAddress, FATSize);
        }

        private int GetFirstRootDirectorySector()
        {
            ushort ReservedSectors = BitConverter.ToUInt16(m_DiskBuffer[0x0E..0x0F]);
            ushort SectorsPerFAT = BitConverter.ToUInt16(m_DiskBuffer[0x16..0x18]);
            byte FATCount = m_DiskBuffer[0x10];

            return ReservedSectors + (FATCount * SectorsPerFAT);
        }
        private void SplitPath(string path, out string fileName, out string fileType, out string parentDirectory)
        {
            string result = path.Replace(Path.PathSeparator, '\\').Replace("./", "\\");
            result = result.TrimStart('\\');

            fileName = result.Split('\\').Last().Split('.').First();
            fileType = result.Split('\\').Last().Split('.').Last();
            if (result.Contains('\\'))
            {
                parentDirectory = result.Replace($"\\{fileName}", "");
            }
            else
            {
                parentDirectory = null;
            }

            if (m_Disk.m_FileSystemFormat == FileSystemType.FAT12)
            {
                if (fileName.Length > 0xB)
                {
                    Console.WriteLine("entry name is to long for FAT12");
                    return;
                }
            }
        }

        public struct Entry
        {
            public char[] m_Name;               // at 0x00: 8
            public char[] m_FileExtension;      // at 0x08: 3
            public byte m_FileAttributes;         // at 0x0B: 1
            // Reserved                            at 0x0C: 1
            public byte m_CreateTimeFine;         // at 0x0D: 1
            public ushort m_CreateTime;           // at 0x0E: 2
            public ushort m_CreateDate;           // at 0x10: 2
            public ushort m_LastAccessDate;       // at 0x12: 2
            public ushort m_EAIndex;              // at 0x14: 2
            public ushort m_LastModTime;          // at 0x16: 2
            public ushort m_LastModDate;          // at 0x18: 2
            public ushort m_FirstCluster;         // at 0x1A: 2
            public int m_FileSizeInByte;          // at 0x1C: 4

            public Entry(string name, 
                         string fileExt,
                         byte fileAttribute,
                         byte createTimeFine,
                         ushort createTime,
                         ushort createDate,
                         ushort lastAccessDate,
                         ushort eAIndex,
                         ushort lastModTime,
                         ushort lastModDate,
                         ushort firstCluster,
                         int fileSizeInBytes)
            {
                m_Name = name.ToCharArray();
                m_FileExtension = fileExt.ToCharArray();

                m_FileAttributes = fileAttribute;
                m_CreateTimeFine = createTimeFine;
                m_CreateTime = createTime;
                m_CreateDate = createDate;
                m_LastAccessDate = lastAccessDate;
                m_EAIndex = eAIndex;
                m_LastModTime = lastModTime;
                m_LastModDate = lastModDate;
                m_FirstCluster = firstCluster;
                m_FileSizeInByte = fileSizeInBytes;
            }
        }
    }
}
