using System.Collections.Generic;
using SimpleStorage.Exception;

namespace SimpleStorage
{
    public interface ISimpleStorage<T1, T2>
    {
        /// <summary>
        /// Add value with specified key to storage
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <exception cref="DuplicatedKeyValueException{K}">when value with specified key already exists</exception>
        void Add(T1 key, T2 value);

        /// <summary>
        /// Get value with specified key from storage
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException{K}">when specified key doesn't exist</exception>
        T2 GetValue(T1 key);

        /// <summary>
        /// Update value with specified key in storage
        /// </summary>
        /// <param name="key"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <exception cref="KeyNotFoundException{K}">when specified key doesn't exist</exception>
        /// <exception cref="ValueModifiedByAnotherThreadException{K}">when value with specified key was modified by another thread</exception>
        void UpdateValue(T1 key, T2 oldValue, T2  newValue);

        /// <summary>
        /// Remove value for specified key (mark item as deleted)
        /// </summary>
        /// <exception cref="KeyNotFoundException{K}">when specified key doesn't exist</exception>
        /// <param name="key"></param>
        void DeleteValue(T1 key);

        /// <summary>
        /// Get all keys that have values
        /// </summary>
        /// <returns>collection of keys</returns>
        ICollection<T1> GetAllKeysThatHaveValues();

        /// <summary>
        /// Clear storage
        /// </summary>
        void Clear();

        /// <summary>
        /// Get elements count in storage
        /// </summary>
        /// <returns></returns>
        int Count();
    }
}