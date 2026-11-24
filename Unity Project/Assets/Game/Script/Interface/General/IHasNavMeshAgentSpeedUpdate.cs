
using System;

public interface IHasNavMeshAgentSpeedUpdate
{
    public event Action<float> OnAgentSpeedUpdate;
}