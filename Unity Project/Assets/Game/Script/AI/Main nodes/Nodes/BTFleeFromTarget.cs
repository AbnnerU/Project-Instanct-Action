using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Scripts.BT
{
    public class BTFleeFromTarget : BTnodeNavMeshAjust
    {
        private Func<Transform> _targetProvider;
        private NavMeshAgent _agent;
        private Transform _agentTransform;
        private INavMeshAgentStateUpdate _agentMovementState;
        private float _fleeDistance;
        private float _sampleRadius;
        private float _stopDistance;
        private float _updateInterval;
        private float _speed;
        private int _mask;

        private int _currentWalkingMask;

        public BTFleeFromTarget(Func<Transform> targetProvider, NavMeshAgent agent, INavMeshAgentStateUpdate agentMovementState,
            float fleeDistance, float sampleRadius, float stopDistance, float updateInterval, float speed, int mask)
        {
            _targetProvider = targetProvider;
            _agent = agent;
            _agentMovementState = agentMovementState;
            _fleeDistance = fleeDistance;
            _sampleRadius = sampleRadius;
            _stopDistance = stopDistance;
            _updateInterval = updateInterval;
            _speed = speed;
            _mask = mask;

            _agentTransform = agent.transform;
        }

        public override IEnumerator Run(BehaviorTree behaviorTree)
        {
            status = BTstatus.RUNNING;

            Transform threat = _targetProvider?.Invoke();

            if (_agent == null || threat == null || !threat.gameObject.activeInHierarchy)
            {
                status = BTstatus.FAILURE;
                yield break;
            }

            Vector3 fleeDirection = _agentTransform.position - threat.position;
            fleeDirection.y = 0f;

            if (fleeDirection.sqrMagnitude <= 0.01f)
                fleeDirection = -threat.forward;

            fleeDirection.y = 0f;

            if (fleeDirection.sqrMagnitude <= 0.01f)
                fleeDirection = -_agentTransform.forward;

            fleeDirection.Normalize();

            Vector3 desiredPosition = _agentTransform.position + (fleeDirection * _fleeDistance);

            if (NavMesh.SamplePosition(desiredPosition, out NavMeshHit result, _sampleRadius, _mask))
                _destinationPosition = result.position;
            else
            {
                status = BTstatus.FAILURE;
                yield break;
            }

            _agent.stoppingDistance = _stopDistance;
            _agent.speed = _speed;

            yield return behaviorTree.StartCoroutine(PlaceAgentOnNavMesh(_agent, _agentTransform));
            if (status == BTstatus.FAILURE) yield break;

            yield return behaviorTree.StartCoroutine(InitialPathPoint(_agent, _agentTransform, _stopDistance, _speed, _destinationPosition));
            if (status != BTstatus.RUNNING) yield break;

            _agent.isStopped = false;

            float currentUpdateInterval;

            while (_agent != null && _agent.enabled && !_agent.isStopped && _agent.isOnNavMesh)
            {
                if (!_agent.pathPending && _agent.remainingDistance <= _stopDistance)
                {
                    status = BTstatus.SUCCESS;
                    yield break;
                }

                currentUpdateInterval = 0f;

                do
                {
                    currentUpdateInterval += Time.deltaTime;

                    if (NavMesh.SamplePosition(_agentTransform.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
                        _currentWalkingMask = hit.mask;

                    _agentMovementState.AgentUpdate(_agent.isOnOffMeshLink, _currentWalkingMask, _agent.velocity);

                    if (_agent.isOnOffMeshLink)
                    {
                        do
                        {
                            yield return null;
                        } while (_agent.isOnOffMeshLink);
                    }

                    yield return null;
                } while (currentUpdateInterval < _updateInterval);

                yield return null;
            }

            status = BTstatus.FAILURE;
        }
    }
}
