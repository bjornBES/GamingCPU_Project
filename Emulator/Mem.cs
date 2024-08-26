using System;
using System.ComponentModel.Design;
using System.Timers;

namespace emulator
{
    public class Mem
    {
        public const int MAX_MEM = 0xFF_FFFF;
        public const int BANKED_MAM_SIZE = 0x9F_FFFF;
        public const int VRAM_START = 0xB1_0000;
        public byte[] m_memory = new byte[MAX_MEM - BANKED_MAM_SIZE];
        byte[][] m_banked_memory = new byte[0x10][];
        BUS BUS;
        public Mem(string[] program)
        {
            Array.Fill<byte>(m_memory, 0);
            for (int i = 0; i < m_banked_memory.Length; i++)
            {
                m_banked_memory[i] = new byte[BANKED_MAM_SIZE];
                Array.Fill<byte>(m_banked_memory[i], 0);
            }
            for (int i = 0; i < program.Length; i++)
            {
                if (string.IsNullOrEmpty(program[i]))
                    continue;

                string[] line = program[i].Split(':');

                byte value = Convert.ToByte(line[1], 16);
                byte bank = Convert.ToByte(line[0], 16);
                m_banked_memory[0xF][i] = value;
            }
            byte color = 0;

            int vram = VRAM_START - BANKED_MAM_SIZE;
            for (int i = vram; i < 0xB4FFFF - BANKED_MAM_SIZE; i += 2)
            {
                m_memory[i] =       0b0010_0000;
                m_memory[i + 1] =   color;
                color++;
            }

            int offset = 0;
            /*
            m_memory[vram + offset++] = 0b0010_1000;
            m_memory[vram + offset++] = 0x7F;
            m_memory[vram + offset++] = 66;
            m_memory[vram + offset++] = 0;

            m_memory[vram + offset++] = 0b0010_1000;
            m_memory[vram + offset++] = 0x7F;
            m_memory[vram + offset++] = 74;
            m_memory[vram + offset++] = 0;

            m_memory[vram + offset++] = 0b0010_1000;
            m_memory[vram + offset++] = 0x7F;
            m_memory[vram + offset++] = 79;
            m_memory[vram + offset++] = 0;

            m_memory[vram + offset++] = 0b0010_1000;
            m_memory[vram + offset++] = 0x7F;
            m_memory[vram + offset++] = 82;
            m_memory[vram + offset++] = 0;

            m_memory[vram + offset++] = 0b0010_1000;
            m_memory[vram + offset++] = 0x7F;
            m_memory[vram + offset++] = 78;
            m_memory[vram + offset++] = 0;

            m_memory[vram + offset++] = 0b0010_1000;
            m_memory[vram + offset++] = 0x7F;
            m_memory[vram + offset++] = 66;
            m_memory[vram + offset++] = 0;
             */
        }
        public void WriteMemory(int address, byte data, byte bank)
        {
            if (address >= 0x0 && address <= BANKED_MAM_SIZE)
            {
                m_banked_memory[bank][address] = data;
            }
            else if (address > BANKED_MAM_SIZE)
            {
                m_memory[address - BANKED_MAM_SIZE] = data;
            }
        }
        public byte ReadMemory(int address, byte bank)
        {
            if (address >= 0x0 && address <= BANKED_MAM_SIZE)
            {
                return m_banked_memory[bank][address];
            }
            else if (address > BANKED_MAM_SIZE)
            {
                return m_memory[address - BANKED_MAM_SIZE];
            }
            Console.Write("Something is wrong");
            throw new Exception("Something is wrong i can feel it");
        }
        public void ConnectBus(BUS bus)
        {
            BUS = bus;
        }
    }
}
