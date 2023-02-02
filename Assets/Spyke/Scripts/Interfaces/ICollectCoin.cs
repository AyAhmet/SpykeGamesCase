using System;

namespace Spyke.Scripts.Interfaces
{
    public interface ICollectCoin
    {
        public event Action<int> OnCoinCollected;

    }
}