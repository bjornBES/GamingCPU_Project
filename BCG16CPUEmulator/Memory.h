#pragma once

#include <iostream>

class Memory
{
public:
	void InitializeMemory();
	void WriteMemory(uint8_t data, uint32_t address);
	void WriteMemory(uint8_t data, uint32_t address, uint8_t bank);

private:

	bool IsInBanked(uint32_t address);

	const uint32_t MAX_BANKED_MEMORY_SIZE = 0x80000;
	const uint32_t MAX_MEMORY_SIZE = (0x1000 * 0x1000) - MAX_BANKED_MEMORY_SIZE;
	
	uint8_t** BankedMemory;
	uint8_t* Memory;

};