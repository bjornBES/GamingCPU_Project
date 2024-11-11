#include "screenIO.h"

const unsigned SCREEN_WIDTH = 80;
const unsigned SCREEN_HEIGHT = 25;
const uint8_t DEFAULT_COLOR = 0x7;

uint8_t *screenBuffer = (uint8_t *)0x00010000;
int cursorx = 0;
int cursory = 0;

uint32_t getCursorPos()
{
    return cursory * SCREEN_WIDTH + cursorx;
}
uint32_t getScreenPos(int x, int y)
{
    return y * SCREEN_WIDTH + x;
}

void clear()
{
    for (int y = 0; y < SCREEN_HEIGHT; y++)
    {
        for (int x = 0; x < SCREEN_WIDTH; x += 2)
        {
            screenBuffer[getScreenPos(x, y)] = 0;
            screenBuffer[getScreenPos(x, y) + 1] = 0;
        }
    }
}

void putc(char c, uint8_t color)
{
    switch (c)
    {
    case '\n':
        cursorx = 0;
        cursory++;
        break;

    default:
        screenBuffer[getCursorPos()] = c;
        screenBuffer[getCursorPos() + 1] = color;
        cursorx += 2;
        break;
    }

    if (cursorx >= SCREEN_WIDTH)
    {
        cursory++;
        cursorx = 0;
    }

    if (cursory >= SCREEN_HEIGHT)
    {
        cursory = 0;
        cursorx = 0;
    }
}
