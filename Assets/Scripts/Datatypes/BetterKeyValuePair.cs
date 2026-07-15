using System;
using NaughtyAttributes;
using UnityEngine;

[Serializable]
public class BetterKeyValuePair<TKey, TValue>
{
    public BetterKeyValuePair(TKey key, TValue value) {
        this.Key = key;
        this.Value = value;
    }
    
    [ShowAssetPreview]
    public TKey Key;

    [ShowAssetPreview]
    public TValue Value;
}