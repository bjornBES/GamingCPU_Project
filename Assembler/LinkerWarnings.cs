using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace assembler
{
    public class LinkerWarnings
    {
        static List<string> s_allreadyCalled = new List<string>();
        static Dictionary<string, string> s_warningMessages = new Dictionary<string, string>()
        {
            {"00000", "Missing bytes from address" },
        };
        public static void MissingByteFromAddress(int LineNumber, string file)
        {
            string msg = "";
            string code = "00000";
            if (CalledCodes(code) == 5)
            {
                return;
            }
            s_allreadyCalled.Add(code);
            msg += $"| Linker Warning  ACL-W{code}:\t";
            msg += s_warningMessages[code];
            msg += $"\n|{file}:{LineNumber}\n\n";

            ProgramErrors.DisplayError(msg);
        }

        static int CalledCodes(string code)
        {
            int times = 0;
            for (int i = 0; i < s_allreadyCalled.Count; i++)
            {
                if (s_allreadyCalled[i] == code)
                {
                    times++;
                }
            }
            return times;
        }
    }
}
