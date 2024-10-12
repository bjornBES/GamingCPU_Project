using System;
using System.Collections.Generic;
using System.Linq;

namespace HexLibrary
{
    public static class HexConverter
    {
        public static string ToHexString(string value, int _base)
        {
            return Convert.ToString(Convert.ToInt32(value, _base), 16);
        }
        public static string ToHexString(short value)
        {
            return Convert.ToString(value, 16);
        }
        public static string ToHexString(int value)
        {
            return Convert.ToString(value, 16);
        }
        public static string ToHexString(long value)
        {
            return Convert.ToString(value, 16);
        }
        public static int FromHexString(string hexString) => Convert.ToInt32(hexString, 16);

        public static string[] SplitHexString(string hexString)
        {
            List<string> result = new List<string>();

            if (hexString.Length == 1)
            {
                hexString = hexString.PadLeft(2, '0');
            }
            else if (hexString.Length % 2 != 0)
            {
                hexString = hexString.PadLeft(hexString.Length + (hexString.Length % 2), '0');
            }

            for (int i = 0; i < hexString.Length; i += 2)
            {
                string _byte = hexString.Substring(i, 2);
                result.Add(_byte);
            }

            return result.ToArray();
        }
        public static string[] SplitHexString(string hexString, int size = 1)
        {
            // Calculate the number of hex digits needed for the given size
            int totalHexDigits = size * 2;

            // Pad the input hex string with leading zeros if it's not the right length
            hexString = hexString.PadLeft(totalHexDigits, '0');

            // Take only the last 'totalHexDigits' characters
            // hexString = hexString.Substring(hexString.Length - totalHexDigits);

            // Split the hex string into pairs of characters (representing bytes)
            string[] hexArray = Enumerable.Range(0, hexString.Length / 2)
                                           .Select(i => hexString.Substring(i * 2, 2))
                                           .ToArray();

            return hexArray;
        }

        public static string[] SortHexStrings(string[] hexNumbers)
        {
            string[] sortedItems = hexNumbers
                        .Select(item => new
                        {
                            Original = item,
                            HexPart = item.Substring(0, 6),
                            NumericHex = Convert.ToInt32(item.Substring(0, 6), 16)
                        })
                        .OrderBy(x => x.NumericHex)
                        .Select(x => x.Original)
                        .ToArray();
            return sortedItems;
        }
    }

    public static class StringFunctions
    {
        public const string _LETTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_";
        public const string _LETTERSAndNUMBER = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890_";
        public const string _NUMBERSDEC = "123456790_";
        public const string _NUMBERSHEX = "123456790ABCDEFabcdef_";
        public const string _OPERATORSWS = "+ - * /";
        public const string _OPERATORS = "+-*/";

        public const string _HEXNUMBERUPPER = "0123456789ABCDEF_";
        public const string _HEXNUMBERLOWER = "0123456789abcdef_";


        public static bool IsLetter(string expr) => IsPatten(expr, _LETTERS);
        public static bool IsLetterOrDidit(string expr) => IsPatten(expr, _LETTERSAndNUMBER);
        public static bool IsNumber(string expr) => IsPatten(expr, _NUMBERSDEC);
        public static bool IsHexUpper(string expr) => IsPatten(expr, _HEXNUMBERUPPER);
        public static bool IsHexLower(string expr) => IsPatten(expr, _HEXNUMBERLOWER);
        public static bool IsHex(string expr) => IsPatten(expr, _NUMBERSHEX);

        static bool IsPatten(string expr, string patten)
        {
            for (int i = 0; i < expr.Length; i++)
            {
                if (!patten.Contains(expr[i]))
                    return false;
            }
            return true;
        }

        public static bool ContainsOperators(string expr)
        {
            for (int i = 0; i < expr.Length; i++)
            {
                if (_OPERATORS.Contains(expr[i]))
                    return true;
            }
            return false;
        }
    }

    public static class SplitFunctions
    {
        public static byte[] SplitWord(ushort word)
        {
            return BitConverter.GetBytes(word);
        }

        public static byte[] SplitTByte(uint tbyte)
        {
            return BitConverter.GetBytes(tbyte);
        }

        public static byte[] SplitDWord(uint dword)
        {
            return BitConverter.GetBytes(dword);
        }
    }
}
