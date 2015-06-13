using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;

namespace Amphetamine.Blocks
{
    /// <summary>
    /// An extremely simple block store, simply stores fixed size blocks of data in a memory mapped file
    /// </summary>
    public sealed class BlockStore
        : IDisposable
    {
        private readonly MemoryMappedFile _file;

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

        private long Offset(long id)
        {
            if (id < 0)
                throw new ArgumentOutOfRangeException("id");

            return id * _header.BlockSize + _rootOffset;
        }

        private Stream GetStream(long id, long length, MemoryMappedFileAccess access)
        {
            return _file.CreateViewStream(Offset(id), length, access);
        }

        public Stream Open(long id, bool read = false, bool write = false)
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

            return GetStream(id, _header.BlockSize, access);
        }

        private BlockPointer AcquireAtOffset(long offset, long size)
        {
            var accessor = _file.CreateViewAccessor(offset, size);
            return new BlockPointer(accessor, offset, size);
        }

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
        /// <returns></returns>
        public static BlockStore Create(MemoryMappedFile file)
        {
            var b = new BlockStore(file);
            b.WriteStoreHeader();
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
