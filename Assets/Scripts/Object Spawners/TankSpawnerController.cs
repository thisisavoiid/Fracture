using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the randomized spawning of explosive tank objects from a predefined pool.
/// </summary>
public class TankSpawnerController : MonoBehaviour
{
    [Tooltip("A list of explosive tank prefabs available for spawning.")]
    [SerializeField] private List<ExplosiveController> _tankPool;

    /// <summary>
    /// Selects and instantiates a random tank from the pool upon initialization.
    /// </summary>
    private void Start()
    {
        if (_tankPool == null || _tankPool.Count == 0)
        {
            Debug.LogWarning($"[TANK SPAWNER CONTROLLER] {gameObject.name} has an empty or null tank pool -");
            return;
        }

        ExplosiveController randomObject = _tankPool[Random.Range(0, _tankPool.Count)];
        ExplosiveController controllerObj = Instantiate(randomObject, transform.position, Quaternion.identity);
        controllerObj.gameObject.name = randomObject.gameObject.name;
        
        Debug.Log($"[TANK SPAWNER CONTROLLER] Spawned {controllerObj.gameObject.name} at {transform.position} -");
    }
}