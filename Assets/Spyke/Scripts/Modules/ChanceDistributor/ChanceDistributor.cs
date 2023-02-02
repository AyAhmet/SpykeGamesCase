using Spyke.Scripts.Interfaces;

namespace Spyke.Scripts.Modules.ChanceDistributor
{
    public abstract class ChanceDistributor : IOutputChance
    {
        public abstract int[] GetNewOutputs();
    }
}