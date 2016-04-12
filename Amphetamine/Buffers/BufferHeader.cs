using System.Runtime.InteropServices;

namespace Amphetamine.Buffers
{
    /// <summary>
    /// A header placed at the start of a buffer
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal struct BufferHeader
    {
        public const ulong MAGIC = 0xFE8877EF5D2E1F9B;

        [FieldOffset(0)]
        public readonly ulong Magic;

        /// <summary>
        /// ID of this buffer
        /// </summary>
        [FieldOffset(sizeof(long) * 1)]
        public readonly long FileId;

        /// <summary>
        /// Start position of this buffer
        /// </summary>
        [FieldOffset(sizeof(long) * 2)]
        public readonly long StartOffset;

        /// <summary>
        /// Length of this buffer
        /// </summary>
        [FieldOffset(sizeof(long) * 3)]
        public readonly long Length;

        public BufferHeader(long fileId, long offset, long length)
        {
            Magic = MAGIC;

            FileId = fileId;

            unsafe
            {
                StartOffset = offset + sizeof(BufferHeader);
            }

            Length = length;
        }
    }
}
