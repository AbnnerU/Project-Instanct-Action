using UnityEngine;

public interface INavMeshAgentStateUpdate 
{
    public void AgentUpdate(bool isOnNavMeshLink, int areaMask, Vector3 velocity);
}
