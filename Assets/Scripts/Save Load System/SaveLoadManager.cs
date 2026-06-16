using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveLoadManager
{
    private static string _saveFileName = "SaveFile";
    private static string _saveFileType = "save";
    private static List<ISaveLoadObject> _saveLoadObjects = new();

    public static void Save()
    {
        string basePath = Application.persistentDataPath;
        string combinedPath = $"{basePath}/{_saveFileName}.{_saveFileType}";

        SaveLoadData data = new SaveLoadData();
        
        Debug.Log($"[SAVE LOAD MANAGER] Starting save for data: {data} -"); 

        foreach (ISaveLoadObject saveLoadObject in _saveLoadObjects)
            saveLoadObject.SaveCallback(data);

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream file = File.Create(combinedPath);

        binaryFormatter.Serialize(file, data);
        file.Close();

        foreach (ISaveLoadObject saveLoadObject in _saveLoadObjects)
            saveLoadObject.SaveCallback(data);
        
        Debug.Log($"[SAVE LOAD MANAGER] Successfully saved the data -"); 
    }

    public static void Load()
    {
        string basePath = Application.persistentDataPath;
        string combinedPath = $"{basePath}/{_saveFileName}.{_saveFileType}";

        BinaryFormatter binaryFormatter = new BinaryFormatter();

        if (!File.Exists(combinedPath))
            return;
        
        FileStream file = File.Open(combinedPath, FileMode.Open);

        SaveLoadData data = (SaveLoadData)binaryFormatter.Deserialize(file);
        file.Close();

        if (data == null)
        {
            Debug.Log($"[SAVE LOAD MANAGER] Invalid save data detected -");
            return;
        }

        foreach (ISaveLoadObject saveLoadObject in _saveLoadObjects)
            saveLoadObject.LoadCallback(data);

        Debug.Log($"[SAVE LOAD MANAGER] Valid save data detected: Invoking load callbacks -");
    }

    public static void Subscribe(ISaveLoadObject saveLoadObject)
    {
        if (_saveLoadObjects.Contains(saveLoadObject))
            return;
        
        _saveLoadObjects.Add(saveLoadObject);

        Debug.Log($"[SAVE LOAD MANAGER] Subscribed {saveLoadObject.GetType().Name} -");
    }

    public static void Unsubscribe(ISaveLoadObject saveLoadObject)
    {
        if (!_saveLoadObjects.Contains(saveLoadObject))
            return;
        
        _saveLoadObjects.Remove(saveLoadObject);

        Debug.Log($"[SAVE LOAD MANAGER] Unsubscribed {saveLoadObject.GetType().Name} -");
    }

    public static void Reset()
    {
        string basePath = Application.persistentDataPath;
        string combinedPath = $"{basePath}/{_saveFileName}.{_saveFileType}";

        File.Delete(combinedPath);

        Debug.Log($"[SAVE LOAD MANAGER] Successfully deleted all the data -"); 
    }
}