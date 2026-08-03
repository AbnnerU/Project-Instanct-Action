using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.ObjectPooling;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AI;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Scripts.Waves
{
    public class MN_Waves : SceneSingleton<MN_Waves>
    {
        [SerializeField] private bool _active = true;

        [Header("Addressables")]
        [SerializeField] private AssetReferenceGameObject[] _array_EnemyPrefabReferences;
        [SerializeField] private bool _warmPoolOnStart = true;
        [SerializeField] private int _warmPoolSizeByPrefab = 3;
        [SerializeField] private bool _resetHealthOnSpawn = true;

        [Header("Spawn")]
        [SerializeField] private Transform[] _array_SpawnPoints;
        [SerializeField] private Transform _spawnParent;
        [SerializeField] private int _maxAliveEnemies = 10;
        [SerializeField] private int _spawnOnStart = 3;
        [SerializeField] private bool _autoSpawn = true;
        [SerializeField] private float _spawnInterval = 2f;
        [SerializeField] private bool _useSpawnPointLocalEulerAngles;
        [SerializeField] private bool _sampleNavMeshPosition = true;
        [SerializeField] private float _navMeshSampleRadius = 1f;

        [Header("Gizmos")]
        [SerializeField] private bool _drawGizmos = true;
        [SerializeField] private float _gizmoRadius = 0.3f;

        private readonly HashSet<GameObject> _hashSet_AliveEnemies = new HashSet<GameObject>();
        private readonly Dictionary<HealthBasicsEvents, Action> _dc_DeathHandlers = new Dictionary<HealthBasicsEvents, Action>();
        private readonly List<AsyncOperationHandle<GameObject>> _list_LoadHandles = new List<AsyncOperationHandle<GameObject>>();
        private readonly List<GameObject> _list_LoadedEnemyPrefabs = new List<GameObject>();

        private Coroutine _spawnRoutine;
        private bool _isLoadingEnemyPrefabs;
        private bool _hasLoadedEnemyPrefabs;
        private bool _hasWarmedPools;

        #region Unity

        private void Start()
        {
            StartCoroutine(InitializeWaves());
        }

        private void OnDisable()
        {
            StopSpawnRoutine();
        }

        private void OnDestroy()
        {
            ClearDeathHandlers();
            ReleaseAddressableHandles();
        }

        private void OnDrawGizmosSelected()
        {
            if (!_drawGizmos) return;

            Gizmos.color = Color.magenta;

            if (_array_SpawnPoints == null || _array_SpawnPoints.Length == 0)
            {
                DrawSpawnGizmo(transform.position);
                return;
            }

            for (int i = 0; i < _array_SpawnPoints.Length; i++)
            {
                Transform spawnPoint = _array_SpawnPoints[i];

                if (spawnPoint == null) continue;

                DrawSpawnGizmo(spawnPoint.position);
                Gizmos.DrawLine(transform.position, spawnPoint.position);
            }
        }

        #endregion

        #region Public API

        public void Enable()
        {
            _active = true;

            if (_autoSpawn && _spawnRoutine == null && gameObject.activeInHierarchy)
                _spawnRoutine = StartCoroutine(AutoSpawnRoutine());
        }

        public void Disable()
        {
            _active = false;
            StopSpawnRoutine();
        }

        public bool TrySpawn()
        {
            if (!_active) return false;

            if (!_hasLoadedEnemyPrefabs)
            {
                StartLoadingEnemyPrefabs();
                return false;
            }

            CleanupAliveEnemies();

            if (_hashSet_AliveEnemies.Count >= _maxAliveEnemies)
                return false;

            GameObject enemyPrefab = GetRandomEnemyPrefab();

            if (enemyPrefab == null)
                return false;

            Transform spawnPoint = GetRandomSpawnPoint();
            Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : transform.position;
            Quaternion spawnRotation = GetSpawnRotation(spawnPoint);

            SpawnEnemy(enemyPrefab, spawnPosition, spawnRotation);
            return true;
        }

        #endregion

        #region Initialization

        private IEnumerator InitializeWaves()
        {
            yield return LoadEnemyPrefabs();

            if (!_hasLoadedEnemyPrefabs)
                yield break;

            WarmPoolsIfNeeded();

            for (int i = 0; i < _spawnOnStart; i++)
                TrySpawn();

            if (_autoSpawn)
                _spawnRoutine = StartCoroutine(AutoSpawnRoutine());
        }

        private void StartLoadingEnemyPrefabs()
        {
            if (_hasLoadedEnemyPrefabs || _isLoadingEnemyPrefabs)
                return;

            StartCoroutine(LoadEnemyPrefabs());
        }

        private IEnumerator LoadEnemyPrefabs()
        {
            if (_hasLoadedEnemyPrefabs || _isLoadingEnemyPrefabs)
                yield break;

            if (_array_EnemyPrefabReferences == null || _array_EnemyPrefabReferences.Length == 0)
            {
                Debug.LogWarning("Wave manager has no enemy prefab references.", this);
                yield break;
            }

            _isLoadingEnemyPrefabs = true;

            for (int i = 0; i < _array_EnemyPrefabReferences.Length; i++)
            {
                AssetReferenceGameObject enemyPrefabReference = _array_EnemyPrefabReferences[i];

                if (enemyPrefabReference == null)
                    continue;

                AsyncOperationHandle<GameObject> loadHandle = enemyPrefabReference.LoadAssetAsync<GameObject>();
                _list_LoadHandles.Add(loadHandle);

                yield return loadHandle;

                if (loadHandle.Status == AsyncOperationStatus.Succeeded && loadHandle.Result != null)
                    _list_LoadedEnemyPrefabs.Add(loadHandle.Result);
                else
                    Debug.LogWarning($"Could not load enemy prefab addressable at index {i}.", this);
            }

            _isLoadingEnemyPrefabs = false;
            _hasLoadedEnemyPrefabs = _list_LoadedEnemyPrefabs.Count > 0;

            WarmPoolsIfNeeded();
        }

        private void WarmPoolsIfNeeded()
        {
            if (_hasWarmedPools || !_warmPoolOnStart || _warmPoolSizeByPrefab <= 0)
                return;

            for (int i = 0; i < _list_LoadedEnemyPrefabs.Count; i++)
            {
                try
                {
                    PoolManager.WarmPool(_list_LoadedEnemyPrefabs[i], _warmPoolSizeByPrefab);
                }
                catch (Exception exception)
                {
                    Debug.LogWarning($"Could not warm pool for {_list_LoadedEnemyPrefabs[i].name}: {exception.Message}", this);
                }
            }

            _hasWarmedPools = true;
        }

        #endregion

        #region Spawn

        private IEnumerator AutoSpawnRoutine()
        {
            while (_active)
            {
                TrySpawn();
                yield return new WaitForSeconds(_spawnInterval);
            }

            _spawnRoutine = null;
        }

        private GameObject SpawnEnemy(GameObject enemyPrefab, Vector3 position, Quaternion rotation)
        {
            if (_sampleNavMeshPosition && NavMesh.SamplePosition(position, out NavMeshHit hit, _navMeshSampleRadius, NavMesh.AllAreas))
                position = hit.position;

            GameObject enemyObject = PoolManager.SpawnObject(enemyPrefab, position, rotation);

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

            if (healthEvents != null && !_dc_DeathHandlers.ContainsKey(healthEvents))
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

        private GameObject GetRandomEnemyPrefab()
        {
            if (_list_LoadedEnemyPrefabs.Count == 0)
                return null;

            int index = UnityEngine.Random.Range(0, _list_LoadedEnemyPrefabs.Count);
            return _list_LoadedEnemyPrefabs[index];
        }

        private Transform GetRandomSpawnPoint()
        {
            if (_array_SpawnPoints == null || _array_SpawnPoints.Length == 0)
                return null;

            int index = UnityEngine.Random.Range(0, _array_SpawnPoints.Length);
            return _array_SpawnPoints[index];
        }

        private Quaternion GetSpawnRotation(Transform spawnPoint)
        {
            if (spawnPoint == null)
                return transform.rotation;

            if (_useSpawnPointLocalEulerAngles)
                return Quaternion.Euler(spawnPoint.localEulerAngles);

            return spawnPoint.rotation;
        }

        private void CleanupAliveEnemies()
        {
            _hashSet_AliveEnemies.RemoveWhere(enemyObject => enemyObject == null || !enemyObject.activeInHierarchy);
        }

        #endregion

        #region Cleanup

        private void StopSpawnRoutine()
        {
            if (_spawnRoutine == null)
                return;

            StopCoroutine(_spawnRoutine);
            _spawnRoutine = null;
        }

        private void ClearDeathHandlers()
        {
            foreach (KeyValuePair<HealthBasicsEvents, Action> deathHandler in _dc_DeathHandlers)
            {
                if (deathHandler.Key != null)
                    deathHandler.Key.OnDeath -= deathHandler.Value;
            }

            _dc_DeathHandlers.Clear();
            _hashSet_AliveEnemies.Clear();
        }

        private void ReleaseAddressableHandles()
        {
            for (int i = 0; i < _list_LoadHandles.Count; i++)
            {
                if (_list_LoadHandles[i].IsValid())
                    Addressables.Release(_list_LoadHandles[i]);
            }

            _list_LoadHandles.Clear();
            _list_LoadedEnemyPrefabs.Clear();
        }

        #endregion

        #region Gizmos

        private void DrawSpawnGizmo(Vector3 position)
        {
            Gizmos.DrawWireSphere(position, _gizmoRadius);

            if (_sampleNavMeshPosition)
                Gizmos.DrawWireSphere(position, _navMeshSampleRadius);
        }

        #endregion
    }
}
