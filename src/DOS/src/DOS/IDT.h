
#pragma once

#include "ctypes.h"

struct IDT
{
    uint32_t    Segment;
    uint16_t    Offset1;                // [0-15]
    uint8_t     Offset2;                // [16-20]
    uint8_t     Flags;                  // 
};

struct IDT IDTS[256];

void LoadEntry(uint8_t index, uint32_t Segment, uint32_t Offset, uint8_t flags);
