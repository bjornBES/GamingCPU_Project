using System;
using System.Collections.Generic;
using System.Text;
using BCG16CPUEmulator.Types;
using HexLibrary;
namespace BCG16CPUEmulator
{
    public class Memory
    {
        public const uint MemBankSize = 0x0080000;
        public const uint TotalMemorySize =    4096 * 4096; // 16 MB
        public const uint MemorySize = TotalMemorySize - MemBankSize;
        public const uint MaxBanks = 0xF;

        public byte[][] BankedMemory = new byte[MaxBanks][];
        public byte[] Mem = new byte[MemorySize + 1];
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
        public uint ReadDWord(Address address)
        {
            return ReadDWord(address, 1);
        }
        public byte ReadByte(Address address, byte bank)
        {
            byte RealBank = (byte)(bank - 1);

            Address MemAddr = address - MemBankSize;
            if (address >= 0 && address <= 0x200)
            {
                return readByteBankedMemory(address, 0, RealBank);
            }
            else if (address >= 0 && address <= MemBankSize + 0x200)
            {
                return readByteBankedMemory(address, 0, RealBank);
            }
            else if (MemAddr >= 0 &&
                    MemAddr < MemorySize)
            {
                return readByteMemory(MemAddr, 0);
            }
            else
            {

            }

            return 0;
        }
        public ushort ReadWord(Address address, byte bank)
        {
            byte RealBank = (byte)(bank - 1);

            Address MemAddr = address - MemBankSize;

            byte high = 0;
            byte low = 0;

            if (address >= 0 && address + 1 >= 0 &&
                address <= MemBankSize && address + 1 <= MemBankSize)
            {
                high = readByteBankedMemory(address, 0, RealBank);
                low = readByteBankedMemory(address, 1, RealBank);
            }
            else if (MemAddr >= 0 && MemAddr + 1 >= 0 &&
                    MemAddr < MemorySize && MemAddr + 1 < MemorySize)
            {
                high = readByteMemory(MemAddr, 0);
                low = readByteMemory(MemAddr, 1);
            }
            else
            {

            }

            return (ushort)((high << 8) | low);
        }
        public uint ReadTByte(Address address, byte bank)
        {
            byte RealBank = (byte)(bank - 1);

            Address MemAddr = address - MemBankSize;

            byte high = 0;
            byte middle = 0;
            byte low = 0;

            if (address >= 0 && address + 1 >= 0 && address + 2 >= 0 &&
                address <= MemBankSize && address + 1 <= MemBankSize && address + 2 <= MemBankSize)
            {
                high = readByteBankedMemory(address, 0, RealBank);
                middle = readByteBankedMemory(address, 1, RealBank);
                low = readByteBankedMemory(address, 2, RealBank);
            }
            else if (MemAddr >= 0 && MemAddr + 1 >= 0 && MemAddr + 2 >= 0 &&
                    MemAddr < MemorySize && MemAddr + 1 < MemorySize && MemAddr + 2 < MemorySize)
            {
                high = readByteMemory(MemAddr, 0);
                middle = readByteMemory(MemAddr, 1);
                low = readByteMemory(MemAddr, 2);
            }
            else
            {

            }

            return (uint)((high << 16) | (middle << 8) | low);

        }
        public uint ReadDWord(Address address, byte bank)
        {
            byte RealBank = (byte)(bank - 1);

            Address MemAddr = address - MemBankSize;

            byte high = 0;
            byte byte3 = 0;
            byte byte2 = 0;
            byte low = 0;

            if (address >= 0 && address + 1 >= 0 && address + 2 >= 0 && address + 3 >= 0 &&
                address <= MemBankSize && address + 1 <= MemBankSize && address + 2 <= MemBankSize && address + 3 <= MemBankSize)
            {
                high = readByteBankedMemory(address, 0, RealBank);
                byte3 = readByteBankedMemory(address, 1, RealBank);
                byte2 = readByteBankedMemory(address, 2, RealBank);
                low = readByteBankedMemory(address, 3, RealBank);
            }
            else if (MemAddr >= 0 && MemAddr + 1 >= 0 && MemAddr + 2 >= 0 && MemAddr + 3 >= 0 &&
                    MemAddr < MemorySize && MemAddr + 1 < MemorySize && MemAddr + 2 < MemorySize && MemAddr + 3 < MemorySize)
            {
                high = readByteMemory(MemAddr, 0);
                byte3 = readByteMemory(MemAddr, 1);
                byte2 = readByteMemory(MemAddr, 2);
                low = readByteMemory(MemAddr, 3);
            }
            else
            {

            }

            return (uint)((high << 24) | (byte3 << 16) | (byte2 << 8) | low);
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
            Address ExtMemAddr = address - TotalMemorySize;
            if (address >= 0 && address <= MemBankSize)
            {
                writeByteBankedMemory(address, 0, RealBank, byteData);
            }
            else if (MemAddr >= 0 &&
                    MemAddr < MemorySize)
            {
                writeByteMemory(MemAddr, 0, byteData);
            }
            else
            {

            }
        }
        public void WriteWord(Address address, byte bank, ushort data)
        {
            byte RealBank = (byte)(bank - 1);
            byte[] byteData = SplitFunctions.SplitWord(data);

            Address MemAddr = address - MemBankSize;
            Address ExtMemAddr = address - TotalMemorySize;

            if (address >= 0 && address + 1 >= 0 &&
                address <= MemBankSize && address + 1 <= MemBankSize)
            {
                writeByteBankedMemory(address, 0, RealBank, byteData);
                writeByteBankedMemory(address, 1, RealBank, byteData);
            }
            else if (MemAddr >= 0 && MemAddr + 1 >= 0 &&
                    MemAddr < MemorySize && MemAddr + 1 < MemorySize)
            {
                writeByteMemory(MemAddr, 0, byteData);
                writeByteMemory(MemAddr, 1, byteData);
            }
            else
            {

            }
        }
        public void WriteTByte(Address address, byte bank, uint data)
        {
            byte RealBank = (byte)(bank - 1);
            byte[] byteData = SplitFunctions.SplitTByte(data);

            Address MemAddr = address - MemBankSize;
            Address ExtMemAddr = address - TotalMemorySize;

            if (address >= 0 && address + 1 >= 0 && address + 2 >= 0 &&
                address <= MemBankSize && address + 1 <= MemBankSize && address + 2 <= MemBankSize)
            {
                writeByteBankedMemory(address, 0, RealBank, byteData);
                writeByteBankedMemory(address, 1, RealBank, byteData);
                writeByteBankedMemory(address, 2, RealBank, byteData);
            }
            else if (MemAddr >= 0 && MemAddr + 1 >= 0 && MemAddr + 2 >= 0 &&
                    MemAddr < MemorySize && MemAddr + 1 < MemorySize && MemAddr + 2 < MemorySize)
            {
                writeByteMemory(MemAddr, 0, byteData);
                writeByteMemory(MemAddr, 1, byteData);
                writeByteMemory(MemAddr, 2, byteData);
            }
            else
            {

            }
        }
        public void WriteDWord(Address address, byte bank, uint data)
        {
            byte RealBank = (byte)(bank - 1);
            byte[] byteData = SplitFunctions.SplitDWord(data);

            Address MemAddr = address - MemBankSize;
            Address ExtMemAddr = address - TotalMemorySize;

            if (address >= 0 && address + 1 >= 0 && address + 2 >= 0 && address + 3 >= 0 &&
                address <= MemBankSize && address + 1 <= MemBankSize && address + 2 <= MemBankSize && address + 3 <= MemBankSize)
            {
                writeByteBankedMemory(address, 0, RealBank, byteData);
                writeByteBankedMemory(address, 1, RealBank, byteData);
                writeByteBankedMemory(address, 2, RealBank, byteData);
                writeByteBankedMemory(address, 3, RealBank, byteData);
            }
            else if (MemAddr >= 0 && MemAddr + 1 >= 0 && MemAddr + 2 >= 0 && MemAddr + 3 >= 0 &&
                    MemAddr < MemorySize && MemAddr + 1 < MemorySize && MemAddr + 2 < MemorySize && MemAddr + 3 < MemorySize)
            {
                writeByteMemory(MemAddr, 0, byteData);
                writeByteMemory(MemAddr, 1, byteData);
                writeByteMemory(MemAddr, 2, byteData);
                writeByteMemory(MemAddr, 3, byteData);
            }
            else
            {

            }
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
