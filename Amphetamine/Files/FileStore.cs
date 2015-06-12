
using System;
using System.IO;
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

        private AllocationTableHeader[] _headers;

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

        public Stream Read(long id)
        {
            throw new NotImplementedException();
        }

        public Stream Write(long id)
        {
            throw new NotImplementedException();
        }

        public void Create()
        {
            throw new NotImplementedException();
        }

        public void Delete()
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
