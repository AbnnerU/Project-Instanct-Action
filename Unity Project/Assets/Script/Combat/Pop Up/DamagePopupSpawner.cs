using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.ObjectPooling;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Random = UnityEngine.Random;

namespace Scripts.Combat
{
    public class DamagePopupSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _damageReceiverReference;
        [SerializeField] private Transform _spawnPositionReference;
        [SerializeField] private Vector3 _minOffSet = -Vector3.up;
        [SerializeField] private Vector3 _maxOffSet = Vector3.up;
        [Header("Container")]
        [SerializeField] private string _containerTag;
        [SerializeField] private Transform _container;
        [SerializeField] private List<DamagePopupAssetData> _list_DamagePopupAssetData = new List<DamagePopupAssetData>();
        [SerializeField] private bool _drawGizmos = true;

        private readonly Dictionary<DamageType, GameObject> _dc_LoadedPopupPrefabs = new Dictionary<DamageType, GameObject>();
        private readonly Dictionary<DamageType, AsyncOperationHandle<GameObject>> _dc_LoadHandles = new Dictionary<DamageType, AsyncOperationHandle<GameObject>>();
        private readonly Dictionary<DamageType, List<DamageResult>> _dc_PendingDamageResults = new Dictionary<DamageType, List<DamageResult>>();
        private readonly HashSet<DamageType> _hashSet_LoadingDamageTypes = new HashSet<DamageType>();

        private IDamageReceiver _damageReceiver;
        private Vector3 _offSet;

        private void Awake()
        {
            CacheDamageReceiver();

            if (_container == null && _containerTag != string.Empty)
                _container = GameObject.FindGameObjectWithTag(_containerTag).transform;
        }

        private void OnEnable()
        {
            if (_damageReceiver == null)
                CacheDamageReceiver();

            if (_damageReceiver != null)
                _damageReceiver.OnDamageReceived += DamageReceiver_OnDamageReceived;
        }

        private void OnDisable()
        {
            if (_damageReceiver != null)
                _damageReceiver.OnDamageReceived -= DamageReceiver_OnDamageReceived;
        }

        private void OnDestroy()
        {
            foreach (KeyValuePair<DamageType, AsyncOperationHandle<GameObject>> loadHandle in _dc_LoadHandles)
            {
                if (loadHandle.Value.IsValid())
                    Addressables.Release(loadHandle.Value);
            }

            _dc_LoadHandles.Clear();
            _dc_LoadedPopupPrefabs.Clear();
            _dc_PendingDamageResults.Clear();
            _hashSet_LoadingDamageTypes.Clear();
        }

        private void DamageReceiver_OnDamageReceived(DamageResult damageResult)
        {
            if (_dc_LoadedPopupPrefabs.TryGetValue(damageResult.DamageType, out GameObject loadedPrefab))
            {
                SpawnPopup(loadedPrefab, damageResult);
                return;
            }

            QueuePendingDamageResult(damageResult);

            if (_hashSet_LoadingDamageTypes.Contains(damageResult.DamageType))
                return;

            DamagePopupAssetData popupAssetData = GetPopupAssetData(damageResult.DamageType);
            if (popupAssetData == null || popupAssetData.PopupPrefabReference == null)
            {
                ClearPendingDamageResults(damageResult.DamageType);
                return;
            }

            StartCoroutine(LoadPopupPrefab(popupAssetData));
        }

        private IEnumerator LoadPopupPrefab(DamagePopupAssetData popupAssetData)
        {
            _hashSet_LoadingDamageTypes.Add(popupAssetData.DamageType);

            AsyncOperationHandle<GameObject> loadHandle = popupAssetData.PopupPrefabReference.LoadAssetAsync<GameObject>();
            yield return loadHandle;

            _hashSet_LoadingDamageTypes.Remove(popupAssetData.DamageType);

            if (loadHandle.Status != AsyncOperationStatus.Succeeded || loadHandle.Result == null)
            {
                Debug.LogWarning($"Could not load damage popup prefab for {popupAssetData.DamageType}.", this);
                ClearPendingDamageResults(popupAssetData.DamageType);
                yield break;
            }

            _dc_LoadHandles[popupAssetData.DamageType] = loadHandle;
            _dc_LoadedPopupPrefabs[popupAssetData.DamageType] = loadHandle.Result;

            SpawnPendingDamageResults(popupAssetData.DamageType, loadHandle.Result);
        }

        private void SpawnPopup(GameObject popupPrefab, DamageResult damageResult)
        {
            Vector3 spawnPosition = GetSpawnPosition(damageResult);
            GameObject popupObject = PoolManager.SpawnObject(popupPrefab, spawnPosition, Quaternion.identity);
            popupObject.transform.SetParent(_container,true);

            if (popupObject.TryGetComponent(out DamagePopupView popupView))
                popupView.Setup(damageResult);
        }

        private Vector3 GetSpawnPosition(DamageResult damageResult)
        {
            _offSet = _maxOffSet;

            if (_minOffSet != _maxOffSet)
            {
                _offSet.x = Random.Range(_minOffSet.x, _maxOffSet.x);
                _offSet.y = Random.Range(_minOffSet.y, _maxOffSet.y);
                _offSet.z = Random.Range(_minOffSet.z, _maxOffSet.z);
            }

            return _spawnPositionReference.position + _offSet;
        }

        private DamagePopupAssetData GetPopupAssetData(DamageType damageType)
        {
            return _list_DamagePopupAssetData.Find(x => x.DamageType == damageType);
        }

        private void QueuePendingDamageResult(DamageResult damageResult)
        {
            if (!_dc_PendingDamageResults.TryGetValue(damageResult.DamageType, out List<DamageResult> pendingResults))
            {
                pendingResults = new List<DamageResult>();
                _dc_PendingDamageResults.Add(damageResult.DamageType, pendingResults);
            }

            pendingResults.Add(damageResult);
        }

        private void SpawnPendingDamageResults(DamageType damageType, GameObject popupPrefab)
        {
            if (!_dc_PendingDamageResults.TryGetValue(damageType, out List<DamageResult> pendingResults))
                return;

            for (int i = 0; i < pendingResults.Count; i++)
                SpawnPopup(popupPrefab, pendingResults[i]);

            _dc_PendingDamageResults.Remove(damageType);
        }

        private void ClearPendingDamageResults(DamageType damageType)
        {
            _dc_PendingDamageResults.Remove(damageType);
        }

        private void CacheDamageReceiver()
        {
            if (_damageReceiverReference == null)
                _damageReceiverReference = gameObject;

            _damageReceiver = _damageReceiverReference.GetComponent<IDamageReceiver>();

            if (_damageReceiver == null)
                _damageReceiver = _damageReceiverReference.GetComponentInParent<IDamageReceiver>();

            if (_damageReceiver == null)
                _damageReceiver = _damageReceiverReference.GetComponentInChildren<IDamageReceiver>();
        }

        private void OnDrawGizmosSelected()
        {
            if (!_drawGizmos) return;

            Transform referenceTransform = _spawnPositionReference != null ? _spawnPositionReference : transform;
            Vector3 minPosition = referenceTransform.position + _minOffSet;
            Vector3 maxPosition = referenceTransform.position + _maxOffSet;
            Vector3 center = (minPosition + maxPosition) * 0.5f;
            Vector3 size = new Vector3(
                Mathf.Abs(maxPosition.x - minPosition.x),
                Mathf.Abs(maxPosition.y - minPosition.y),
                Mathf.Abs(maxPosition.z - minPosition.z));

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(referenceTransform.position, center);
            Gizmos.DrawWireCube(center, size);
            Gizmos.DrawWireSphere(center, 0.1f);
        }

    }

    [Serializable]
    public class DamagePopupAssetData
    {
        public DamageType DamageType;
        public AssetReferenceGameObject PopupPrefabReference;
    }
}
