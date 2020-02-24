using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using SimpleStorage.Exception;
using SimpleStorageTest.TestContext;
using SimpleStorageTest.Utils;

namespace SimpleStorageTest
{
    [TestFixture]
    [Category("ConcurrencyTest")]
    abstract class SimpleStorageConcurrentTestBase<T1,T2>
    {
        [Test]
        public async Task Add_Value_Concurrently_Should_Work_At_Rate_More_Than_5000_Requests_Per_Second()
        {
            //arrange
            var numberOfItems = 100;
            int numberOfThreads = 10;
            var expectedNumberOfItems = numberOfItems * numberOfThreads;

            var storageTestContext = CreateStorageTestContext();
            var storage = storageTestContext.CreateStorage();

            Stopwatch stopwatch = new Stopwatch();

            //act
            stopwatch.Start();
            var tasks = new List<Task>();
            for (int i = 1; i <= numberOfThreads; i++)
            {
                var threadNumber = i;
                tasks.Add(Task.Run(async () =>
                {
                    foreach (var testDataForAdd in storageTestContext.GenerateTestDataForAdd(numberOfItems, threadNumber))
                    {
                        await Task.Yield();
                        storage.Add(testDataForAdd.Key, testDataForAdd.Value);
                    }
                }));
            }

            await Task.WhenAll(tasks);
            stopwatch.Stop();

            //assert
            var actualElementsCount = storage.Count();
            Assert.That(actualElementsCount, Is.EqualTo(expectedNumberOfItems));
            var requestsPerSecond = TestUtils.CalculateRequestsPerSecond(expectedNumberOfItems, stopwatch);
            Assert.That(requestsPerSecond, Is.GreaterThan(5000));
        }

        [Test]
        public async Task Update_Value_Should_Work_At_Rate_More_Than_5000_Values_Per_Second()
        {
            //arrange
            var numberOfItems = 100;
            int numberOfThreads = 10;
            var expectedNumberOfItems = numberOfItems * numberOfThreads;

            var storageTestContext = CreateStorageTestContext();
            var initialStorageData = storageTestContext.GenerateTestDataSequentially(expectedNumberOfItems);
            var storage = storageTestContext.CreateStorage(initialStorageData);

            Stopwatch stopwatch = new Stopwatch();

            //act
            stopwatch.Start();
            var tasks = new List<Task>();
            for (int i = 1; i <= numberOfThreads; i++)
            {
                var threadNumber = i;
                tasks.Add(Task.Run(async () =>
                {
                    foreach (var testDataForUpdate in storageTestContext.GenerateTestDataForUpdate(numberOfItems,
                        threadNumber))
                    {
                        await Task.Yield();
                        storage.UpdateValue(testDataForUpdate.Key, initialStorageData[testDataForUpdate.Key],
                            testDataForUpdate.Value);
                    }
                }));
            }

            await Task.WhenAll(tasks);
            stopwatch.Stop();

            //assert
            var actualElementsCount = storage.Count();
            Assert.That(actualElementsCount, Is.EqualTo(expectedNumberOfItems));
            var requestsPerSecond = TestUtils.CalculateRequestsPerSecond(expectedNumberOfItems, stopwatch);
            Assert.That(requestsPerSecond, Is.GreaterThan(5000));
        }


        [Test]
        public async Task Add_SameValue_Concurrently_Should_Fail()
        {
            //arrange
            var numberOfItems = 100;
            int numberOfThreads = 10;
            var expectedNumberOfItems = numberOfItems;

            var storageTestContext = CreateStorageTestContext();
            var storage = storageTestContext.CreateStorage();

            Stopwatch stopwatch = new Stopwatch();
            var generatedTestDataToAdd = storageTestContext.GenerateTestDataForAdd(numberOfItems).ToList();

            //act
            stopwatch.Start();
            var tasks = new List<Task>();
            for (int i = 1; i <= numberOfThreads; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    foreach (var testDataForAdd in generatedTestDataToAdd)
                    {
                        await Task.Yield();
                        storage.Add(testDataForAdd.Key, testDataForAdd.Value);
                    }
                }));
            }

            bool duplicatedKeyValueExceptionIsFired = false;
            try
            {
                await Task.WhenAll(tasks);
            }
            catch (DuplicatedKeyValueException<T1>)
            {
                duplicatedKeyValueExceptionIsFired = true;
            }

            stopwatch.Stop();

            //assert
            var actualElementsCount = storage.Count();
            Assert.That(actualElementsCount, Is.EqualTo(expectedNumberOfItems));
            Assert.That(duplicatedKeyValueExceptionIsFired, Is.True);
      }


        [Test]
        public async Task Update_SameValue_Concurrently_Should_Fail()
        {
            //arrange
            var numberOfItems = 100;
            int numberOfThreads = 10;
            var expectedNumberOfItems = numberOfItems * numberOfThreads;

            var storageTestContext = CreateStorageTestContext();
            var initialStorageData = storageTestContext.GenerateTestDataSequentially(expectedNumberOfItems);
            var storage = storageTestContext.CreateStorage(initialStorageData);

            Stopwatch stopwatch = new Stopwatch();
            var testDataForUpdates = storageTestContext.GenerateTestDataForUpdate(numberOfItems);

            //act
            stopwatch.Start();
            var tasks = new List<Task>();
            for (int i = 1; i <= numberOfThreads; i++)
            {
               
                tasks.Add(Task.Run(async () =>
                {
                    foreach (var testDataForUpdate in testDataForUpdates)
                    {
                        await Task.Yield();
                        storage.UpdateValue(testDataForUpdate.Key, initialStorageData[testDataForUpdate.Key],
                            testDataForUpdate.Value);
                    }
                }));
            }

            bool valueWasModifiedInAnotherThreadExceptionFired = false;
            try
            {
                await Task.WhenAll(tasks);
            }
            catch (ValueModifiedByAnotherThreadException<T1>)
            {
                valueWasModifiedInAnotherThreadExceptionFired = true;
            }

            stopwatch.Stop();

            //assert
            var actualElementsCount = storage.Count();
            Assert.That(actualElementsCount, Is.EqualTo(expectedNumberOfItems));
            Assert.That(valueWasModifiedInAnotherThreadExceptionFired, Is.True);
            var requestsPerSecond = TestUtils.CalculateRequestsPerSecond(expectedNumberOfItems, stopwatch);
            Assert.That(requestsPerSecond, Is.GreaterThan(5000));
        }

        [Test]
        public async Task Delete_SameValue_Concurrently_Should_Fail()
        {
            //arrange
            var numberOfItems = 100;
            int numberOfThreads = 10;
            var expectedNumberOfItems = numberOfItems * numberOfThreads;

            var storageTestContext = CreateStorageTestContext();
            var initialStorageData = storageTestContext.GenerateTestDataSequentially(expectedNumberOfItems);
            var storage = storageTestContext.CreateStorage(initialStorageData);

            Stopwatch stopwatch = new Stopwatch();
            var generatedTestDataForDelete = storageTestContext.GenerateTestDataForUpdate(numberOfItems);

            //act
            stopwatch.Start();
            var tasks = new List<Task>();
            for (int i = 1; i <= numberOfThreads; i++)
            {

                tasks.Add(Task.Run(async () =>
                {
                    foreach (var testDataForDelete in generatedTestDataForDelete)
                    {
                        await Task.Yield();
                        storage.DeleteValue(testDataForDelete.Key);
                    }
                }));
            }

            await Task.WhenAll(tasks);
            stopwatch.Stop();

            //assert
            var actualElementsCount = storage.Count();
            Assert.That(actualElementsCount, Is.EqualTo(expectedNumberOfItems));
            var requestsPerSecond = TestUtils.CalculateRequestsPerSecond(expectedNumberOfItems, stopwatch);
            Assert.That(requestsPerSecond, Is.GreaterThan(5000));
        }

        protected abstract StorageTestContext<T1, T2> CreateStorageTestContext();

    }
}