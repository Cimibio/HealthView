using UnityEngine;
using UnityEngine.UI;

public class SimpleHealthBarView : MonoBehaviour
{
    [SerializeField] private Health _health;
    [SerializeField] private Slider _slider;

    private void OnEnable()
    {
        if (_health != null)
        {
            _health.HealthChanged += DrawFillZone;
            SetupSlider();
            DrawFillZone(_health.CurrentHealth, _health.MaxHealth);
        }
    }

    private void OnDisable()
    {
        if (_health != null)        
            _health.HealthChanged -= DrawFillZone;        
    }

    private void SetupSlider()
    {
        _slider.minValue = 0;
        _slider.maxValue = _health.MaxHealth;
        _slider.wholeNumbers = false;
    }

    private void DrawFillZone(float current, float max)
    {
        _slider.value = current;
    }
}