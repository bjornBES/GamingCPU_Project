#pragma once

#include "ctypes.h"

bool ReadSector(uint8_t drive, uint8_t head, uint16_t track, uint16_t sector, uint8_t* des);
