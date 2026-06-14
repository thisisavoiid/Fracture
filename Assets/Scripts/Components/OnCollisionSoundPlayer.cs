using TMPro;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class OnCollisionSoundPlayer : MonoBehaviour
{
    [SerializeField] private Sound _sound;
    public Sound Sound => _sound;
    
    private void OnCollisionEnter(Collision collision)
    {
        AudioManager.Instance.PlaySound(_sound, collision.contacts[0].point);
    }

    public void SetSound(Sound sound) => _sound = sound;
}
