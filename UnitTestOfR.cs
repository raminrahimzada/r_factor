using System;
using System.Linq;
using RFactor;
using Xunit;

namespace RFactorTests
{
    public class UnitTestOfR
    {
        private static readonly Random _random = new Random();
        private const int Upper = 100;

        private static ulong GetRandomInteger()
        {
            return (ulong)_random.Next(1, 10000);
        }

        [Fact]
        public void Test_All()
        {
            for (int i = 0; i < Upper; i++)
            {
                ulong p1 = GetRandomInteger();
                ulong p2 = GetRandomInteger();
                ulong n = p1 * p2;
                var all = R.Find(n).ToArray();
                var leftSideArr = all.Select(x => R.Number(x.Item1)).ToArray();
                var rightSideArr = all.Select(x => R.Number(x.Item2)).ToArray();
                Assert.Contains(p1, leftSideArr);
                Assert.Contains(p2, leftSideArr);
                Assert.Contains(p1, rightSideArr);
                Assert.Contains(p2, rightSideArr);
            }
        }
    }
}
