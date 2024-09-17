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

        public const uint TotalMemorySize = 16 * (1024 * 1024);             // 16 MB
        public const uint InterruptRoutineCount = 0xFF;
        public const uint AllInterruptRoutineSize = 1024;

        public const uint MemorySize = TotalMemorySize - MemBankSize;
        public const uint MaxBanks = 0xFF;

        public byte[][] BankedMemory = new byte[MaxBanks][];
        public byte[] Mem = new byte[MemorySize];
        BUS m_BUS;


        public Memory()
        {
            Mem.Initialize();

            for (int b = 0; b < BankedMemory.Length; b++)
            {
                BankedMemory[b] = new byte[MemBankSize + 1];
                BankedMemory[b].Initialize();
            }
        }

        public byte ReadByte(Address address)
        {
            return ReadByte(address, 1);
        }
        public ushort ReadWord(Address address)
        {
            return ReadWord(address, 1);
        }
        public uint ReadTByte(Address address)
        {
            return ReadTByte(address, 1);
        }
        public int ReadDWord(Address address)
        {
            return ReadDWord(address, 1);
        }
        public byte ReadByte(Address address, byte bank)
        {
            byte RealBank = (byte)(bank - 1);

            Address MemAddr = address - MemBankSize;

            if (address >= 0 && address <= 0x200 - 1)
            {
                return 0;
            }
            else if ((m_BUS.m_CPU.CR0 & 0x11) == 0x00)
            {
                MemAddr = MemAddr & 0x00FFFF;
                address = address & 0x00FFFF;
            }
            else if ((m_BUS.m_CPU.CR0 & 0x01) == 0x01)
            {
                MemAddr = MemAddr & 0x0FFFFF;
                address = address & 0x0FFFFF;
            }
            else if ((m_BUS.m_CPU.CR0 & 0x11) == 0x11)
            {
                MemAddr = MemAddr & 0xFFFFFF;
                address = address & 0xFFFFFF;
            }
            else if ((m_BUS.m_CPU.CR0 & 0x10) == 0x10)
            {
                MemAddr = MemAddr & 0xF0FFFF;
                address = address & 0xF0FFFF;
            }
            
            if (address >= 0 && address <= 0x200-1)
            {
            }
            else if (address >= 0 && address <= MemBankSize)
            {
                return readByteMemory(address, 0);
            }
            else if (MemAddr >= 0 && MemAddr < MemBankEndAddress)
            {
                return readByteBankedMemory(MemAddr, 0, RealBank);
            }
            else if (address >= 0 && address < MemorySize)
            {
                return readByteMemory(address, 0);
            }
            else
            {

            }

            return 0;
        }
        public ushort ReadWord(Address address, byte bank)
        {
            byte high = ReadByte(address, bank);
            byte low = ReadByte(address + 1, bank);

            return (ushort)((high << 8) | low);
        }
        public uint ReadTByte(Address address, byte bank)
        {
            byte high = ReadByte(address, bank);
            byte middle = ReadByte(address + 1, bank);
            byte low = ReadByte(address + 2, bank);

            return (uint)((high << 16) | (middle << 8) | low);

        }
        public int ReadDWord(Address address, byte bank)
        {
            ushort high = ReadWord(address, bank);
            ushort low = ReadWord(address + 2, bank);

            return (int)((high << 16) | low);
        }
        public byte[] ReadBytes(Address address, byte bank, int count)
        {
            byte RealBank = (byte)(bank - 1);

            Address MemAddr = address - MemBankSize;

            if (address >= 0 && address <= 0x200 - 1)
            {
                return null;
            }
            else if ((m_BUS.m_CPU.CR0 & 0x11) == 0x00)
            {
                MemAddr = MemAddr & 0x00FFFF;
                address = address & 0x00FFFF;
            }
            else if ((m_BUS.m_CPU.CR0 & 0x01) == 0x01)
            {
                MemAddr = MemAddr & 0x0FFFFF;
                address = address & 0x0FFFFF;
            }
            else if ((m_BUS.m_CPU.CR0 & 0x11) == 0x11)
            {
                MemAddr = MemAddr & 0xFFFFFF;
                address = address & 0xFFFFFF;
            }
            else if ((m_BUS.m_CPU.CR0 & 0x10) == 0x10)
            {
                MemAddr = MemAddr & 0xF0FFFF;
                address = address & 0xF0FFFF;
            }

            if (address >= 0 && address <= 0x200 - 1)
            {
            }
            else if (address >= 0 && address <= MemBankSize)
            {
                return Mem[(int)address..(address + count)];
            }
            else if (MemAddr >= 0 && MemAddr < MemBankEndAddress)
            {
                return BankedMemory[RealBank][(int)address..(address + count)];
            }
            else if (address >= 0 && address < MemorySize)
            {
                return Mem[(int)address..(address + count)];
            }
            else
            {
            }
                return null;
        }

        public void WriteByte(Address address, byte data)
        {
            WriteByte(address, 1, data);
        }
        public void WriteWord(Address address, ushort data)
        {
            WriteWord(address, 1, data);
        }
        public void WriteTByte(Address address, uint data)
        {
            WriteTByte(address, 1, data);
        }
        public void WriteDWord(Address address, uint data)
        {
            WriteDWord(address, 1, data);
        }
        public void WriteByte(Address address, byte bank, byte data)
        {
            byte[] byteData = new byte[] { data };
            byte RealBank = (byte)(bank - 1);
                Address MemAddr = address - MemBankSize;

            if (address >= 0 && address <= 0x200 - 1)
            {
                return;
            }
            if ((m_BUS.m_CPU.CR0 & 0x11) == 0x00)
            {
                MemAddr = MemAddr & 0x00FFFF;
                address = address & 0x00FFFF;
            }
            else if ((m_BUS.m_CPU.CR0 & 0x01) == 0x01)
            {
                MemAddr = MemAddr & 0x0FFFFF;
                address = address & 0x0FFFFF;
            }
            else if ((m_BUS.m_CPU.CR0 & 0x11) == 0x11)
            {
                MemAddr = MemAddr & 0xFFFFFF;
                address = address & 0xFFFFFF;
            }
            else if ((m_BUS.m_CPU.CR0 & 0x10) == 0x10)
            {
                MemAddr = MemAddr & 0xF0FFFF;
                address = address & 0xF0FFFF;
            }

            if (address >= 0 && address <= 0x200 - 1)
            {
                return;
            }
            else if (address >= 0 && address <= MemBankSize)
            {
                writeByteMemory(address, 0, byteData);
            }
            else if (MemAddr >= 0 && MemAddr < MemBankEndAddress)
            {
                writeByteBankedMemory(MemAddr, 0, RealBank, byteData);
            }
            else if (address >= 0 && address < MemorySize)
            {
                writeByteMemory(address, 0, byteData);
            }

        }
        public void WriteWord(Address address, byte bank, ushort data)
        {
            byte RealBank = (byte)(bank - 1);
            byte[] byteData = SplitFunctions.SplitWord(data);

            WriteByte(address, RealBank, byteData[0]);
            WriteByte(address + 1, RealBank, byteData[1]);
        }
        public void WriteTByte(Address address, byte bank, uint data)
        {
            byte RealBank = (byte)(bank - 1);
            byte[] byteData = SplitFunctions.SplitTByte(data);

            WriteByte(address, RealBank, byteData[0]);
            WriteByte(address + 1, RealBank, byteData[1]);
            WriteByte(address + 2, RealBank, byteData[2]);
        }
        public void WriteDWord(Address address, byte bank, uint data)
        {
            byte RealBank = (byte)(bank - 1);
            byte[] byteData = SplitFunctions.SplitDWord(data);

            WriteByte(address, RealBank, byteData[0]);
            WriteByte(address + 1, RealBank, byteData[1]);
            WriteByte(address + 2, RealBank, byteData[2]);
            WriteByte(address + 3, RealBank, byteData[3]);
        }

        void writeByteBankedMemory(Address address, int offset, byte bank, byte[] data)
        {
            Address RealAddress = address + offset;
            BankedMemory[bank][RealAddress] = data[offset];
        }
        void writeByteMemory(Address address, int offset, byte[] data)
        {
            Address RealAddress = address + offset;
            Mem[RealAddress] = data[offset];
        }
        byte readByteBankedMemory(Address address, int offset, byte bank)
        {
            Address RealAddress = address + offset;
            return BankedMemory[bank][RealAddress];
        }
        byte readByteMemory(Address address, int offset)
        {
            Address RealAddress = address + offset;
            return Mem[RealAddress];
        }

        public void ConnectBus(BUS bus)
        {
            m_BUS = bus;
        }
    }
}
