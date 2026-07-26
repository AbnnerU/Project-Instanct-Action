using System;
using System.Collections;
using UnityEngine;

namespace Scripts.BT
{
    public class BTLookAtTarget : BTnode
    {
        private Func<Transform> _targetProvider;
        private Transform _transformToLook;
        private bool _onlyY;

        public BTLookAtTarget(Func<Transform> targetToAtLook, Transform transformToAffect, bool onlyY)
        {
            _targetProvider = targetToAtLook;
            _transformToLook = transformToAffect;
            _onlyY = onlyY;
        }

        public override IEnumerator Run(BehaviorTree behaviorTree)
        {
            status = BTstatus.RUNNING;

            if(_transformToLook != null && _targetProvider != null && _targetProvider() != null)
            {
                if (_onlyY)
                {
                    Vector3 dir = _targetProvider().position - _transformToLook.position;

                    dir.y = 0;

                    _transformToLook.rotation = Quaternion.LookRotation(dir);

                    status = BTstatus.SUCCESS;

                    yield break;
                }
                else
                    _transformToLook.LookAt(_transformToLook.position);     
            }

            status = BTstatus.FAILURE;

            yield break;
        }
    }
}
