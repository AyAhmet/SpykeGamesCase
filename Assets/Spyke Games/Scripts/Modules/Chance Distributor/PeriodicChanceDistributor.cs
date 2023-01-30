using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public class PeriodicChanceDistributor : ChanceDistributor
{
    private readonly Dictionary<int, int> m_ChanceTable;
    private readonly int[] m_OutputArray;

    
    public PeriodicChanceDistributor(IOutputsAndOdds outputsAndOdds)
    {
        m_ChanceTable = outputsAndOdds.GetOutputsAndOddsAsDictionary();
        m_OutputArray = new int[outputsAndOdds.GetTotalOutputCount()];
    }


    public override int[] GetNewOutputs()
    {
        GenerateNewOutputs();
        return m_OutputArray;
    }


    private void GenerateNewOutputs()
    {
        var keys = m_ChanceTable.Keys.ToList();
        var keyCount = keys.Count;
        var occurrenceCounts = new int[keyCount];
        var occurrenceRatios = new float[keyCount];
        var probabilityArr = GetProbabilityArray();

        for (var i = 0; i < m_OutputArray.Length; i++)
        {
            var randKeyIndex = 0;
            
            do {
                randKeyIndex = Random.Range(0, keyCount);
            } while (occurrenceRatios[randKeyIndex] > probabilityArr[randKeyIndex]);

            m_OutputArray[i] = randKeyIndex;

            occurrenceCounts[randKeyIndex]++;
            UpdateOccurrenceRatios(occurrenceRatios, occurrenceCounts);
        }
    }


    private void UpdateOccurrenceRatios(float[] occurrenceRatios, int[] occurrenceCounts)
    {
        float total = occurrenceCounts.Sum(); // TODO: Optimize
        
        for (var i = 0; i < occurrenceCounts.Length; i++)
        {
            occurrenceRatios[i] = occurrenceCounts[i] / total;
        }
    }


    private float[] GetProbabilityArray()
    {
        var keys = m_ChanceTable.Keys.ToList();
        var keyCount = keys.Count;
        var probabilityArr = new float[keyCount];

        for (var i = 0; i < keyCount; i++)
        {
            probabilityArr[i] = (float) m_ChanceTable[keys[i]] / m_OutputArray.Length;
        }

        return probabilityArr;
    }
}