using assembler.global;
using emulator;
using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using StbImageSharp;
using System.Drawing.Imaging;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;
using System.Collections.Generic;
using System.IO;
using ConsoleGameEngine;

class Program
{
    static bool TestDisplay = true;
    static Dictionary<byte, Character> characters = new Dictionary<byte, Character>();
    public static int Main(string[] args)
    {

        IPS2_Device keyborad = new PS2_Keyborad();

        //keyborad.Tick();
        

        if (!TestDisplay)
        {
            if(args.Length <= 0)
            {
                Console.WriteLine("ERORR: missing file");
                return 1;
            }

            string BinaryFile = args[0];
            string[] Contents = File.ReadAllText(BinaryFile).Replace("\r\n", "\n").Split('\n');

            bool running = true;
            Console.CursorVisible = false;
            //00000000011111111112222222222
            //12345678901234567890123456789
            //A = {00 00}    |
            //HL= {0000 0000}|
            while (running)
            {

                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo KeyInfo = Console.ReadKey();
                    switch (KeyInfo.Key)
                    {
                        case ConsoleKey.Spacebar:
                        case ConsoleKey.Enter:
                            break;
                        case ConsoleKey.Escape:
                            running = false;
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        else
        {
            CPUSettings settings = new CPUSettings()
            {
                m_StepTicks = true,
            };

            Screen screen = new Screen();
            screen.Run(args, GetFont("./Emulator/VGA-8x16.png"), settings);
        }
        return 0;
    }

    static Font GetFont(string filePath)
    {
        using (var stream = File.OpenRead(filePath))
        {
            List<byte[]> data = new List<byte[]>();
            string contents = "";
            try
            {
                ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
                if (image.SourceComp == ColorComponents.Grey || image.SourceComp == ColorComponents.RedGreenBlueAlpha)
                {
                    int width = image.Width;
                    int height = image.Height;
                    int charsPerRow = width / 8;
                    int totalChars = (width / 8) * (height / 16);

                    for (int charIndex = 0; charIndex < totalChars; charIndex++)
                    {
                        byte[] charData = new byte[8 * 16];
                        int charRow = charIndex / charsPerRow;
                        int charCol = charIndex % charsPerRow;

                        for (int row = 0; row < 16; row++)
                        {
                            for (int col = 0; col < 8; col++)
                            {
                                int pixelX = charCol * 8 + col;
                                int pixelY = charRow * 16 + row;
                                int pixelIndex = (pixelY * width + pixelX) * 4;

                                byte grayValue = image.Data[pixelIndex]; // Assuming grayscale is in R channel
                                charData[row * 8 + col] = grayValue > 0 ? (byte)1 : (byte)0;
                            }
                        }
                        data.Add(charData);
                    }

                    // Print the byte arrays in the specified format
                    for (int i = 0; i < data.Count; i++)
                    {
                        contents += $"{i}:\t";
                        for (int a = 0; a < data[i].Length; a++)
                        {
                            contents += $"{data[i][a]}";
                            if (a != 0 && (a + 1) % 8 == 0)
                            {
                                contents += "\n\t";
                            }
                        }
                        foreach (var b in data[i])
                        {
                        }
                        contents += $"\n\n";
                    }
                    File.WriteAllText("./Emulator/CHARBYTES.txt", contents);
                }
            }
            finally
            {
                stream.Dispose();
            }

            contents = "";

            for (int index = 0; index < data.Count; index++)
            {
                contents += $"{index}:\n";
                List<Color[]> colors = new List<Color[]>();
                byte[] bytedata = data[index];
                for (int d = 0; d < bytedata.Length; d += 8)
                {
                    List<Color> colorBuffer = new List<Color>();
                    for (int i = d; i < d + 8; i++)
                    {
                        Color color = new Color(bytedata[i], bytedata[i], bytedata[i]);
                        colorBuffer.Add(color);
                        contents += $"{bytedata[i]}";
                    }
                    colors.Add(colorBuffer.ToArray());
                    contents += "\n";
                }
                contents += "\n\n";
                characters.Add((byte)index, new Character() { m_Pixels = colors });
            }

            File.WriteAllText("./Emulator/CHAR.txt", contents);
        }

        return new Font()
        {
            m_Characters = characters
        };
    }
}