using System;
using System.IO.MemoryMappedFiles;

namespace Amphetamine.Blocks
{
    public sealed class BlockPointer
        : IDisposable
    {
        public IntPtr Pointer { get; private set; }

        private readonly bool _ownAccessor;
        private readonly MemoryMappedViewAccessor _accessor;

        public long Length { get; private set; }

        public BlockPointer(MemoryMappedViewAccessor accessor, bool ownAccessor)
        {
            _ownAccessor = ownAccessor;

            unsafe
            {
                Length = accessor.Capacity;

                byte* ptr = null;
                _accessor = accessor;
                accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);
                Pointer = new IntPtr(ptr + accessor.PointerOffset);
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
                if (_ownAccessor)
                    _accessor.Dispose();
            }

            _disposed = true;
        }

        public delegate void DoWithReferenceDelegate<T>(ref T t) where T : struct;
    }
}
