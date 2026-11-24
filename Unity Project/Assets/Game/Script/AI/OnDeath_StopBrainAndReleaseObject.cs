using System.Collections;
using Scripts.BehaviourTree.Agents;
using Scripts.ObjectPooling;
using UnityEngine;
using UnityEngine.AI;

namespace Scripts.AI
{
    public class OnDeath_StopBrainAndReleaseObject : OnDeathActionBase
    {
        [SerializeField] private EnemyBrain _enemyBrainComponent;
        [SerializeField] private NavMeshAgent _navMeshAgentComponent;
        [SerializeField] private GameObject _objectToRelease;
        [SerializeField] private float _releaseDelay = 1.5f;
        [SerializeField] private bool _releaseObject = true;

        private Coroutine _releaseRoutine;

        protected override void Awake()
        {
            if (_enemyBrainComponent == null)
                TryGetComponent(out _enemyBrainComponent);

            if (_navMeshAgentComponent == null)
                TryGetComponent(out _navMeshAgentComponent);

            if (_objectToRelease == null)
                _objectToRelease = gameObject;

            base.Awake();
        }

        private void OnDisable()
        {
            if (_releaseRoutine != null)
            {
                StopCoroutine(_releaseRoutine);
                _releaseRoutine = null;
            }
        }

        protected override void HealthEvents_OnDeath()
        {
            StopEnemy();

            if (!_releaseObject)
                return;

            if (_releaseRoutine != null)
                StopCoroutine(_releaseRoutine);

            _releaseRoutine = StartCoroutine(ReleaseAfterDelay());
        }

        private IEnumerator ReleaseAfterDelay()
        {
            float currentTime = 0f;

            while (currentTime < _releaseDelay)
            {
                currentTime += Time.deltaTime;
                yield return null;
            }

            _releaseRoutine = null;

            PoolManager.ReleaseObject(_objectToRelease);
        }

        private void StopEnemy()
        {
            _enemyBrainComponent?.StopBehaviourTree();

            if (_navMeshAgentComponent == null || !_navMeshAgentComponent.enabled || !_navMeshAgentComponent.isOnNavMesh)
                return;

            _navMeshAgentComponent.isStopped = true;
            _navMeshAgentComponent.velocity = Vector3.zero;
            _navMeshAgentComponent.ResetPath();
        }
    }
}
