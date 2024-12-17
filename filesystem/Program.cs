using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Text;

namespace filesystem
{
    public class Program
    {
        public static bool m_Admin = false;
        static List<FileInfo> m_files = new List<FileInfo>();
        static string m_outputFile;
        static string m_bootSectorFile;
        static int m_diskSize = FileSystemBase._144MB;
        static bool m_openCommandLine = false;
        static int Main(string[] args)
        {
            m_Admin = true;
            decodeArguments(args);

            Disk result = new Disk();
            result.m_DiskPath = m_outputFile;
            result.m_DiskSize = m_diskSize;
            result.m_FileSystemFormat = FileSystemType.BFS01;
            FileSystemBFS01 fileSystem = new FileSystemBFS01();

            if (m_openCommandLine)
            {
                openCommandLine(result);
            }

            fileSystem.FormatDiskNoFiles(result);
            byte[] bootsector = File.ReadAllBytes(m_bootSectorFile);
            fileSystem.WriteToDisk(bootsector);
            for (int i = 0; i < m_files.Count; i++)
            {
                string path = $"./{m_files[i].Name}".ToUpper();
                byte[] data = File.ReadAllBytes(m_files[i].FullName);
                int sectorSize = (int)MathF.Round((data.Length / 512f), MidpointRounding.AwayFromZero) + 1;
                fileSystem.CreateFile(path, sectorSize);
                Console.WriteLine($"Created file {m_files[i].Name}");
                fileSystem.WriteEntry(path, data);
                Console.WriteLine("");
            }
            fileSystem.SaveFile();
            /*
            string inputfile = args[0];
            if (!File.Exists(inputfile))
            {
                File.Create(inputfile).Close();
            }
            if (!File.Exists(args[1]))
            {
                File.Create(args[1]).Close();
            }
            Disk HDDdisk = new Disk()
            {
                m_DiskPath = args[1],
                m_DiskLetter = 'C',
                m_DiskSize = DiskSize._20MB,
                WriteEnable = 1,
            };
            Disk floppyDisk1 = new Disk()
            {
                m_DiskPath = inputfile,
                m_DiskSize = DiskSize._F3MB,
                m_DiskLetter = 'A',
                WriteEnable = 1,
            };

            FileSystemBFS01 fileSystem = new FileSystemBFS01();
            fileSystem.FormatDisk(floppyDisk1);
            fileSystem.SaveFile();

            fileSystem.CreateFile("./test.txt", 1);
            fileSystem.WriteEntry("./test.txt", Encoding.ASCII.GetBytes("HELLO WORLD"));
            fileSystem.CreateDirectory("./Users");
            fileSystem.CreateDirectory("./Users/BES");
            fileSystem.CreateFile("./Users/BES/Users.txt", 2);
            fileSystem.WriteEntry("./Users/BES/Users.txt", Encoding.ASCII.GetBytes("USER:BJORNBES\0\nPASSWORD:1234\0"));

            fileSystem.WriteEntry("./Disk/Disk.inf", Encoding.ASCII.GetBytes("Disk: Hello world"));

            fileSystem.ReadEntryPage("./Disk/Disk.inf", out byte[] buffer);

            buffer = fileSystem.ReadDiskSector(0, 0, 2);

            fileSystem.SaveFile();

            fileSystem.FormatDisk(HDDdisk);

            fileSystem.CreateFile("./test.txt", 1);
            fileSystem.SaveFile();
             */
            return 0;
        }

        static void openCommandLine(Disk disk)
        {
            FileSystemBFS01 fileSystem = new FileSystemBFS01();
            fileSystem.BFS01LoadFile(disk);

            string currentDir = "/";

            while (true)
            {
                Console.Write($"{currentDir}:");
                string line = Console.ReadLine();
                string command = line.Split(' ')[0];
                string[] arguments = line.Replace(command, "").Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

                /*
                Console.Write(command + "\t");
                for (int i = 0; i < arguments.Length; i++)
                {
                    Console.Write($"argument{i}:{arguments[i]}");
                }
                Console.WriteLine();
                 */

                if (command == "exit")
                {
                    Console.Clear();
                    break;
                }
                else if (command == "cd")
                {
                    if (arguments.Length == 0)
                    {
                        Console.WriteLine($"{currentDir}");
                        continue;
                    }
                    string path = arguments[0];
                    Console.WriteLine($"cd to {path}");
                }
                else if (command == "dir")
                {
                    FileSystemBFS01.Entry[] entries = fileSystem.GetEntrysByPath(currentDir);

                    Console.WriteLine($"\tDirectory: {currentDir}");

                    Console.WriteLine($"\tLastWriteTime\tLength\t    Name");
                    for (int i = 0; i < entries.Length; i++)
                    {
                        int size = entries[i].m_FileSizeInPages * 512;
                        Console.WriteLine($"\t{entries[i].m_DateTime.m_Day}-{entries[i].m_DateTime.m_Month}-{entries[i].m_DateTime.m_Year}\t\t{size}\t    {entries[i].m_Name.PadRight(0xB, ' ')}.{entries[i].m_Type}");
                    }
                    Console.WriteLine();
                }
                else if (command == "mkdir")
                {
                    string path = currentDir + arguments[0];
                    fileSystem.CreateDirectory(path);
                }
                else if (command == "hexdump")
                {
                    if (arguments.Length <= 0)
                    {
                        Console.WriteLine("hexdump: no arguments found");
                        Console.WriteLine("hexdump ./inputfile.bin");
                        continue;
                    }
                    string path = currentDir + arguments[0];
                    FileSystemBFS01.Entry entry = fileSystem.GetEntryByPath(path);
                    fileSystem.ReadEntrySector(entry, out byte[] bytes);
                    string asciiCharset = "qwertyuiopåasdfghjklæøzxcvbnmQWERTYUIOPÅASDFGHJKLÆØZXCVBNM,.?!+-*/^=<([{\\|&@`~$€£#%'\";:_}])";
                    string asciiText = "";
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        string hex =Convert.ToString(bytes[i], 16).PadLeft(2, '0');
                        char c = Encoding.ASCII.GetString(new byte[] {bytes[i]})[0];

                        if (i % 16 == 0)
                        {
                            if (i != 0)
                            {
                                Console.WriteLine($"\t|{asciiText}");
                                asciiText = "";
                            }
                            else
                            {
                                Console.WriteLine($"");
                            }
                            Console.Write($"{Convert.ToString(i, 16).PadLeft(4, '0')}: ");
                        }

                        if (asciiCharset.Contains(c))
                        {
                            asciiText += c;
                        }
                        else
                        {
                            asciiText += '.';
                        }

                        Console.Write($"{hex} ");
                    }
                    Console.WriteLine($"\t|{asciiText}");
                    Console.WriteLine($"");
                }
            }

            Environment.Exit(0);
        }

        static void decodeArguments(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "[")
                {
                    i++;
                    while (args[i] != "]")
                    {
                        m_files.Add(new FileInfo(args[i]));
                        i++;
                    }
                }
                else if (args[i] == "-o")
                {
                    i++;
                    m_outputFile = args[i];
                }
                else if (args[i] == "-B")
                {
                    i++;
                    m_bootSectorFile = args[i];
                }
                else if (args[i] == "-S")
                {
                    i++;
                    if (args[i].StartsWith("0x"))
                    {
                        args[i] = args[i].Substring(2);
                    }
                    m_diskSize = int.Parse(args[i], System.Globalization.NumberStyles.Any);
                }
                else if (args[i] == "-O")
                {
                    m_openCommandLine = true;
                }
            }
        }
    }

    public class Disk
    {
        public int m_DiskSize;

        public bool m_Is80Track = false;

        public char m_DiskLetter;
        public string m_DiskPath;

        public bool m_WriteEnable;
        public FileSystemType m_FileSystemFormat = FileSystemType.BFS01;

        public int NumberOfPlatters;
        public int NumberOfHeads 
        { 
            get
            {
                return NumberOfPlatters * 2;
            } 
        }
        public int TracksPerSurface;
        public int SectorsPerTrack
        {
            get
            {
                return m_DiskSize / TracksPerSurface * NumberOfHeads * 512;
            }
        }
        public int NumberOfSectors
        {
            get
            {
                return SectorsPerTrack * TracksPerSurface * NumberOfHeads;
            }
        }
    }

    public enum DiskSize
    {
        none = -1,
        Floopy3p5,
        HDD20MB,
    }

    public enum FileSystemType
    {
        BFS01 = 0,
        BFS1 = 0,
        BFS10 = 1,
        FAT12 = 2,
        FAT16 = 3,
    }
}