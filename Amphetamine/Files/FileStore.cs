using Amphetamine.Blocks;
using Amphetamine.Extensions;
using System;
using System.Globalization;
using System.IO;

namespace Amphetamine.Files
{
    /// <summary>
    /// An extremely simple file store, stores files in contiguous blocks in a block store
    /// </summary>
    public sealed class FileStore
        : IDisposable
    {
        private readonly BlockStore _blocks;

        private FileStore(BlockStore blocks)
        {
            _blocks = blocks;
        }

        private void OpenStore()
        {
        }

        private void CreateStore()
        {
        }

        private FileHeader GetFileHeader(long id)
        {
            using (var ptr = _blocks.Acquire(id))
            unsafe
            {
                var header = (FileHeader*)ptr.Pointer;
                if (header->Magic != FileHeader.MAGIC)
                    throw new FileNotFoundException("No file header found", id.ToString(CultureInfo.InvariantCulture));

                return *header;
            }
        }

        public Stream Open(long id, bool read = false, bool write = false)
        {
            var h = GetFileHeader(id);
            return _blocks.OpenAtOffset(h.StartOffset, h.Length, read, write);
        }

        public BlockPointer Acquire(long id)
        {
            var h = GetFileHeader(id);
            return _blocks.AcquireAtOffset(h.StartOffset, h.Length);
        }

        public void Create(long id, long size)
        {
            using (var file = _blocks.Acquire(id))
            unsafe
            {
                *((FileHeader*)file.Pointer) = new FileHeader(
                    id,
                    _blocks.Offset(id),
                    size
                );
            }
        }

        public void Delete(long id, bool clear = false)
        {
            using (var file = _blocks.Acquire(id))
            unsafe
            {
                //Clear file header
                *((FileHeader*)file.Pointer) = default(FileHeader);

                //Clear the actual data (if necessary)
                if (clear)
                    new IntPtr(file.Pointer).MemSet(file.Length, 0);
            }
        }

        #region static factories
        public static FileStore Create(BlockStore blockStorage)
        {
            var b = new FileStore(blockStorage);
            b.CreateStore();
            return b;
        }

        public static FileStore Open(BlockStore blockStorage)
        {
            var b = new FileStore(blockStorage);
            b.OpenStore();
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
                _blocks.Dispose();
            }

            _disposed = true;
        }
        #endregion
    }
}
