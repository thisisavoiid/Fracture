using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Procedural Generation/Rules Asset/New Rules Asset")]
public class ArenaGenerationRulesAsset : ScriptableObject
{
    [SerializeField] 
    private List<TypeRule> _rules;

    public List<TypeRule> Rules => _rules;

    private void Reset()
    {
        foreach (int arenaItemType in typeof(ArenaItemType).GetEnumValues())
        {
            if (arenaItemType == 0)
                continue;
                
            TypeRule rule = new TypeRule();
            rule.ExcludesFromNeighbors = new();
            rule.Type = (ArenaItemType)arenaItemType;
            rule.Weight = 0.5f;

            _rules.Add(rule);
        }
    }
}