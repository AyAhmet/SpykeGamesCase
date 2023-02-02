using Spyke.Scripts.Interfaces;
using UnityEngine;

namespace Spyke.Scripts.Controllers
{
    public class CoinParticleController : MonoBehaviour
    {
        [SerializeField] private ParticleSystem CoinParticleSystem;
        [SerializeField] private int MinParticleCount;
        [SerializeField] private int MaxParticleCount;
        [SerializeField] private int CoinRequiredForMaxParticleCount;
    
    
        public void Subscribe(ICollectCoin coinCollector)
        {
            coinCollector.OnCoinCollected += ActivateCoinParticles;
        }
    
    
        private void ActivateCoinParticles(int coinCollected)
        {
            var t = Mathf.Clamp01((float) coinCollected / CoinRequiredForMaxParticleCount);
            var coinCount = Mathf.Lerp(MinParticleCount, MaxParticleCount, t);
            var emissionModule = CoinParticleSystem.emission;
            var burst = emissionModule.GetBurst(0);
            burst.count = (int) coinCount;
            emissionModule.SetBurst(0, burst);
            
            CoinParticleSystem.Play();
        }
    }
}

