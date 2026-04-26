public class SimpleHealthBarView : HealthBarView
{
    protected override void ApplyChanges(float current, float max)
    {
        _slider.value = current;
    }
}