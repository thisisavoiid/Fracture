using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Explosive", menuName = "Explosives/New Explosive", order = 1)]
public class Explosive : ScriptableObject
{
    public string Name;
    public ExplosiveConfig Config;
    public Sound DetonationCycleStartSound;
    public Sound ExplodeSound;
}