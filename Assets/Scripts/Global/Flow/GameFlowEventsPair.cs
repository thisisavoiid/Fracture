using System;
using System.Collections.Generic;
using ToolkitByJonathan;

[Serializable]
public struct GameFlowEventsPair
{
    public FlowType Type;
    public List<ScriptableEvent> InvokeEvents;
}