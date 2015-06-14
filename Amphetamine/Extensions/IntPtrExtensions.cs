using System;

namespace Amphetamine.Extensions
{
    internal static class IntPtrExtensions
    {
        public unsafe static void MemSet(this IntPtr pointer, byte value, long length, long offset = 0)
        {
            byte* ptr = ((byte*)pointer.ToPointer()) + offset;

            ulong value8 = (ulong)value << 56 | (ulong)value << 48 | (ulong)value << 40 | (ulong)value << 32 | (ulong)value << 24 | (ulong)value << 16 | (ulong)value << 8 | (ulong)value;

            //Set the memory to zero (in ulong sized blocks)
            var count = 0;
            while ((length - count) >= sizeof(ulong))
            {
                *(ulong*)(ptr + count) = value8;
                count += sizeof(ulong);
            }

            //Set the rest to zero (byte by byte)
            for (int i = 0; i < length - count; i++)
                *(ptr + count + i) = value;
        }
    }
}
