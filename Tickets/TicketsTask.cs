using System;
using System.Numerics;

namespace Tickets
{
    public class TicketsTask
    {
        public static BigInteger Solve(int halfLength, int totalSum)
        {
            if (totalSum % 2 != 0)
                return 0;
            var halfSum = totalSum / 2;
            var combinationsCount = CalculateCombinationsCount(halfLength, halfSum);
            return combinationsCount * combinationsCount;
        }

        private static BigInteger CalculateCombinationsCount(int halfLength, int halfSum)
        {
            var dpTable = new BigInteger[halfLength + 1, halfSum + 1];
            for (var length = 1; length <= halfLength; length++)
                dpTable[length, 0] = 1;
            for (var sum = 1; sum <= halfSum && sum < 10; sum++)
                dpTable[1, sum] = 1;

            for (var length = 2; length <= halfLength; length++)
                for (var sum = 1; sum <= halfSum; sum++)
                {
                    if (sum < 10)
                        dpTable[length, sum] = dpTable[length, sum - 1] + dpTable[length - 1, sum];
                    else if (9 * length < sum)
                        dpTable[length, sum] = 0;
                    else
                    {
                        BigInteger totalCombinations = 0;
                        for (var digit = 0; digit < 10; digit++)
                            totalCombinations += dpTable[length - 1, sum - digit];
                        dpTable[length, sum] = totalCombinations;
                    }
                }

            return dpTable[halfLength, halfSum];
        }
    }
}