using System.Collections.Generic;
using UnityEditor.AssetImporters;
using UnityEngine;

public class ExplosiveCollectionManager : CollectionManager
{
    private static ExplosiveCollectionManager _instance;
    public static ExplosiveCollectionManager Instance => _instance;
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
}