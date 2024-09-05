using System;
using System.IO;
using System.Text;

namespace filesystem
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 0)
            {
                Console.WriteLine("on input file");
                return 1;
            }
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

            FileSystem fileSystem = new FileSystem();
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

            fileSystem.ReadSector(0, 0, 2, out buffer);

            fileSystem.SaveFile();

            fileSystem.FormatDisk(HDDdisk);

            fileSystem.CreateFile("./test.txt", 1);
            fileSystem.SaveFile();
            return 0;
        }
    }

    public class Disk
    {
        public DiskSize m_DiskSize;

        public char m_DiskLetter;
        public string m_DiskPath;

        public byte WriteEnable;
    }

    public enum DiskSize
    {
        /// <summary>
        /// 20 MB HDD
        /// </summary>
        _20MB = 0x00,
        _40MB = 0x01,
        _60MB = 0x02,

        /// <summary>
        /// 
        /// </summary>
        _F5MB = 0x80,
        
        /// <summary>
        /// 1,44 MB Floppy
        /// </summary>
        _F3MB = 0x81,
    }
}