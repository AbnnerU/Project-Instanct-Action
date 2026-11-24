using UnityEngine;

namespace Scripts.CameraSystem.CameraFunctions
{
    public interface ICameraFollow
    {
        void StartFollowing(Transform target, Vector3 offSet);
        void StopFollowing(Transform targetToStop = null);
    }
}