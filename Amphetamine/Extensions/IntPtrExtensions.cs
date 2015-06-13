using System;

namespace Amphetamine.Extensions
{
    internal static class IntPtrExtensions
    {
        public unsafe static void MemSet(this IntPtr pointer, long length, byte value)
        {
            byte* ptr = (byte*)pointer.ToPointer();

            ulong value8 = (ulong)value << 56 | (ulong)value << 48 | (ulong)value << 40 | (ulong)value << 32 | (ulong)value << 24 | (ulong)value << 16 | (ulong)value << 8 | (ulong)value;

            //Set the memory to zero (in ulong sized blocks)
            var offset = 0;
            while ((length - offset) >= sizeof(ulong))
            {
                *(ulong*)(ptr + offset) = value8;
                offset += sizeof(ulong);
            }

            //Set the rest to zero (byte by byte)
            for (int i = 0; i < length - offset; i++)
                *(ptr + offset + i) = value;
        }
    }
}
