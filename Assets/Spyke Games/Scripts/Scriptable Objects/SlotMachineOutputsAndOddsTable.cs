using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Slot Machine Outputs and Odds Table 000", menuName = "Scriptable Objects/Slot Machine Outputs and Odds Table")]
public class SlotMachineOutputsAndOddsTable : OutputsAndOddsTable
{
    [SerializeField] private SlotMachineRow[] Table;

    private Dictionary<int, int> m_CachedOutputsAndOddsDictionary;
    private int m_TotalOutputCount;
    
    private void CacheFields()
    {
        m_CachedOutputsAndOddsDictionary = new Dictionary<int, int>();
        m_TotalOutputCount = 0;

        for (var index = 0; index < Table.Length; index++)
        {
            var odds = Table[index].Odds;
            m_CachedOutputsAndOddsDictionary.Add(index, odds);
            m_TotalOutputCount += odds;
        }
    }

    public override Dictionary<int, int> GetOutputsAndOddsAsDictionary()
    {
        if (m_CachedOutputsAndOddsDictionary == null)
            CacheFields();

        return m_CachedOutputsAndOddsDictionary;
    }

    public override int GetTotalOutputCount()
    {
        if (m_CachedOutputsAndOddsDictionary == null)
            CacheFields();

        return m_TotalOutputCount;
    }

    public override int[] GetSymbolSequenceAtRowIndex(int rowIndex)
    {
        return Table[rowIndex].OrderAsRaw;
    }

    public override int GetCoinRewardAtRowIndex(int rowIndex)
    {
        return Table[rowIndex].CoinReward;
    }
}
