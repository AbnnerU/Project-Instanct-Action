using System.Collections;
using Scripts.BehaviourTree.Agents;
using UnityEngine;
using UnityEngine.AI;

namespace Scripts.AI
{
    public class HitReaction_BlockNavMeshAgent : MonoBehaviour
    {
        [SerializeField] private GameObject _hittableReference;
        [SerializeField] private NavMeshAgent _navMeshAgentComponent;
        [SerializeField] private EnemyBrain _enemyBrainComponent;
        [SerializeField] private float _blockTime = 0.25f;
        [SerializeField] private bool _resetPath = true;

        private Coroutine _blockAgentRoutine;
        private IHittable _hittableHandler;

        private void Reset()
        {
            TryGetComponent(out _navMeshAgentComponent);
            TryGetComponent(out _enemyBrainComponent);
            _hittableReference = gameObject;
        }

        private void Start()
        {
            if (_hittableReference == null)
                _hittableReference = gameObject;

            if (_navMeshAgentComponent == null)
                TryGetComponent(out _navMeshAgentComponent);

            if (_enemyBrainComponent == null)
                TryGetComponent(out _enemyBrainComponent);

            _hittableHandler = _hittableReference.GetComponent<IHittable>();

            if (_hittableHandler != null)
                _hittableHandler.OnHitted += OnHitted;
        }

        private void OnDestroy()
        {
            if (_hittableHandler != null)
                _hittableHandler.OnHitted -= OnHitted;
        }

        private void OnHitted(int damage)
        {
            if (_blockAgentRoutine != null)
                StopCoroutine(_blockAgentRoutine);

            _blockAgentRoutine = StartCoroutine(BlockAgent());
        }

        private IEnumerator BlockAgent()
        {
            float currentTime = 0f;

            _enemyBrainComponent?.StopBehaviourTree();

            do
            {
                StopAgent();
                currentTime += Time.deltaTime;
                yield return null;
            } while (currentTime < _blockTime);

            _blockAgentRoutine = null;
            _enemyBrainComponent?.StartBehaviourTree();
        }

        private void StopAgent()
        {
            if (_navMeshAgentComponent == null || !_navMeshAgentComponent.enabled || !_navMeshAgentComponent.isOnNavMesh)
                return;

            _navMeshAgentComponent.isStopped = true;
            _navMeshAgentComponent.velocity = Vector3.zero;

            if (_resetPath)
                _navMeshAgentComponent.ResetPath();
        }
    }
}
