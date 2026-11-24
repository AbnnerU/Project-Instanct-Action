using UnityEngine;

public class LifeCycle_HandleCollider : LifeCycleActionsBase
{
    [SerializeField] private Collider _colliderComponent;

    protected override void HealthEvents_OnDeath() => _colliderComponent.enabled = false;

    protected override void HealthEvents_OnRespawn() => _colliderComponent.enabled = true;
}
