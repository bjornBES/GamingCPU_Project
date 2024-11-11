#include "IDT.h"

void LoadEntry(uint8_t index, uint32_t Segment, uint32_t Offset, uint8_t flags)
{
    IDTS[index].Segment = Segment;
    IDTS[index].Offset1 = (Offset & 0x0000FFFF);
    IDTS[index].Offset2 = ((Offset & 0x000F0000) >> 16);
    IDTS[index].Flags = flags;
}