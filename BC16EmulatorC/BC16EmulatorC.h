#pragma once

#include "resource.h"
#include <windows.h>

#include <stdint.h>

// Constants
#define MAX_LOADSTRING 100

typedef struct COLOR
{
	uint8_t Red;
	uint8_t Green;
	uint8_t Blue;
};

typedef COLOR color;

COLORREF Color2ColorRef(color* color)
{
	COLORREF result = 0;

	result |= color->Red;
	result |= color->Green << 8;
	result |= color->Blue << 16;
	return result;
}

// Global Variables
extern HINSTANCE hInst;
extern WCHAR szTitle[MAX_LOADSTRING];
extern WCHAR szWindowClass[MAX_LOADSTRING];
extern OPENFILENAME binaryFile;
extern LPWSTR binaryFilePath;
extern COLOR ColorBuffer[720][400];
extern size_t StartPos;

// Function Declarations
ATOM MyRegisterClass(HINSTANCE hInstance);
BOOL InitInstance(HINSTANCE, int);
LRESULT CALLBACK WndProc(HWND, UINT, WPARAM, LPARAM);
INT_PTR CALLBACK About(HWND, UINT, WPARAM, LPARAM);

void ClearWindow(HWND hWnd);
void DrawBinaryFileText(HDC hdc, const wchar_t* binaryFile);
void DrawScreen(HDC hdc);
