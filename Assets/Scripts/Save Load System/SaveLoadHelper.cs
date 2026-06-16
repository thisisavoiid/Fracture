using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public abstract class SaveLoadHelper : MonoBehaviour, ISaveLoadObject
{
    [Button] public void InitializeSave() => SaveLoadManager.Save();
    [Button] public void InitializeLoad() => SaveLoadManager.Load();
    [Button] public void InitializeReset() => SaveLoadManager.Reset();
    public abstract void LoadCallback(SaveLoadData data);
    public abstract void SaveCallback(SaveLoadData data);
    public void Subscribe(ISaveLoadObject saveLoadObject) => SaveLoadManager.Subscribe(saveLoadObject);
    public void Unsubscribe(ISaveLoadObject saveLoadObject) => SaveLoadManager.Unsubscribe(saveLoadObject);
    private void OnEnable() => Subscribe(this);
    private void OnDisable() => Unsubscribe(this);
}
