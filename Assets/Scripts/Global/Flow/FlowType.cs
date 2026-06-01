using System;
using UnityEngine;

[Serializable]
public enum FlowType
{
    Undefined,
    MainMenuEnter,
    LoadoutSelectionEnter,
    LoadoutSelectionDone,
    MatchStart,
    MatchEnd
}