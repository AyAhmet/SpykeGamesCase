using UnityEngine;

[System.Serializable]
public struct SlotMachineRow
{
    [SerializeField] private SlotMachineSymbols[] OrderOfSymbols;
    [SerializeField] private int m_Odds;
    [SerializeField] private int m_CoinReward;

    public int Odds => m_Odds;
    public int CoinReward => m_CoinReward;
    public int[] OrderAsRaw => GetOrderOfSymbolsAsRawData();

    private int[] GetOrderOfSymbolsAsRawData()
    {
        var rawArray = new int[OrderOfSymbols.Length];

        for (var i = 0; i < OrderOfSymbols.Length; i++)
        {
            rawArray[i] = (int) OrderOfSymbols[i];
        }

        return rawArray;
    } 
}