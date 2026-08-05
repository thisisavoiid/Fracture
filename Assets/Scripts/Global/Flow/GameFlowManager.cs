using System;
using System.Collections.Generic;
using NaughtyAttributes;
using ToolkitByJonathan;
using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    [SerializeField] private List<GameFlowEventsPair> _gameFlowList;
    [SerializeField] private bool _useAutoGameFlow;
    [SerializeField]
    [ShowIf("_useAutoGameFlow")]
    private GameFlowEventsPair _autoGameFlow;
    private Dictionary<FlowType, List<ScriptableEvent>> _gameFlowDict = new();
    private FlowType _currentFlow = FlowType.Undefined;

    private static GameFlowManager _instance;
    public static GameFlowManager Instance => _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            _instance = this;
        }
        LoadGameFlows();
    }

    private void Start()
    {
        if (!_useAutoGameFlow)
            return;

        if (_currentFlow == FlowType.Undefined)
            ChangeFlow(_autoGameFlow.Type);
    }
    private void LoadGameFlows()
    {
        if (_gameFlowList.Count == 0)
            return;

        foreach (GameFlowEventsPair gameFlow in _gameFlowList)
        {
            _gameFlowDict.Add(gameFlow.Type, gameFlow.InvokeEvents);
            Debug.Log($"[GAME FLOW MANAGER] Successfully fetched game flow of type: {gameFlow.Type} -");
        }

        Debug.Log($"[GAME FLOW MANAGER] Fetched {_gameFlowDict.Keys.Count} game flows! -");
    }

    private void ChangeFlow(FlowType type)
    {
        if (!_gameFlowDict.ContainsKey(type))
            return;

        if (_gameFlowDict[type].Count == 0)
            return;

        Debug.Log($"[GAME FLOW MANAGER] Flow change: {_currentFlow} => {type} -");

        _currentFlow = type;

        foreach (ScriptableEvent flowEvent in _gameFlowDict[_currentFlow])
            flowEvent?.Invoke();

    }

    [ContextMenu("Force Start: Start Match Flow")]
    public void StartMatchFlow()
    {
        ChangeFlow(FlowType.MatchStart);
    }

    [ContextMenu("Force Start: End Match Flow")]
    public void EndMatchFlow()
    {
        ChangeFlow(FlowType.MatchEnd);
    }

    [ContextMenu("Force Start: Main Menu Enter")]
    public void MainMenuEnterFlow()
    {
        ChangeFlow(FlowType.MainMenuEnter);
    }

    [ContextMenu("Force Start: Loadout Selection Enter Flow")]
    public void LoadoutSelectionEnterFlow()
    {
        ChangeFlow(FlowType.LoadoutSelectionEnter);
    }

    [ContextMenu("Force Start: Loudout Selection Done Flow")]
    public void LoadoutSelectionDoneFlow()
    {
        ChangeFlow(FlowType.LoadoutSelectionDone);
    }
}
