using System;
using System.IO.MemoryMappedFiles;

namespace Amphetamine.Blocks
{
    public sealed class BlockPointer
        : IDisposable
    {
        public unsafe void* Pointer { get; private set; }

        private readonly MemoryMappedViewAccessor _accessor;

        public BlockPointer(MemoryMappedViewAccessor accessor, long offset)
        {
            unsafe
            {
                byte* ptr = null;
                _accessor = accessor;
                accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);
                Pointer = ptr + offset;
            }
        }

        ~BlockPointer()
        {
            Dispose(false);
        }

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
                _accessor.SafeMemoryMappedViewHandle.ReleasePointer();
            }

            _disposed = true;
        }

        public delegate void DoWithReferenceDelegate<T>(ref T t) where T : struct;
    }
}
