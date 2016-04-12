using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Amphetamine.Blocks;
using Amphetamine.Extensions;

namespace Amphetamine.Buffers
{
    /// <summary>
    /// A store of variable size buffers in a memory mapped file
    /// </summary>
    /// <remarks>Based on the <see cref="BlockStore"></see>. Each buffer is a continuous set of blocks</remarks>
    public sealed class BufferStore
        : IDisposable
    {
        public BlockStore Blocks { get; }

        private BufferStore(BlockStore blocks)
        {
            Blocks = blocks;
        }

        private void OpenStore()
        {
            //Read any necessary initialisation information from the block store
        }

        private void CreateStore()
        {
            //Write any necessary initialisation information from the block store
        }

        private unsafe BufferHeader GetFileHeader(long id)
        {
            using (var ptr = Blocks.Acquire(id))
            {
                var header = (BufferHeader*)ptr.Pointer;
                if (header->Magic != BufferHeader.MAGIC)
                    throw new FileNotFoundException("No file header found", id.ToString(CultureInfo.InvariantCulture));

                return *header;
            }
        }

        public Stream Open(long id, bool read = false, bool write = false)
        {
            unsafe
            {
                var h = GetFileHeader(id);
                return Blocks.OpenAtOffset(h.StartOffset, h.Length, read, write);
            }
        }

        public BlockPointer Acquire(long id)
        {
            unsafe
            {
                var h = GetFileHeader(id);
                var p = Blocks.AcquireAtOffset(h.StartOffset, h.Length);
                return p;
            }
        }

        /// <summary>
        /// Create a new file
        /// </summary>
        /// <param name="id">Unique ID of this file</param>
        /// <param name="size">The size (in bytes) of this file</param>
        public void Create(long id, long size)
        {
            using (var file = Blocks.Acquire(id))
            unsafe
            {
                *((BufferHeader*)file.Pointer) = new BufferHeader(
                    id,
                    Blocks.Offset(id) + Marshal.SizeOf(typeof(BufferHeader)),
                    size
                );
            }
        }

        public void Delete(long id, bool clear = false)
        {
            using (var file = Blocks.Acquire(id))
            unsafe
            {
                //Clear file header
                *(BufferHeader*)Blocks.Acquire(id).Pointer = default(BufferHeader);

                //Clear the actual data (if necessary)
                if (clear)
                    file.Pointer.MemSet(0, file.Length);
            }
        }

        #region static factories
        public static BufferStore Create(BlockStore blockStorage)
        {
            var b = new BufferStore(blockStorage);
            b.CreateStore();
            b.OpenStore();
            return b;
        }

        public static BufferStore Open(BlockStore blockStorage)
        {
            var b = new BufferStore(blockStorage);
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
                Blocks.Dispose();
            }

            _disposed = true;
        }
        #endregion
    }
}
