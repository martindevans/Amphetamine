using System;
using System.IO;
using System.IO.MemoryMappedFiles;

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

        private BlockStore(MemoryMappedFile file)
        {
            _file = file;
        }

        private void Open()
        {
            using (var stream = _file.CreateViewStream(0, StoreHeader.SIZE))
                _header = StoreHeader.Read(stream);
        }

        private void Create(int pageSize = 16384)
        {
            StoreHeader h = new StoreHeader(
                blockSize: pageSize
            );

            using (var stream = _file.CreateViewStream(0, StoreHeader.SIZE))
                h.Write(stream);
        }

        private Stream GetStream(long id, MemoryMappedFileAccess access)
        {
            if (id < 0)
                throw new ArgumentOutOfRangeException("id");

            var offset = id * _header.BlockSize + StoreHeader.SIZE;
            return _file.CreateViewStream(offset, _header.BlockSize, access);
        }

        public Stream Read(long id)
        {
            return GetStream(id, MemoryMappedFileAccess.Read);
        }

        public Stream Write(long id)
        {
            return GetStream(id, MemoryMappedFileAccess.Write);
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
            b.Open();
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
            b.Create();
            b.Open();
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
                    _file.Dispose();
                }

                //Dispose managed resources
                //(none)
            }

            _disposed = true;
        }
        #endregion
    }
}
