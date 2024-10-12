using filesystem;
using System.Collections.Generic;
using System.IO;

namespace BCG16CPUEmulator
{
    public class CPUSettings
    {
        public static string m_InputFile = "\0";

        public static List<FileInfo> m_Files = new List<FileInfo>();

        public static Dictionary<int, Disk> m_DiskPaths = new Dictionary<int, Disk>();
    }
}