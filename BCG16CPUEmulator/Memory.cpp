#include "Memory.h"

void Memory::InitializeMemory()
{
	Memory = (uint8_t*)malloc(MAX_MEMORY_SIZE);
	
	BankedMemory = (uint8_t**)malloc(0xF);
	for (size_t i = 0; i < 0xF; i++)
	{
		if (BankedMemory == nullptr)
		{
			break;
		}

		BankedMemory[i] = (uint8_t*)malloc(MAX_BANKED_MEMORY_SIZE);
	}
}

void Memory::WriteMemory(uint8_t data, uint32_t address)
{
	WriteMemory(data, address, 0);
}

void Memory::WriteMemory(uint8_t data, uint32_t address, uint8_t bank)
{
	if (IsInBanked(address))
	{
		BankedMemory[bank][address - MAX_BANKED_MEMORY_SIZE] = data;
	}
	else
	{
		Memory[address] = data;
	}
}

bool Memory::IsInBanked(uint32_t address)
{
	if (address >= 0x80000 && address <= 0xF0000)
	{
		return true;
	}

	return false;
}
