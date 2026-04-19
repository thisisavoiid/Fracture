using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct StateTransition
{
    public State State;
    
    public List<StateConditionPair> TargetStateConditions;
}