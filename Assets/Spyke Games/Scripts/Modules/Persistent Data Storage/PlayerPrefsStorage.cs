using UnityEngine;

public class PlayerPrefsStorage : PersistentDataStorage
{
    public override void SaveJson(string key, IJsonSave jsonConvertableObj)
    {
        var jsonString = jsonConvertableObj.ToJson();
        PlayerPrefs.SetString(key, jsonString);
    }

    
    public override void LoadJsonOverwrite<T>(string key, T overwriteObj)
    {
        var jsonString = PlayerPrefs.GetString(key);
        JsonUtility.FromJsonOverwrite(jsonString, overwriteObj);
    }

    
    public override T LoadJson<T>(string key)
    {
        var jsonString = PlayerPrefs.GetString(key);
        return JsonUtility.FromJson<T>(jsonString);
    }
}
