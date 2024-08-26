using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace assembler
{
    public class AssemblerError
    {
        static Dictionary<string, string> s_warningMessages = new Dictionary<string, string>()
        {
            {"00000", "Operand is Invalid" },
            {"00001", "Expected \'{V0}\'" },
            {"00002", "Expected an Ident" },
            {"00003", "Expected an Expr" },
            {"00004", "Expected an \'{V0}\'" },
            {"00005", "Instruction is Invalid" },
        };
        public static void ExpectedExpr(Token token, string file)
        {
            string msg = "";
            string code = "00003";
            msg += PrintStd(code);
            msg += s_warningMessages[code] + "\n";
            msg += PrintEnd(token.line, token.column, token.m_file);

            ProgramErrors.DisplayError(msg);
            Environment.Exit(1);
        }
        public static void ExpectedToken(Token token, string file, TokenType expectedToken)
        {
            string msg = "";
            string code = "00004";
            msg += PrintStd(code);
            msg += s_warningMessages[code] + "\n";
            msg += PrintEnd(token.line, token.column, token.m_file);

            ProgramErrors.DisplayError(msg, Tokenizer.tokenToString(expectedToken));
            Environment.Exit(1);
        }
        public static void ExpectedIdent(string file, long line, long column)
        {
            string msg = "";
            string code = "00002";
            msg += PrintStd(code);
            msg += s_warningMessages[code] + "\n";
            msg += PrintEnd(line, column, file);

            ProgramErrors.DisplayError(msg);
            Environment.Exit(1);
        }
        public static void InvalidOperand(Token token, string file, string line)
        {
            string code = "00000";
            string msg = "";
            msg += PrintStd(code);
            msg += s_warningMessages[code] + "\n";
            msg += "| \t";

            string[] s_line = line.Split(' ');
            for (int i = 0; i < s_line.Length; i++)
            {
                if (token.ident.ToUpper() == s_line[i].ToUpper())
                {
                    msg +=  s_line[i] + " ";
                }
                else
                {
                    msg += s_line[i] + " ";
                }
            }
            int index = line.ToUpper().IndexOf(token.ident.ToUpper());
            
            msg += "\n";
            msg += "|\t";
            msg += " ".PadRight(Math.Clamp(index, 0, int.MaxValue), ' ') + "".PadLeft(token.ident.Length, '~');
            msg += PrintEnd(token.line, token.column, token.m_file);

            ProgramErrors.DisplayError(msg);
            Environment.Exit(1);
        }
        public static void InvalidInstruction(Token token, string file, string line)
        {
            string code = "00005";
            string msg = "";
            msg += PrintStd(code);
            msg += s_warningMessages[code] + "\n";
            msg += "| \t";

            string[] s_line = line.Split(' ');
            for (int i = 0; i < s_line.Length; i++)
            {
                if (token.ident.ToUpper() == s_line[i].ToUpper())
                {
                    msg +=  s_line[i] + " ";
                }
                else
                {
                    msg += s_line[i] + " ";
                }
            }
            int index = line.ToUpper().IndexOf(token.ident.ToUpper());
            
            msg += "\n";
            msg += "|\t";
            msg += " ".PadRight(Math.Clamp(index, 0, int.MaxValue), ' ') + "".PadLeft(token.ident.Length, '~');
            msg += PrintEnd(token.line, token.column, token.m_file);

            ProgramErrors.DisplayError(msg);
            Environment.Exit(1);
        }
        public static void ExpectedChar(char expectedC, string file, long line, long column)
        {
            string msg = "";
            string code = "00001";
            
            msg += PrintStd(code);
            msg += s_warningMessages[code];
            msg += PrintEnd(line, column, file);

            ProgramErrors.DisplayError(msg, expectedC);
            Environment.Exit(1);
        }

        static string PrintStd(string code)
        {
            string msg = "";
            msg += $"|";
            msg += $" Assembler Error  ";
            msg += $"ACL-E{code}:\t\t";
            return msg;
        }
        static string PrintEnd(long line, long column, string file)
        {
            string str = "";
            str += $"| \n";
            str += $"| {file}:{line}:{column}\n";
            return str;
        }
    }
}
