using System.Runtime.InteropServices;

namespace Amphetamine.Blocks
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct StoreHeader
    {
        public const ulong MAGIC = 0xFE8877EF5D2E1F9B;

        [FieldOffset(0)]
        public readonly ulong Magic;

        [FieldOffset(sizeof(ulong))]
        public readonly int BlockSize;

        [FieldOffset(sizeof(ulong) + sizeof(int))]
        public readonly long TotalSize;

        public StoreHeader(int blockSize, long totalSize)
        {
            Magic = MAGIC;

            BlockSize = blockSize;
            TotalSize = totalSize;
        }
    }
}
