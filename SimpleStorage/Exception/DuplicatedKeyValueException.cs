namespace SimpleStorage.Exception
{
    public class DuplicatedKeyValueException<T1> : SimpleStorageException
    {
        public DuplicatedKeyValueException(T1 key):base("Key already exists in storage")
        {
            Key = key;
        }

        public T1 Key { get; }
    }
}