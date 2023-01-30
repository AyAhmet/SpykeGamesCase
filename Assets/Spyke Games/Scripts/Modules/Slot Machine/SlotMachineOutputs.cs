using UnityEngine;

[System.Serializable]
public class SlotMachineOutputs : IJsonSave
{
    public int[] Outputs;
    public int CurrentIndex;

    public SlotMachineOutputs(int[] outputs)
    {
        Outputs = outputs;
    }

    
    public bool CanGetNext()
    {
        return CurrentIndex < Outputs.Length;
    }


    public int GetNext()
    {
        return Outputs[CurrentIndex++];
    }
    

    public string ToJson()
    {
        return UnityEngine.JsonUtility.ToJson(this);
    }

    
    public void FromJsonOverwrite(string json)
    {
        UnityEngine.JsonUtility.FromJsonOverwrite(json, this);
    }
}