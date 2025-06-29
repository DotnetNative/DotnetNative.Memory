namespace DotnetNative;
public unsafe static class MemoryAllocator
{
    static nint VirtualAlloc(nint address, long size) => Kernel32.VirtualAlloc(address, size, MemoryState.Commit | MemoryState.Reserve, MemoryProtect.ExecuteReadWrite);

    public static MemoryBaseInfo AllocateNear(nint nearAddress, long size)
    {
        MemoryBaseInfo mbi;

        var topBarrierAddress = Math.Min(nearAddress + 0x7FFFFFF0 - size, 0x7FFFFFFFFFFF);
        var botBarrierAddress = Math.Max(nearAddress - 0x7FFFFFF0, 0x10000);

        var address = nearAddress;
        while (address < topBarrierAddress)
        {
            Query(address, &mbi);
            if (mbi.State == MemoryState.Free && mbi.RegionSize >= size)
            {
                var allocatedAddress = VirtualAlloc(address, size);
                if (allocatedAddress != default)
                    return Query(allocatedAddress);
                else address += 0x1000;
            }
            else address = (IntPtr)(mbi.BaseAddress + mbi.RegionSize);
        }

        address = nearAddress;
        while (address < topBarrierAddress)
        {
            Query(address, &mbi);
            if (mbi.State == MemoryState.Free && mbi.RegionSize >= size)
            {
                var allocatedAddress = VirtualAlloc(address, size);
                if (allocatedAddress != default)
                    return Query(allocatedAddress);
                else address -= 0x1000;
            }
            else address = mbi.AllocationBase - 1;
        }

        return default;
    }

    public static MemoryBaseInfo Allocate(nint address, long size)
    {
        address = VirtualAlloc(address, size);
        if (address == default)
            return default;

        return Query(address);
    }

    public static MemoryBaseInfo Allocate(long size) => Allocate(default, size);

    public static MemoryBaseInfo Query(nint address) => Kernel32.VirtualQuery(address);

    public static void Query(nint address, MemoryBaseInfo* mbi) => Kernel32.VirtualQuery(address, mbi);

    public static void Free(MemoryBaseInfo* mbi) => Free(mbi->BaseAddress);

    // https://learn.microsoft.com/en-us/windows/win32/api/memoryapi/nf-memoryapi-virtualfree: for MemoryFreeType.Release size should be zero
    public static void Free(nint address) => Kernel32.VirtualFree(address, 0, MemoryFreeType.Release);

    public static MemoryBaseInfo QueryNextTop(MemoryBaseInfo* mbi) => Query((IntPtr)(mbi->BaseAddress + mbi->RegionSize));

    public static MemoryBaseInfo QueryNextBot(MemoryBaseInfo* mbi) => Query(mbi->BaseAddress - 1);
}