using System;

public interface ICollectCoin
{
    public event Action<int> OnCoinCollected;

}
