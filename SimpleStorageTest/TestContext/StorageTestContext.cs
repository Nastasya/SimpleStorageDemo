using System.Collections.Generic;
using System.Diagnostics;
using SimpleStorage;

namespace SimpleStorageTest.TestContext
{
    public abstract class StorageTestContext<T1, T2>
    {
        public abstract ISimpleStorage<T1, T2> CreateStorage(Dictionary<T1, T2> values = null);

        public abstract IEnumerable<TestDataItem<T1, T2>> GenerateTestDataForAdd(int numberOfItems, int batchNumber = 1);

        public abstract IEnumerable<TestDataItem<T1, T2>> GenerateTestDataForUpdate(int numberOfItems,
            int batchNumber = 1);

        public abstract TestDataItem<T1, T2> GetTestDataWithNullKey();

        public abstract TestDataItem<T1, T2> GetTestDataWithNullValue();

        public abstract T1 GetNotExistingKey();
        public abstract Dictionary<T1, T2> GenerateTestDataSequentially(int numberOfItems);
    }
}