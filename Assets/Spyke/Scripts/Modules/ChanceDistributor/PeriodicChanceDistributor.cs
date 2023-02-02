using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Spyke.Scripts.Modules.ChanceDistributor
{
    public class PeriodicChanceDistributor : ChanceDistributor
    {
        private readonly Dictionary<int, int> m_ChanceTable;
        private readonly int[] m_OutputArray;

    
        public PeriodicChanceDistributor(Dictionary<int, int> chanceTable)
        {
            m_ChanceTable = chanceTable;
            m_OutputArray = new int[chanceTable.Values.Sum()];
        }


        public override int[] GetNewOutputs()
        {
            GenerateNewOutputs();
            return m_OutputArray;
        }


        private void GenerateNewOutputs()
        {
            var keyCount = m_ChanceTable.Keys.Count;
            var occurrenceCounts = new int[keyCount];
            var occurrenceRatios = new float[keyCount];
            var probabilityArr = GetProbabilityArray();

            var rand = new System.Random();
            for (var i = 0; i < m_OutputArray.Length; i++)
            {
                var selectedKey = 0;
                var priorityKey = GetPriorityKeyIndex(i, probabilityArr, occurrenceCounts);

                if (priorityKey != -1)
                    selectedKey = priorityKey;
                else
                {
                    var possibleSelections = GetPossibleSelections(probabilityArr, occurrenceRatios);
                    selectedKey = possibleSelections[rand.Next(0, possibleSelections.Count)];
                }
                
                m_OutputArray[i] = selectedKey;

                occurrenceCounts[selectedKey]++;
                UpdateOccurrenceRatios(occurrenceRatios, occurrenceCounts);
            }
        }

        private int GetPriorityKeyIndex(int selectionIndex, float[] probabilityArr, int[] occurrenceCounts)
        {
            if (selectionIndex == m_OutputArray.Length - 1) return -1;
            
            for (var i = 0; i < probabilityArr.Length; i++)
            {
                if ((float) (occurrenceCounts[i] + 1) / (selectionIndex + 2) < probabilityArr[i])
                {
                    return i;
                } 
            }

            return -1;
        }

        private List<int> GetPossibleSelections(float[] probabilityArr, float[] occurrenceRatios)
        {
            var possibleSelections = new List<int>();
            for (var i = 0; i < probabilityArr.Length; i++)
            {
                if (occurrenceRatios[i] <= probabilityArr[i])
                {
                    possibleSelections.Add(i);
                }
            }

            return possibleSelections;
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
            var probabilityArr = new float[m_ChanceTable.Keys.Count];

            foreach (var pair in m_ChanceTable)
            {
                probabilityArr[pair.Key] = (float) pair.Value / m_OutputArray.Length;
            }

            return probabilityArr;
        }
    }
}