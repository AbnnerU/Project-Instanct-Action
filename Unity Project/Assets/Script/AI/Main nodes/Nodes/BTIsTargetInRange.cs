using System;
using System.Collections;
using UnityEngine;

namespace Scripts.BT
{
    public class BTIsTargetInRange : BTnode
    {
        private readonly Func<Transform> _func_TargetProvider;
        private readonly Transform _origin;
        private readonly float _range;
        private readonly bool _useXZDistance;

        public BTIsTargetInRange(Func<Transform> targetProvider, Transform origin, float range, bool useXZDistance)
        {
            _func_TargetProvider = targetProvider;
            _origin = origin;
            _range = range;
            _useXZDistance = useXZDistance;
        }

        public override IEnumerator Run(BehaviorTree behaviorTree)
        {
            status = BTstatus.RUNNING;

            Transform target = _func_TargetProvider?.Invoke();

            if (_origin == null || target == null)
            {
                status = BTstatus.FAILURE;
                yield break;
            }

            Vector3 difference = target.position - _origin.position;

            if (_useXZDistance)
                difference.y = 0f;

            status = difference.magnitude <= _range ? BTstatus.SUCCESS : BTstatus.FAILURE;
        }
    }
}
