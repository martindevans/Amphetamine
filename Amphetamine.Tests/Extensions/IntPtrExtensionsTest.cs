using Amphetamine.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Runtime.InteropServices;

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
            for (var i = 0; i < 5; i++)
                Assert.AreEqual(3, _data[i]);

            //Check that outside the specified range has *not* been set
            for (var i = 5; i < _data.Length; i++)
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
            for (var i = 0; i < 3; i++)
                Assert.AreEqual(i + 1, _data[i]);
            for (var i = 8; i < _data.Length; i++)
                Assert.AreEqual(i + 1, _data[i]);
        }

        [TestMethod]
        public void AssertThat_PointerSet_SetsValueIntoPointer()
        {
            //Test data
            TestStruct data = new TestStruct { A = 123424, B = 6463 };

            //Write data into byte array by setting pointer
            byte[] output = new byte[12];
            fixed (byte* a = &output[0])
                new IntPtr(a).Set(ref data);

            //Check that the values in the array are as we expect
            Assert.AreEqual(data.A, BitConverter.ToInt32(output, 0));
            Assert.AreEqual(data.B, BitConverter.ToInt32(output, 4));
        }

        [TestMethod]
        public void AssertThat_PointerAs_GetsValueInPointer()
        {
            //Test data
            TestStruct data = new TestStruct { A = 123424, B = 6463 };

            //Write data into byte array by setting pointer
            byte[] output = new byte[12];
            fixed (byte* a = &output[0])
                new IntPtr(a).Set(ref data);

            TestStruct o;
            fixed (byte* a = &output[0])
                new IntPtr(a).As(out o);

            //Check that the values we read are as expected
            Assert.AreEqual(data.A, o.A);
            Assert.AreEqual(data.B, o.B);
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
