using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Procedural Generation/Constraints Asset/New Constraints Asset")]
public class ArenaGenerationConstraintsAsset : ScriptableObject
{
    [SerializeField]
    public List<GenerationConstraint> Constraints = new();
}