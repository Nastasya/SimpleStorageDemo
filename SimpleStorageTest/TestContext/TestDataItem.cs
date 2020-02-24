namespace SimpleStorageTest.TestContext
{
    public class TestDataItem<T1,T2>
    {
        public TestDataItem(T1 key, T2 value)
        {
            Key = key;
            Value = value;
        }

        public T1 Key { get; private set; }
        public T2 Value { get; private set; }
    }
}