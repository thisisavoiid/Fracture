using UnityEngine;

[CreateAssetMenu(menuName = "Game Events/New Game Event")]
public class GameEvent : ScriptableObject
{
    [SerializeField] private string _name;
    public string Name => _name;
}