// BC16EmulatorC.cpp : Defines the entry point for the application.
//

#include "framework.h"
#include "BC16EmulatorC.h"

// Global Variables
HINSTANCE hInst;
WCHAR szTitle[MAX_LOADSTRING];
WCHAR szWindowClass[MAX_LOADSTRING];
OPENFILENAME binaryFile;
LPWSTR binaryFilePath;
COLOR ColorBuffer[720][400];
size_t StartPos = 0;

int APIENTRY wWinMain(_In_ HINSTANCE hInstance,
                     _In_opt_ HINSTANCE hPrevInstance,
                     _In_ LPWSTR    lpCmdLine,
                     _In_ int       nCmdShow)
{
    for (size_t y = 0; y < 400; y++)
    {
        for (size_t x = 0; x < 720; x++)
        {
            ColorBuffer[x][y].Red = 0;
            ColorBuffer[x][y].Green = 0;
            ColorBuffer[x][y].Blue = 0;
        }
    }

    UNREFERENCED_PARAMETER(hPrevInstance);
    UNREFERENCED_PARAMETER(lpCmdLine);

    // TODO: Place code here.

    // Initialize global strings
    LoadStringW(hInstance, IDS_APP_TITLE, szTitle, MAX_LOADSTRING);
    LoadStringW(hInstance, IDC_BC16EMULATORC, szWindowClass, MAX_LOADSTRING);
    MyRegisterClass(hInstance);

    // Perform application initialization:
    if (!InitInstance (hInstance, nCmdShow))
    {
        return FALSE;
    }

    HACCEL hAccelTable = LoadAccelerators(hInstance, MAKEINTRESOURCE(IDC_BC16EMULATORC));

    MSG msg;

    // Main message loop:
    while (GetMessage(&msg, nullptr, 0, 0))
    {
        if (!TranslateAccelerator(msg.hwnd, hAccelTable, &msg))
        {
            TranslateMessage(&msg);
            DispatchMessage(&msg);

            // Request a repaint by invalidating the entire client area
            // InvalidateRect(msg.hwnd, NULL, TRUE); // NULL means the entire window is invalidated
            // UpdateWindow(msg.hwnd);               // Forces the WM_PAINT message to be processed immediately

        }
    }

    return (int) msg.wParam;
}



//
//  FUNCTION: MyRegisterClass()
//
//  PURPOSE: Registers the window class.
//
ATOM MyRegisterClass(HINSTANCE hInstance)
{
    WNDCLASSEXW wcex;

    wcex.cbSize = sizeof(WNDCLASSEX);

    wcex.style          = CS_HREDRAW | CS_VREDRAW;
    wcex.lpfnWndProc    = WndProc;
    wcex.cbClsExtra     = 0;
    wcex.cbWndExtra     = 0;
    wcex.hInstance      = hInstance;
    wcex.hIcon          = LoadIcon(hInstance, MAKEINTRESOURCE(IDI_BC16EMULATORC));
    wcex.hCursor        = LoadCursor(nullptr, IDC_ARROW);
    wcex.hbrBackground  = (HBRUSH)(COLOR_WINDOW+1);
    wcex.lpszMenuName   = MAKEINTRESOURCEW(IDC_BC16EMULATORC);
    wcex.lpszClassName  = szWindowClass;
    wcex.hIconSm        = LoadIcon(wcex.hInstance, MAKEINTRESOURCE(IDI_SMALL));

    return RegisterClassExW(&wcex);
}

//
//   FUNCTION: InitInstance(HINSTANCE, int)
//
//   PURPOSE: Saves instance handle and creates main window
//
//   COMMENTS:
//
//        In this function, we save the instance handle in a global variable and
//        create and display the main program window.
//
BOOL InitInstance(HINSTANCE hInstance, int nCmdShow)
{
   hInst = hInstance; // Store instance handle in our global variable

   HWND hWnd = CreateWindowW(szWindowClass, szTitle, WS_OVERLAPPEDWINDOW,
      CW_USEDEFAULT, 0, CW_USEDEFAULT, 0, nullptr, nullptr, hInstance, nullptr);

   if (!hWnd)
   {
      return FALSE;
   }

   ShowWindow(hWnd, nCmdShow);
   UpdateWindow(hWnd);

   return TRUE;
}

//
//  FUNCTION: WndProc(HWND, UINT, WPARAM, LPARAM)
//
//  PURPOSE: Processes messages for the main window.
//
//  WM_COMMAND  - process the application menu
//  WM_PAINT    - Paint the main window
//  WM_DESTROY  - post a quit message and return
//
//
LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    switch (message)
    {
    case WM_COMMAND:
        {
            int wmId = LOWORD(wParam);
            // Parse the menu selections:
            switch (wmId)
            {
            case IDM_ABOUT:
                DialogBox(hInst, MAKEINTRESOURCE(IDD_ABOUTBOX), hWnd, About);
                break;
            case IDM_EXIT:
                DestroyWindow(hWnd);
                break;
            case IDM_LOAD_BINARY:
            {
                binaryFilePath = (WCHAR*)malloc(MAX_PATH * sizeof(WCHAR));

                // Initialize OPENFILENAME
                ZeroMemory(&binaryFile, sizeof(binaryFile));
                binaryFile.lStructSize = sizeof(binaryFile);
                binaryFile.hwndOwner = hWnd;
                binaryFile.lpstrFile = binaryFilePath;
                binaryFile.lpstrFile[0] = '\0';
                binaryFile.nMaxFile = MAX_PATH;
                binaryFile.lpstrFilter = L"Binary Files\0*.bin\0";
                binaryFile.nFilterIndex = 1;
                binaryFile.lpstrFileTitle = NULL;
                binaryFile.nMaxFileTitle = 0;
                binaryFile.lpstrInitialDir = NULL;
                binaryFile.Flags = OFN_PATHMUSTEXIST | OFN_FILEMUSTEXIST;

                // Display the Open dialog box
                if (GetOpenFileName(&binaryFile) == TRUE)
                {
                    // The selected file path is now stored in binaryFilePath
                    MessageBox(hWnd, binaryFilePath, L"Selected File", MB_OK);
                    // Invalidate the window to trigger a repaint
                    InvalidateRect(hWnd, NULL, TRUE);
                }
                else
                {
                    // If the user cancels, free the allocated memory
                    free(binaryFilePath);
                    binaryFilePath = NULL;
                }
            }
            break;

            default:
                return DefWindowProc(hWnd, message, wParam, lParam);
            }
        }
        break;
    case WM_PAINT:
        {
            HDC hdc = GetDC(hWnd);

            DrawBinaryFileText(hdc, binaryFilePath);

            DrawScreen(hdc);
            StartPos++;
            if (StartPos == 4)
            {
                StartPos = 0;
            }

            ReleaseDC(hWnd, hdc); // Release the device context

            RECT clintSize;
            clintSize.top = 0;
            clintSize.left = 0;
            clintSize.right = 720;
            clintSize.bottom = 400;

            InvalidateRect(hWnd, &clintSize, false);
        }
        break;
    case WM_DESTROY:
        PostQuitMessage(0);
        break;
    default:
        return DefWindowProc(hWnd, message, wParam, lParam);
    }
    return 0;
}

void ClearWindow(HWND hWnd)
{
    HDC hdc = GetDC(hWnd);
    RECT rect;
    GetClientRect(hWnd, &rect);

    // Fill the client area with the window's background color
    FillRect(hdc, &rect, (HBRUSH)(COLOR_WINDOW + 1));

    ReleaseDC(hWnd, hdc);
}

void DrawScreen(HDC hdc)
{
    for (size_t y = StartPos; y < 400; y += 4)
    {
        for (size_t x = 0; x < 720; x++)
        {
            SetPixel(hdc, (int)x, (int)y, Color2ColorRef(&ColorBuffer[x][y]));
            ColorBuffer[x][y].Red += 2;
            ColorBuffer[x][y].Green += 4;
            ColorBuffer[x][y].Blue += 3;
        }
    }
}

// Example: Draw text in a specified rectangle area
void DrawBinaryFileText(HDC hdc, const wchar_t* binaryFile)
{
    RECT pos = { 0, 200, 700, 500 }; // Initialize RECT without 'new'

    // Use DT_CENTER | DT_VCENTER to center the text within the rectangle
    DrawTextExW(hdc, (LPWSTR)binaryFile, -1, &pos, DT_CENTER | DT_VCENTER | DT_WORDBREAK, NULL);
}


// Message handler for about box.
INT_PTR CALLBACK About(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam)
{
    UNREFERENCED_PARAMETER(lParam);
    switch (message)
    {
    case WM_INITDIALOG:
        return (INT_PTR)TRUE;

    case WM_COMMAND:
        if (LOWORD(wParam) == IDOK || LOWORD(wParam) == IDCANCEL)
        {
            EndDialog(hDlg, LOWORD(wParam));
            return (INT_PTR)TRUE;
        }
        break;
    }
    return (INT_PTR)FALSE;
}
