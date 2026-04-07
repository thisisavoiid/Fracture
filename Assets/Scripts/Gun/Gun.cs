using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Gun", menuName = "ScriptableObjects/Gun", order = 1)]
public class Gun : ScriptableObject
{
    public string Name;
    public GunConfig Stats;
    public Sound Sound;
    public GunType Type;
}