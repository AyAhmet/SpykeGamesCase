using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOutputsAndOdds
{
    public Dictionary<int, int> GetOutputsAndOddsAsDictionary();

    public int GetTotalOutputCount();

    public int[] GetSymbolSequenceAtRowIndex(int rowIndex);

    public int GetCoinRewardAtRowIndex(int rowIndex);
}
