namespace Spyke.Scripts.Interfaces
{
    public interface ISlotMachine
    {
        public int GetNextRowIndex();

        public void SaveCurrentOutputsToDisk();
    }
}