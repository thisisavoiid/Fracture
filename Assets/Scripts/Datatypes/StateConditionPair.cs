using System;
using UnityEngine;

[Serializable]
public struct StateConditionPair
{
    public State State;
    public StateCondition Condition;
    public ConditionModifier Modifier;
}