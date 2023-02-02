using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Spyke.Scripts.Modules.ChanceDistributor;
using UnityEngine;

public class ChanceDistributorTests
{
    public Dictionary<int, int> ExampleChanceTable = new Dictionary<int, int>()
    {
        {0, 13},
        {1, 13},
        {2, 13},
        {3, 13},
        {4, 13},
        {5, 9},
        {6, 8},
        {7, 7},
        {8, 6},
        {9, 5}
    };

    private Dictionary<int, int> GetRandomInputDictionary(int length)
    {
        var rand = new System.Random();
        var numberOfUniqueChances = Mathf.CeilToInt(Mathf.Sqrt(length));
        var chancesArr = new int[numberOfUniqueChances];
        
        for (var i = 0; i < length; i++)
        {
            var randKey = rand.Next(0, numberOfUniqueChances);
            chancesArr[randKey]++;
        }

        var chancesDict = new Dictionary<int, int>();
        for (int i = 0; i < chancesArr.Length; i++)
        {
            if (chancesDict.ContainsKey(i) == false)
                chancesDict.Add(i, chancesArr[i]);
            else
                chancesDict[i]++;
        }

        return chancesDict;
    }

    [Test]
    public void EqualChanceCountInputAndOutputTest()
    {
        var totalChanceCount = 100;
        var randomGeneratedInputDictionary = GetRandomInputDictionary(totalChanceCount);
        var chanceDistributor = new PeriodicChanceDistributor(randomGeneratedInputDictionary);
        var outputsArray = chanceDistributor.GetNewOutputs();
        var keys = randomGeneratedInputDictionary.Keys;

        foreach (var k in keys)
        {
            var sum = outputsArray.Count(x => x == k);
            Assert.AreEqual(randomGeneratedInputDictionary[k], sum);
        }
    }


    [Test]
    public void PeriodicDistributionTest()
    {
        var totalChanceCount = 100;
        var randomGeneratedInputDictionary = GetRandomInputDictionary(totalChanceCount);
        var chanceDistributor = new PeriodicChanceDistributor(randomGeneratedInputDictionary);
        var outputsArray = chanceDistributor.GetNewOutputs();
        var keys = randomGeneratedInputDictionary.Keys;

        foreach (var k in keys)
        {
            var occurrenceIndexes = new List<int>();

            for (var i = 0; i < outputsArray.Length; i++)
            {
                if (outputsArray[i] == k)
                {
                    occurrenceIndexes.Add(i);
                }
            }
            
            var period = (float) totalChanceCount / randomGeneratedInputDictionary[k];
            for (var i = 0; i < randomGeneratedInputDictionary[k]; i++)
            {
                var periodStarts = Mathf.FloorToInt(i * period);
                var periodEnds = Mathf.CeilToInt((i + 1) * period);
                var occurredAt = occurrenceIndexes[i];
                var isInPeriod = occurredAt >= periodStarts & occurredAt <= periodEnds;
                Assert.IsTrue(isInPeriod);
            }
        }
    }
}
