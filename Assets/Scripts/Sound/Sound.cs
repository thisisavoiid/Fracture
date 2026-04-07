using UnityEngine;

[CreateAssetMenu(fileName = "Sound", menuName = "ScriptableObjects/Sound", order = 1)]
public class Sound : ScriptableObject
{
    [SerializeField] private SoundConfig _soundConfig;
    public SoundConfig Data => _soundConfig;
}