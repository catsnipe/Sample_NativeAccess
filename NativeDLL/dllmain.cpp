// dllmain.cpp : DLL アプリケーションのエントリ ポイントを定義します。
#include "pch.h"

typedef struct MyStruct
{
    int   a;
    short b[8];
} MyStruct;

extern "C"
{
    __declspec(dllexport) int __stdcall Test(int a, int b)
    {
        return a + b;
    }

    __declspec(dllexport) void __stdcall TestString(char* msg)
    {
    }

    __declspec(dllexport) void __stdcall TestByteData(char* array, int length)
    {
        array[0] = 4;
        array[1] = 3;
        array[2] = 2;
        array[3] = 1;
        array[4] = 0;
    }

    __declspec(dllexport) void __stdcall TestStructure(MyStruct* data)
    {
        data->a = 10;
        data->b[0] = 1;
        data->b[1] = 2;
        data->b[2] = 3;
        data->b[3] = 4;
        data->b[4] = 5;
    }
}
