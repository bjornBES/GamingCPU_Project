using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace emulator
{
    public class BUS
    {
        public CPU m_CPU;
        public Mem m_Mem;
        public BUS(string[] program)
        {
            m_CPU = new CPU();
            m_CPU.ConnectBus(this);
            m_Mem = new Mem(program);
            m_Mem.ConnectBus(this);
            m_CPU.CPU_RESET();
        }
        public void Interrupt()
        {

        }
        public void InterruptScrrenDone()
        {

        }
        public void Write(int address, byte data, byte bank = 0)
        {
            if (bank == 1 && address >= 0x010000 && address <= 0x0100FF)
            {
                // ports
            }
            else
            {
                // memory
                m_Mem.WriteMemory(address, data, bank);
            }
        }
        public void Write(int address, byte[] data, byte bank = 0)
        {
            if (bank == 1 && address >= 0x010000 && address <= 0x0100FF)
            {
                // ports
            }
            else
            {
                // memory
                for (int i = 0; i < data.Length; i++)
                {
                    m_Mem.WriteMemory(address + i, data[i], bank);
                }
            }
        }
        public void WriteWord(int address, ushort data, byte bank = 0)
        {
            if (bank == 1 && address >= 0x010000 && address <= 0x0100FF)
            {
                // ports
            }
            else
            {
                // memory
                byte[] bytes = BitConverter.GetBytes(data);
                m_Mem.WriteMemory(address, bytes[1], bank);
                m_Mem.WriteMemory(address + 1, bytes[0], bank);
            }
        }
        public void WriteDWord(int address, uint data, byte bank = 0)
        {
            if (bank == 1 && address >= 0x010000 && address <= 0x0100FF)
            {
                // ports
            }
            else
            {
                // memory
                byte[] bytes = BitConverter.GetBytes(data);
                m_Mem.WriteMemory(address, bytes[3], bank);
                m_Mem.WriteMemory(address + 1, bytes[2], bank);
                m_Mem.WriteMemory(address + 2, bytes[1], bank);
                m_Mem.WriteMemory(address + 3, bytes[0], bank);
            }
        }
        public byte Read(int address, byte bank)
        {
            if (bank == 1 && address >= 0x010000 && address <= 0x0100FF)
            {
                // ports
                return 0xFF;
            }
            else
            {
                // memory
                return m_Mem.ReadMemory(address, bank);
            }
        }
        public ushort ReadWord(int address, byte bank)
        {
            if (bank == 1 && address >= 0x010000 && address <= 0x0100FF)
            {
                // ports
                return 0xFF;
            }
            else
            {
                // memory
        
                byte Low = m_Mem.ReadMemory(address + 1,bank);
                byte High = m_Mem.ReadMemory(address, bank);
                return (byte)((High << 8) | Low);
            }
        }

        public uint ReadDWord(int address, byte bank)
        {
            if (bank == 1 && address >= 0x010000 && address <= 0x0100FF)
            {
                // ports
                return 0xFF;
            }
            else
            {
                // memory

                byte LowLow   = m_Mem.ReadMemory(address + 3, bank);
                byte LowHigh  = m_Mem.ReadMemory(address + 2, bank);
                byte HighLow  = m_Mem.ReadMemory(address + 1, bank);
                byte HighHigh = m_Mem.ReadMemory(address, bank);
                return (uint)((HighHigh << 24) | (HighLow << 16) | (LowHigh << 8) | (LowLow));
            }
        }
    }
}
