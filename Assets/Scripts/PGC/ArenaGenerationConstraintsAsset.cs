using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Procedural Generation/Constraints Asset/New Constraints Asset")]
public class ArenaGenerationConstraintsAsset : ScriptableObject
{
    [SerializeField]
    private List<GenerationConstraint> _constraints = new();

    public List<GenerationConstraint> Constraints => _constraints;
    
    public void AddConstraint(GenerationConstraint constraint)
    {
        _constraints.Add(constraint);
    }

    public void RemoveConstraint(GenerationConstraint constraint)
    {
        if (!_constraints.Contains(constraint))
            return;
        
        _constraints.Remove(constraint);
    }

    public void Reset()
    {
        _constraints.Clear();
    }
}