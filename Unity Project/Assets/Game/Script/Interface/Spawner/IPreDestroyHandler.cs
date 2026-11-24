using System;
using UnityEngine;

namespace Scripts.Spawner
{
    public interface IPreDestroyHandler
    {
        event Action<GameObject> OnBeforeDestroy;
    }
}