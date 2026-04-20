using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private float _defaultHealth;
    [SerializeField] private bool _autoInvokeDie;
    private float _health;
    public UnityEvent OnDeath;
    public UnityEvent<float> OnHealthRefresh;

    private void Awake()
    {
        _health = _defaultHealth;
    }

    public void Die()
    {
        Debug.Log($"[HEALTH SYSTEM] {gameObject.name} died -");
        _health = 0;
        OnDeath?.Invoke();
    }

    public void TakeDamage(float dmg)
    {
        float targetHealth = Mathf.Max(0, _health - dmg);
        _health = targetHealth;

        OnHealthRefresh?.Invoke(_health);
        
        Debug.Log($"[HEALTH SYSTEM] {gameObject.name} took damage. Current health: {_health} -");

        if (_health == 0 && _autoInvokeDie)
            Die();
    }
}