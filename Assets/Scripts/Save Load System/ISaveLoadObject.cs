using UnityEngine.Rendering;

public interface ISaveLoadObject
{
    public abstract void SaveCallback(SaveLoadData data);
    public abstract void LoadCallback(SaveLoadData data);
    public abstract void Subscribe(ISaveLoadObject saveLoadObject);
    public abstract void Unsubscribe(ISaveLoadObject saveLoadObject);
}