using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public unsafe class NativePluginSample : MonoBehaviour
{
    [DllImport("NativeDLL")]
    private static extern int Test(int a, int b);

    [DllImport("NativeDLL")]
    private static extern string TestString(StringBuilder msg, int length);

    [DllImport("NativeDLL")]
    private static extern void TestByteData(char* array, int length);

    [DllImport("NativeDLL")]
    private static extern void TestStructure(IntPtr pStruct);

    struct MyStruct
    {
        public int a;
        public fixed short b[8];
    }

    void Start()
    {
        // int を渡す
        int a = 2;
        int b = 3;
        int c = Test(a, b);

        Debug.Log($"{a} + {b} = {c}");

        // StringBuilder を渡す。c++ 側で書き換えて元に戻す
        StringBuilder s = new StringBuilder();
        s.Append("string argument");

        TestString(s, s.Length);

        Debug.Log($"{s}");

        // byte[] 型のデータを渡す。c++ 側で書き換えて元に戻す
        byte[]    bytes   = { 0, 1, 2, 3, 4,};

        // 1. c++ に渡すためのデータを作る
        int       size    = Marshal.SizeOf(typeof(byte)) * bytes.Length;
        IntPtr    ptr     = Marshal.AllocCoTaskMem(size);
        Marshal.Copy(bytes, 0, ptr, bytes.Length);
        char*     charptr = (char*)(ptr.ToPointer());

        // 2. c++ の関数をコール
        TestByteData(charptr, bytes.Length);

        // 3. c++ に渡したデータの変化を c# に返す
        Marshal.Copy(ptr, bytes, 0, bytes.Length);  // dest と startIndex の位置が変わる…

        Debug.Log($"byte: {bytes[0]}, {bytes[1]}, {bytes[2]}, {bytes[3]}, {bytes[4]}");

        Marshal.FreeCoTaskMem(ptr); // Alloc の解放を忘れずに

        // 構造体のデータを渡す。c++ 側で書き換えて元に戻す
        MyStruct ins = new MyStruct();
        
        // 1. c++ に渡すためのデータを作る
        IntPtr pStructure = Marshal.AllocCoTaskMem(Marshal.SizeOf(ins));
        Marshal.StructureToPtr(ins, pStructure, false);

        // 2. c++ の関数をコール
        TestStructure(pStructure);

        // 3. c++ に渡したデータの変化を c# に返す
        ins = (MyStruct)Marshal.PtrToStructure(pStructure, typeof(MyStruct));

        Debug.Log($"struct: a/{ins.a}, b0/{ins.b[0]}, b1/{ins.b[1]}, b2/{ins.b[2]}");

        Marshal.FreeCoTaskMem(pStructure); // Alloc の解放を忘れずに
    }
}
