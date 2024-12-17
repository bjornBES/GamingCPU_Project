using filesystem;
using System;
using System.IO;

namespace BC16CPUEmulator
{
    public class ArgumentFunctions : CPUSettings
    {
        static bool m_usedInputFile = false;

        public static void GetInputFile(string[] args, ref int i)
        {
            if (m_usedInputFile)
            {
                Console.WriteLine("ERROR: can't have 2 input files");
                Environment.Exit(1);
            }

            i++;
            m_InputFile = args[i];
            //UsedInputFile = true;
        }
        public static void GetCPUConfFile(string[] args, ref int i)
        {
            i++;
            m_ConfFile = args[i];
        }
        public static void SetDriveBinary(string[] args, ref int i)
        {
            i++;
            if (args[i] != "[")
            {

            }
            i++;
            string setting = "";
            while (args[i] != "]")
            {
                setting += args[i].Trim(' ') + ",";
                i++;
            }
            if (args[i] != "]")
            {

            }
            i++;
            string path = args[i];
            decodeSettings(setting, out int index, out bool Use80, out bool writeEnable, out FileSystemType fileSystemType, out int diskSize);
        

            if (m_DiskPaths.ContainsKey(index))
            {
                Console.WriteLine($"Drive {index} in use");
                Environment.Exit(1);
            }

            if (index < 1 || index > 4)
            {
                Console.WriteLine("The drive needs to be within 1 to 4");
                Console.WriteLine("In the arguments you need to put this");
                Console.WriteLine("-d number path");
                Console.WriteLine("where number is the drive number between 1 and 4");
                Console.WriteLine("where path is the path to the disk binary");
                Environment.Exit(1);
            }
                
            Disk disk = new Disk()
            {
                m_DiskPath = Path.GetFullPath(path),
                m_DiskSize = diskSize,
                m_FileSystemFormat = fileSystemType,
                m_Is80Track = Use80,
                m_WriteEnable = writeEnable
            };

            m_DiskPaths.Add(index, disk);
        }

        static void decodeSettings(string setting, out int index, out bool Use80Tracks, out bool writeEnable, out FileSystemType fileSystemType, out int diskSize)
        {
            string[] settings = setting.Split(',');

            index = 0;
            Use80Tracks = false;
            writeEnable = false;
            fileSystemType = FileSystemType.BFS01;
            diskSize = FileSystemBase._144MB;
            for (int i = 0; i < settings.Length; i++)
            {
                if (settings[i].StartsWith("index", StringComparison.OrdinalIgnoreCase))
                {
                    settings[i] = settings[i].Replace("index", "");
                    if (string.IsNullOrEmpty(settings[i]))
                    {
                        i++;
                        index = Convert.ToInt32(settings[i].Replace("index", ""));
                    }
                    else
                    {
                        index = Convert.ToInt32(settings[i].Replace("index", ""));
                    }
                }
                else if (settings[i].StartsWith("ut80", StringComparison.OrdinalIgnoreCase))
                {
                    Use80Tracks = true;
                }
                else if (settings[i].StartsWith("WE", StringComparison.OrdinalIgnoreCase))
                {
                    writeEnable = true;
                }
                else if (settings[i].StartsWith("BFS01", StringComparison.OrdinalIgnoreCase))
                {
                    fileSystemType = FileSystemType.BFS01;
                }
                else if (settings[i].StartsWith("BFS1", StringComparison.OrdinalIgnoreCase))
                {
                    fileSystemType = FileSystemType.BFS01;
                }
                else if (settings[i].StartsWith("FAT12", StringComparison.OrdinalIgnoreCase))
                {
                    fileSystemType = FileSystemType.FAT12;
                }
                else if (settings[i].StartsWith("FAT16", StringComparison.OrdinalIgnoreCase))
                {
                    fileSystemType = FileSystemType.FAT16;
                }
            }
        }
    }
}