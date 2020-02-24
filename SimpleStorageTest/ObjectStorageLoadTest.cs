using NUnit.Framework;
using SimpleStorageTest.TestContext;

namespace SimpleStorageTest
{
    [TestFixture]
    [Category("LoadTest")]
    class ObjectStorageLoadTest : SimpleStorageLoadTestBase<string, TestObject>
    {
        protected override StorageTestContext<string, TestObject> CreateStorageTestContext()
        {
            return new ObjectStorageTestContext();
        }
    }
}