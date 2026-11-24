using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Scripts.BT
{
    public abstract class BTnodeNavMeshAjust : BTnode
    {
        protected Vector3 _destinationPosition;

        public IEnumerator PlaceAgentOnNavMesh(NavMeshAgent agent, Transform agentTransform)
        {
            if (agent.isOnNavMesh == false || agent.enabled == false)
            {
                float placementAttempts = 0;

                agent.enabled = true;

                do
                {
                    placementAttempts++;

                    if (placementAttempts > 1)
                    {
                        NavMeshHit hit;
                        if (NavMesh.SamplePosition(agentTransform.position, out hit, 100f, NavMesh.AllAreas))
                        {
                            Debug.Log("Agent repositioned");
                            agent.Warp(hit.position);

                        }
                        else
                        {
                            Debug.LogError("Can't place agent in Navmesh (" + agent.gameObject.name + ")");
                            status = BTstatus.FAILURE;
                            yield break;
                        }

                    }

                    yield return null;

                } while (agent.isOnNavMesh == false);
            }

            yield break;
        }

        public IEnumerator InitialPathPoint(NavMeshAgent agent, Transform agentTransform, float distance, float speed, Vector3 initialValue)
        {
            if (agent == null || agent.isOnNavMesh == false)
            {
                status = BTstatus.FAILURE;
                yield break;
            }

            _destinationPosition = initialValue;
          
            if((_destinationPosition - agentTransform.position).magnitude <= distance)
            {
                status = BTstatus.SUCCESS;
                yield break;
            }

            agent.stoppingDistance = distance;
            agent.speed = speed;
            agent.SetDestination(_destinationPosition);

            while (agent.pathPending == true)
                yield return null;

            if (agent.remainingDistance <= distance && (_destinationPosition - agentTransform.position).magnitude > distance)
            {
                float value = 5;
                float addValue = 5;
                float maxValue = 20;
                do
                {
                    if (value >= maxValue || agent == null || agent.isOnNavMesh == false)
                    {
                        status = BTstatus.FAILURE;
                        yield break;
                    }

                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(_destinationPosition, out hit, value, NavMesh.AllAreas))
                    {
                        _destinationPosition = hit.position;
                        break;
                    }

                    yield return null;
                    value += addValue;
                } while (agent.remainingDistance <= distance && (_destinationPosition - agentTransform.position).magnitude > distance);
            }

            if (agent == null || agent.isOnNavMesh == false)
            {
                status = BTstatus.FAILURE;
                yield break;
            }

            if (_destinationPosition != initialValue)
            {
                agent.SetDestination(_destinationPosition);

                while (agent.pathPending == true)
                    yield return null;

                if (agent == null || agent.isOnNavMesh == false)
                {
                    status = BTstatus.FAILURE;
                    yield break;
                }
            }

            yield break;
        }
    }
}