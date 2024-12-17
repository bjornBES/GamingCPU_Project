using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonBCGCPU.Types;
using HexLibrary;
namespace BC16CPUEmulator
{
    public class Memory
    {
        public const ulong TotalMemorySize = 4 * (ulong)(1024 * 1024 * 1024);               // 16 MB
        public const int MemSize = 1024 * 1024 * 1024;
        public const ulong MemorySize = TotalMemorySize;

        public byte[] m_Mem1 = new byte[MemSize];
        public byte[] m_Mem2 = new byte[MemSize];
        BUS m_bUS;

        public void Reset()
        {
            Array.Clear(m_Mem1, 0, m_Mem1.Length);
            Array.Clear(m_Mem2, 0, m_Mem2.Length);
        }

        public Memory()
        {
            m_Mem1.Initialize();
            m_Mem2.Initialize();
        }

        Address MaskAddress(Address Address)
        {
            Address mask = 0x000FFFFF;

            if ((m_bUS.m_CPU.m_CR0 & (BC16CPU_Registers.CR0_EnableExtendedMode | BC16CPU_Registers.CR0_EnableProtectedMode)) != 0)
            {
                mask |= 0xFF0FFFFF;
            }
            if ((m_bUS.m_CPU.m_CR0 & (BC16CPU_Registers.CR0_EnableExtendedMode | BC16CPU_Registers.CR0_EnableA24)) == BC16CPU_Registers.CR0_EnableExtendedMode)
            {
                mask |= 0x00FFFFFF;
            }

            Address &= mask;
            return Address;
        }

        public byte ReadByte(Address pAddress)
        {
            Address MemAddr = pAddress;

            MemAddr = MaskAddress(MemAddr);

            if (MemAddr >= 0 && MemAddr <= MemorySize)
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

            MemAddr = MaskAddress(MemAddr);
            List<byte> bytes = new List<byte>();

            if (MemAddr >= 0 && MemAddr <= MemorySize)
            {
                for (int i = 0; i < count; i++)
                {
                    bytes.Add(readByteMemory(MemAddr, i));
                }
            }
            else
            {
            }
            return bytes.ToArray();
        }
        public byte[] ReadVRAM(int count)
        {
            Address MemAddr = 0x00010000;

            List<byte> bytes = new List<byte>();
            if (MemAddr >= 0 && MemAddr <= MemorySize)
            {
                for (int i = 0; i < count; i++)
                {
                    bytes.Add(readByteMemory(MemAddr, i));
                }
            }
            else
            {
            }
            return bytes.ToArray();
        }

        public void WriteByte(Address pAddress, byte data)
        {
            byte[] byteData = new byte[] { data };
            Address MemAddr = pAddress;
            MemAddr = MaskAddress(MemAddr);

            if (MemAddr >= 0 && MemAddr <= MemorySize)
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

            MemAddr = MaskAddress(MemAddr);

            if (MemAddr >= 0 && MemAddr <= MemorySize)
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
            if (RealAddress < MemSize)
            {
                m_Mem1[RealAddress] = data[offset];
            }
            else
            {
                m_Mem2[RealAddress - MemSize] = data[offset];
            }
        }
        byte readByteMemory(Address pAddress, int offset)
        {
            Address RealAddress = pAddress + offset;
            if (RealAddress < MemSize)
            {
                return m_Mem1[RealAddress];
            }
            else
            {
                return m_Mem2[RealAddress - MemSize];
            }
        }

        public void ConnectBus(BUS bus)
        {
            m_bUS = bus;
        }
    }
}
