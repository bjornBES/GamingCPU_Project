using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace assembler
{
    public class AssemblerWarnings
    {
        static List<string> s_allreadyCalled = new List<string>();
        static Dictionary<string, string> s_warningMessages = new Dictionary<string, string>()
        {
            {"00000", "Operand is bigger then a {V1} byte(s)" },
            {"00001", "" },
            {"00002", "Redundant Code becurse of a JUMP instruction" },
        };

        public static void OperandToBig(Token token, string file, string line, int orgSize)
        {
            string code = "00000";
            if (CalledCodes(code) == 5)
            {
                return;
            }
            s_allreadyCalled.Add(code);
            Console.Write($"| Assembler Warning  ");
            Console.Write($"ACL-W{code}:\t");
            Console.WriteLine(s_warningMessages[code].Replace("{V1}", orgSize.ToString()));
            Console.WriteLine($"| \t{line}");
            Console.WriteLine($"| \n{file}:{token.line},{token.column}");
            Console.WriteLine("");
        }

        public static void RedundantCode(int LineNumber, string file, string line)
        {
            string code = "00001";
            if (CalledCodes(code) == 5)
            {
                return;
            }
            s_allreadyCalled.Add(code);

            Console.Write($"Assembler Warning  ");
            Console.Write($"ACL-W{code}:\t");
            Console.WriteLine(s_warningMessages[code]);
            Console.WriteLine($"\t{line}");
            Console.WriteLine($"\n{file}:{LineNumber}");
        }
        public static void RedundantCodeBecurseOfJump(int LineNumber, string file, string line)
        {
            string code = "00002";
            if (CalledCodes(code) == 5)
            {
                return;
            }
            s_allreadyCalled.Add(code);

            Console.Write($"Assembler Warning  ");
            Console.Write($"ACL-W{code}:\t");
            Console.WriteLine(s_warningMessages[code]);
            Console.WriteLine($"\t{line}");
            Console.WriteLine($"\n{file}:{LineNumber}");
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
