using System;
using System.Collections.Generic;
using System.Linq;
using SimpleStorage;

namespace SimpleStorageTest.TestContext
{
    public class ObjectStorageTestContext : StorageTestContext<string,TestObject>
    {
        public override ISimpleStorage<string,TestObject> CreateStorage(Dictionary<string, TestObject> data)
        {
            if (data != null)
            {
                return new ObjectStorage<string, TestObject>(data.ToDictionary(k => k.Key, v => v.Value));
            }
            return new ObjectStorage<string, TestObject>();
        }


        public override IEnumerable<TestDataItem<string, TestObject>> GenerateTestDataForAdd(int numberOfItems,
            int batchNumber = 1)
        {
            var batchStart = numberOfItems*(batchNumber-1);
            var batchEnd = batchStart + numberOfItems;
            for (int i = batchStart; i < batchEnd; i++)
            {
                var key = string.Concat("key", i + 1);
                yield return new TestDataItem<string, TestObject>(key, new TestObject(key + "Data1", key + "Data2"));
            }
        }

        public override IEnumerable<TestDataItem<string, TestObject>> GenerateTestDataForUpdate(int numberOfItems,
            int batchNumber = 1)
        {
            var batchStart = numberOfItems * (batchNumber - 1);
            var batchEnd = batchStart + numberOfItems;
            for (int i = batchStart; i < batchEnd; i++)
            {
                var key = string.Concat("key", i + 1);
                yield return new TestDataItem<string, TestObject>(key, new TestObject(key + "Data1Updated", key + "Data2Updated"));
            }
        }

        public override TestDataItem<string, TestObject> GetTestDataWithNullValue()
        {
            return new TestDataItem<string, TestObject>("key1", null);
        }

        public override string GetNotExistingKey()
        {
            return "notExistingKey";
        }

        public override Dictionary<string, TestObject> GenerateTestDataSequentially(int numberOfItems)
        {
            Dictionary<string, TestObject> initialStorageData = new Dictionary<string, TestObject>();
            for (int i = 0; i < numberOfItems; i++)
            {
                var key = string.Concat("key", i + 1);
                initialStorageData.Add(key, new TestObject(key + "Data1", key + "Data2"));
            }

            return initialStorageData;
        }

        public override TestDataItem<string, TestObject> GetTestDataWithNullKey()
        {
            return new TestDataItem<string, TestObject>(null, new TestObject(string.Empty, String.Empty));
        }
    }
}