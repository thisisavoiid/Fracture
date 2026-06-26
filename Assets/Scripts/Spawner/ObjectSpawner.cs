using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Spawnable Type, Amount")]
    [SerializeField] private List<BetterKeyValuePair<Spawnable, int>> _spawnables = new();

    [Button]
    public void Spawn()
    {
        if (_spawnables.Count == 0)
            return;
        
        int index = Random.Range(0, _spawnables.Count);
        Spawnable obj = _spawnables[index].Key;

        if (obj == null)
            return;
        
        for (int i=0; i<_spawnables[index].Value; i++)
        {
            Spawnable instance = Instantiate(obj, transform);
            instance.name = $"{obj.name} {i+1}";
            instance.Spawn();
        }
    }
}
