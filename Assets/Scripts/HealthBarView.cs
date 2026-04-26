using UnityEngine;
using UnityEngine.UI;

public abstract class HealthBarView : MonoBehaviour
{
    [SerializeField] protected Health _health;
    [SerializeField] protected Slider _slider;

    protected virtual void OnEnable()
    {
        if (_health != null)
        {
            _health.Changed += ApplyChanges;
            SetupSlider();
            SetInitialValue();
        }
    }

    protected virtual void OnDisable()
    {
        if (_health != null)
            _health.Changed -= ApplyChanges;
    }

    protected virtual void SetupSlider()
    {
        _slider.minValue = 0;
        _slider.maxValue = _health.Max;
        _slider.wholeNumbers = false;
    }

    protected virtual void SetInitialValue()
    {
        _slider.value = _health.Current;
    }

    protected abstract void ApplyChanges(float current, float max);
}