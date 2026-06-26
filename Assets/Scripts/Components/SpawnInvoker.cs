using UnityEngine;

public class SpawnInvoker : Spawnable
{
    [SerializeField] private Spawnable _spawnable;
    public override void Spawn()
    {
        if (_spawnable == null)
            return;
        
        _spawnable.Spawn();
    }
}
