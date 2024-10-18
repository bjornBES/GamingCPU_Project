using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonBCGCPU.Types;
using HexLibrary;
namespace BCG16CPUEmulator
{
    public class Memory
    {
        public const uint MemBankEndAddress = 0x008_0000;

        public const uint TotalMemorySize = 16 * 1024 * 1024;               // 16 MB
        public const uint InterruptRoutineCount = 0xFF;
        public const uint AllInterruptRoutineSize = 1024;

        public const uint MemorySize = TotalMemorySize;

        public byte[] m_Mem = new byte[TotalMemorySize];
        BUS m_bUS;

        public void Reset()
        {
            Array.Clear(m_Mem, 0, m_Mem.Length);

        }

        public Memory()
        {
            m_Mem.Initialize();
        }

        public byte ReadByte(Address pAddress)
        {
            Address MemAddr = pAddress;

            if (MemAddr >= 0 && MemAddr <= 0x200 - 1)
            {
                return 0;
            }
            else if ((m_bUS.m_CPU.m_CR0 & BC16CPU_Registers.CR0_EnableExtendedMode) == 0x00)
            {
                MemAddr = MemAddr & 0x00FFFF;
            }
            else if ((m_bUS.m_CPU.m_CR0 & BC16CPU_Registers.CR0_EnableExtendedMode) == BC16CPU_Registers.CR0_EnableExtendedMode)
            {
                MemAddr = MemAddr & 0xFFFFFF;
            }

            if (MemAddr >= 0 && MemAddr <= 0x200 - 1)
            {
            }
            else if (MemAddr >= 0 && MemAddr <= MemorySize)
            {
                return readByteMemory(MemAddr, 0);
            }
            else
            {

            }

            return 0;
        }
        public ushort ReadWord(Address pAddress)
        {
            byte[] bytes = ReadBytes(pAddress, 2);

            return BitConverter.ToUInt16(bytes);
        }
        public uint ReadTByte(Address pAddress)
        {
            List<byte> bytes = ReadBytes(pAddress, 3).ToList();
            bytes.Add(0);

            return BitConverter.ToUInt32(bytes.ToArray());

        }
        public int ReadDWord(Address pAddress)
        {
            byte[] bytes = ReadBytes(pAddress, 4);

            return BitConverter.ToInt32(bytes);
        }
        public byte[] ReadBytes(Address pAddress, int count)
        {
            Address MemAddr = pAddress;

            if (pAddress >= 0x1000 && pAddress <= 0x1200 - 1)
            {
                return null;
            }
            else if ((m_bUS.m_CPU.m_CR0 & BC16CPU_Registers.CR0_EnableExtendedMode) == 0x00)
            {
                MemAddr = MemAddr & 0x00FFFF;
            }
            else if ((m_bUS.m_CPU.m_CR0 & BC16CPU_Registers.CR0_EnableExtendedMode) == BC16CPU_Registers.CR0_EnableExtendedMode)
            {
                MemAddr = MemAddr & 0xFFFFFF;
            }

            if (MemAddr >= 0x1000 && MemAddr <= 0x1200 - 1)
            {
            }
            else if (MemAddr >= 0 && MemAddr <= MemorySize)
            {
                return m_Mem[(int)pAddress..(MemAddr + count)];
            }
            else
            {
            }
            return null;
        }

        public void WriteByte(Address pAddress, byte data)
        {
            byte[] byteData = new byte[] { data };
            Address MemAddr = pAddress ;

            if (MemAddr >= 0x1000 && MemAddr <= 0x1200 - 1)
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

            if (MemAddr >= 0x1000 && MemAddr <= 0x1200 - 1)
            {
                return;
            }
            else if (MemAddr >= 0 && MemAddr <= MemorySize)
            {
                writeByteMemory(MemAddr, 0, byteData);
            }
        }
        public void WriteWord(Address pAddress, ushort data)
        {
            byte[] byteData = SplitFunctions.SplitWord(data);

            WriteByte(pAddress, byteData[0]);
            WriteByte(pAddress + 1, byteData[1]);
        }
        public void WriteTByte(Address pAddress, uint data)
        {
            byte[] byteData = SplitFunctions.SplitTByte(data);

            WriteByte(pAddress, byteData[0]);
            WriteByte(pAddress + 1, byteData[1]);
            WriteByte(pAddress + 2, byteData[2]);
        }
        public void WriteDWord(Address pAddress, uint data)
        {
            byte[] byteData = SplitFunctions.SplitDWord(data);

            WriteByte(pAddress, byteData[0]);
            WriteByte(pAddress + 1, byteData[1]);
            WriteByte(pAddress + 2, byteData[2]);
            WriteByte(pAddress + 3, byteData[3]);
        }
        public void WriteBytes(Address pAddress, byte[] data)
        {
            byte[] byteData = data;
            Address MemAddr = pAddress;

            if (MemAddr >= 0 && MemAddr <= 0x200 - 1)
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

            if (MemAddr >= 0 && MemAddr <= 0x200 - 1)
            {
                return;
            }
            else if (MemAddr >= 0 && MemAddr <= MemorySize)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    writeByteMemory(pAddress, i, byteData);
                }
            }
        }

        void writeByteMemory(Address pAddress, int offset, byte[] data)
        {
            Address RealAddress = pAddress + offset;
            m_Mem[RealAddress] = data[offset];
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
