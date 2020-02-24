namespace SimpleStorageTest.TestContext
{
    public class TestObject
    {
        public TestObject(string field1, string field2)
        {
            Field1 = field1;
            Field2 = field2;
        }
        public string Field1 { get; private set; }
        public string Field2 { get; private set; }
    }
}