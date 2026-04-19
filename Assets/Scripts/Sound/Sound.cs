using UnityEngine;

[CreateAssetMenu(fileName = "Sound", menuName = "Sounds/New Sound", order = 1)]
public class Sound : ScriptableObject
{
    [SerializeField] private SoundConfig _soundConfig;
    public SoundConfig Data => _soundConfig;
}