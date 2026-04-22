using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms;

public class BodyPart : MonoBehaviour, IShootable
{
    [SerializeField] private float _damageMultiplier = 1.0f;
    public UnityEvent<float> OnDamageTaken;

    public void Hit(float dmg, Vector3 point)
    {
        OnDamageTaken?.Invoke(dmg * _damageMultiplier);
    }
}