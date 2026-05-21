using System;
using System.Collections.Generic;
[Serializable]
public struct GameFlowEventsPair
{
    public FlowType Type;
    public List<ScriptableEvent> InvokeEvents;
}