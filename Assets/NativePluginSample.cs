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
        // int ��n��
        int a = 2;
        int b = 3;
        int c = Test(a, b);

        Debug.Log($"{a} + {b} = {c}");

        // StringBuilder ��n���Bc++ ���ŏ��������Č��ɖ߂�
        StringBuilder s = new StringBuilder();
        s.Append("string argument");

        TestString(s, s.Length);

        Debug.Log($"{s}");

        // byte[] �^�̃f�[�^��n���Bc++ ���ŏ��������Č��ɖ߂�
        byte[]    bytes   = { 0, 1, 2, 3, 4,};

        // 1. c++ �ɓn�����߂̃f�[�^�����
        int       size    = Marshal.SizeOf(typeof(byte)) * bytes.Length;
        IntPtr    ptr     = Marshal.AllocCoTaskMem(size);
        Marshal.Copy(bytes, 0, ptr, bytes.Length);
        char*     charptr = (char*)(ptr.ToPointer());

        // 2. c++ �̊֐����R�[��
        TestByteData(charptr, bytes.Length);

        // 3. c++ �ɓn�����f�[�^�̕ω��� c# �ɕԂ�
        Marshal.Copy(ptr, bytes, 0, bytes.Length);  // dest �� startIndex �̈ʒu���ς��c

        Debug.Log($"byte: {bytes[0]}, {bytes[1]}, {bytes[2]}, {bytes[3]}, {bytes[4]}");

        Marshal.FreeCoTaskMem(ptr); // Alloc �̉����Y�ꂸ��

        // �\���̂̃f�[�^��n���Bc++ ���ŏ��������Č��ɖ߂�
        MyStruct ins = new MyStruct();
        
        // 1. c++ �ɓn�����߂̃f�[�^�����
        IntPtr pStructure = Marshal.AllocCoTaskMem(Marshal.SizeOf(ins));
        Marshal.StructureToPtr(ins, pStructure, false);

        // 2. c++ �̊֐����R�[��
        TestStructure(pStructure);

        // 3. c++ �ɓn�����f�[�^�̕ω��� c# �ɕԂ�
        ins = (MyStruct)Marshal.PtrToStructure(pStructure, typeof(MyStruct));

        Debug.Log($"struct: a/{ins.a}, b0/{ins.b[0]}, b1/{ins.b[1]}, b2/{ins.b[2]}");

        Marshal.FreeCoTaskMem(pStructure); // Alloc �̉����Y�ꂸ��
    }
}
