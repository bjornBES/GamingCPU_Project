
#include "ctypes.h"
#include "screenIO.h"
#include "IDT.h"

extern uint8_t* _IDT_Start;

void _LoadIDT(uint8_t* IDT, uint16_t SizeOfIDT, uint8_t* Des);

int main(uint8_t drive)
{
    _LoadIDT(IDTS, 0x4000, (uint8_t*)0x0);
}