using System.Collections.Generic;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;

public class Spawner<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] private List<T> _objectPool;

    [Button("Invoke Spawning")]
    public void Spawn(int quantity = 1)
    {
        for (int i = 0; i < quantity; i++)
        {
            T randomObject = _objectPool[Random.Range(0, _objectPool.Count)];
            T newGameObject = Instantiate(randomObject, transform.position, Quaternion.identity);
            newGameObject.name = randomObject.gameObject.name;
        }
    }
}