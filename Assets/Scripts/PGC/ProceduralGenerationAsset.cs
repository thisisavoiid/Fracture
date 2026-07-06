using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

[CreateAssetMenu(menuName = "Procedural Generation/Procedural Generation Asset/New Procedural Generation Asset")]
public class ProceduralGenerationAsset : ScriptableObject
{
    [SerializeField] private string _name;
    public string Name => _name;

    [SerializeField] private ArenaItemType _type;
    public ArenaItemType Type => _type;

    [SerializeField] private Vector3 _offset;
    public Vector3 Offset => _offset;

    [SerializeField] private List<GameObject> _prefabs;
    public List<GameObject> Prefabs => _prefabs;
}