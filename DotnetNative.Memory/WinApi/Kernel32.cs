using DotnetNative;
using System.Runtime.InteropServices;

unsafe static partial class Kernel32
{
    const string kernel = "kernel32";

    [LibraryImport(kernel)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool VirtualProtect(nint address, long size, MemoryProtect newProtect, MemoryProtect* oldProtect);

    public static bool VirtualProtect(nint address, long size, MemoryProtect newProtect)
    {
        MemoryProtect oldProtection;
        return VirtualProtect(address, size, newProtect, &oldProtection);
    }

    [LibraryImport(kernel)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool VirtualQuery(nint address, MemoryBaseInfo* buffer, int length);

    public static bool VirtualQuery(nint baseAddress, MemoryBaseInfo* mbi) => VirtualQuery(baseAddress, mbi, sizeof(MemoryBaseInfo));

    public static MemoryBaseInfo VirtualQuery(nint baseAddress)
    {
        MemoryBaseInfo mbi;
        if (!VirtualQuery(baseAddress, &mbi, sizeof(MemoryBaseInfo)))
            return default;
        return mbi;
    }

    [LibraryImport(kernel)] 
    public static partial nint VirtualAlloc(nint address, long size, MemoryState allocationType, MemoryProtect protect);

    [LibraryImport(kernel)] 
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool VirtualFree(nint address, long size, MemoryFreeType freeType);
}