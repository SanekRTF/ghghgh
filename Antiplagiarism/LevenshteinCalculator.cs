using System;
using System.Collections.Generic;
using System.Linq;

using DocumentTokens = System.Collections.Generic.List<string>;

namespace Antiplagiarism
{
    public class LevenshteinCalculator
    {
        public List<ComparisonResult> CompareDocumentsPairwise(List<DocumentTokens> documents)
        {
            var comparisonResults = new List<ComparisonResult>();
            for (var docIndex1 = 0; docIndex1 < documents.Count; docIndex1++)
            {
                for (var docIndex2 = docIndex1 + 1; docIndex2 < documents.Count; docIndex2++)
                {
                    comparisonResults.Add(CalculateDistance(documents[docIndex1], documents[docIndex2]));
                }
            }

            return comparisonResults;
        }

        private ComparisonResult CalculateDistance(DocumentTokens firstDocument, DocumentTokens secondDocument)
        {
            var editDistanceMatrix = new double[firstDocument.Count + 1, secondDocument.Count + 1];
            for (var i = 0; i <= firstDocument.Count; i++)
                editDistanceMatrix[i, 0] = i;
            for (var j = 0; j <= secondDocument.Count; j++)
                editDistanceMatrix[0, j] = j;

            for (var i = 1; i <= firstDocument.Count; i++)
            {
                for (var j = 1; j <= secondDocument.Count; j++)
                {
                    if (firstDocument[i - 1] == secondDocument[j - 1])
                        editDistanceMatrix[i, j] = editDistanceMatrix[i - 1, j - 1];
                    else
                    {
                        var replacementCost =
                            TokenDistanceCalculator.GetTokenDistance(firstDocument[i - 1], secondDocument[j - 1]) + editDistanceMatrix[i - 1, j - 1];
                        var additionCost = 1 + editDistanceMatrix[i - 1, j];
                        var removalCost = 1 + editDistanceMatrix[i, j - 1];
                        var minimumDistance = Math.Min(replacementCost, Math.Min(additionCost, removalCost));
                        editDistanceMatrix[i, j] = minimumDistance;
                    }
                }
            }
            return new ComparisonResult(firstDocument, secondDocument, editDistanceMatrix[firstDocument.Count, secondDocument.Count]);
        }
    }
}