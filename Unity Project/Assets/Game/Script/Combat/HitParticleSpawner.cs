using System.Collections.Generic;
using Scripts.ObjectPooling;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Scripts.Combat
{
    public class HitParticleSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _damageReceiverReference;
        [SerializeField] private AssetReferenceGameObject _particlePrefabReference;
        [SerializeField] private Vector3 _positionOffset;
        [Header("Container")]
        [SerializeField] private Transform _container;
        [SerializeField] private bool _findContainerByTag;
        [SerializeField] private string _containerTag;

        private readonly List<Vector3> _pendingHitPoints = new List<Vector3>();
        private GameObject _particlePrefab;

        private IDamageReceiver _damageReceiver;
        private AsyncOperationHandle<GameObject> _loadHandle;


        private void Awake()
        {
            if (_damageReceiverReference == null)
                _damageReceiverReference = gameObject;

            _damageReceiver = _damageReceiverReference.GetComponent<IDamageReceiver>();
            if (_damageReceiver == null)
                _damageReceiver = _damageReceiverReference.GetComponentInParent<IDamageReceiver>();
            if (_damageReceiver == null)
                _damageReceiver = _damageReceiverReference.GetComponentInChildren<IDamageReceiver>();

            if (_findContainerByTag)
                _container = GameObject.FindGameObjectWithTag(_containerTag).transform;
        }

        private void OnEnable()
        {
            if (_damageReceiver != null)
                _damageReceiver.OnDamageReceived += DamageReceiver_OnDamageReceived;

            SpawnPendingHits();
        }

        private void Start()
        {
            if (_particlePrefabReference == null || !_particlePrefabReference.RuntimeKeyIsValid())
            {
                Debug.LogWarning("Hit particle prefab reference is missing.", this);
                return;
            }

            _loadHandle = _particlePrefabReference.LoadAssetAsync<GameObject>();
            _loadHandle.Completed += OnParticlePrefabLoaded;
        }

        private void OnDisable()
        {
            if (_damageReceiver != null)
                _damageReceiver.OnDamageReceived -= DamageReceiver_OnDamageReceived;

            _pendingHitPoints.Clear();
        }

        private void OnDestroy()
        {
            if (_loadHandle.IsValid())
            {
                _loadHandle.Completed -= OnParticlePrefabLoaded;
                Addressables.Release(_loadHandle);
            }
        }

        private void OnParticlePrefabLoaded(AsyncOperationHandle<GameObject> loadHandle)
        {
            if (loadHandle.Status != AsyncOperationStatus.Succeeded || loadHandle.Result == null)
            {
                Debug.LogWarning("Could not load hit particle prefab.", this);
                _pendingHitPoints.Clear();
                return;
            }

            _particlePrefab = loadHandle.Result;
            SpawnPendingHits();
        }

        private void SpawnPendingHits()
        {
            if (!isActiveAndEnabled || _particlePrefab == null)
                return;

            for (int i = 0; i < _pendingHitPoints.Count; i++)
                SpawnParticle(_pendingHitPoints[i]);

            _pendingHitPoints.Clear();
        }

        private void DamageReceiver_OnDamageReceived(DamageResult result)
        {
            if (!result.WasApplied || result.FinalDamage <= 0f)
                return;

            if (_particlePrefab == null)
            {
                if (_loadHandle.IsValid() && !_loadHandle.IsDone)
                    _pendingHitPoints.Add(result.HitPoint);
                return;
            }

            SpawnParticle(result.HitPoint);
        }

        private void SpawnParticle(Vector3 hitPoint)
        {
            GameObject particle = PoolManager.SpawnObject(
                _particlePrefab, hitPoint + _positionOffset, Quaternion.identity);

            if (_container != null)
                particle.transform.SetParent(_container, true);
        }
    }
}
