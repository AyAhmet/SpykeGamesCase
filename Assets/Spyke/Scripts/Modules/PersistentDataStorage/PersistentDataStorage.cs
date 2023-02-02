using Spyke.Scripts.Interfaces;

namespace Spyke.Scripts.Modules.PersistentDataStorage
{
    public abstract class PersistentDataStorage : IPersistData
    {
        public abstract void SaveJson(string key, IJsonSave jsonString);
    
        public abstract void LoadJsonOverwrite<T>(string key, T overwriteObj);
    
        public abstract T LoadJson<T>(string key);
    }
}