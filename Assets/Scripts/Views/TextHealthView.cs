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
            _health.Changed += WriteHealth;
            WriteHealth(_health.Current, _health.Max);
        }
    }

    private void OnDisable()
    {
        if (_health != null)        
            _health.Changed -= WriteHealth;        
    }
   
    private void WriteHealth(float current, float max)
    {
        _healthText.text = string.Format(_format, Mathf.RoundToInt(current), Mathf.RoundToInt(max));
    }    
}