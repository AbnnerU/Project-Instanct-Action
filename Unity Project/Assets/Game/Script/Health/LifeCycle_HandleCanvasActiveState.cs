using UnityEngine;

public class LifeCycle_HandleCanvasActiveState : LifeCycleActionsBase
{
    [SerializeField] private Canvas _canvas;

    protected override void HealthEvents_OnDeath() => _canvas.enabled = false;

    protected override void HealthEvents_OnRespawn() => _canvas.enabled = true;
}
