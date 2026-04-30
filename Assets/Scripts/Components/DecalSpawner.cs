using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class DecalSpawner : MonoBehaviour
{
    [SerializeField] private DecalConfig _decalConfig;
    [SerializeField] private DecalObject _decalObject;

    public DecalObject SpawnDecal(Vector3 pos, Quaternion dir, Transform parent)
    {
        
        if (_decalObject == null)
        {
            Debug.LogWarning($"[DECAL SPAWNER] You tried spawning a decal while it was null (On object: {this.gameObject.name}) -");
            return null;
        }

        DecalObject newDecalObject = Instantiate(
            _decalObject,
            pos,
            dir,
            parent
        );

        newDecalObject.Initialize(_decalConfig);

        return newDecalObject;
    }
}
