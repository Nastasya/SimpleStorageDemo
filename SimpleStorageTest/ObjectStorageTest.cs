using System;
using NUnit.Framework;
using SimpleStorageTest.TestContext;

namespace SimpleStorageTest
{
    [TestFixture]
    [Category("UnitTest")]
    class ObjectStorageTest: SimpleStorageTestBase<string,TestObject>
    {
        protected override StorageTestContext<string,TestObject> CreateStorageTestContext()
        {
            return new ObjectStorageTestContext();
        }
    }
}