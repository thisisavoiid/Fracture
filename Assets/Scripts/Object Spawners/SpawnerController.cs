using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class TankSpawnerController : MonoBehaviour
{
    [SerializeField] private List<ExplosiveController> _tankPool;
    private void Start()
    {
        ExplosiveController randomObject = _tankPool[Random.Range(0, _tankPool.Count)];
        ExplosiveController controllerObj = Instantiate(randomObject, transform.position, Quaternion.identity);
        controllerObj.gameObject.name = randomObject.gameObject.name;
    }
}
