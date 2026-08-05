using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using ToolkitByJonathan;

public class Health : MonoBehaviour
{
    [SerializeField] private float _defaultHealth;
    public float DefaultHealth => _defaultHealth;
    
    [SerializeField] private bool _autoInvokeDie;
    [SerializeField] private ScriptableEvent _deathEvent;
    [SerializeField] private ScriptableEvent _damageTakenEvent;

    private float _health;
    private bool _hasAlreadyDied = false;
    public float CurrentHealth => _health;
    
    public UnityEvent OnDeath;
    public UnityEvent<float> OnHealthRefresh;

    private void Awake()
    {
        _health = _defaultHealth;
    }

    public void Die()
    {
        if (_hasAlreadyDied)
            return;
        
        _hasAlreadyDied = true;
        Debug.Log($"[HEALTH SYSTEM] {gameObject.name} died -");
        _health = 0;
        _deathEvent?.Invoke();
        OnDeath?.Invoke();
    }

    public void TakeDamage(float dmg)
    {
        if (_hasAlreadyDied)
            return;
        
        float targetHealth = Mathf.Max(0, _health - dmg);
        _health = targetHealth;

        OnHealthRefresh?.Invoke(_health);
        _damageTakenEvent?.Invoke();

        Debug.Log($"[HEALTH SYSTEM] {gameObject.name} took damage. Current health: {_health} -");

        if (_health == 0 && _autoInvokeDie)
            Die();
    }
}