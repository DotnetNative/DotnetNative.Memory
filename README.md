# Memory.Extensions [![NuGet](https://img.shields.io/nuget/v/Yotic.Memory.Extensions.svg)](https://www.nuget.org/packages/Yotic.Memory.Extensions)

Memory utils for convenient work with memory, arrays and pointers.\
The library is mainly intended for use with native code, such as C++ or in conjunction with Native AOT. All actions are carried out with the heap, all allocated pointers do not change the address, so they can be safely used from any part of the code

Allocating memory
------------------------------
```C#
void* ptr = Memory.Alloc(6); // Allocates 6 bytes in memory
// ...
Memory.Free(ptr); // Free pointer
```
You can use other overloads of Alloc method for convenient work
```C#
int* ptr = Memory.Alloc<int>(6); // Allocates sizeof(int) * 6 bytes in memory
```

Allocating strings
------------------------------
```C#
string managedString = "Some unicode string";
using CoMem strCo = new CoMem(managedString, CoStrType.Uni);

ushort* unmanagedStringPtr = (ushort*)strCo;
//...
```

Versions
------------------------------
| Start ordinal | Framework | Description                  | Date         |
| ---           | ---       | ---                          | ---          |
| 1.0.0         | .net9.0   | -                            | Jun 26, 2025 |
| 0.0.0         | -         | -                            | Jan 1, 1970  |
| 2.0.1         | .net8.0   | Added MIT License            | Apr 28, 2024 |
| 2.0.0         | .net8.0   | Switched to DotnetNativeBase | Apr 25, 2024 |
| 1.1.0         | .net8.0   | Changed framework            | Nov 15, 2023 |
| 1.0.0         | .net7.0   |                              | Sep 5, 2023  |
