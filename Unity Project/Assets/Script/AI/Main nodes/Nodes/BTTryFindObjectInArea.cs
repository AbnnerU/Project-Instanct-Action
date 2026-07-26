using System;
using System.Collections;
using UnityEngine;

namespace Scripts.BT
{
    public class BTTryFindObjectInArea<T> : BTnode
    {
        private Action<T> _callback;
        private Collider[] _results;
        private Transform _transform;
        private Func<Vector3> _startPositionValueProvider;
        private Func<T, bool> _exclusionPredicate;
        private LayerMask _mask;
        private float _areaRadius;
        private string[] _tags;

        private Vector3 _position;
        private QueryTriggerInteraction _triggerInteractionOverride;
        public BTTryFindObjectInArea(Collider[] results, Transform transform, LayerMask mask, float areaRadius, string[] tags)
        {
            _results = results;
            _transform = transform;
            _mask = mask;
            _areaRadius = areaRadius;
            _tags = tags;
            _triggerInteractionOverride = QueryTriggerInteraction.UseGlobal;
        }

        public BTTryFindObjectInArea(Collider[] results, Func<Vector3> startPositionValueProvider, LayerMask mask, float areaRadius, string[] tags)
        {
            _results = results;
            _startPositionValueProvider = startPositionValueProvider;
            _mask = mask;
            _areaRadius = areaRadius;
            _tags = tags;
            _triggerInteractionOverride = QueryTriggerInteraction.UseGlobal;
        }

        public BTTryFindObjectInArea(Collider[] results, Transform transform, LayerMask mask, float areaRadius, string[] tags, Action<T> onFoundCallback)
        {
            _results = results;
            _transform = transform;
            _mask = mask;
            _areaRadius = areaRadius;
            _tags = tags;
            _callback = onFoundCallback;
            _triggerInteractionOverride = QueryTriggerInteraction.UseGlobal;
        }

        public BTTryFindObjectInArea(Collider[] results, Transform transform, LayerMask mask, float areaRadius, string[] tags, Action<T> onFoundCallback, QueryTriggerInteraction queryTriggerInteraction)
        {
            _results = results;
            _transform = transform;
            _mask = mask;
            _areaRadius = areaRadius;
            _tags = tags;
            _callback = onFoundCallback;
            _triggerInteractionOverride = queryTriggerInteraction;
        }

        public BTTryFindObjectInArea(Collider[] results, Func<Vector3> startPositionValueProvider, LayerMask mask, float areaRadius, string[] tags, Action<T> onFoundCallback)
        {
            _results = results;
            _startPositionValueProvider = startPositionValueProvider;
            _mask = mask;
            _areaRadius = areaRadius;
            _tags = tags;
            _callback = onFoundCallback;
            _triggerInteractionOverride = QueryTriggerInteraction.UseGlobal;
        }

        public BTTryFindObjectInArea(Collider[] results, Transform transform, LayerMask mask, float areaRadius, string[] tags, Action<T> onFoundCallback, Func<T, bool> exclusionPredicate)
        {
            _results = results;
            _transform = transform;
            _mask = mask;
            _areaRadius = areaRadius;
            _tags = tags;
            _callback = onFoundCallback;
            _exclusionPredicate = exclusionPredicate;
            _triggerInteractionOverride = QueryTriggerInteraction.UseGlobal;
        }

        public BTTryFindObjectInArea(Collider[] results, Transform transform, LayerMask mask, float areaRadius, string[] tags, Action<T> onFoundCallback, Func<T, bool> exclusionPredicate, QueryTriggerInteraction queryTriggerInteraction)
        {
            _results = results;
            _transform = transform;
            _mask = mask;
            _areaRadius = areaRadius;
            _tags = tags;
            _callback = onFoundCallback;
            _exclusionPredicate = exclusionPredicate;
            _triggerInteractionOverride = queryTriggerInteraction;
        }


        public BTTryFindObjectInArea(Collider[] results, Func<Vector3> startPositionValueProvider, LayerMask mask, float areaRadius, string[] tags, Action<T> onFoundCallback, Func<T, bool> exclusionPredicate)
        {
            _results = results;
            _startPositionValueProvider = startPositionValueProvider;
            _mask = mask;
            _areaRadius = areaRadius;
            _tags = tags;
            _callback = onFoundCallback;
            _exclusionPredicate = exclusionPredicate;
            _triggerInteractionOverride = QueryTriggerInteraction.UseGlobal;
        }

        public BTTryFindObjectInArea(Collider[] results, Func<Vector3> startPositionValueProvider, LayerMask mask, float areaRadius, string[] tags, Action<T> onFoundCallback, Func<T, bool> exclusionPredicate, QueryTriggerInteraction queryTriggerInteraction)
        {
            _results = results;
            _startPositionValueProvider = startPositionValueProvider;
            _mask = mask;
            _areaRadius = areaRadius;
            _tags = tags;
            _callback = onFoundCallback;
            _exclusionPredicate = exclusionPredicate;
            _triggerInteractionOverride = queryTriggerInteraction;
        }

        public override IEnumerator Run(BehaviorTree behaviorTree)
        {
            status = BTstatus.RUNNING;

            if (_transform)
                _position = _transform.position;
            else if (_startPositionValueProvider != null)
                _position = _startPositionValueProvider();
            else
            {
                status = BTstatus.FAILURE;
                yield break;
            }


            int hits = Physics.OverlapSphereNonAlloc(_position, _areaRadius, _results, _mask, _triggerInteractionOverride);

            if (hits > 0)
            {
                for (int i = 0; i < hits; i++)
                {
                    Collider c = _results[i];

                    bool findTag = false;

                    foreach (var t in _tags)
                    {
                        if (c.CompareTag(t))
                        {
                            findTag = true;
                            break;
                        }
                    }

                    if (findTag == false) continue;
                    
                    if(typeof(T) == typeof(GameObject))
                    {
                        if (_exclusionPredicate != null && _exclusionPredicate((T)(object)c.gameObject)) continue;

                        _callback?.Invoke((T)(object)c.gameObject);
                    }
                    else
                    {
                        if (!c.TryGetComponent<T>(out var comp)) continue;

                        if (_exclusionPredicate != null && _exclusionPredicate(comp)) continue;

                        _callback?.Invoke(comp);
                    }

                    status = BTstatus.SUCCESS;
                    yield break;
                }
            }

            status = BTstatus.FAILURE;

            yield break;
        }
    }
}