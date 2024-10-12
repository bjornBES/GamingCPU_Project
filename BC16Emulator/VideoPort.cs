using CommonBCGCPU.Types;
using CommonBCGCPU;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using ConsoleGameEngine;
using System.Diagnostics.Tracing;

namespace BCG16CPUEmulator
{
    public class VideoPort : IPort
    {
        public VideoPort()
        {
            PortIDStart = 0x02;
            PortIDEnd = 0x04;

            m_Scrren = new ConsoleGame();
        }

        public void ConnectBus(CPUBus bus)
        {
            BUS = bus;
        }

        public byte InterruptIndex { get; set; }
        public Address Address { get; set; }
        public bool ReadRam { get; set; }
        public bool WriteRam { get; set; }
        public ushort Databus { get; set; }
        public ushort Outputbus { get; set; }
        public int PortIDStart { get; set; }
        public int PortIDEnd { get; set; }
        public object Bus { get; set; }
        public CPUBus BUS { get; set; }

        public ConsoleGame m_Scrren;
        Color[] m_colorArray;

        byte m_commandRegister;

        // commands
        const byte CommandClearScreen = 0x05;


        byte m_videoMode;
        byte[] m_videoBuffer = new byte[0x001_0000];

        public void Tick()
        {
            m_videoBuffer.Initialize();
            m_videoBuffer = BUS.ReadBytes(0x0007_0000, 0, 0x001_0000);

            if (m_videoBuffer != null)
            {
                Thread VideoThead = new Thread(new ThreadStart(render));
                VideoThead.Start();

                while (VideoThead.ThreadState == ThreadState.Running)
                {

                }
            }
            else
            {
                m_videoBuffer = new byte[0x001_0000];
            }

            switch (m_commandRegister)
            {
                case CommandClearScreen:
                    clear();
                    break;
                default:
                    return;
            }
                    m_commandRegister = 0;
        }

        void render()
        {
            DateTime OldTime = DateTime.Now;
            DateTime NewTime;

            byte _color;
            byte _char;

            int DelayTime;
            double time;

            int curx;
            int cury;

            int videoIndex = 0;

            getResolution(m_videoMode, out Vector screenSize, out Vector fontSize);

            for (cury = 0; cury < screenSize.X; cury++)
            {
                for (curx = 0; curx < screenSize.Y; curx++)
                {
                    Point cursor = new Point(curx, cury);
                    switch (m_videoMode)
                    {
                        case 0x00:
                            _char = m_videoBuffer[videoIndex];
                            _color = m_videoBuffer[videoIndex + 1];

                            m_Scrren.Engine.WriteText(cursor, ((char)_char).ToString(), _color);

                            videoIndex += 2;
                            break;
                        case 0x01:
                            _color = m_videoBuffer[videoIndex];
                            videoIndex++;
                            m_Scrren.Engine.SetPixel(cursor, _color);
                            break;
                        default:
                            break;
                    }
                }
            }

            NewTime = DateTime.Now;
            time = (NewTime - OldTime).TotalMilliseconds;

            DelayTime = (int)((double)16.68f - time);
            if (DelayTime > 0)
            {
                Thread.Sleep(DelayTime);
            }

            m_Scrren.Engine.DisplayBuffer();
            // done with rendering
        }
        void getResolution(int mode, out Vector screenSize, out Vector fontSize)
        {
            screenSize = new Vector();
            fontSize = new Vector();
            switch (mode)
            {
                case 0x00:
                    screenSize = new Vector(25, 40);
                    fontSize = new Vector(8, 16);
                    break;
                case 0x01:
                    screenSize = new Vector(400, 360);
                    fontSize = new Vector(2, 2);
                    break;
                case 0x02:
                    screenSize = new Vector(25, 80);
                    fontSize = new Vector(8, 16);
                    break;
                default:
                    break;
            }
        }
        void clear()
        {
            m_Scrren.Engine.ClearBuffer();
        }

        void switchVideoMode(byte mode)
        {
            m_Scrren.Close();
            while (m_Scrren.Running)
            {

            }
            m_videoMode = mode;
            getResolution(mode, out Vector screenSize, out Vector fontSize);
            switch (m_videoMode)
            {
                case 0x00:
                    m_colorArray = new Color[16];
                    m_colorArray[0] = new Color(255, 255, 255);
                    break;
                case 0x01:
                    m_colorArray = new Color[16];
                    m_colorArray[0] = new Color(0, 0, 0);
                    m_colorArray[1] = new Color(255, 255, 255);
                    break;
                default:
                    break;
            }
            m_colorArray.Initialize();
            m_Scrren.Construct(640, 480, fontSize.X, fontSize.Y, FramerateMode.MaxFps, m_colorArray);
            m_Scrren.TargetFramerate = 60;
            m_Scrren.Engine.SetBackground(0xF);
        }

        public void Reset()
        {
            if (m_Scrren.Engine != null)
            {
                clear();
            }
            switchVideoMode(0);
            m_commandRegister = 0;
            m_videoBuffer = new byte[0x001_0000];
        }
        public void Write(byte data, ushort Port)
        {
            if (Port == 0x02)
            {
                m_commandRegister = data;
            }
        }
        public void Write(ushort data, ushort Port)
        {
            if(data == 0x7F55)
            {
                return;
            }
        }
        public byte Read(out byte data, ushort Port)
        {
            data = 0;
            return data;
        }
        public ushort Read(out ushort data, ushort Port)
        {
            data = 0;
            return data;
        }
    }
}
