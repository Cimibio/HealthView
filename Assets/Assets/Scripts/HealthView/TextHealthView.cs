using UnityEngine;
using TMPro;

public class TextHealthView : MonoBehaviour
{
    [SerializeField] private Health _health;
    [SerializeField] private TMP_Text _healthText;
    [SerializeField] private string _format = "{0}/{1}";

    private void OnEnable()
    {
        if (_health != null)
        {
            _health.HealthChanged += WriteHealth;
            WriteHealth(_health.CurrentHealth, _health.MaxHealth);
        }
    }

    private void OnDisable()
    {
        if (_health != null)        
            _health.HealthChanged -= WriteHealth;        
    }

    private void WriteHealth(float current, float max)
    {
        _healthText.text = string.Format(_format, Mathf.RoundToInt(current), Mathf.RoundToInt(max));
    }
}