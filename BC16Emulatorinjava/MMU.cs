using CommonBCGCPU.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace BC16CPUEmulator
{
    public class MMU
    {
        private const int blockSize = 4 * 1024; // 4 KB per block
        private const int pageTableSize = 64 * 1024; // 64 KB per block

        public BUS BUS;

        public MMU()
        {
        }

        // Allocate memory for a program (initial 4 KB)
        public bool Allocate(byte programId)
        {
            if ((BUS.m_CPU.m_CR0 & BC16CPU_Registers.CR0_EnableProtectedMode) == BC16CPU_Registers.CR0_EnableProtectedMode)
            {
                for (int i = 0; i < pageTableSize; i++)
                {
                    Address address = (BUS.m_CPU.m_PTA * 16) + i;
                    if (BUS.m_Memory.ReadByte(address) == 0x0)
                    {
                        BUS.m_Memory.WriteByte(address, programId);
                        return true;
                    }
                }
                Console.WriteLine($"Failed to allocate memory for program {programId}: Not enough memory.");
                return false;
            }
            return false;
        }

        // Request additional 4 KB memory for a program
        public bool RequestMoreMemory(byte programId)
        {
            if ((BUS.m_CPU.m_CR0 & BC16CPU_Registers.CR0_EnableProtectedMode) == BC16CPU_Registers.CR0_EnableProtectedMode)
            {
                for (int i = 0; i < pageTableSize; i++)
                {
                    Address address = (BUS.m_CPU.m_PTA * 16) + i;
                    if (BUS.m_Memory.ReadByte(address) == 0x0)
                    {
                        BUS.m_Memory.WriteByte(address, programId);
                        return true;
                    }
                }
                Console.WriteLine($"Failed to allocate additional memory for program {programId}: Not enough memory.");
                return false;
            }
            return false;
        }

        public int AllocatedBlocks(byte programId)
        {
            int number = 0;
            if ((BUS.m_CPU.m_CR0 & BC16CPU_Registers.CR0_EnableProtectedMode) == BC16CPU_Registers.CR0_EnableProtectedMode)
            {
                for (int i = 0; i < pageTableSize; i++)
                {
                    Address address = (BUS.m_CPU.m_PTA * 16) + i;
                    if (BUS.m_Memory.ReadByte(address) == programId)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        // Deallocate memory for a program
        public void Deallocate(byte programId)
        {
            if ((BUS.m_CPU.m_CR0 & BC16CPU_Registers.CR0_EnableProtectedMode) == BC16CPU_Registers.CR0_EnableProtectedMode)
            {
                for (int i = 0; i < pageTableSize; i++)
                {
                    Address address = (BUS.m_CPU.m_PTA * 16) + i;
                    if (BUS.m_Memory.ReadByte(address) == programId)
                    {
                        BUS.m_Memory.WriteByte(address, 0);
                    }
                }
            }
        }

        // Get the physical address from program ID and virtual address
        public int GetPhysicalAddress(int programId, int virtualAddress)
        {
            if ((BUS.m_CPU.m_CR0 & BC16CPU_Registers.CR0_EnableProtectedMode) == BC16CPU_Registers.CR0_EnableProtectedMode)
            {
                List<int> blocks = new List<int>();
                for (int i = 0; i < pageTableSize; i++)
                {
                    Address address = (BUS.m_CPU.m_PTA * 16) + i;
                    if (BUS.m_Memory.ReadByte(address) == programId)
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
            return virtualAddress;
        }

        public void ConnectBus(BUS bUS)
        {
            BUS = bUS;
        }
    }
}
