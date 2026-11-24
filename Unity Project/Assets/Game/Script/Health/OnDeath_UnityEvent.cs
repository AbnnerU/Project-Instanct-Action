using UnityEngine.Events;

public class OnDeath_UnityEvent : OnDeathActionBase
{
    public UnityEvent UEDeath;
    protected override void HealthEvents_OnDeath() => UEDeath?.Invoke();
}
