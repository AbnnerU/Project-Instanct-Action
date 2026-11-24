using System;

namespace Scripts.Spawner
{
    public interface IAddressableButtonSpawner
    {
        public event Action OnButtonsSpawned;
    }
}