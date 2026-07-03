using NaughtyAttributes;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    protected GameObject _holder;
    public GameObject Holder => _holder;

    [SerializeField] 
    [Expandable]
    protected ItemConfiguration _config;
    public ItemConfiguration Config => _config;
    public abstract bool Use(ItemUsageData usageData);
    public void SetHolder(GameObject gameObject) => _holder = gameObject;
}