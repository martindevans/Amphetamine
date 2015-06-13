using System.Runtime.InteropServices;

namespace Amphetamine.Files
{
    /// <summary>
    /// Individual file record in an allocation table, table is filled with these records
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal struct FileHeader
    {
        public const ulong MAGIC = 0xFE8877EF5D2E1F9B;

        [FieldOffset(0)]
        public readonly ulong Magic;

        /// <summary>
        /// ID of this file
        /// </summary>
        [FieldOffset(sizeof(long) * 0)]
        public long FileId;

        /// <summary>
        /// Start position of this file
        /// </summary>
        [FieldOffset(sizeof(long) * 1)]
        public long StartOffset;

        /// <summary>
        /// Length of this file
        /// </summary>
        [FieldOffset(sizeof(long) * 2)]
        public long Length;

        public FileHeader(long fileId, long offset, long length)
        {
            Magic = MAGIC;

            FileId = fileId;
            StartOffset = offset;
            Length = length;
        }
    }
}
