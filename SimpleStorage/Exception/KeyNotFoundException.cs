namespace SimpleStorage.Exception
{
    public class KeyNotFoundException<T1> : SimpleStorageException
    {
        public KeyNotFoundException(T1 key) : base(
            "Value with specified key was not found")
        {
            Key = key;
        }

        public T1 Key { get; }
    }
}