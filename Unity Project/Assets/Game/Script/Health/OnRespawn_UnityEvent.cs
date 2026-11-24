using UnityEngine;
using UnityEngine.Events;

public class OnRespawn_UnityEvent : MonoBehaviour
{
    public UnityEvent UERespawn;

    [SerializeField] private HealthBasicsEvents _healthBasicsEvents;

    private void Awake() => _healthBasicsEvents.OnRespawn += Execute;

    private void OnDestroy() => _healthBasicsEvents.OnRespawn -= Execute;

    private void Execute() => UERespawn?.Invoke();
}