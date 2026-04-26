using UnityEngine;
using System.Collections;

public class SmoothHealthBarView : HealthBarView
{
    [SerializeField] private float _smoothSpeed = 15f;

    private Coroutine _smoothCoroutine;

    protected override void OnDisable()
    {
        base.OnDisable();

        if (_smoothCoroutine != null)
        {
            StopCoroutine(_smoothCoroutine);
            _smoothCoroutine = null;
        }
    }

    protected override void ApplyChanges(float current, float max)
    {
        if (_smoothCoroutine != null)
            StopCoroutine(_smoothCoroutine);

        _smoothCoroutine = StartCoroutine(SmoothChange(current));
    }

    private IEnumerator SmoothChange(float targetValue)
    {
        while (Mathf.Abs(_slider.value - targetValue) > 0.01f)
        {
            _slider.value = Mathf.MoveTowards(_slider.value, targetValue, _smoothSpeed * Time.deltaTime);
            yield return null;
        }

        _slider.value = targetValue;
        _smoothCoroutine = null;
    }
}