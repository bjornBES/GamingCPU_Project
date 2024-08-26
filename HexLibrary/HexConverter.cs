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
        public const string _NUMBERSDEC = "123456790_";
        public const string _NUMBERSHEX = "123456790ABCDEFabcdef_";
        public const string _OPERATORSWS = "+ - * /";
        public const string _OPERATORS = "+-*/";


        public static bool IsLetter(string expr) => IsPatten(expr, _LETTERS);
        public static bool IsNumber(string expr) => IsPatten(expr, _NUMBERSDEC);

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
            return expr.Split(' ').ToList().Find(str =>
            {
                bool results = false;
                _OPERATORSWS.Split(' ').ToList().ForEach(op =>
                {
                    if (op == str)
                    {
                        results = true;
                    }
                });
                return results;
            }) != null;
        }
    }

    public static class SplitFunctions
    {
        public static byte[] SplitWord(ushort word)
        {
            return new byte[]
            {
            (byte)(word >> 8),         // Upper byte
            (byte)(word & 0xFF),       // Lower byte
            };
        }

        public static byte[] SplitTByte(uint tbyte)
        {
            return new byte[]
            {
            (byte)((tbyte >> 16) & 0xFF),// Highest byte
            (byte)((tbyte >> 8) & 0xFF), // Middle byte
            (byte)(tbyte & 0xFF),       // Lowest byte
            };
        }

        public static byte[] SplitDWord(uint dword)
        {
            return new byte[]
            {
            (byte)((dword >> 24) & 0xFF), // Highest byte
            (byte)((dword >> 16) & 0xFF), // 3rd byte
            (byte)((dword >> 8) & 0xFF), // 2nd byte
            (byte)(dword & 0xFF),       // Lowest byte
            };
        }
    }
}
