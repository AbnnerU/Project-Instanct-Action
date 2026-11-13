using System.Collections;
using System.Collections.Generic;
using Scripts.Combat;
using Scripts.Movement;
using Scripts.ObjectPooling;
using Scripts.Status;
using UnityEngine;

namespace Scripts.Abilities
{
    public class OrbitingObjectsAbility : AbilityBase, IDamageDataAbility
    {
        [SerializeField] private OrbitMotion _orbPrefab;
        [SerializeField] private Transform _orbitTarget;
        [SerializeField] private Transform _orbContainer;
        [SerializeField] private bool _containerByTag;
        [SerializeField] private string _tag;
        [SerializeField, Min(1)] private int _orbCount = 3;
        [SerializeField] private Vector3 _centerOffset = Vector3.up;
        [SerializeField] private Vector3 _orbitAxis = Vector3.up;
        [SerializeField, Min(0)] private float _orbitRadius = 1.5f;
        [SerializeField] private float _orbitSpeed = 180f;
        [SerializeField, Min(0.01f)] private float _duration = 8f;
        [SerializeField] private DamageData _damageData = new DamageData();

        private StatusInfoSourceBase _statusSource;
        private GameObject _owner;
        private ICriticalDamagePermissionsProvider _criticalPermissions;
        private IDamageResultReporter _reporter;
        private readonly List<GameObject> _list_Orbs = new List<GameObject>();
        private bool _isSpawning;
        private int _generation;

        public void Setup(StatusInfoSourceBase statusSource, GameObject owner)
        {
            _statusSource = statusSource;
            _owner = owner;

            if (_orbitTarget == null && owner != null) 
                _orbitTarget = owner.transform;

            if (_containerByTag)
                _orbContainer = GameObject.FindGameObjectWithTag(_tag).transform;

            _criticalPermissions = owner != null ? owner.GetComponent<ICriticalDamagePermissionsProvider>() : null;
            _reporter = owner != null ? owner.GetComponent<IDamageResultReporter>() : null;
        }

        public void SetDamageData(IReadOnlyList<DamageData> list_Damage)
        {
            if (list_Damage != null && list_Damage.Count > 0 && list_Damage[0] != null) _damageData = list_Damage[0];
        }

        public override void Execute()
        {
            if (!CanExecute() || !isActiveAndEnabled || _isSpawning || _orbitTarget == null) return;
            if (_orbPrefab == null || !_orbPrefab.TryGetComponent<TriggerContactDamageRequest>(out _) ||
                !_orbPrefab.TryGetComponent<Collider>(out var collider) || !collider.isTrigger)
            {
                Debug.LogWarning("[OrbitingObjectsAbility] Orb prefab needs OrbitMotion, TriggerContactDamageRequest and a trigger Collider on its root.", this);
                return;
            }

            Cancel();
            _isSpawning = true;
            int generation = _generation;
            try
            {
                var request = new DamageRequest(_damageData, _statusSource != null ? _statusSource.GetStatusInfo() : null,
                    _owner != null ? _owner : gameObject, null, null, _orbitTarget.position,
                    allowedCriticalTypes: _criticalPermissions != null ? _criticalPermissions.GetAllowedCriticalDamageTypes() : DamageRequest.DefaultCriticalTypes,
                    attackerDamageResultReporter: _reporter);
                int count = Mathf.Max(1, _orbCount);
                for (int i = 0; i < count; i++)
                {
                    GameObject orb = PoolManager.SpawnObject(_orbPrefab.gameObject, _orbitTarget.position + _centerOffset, Quaternion.identity);
                    _list_Orbs.Add(orb);
                    orb.transform.SetParent(_orbContainer, true);
                    orb.GetComponent<OrbitMotion>().Setup(_orbitTarget, _centerOffset, _orbitAxis, _orbitRadius, _orbitSpeed, 360f * i / count);
                    orb.GetComponent<TriggerContactDamageRequest>().Setup(request);
                }
                OnAbilityStarted?.Invoke(this);
                if (generation == _generation && Initialized && isActiveAndEnabled)
                    StartCoroutine(ExpireOrbs(Mathf.Max(0.01f, _duration)));
            }
            catch
            {
                Cancel();
                throw;
            }
            finally
            {
                _isSpawning = false;
                OnAbilityFinished?.Invoke(this);
            }
        }

        private IEnumerator ExpireOrbs(float duration)
        {
            yield return new WaitForSeconds(duration);
            ReleaseOrbs();
        }

        public override void Cancel()
        {
            _generation++;
            StopAllCoroutines();
            ReleaseOrbs();
        }

        private void ReleaseOrbs()
        {
            while (_list_Orbs.Count > 0)
            {
                int index = _list_Orbs.Count - 1;
                GameObject orb = _list_Orbs[index];
                _list_Orbs.RemoveAt(index);
                if (orb != null && orb.activeSelf) PoolManager.ReleaseObject(orb);
            }
        }

        protected override void OnDeactivate() => Cancel();
        private void OnDisable() => Cancel();
    }
}
