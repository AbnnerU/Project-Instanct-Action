using System;
using System.Collections;
using UnityEngine;

namespace Scripts.BT
{
    public class BTIsTargetAlive : BTnode
    {
        private readonly Func<Transform> _targetProvider;
        private readonly Action _onTargetUnavailable;

        public BTIsTargetAlive(Func<Transform> targetProvider, Action onTargetUnavailable = null)
        {
            _targetProvider = targetProvider;
            _onTargetUnavailable = onTargetUnavailable;
        }

        public override IEnumerator Run(BehaviorTree behaviorTree)
        {
            Transform target = _targetProvider?.Invoke();
            IHittable health = target != null ? target.GetComponentInParent<IHittable>() : null;
            bool alive = target != null && target.gameObject.activeInHierarchy && health != null && health.IsAlive();
            status = alive ? BTstatus.SUCCESS : BTstatus.FAILURE;
            if (!alive) _onTargetUnavailable?.Invoke();
            yield break;
        }
    }
}
