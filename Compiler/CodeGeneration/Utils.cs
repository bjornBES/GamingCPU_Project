public class Utils
    {
        public static string ToHex(int number, int padding)
        {
            return Convert.ToString(number, 16).PadLeft(padding, '0');
        }
    }
