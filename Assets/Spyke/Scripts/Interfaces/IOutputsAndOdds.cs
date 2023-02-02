using System.Collections.Generic;

namespace Spyke.Scripts.Interfaces
{
    public interface IOutputsAndOdds
    {
        public Dictionary<int, int> GetOutputsAndOddsAsDictionary();
    
        public int GetTotalOutputCount();
    
        public int[] GetSymbolSequenceAtRowIndex(int rowIndex);
    
        public int GetCoinRewardAtRowIndex(int rowIndex);
    }
}

