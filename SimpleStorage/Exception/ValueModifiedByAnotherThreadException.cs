namespace SimpleStorage.Exception
{
    public class ValueModifiedByAnotherThreadException<T1> : SimpleStorageException
    {
        public ValueModifiedByAnotherThreadException(T1 key) : base(
            "Value was modified by another thread and can't be updated")
        {
            Key = key;
        }

        public T1 Key { get; }
    }
}