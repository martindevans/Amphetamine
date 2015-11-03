using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using Amphetamine.Blocks;
using Amphetamine.Buffers;
using Amphetamine.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Amphetamine.Tests.Files
{
    [TestClass]
    public class FileStoreTest
    {
        private MemoryMappedFile _file;
        private BlockStore _blocks;
        private BufferStore _buffers;

        [TestInitialize]
        public void TestInitialize()
        {
            _file = MemoryMappedFile.CreateNew(Guid.NewGuid().ToString(), 1000000);
            _blocks = BlockStore.Create(_file);
            _buffers = BufferStore.Create(_blocks);
        }

        [TestMethod]
        public void AssertThat_FileStoreCreate_CreatesFileStore()
        {
            Assert.IsNotNull(_buffers);
        }

        [TestMethod]
        public void AssertThat_FileStoreOpen_OpensExistingFileStore()
        {
            var store = BufferStore.Open(_blocks);

            Assert.IsNotNull(store);
        }

        [TestMethod]
        public void AssertThat_FileStoreOpenStream_AndWriteToStream_WritesData()
        {
            _buffers.Create(1, 20000);

            using (var f = _buffers.Open(1, false, true))
                f.Write(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 0, 10);

            using (var f = _buffers.Open(1, true, false))
            {
                var b = new byte[10];
                f.Read(b, 0, 10);

                for (int i = 0; i < 9; i++)
                    Assert.AreEqual(i + 1, b[i]);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void AssertThat_OpeningNonFile_Throws()
        {
            _buffers.Open(2);
        }

        [TestMethod]
        public void AssertThat_AccessingDeletedFile_Throws()
        {
            _buffers.Create(1, 20000);

            using (var s = _buffers.Open(1, false, true))
                s.WriteByte(24);

            _buffers.Delete(1, true);

            try
            {
                _buffers.Open(1);
            }
            catch (FileNotFoundException)
            {
                return;
            }

            Assert.Fail("Opening deleted file did not throw");
        }

        [TestMethod]
        public void AssertThat_AcquireThenRead_ReadsCorrectValues()
        {
            var testData = new TestStruct { A = 234234, B = 3462546 };

            //Create a file and write test data into it (using acquire method)
            _buffers.Create(1, 100);
            using (var f = _buffers.Acquire(1))
            {
                f.Pointer.Set(ref testData);
            }

            //Open the file and acquire a pointer to the contents, interpreted as a pointer to testStruct
            using (var f = _buffers.Acquire(1))
            unsafe
            {
                var s2 = (TestStruct*)f.Pointer;

                Assert.AreEqual(testData.A, s2->A);
                Assert.AreEqual(testData.B, s2->B);
            }
        }

        [TestMethod]
        public void AssertThat_AcquireThenWrite_WritesCorrectValues()
        {
            var testData = new TestStruct { A = 234234, B = 3462546 };

            //Create a file and write test data into it (using acquire method)
            _buffers.Create(1, 100);
            using (var f = _buffers.Acquire(1))
                f.Pointer.Set(ref testData);
            
            //Read the test data
            using (var stream = new BinaryReader(_buffers.Open(1, true, false)))
            {
                var a = stream.ReadInt32();
                var b = stream.ReadInt64();

                Assert.AreEqual(testData.A, a);
                Assert.AreEqual(testData.B, b);
            }
        }

        [TestMethod]
        public void AssertThat_DisposingFilestore_DisposesUnderlyingMmap()
        {
            _buffers.Dispose();

            Assert.IsTrue(_file.SafeMemoryMappedFileHandle.IsClosed);
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct TestStruct
        {
            [FieldOffset(0)]
            public int A;

            [FieldOffset(4)]
            public long B;
        }
    }
}
