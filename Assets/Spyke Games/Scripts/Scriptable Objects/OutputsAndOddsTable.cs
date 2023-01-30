using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OutputsAndOddsTable : ScriptableObject, IOutputsAndOdds
{
    public abstract Dictionary<int, int> GetOutputsAndOddsAsDictionary();
    
    public abstract int GetTotalOutputCount();
    
    public abstract int[] GetSymbolSequenceAtRowIndex(int rowIndex);
    
    public abstract int GetCoinRewardAtRowIndex(int rowIndex);
}
