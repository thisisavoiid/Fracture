using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/New Game Settings")]
public class GameSettings : ScriptableObject
{
    [SerializeField] [Range(1,10)] private int _enemyCount;
    public int EnemyCount => _enemyCount;

    [SerializeField] private TimeMS _matchTime;
    public TimeMS MatchTime => _matchTime;

    [SerializeField] private List<GameEvent> _validGameEvents;
    public List<GameEvent> ValidGameEvents => _validGameEvents;

    [SerializeField] private Range _timeBetweenGameEvents;
    public Range TimeBetweenGameEvents => _timeBetweenGameEvents;

    [SerializeField] private Range _gameEventDuration;
    public Range GameEventDuration => _gameEventDuration;
}