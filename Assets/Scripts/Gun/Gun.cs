using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Gun", menuName = "Gun/New Gun", order = 1)]
public class Gun : ScriptableObject
{
    public string Name;
    public GunConfig Stats;
    public Sound Sound;
    public GunBehaviour Behaviour;
}