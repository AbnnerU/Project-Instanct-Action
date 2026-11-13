using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Combat
{
    [RequireComponent(typeof(Collider))]
    public class TriggerContactDamageRequest : MonoBehaviour, IDamageDealer
    {
        public event System.Action<DamageResult> OnDamageDealt;
        [SerializeField] private LayerMask _targetLayers;
        [SerializeField, Min(0.01f)] private float _damageInterval = 0.5f;
        private readonly Dictionary<IDamageReceiver, float> _nextDamageTimes = new Dictionary<IDamageReceiver, float>();
        private DamageRequest _request;
        private bool _prepared;

        public void Setup(DamageRequest request)
        {
            _request = request;
            _nextDamageTimes.Clear();
            _prepared = true;
        }

        private void OnTriggerEnter(Collider other) => TryDamage(other);
        private void OnTriggerStay(Collider other) => TryDamage(other);

        private void TryDamage(Collider other)
        {
            if (!_prepared || (_targetLayers.value & (1 << other.gameObject.layer)) == 0) return;

            if (_request.Attacker != null && other.transform.IsChildOf(_request.Attacker.transform)) return;

            var receiver = other.GetComponentInParent<IDamageReceiver>();

            if (receiver == null || !(receiver is Component component)) return;

            if (_nextDamageTimes.TryGetValue(receiver, out float nextTime) && Time.time < nextTime) return;

            _nextDamageTimes[receiver] = Time.time + Mathf.Max(0.01f, _damageInterval);
            DamageResult result = receiver.ReceiveDamage(_request.WithHit(component.gameObject, other, other.ClosestPoint(transform.position)));
            OnDamageDealt?.Invoke(result);
        }

        private void OnDisable()
        {
            _prepared = false;
            _request = default;
            _nextDamageTimes.Clear();
        }
    }
}
