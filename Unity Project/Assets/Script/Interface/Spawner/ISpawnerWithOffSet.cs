using UnityEngine;

namespace Scripts.Spawner
{
    public interface ISpawnerWithOffSet : ISpawner
    {
        void SetOffSet(Vector3 offSet);
    }
}