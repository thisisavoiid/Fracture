using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver
{
    public List<TKey> Keys = new();
    public List<TValue> Values = new();

    private Dictionary<TKey, TValue> _dict = new();
    public Dictionary<TKey, TValue> Dictionary => _dict;

    public void Add(TKey key, TValue value)
    {
        _dict.Add(key, value);
    }

    public void Remove(TKey key)
    {
        _dict.Remove(key);
    }

    public void Clear()
    {
        _dict.Clear();
    }

    public void OnAfterDeserialize()
    {
        _dict.Clear();

        int entryCount = Mathf.Min(Keys.Count, Values.Count);

        for (int i = 0; i < entryCount; i++)
        {
            if (!_dict.ContainsKey(Keys[i]))
                _dict.Add(Keys[i], Values[i]);
        }
    }

    public void OnBeforeSerialize()
    {
        if (Keys.Count > 0 || Values.Count > 0)
            return;

        Keys.Clear();
        Values.Clear();

        foreach (KeyValuePair<TKey, TValue> keyValuePair in _dict)
        {
            if (!Keys.Contains(keyValuePair.Key))
            {
                Keys.Add(keyValuePair.Key);
                Values.Add(keyValuePair.Value);
            }
        }
    }
}