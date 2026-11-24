
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Scripts.BT
{
    public class BTGoToPosition : BTnodeNavMeshAjust
    {
        private NavMeshAgent _agent;
        private Transform _agentTransform;
        private INavMeshAgentStateUpdate _agentMovementState;
        private IHasPositionProvider _positionProvider;
        private Func<Vector3> _funcPositionProvider;
        private float _distance;
        private float _updateInterval;
        private float _speed;

        private int _currentWalkingMask;
        public BTGoToPosition(IHasPositionProvider positionProvider, NavMeshAgent agent, INavMeshAgentStateUpdate agentMovementState, float distance, float updateInterval, float speed)
        {
            this._positionProvider = positionProvider;
            this._agent = agent;
            this._distance = distance;
            this._updateInterval = updateInterval;
            this._agentMovementState = agentMovementState;

            _agentTransform = agent.transform;
            this._speed = speed;
        }

        public BTGoToPosition(Func<Vector3> funcPositionProvider, NavMeshAgent agent, INavMeshAgentStateUpdate agentMovementState, float distance, float updateInterval, float speed)
        {
            this._funcPositionProvider = funcPositionProvider;
            this._agent = agent;
            this._distance = distance;
            this._updateInterval = updateInterval;
            this._agentMovementState = agentMovementState;

            _agentTransform = agent.transform;
            this._speed = speed;
        }


        public override IEnumerator Run(BehaviorTree behaviorTree)
        {
            status = BTstatus.RUNNING;

            if (_agent == null)
            {
                status = BTstatus.FAILURE;
                yield break;
            }

            if (_positionProvider != null)
                _destinationPosition = _positionProvider.GetCurrentPositionProvided();
            else if (_funcPositionProvider != null)
                _destinationPosition = _funcPositionProvider();
            else
            {
                status = BTstatus.FAILURE;
                yield break;
            }

            if (_destinationPosition == null)
            {
                status = BTstatus.FAILURE;
                yield break;
            }

            _agent.stoppingDistance = _distance;
            _agent.speed = _speed;

            yield return behaviorTree.StartCoroutine(PlaceAgentOnNavMesh(_agent, _agentTransform));
            if(status == BTstatus.FAILURE) yield break;
            yield return behaviorTree.StartCoroutine(InitialPathPoint(_agent, _agentTransform, _distance, _speed, _destinationPosition));
            if (status != BTstatus.RUNNING) yield break;

            _agent.isStopped = false;

            float currentUpdateInterval;

            while ((_positionProvider != null || _funcPositionProvider != null) && (_destinationPosition == _positionProvider?.GetCurrentPositionProvided() || _destinationPosition == _funcPositionProvider()) &&
                _agent != null && _agent.enabled == true && _agent.isStopped == false && _agent.isOnNavMesh)
            {
                currentUpdateInterval = 0;
                if (_agent.pathPending == false && _agent.remainingDistance < _distance)
                {
                    status = BTstatus.SUCCESS;

                    yield break;
                }
                else
                {
                    do
                    {
                        currentUpdateInterval += Time.deltaTime;

                        if (NavMesh.SamplePosition(_agentTransform.position, out var hit, 1.0f, NavMesh.AllAreas))
                            _currentWalkingMask = hit.mask;

                        _agentMovementState.AgentUpdate(_agent.isOnOffMeshLink, _currentWalkingMask, _agent.velocity);

                        if (_agent.isOnOffMeshLink)
                        {
                            do
                            {
                                Debug.Log("Agent On Nav Mesh Link");
                                yield return null;

                            } while (_agent.isOnOffMeshLink);
                        }

                        yield return null;
                    } while (currentUpdateInterval < _updateInterval);
                }

                yield return null;
            }

            status = BTstatus.FAILURE;

            yield break;
        }

    }
}