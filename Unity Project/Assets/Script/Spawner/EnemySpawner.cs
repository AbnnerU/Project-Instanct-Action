using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.ObjectPooling;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AI;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Scripts.Spawner
{
    public class EnemySpawner : MonoBehaviour, ISpawner
    {
        public string Name => _spawnerName;

        [SerializeField] private string _spawnerName = "Enemy Spawner";
        [SerializeField] private bool _active = true;

        [Header("Addressable")]
        [SerializeField] private AssetReferenceGameObject _enemyPrefabReference;
        [SerializeField] private bool _warmPoolOnStart = true;
        [SerializeField] private int _warmPoolSize = 3;
        [SerializeField] private bool _resetHealthOnSpawn = true;

        [Header("Spawn")]
        [SerializeField] private List<Transform> _list_SpawnPoints = new List<Transform>();
        [SerializeField] private Transform _spawnParent;
        [SerializeField] private int _maxAliveEnemies = 1;
        [SerializeField] private int _spawnOnStart = 1;
        [SerializeField] private bool _autoSpawn = true;
        [SerializeField] private float _spawnInterval = 2f;
        [SerializeField] private bool _sampleNavMeshPosition = true;
        [SerializeField] private float _navMeshSampleRadius = 1f;

        [Header("Gizmos")]
        [SerializeField] private bool _drawGizmos = true;
        [SerializeField] private float _gizmoRadius = 0.3f;

        private readonly HashSet<GameObject> _hashSet_AliveEnemies = new HashSet<GameObject>();
        private readonly Dictionary<HealthBasicsEvents, Action> _dc_DeathHandlers = new Dictionary<HealthBasicsEvents, Action>();
        private readonly List<SpawnRequest> _list_PendingSpawnRequests = new List<SpawnRequest>();

        private Coroutine _spawnRoutine;
        private AsyncOperationHandle<GameObject> _enemyPrefabLoadHandle;
        private GameObject _loadedEnemyPrefab;
        private bool _isLoadingEnemyPrefab;
        private bool _hasWarmedPool;

        private void Start()
        {
            StartCoroutine(InitializeSpawner());
        }

        private void OnDisable()
        {
            if (_spawnRoutine != null)
            {
                StopCoroutine(_spawnRoutine);
                _spawnRoutine = null;
            }
        }

        private void OnDestroy()
        {
            foreach (KeyValuePair<HealthBasicsEvents, Action> deathHandler in _dc_DeathHandlers)
            {
                if (deathHandler.Key != null)
                    deathHandler.Key.OnDeath -= deathHandler.Value;
            }

            _dc_DeathHandlers.Clear();
            _hashSet_AliveEnemies.Clear();

            if (_enemyPrefabLoadHandle.IsValid())
                Addressables.Release(_enemyPrefabLoadHandle);
        }

        public void Spawn(Vector3 position)
        {
            if (_loadedEnemyPrefab == null)
            {
                QueueSpawnRequest(position, Quaternion.identity);
                StartLoadingEnemyPrefab();
                return;
            }

            SpawnEnemy(position, Quaternion.identity);
        }

        public void Enable()
        {
            _active = true;

            if (_autoSpawn && _spawnRoutine == null && gameObject.activeInHierarchy)
                _spawnRoutine = StartCoroutine(AutoSpawnRoutine());
        }

        public void Disable()
        {
            _active = false;

            if (_spawnRoutine != null)
            {
                StopCoroutine(_spawnRoutine);
                _spawnRoutine = null;
            }
        }

        public bool TrySpawn()
        {
            if (!_active) return false;

            if (_loadedEnemyPrefab == null)
            {
                StartLoadingEnemyPrefab();
                return false;
            }

            CleanupAliveEnemies();

            if (_hashSet_AliveEnemies.Count >= _maxAliveEnemies)
                return false;

            Transform spawnPoint = GetSpawnPoint();
            Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : transform.position;
            Quaternion spawnRotation = spawnPoint != null ? spawnPoint.rotation : transform.rotation;

            SpawnEnemy(spawnPosition, spawnRotation);
            return true;
        }

        private IEnumerator AutoSpawnRoutine()
        {
            while (_active)
            {
                TrySpawn();
                yield return new WaitForSeconds(_spawnInterval);
            }

            _spawnRoutine = null;
        }

        private IEnumerator InitializeSpawner()
        {
            yield return LoadEnemyPrefab();

            if (_loadedEnemyPrefab == null)
                yield break;

            WarmPoolIfNeeded();

            for (int i = 0; i < _spawnOnStart; i++)
                TrySpawn();

            if (_autoSpawn)
                _spawnRoutine = StartCoroutine(AutoSpawnRoutine());
        }

        private void StartLoadingEnemyPrefab()
        {
            if (_loadedEnemyPrefab != null || _isLoadingEnemyPrefab || _enemyPrefabReference == null)
                return;

            StartCoroutine(LoadEnemyPrefab());
        }

        private IEnumerator LoadEnemyPrefab()
        {
            if (_loadedEnemyPrefab != null || _isLoadingEnemyPrefab)
                yield break;

            if (_enemyPrefabReference == null)
            {
                Debug.LogWarning("Enemy prefab reference is null.", this);
                yield break;
            }

            _isLoadingEnemyPrefab = true;
            _enemyPrefabLoadHandle = _enemyPrefabReference.LoadAssetAsync<GameObject>();
            yield return _enemyPrefabLoadHandle;
            _isLoadingEnemyPrefab = false;

            if (_enemyPrefabLoadHandle.Status != AsyncOperationStatus.Succeeded || _enemyPrefabLoadHandle.Result == null)
            {
                Debug.LogWarning("Could not load enemy prefab addressable.", this);
                yield break;
            }

            _loadedEnemyPrefab = _enemyPrefabLoadHandle.Result;
            WarmPoolIfNeeded();
            SpawnPendingRequests();
        }

        private void WarmPoolIfNeeded()
        {
            if (_hasWarmedPool || !_warmPoolOnStart || _loadedEnemyPrefab == null || _warmPoolSize <= 0)
                return;

            PoolManager.WarmPool(_loadedEnemyPrefab, _warmPoolSize);
            _hasWarmedPool = true;
        }

        private GameObject SpawnEnemy(Vector3 position, Quaternion rotation)
        {
            if (_sampleNavMeshPosition && NavMesh.SamplePosition(position, out NavMeshHit hit, _navMeshSampleRadius, NavMesh.AllAreas))
                position = hit.position;

            if (_loadedEnemyPrefab == null)
                return null;

            GameObject enemyObject = PoolManager.SpawnObject(_loadedEnemyPrefab, position, rotation);

            if (_spawnParent != null)
                enemyObject.transform.SetParent(_spawnParent, true);

            _hashSet_AliveEnemies.Add(enemyObject);

            PrepareHealth(enemyObject);

            return enemyObject;
        }

        private void PrepareHealth(GameObject enemyObject)
        {
            HealthBasicsEvents healthEvents = enemyObject.GetComponent<HealthBasicsEvents>();

            if (healthEvents == null)
                healthEvents = enemyObject.GetComponentInChildren<HealthBasicsEvents>();

            if (healthEvents == null)
                return;

            if (!_dc_DeathHandlers.ContainsKey(healthEvents))
            {
                Action deathHandler = () => _hashSet_AliveEnemies.Remove(enemyObject);
                _dc_DeathHandlers.Add(healthEvents, deathHandler);
                healthEvents.OnDeath += deathHandler;
            }

            if (!_resetHealthOnSpawn)
                return;

            IHittable hittable = enemyObject.GetComponent<IHittable>();

            if (hittable == null)
                hittable = enemyObject.GetComponentInChildren<IHittable>();

            hittable?.ResetHealth();
        }


        private Transform GetSpawnPoint()
        {
            if (_list_SpawnPoints == null || _list_SpawnPoints.Count == 0)
                return null;

            int index = UnityEngine.Random.Range(0, _list_SpawnPoints.Count);
            return _list_SpawnPoints[index];
        }

        private void CleanupAliveEnemies()
        {
            _hashSet_AliveEnemies.RemoveWhere(enemyObject => enemyObject == null || !enemyObject.activeInHierarchy);
        }

        private void QueueSpawnRequest(Vector3 position, Quaternion rotation)
        {
            _list_PendingSpawnRequests.Add(new SpawnRequest(position, rotation));
        }

        private void SpawnPendingRequests()
        {
            if (_loadedEnemyPrefab == null || _list_PendingSpawnRequests.Count == 0)
                return;

            for (int i = 0; i < _list_PendingSpawnRequests.Count; i++)
                SpawnEnemy(_list_PendingSpawnRequests[i].Position, _list_PendingSpawnRequests[i].Rotation);

            _list_PendingSpawnRequests.Clear();
        }

        private void OnDrawGizmosSelected()
        {
            if (!_drawGizmos) return;

            Gizmos.color = Color.red;

            if (_list_SpawnPoints == null || _list_SpawnPoints.Count == 0)
            {
                DrawSpawnGizmo(transform.position);
                return;
            }

            for (int i = 0; i < _list_SpawnPoints.Count; i++)
            {
                if (_list_SpawnPoints[i] == null) continue;

                DrawSpawnGizmo(_list_SpawnPoints[i].position);
                Gizmos.DrawLine(transform.position, _list_SpawnPoints[i].position);
            }
        }

        private void DrawSpawnGizmo(Vector3 position)
        {
            Gizmos.DrawWireSphere(position, _gizmoRadius);

            if (_sampleNavMeshPosition)
                Gizmos.DrawWireSphere(position, _navMeshSampleRadius);
        }

        private readonly struct SpawnRequest
        {
            public readonly Vector3 Position;
            public readonly Quaternion Rotation;

            public SpawnRequest(Vector3 position, Quaternion rotation)
            {
                Position = position;
                Rotation = rotation;
            }
        }
    }
}
