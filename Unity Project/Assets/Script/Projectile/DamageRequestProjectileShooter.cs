using Scripts.Combat;
using Scripts.ObjectPooling;
using Scripts.Status;
using UnityEngine;

namespace Scripts.Projectile
{
    public class DamageRequestProjectileShooter : MonoBehaviour, ICanShootProjectile
    {
        #region Fields

        [Header("References")]
        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private Transform _firePoint;
        [SerializeField] private GameObject _attackerObject;
        [SerializeField] private StatusInfoSourceBase _statusSource;
        [Header("Container")]
        [SerializeField] private Transform _projectileContainer;
        [SerializeField] private string _containerTag;

        [Header("Damage")]
        [SerializeField] private DamageData _damageData = new DamageData();

        [Header("Projectile")]
        [SerializeField] private float _projectileSpeed = 8f;
        [SerializeField] private Vector3 _gravityValue = Vector3.zero;
        [SerializeField] private float _projectileRadius = 0.25f;
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private bool _useGroundLayer;
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private string _targetTag;

        #endregion

        #region Unity

        private void Awake()
        {
            GetReferences();
        }

        #endregion

        #region Public

        public void Shoot()
        {
            if (_projectilePrefab == null)
            {
                Debug.LogWarning("Projectile prefab is missing.", this);
                return;
            }

            Transform projectileOrigin = _firePoint != null ? _firePoint : transform;
            Vector3 spawnPosition = projectileOrigin.position;
            Quaternion spawnRotation = projectileOrigin.rotation;
            Vector3 direction = projectileOrigin.forward;

            GameObject projectileObject = PoolManager.SpawnObject(_projectilePrefab.gameObject, spawnPosition, spawnRotation);

            if (_projectileContainer != null)
                projectileObject.transform.SetParent(_projectileContainer);

            if (!projectileObject.TryGetComponent(out DamageRequestProjectile projectile))
            {
                Debug.LogWarning("Spawned projectile does not have a DamageRequestProjectile component.", projectileObject);
                return;
            }

            DamageRequest damageRequest = new DamageRequest(
                _damageData,
                _statusSource != null ? _statusSource.GetStatusInfo() : null,
                _attackerObject != null ? _attackerObject : gameObject,
                null,
                null,
                spawnPosition);

            projectile.SetDamageRequest(damageRequest);
            projectile.EnableProjectile(
                spawnPosition,
                direction * _projectileSpeed,
                _gravityValue,
                _layerMask,
                _useGroundLayer,
                _groundLayer,
                _projectileRadius,
                _targetTag);
        }

        #endregion

        #region Setup

        private void GetReferences()
        {
            if (_firePoint == null)
                _firePoint = transform;

            if (_attackerObject == null)
                _attackerObject = gameObject;

            if (_statusSource == null)
                _statusSource = GetComponentInParent<StatusInfoSourceBase>();

            if (_containerTag != string.Empty)
                _projectileContainer = GameObject.FindGameObjectWithTag(_containerTag).transform;
        }

        #endregion
    }
}
