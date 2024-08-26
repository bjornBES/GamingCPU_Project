using System;
using System.Collections.Generic;
using System.Text;

namespace assembler
{
    public class LinkerErrors
    {
        static Dictionary<string, string> s_warningMessages = new Dictionary<string, string>()
        {
            { "00000", "symbol to '{V0}' has not been referenced" }
        };

        public static void MissingLabel(string name, int line, string file)
        {
            string msg = "";

            const string code = "00000";
            msg += PrintStd(code);
            msg += s_warningMessages[code];
            msg += PrintEnd(line, file);
            ProgramErrors.DisplayError(msg, name);
            Environment.Exit(1);
        }

        static string PrintStd(string code)
        {
            string str = "";
            str += $"|";
            str += $" Linker Error  ";
            str += $"ACL-E{code}:\t\t";
            return str;
        }
        static string PrintEnd(int line, string file)
        {
            string str = "";
            str += $"| \n";
            str += $"| {file}:{line}\n\n";
            return str;
        }
    }
}
