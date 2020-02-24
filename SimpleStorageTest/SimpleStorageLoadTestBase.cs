using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using SimpleStorageTest.TestContext;

namespace SimpleStorageTest
{
    [TestFixture]
    [Category("LoadTest")]
    abstract class SimpleStorageLoadTestBase<T1,T2>
    {
        [Test]
        public void Add_Value_Should_Work_At_Rate_More_Than_300000_Values_Per_Second()
        {
            //arrange
            var storageTestContext = CreateStorageTestContext();
            var storage = storageTestContext.CreateStorage();


            //act
            Stopwatch w = new Stopwatch();
            w.Start();
            var numberOfItems = 1000000;

            foreach (var testDataToAdd in storageTestContext.GenerateTestDataForAdd(numberOfItems))
            {
                storage.Add(testDataToAdd.Key, testDataToAdd.Value);
            }

            w.Stop();


            //assert
            var elementsNumber = storage.Count();
            Assert.That(elementsNumber, Is.EqualTo(numberOfItems));
            Assert.That((numberOfItems / w.Elapsed.TotalMilliseconds) * 1000, Is.GreaterThan(300000));
        }

        [Test]
        public void Update_Value_Should_Work_At_Rate_More_Than_300000_Values_Per_Second()
        {
            //arrange
            var storageTestContext = CreateStorageTestContext();
            var numberOfItems = 1000000;
            var initialStorageData = storageTestContext.GenerateTestDataSequentially(1000000);
            var storage = storageTestContext.CreateStorage(initialStorageData);

            var testDataForUpdate = storageTestContext.GenerateTestDataForUpdate(1000000)
                .ToDictionary(k => k.Key, v => v.Value);

            //act
            Stopwatch w = new Stopwatch();
            w.Start();
            foreach (var testDataToUpdate in initialStorageData)
            {
                var newValue = testDataForUpdate[testDataToUpdate.Key];
                storage.UpdateValue(testDataToUpdate.Key, testDataToUpdate.Value, newValue);
            }
            w.Stop();

            //assert
            var elementsNumber = storage.Count();
            Assert.That(elementsNumber, Is.EqualTo(numberOfItems));
            Assert.That((numberOfItems / w.Elapsed.TotalMilliseconds) * 1000, Is.GreaterThan(300000));
        }

        [Test]
        public void Delete_Value_Should_Work_At_Rate_More_Than_300000_Values_Per_Second()
        {
            //arrange
            var storageTestContext = CreateStorageTestContext();
            var numberOfItems = 1000000;
            var testData = storageTestContext.GenerateTestDataSequentially(1000000);
            var storage = storageTestContext.CreateStorage(testData);
         
            //act
            Stopwatch w = new Stopwatch();
            w.Start();


            foreach (var testDataToUpdate in testData)
            {
                storage.DeleteValue(testDataToUpdate.Key);
            }

            w.Stop();


            //assert
            var elementsNumber = storage.Count();
            Assert.That(elementsNumber, Is.EqualTo(numberOfItems));
            Assert.That((numberOfItems / w.Elapsed.TotalMilliseconds) * 1000, Is.GreaterThan(300000));
        }




        protected abstract StorageTestContext<T1, T2> CreateStorageTestContext();
    }
}
