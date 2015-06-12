using System;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;

namespace Amphetamine.Blocks
{
    internal class StoreHeader
    {
        public const int SIZE =
            4                   // Magic
            + sizeof(uint);     // BlockSize

        private static readonly byte[] _magic = { 65, 109, 112, 104 };

        public readonly int BlockSize;

        public StoreHeader(int blockSize)
        {
            BlockSize = blockSize;
        }

        public void Write(Stream stream)
        {
#if DEBUG
            var start = stream.Position;
#endif

            using (BinaryWriter w = new BinaryWriter(stream))
            {
                w.Write(_magic);

                w.Write(BlockSize);

#if DEBUG
                Debug.Assert(start + SIZE == stream.Position);
#endif
            }
        }

        public static StoreHeader Read(MemoryMappedViewStream stream)
        {
            using (BinaryReader r = new BinaryReader(stream))
            {
                if (_magic.Any(b => r.ReadByte() != b))
                    throw new ArgumentException("stream");

                return new StoreHeader(
                    blockSize: r.ReadInt32()
                );
            }
        }
    }
}
