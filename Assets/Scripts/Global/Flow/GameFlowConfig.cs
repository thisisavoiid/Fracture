using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public struct GameFlowEventsPair
{
    public FlowType Type;
    public List<ScriptableEvent> InvokeEvents;
}