using System;
using UnityEngine;

namespace Scripts.Spawner
{
    public interface IHasSpawnValidation 
    {
        event Func<GameObject, bool> OnRequestValidationToSpawn;
    }
}