using UnityEngine;

public abstract class StateCondition : ScriptableObject
{
    public abstract bool IsMet();
}