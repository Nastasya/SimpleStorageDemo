using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SimpleStorage.Exception;

namespace SimpleStorage
{
    public sealed class ObjectStorage<T1, T2> : IDisposable, ISimpleStorage<T1, T2> where T2: class
    {
        private readonly ReaderWriterLockSlim locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        private readonly Dictionary<T1, T2> keyValueMap;
        private readonly HashSet<T1> activeValuesSet;

        public ObjectStorage() : this(new Dictionary<T1, T2>())
        {
        }

        public ObjectStorage(Dictionary<T1, T2> data)
        {
            keyValueMap = data;
            activeValuesSet = new HashSet<T1>(data.Where(kvp => kvp.Value != null).Select(k => k.Key));
        }

        public void Add(T1 key, T2 value)
        {
            locker.EnterWriteLock();
            try
            {
                AddValueUnsafe(key, value);
            }
            finally
            {
                locker.ExitWriteLock();
            }
        }

        public T2 GetValue(T1 key)
        {
            locker.EnterReadLock();
            try
            {
                return GeValueUnsafe(key);
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        public void UpdateValue(T1 key, T2 oldValue, T2 newValue)
        {
            locker.EnterUpgradeableReadLock();
            try
            {
                if (!keyValueMap.TryGetValue(key, out var currentValue))
                {
                    throw new KeyNotFoundException<T1>(key);
                }

                if (!Equals(oldValue, currentValue))
                {
                    throw new ValueModifiedByAnotherThreadException<T1>(key);
                }

                if (currentValue != newValue)
                {
                    locker.EnterWriteLock();
                    try
                    {
                        UpdateValueUnsafe(key, newValue);
                    }
                    finally
                    {
                        locker.ExitWriteLock();
                    }
                }
            }
            finally
            {
                locker.ExitUpgradeableReadLock();
            }
        }

        public void DeleteValue(T1 key)
        {
            locker.EnterUpgradeableReadLock();
            try
            {
                if (!keyValueMap.TryGetValue(key, out var currentValue))
                {
                    throw new KeyNotFoundException<T1>(key);
                }

                if (currentValue != null)
                {
                    locker.EnterWriteLock();
                    try
                    {
                        RemoveValueUnsafe(key);
                    }
                    finally
                    {
                        locker.ExitWriteLock();
                    }
                }
            }
            finally
            {
                locker.ExitUpgradeableReadLock();
            }
        }

        public ICollection<T1> GetAllKeysThatHaveValues()
        {
            locker.EnterReadLock();
            try
            {
                return activeValuesSet;
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        public void Clear()
        {
            locker.EnterWriteLock();
            try
            {
               keyValueMap.Clear();
               activeValuesSet.Clear();
            }
            finally { locker.ExitWriteLock(); }
        }

        public int Count()
        {
            locker.EnterReadLock();
            try
            {
                return keyValueMap.Count;
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        public void Dispose()
        {
            locker?.Dispose();
        }

        private void AddValueUnsafe(T1 key, T2 value)
        {
            try
            {
                keyValueMap.Add(key, value);
            }
            catch (ArgumentNullException)
            {
                throw;
            }
            catch (ArgumentException)
            {
                throw new DuplicatedKeyValueException<T1>(key);
            }

            activeValuesSet.Add(key);
        }
        private T2 GeValueUnsafe(T1 key)
        {
            if (!keyValueMap.TryGetValue(key, out var value))
            {
                throw new KeyNotFoundException<T1>(key);
            }

            return value;
        }

        private void UpdateValueUnsafe(T1 key, T2 newValue)
        {
            keyValueMap[key] = newValue;
            if (newValue == null)
            {
                activeValuesSet.Remove(key);
            }
            else
            {
                activeValuesSet.Add(key);
            }
        }

        private void RemoveValueUnsafe(T1 key)
        {
            keyValueMap[key] = null;
            activeValuesSet.Remove(key);
        }

    }
}