using System;
using System.IO.MemoryMappedFiles;
using Amphetamine.Blocks;
using Amphetamine.Files;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Amphetamine.Tests.Files
{
    [TestClass]
    public class FileStoreTest
    {
        private MemoryMappedFile _file;
        private BlockStore _blocks;
        private FileStore _files;

        [TestInitialize]
        public void TestInitialize()
        {
            _file = MemoryMappedFile.CreateNew(Guid.NewGuid().ToString(), 1000000);
            _blocks = BlockStore.Create(_file);
            _files = FileStore.Create(_blocks);
        }

        [TestMethod]
        public void AssertThat_FileStoreCreate_CreatesFileStore()
        {
            Assert.IsNotNull(_files);
        }
    }
}
