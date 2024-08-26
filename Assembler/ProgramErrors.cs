using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Text;

namespace assembler
{
    public class ProgramErrors
    {
        public static void DisplayError(string msg)
        {
            if (AssemblerSettings.PrintIntoErrorFile)
            {
                PrintToFile(msg);
            }
            else
            {
                Console.WriteLine(msg);
            }
        }

        public static void DisplayError(string f, params object[] fmt) 
        {
            string msg = "";
            int fmtIndex = 0;
            for (int i = 0; i < f.Length; i++)
            {
                if (f[i] == '{')
                {
                    i++;
                    if (f[i] == 'V')
                    {
                        i++;
                        string s_fmtIndex = fmtIndex.ToString();
                        for (int j = 0; j < s_fmtIndex.Length; j++)
                        {
                            if (s_fmtIndex[j] == f[i])
                            {
                                i++;
                            }
                            else
                            {
                                throw new NotImplementedException("Somethings wrong i can feel it PG-E0000");
                            }
                        }
                        if (f[i] == '}')
                        {
                            msg += fmt[fmtIndex];
                            fmtIndex++;
                        }
                    }
                }
                else
                {
                    msg += f[i];
                }
            }

            if (AssemblerSettings.PrintIntoErrorFile)
            {
                PrintToFile(msg);
            }
            else
            {
                Console.WriteLine(msg);
            }
        }
        static void PrintToFile(string msg)
        {
                Console.WriteLine("error");
            File.WriteAllText(AssemblerSettings.ErrorFile, File.ReadAllText(AssemblerSettings.ErrorFile) + msg);
        }
    }
}
