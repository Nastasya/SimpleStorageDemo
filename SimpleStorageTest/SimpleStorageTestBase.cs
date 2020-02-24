using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SimpleStorage;
using SimpleStorage.Exception;
using SimpleStorageTest.TestContext;

namespace SimpleStorageTest
{
    [TestFixture]
    [Category("UnitTest")]
    abstract class SimpleStorageTestBase<T1,T2>
    {
        #region Add
        [Test]
        public void Add_ValueWithUniqueKey_ShouldSuccess()
        {
            //arrange
            var storageTestContext = CreateStorageTestContext();
            var storage = storageTestContext.CreateStorage();
            var testDataToAdd = storageTestContext.GenerateTestDataForAdd(1).First();

            //act
            storage.Add(testDataToAdd.Key, testDataToAdd.Value);

            //assert
            var addedValue = storage.GetValue(testDataToAdd.Key);
            Assert.That(addedValue, Is.EqualTo(testDataToAdd.Value));
        }

        [Test]
        public void Add_NullValueWithUniqueKey_ShouldSuccess()
        {
            //arrange
            var storageTestContext = CreateStorageTestContext();
            var storage = storageTestContext.CreateStorage();
            var testDataToAdd = storageTestContext.GetTestDataWithNullValue();

            //act
            storage.Add(testDataToAdd.Key, testDataToAdd.Value);

            //assert
            var addedValue = storage.GetValue(testDataToAdd.Key);
            Assert.That(addedValue, Is.Null);
        }


        [Test]
        public void Add_ValueWithNonUniqueKey_ShouldFail()
        {
            //arrange
            var storageTestContext = CreateStorageTestContext();
            var storage = storageTestContext.CreateStorage();
            var testDataToAdd = storageTestContext.GenerateTestDataForAdd(1).First();

            //act
            storage.Add(testDataToAdd.Key, testDataToAdd.Value);

            //assert
            Assert.Throws<DuplicatedKeyValueException<T1>>(() => storage.Add(testDataToAdd.Key, testDataToAdd.Value));
        }

        [Test]
        public void Add_ValueWithNullKey_ShouldFail()
        {
            //arrange
            var storageTestContext = CreateStorageTestContext();
            var storage = storageTestContext.CreateStorage();
            var testDataToAdd = storageTestContext.GetTestDataWithNullKey();

            //act

            //assert
            Assert.Throws<ArgumentNullException>(() => storage.Add(testDataToAdd.Key, testDataToAdd.Value));

        }

        #endregion

        #region Get

        [Test]
        public void Get_ValueByNotExistingKey_ShouldFail()
        {
            //arrange
            var storageTestContext = CreateStorageTestContext();
            var storage = storageTestContext.CreateStorage();
            var notExistingKey = storageTestContext.GetNotExistingKey();
            
            //act

            //assert
            Assert.Throws<KeyNotFoundException<T1>>(() => storage.GetValue(notExistingKey));
        }

        [Test]
        public void Get_ValueByExistingKey_ShouldReturnValue()
        {
            //arrange
            var storageTestContext = CreateStorageTestContext();
            var initialStorageData = storageTestContext.GenerateTestDataSequentially(3);
            var storage = storageTestContext.CreateStorage(initialStorageData);
            var expectedStorageValues = initialStorageData;

            //act
            Dictionary<T1, T2> actualStorageValues = GetStorageValues(storage, initialStorageData.Keys);
            
            //assert
            CollectionAssert.AreEquivalent(expectedStorageValues, actualStorageValues);
        }

        #endregion

        #region Delete

        [Test]
        public void Delete_ValueByNotExistingKey_ShouldFail()
        {
            //arrange
            var storageTestContext = CreateStorageTestContext();
            var initialStorageData = storageTestContext.GenerateTestDataSequentially(3);
            var storage = storageTestContext.CreateStorage(initialStorageData);
            var notExistingKey = storageTestContext.GetNotExistingKey();
            
            //act

            //assert
            Assert.Throws<KeyNotFoundException<T1>>(() => storage.DeleteValue(notExistingKey));
        }

        [Test]
        public void Delete_ValueByExistingKey_ShouldSuccess()
        {
            //arrange
            var storageTestContext = CreateStorageTestContext();
            var initialStorageData = storageTestContext.GenerateTestDataSequentially(3);
            var storage = storageTestContext.CreateStorage(initialStorageData);
            var expectedStorageValuesAfterDeletion = initialStorageData.ToDictionary(k => k.Key, v => default(T2));
            //act
            foreach (var storageElement in initialStorageData)
            {
                storage.DeleteValue(storageElement.Key);
            }

            //assert
            Dictionary<T1, T2> actualStorageValuesAfterDeletion = GetStorageValues(storage, initialStorageData.Keys);
            CollectionAssert.AreEquivalent(expectedStorageValuesAfterDeletion, actualStorageValuesAfterDeletion);
        }

        #endregion

        #region Update

        [Test]
        public void Update_ValueByNotExistingKey_ShouldFail()
        {
            //arrange
            var storageTestContext = CreateStorageTestContext();
            var initialStorageData = storageTestContext.GenerateTestDataSequentially(3);
            var storage = storageTestContext.CreateStorage(initialStorageData);
            var notExistingKey = storageTestContext.GetNotExistingKey();
            var valueToUpdate = storageTestContext.GenerateTestDataForUpdate(1).First();
            //act

            //assert
            Assert.Throws<KeyNotFoundException<T1>>(() =>
                storage.UpdateValue(notExistingKey, initialStorageData[valueToUpdate.Key], valueToUpdate.Value));
        }

        [Test]
        public void Update_ValueByExistingKey_ShouldSuccess()
        {
            //arrange
            var storageTestContext = CreateStorageTestContext();
            var initialStorageData = storageTestContext.GenerateTestDataSequentially(3);
            var storage = storageTestContext.CreateStorage(initialStorageData);

            var itemsToUpdate = storageTestContext.GenerateTestDataForUpdate(3).ToDictionary(k => k.Key, v => v.Value);
            Dictionary<T1, T2> expectedValues = itemsToUpdate;

            //act
            foreach (var itemToUpdate in itemsToUpdate)
            {
                storage.UpdateValue(itemToUpdate.Key, initialStorageData[itemToUpdate.Key], itemToUpdate.Value);
            }

            //assert
            var storageValuesAfterUpdate = GetStorageValues(storage, initialStorageData.Keys);
            CollectionAssert.AreEquivalent(expectedValues, storageValuesAfterUpdate);

        }

        private static Dictionary<T1, T2> GetStorageValues(ISimpleStorage<T1, T2> storage, Dictionary<T1, T2>.KeyCollection keys)
        {
            Dictionary<T1, T2> resultValues = new Dictionary<T1, T2>();
            foreach (var storageElementKey in keys)
            {
                var getValueResult = storage.GetValue(storageElementKey);
                resultValues.Add(storageElementKey, getValueResult);
            }

            return resultValues;
        }

        #endregion

        #region GetAllKeysThatHaveValues
        [Test]
        public void Get_KeysThatHaveValues_ShouldReturnValue()
        {
            //arrange
            var storageTestContext = CreateStorageTestContext();
            var initialStorageData = storageTestContext.GenerateTestDataSequentially(3);
            var storage = storageTestContext.CreateStorage(initialStorageData);
            var itemToDeleteKey = initialStorageData.First().Key;
            initialStorageData.Remove(itemToDeleteKey);
            var expectedKeysList = initialStorageData.Keys;

            //act
            storage.DeleteValue(itemToDeleteKey);
            var actualKeysList = storage.GetAllKeysThatHaveValues();

            //assert
            CollectionAssert.AreEquivalent(expectedKeysList, actualKeysList);
        }

        #endregion

        protected abstract StorageTestContext<T1,T2> CreateStorageTestContext();
    }
}
