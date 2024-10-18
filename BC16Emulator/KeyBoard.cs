using CommonBCGCPU;
using CommonBCGCPU.Types;
using ConsoleGameEngine;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace BCG16CPUEmulator
{
    public class KeyBoard : IPort
    {
        public KeyBoard()
        {
            PortIDStart = 0x0;
            PortIDEnd = 0x0;
            InterruptIndex = 0;                    // IRQ0
        }
        public int PortIDStart { get; set; }
        public int PortIDEnd { get; set; }
        public byte InterruptIndex { get; set; }
        public Address Address { get; set; }
        public bool ReadRam { get; set; }
        public bool WriteRam { get; set; }
        public ushort Databus { get; set; }
        public ushort Outputbus { get; set; }
        public CPUBus BUS { get; set; }
        public ConsoleGame m_Scrren;



        short m_keyState;
        short m_previousState;

        ConsoleKey m_key;

        public void ConnectBus(CPUBus bus)
        {
            BUS = bus;
        }

        public void INTA()
        {

        }

        public byte Read(out byte data, ushort Port)
        {
            data = (byte)m_key;
            // if the key is Releasing
            if ((m_keyState & 0x8000) != 0x8000 && (m_previousState & 0x8000) == 0x8000)
            {
                m_previousState = 0;
                data = 0xF0;
            }

            return data;
        }

        public ushort Read(out ushort data, ushort Port)
        {
            Read(out byte d, Port);
            data = d;
            return data;
        }

        public void Reset()
        {
        }

        public void Tick()
        {
            Thread thread = new Thread(new ThreadStart(checkKey));
            thread.Start();
        }

        void checkKey()
        {
            for (int i = 7; i < byte.MaxValue; i++)
            {
                short key = NativeMethods.GetAsyncKeyState(i);

                if (key == 0x0000)
                {
                    continue;
                }

                m_keyState = key;

                // if key is pressed down
                if ((m_keyState & 0x0001) == 0x0001)
                {
                    m_key = (ConsoleKey)i;
                    break;
                }

                // if key is Held
                if ((m_keyState & 0x8000) == 0x8000)
                {
                    m_key = (ConsoleKey)i;
                    break;
                }

                // if the key is Releasing
                if ((m_keyState & 0x8000) != 0x8000 && (m_previousState & 0x8000) == 0x8000)
                {
                    break;
                }
            }
            m_previousState = m_keyState;
        }

        public void Write(byte data, ushort Port)
        {
        }

        public void Write(ushort data, ushort Port)
        {
        }
    }
}
