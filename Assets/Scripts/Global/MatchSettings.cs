using UnityEngine;

[CreateAssetMenu(menuName = "Settings/New Match Settings")]
public class MatchSettings : ScriptableObject
{
    [SerializeField] [Range(1,10)] private int _enemyCount;
    public int EnemyCount => _enemyCount;

    [SerializeField] private TimeMS _matchTime;
    public TimeMS MatchTime => _matchTime;
}