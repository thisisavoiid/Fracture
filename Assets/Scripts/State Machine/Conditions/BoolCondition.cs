using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "State Machine/Conditions/Bool Condition", fileName = "Bool Condition")]
public class BoolCondition : StateCondition
{
    [SerializeField] private BoolVariable _variable;
    [SerializeField] private bool _targetState;
    public override bool IsMet()
    {
        if (_variable == null)
        {
            Debug.LogWarning($"[STATE CONDITION] The variable on a {this.GetType().Name} object is null / not set. Returning false as default -");
            return false;
        }
        
        return _variable.Value == _targetState;
    }
}