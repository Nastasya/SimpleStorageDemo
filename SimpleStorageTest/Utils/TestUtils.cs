using System.Diagnostics;

namespace SimpleStorageTest.Utils
{
    class TestUtils
    {
        public static double CalculateRequestsPerSecond(int expectedNumberOfItems, Stopwatch stopwatch)
        {
            return (expectedNumberOfItems / stopwatch.Elapsed.TotalMilliseconds) * 1000;
        }
    }
}
