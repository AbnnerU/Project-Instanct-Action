
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Scripts.BT 
{
    public class BTGoToTarget : BTnodeNavMeshAjust
    {
        private Func<Transform> _funcTargetProvider;
        private IHasTargetProvider _targetProvider;
        private NavMeshAgent _agent;
        private Transform _agentTransform;
        private INavMeshAgentStateUpdate _agentMovementState;
        private float _distance;
        private float _updateInterval;
        private float _speed;

        private Transform _currentTarget;

        private int _currentWalkingMask;

        public BTGoToTarget(IHasTargetProvider targetProvider, NavMeshAgent agent, INavMeshAgentStateUpdate agentMovementState,  float distance, float updateInterval, float speed)
        {
            this._targetProvider = targetProvider;
            this._agent = agent;
            this._distance = distance;
            this._updateInterval = updateInterval;
            this._agentMovementState = agentMovementState;         
            this._speed = speed;

            _agentTransform = agent.transform;
        }

        public BTGoToTarget(Func<Transform> funcTargetProvider, NavMeshAgent agent, INavMeshAgentStateUpdate agentMovementState, float distance, float updateInterval, float speed)
        {
            _funcTargetProvider = funcTargetProvider;
            _agent = agent;
            _agentMovementState = agentMovementState;
            _distance = distance;
            _updateInterval = updateInterval;
            _speed = speed;

            _agentTransform = agent.transform;
        }

        public override IEnumerator Run(BehaviorTree behaviorTree)
        {
            status = BTstatus.RUNNING;

            if (_targetProvider != null)
                _currentTarget = _targetProvider.GetCurrentTarget();
            else if (_funcTargetProvider != null)
                _currentTarget = _funcTargetProvider();
            else
            {
                status = BTstatus.FAILURE;
                yield break;
            }


            if (_agent == null || _currentTarget == null || _currentTarget.gameObject.activeSelf == false)
            {
                status = BTstatus.FAILURE;

                yield break;
            }

            _destinationPosition = _currentTarget.position;

            _agent.stoppingDistance = _distance;
            _agent.speed = _speed;

            yield return behaviorTree.StartCoroutine(PlaceAgentOnNavMesh(_agent, _agentTransform));
            if (status == BTstatus.FAILURE) yield break;
            yield return behaviorTree.StartCoroutine(InitialPathPoint(_agent, _agentTransform, _distance, _speed, _destinationPosition));
            if (status != BTstatus.RUNNING) yield break;

            _agent.isStopped = false;

            float currentUpdateInterval;

            while (_currentTarget != null && 
                ((_targetProvider!=null && _currentTarget == _targetProvider.GetCurrentTarget()) || (_funcTargetProvider!=null && _currentTarget == _funcTargetProvider())) && 
                _currentTarget.gameObject.activeSelf == true && _agent != null && _agent.enabled == true && _agent.isStopped == false)
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

                if (_currentTarget != null)
                {
                    if(_agent == null || _agent.isOnNavMesh == false)
                    {
                        status = BTstatus.FAILURE;
                        yield break;
                    }

                    _agent.SetDestination(_currentTarget.position);
                }
                    

                yield return null;
            }

            status = BTstatus.FAILURE;

            yield break;
        }

    }
}

