using System;
using System.Collections.Generic;
using System.Text;
using CommonBCGCPU.Types;
using HexLibrary;
namespace BCG16CPUEmulator
{
    public class Memory
    {
        public const uint MemBankSize = 0x008_0000;
        public const uint MemBankEndAddress = 0x008_0000;

        public const uint TotalMemorySize = 16 * 1024 * 1024;               // 16 MB
        public const uint InterruptRoutineCount = 0xFF;
        public const uint AllInterruptRoutineSize = 1024;

        public const uint MemorySize = TotalMemorySize - MemBankSize;
        public const uint MaxBanks = 0xFF;

        public byte[][] m_BankedMemory = new byte[MaxBanks][];
        public byte[] m_Mem = new byte[MemorySize];
        BUS m_bUS;

        public void Reset()
        {
            Array.Clear(m_Mem, 0, m_Mem.Length);
            for (int b = 0; b < m_BankedMemory.Length; b++)
            {
                Array.Clear(m_BankedMemory[b], 0, m_BankedMemory[b].Length);
            }

        }

        public Memory()
        {
            m_Mem.Initialize();

            for (int b = 0; b < m_BankedMemory.Length; b++)
            {
                m_BankedMemory[b] = new byte[MemBankSize + 1];
                m_BankedMemory[b].Initialize();
            }
        }

        public byte ReadByte(Address pAddress)
        {
            return ReadByte(pAddress, 1);
        }
        public ushort ReadWord(Address pAddress)
        {
            return ReadWord(pAddress, 1);
        }
        public uint ReadTByte(Address pAddress)
        {
            return ReadTByte(pAddress, 1);
        }
        public int ReadDWord(Address pAddress)
        {
            return ReadDWord(pAddress, 1);
        }
        public byte ReadByte(Address pAddress, byte bank)
        {
            byte RealBank = (byte)(bank - 1);

            Address MemAddr = pAddress - MemBankSize;

            if (pAddress >= 0 && pAddress <= 0x200 - 1)
            {
                return 0;
            }
            else if ((m_bUS.m_CPU.m_CR0 & BC16CPU_Registers.CR0_EnableExtendedMode) == 0x00)
            {
                MemAddr = MemAddr & 0x00FFFF;
                pAddress = pAddress & 0x00FFFF;
            }
            else if ((m_bUS.m_CPU.m_CR0 & BC16CPU_Registers.CR0_EnableExtendedMode) == BC16CPU_Registers.CR0_EnableExtendedMode)
            {
                MemAddr = MemAddr & 0xFFFFFF;
                pAddress = pAddress & 0xFFFFFF;
            }
            
            if (pAddress >= 0 && pAddress <= 0x200-1)
            {
            }
            else if (pAddress >= 0 && pAddress <= MemBankSize)
            {
                return readByteMemory(pAddress, 0);
            }
            else if (MemAddr >= 0 && MemAddr < MemBankEndAddress)
            {
                return readByteBankedMemory(MemAddr, 0, RealBank);
            }
            else if (pAddress >= 0 && pAddress < MemorySize)
            {
                return readByteMemory(pAddress, 0);
            }
            else
            {

            }

            return 0;
        }
        public ushort ReadWord(Address pAddress, byte bank)
        {
            byte high = ReadByte(pAddress, bank);
            byte low = ReadByte(pAddress + 1, bank);

            return (ushort)((high << 8) | low);
        }
        public uint ReadTByte(Address pAddress, byte bank)
        {
            byte high = ReadByte(pAddress, bank);
            byte middle = ReadByte(pAddress + 1, bank);
            byte low = ReadByte(pAddress + 2, bank);

            return (uint)((high << 16) | (middle << 8) | low);

        }
        public int ReadDWord(Address pAddress, byte bank)
        {
            ushort high = ReadWord(pAddress, bank);
            ushort low = ReadWord(pAddress + 2, bank);

            return (int)((high << 16) | low);
        }
        public byte[] ReadBytes(Address pAddress, byte bank, int count)
        {
            byte RealBank = (byte)(bank - 1);

            Address MemAddr = pAddress - MemBankSize;

            if (pAddress >= 0 && pAddress <= 0x200 - 1)
            {
                return null;
            }
            else if ((m_bUS.m_CPU.m_CR0 & BC16CPU_Registers.CR0_EnableExtendedMode) == 0x00)
            {
                MemAddr = MemAddr & 0x00FFFF;
                pAddress = pAddress & 0x00FFFF;
            }
            else if ((m_bUS.m_CPU.m_CR0 & BC16CPU_Registers.CR0_EnableExtendedMode) == BC16CPU_Registers.CR0_EnableExtendedMode)
            {
                MemAddr = MemAddr & 0xFFFFFF;
                pAddress = pAddress & 0xFFFFFF;
            }

            if (pAddress >= 0 && pAddress <= 0x200 - 1)
            {
            }
            else if (pAddress >= 0 && pAddress <= MemBankSize)
            {
                return m_Mem[(int)pAddress..(pAddress + count)];
            }
            else if (MemAddr >= 0 && MemAddr < MemBankEndAddress)
            {
                return m_BankedMemory[RealBank][(int)pAddress..(pAddress + count)];
            }
            else if (pAddress >= 0 && pAddress < MemorySize)
            {
                return m_Mem[(int)pAddress..(pAddress + count)];
            }
            else
            {
            }
                return null;
        }

        public void WriteByte(Address pAddress, byte data)
        {
            WriteByte(pAddress, 1, data);
        }
        public void WriteWord(Address pAddress, ushort data)
        {
            WriteWord(pAddress, 1, data);
        }
        public void WriteTByte(Address pAddress, uint data)
        {
            WriteTByte(pAddress, 1, data);
        }
        public void WriteDWord(Address pAddress, uint data)
        {
            WriteDWord(pAddress, 1, data);
        }
        public void WriteByte(Address pAddress, byte bank, byte data)
        {
            byte[] byteData = new byte[] { data };
            byte RealBank = (byte)(bank - 1);
                Address MemAddr = pAddress - MemBankSize;

            if (pAddress >= 0 && pAddress <= 0x200 - 1)
            {
                return;
            }
            else if ((m_bUS.m_CPU.m_CR0 & BC16CPU_Registers.CR0_EnableExtendedMode) == 0x00)
            {
                MemAddr = MemAddr & 0x00FFFF;
                pAddress = pAddress & 0x00FFFF;
            }
            else if ((m_bUS.m_CPU.m_CR0 & BC16CPU_Registers.CR0_EnableExtendedMode) == BC16CPU_Registers.CR0_EnableExtendedMode)
            {
                MemAddr = MemAddr & 0xFFFFFF;
                pAddress = pAddress & 0xFFFFFF;
            }

            if (pAddress >= 0 && pAddress <= 0x200 - 1)
            {
                return;
            }
            else if (pAddress >= 0 && pAddress <= MemBankSize)
            {
                writeByteMemory(pAddress, 0, byteData);
            }
            else if (MemAddr >= 0 && MemAddr < MemBankEndAddress)
            {
                writeByteBankedMemory(MemAddr, 0, RealBank, byteData);
            }
            else if (pAddress >= 0 && pAddress < MemorySize)
            {
                writeByteMemory(pAddress, 0, byteData);
            }

        }
        public void WriteWord(Address pAddress, byte bank, ushort data)
        {
            byte RealBank = (byte)(bank - 1);
            byte[] byteData = SplitFunctions.SplitWord(data);

            WriteByte(pAddress, RealBank, byteData[0]);
            WriteByte(pAddress + 1, RealBank, byteData[1]);
        }
        public void WriteTByte(Address pAddress, byte bank, uint data)
        {
            byte RealBank = (byte)(bank - 1);
            byte[] byteData = SplitFunctions.SplitTByte(data);

            WriteByte(pAddress, RealBank, byteData[0]);
            WriteByte(pAddress + 1, RealBank, byteData[1]);
            WriteByte(pAddress + 2, RealBank, byteData[2]);
        }
        public void WriteDWord(Address pAddress, byte bank, uint data)
        {
            byte RealBank = (byte)(bank - 1);
            byte[] byteData = SplitFunctions.SplitDWord(data);

            WriteByte(pAddress, RealBank, byteData[0]);
            WriteByte(pAddress + 1, RealBank, byteData[1]);
            WriteByte(pAddress + 2, RealBank, byteData[2]);
            WriteByte(pAddress + 3, RealBank, byteData[3]);
        }

        void writeByteBankedMemory(Address pAddress, int offset, byte bank, byte[] data)
        {
            Address RealAddress = pAddress + offset;
            m_BankedMemory[bank][RealAddress] = data[offset];
        }
        void writeByteMemory(Address pAddress, int offset, byte[] data)
        {
            Address RealAddress = pAddress + offset;
            m_Mem[RealAddress] = data[offset];
        }
        byte readByteBankedMemory(Address pAddress, int offset, byte bank)
        {
            Address RealAddress = pAddress + offset;
            return m_BankedMemory[bank][RealAddress];
        }
        byte readByteMemory(Address pAddress, int offset)
        {
            Address RealAddress = pAddress + offset;
            return m_Mem[RealAddress];
        }

        public void ConnectBus(BUS bus)
        {
            m_bUS = bus;
        }
    }
}
