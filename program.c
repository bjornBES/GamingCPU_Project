#include <stdio.h>

#define test 1

#if test >= 2

void PrintHelloWorld()
{
    printf("Hello world!");
}

#endif

int main()
{
    return 0;
}