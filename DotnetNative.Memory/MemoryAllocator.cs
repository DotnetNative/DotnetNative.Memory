namespace DotnetNative;
public unsafe static class MemoryAllocator
{
    public static MemoryBaseInfo AllocateNear(IntPtr address, long size)
    {
        size = (size & 0xFFFFF000) + ((size & 0xFFF) > 0 ? 0x1000 : 0); // int anyway

        var mbi = FindFreeNear(address, size);
        if (!mbi.IsValid)
            return default;

        return Allocate(mbi.BaseAddress, size);
    }

    public static MemoryBaseInfo Allocate(IntPtr address, long size)
    {
        address = Kernel32.VirtualAlloc(address, size, MemoryState.Commit | MemoryState.Reserve, MemoryProtect.ExecuteReadWrite);
        return Query(address);
    }

    public static MemoryBaseInfo Allocate(long size) => Allocate(default, size);

    public static MemoryBaseInfo Query(IntPtr address) => Kernel32.VirtualQuery(address);

    public static void Query(IntPtr address, MemoryBaseInfo* mbi) => Kernel32.VirtualQuery(address, mbi);

    public static MemoryBaseInfo FindFreeNear(IntPtr address, long size)
    {
        var top = QueryTopFirstFree(address, size);
        var bot = QueryBotFirstFree(address, size);

        var topDistance = top.BaseAddress - address;
        var botDistance = address - bot.BaseAddress;

        if (!top.IsValid || !bot.IsValid)
        {
            if (top.IsValid && topDistance < 0xFFFFFFFF)
                return top;

            if (bot.IsValid && botDistance < 0xFFFFFFFF)
                return bot;

            return default;
        }

        if (botDistance < topDistance && botDistance < 0xFFFFFFFF)
            return bot;

        if (topDistance < botDistance && topDistance < 0xFFFFFFFF)
            return top;

        return default;
    }

    // https://learn.microsoft.com/en-us/windows/win32/api/memoryapi/nf-memoryapi-virtualfree: for MemoryFreeType.Release size should be zero
    public static void Free(MemoryBaseInfo* mbi) => Free(mbi->BaseAddress);
    public static void Free(IntPtr address) => Kernel32.VirtualFree(address, 0, MemoryFreeType.Release);

    public static MemoryBaseInfo QueryTopFirstFree(IntPtr address, long size)
    {
        var mbi = Query(address);
        while (address < 0x7FFFFFFFFFFF)
        {
            if (mbi.State == MemoryState.Free && mbi.RegionSize >= size)
                return mbi;

            address = (IntPtr)(mbi.BaseAddress + mbi.RegionSize);
            Query(address, &mbi);
        }

        return default;
    }

    public static MemoryBaseInfo QueryBotFirstFree(IntPtr address, long size)
    {
        var mbi = Query(address);
        while (address > 0x10000)
        {
            if (mbi.State == MemoryState.Free && mbi.RegionSize >= size)
                return mbi;

            address = mbi.BaseAddress - 1;
            Query(address, &mbi);
        }

        return default;
    }

    public static MemoryBaseInfo QueryNextTop(MemoryBaseInfo* mbi) => Query((IntPtr)((long)mbi->BaseAddress + mbi->RegionSize));
    public static MemoryBaseInfo QueryNextBot(MemoryBaseInfo* mbi) => Query(mbi->BaseAddress - 1);
}