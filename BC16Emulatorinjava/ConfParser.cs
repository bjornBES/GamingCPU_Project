
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using BC16CPUEmulator;
using filesystem;

public class ConfParser : CPUSettings
{
    string[] src;
    string ConfPath = "";

    public void Build()
    {
        ConfPath = Path.GetFullPath(m_ConfFile);
        string temp = File.ReadAllText(m_ConfFile);
        temp = Regex.Replace(temp, @"\\[\s]*(\n\r|\n)", " ").Trim();
        src = temp.Split(Environment.NewLine);
        Parse();
    }

    void Parse()
    {
        for (int i = 0; i < src.Length; i++)
        {
            if (src[i].StartsWith("input-path = ", StringComparison.OrdinalIgnoreCase))
            {
                string path = src[i].Replace("input-path = ", "", StringComparison.OrdinalIgnoreCase).Trim('"');
                m_InputFile = Path.GetFullPath(path);
            }
            else if (src[i].StartsWith("drive", StringComparison.OrdinalIgnoreCase))
            {
                i++;
                if (!src[i].StartsWith("{", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"systax error in {ConfPath}:{i}");
                    Environment.Exit(1);
                }

                i++;
                src[i] = Regex.Replace(src[i], @"\s+", " ").Trim();
                src[i] = src[i].Replace(", ", ",");
                src[i] = src[i].Replace(": ", ":");
                string[] tokens = src[i].Split(',', ':');

                Disk disk = parseDriveSection(tokens, out int index);

                if (disk.m_Is80Track)
                {
                    disk.TracksPerSurface = 80;
                }
                else
                {
                    disk.TracksPerSurface = 40;
                }

                i++;
                if (!src[i].StartsWith("}", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"systax error in {ConfPath}:{i}");
                    Environment.Exit(1);
                }
                m_DiskPaths.Add(index, disk);
            }
        }
    }

    private Disk parseDriveSection(string[] tokens, out int index)
    {
        index = -1;
        Disk result = new Disk();
        for (int i = 1; i < tokens.Length; i++)
        {
            if (Regex.IsMatch(tokens[i], @"index[\s*]=[\s*]", RegexOptions.IgnoreCase))
            {
                string value = Regex.Replace(tokens[i], @"index[\s*]=[\s*]", "", RegexOptions.IgnoreCase);
                index = parseNumber(value);
            }
            else if (Regex.IsMatch(tokens[i], @"UseTrack80[\s*]=[\s*]", RegexOptions.IgnoreCase))
            {
                string value = Regex.Replace(tokens[i], @"UseTrack80[\s*]=[\s*]", "", RegexOptions.IgnoreCase);
                result.m_Is80Track = bool.Parse(value);
            }
            else if (Regex.IsMatch(tokens[i], @"WriteEnable[\s*]=[\s*]", RegexOptions.IgnoreCase))
            {
                string value = Regex.Replace(tokens[i], @"WriteEnable[\s*]=[\s*]", "", RegexOptions.IgnoreCase);
                result.m_WriteEnable = bool.Parse(value);
            }
            else if (Regex.IsMatch(tokens[i], @"FST[\s*]=[\s*]", RegexOptions.IgnoreCase))
            {
                string value = Regex.Replace(tokens[i], @"FST[\s*]=[\s*]", "", RegexOptions.IgnoreCase);
                result.m_FileSystemFormat = parseEnum<FileSystemType>(value);
            }
            else if (Regex.IsMatch(tokens[i], @"size[\s*]=[\s*]", RegexOptions.IgnoreCase))
            {
                string value = Regex.Replace(tokens[i], @"size[\s*]=[\s*]", "", RegexOptions.IgnoreCase);
                result.m_DiskSize = parseNumber(value) * 1024;
            }
            else if (Regex.IsMatch(tokens[i], @"path[\s*]=[\s*]", RegexOptions.IgnoreCase))
            {
                string path = Regex.Replace(tokens[i], @"path[\s*]=[\s*]", "", RegexOptions.IgnoreCase).Trim('"');
                result.m_DiskPath = Path.GetFullPath(path);
            }
            else if (Regex.IsMatch(tokens[i], @"platters[\s*]=[\s*]", RegexOptions.IgnoreCase))
            {
                string value = Regex.Replace(tokens[i], @"platters[\s*]=[\s*]", "", RegexOptions.IgnoreCase).Trim('"');
                result.NumberOfPlatters = parseNumber(value);
            }
        }
        return result;
    }

    T parseEnum<T>(string value) where T : Enum
    {
        if (Enum.TryParse(typeof(T), value, out object result))
        {
            return (T)result;
        }

        string[] names = Enum.GetNames(typeof(T));
        T[] values = (T[])Enum.GetValues(typeof(T));

        for (int i = 0; i < names.Length; i++)
        {
            if (names[i] == value)
            {
                return values[i];
            }
        }

        return default;
    }
    int parseNumber(string value)
    {
        if (value == null)
        {
            return -1;
        }

        int result = int.Parse(value, System.Globalization.NumberStyles.Any);


        return result;
    }
}