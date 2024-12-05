using System;
using System.Collections.Generic;

namespace Antiplagiarism
{
    public static class LongestCommonSubsequenceCalculator
    {
        public static List<string> Calculate(List<string> firstSequence, List<string> secondSequence)
        {
            var optimizationTable = CreateOptimizationTable(firstSequence, secondSequence);
            return RestoreAnswer(optimizationTable, firstSequence, secondSequence);
        }

        private static int[,] CreateOptimizationTable(List<string> firstSequence, List<string> secondSequence)
        {
            var optimizationTable = new int[firstSequence.Count + 1, secondSequence.Count + 1];
            for (var rowIndex = 1; rowIndex <= firstSequence.Count; rowIndex++)
            {
                for (var colIndex = 1; colIndex <= secondSequence.Count; colIndex++)
                {
                    if (firstSequence[rowIndex - 1] == secondSequence[colIndex - 1])
                        optimizationTable[rowIndex, colIndex] = optimizationTable[rowIndex - 1, colIndex - 1] + 1;
                    else
                        optimizationTable[rowIndex, colIndex] = Math.Max(optimizationTable[rowIndex - 1, colIndex], optimizationTable[rowIndex, colIndex - 1]);
                }
            }
            return optimizationTable;
        }

        private static List<string> RestoreAnswer(int[,] optimizationTable, List<string> firstSequence, List<string> secondSequence)
        {
            var commonSubsequence = new List<string>();
            var pointerX = firstSequence.Count;
            var pointerY = secondSequence.Count;
            while (pointerX > 0 && pointerY > 0 && optimizationTable[pointerX, pointerY] != 0)
            {
                if (firstSequence[pointerX - 1] == secondSequence[pointerY - 1])
                {
                    commonSubsequence.Add(firstSequence[pointerX - 1]);
                    pointerX -= 1;
                    pointerY -= 1;
                }
                else
                {
                    if (optimizationTable[pointerX - 1, pointerY] < optimizationTable[pointerX, pointerY - 1])
                        pointerY -= 1;
                    else
                        pointerX -= 1;
                }
            }
            commonSubsequence.Reverse();
            return commonSubsequence;
        }
    }
}