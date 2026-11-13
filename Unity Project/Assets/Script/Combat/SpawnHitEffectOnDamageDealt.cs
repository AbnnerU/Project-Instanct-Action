using Scripts.ObjectPooling;
using UnityEngine;

namespace Scripts.Combat
{
    public class SpawnHitEffectOnDamageDealt : MonoBehaviour
    {
        [Tooltip("Component implementing IDamageDealer. Defaults to this GameObject.")]
        [SerializeField] private GameObject _damageDealerObject;
        [SerializeField] private GameObject _effectPrefab;
        [SerializeField] private Vector3 _positionOffset;
        [SerializeField] private Vector3 _rotation;

        private IDamageDealer _damageDealer;

        public void SetEffectPrefab(GameObject prefab) => _effectPrefab = prefab;

        private void OnEnable()
        {
            _damageDealer = _damageDealerObject.GetComponent<IDamageDealer>();
              

            if (_damageDealer == null)
            {
                Debug.LogWarning($"[{nameof(SpawnHitEffectOnDamageDealt)}] Missing IDamageDealer.", this);
                return;
            }

            _damageDealer.OnDamageDealt += DamageDealer_OnDamageDealt;
        }

        private void OnDisable()
        {
            if (_damageDealer != null)
                _damageDealer.OnDamageDealt -= DamageDealer_OnDamageDealt;
            _damageDealer = null;
        }

        private void DamageDealer_OnDamageDealt(DamageResult result)
        {
            if (!result.WasApplied || _effectPrefab == null) return;

            PoolManager.SpawnObject(_effectPrefab, result.HitPoint + _positionOffset, Quaternion.Euler(_rotation));
        }
    }
}
