using CommonBCGCPU.Types;
using CommonBCGCPU;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using ConsoleGameEngine;
using System.Diagnostics.Tracing;
using System.Linq;

namespace BC16CPUEmulator
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

        public void INTA()
        {

        }

        int _outputIndex = 0;
        List<ushort> arguments = new List<ushort>();
        ushort[] _outputBuffer = new ushort[10];
        public byte InterruptIndex { get; set; }
        public int PortIDStart { get; set; }
        public int PortIDEnd { get; set; }
        public CPUBus BUS { get; set; }

        public ConsoleGame m_Scrren;
        Color[] m_colorArray;

        ushort m_commandRegister;

        const int TargetFramerate = 60;

        // commands
        const byte CommandClearScreen = 0x05;
        const byte GetRes = 0x10;
        const byte SetMode = 0x11;

        byte m_videoMode;
        Thread VideoThead;

        public void Tick()
        {
            if (VideoThead == null || VideoThead.ThreadState != ThreadState.Running)
            {
                VideoThead = new Thread(new ThreadStart(render));
                VideoThead.Start();
            }
            switch (m_commandRegister)
            {
                case CommandClearScreen:
                    clear();
                    break;
                case GetRes:
                    getResolution(m_videoMode, out Vector screenSize, out Vector fontSize);
                    _outputBuffer[0x0] = (ushort)screenSize.X;
                    _outputBuffer[0x1] = (ushort)screenSize.Y;
                    _outputBuffer[0x2] = (ushort)fontSize.X;
                    _outputBuffer[0x3] = (ushort)fontSize.Y;
                    _outputBuffer[0x4] = m_videoMode;
                    break;
                case SetMode:
                    if(arguments.Count == 1)
                    {
                        switchVideoMode((byte)arguments[0]);
                        getResolution(m_videoMode, out screenSize, out fontSize);
                        _outputBuffer[0x0] = (ushort)screenSize.X;
                        _outputBuffer[0x1] = (ushort)screenSize.Y;
                        _outputBuffer[0x2] = (ushort)fontSize.X;
                        _outputBuffer[0x3] = (ushort)fontSize.Y;
                        _outputBuffer[0x4] = m_videoMode;
                        break;
                    }
                    return;
                default:
                    return;
            }
                    m_commandRegister = 0;
        }
        int framerateSamplesIndex = 0;
        double[] framerateSamples = new double[60];
        float DeltaTime;
        void render()
        {
            byte[] buffer = BUS.ReadVRAM(0x002_0000);

            DateTime OldTime = DateTime.UtcNow;

            byte _color;
            byte _char;

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
                                _char = buffer[videoIndex];
                                _color = buffer[videoIndex + 1];

                                m_Scrren.Engine.WriteText(cursor, ((char)_char).ToString(), _color);

                                videoIndex += 2;
                                break;
                            case 0x01:
                                _color = buffer[videoIndex];
                                videoIndex++;
                                m_Scrren.Engine.SetPixel(cursor, _color);
                                break;
                            default:
                                break;
                        }
                    }
                }

            framerateSamplesIndex++;
            float uncorrectedSleepDuration = 1000 / TargetFramerate;

            double computingDuration = (DateTime.UtcNow - OldTime).TotalMilliseconds;
            int sleepDuration = (int)(uncorrectedSleepDuration - computingDuration);
            if (sleepDuration > 0)
            {
                Thread.Sleep(sleepDuration);
            }

            TimeSpan diff = DateTime.UtcNow - OldTime;
            DeltaTime = (float)(1 / (TargetFramerate * diff.TotalSeconds));

            framerateSamplesIndex %= TargetFramerate;

            framerateSamples[framerateSamplesIndex] = diff.TotalSeconds;

            m_Scrren.Engine.DisplayBuffer();
            double FPS = 1 / (framerateSamples.Sum() / TargetFramerate);
            m_Scrren.SetTitle($"FPS = {FPS}");

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
            if (m_Scrren.Engine != null)
            {
                m_Scrren.Engine.ClearBuffer();
            }
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
                    m_colorArray[1] = new Color(0, 0, 0);
                    m_colorArray[0xF] = new Color(0, 0, 0);
                    break;
                case 0x01:
                    m_colorArray = new Color[16];
                    m_colorArray[0] = new Color(255, 255, 255);
                    m_colorArray[1] = new Color(0, 0, 0);
                    m_colorArray[0xF] = new Color(0, 0, 0);
                    break;
                default:
                    break;
            }
            m_colorArray.Initialize();
            m_Scrren.Construct(640, 480, fontSize.X, fontSize.Y, FramerateMode.MaxFps, m_colorArray);
            m_Scrren.Engine.SetBackground(0xF);
        }

        public void Reset()
        {
            if (m_Scrren.Engine != null)
            {
                clear();
            }
            switchVideoMode(0);
            _outputIndex = 0;
            m_commandRegister = 0;
        }
        public void Write(byte data, ushort Port)
        {
            Write((ushort)data, Port);
        }
        public void Write(ushort data, ushort Port)
        {
            switch (data)
            {
                case 0x8001:
                    _outputIndex = 0;
                    return;
                default:
                    break;
            }

            if (m_commandRegister != 0)
            {
                arguments.Add(data);
                return;
            }

            m_commandRegister = data;
        }
        public byte Read(out byte data, ushort Port)
        {
            ushort _data = Read(out ushort _, Port);
            data = (byte) _data;
            return data;
        }
        public ushort Read(out ushort data, ushort Port)
        {
            data = _outputBuffer[_outputIndex];
            _outputIndex++;
            return data;
        }
    }
}
