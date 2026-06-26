using System;
using UnityEngine;

[Serializable]
public class BetterKeyValuePair<TKey, TValue>
{
    public BetterKeyValuePair(TKey key, TValue value) {
        this.Key = key;
        this.Value = value;
    }
    
    public TKey Key;
    public TValue Value;
}