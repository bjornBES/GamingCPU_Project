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
        public VideoPort(ushort portID)
        {
            m_PortID = portID;
            m_InterruptIndex = 0xF;     // IRQ 15
            m_PortIDStart = 0x02;
            m_PortIDEnd = 0x04;

            scrren = new ConsoleGame();
        }

        public void ConnectBus(CPUBus bus)
        {
            m_BUS = bus;
        }

        public ushort m_PortID { get; set; }
        public byte m_InterruptIndex { get; set; }
        public Address m_Address { get; set; }
        public bool m_ReadRam { get; set; }
        public bool m_WriteRam { get; set; }
        public ushort m_Databus { get; set; }
        public ushort m_Outputbus { get; set; }
        public int m_PortIDStart { get; set; }
        public int m_PortIDEnd { get; set; }
        public object m_bus { get; set; }
        public CPUBus m_BUS { get; set; }

        ConsoleGame scrren;
        Color[] colorArray;

        byte CommandRegister;

        bool TryingIRQ = false;

        // commands
        const byte CommandClearScreen = 0x05;


        byte VideoMode;
        byte[] VideoBuffer = new byte[0x004_0000];

        public void Tick()
        {
            VideoBuffer.Initialize();
            VideoBuffer = m_BUS.ReadBytes(0x004_0000, 0, 0x004_0000);

            Thread VideoThead = new Thread(new ThreadStart(Render));
            VideoThead.Start();

            while (VideoThead.ThreadState == ThreadState.Running)
            {

            }

            switch (CommandRegister)
            {
                case CommandClearScreen:
                    Clear();
                    break;
                default:
                    return;
            }
                    CommandRegister = 0;
        }

        void Render()
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


            switch (VideoMode)
            {
                case 0x00:
                    for (cury = 0; cury < 400 / 16; cury++)
                    {
                        for (curx = 0; curx < 360 / 8; curx++)
                        {
                            _char = VideoBuffer[videoIndex];
                            _color = VideoBuffer[videoIndex + 1];

                            if (_char == '\r')
                            {
                                curx = -1;
                                videoIndex += 2;
                                continue;
                            }
                            else if (_char == '\n')
                            {
                                cury = 0;
                                videoIndex += 2;
                                break;
                            }

                            scrren.Engine.WriteText(new Point(curx, cury), ((char)_char).ToString(), _color);

                            videoIndex += 2;
                        }
                    }
                    break;
                default:
                    break;
            }

            NewTime = DateTime.Now;
            time = (NewTime - OldTime).TotalMilliseconds;

            DelayTime = (int)((double)16.67f - time);
            if (DelayTime > 0)
            {
                Thread.Sleep(DelayTime);
            }

            scrren.Engine.DisplayBuffer();
            // done with rendering
            if (TryingIRQ)
            {
                m_BUS.IRQ(this);
            }
            TryingIRQ = true;
        }
        void Clear()
        {
            scrren.Engine.ClearBuffer();
        }

        void SwitchVideoMode(byte mode)
        {
            scrren.Close();
            while (scrren.Running)
            {

            }
            VideoMode = mode;
            switch (VideoMode)
            {
                case 0x00:
                    colorArray = new Color[16];
                    colorArray[0] = new Color(255, 255, 255);
                    scrren.TargetFramerate = 60;
                    scrren.Construct(400/8, 360/16, 8, 16, FramerateMode.MaxFps, colorArray);
                    scrren.Engine.SetBackground(0xF);
                    break;
                case 0x01:
                    colorArray = new Color[16];
                    colorArray[0] = new Color(255, 255, 255);
                    scrren.TargetFramerate = 60;
                    scrren.Construct(400 / 8, 360 / 8, 8, 8, FramerateMode.MaxFps, colorArray);
                    scrren.Engine.SetBackground(0xF);
                    break;
                default:
                    break;
            }
        }

        public void Reset()
        {
            SwitchVideoMode(0);
            CommandRegister = 0;
        }

        public void Write(byte data, ushort Port)
        {
            if (Port == 0x02)
            {
                CommandRegister = data;
            }
        }

        public void Write(ushort data, ushort Port)
        {
            if(data == 0x7F55)
            {
                TryingIRQ = false;
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
