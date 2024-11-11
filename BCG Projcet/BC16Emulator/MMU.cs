using CommonBCGCPU.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace BC16CPUEmulator
{
    public class MMU
    {
        private const int blockSize = 64 * 1024; // 64 KB per block
        private const int pageTableSize = 64 * 1024; // 64 KB per block

        public BUS BUS;

        public MMU()
        {
        }

        // Allocate memory for a program (initial 64 KB)
        public bool Allocate(byte programId)
        {
            for (int i = 0; i < pageTableSize; i++)
            {
                Address address = (BUS.m_CPU.m_PTA * 16) + i;
                if (BUS.m_Memory.m_Mem[address] == 0x0)
                {
                    BUS.m_Memory.m_Mem[address] = programId;
                    return true;
                }
            }
            Console.WriteLine($"Failed to allocate memory for program {programId}: Not enough memory.");
            return false;
        }

        // Request additional 64 KB memory for a program
        public bool RequestMoreMemory(byte programId)
        {
            for (int i = 0; i < pageTableSize; i++)
            {
                Address address = (BUS.m_CPU.m_PTA * 16) + i;
                if (BUS.m_Memory.m_Mem[address] == 0x0)
                {
                    BUS.m_Memory.m_Mem[address] = programId;
                    return true;
                }
            }
            Console.WriteLine($"Failed to allocate additional memory for program {programId}: Not enough memory.");
            return false;
        }

        // Deallocate memory for a program
        public void Deallocate(byte programId)
        {
            for (int i = 0; i < pageTableSize; i++)
            {
                Address address = (BUS.m_CPU.m_PTA * 16) + i;
                if (BUS.m_Memory.m_Mem[address] == programId)
                {
                    BUS.m_Memory.m_Mem[address] = 0;
                }
            }
        }

        // Show the current memory map (allocation status)
        public void ShowMemoryMap()
        {
            Console.WriteLine("Memory Map:");
            for (int i = 0; i < pageTableSize; i += 8)
            {
                Address address = (BUS.m_CPU.m_PTA * 16) + i;
                Console.Write($"{Convert.ToString(i + 00, 16).PadLeft(4, '0')}:{(BUS.m_Memory.m_Mem[address + 00] > 0 ? "T" : "F")}{BUS.m_Memory.m_Mem[address + 00]} ");
                Console.Write($"{Convert.ToString(i + 01, 16).PadLeft(4, '0')}:{(BUS.m_Memory.m_Mem[address + 01] > 0 ? "T" : "F")}{BUS.m_Memory.m_Mem[address + 01]} ");
                Console.Write($"{Convert.ToString(i + 02, 16).PadLeft(4, '0')}:{(BUS.m_Memory.m_Mem[address + 02] > 0 ? "T" : "F")}{BUS.m_Memory.m_Mem[address + 02]} ");
                Console.Write($"{Convert.ToString(i + 03, 16).PadLeft(4, '0')}:{(BUS.m_Memory.m_Mem[address + 03] > 0 ? "T" : "F")}{BUS.m_Memory.m_Mem[address + 03]} ");
                Console.Write($"{Convert.ToString(i + 04, 16).PadLeft(4, '0')}:{(BUS.m_Memory.m_Mem[address + 04] > 0 ? "T" : "F")}{BUS.m_Memory.m_Mem[address + 04]} ");
                Console.Write($"{Convert.ToString(i + 05, 16).PadLeft(4, '0')}:{(BUS.m_Memory.m_Mem[address + 05] > 0 ? "T" : "F")}{BUS.m_Memory.m_Mem[address + 05]} ");
                Console.Write($"{Convert.ToString(i + 06, 16).PadLeft(4, '0')}:{(BUS.m_Memory.m_Mem[address + 06] > 0 ? "T" : "F")}{BUS.m_Memory.m_Mem[address + 06]} ");
                Console.Write($"{Convert.ToString(i + 07, 16).PadLeft(4, '0')}:{(BUS.m_Memory.m_Mem[address + 07] > 0 ? "T" : "F")}{BUS.m_Memory.m_Mem[address + 07]} ");
                Console.WriteLine();
            }
        }

        // Get the physical address from program ID and virtual address
        public int GetPhysicalAddress(int programId, int virtualAddress)
        {
            List<int> blocks = new List<int>();
            for (int i = 0; i < pageTableSize; i++)
            {
                Address address = (BUS.m_CPU.m_PTA * 16) + i;
                if (BUS.m_Memory.m_Mem[address] == programId)
                {
                    blocks.Add(i);
                }
            }

            int blockIndex = virtualAddress / blockSize;  // Which 64KB block the virtual address is referencing
            int offset = virtualAddress % blockSize;      // Offset within that block

            if (blockIndex >= blocks.Count)
            {
                throw new ArgumentException($"Virtual address {virtualAddress} exceeds allocated memory for program {programId}.");
            }

            int physicalBlock = blocks[blockIndex];       // Find the physical block corresponding to this virtual block
            int physicalAddress = physicalBlock * blockSize + offset;

            Console.WriteLine($"Program {programId}, Virtual Address {Convert.ToString(virtualAddress, 16).PadLeft(8, '0')}: Physical Address = {Convert.ToString(physicalAddress, 16).PadLeft(8, '0')}");
            return physicalAddress;
        }

        public void ConnectBus(BUS bUS)
        {
            BUS = bUS;
        }
    }
}
