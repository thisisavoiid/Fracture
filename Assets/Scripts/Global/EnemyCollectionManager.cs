using System.Collections.Generic;
using UnityEngine;

public class EnemyCollectionManager : CollectionManager
{
    private static EnemyCollectionManager _instance;
    public static EnemyCollectionManager Instance => _instance;
    
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