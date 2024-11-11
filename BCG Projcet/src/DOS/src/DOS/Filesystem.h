#include <stdint.h>
typedef uint8_t bool;

typedef struct DISK{
    uint8_t id;
    uint16_t tracks;
    uint16_t sectors;
    uint16_t heads;
};

typedef struct File
{
    char* fileName[0xB];
    char* fileType[0x5];
    uint16_t startingSector;
    uint8_t fileSizeInSectors;
    uint8_t flags;
    uint16_t CreateDate;
    uint16_t CreateTime;
    uint8_t usused[0x8];

};


typedef struct FileHandler {
    uint32_t Handler;
    uint8_t IsDirectory;
    uint16_t Position;
    uint16_t Size;
};

struct FileData
{
    uint8_t Buffer[512];
    struct FileHandler Public;
    bool Opened;
    uint32_t FirstSector;
};

struct FileHandler fileHandlers[0x08];
