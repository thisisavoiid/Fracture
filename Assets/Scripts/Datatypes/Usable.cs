using System;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    protected GameObject _holder;
    public GameObject Holder => _holder;
    [SerializeField] protected ItemConfiguration _config;
    public ItemConfiguration Config => _config;
    public abstract void Use(ItemUsageData usageData);
    public void SetHolder(GameObject gameObject) => _holder = gameObject;
}