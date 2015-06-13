
using System;
using System.Globalization;
using System.IO;
using System.IO.MemoryMappedFiles;
using Amphetamine.Blocks;

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

        private Stream GetStream(long id, MemoryMappedFileAccess access)
        {
            long length, start;

            using (var ptr = _blocks.Acquire(id))
            unsafe
            {
                var header = (FileHeader*)ptr.Pointer;
                if (header->Magic != FileHeader.MAGIC)
                    throw new FileNotFoundException("No file header found", id.ToString(CultureInfo.InvariantCulture));

                length = header->Length;
                start = header->StartOffset;
            }

            //return _blocks.GetStreamAtOffset(start, length, access);
            throw new NotImplementedException();
        }

        public Stream Read(long id)
        {
            return GetStream(id, MemoryMappedFileAccess.Read);
        }

        public Stream Write(long id)
        {
            return GetStream(id, MemoryMappedFileAccess.Read);
        }

        public BlockPointer Acquire(long id)
        {
            throw new NotImplementedException();
        }

        public void Create(long id, long size)
        {
            throw new NotImplementedException();
        }

        public void Delete(long id)
        {
            throw new NotImplementedException();
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
