using System;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using Amphetamine.Blocks;
using Amphetamine.Extensions;

namespace Sample
{
    class Program
    {
        private static void Main(string[] args)
        {
            const int size = 10000000;
            var f = MemoryMappedFile.CreateNew(Guid.NewGuid().ToString(), 1000000);

            var store = BlockStore.Create(f);

            Stopwatch w = new Stopwatch();
            w.Start();
            int count = 1000;
            for (int j = 0; j < count; j++)
            {
                for (int i = 0; i < (int)store.BlockCount; i++)
                {
                    var s = new TestStruct {
                        A = i,
                        B = -i,
                        C = i * 1.4f,
                        D = i / 214534574542.2
                    };

                    //using (var stream = new StreamWriter(store.Open(i, false, true)))
                    //{
                    //    stream.Write(s.A);
                    //    stream.Write(s.B);
                    //    stream.Write(s.C);
                    //    stream.Write(s.D);
                    //}
                    using (var a = store.Acquire(i))
                    unsafe
                    {
                        *((TestStruct*)a.Pointer.ToPointer()) = s;
                    }
                }
            }

            long total = count * store.BlockCount;
            w.Stop();
            Console.WriteLine("Total: {0}ms", w.Elapsed.TotalMilliseconds);
            Console.WriteLine("Per Struct: {0}ms", w.Elapsed.TotalMilliseconds / total);

            Console.ReadLine();
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct TestStruct
        {
            [FieldOffset(0)]
            public int A;

            [FieldOffset(4)]
            public long B;

            [FieldOffset(12)]
            public float C;

            [FieldOffset(16)]
            public double D;
        }
    }
}
