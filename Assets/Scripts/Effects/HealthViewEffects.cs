using UnityEngine;
using TMPro;
using System.Collections;

public class HealthViewEffects : MonoBehaviour
{
    private const float FullCycle = Mathf.PI * 2;

    [Header("Target")]
    [SerializeField] private Health _health;
    [SerializeField] private Transform _target;
    [SerializeField] private TMP_Text _targetText;

    [Header("Damage Juice Settings")]
    [Tooltip("Total duration of the damage effect in seconds")]
    [SerializeField] private float _damageTotalDuration = 0.3f;

    [Tooltip("Initial wobble amplitude in degrees (first, strongest swings)")]
    [SerializeField] private float _damageInitialAmplitude = 15f;

    [Tooltip("Final wobble amplitude in degrees (last, weakest jitter before stopping)")]
    [SerializeField] private float _damageFinalAmplitude = 1f;

    [Tooltip("Initial oscillation frequency (slow, wide swings)")]
    [SerializeField] private float _damageInitialFrequency = 8f;

    [Tooltip("Final oscillation frequency (fast jitter before stopping)")]
    [SerializeField] private float _damageFinalFrequency = 15f;

    [Tooltip("Maximum text scale multiplier on damage (1 = no change, 2 = twice the size)")]
    [SerializeField] private float _damageScaleMultiplier = 2f;

    [Tooltip("Scale decay speed on damage (higher = faster return to normal size)")]
    [SerializeField] private float _damageScaleDecaySpeed = 4f;

    [Tooltip("Amplitude decay curve: how fast the swing range decreases")]
    [SerializeField] private AnimationCurve _damageAmplitudeCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    [Tooltip("Frequency ramp curve: how fast oscillations speed up")]
    [SerializeField] private AnimationCurve _damageFrequencyCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Tooltip("Time nonlinearity curve: early moments can last longer than later ones")]
    [SerializeField] private AnimationCurve _damageTimeCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Header("Heal Juice Settings")]
    [Tooltip("Total duration of the heal effect in seconds")]
    [SerializeField] private float _healTotalDuration = 0.5f;

    [Tooltip("Maximum scale when expanding on heal (1 = no change)")]
    [SerializeField] private float _healScaleMultiplier = 1.8f;

    [Tooltip("Portion of total duration before ripples start (0-1, where 0.1 = first 10% is ripple-free expansion)")]
    [SerializeField] private float _healRippleDelay = 0.1f;

    [Tooltip("Minimum scale when contracting after heal (1 = no change)")]
    [SerializeField] private float _healScaleMinMultiplier = 0.9f;

    [Tooltip("Number of ripple oscillations after the initial splash")]
    [SerializeField] private int _healRippleCount = 2;

    [Tooltip("How quickly the ripples fade (higher = faster decay)")]
    [SerializeField] private float _healRippleDecay = 3f;

    [Tooltip("Heal effect animation curve (controls the overall scale progression)")]
    [SerializeField] private AnimationCurve _healScaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Vector3 _originalScale;
    private Quaternion _originalRotation;
    private FontStyles _originalFontStyle;
    private Coroutine _damageCoroutine;
    private Coroutine _healCoroutine;
    private float _lastHealth;

    private Transform Target => _target != null ? _target : transform;

    private void Awake()
    {
        _originalScale = Target.localScale;
        _originalRotation = Target.localRotation;
        _lastHealth = _health.Current;

        if (_targetText != null)
            _originalFontStyle = _targetText.fontStyle;
    }

    private void OnEnable()
    {
        if (_health != null)        
            _health.Changed += ApplyEffect;        
    }

    private void OnDisable()
    {
        if (_health != null)        
            _health.Changed -= ApplyEffect;        

        StopAllJuiceEffects();
        Target.localScale = _originalScale;
        Target.localRotation = _originalRotation;

        if (_targetText != null)
            _targetText.fontStyle = _originalFontStyle;
    }

    private void StopAllJuiceEffects()
    {
        if (_damageCoroutine != null)
        {
            StopCoroutine(_damageCoroutine);
            _damageCoroutine = null;
        }

        if (_healCoroutine != null)
        {
            StopCoroutine(_healCoroutine);
            _healCoroutine = null;
        }
    }

    private void ApplyEffect(float current, float max)
    {
        StopAllJuiceEffects();

        if (current >= _lastHealth)
            _healCoroutine = StartCoroutine(WaterDropEffect());
        else
            _damageCoroutine = StartCoroutine(CoinWobbleEffect());

        _lastHealth = current;
    }

    private IEnumerator CoinWobbleEffect()
    {
        float elapsed = 0f;

        while (elapsed < _damageTotalDuration)
        {
            elapsed += Time.deltaTime;

            float linearProgress = Mathf.Clamp01(elapsed / _damageTotalDuration);
            float nonLinearProgress = _damageTimeCurve.Evaluate(linearProgress);
            float amplitude = Mathf.Lerp(_damageInitialAmplitude, _damageFinalAmplitude, _damageAmplitudeCurve.Evaluate(nonLinearProgress));
            float frequency = Mathf.Lerp(_damageInitialFrequency, _damageFinalFrequency, _damageFrequencyCurve.Evaluate(nonLinearProgress));
            float phase = frequency * nonLinearProgress * FullCycle;
            float angle = Mathf.Cos(phase) * amplitude;
            float scaleProgress = 1 + (_damageScaleMultiplier - 1) * Mathf.Exp(-nonLinearProgress * _damageScaleDecaySpeed);

            _target.localRotation = _originalRotation * Quaternion.Euler(0, 0, angle);
            _target.localScale = _originalScale * scaleProgress;

            yield return null;
        }

        _target.localRotation = _originalRotation;
        _target.localScale = _originalScale;
    }

    private IEnumerator WaterDropEffect()
    {
        float elapsed = 0f;
        
        while (elapsed < _healTotalDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / _healTotalDuration);
            float baseScale = _healScaleCurve.Evaluate(progress);
            float rippleScale = 0f;

            if (progress > _healRippleDelay)
            {
                float rippleProgress = (progress - _healRippleDelay) / (1 - _healRippleDelay);
                float ripplePhase = rippleProgress * _healRippleCount * FullCycle;
                float rippleAmplitude = Mathf.Exp(-rippleProgress * _healRippleDecay);
                rippleScale = Mathf.Sin(ripplePhase) * rippleAmplitude * (_healScaleMinMultiplier - 1);
            }

            float targetScale = Mathf.Lerp(_originalScale.x, _healScaleMultiplier, baseScale) + rippleScale;
            float scaleProgress = Mathf.Max(_healScaleMinMultiplier, targetScale);

            _target.localScale = _originalScale * scaleProgress;

            yield return null;
        }

        _target.localScale = _originalScale;
    }
}