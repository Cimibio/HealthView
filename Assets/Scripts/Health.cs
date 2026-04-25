using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _currentHealth;

    public event Action<float, float> HealthChanged;
    public event Action Died;
    public event Action Hitted;

    public float CurrentHealth => _currentHealth;
    public float MaxHealth => _maxHealth;
    public bool IsAlive => _currentHealth > 0;
    public bool IsFull => _currentHealth >= _maxHealth;

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (!IsAlive)
            return;

        _currentHealth = Mathf.Max(0, _currentHealth - amount);

        Hitted?.Invoke();
        HealthChanged?.Invoke(_currentHealth, _maxHealth);

        if (_currentHealth <= 0)
            Died?.Invoke();
    }

    public void Heal(float amount)
    {
        if (!IsAlive)
            return;

        _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);

        HealthChanged?.Invoke(_currentHealth, _maxHealth);
    }

    public void Reset()
    {
        _currentHealth = _maxHealth;
        HealthChanged?.Invoke(_currentHealth, _maxHealth);
    }
}