using filesystem;
using System.Collections.Generic;
using System.IO;

namespace BC16CPUEmulator
{
    public class CPUSettings
    {
        public static string m_InputFile = null;
        public static string m_ConfFile = "";

        public static Dictionary<int, Disk> m_DiskPaths = new Dictionary<int, Disk>();
    }
}