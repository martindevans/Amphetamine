using System;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using Amphetamine.Blocks;
using Amphetamine.Extensions;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Sample
{
    public class Program
    {
        private readonly BlockStore _store;

        private static void Main(string[] args)
        {
            Console.WriteLine(BenchmarkRunner.Run<Program>());
        }

        public Program()
        {
            _store = BlockStore.Create(MemoryMappedFile.CreateNew(Guid.NewGuid().ToString(), 10000));
        }

        [Benchmark]
        public void Pointers()
        {
            for (var i = 0; i < (int)_store.BlockCount; i++)
            {
                var s = new TestStruct
                {
                    A = i,
                    B = -i,
                    C = i * 1.4f,
                    D = i / 214534574542.2
                };

                using (var a = _store.Acquire(i))
                {
                    unsafe
                    {
                        *((TestStruct*)a.Pointer.ToPointer()) = s;
                    }
                }
            }
        }

        [Benchmark]
        public void Stream()
        {
            for (var i = 0; i < (int)_store.BlockCount; i++)
            {
                var s = new TestStruct {
                    A = i,
                    B = -i,
                    C = i * 1.4f,
                    D = i / 214534574542.2
                };

                using (var stream = new BinaryWriter(_store.Open(i)))
                {
                    stream.Write(s.A);
                    stream.Write(s.B);
                    stream.Write(s.C);
                    stream.Write(s.D);
                }
            }
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
