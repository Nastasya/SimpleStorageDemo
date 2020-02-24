using NUnit.Framework;
using SimpleStorageTest.TestContext;

namespace SimpleStorageTest
{
    [TestFixture]
    [Category("ConcurrencyTest")]
    class ObjectStorageConcurrencyTest : SimpleStorageConcurrentTestBase<string, TestObject>
    {
        protected override StorageTestContext<string, TestObject> CreateStorageTestContext()
        {
            return new ObjectStorageTestContext();
        }
    }
}