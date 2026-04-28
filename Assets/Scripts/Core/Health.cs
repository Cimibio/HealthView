using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float _maxValue = 100f;
    [SerializeField] private float _currentValue;

    public event Action<float, float> Changed;
    public event Action Died;
    public event Action Hitted;

    public float Current => _currentValue;
    public float Max => _maxValue;
    public bool IsAlive => _currentValue > 0;

    private void Awake()
    {
        _currentValue = _maxValue;
    }

    public void TakeDamage(float amount)
    {
        if (!IsAlive)
            return;

        _currentValue = Mathf.Max(0, _currentValue - amount);

        Hitted?.Invoke();
        Changed?.Invoke(_currentValue, _maxValue);

        if (_currentValue <= 0)
            Died?.Invoke();
    }

    public void Heal(float amount)
    {
        if (!IsAlive)
            return;

        _currentValue = Mathf.Min(_maxValue, _currentValue + amount);

        Changed?.Invoke(_currentValue, _maxValue);
    }

    public void Reset()
    {
        _currentValue = _maxValue;
        Changed?.Invoke(_currentValue, _maxValue);
    }
}