using System;
using System.IO.Abstractions;
using System.Runtime.InteropServices;
using Amphetamine.Buffers;
using Earmark;

namespace Amphetamine.Filesystem
{
    public class MemoryMappedFileSystem
        : IFileSystem
    {
        private readonly IAllocator _allocator;
        private readonly BufferStore _buffers;

        private MemoryMappedFileSystem(BufferStore buffers)
        {
            _buffers = buffers;

            _allocator = new SlabAllocator(buffers.Blocks.BlockSize, (int)buffers.Blocks.BlockCount);

            File = new File(this);
            Directory = new Directory(this);
            Path = new PathWrapper();
        }

        private void OpenStore()
        {
            using (var pointer = _buffers.Acquire(0))
            unsafe
            {
                //Get pointer to header
                var header = ((SystemHeader*)pointer.Pointer);

                //Check that the magic is correct
                if (header->Magic != SystemHeader.MAGIC)
                    throw new ArgumentException("Incorrect filesystem header magic");

                //Initialise the allocator with the map of already allocated blocks
                //todo: Initialise allocator
            }
        }

        private void CreateStore(string name)
        {
            var headerSize = Marshal.SizeOf(typeof(SystemHeader));
            _buffers.Create(0, headerSize);

            //todo: set the header buffer space to allocated

            unsafe
            {
                //Place header into buffer 0
                using (var pointer = _buffers.Acquire(0))
                {
                    *((SystemHeader*)pointer.Pointer) = new SystemHeader(name);

                    //todo: store allocation data in the header
                }
            }
        }

        #region static factories
        public static MemoryMappedFileSystem Create(BufferStore bufferStorage, string driveName)
        {
            var b = new MemoryMappedFileSystem(bufferStorage);
            b.CreateStore(driveName);
            b.OpenStore();
            return b;
        }

        public static MemoryMappedFileSystem Open(BufferStore bufferStorage)
        {
            var b = new MemoryMappedFileSystem(bufferStorage);
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
                _buffers.Dispose();
            }

            _disposed = true;
        }
        #endregion

        #region IFileSystem
        public FileBase File { get; }

        public DirectoryBase Directory { get; }

        public IFileInfoFactory FileInfo { get { return (IFileInfoFactory)File; } }

        public PathBase Path { get; }

        public IDirectoryInfoFactory DirectoryInfo { get { return (IDirectoryInfoFactory)Directory; } }
        #endregion
    }
}
