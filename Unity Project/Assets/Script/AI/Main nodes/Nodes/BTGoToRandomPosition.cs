using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Scripts.BT
{
    public class BTGoToRandomPosition : BTnodeNavMeshAjust
    {
        private NavMeshAgent _agent;
        private Transform _agentTransform;
        private INavMeshAgentStateUpdate _agentMovementState;
        private Vector3 _maxMoveDistance;
        private float _distance;
        private float _updateInterval;
        private float _speed;
        private int _mask;
        private float _areaVerificationRadius;

        private int _currentWalkingMask;

        public BTGoToRandomPosition(NavMeshAgent agent, INavMeshAgentStateUpdate agentMovementState, Vector3 maxMoveDistance, float distance, float updateInterval, float speed, int mask, float areaVerificationRadius)
        {
            _agent = agent;
            _agentMovementState = agentMovementState;
            _maxMoveDistance = maxMoveDistance;
            _distance = distance;
            _updateInterval = updateInterval;
            _speed = speed;
            _mask = mask;
            _areaVerificationRadius = areaVerificationRadius;

            _agentTransform = agent.transform;
        }

        public override IEnumerator Run(BehaviorTree behaviorTree)
        {
            status = BTstatus.RUNNING;

            if (_agent == null)
            {
                status = BTstatus.FAILURE;
                yield break;
            }

            float x = Random.Range(-_maxMoveDistance.x, _maxMoveDistance.x);
            float y = Random.Range(-_maxMoveDistance.y, _maxMoveDistance.y);
            float z = Random.Range(-_maxMoveDistance.z, _maxMoveDistance.z);

            if (NavMesh.SamplePosition((new Vector3(x, y, z)) + _agentTransform.position, out NavMeshHit result, _areaVerificationRadius, _mask))
                _destinationPosition = result.position;
            else
            {
                status = BTstatus.FAILURE;
                yield break;
            }

            _agent.stoppingDistance = _distance;
            _agent.speed = _speed;

            yield return behaviorTree.StartCoroutine(PlaceAgentOnNavMesh(_agent, _agentTransform));
            if (status == BTstatus.FAILURE) yield break;
            yield return behaviorTree.StartCoroutine(InitialPathPoint(_agent, _agentTransform, _distance, _speed, _destinationPosition));
            if (status != BTstatus.RUNNING) yield break;

            _agent.isStopped = false;

            float currentUpdateInterval;

            while (_agent != null && _agent.enabled == true && _agent.isStopped == false)
            {
                currentUpdateInterval = 0;

                if (_agent.pathPending == false && _agent.remainingDistance < _distance )
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