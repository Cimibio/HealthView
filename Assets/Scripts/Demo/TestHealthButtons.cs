using UnityEngine;
using UnityEngine.UI;

public class TestHealthButtons : MonoBehaviour
{
    [SerializeField] private Health _health;
    [SerializeField] private Button _damageButton;
    [SerializeField] private Button _healButton;
    [SerializeField] private float _damageAmount = 10f;
    [SerializeField] private float _healAmount = 15f;

    private void OnEnable()
    {
        _damageButton.onClick.AddListener(DealDamage);
        _healButton.onClick.AddListener(ApplyHeal);
    }

    private void OnDisable()
    {
        _damageButton.onClick.RemoveListener(DealDamage);
        _healButton.onClick.RemoveListener(ApplyHeal);
    }

    private void DealDamage()
    {
        _health.TakeDamage(_damageAmount);
    }

    private void ApplyHeal()
    {
        _health.Heal(_healAmount);
    }
}