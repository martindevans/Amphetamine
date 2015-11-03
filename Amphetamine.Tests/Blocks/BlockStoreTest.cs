using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using Amphetamine.Blocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Amphetamine.Tests.Blocks
{
    [TestClass]
    public class BlockStoreTest
    {
        private MemoryMappedFile _file;
        private BlockStore _store;

        [TestInitialize]
        public void TestInitialize()
        {
            _file = MemoryMappedFile.CreateNew(Guid.NewGuid().ToString(), 1000000);
            _store = BlockStore.Create(_file);
        }

        [TestMethod]
        public void AssertThat_BlockStoreCreate_CreatesBlockStore()
        {
            Assert.IsNotNull(_store);
        }

        [TestMethod]
        public void AssertThat_BlockStoreCount_IsCorrect()
        {
            Assert.AreEqual(1000000 / _store.BlockSize, _store.BlockCount);
        }

        [TestMethod]
        public void AssertThat_BlockStoreDispose_ClosesFile()
        {
            _store.Dispose();

            Assert.IsTrue(_file.SafeMemoryMappedFileHandle.IsClosed);
        }

        [TestMethod]
        public void AssertThat_BlockStoreOpen_OpensPreviouslyCreatedBlockStore()
        {
            var c = BlockStore.Open(_file);

            Assert.IsNotNull(c);
            Assert.AreEqual(c.BlockSize, _store.BlockSize);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AssertThat_BlockStoreOpen_Throws_WhenGivenUninitializedFile()
        {
            BlockStore.Open(MemoryMappedFile.CreateNew(Guid.NewGuid().ToString(), 1000000));
        }

        [TestMethod]
        public void AssertThat_BlockStoreRead_ReadsZeroForNullBlock()
        {
            using (var str = _store.Open(24, read: true))
            {
                byte[] bytes = new byte[_store.BlockSize];
                str.Read(bytes, 0, _store.BlockSize);

                foreach (byte byt in bytes)
                    Assert.AreEqual(0, byt);
            }
        }

        [TestMethod]
        public void AssertThat_BlockStoreWrite_WritesBlockData()
        {
            byte[] data = new byte[_store.BlockSize];
            Random r = new Random();
            r.NextBytes(data);

            using (var str = _store.Open(12, write: true))
                str.Write(data, 0, data.Length);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void AssertThat_ReadStream_Throws_WhenWrittenTo()
        {
            using (var str = _store.Open(2, read: true))
                str.WriteByte(1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AssertThat_VeryLargeOffset_Throws_WhenOpeningStream()
        {
            _store.Open(_store.BlockCount + 1, read: true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AssertThat_NegativeOffset_Throws_WhenOpeningStream()
        {
            _store.Open(-1, read: true);
        }

        [TestMethod]
        public unsafe void AssertThat_AcquirePointer_IsWriteable()
        {
            const int VALUE = 23523;

            //Write the value (using a pointer)
            using (var block = _store.Acquire(0))
            {
                var i = (int*)block.Pointer;
                (*i) = VALUE;
            }

            //Read the value (conventionally)
            var b = new BinaryReader(_store.Open(0, read: true));
            Assert.AreEqual(VALUE, b.ReadInt32());
        }

        [TestMethod]
        public unsafe void AssertThat_AcquirePointer_IsReadable()
        {
            const int VALUE = 23523;

            //Write the value (conventionally)
            var b = new BinaryWriter(_store.Open(0, write: true));
            b.Write(VALUE);

            //Read the value (using a pointer)
            using (var block = _store.Acquire(0))
            {
                var i = (int*)block.Pointer;
                Assert.AreEqual(VALUE, *i);
            }
        }
    }
}
