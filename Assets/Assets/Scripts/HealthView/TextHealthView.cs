using UnityEngine;
using TMPro;
using System.Collections;

public class TextHealthView : MonoBehaviour
{
    private const float FullCycle = Mathf.PI * 2;

    [SerializeField] private Health _health;
    [SerializeField] private TMP_Text _healthText;
    [SerializeField] private string _format = "{0}/{1}";

    [Header("Juice Settings")]
    [Tooltip("Total duration of the effect in seconds")]
    [SerializeField] private float _totalDuration = 0.3f;
    [Tooltip("Initial wobble amplitude in degrees (first, strongest swings)")]
    [SerializeField] private float _initialAmplitude = 15f;
    [Tooltip("Final wobble amplitude in degrees (last, weakest jitter before stopping)")]
    [SerializeField] private float _finalAmplitude = 1f;
    [Tooltip("Initial oscillation frequency (slow, wide swings)")]
    [SerializeField] private float _initialFrequency = 8f;
    [Tooltip("Final oscillation frequency (fast jitter before stopping)")]
    [SerializeField] private float _finalFrequency = 15f;
    [Tooltip("Maximum text scale multiplier (1 = no change, 2 = twice the size)")]
    [SerializeField] private float _scaleMultiplier = 2f;
    [Tooltip("Scale decay speed (higher = faster return to normal size)")]
    [SerializeField] private float _scaleDecaySpeed = 4f;
    [Tooltip("Amplitude decay curve: how fast the swing range decreases (left to right: start → end of effect, bottom to top: amplitude strength)")]
    [SerializeField] private AnimationCurve _amplitudeCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    [Tooltip("Frequency ramp curve: how fast oscillations speed up (left to right: start → end of effect, bottom to top: frequency)")]
    [SerializeField] private AnimationCurve _frequencyCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [Tooltip("Time nonlinearity curve: early moments can last longer than later ones (left to right: real time, bottom to top: perceived progress)")]
    [SerializeField] private AnimationCurve _timeCurve = AnimationCurve.Linear(0, 0, 1, 1);

    private Vector3 _originalScale;
    private Quaternion _originalRotation;
    private Coroutine _juiceCoroutine;

    private void Awake()
    {
        _originalScale = _healthText.transform.localScale;
        _originalRotation = _healthText.transform.localRotation;
    }

    private void OnEnable()
    {
        if (_health != null)
        {
            _health.HealthChanged += WriteHealth;
            _health.Hitted += OnHitted;
            WriteHealth(_health.CurrentHealth, _health.MaxHealth);
        }
    }

    private void OnDisable()
    {
        if (_health != null)
        {
            _health.HealthChanged -= WriteHealth;
            _health.Hitted -= OnHitted;
        }

        if (_juiceCoroutine != null)
        {
            StopCoroutine(_juiceCoroutine);
            _healthText.transform.localScale = _originalScale;
            _healthText.transform.localRotation = _originalRotation;
        }
    }

    private void WriteHealth(float current, float max)
    {
        _healthText.text = string.Format(_format, Mathf.RoundToInt(current), Mathf.RoundToInt(max));
    }

    private void OnHitted()
    {
        if (_juiceCoroutine != null)
            StopCoroutine(_juiceCoroutine);

        _juiceCoroutine = StartCoroutine(CoinWobbleEffect());
    }

    private IEnumerator CoinWobbleEffect()
    {
        float elapsed = 0f;
        Transform textTransform = _healthText.transform;

        while (elapsed < _totalDuration)
        {
            elapsed += Time.deltaTime;

            float linearProgress = Mathf.Clamp01(elapsed / _totalDuration);
            float nonLinearProgress = _timeCurve.Evaluate(linearProgress);
            float amplitude = Mathf.Lerp(_initialAmplitude, _finalAmplitude, _amplitudeCurve.Evaluate(nonLinearProgress));
            float frequency = Mathf.Lerp(_initialFrequency, _finalFrequency, _frequencyCurve.Evaluate(nonLinearProgress));
            float phase = frequency * nonLinearProgress * FullCycle;
            float angle = Mathf.Cos(phase) * amplitude;
            float scaleProgress = 1 + (_scaleMultiplier - 1) * Mathf.Exp(-nonLinearProgress * _scaleDecaySpeed);

            textTransform.localRotation = _originalRotation * Quaternion.Euler(0, 0, angle);
            textTransform.localScale = _originalScale * scaleProgress;

            yield return null;
        }

        textTransform.localRotation = _originalRotation;
        textTransform.localScale = _originalScale;
    }
}