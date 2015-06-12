using System.Runtime.InteropServices;

namespace Amphetamine.Files
{
    /// <summary>
    /// First item in an allocation table block, indicates metadata about the rest of the block
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal struct AllocationTableHeader
    {
        /// <summary>
        /// Smallest file ID held in this alloc table
        /// </summary>
        [FieldOffset(sizeof(long) * 0)]
        public long SmallestFileId;

        /// <summary>
        /// Largest file ID held in this alloc table
        /// </summary>
        [FieldOffset(sizeof(long) * 1)]
        public long LargestFileId;

        /// <summary>
        /// If there is not enough space in this block for the entire allocation table a new one will be created. This field is the Block ID of the next alloc table
        /// </summary>
        [FieldOffset(sizeof(long) * 2)]
        public long NextAllocationTableBlockId;
    }

    /// <summary>
    /// Individual file record in an allocation table, table is filled with these records
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal struct FileRecord
    {
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
    }
}
