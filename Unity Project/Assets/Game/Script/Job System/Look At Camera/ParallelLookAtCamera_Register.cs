using UnityEngine;

namespace Scripts.Job.LookAtCamera
{
    public class ParallelLookAtCamera_Register : MonoBehaviour
    {
        [SerializeField] private Transform _transformRef;

        private void OnEnable()
        {
            if (MN_ParallelLookAtCamera.Instance != null)
                MN_ParallelLookAtCamera.Instance.TryAddAgent(_transformRef);         
        }


        private void OnDisable()
        {
            if (MN_ParallelLookAtCamera.Instance != null)
                MN_ParallelLookAtCamera.Instance.TryRemoveAgent(_transformRef);
        }

        private void OnDestroy()
        {
            if (MN_ParallelLookAtCamera.Instance != null)
                MN_ParallelLookAtCamera.Instance.TryRemoveAgent(_transformRef);
        }
    }
}