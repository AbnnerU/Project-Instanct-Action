using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Combat
{
    public class SphereAreaDamageRequest : AreaDamageRequestBase
    {
        [SerializeField, Min(0.01f)] private float _radius = 2f;
        [SerializeField] private Vector3 _centerOffset;
        [SerializeField] private LayerMask _targetLayers;
        [SerializeField] private QueryTriggerInteraction _queryTriggers = QueryTriggerInteraction.Collide;
        [Tooltip("Optional unit-diameter ground indicator; X/Z scale is set to the damage diameter.")]
        [SerializeField] private Transform _radiusIndicator;

        public override void Setup(DamageRequest request)
        {
            if (_radiusIndicator != null)
                _radiusIndicator.localScale = new Vector3(_radius * 2, _radiusIndicator.localScale.y, _radius * 2);
            base.Setup(request);
        }

        protected override void ApplyDamage()
        {
            Vector3 center = transform.position + transform.rotation * _centerOffset;
            var receivers = new HashSet<IDamageReceiver>();
            foreach (Collider collider in Physics.OverlapSphere(center, _radius, _targetLayers, _queryTriggers))
            {
                var receiver = collider.GetComponentInParent<IDamageReceiver>();
                if (receiver == null || !receivers.Add(receiver)) continue;
                var component = receiver as Component;
                if (component == null) continue;
                DamageResult result = receiver.ReceiveDamage(Request.WithHit(component.gameObject, collider, collider.ClosestPoint(center)));
                ReportDamageDealt(result);
                if (!isActiveAndEnabled) break;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position + transform.rotation * _centerOffset, _radius);
        }
    }
}
