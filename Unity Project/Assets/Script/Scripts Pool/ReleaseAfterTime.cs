using System.Collections;
using UnityEngine;

namespace Scripts.ObjectPooling
{
    public class ReleaseAfterTime : MonoBehaviour
    {
        #region Fields

        [SerializeField] private float _releaseDelay = 3f;

        private Coroutine _releaseRoutine;

        #endregion

        #region Unity

        private void OnEnable()
        {
            StartReleaseRoutine();
        }

        private void OnDisable()
        {
            StopReleaseRoutine();
        }

        #endregion

        #region Release

        private void StartReleaseRoutine()
        {
            StopReleaseRoutine();
            _releaseRoutine = StartCoroutine(ReleaseRoutine());
        }

        private IEnumerator ReleaseRoutine()
        {
            yield return new WaitForSeconds(_releaseDelay);

            _releaseRoutine = null;
            PoolManager.ReleaseObject(gameObject);
        }

        private void StopReleaseRoutine()
        {
            if (_releaseRoutine == null)
                return;

            StopCoroutine(_releaseRoutine);
            _releaseRoutine = null;
        }

        #endregion
    }
}
