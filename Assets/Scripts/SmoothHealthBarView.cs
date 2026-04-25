using UnityEngine;
using UnityEngine.UI;

public class SmoothHealthBarView : MonoBehaviour
{
    [SerializeField] private Health _health;
    [SerializeField] private Slider _slider;
    [SerializeField] private float _smoothSpeed = 15f;

    private float _targetValue;

    private void OnEnable()
    {
        if (_health != null)
        {
            _health.HealthChanged += DrawFillZone;
            SetupSlider();
            _targetValue = _health.CurrentHealth;
            _slider.value = _targetValue;
        }
    }

    private void Update()
    {
        _slider.value = Mathf.MoveTowards(_slider.value, _targetValue, _smoothSpeed * Time.deltaTime);
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
        _targetValue = current;
    }
}