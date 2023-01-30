public interface IJsonSave
{
    public string ToJson();
    public void FromJsonOverwrite(string json);
}