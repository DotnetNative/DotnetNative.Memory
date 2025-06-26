namespace DotnetNative;

[Flags]
public enum MemoryProtect
{
    ZeroAccess       = 0x0000,
    NoAccess         = 0x0001,
    ReadOnly         = 0x0002,
    ReadWrite        = 0x0004,
    WriteCopy        = 0x0008,
    Execute          = 0x0010,
    ExecuteRead      = 0x0020,
    ExecuteReadWrite = 0x0040,
    ExecuteWriteCopy = 0x0080,
    Guard            = 0x0100,
    ReadWriteGuard   = 0x0104,
    NoCache          = 0x0200
}

public static class MemoryProtectExtensions
{
    public static bool IsWritable(this MemoryProtect self) =>
        self is MemoryProtect.WriteCopy
             or MemoryProtect.ReadWrite
             or MemoryProtect.ExecuteWriteCopy
             or MemoryProtect.ReadWriteGuard
             or MemoryProtect.ExecuteReadWrite;

    public static bool IsReadable(this MemoryProtect self) =>
        self is MemoryProtect.ReadOnly
             or MemoryProtect.ReadWrite
             or MemoryProtect.ReadWriteGuard
             or MemoryProtect.ExecuteReadWrite;

    public static bool IsExecutable(this MemoryProtect self) =>
        self is MemoryProtect.Execute
             or MemoryProtect.ExecuteRead
             or MemoryProtect.ExecuteWriteCopy
             or MemoryProtect.ExecuteReadWrite;
}