using UnityEngine;

namespace Scripts.Spawner
{
    public interface ISpawner:IHasActiveState
    {
        string Name { get; }

        void Spawn(Vector3 position);
    }
}