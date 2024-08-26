using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ConsoleGameEngine;

namespace emulator
{
    public class Screen : ConsoleGame
    {
        public BUS m_bus;
        public Color[] VGAcolors = Palettes.Default;
        public Font m_font;
        string[] m_args;
        public void Run(string[] args, Font font, CPUSettings settings)
        {
            m_args = args;
            m_font = font;

            if (m_args.Length <= 0)
            {
                Console.WriteLine("ERORR: missing file");
                Exit();
                Environment.Exit(1);
            }

            string BinaryFile = m_args[0];

            if (!File.Exists(BinaryFile))
            {
                Console.WriteLine("ERORR: missing file");
                Exit();
                Environment.Exit(1);
            }

            string[] Contents = File.ReadAllText(BinaryFile).Replace("\r\n", "\n").Split('\n');

            m_bus = new BUS(Contents);



            Construct(320, 240, 2,2, FramerateMode.MaxFps, VGAcolors);
            TargetFramerate = 60;

        }
        public Screen() 
        {
            
        }

        public override void Start()
        {
        }

        public override void Render()
        {
            base.Render();

            MoveVRamToBuffer();

            Engine.DisplayBuffer();
            IsRendering = false;
        }

            byte color = 0;
        public override void Update()
        {
            Console.Title = $"FPS = {GetFramerate()}";

            color++;

            int vram = Mem.VRAM_START - Mem.BANKED_MAM_SIZE;
            for (int i = vram; i < 0xB4FFFF - Mem.BANKED_MAM_SIZE; i += 2)
            {
                m_bus.m_Mem.m_memory[i + 1] = color;
                color++;
            }

            if (Engine.GetKeyDown(ConsoleKey.Escape))
            {
                Close();
            }
        }

        const int VRAMLength = 0x40000;
        byte[] buffer = new byte[VRAMLength];
        void MoveVRamToBuffer()
        {
            int RenderBufferAddress = Mem.VRAM_START - Mem.BANKED_MAM_SIZE;
            int width = 0;
            int height = 0;
            Array.Copy(m_bus.m_Mem.m_memory, RenderBufferAddress, buffer, 0, VRAMLength);
            for (int address = 0; address < RenderBufferAddress; address++)
            {
                byte flags = (byte)(buffer[address] & 0xFF);
                bool Alpah = (byte)(flags & 0x20) == 0x20;

                if ((flags & 0x0F) == 0x00)
                {
                    address++;
                    byte colordata = buffer[address];
                    Engine.SetPixel(new Point(width, height), colordata, Alpah);
                }
                else if ((flags & 0x08) == 0x08)
                {
                    if (height != 0)
                    {
                        height -= 16;
                    }

                    address++;
                    byte colordata = buffer[address];
                    address++;
                    byte data = buffer[address];
                    address++;
                    byte attributes = buffer[address];

                    PrintChar(new Point(width, height), colordata, m_font.m_Characters[data]);
                    width += 8;
                    height += 16;
                }

                width++;
                if (width >= Engine.WindowSize.X)
                {
                    width = 0;
                    height++;
                }
                if (height >= Engine.WindowSize.Y)
                {
                    break;
                }
            }
        }

        void PrintChar(Point startingPoint, int colorIndex, Character character)
        {
            int cursorX = startingPoint.X;
            int cursorY = startingPoint.Y;
            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    Point CursorPos = new Point(cursorX + x, cursorY + y);
                    Point IndexCursor = new Point(x, y);

                    Color c = character.m_Pixels[IndexCursor.Y][IndexCursor.X];
                    byte color = (byte)((byte)((c.R + c.G + c.B) / 3) * colorIndex);

                    Engine.SetPixel(CursorPos, color);
                }
            }
        }
    }
}
