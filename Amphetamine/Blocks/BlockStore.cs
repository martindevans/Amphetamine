using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;

namespace Amphetamine.Blocks
{
    /// <summary>
    /// A store of fixed size pages in a memory mapped file
    /// </summary>
    public sealed class BlockStore
        : IDisposable
    {
        private readonly MemoryMappedFile _file;

        private readonly long _length;

        public long BlockCount
        {
            get { return _length / BlockSize; }
        }

        private StoreHeader _header;
        public int BlockSize
        {
            get
            {
                return _header.BlockSize;
            }
        }

        private static readonly long _rootOffset = Marshal.SizeOf(typeof(StoreHeader));

        private BlockStore(MemoryMappedFile file)
        {
            _file = file;

            using (var view = file.CreateViewAccessor())
                _length = view.Capacity;
        }

        #region store header
        private void ReadStoreHeader()
        {
            //Copy header out of memory map
            using (var pointer = AcquireAtOffset(0, _rootOffset))
            unsafe
            {
                _header = *((StoreHeader*)pointer.Pointer);
            }

            //Check that the magic is correct
            if (_header.Magic != StoreHeader.MAGIC)
                throw new ArgumentException("Incorrect blockstore header magic");
        }

        private void WriteStoreHeader(int pageSize = 16384)
        {
            using (var a = _file.CreateViewAccessor())
            {
                StoreHeader h = new StoreHeader(
                    blockSize: pageSize,
                    totalSize: a.Capacity
                );

                //Write store header
                using (var pointer = AcquireAtOffset(0, _rootOffset))
                unsafe
                {
                    *((StoreHeader*)pointer.Pointer) = h;
                }
            }
        }
        #endregion

        public long Offset(long id)
        {
            if (id < 0)
                throw new ArgumentOutOfRangeException(nameof(id));

            var off = id * _header.BlockSize + _rootOffset;
            if (off > _length)
                throw new ArgumentOutOfRangeException(nameof(id), $"Cannot calculate offset for block {id}, it is out of range");

            return off;
        }

        /// <summary>
        /// Open a stream at the given offset, with the given length and access type (no respect for block boundaries)
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="read"></param>
        /// <param name="write"></param>
        /// <returns></returns>
        public Stream OpenAtOffset(long offset, long length, bool read, bool write)
        {
            MemoryMappedFileAccess access;
            if (read && write)
                access = MemoryMappedFileAccess.ReadWrite;
            else if (read)
                access = MemoryMappedFileAccess.Read;
            else if (write)
                access = MemoryMappedFileAccess.Write;
            else
                throw new InvalidOperationException("Must specify read/write when Opening a block");

            return _file.CreateViewStream(offset, length, access);
        }

        /// <summary>
        /// Open a stream to the given block
        /// </summary>
        /// <param name="id"></param>
        /// <param name="read"></param>
        /// <param name="write"></param>
        /// <returns></returns>
        public Stream Open(long id, bool read = false, bool write = false)
        {
            return OpenAtOffset(Offset(id), _header.BlockSize, read, write);
        }

        /// <summary>
        /// Acquire a pointer at the given offset, with the given length (no respect for block boundaries)
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public BlockPointer AcquireAtOffset(long offset, long size)
        {
            var accessor = _file.CreateViewAccessor(offset, size);
            return new BlockPointer(accessor, offset, size);
        }

        /// <summary>
        /// Acquire a point to the block with the given ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BlockPointer Acquire(long id)
        {
            return AcquireAtOffset(Offset(id), _header.BlockSize);
        }

        #region static factories
        /// <summary>
        /// Open an existing block store
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static BlockStore Open(MemoryMappedFile file)
        {
            var b = new BlockStore(file);
            b.ReadStoreHeader();
            return b;
        }

        /// <summary>
        /// Create a new block store in the specified file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static BlockStore Create(MemoryMappedFile file, int pageSize = 16384)
        {
            var b = new BlockStore(file);
            b.WriteStoreHeader(pageSize);
            b.ReadStoreHeader();
            return b;
        }
        #endregion

        #region disposal
        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    //Dispose unmanaged resources
                    //(none)
                }

                //Dispose managed resources
                _file.Dispose();
            }

            _disposed = true;
        }
        #endregion
    }
}
