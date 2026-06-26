using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] private List<Spawnable> _spawnables = new();

    [Button]
    public void Spawn()
    {
        if (_spawnables.Count == 0)
            return;
        
        int index = Random.Range(0, _spawnables.Count);
        Spawnable obj = _spawnables[index];

        if (obj == null)
            return;
        
        Spawnable instance = Instantiate(obj, transform);
        instance.name = obj.name;

        instance.Spawn();
    }
}
