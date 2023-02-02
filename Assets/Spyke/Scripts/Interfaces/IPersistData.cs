namespace Spyke.Scripts.Interfaces
{
    public interface IPersistData
    {
        public void SaveJson(string key, IJsonSave jsonString);
    
        public void LoadJsonOverwrite<T>(string key, T overwriteObj);
        
        public T LoadJson<T>(string key);
    }
}
