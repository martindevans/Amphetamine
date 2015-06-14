using Amphetamine.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Amphetamine.Tests.Extensions
{
    [TestClass]
    public unsafe class IntPtrExtensionsTest
    {
        private readonly byte[] _data = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17 };

        [TestInitialize]
        public void Initialize()
        {
        }

        [TestMethod]
        public  void AssertThat_MemSet_SetsAllMemoryToSpecifiedValue()
        {
            fixed (byte* rawPtr = &_data[0])
            {
                var intPtr = new IntPtr(rawPtr);
                intPtr.MemSet(3, _data.Length);
            }

            foreach (var b in _data)
                Assert.AreEqual(3, b);
        }

        [TestMethod]
        public void AssertThat_MemSet_SetsSpecifiedLengthOfMemoryToSpecifiedValue()
        {
            fixed (byte* rawPtr = &_data[0])
            {
                var intPtr = new IntPtr(rawPtr);
                intPtr.MemSet(3, 5);
            }

            //Check that the specified range has been set
            for (int i = 0; i < 5; i++)
                Assert.AreEqual(3, _data[i]);

            //Check that outside the specified range has *not* been set
            for (int i = 5; i < _data.Length; i++)
                Assert.AreEqual(i + 1, _data[i]);
        }

        [TestMethod]
        public void AssertThat_MemSet_SetsSpecifiedOffsetOfMemoryToSpecifiedValue()
        {
            fixed (byte* rawPtr = &_data[0])
            {
                var intPtr = new IntPtr(rawPtr);
                intPtr.MemSet(3, 5, offset: 3);
            }

            //Check that the specified range has been set
            for (int i = 3; i < 8; i++)
                Assert.AreEqual(3, _data[i]);

            //Check that outside the specified range has *not* been set
            for (int i = 0; i < 3; i++)
                Assert.AreEqual(i + 1, _data[i]);
            for (int i = 8; i < _data.Length; i++)
                Assert.AreEqual(i + 1, _data[i]);
        }
    }
}
