using Scripts.ObjectPooling;
using UnityEngine;

namespace Scripts.Projectile
{
    public abstract class Projectile3DBase : MonoBehaviour
    {
        [SerializeField] protected bool _drawGizmos;
        [SerializeField] protected bool _active;
        [SerializeField] protected Vector3 _gravityValue = Physics.gravity;
        [SerializeField] protected Vector3 _projectileVelocity;
        [SerializeField] protected string _targetTag;
        [SerializeField] protected LayerMask _layerMask;
        [SerializeField] protected bool _useGroundLayer;
        [SerializeField] protected LayerMask _groundLayer;
        [SerializeField] protected float _projectileRadius;
        [SerializeField] protected int _maxAllocation = 8;
        [SerializeField] protected bool _releaseObjectOnHit = true;
        [SerializeField] protected bool _continueMovementOnHit = false;

        protected RaycastHit[] _hitResults;
        protected Transform _transform;

        protected virtual void Awake()
        {
            _hitResults = new RaycastHit[Mathf.Max(1, _maxAllocation)];
            _transform = transform;
        }

        protected virtual void OnDisable()
        {
            _active = false;
        }

        protected virtual void Update()
        {
            if (!_active)
                return;

            MoveProjectile();
        }

        public virtual void EnableProjectile(
            Vector3 startPoint,
            Vector3 speedOrientation,
            Vector3 gravityValue,
            LayerMask layerMask,
            bool useGroundLayer,
            LayerMask groundLayer,
            float radius,
            string targetTag)
        {
            if (_transform == null)
                _transform = transform;

            _transform.position = startPoint;
            _projectileVelocity = speedOrientation;
            _gravityValue = gravityValue;
            _projectileRadius = radius;
            _targetTag = targetTag;
            _useGroundLayer = useGroundLayer;
            _groundLayer = groundLayer;
            _layerMask = layerMask;
            _active = true;
        }

        protected virtual void MoveProjectile()
        {
            float currentDeltaTime = Time.deltaTime;
            Vector3 currentPosition = _transform.position;

            _projectileVelocity += _gravityValue * currentDeltaTime;

            Vector3 targetPosition = currentPosition + (_projectileVelocity * currentDeltaTime);
            Vector3 movementDirection = targetPosition - currentPosition;
            float distanceThisFrame = movementDirection.magnitude;

            if (distanceThisFrame <= 0.001f)
                return;

            Vector3 normalizedDirection = movementDirection / distanceThisFrame;

            int hits = Physics.SphereCastNonAlloc(
                currentPosition,
                _projectileRadius,
                normalizedDirection,
                _hitResults,
                distanceThisFrame,
                _layerMask);

            SortHitsByDistance(hits);

            for (int i = 0; i < hits; i++)
            {
                RaycastHit hit = _hitResults[i];
                Collider hitCollider = hit.collider;

                if (hitCollider == null)
                    continue;

                bool isGroundHit = IsGroundHit(hitCollider);
                if (!isGroundHit && !IsTargetHit(hitCollider))
                    continue;

                // The sphere center and its contact point are different positions.
                Vector3 impactPosition = currentPosition + normalizedDirection * Mathf.Clamp(hit.distance, 0f, distanceThisFrame);
                hit.point = GetHitPoint(hitCollider, hit, currentPosition);

                if (isGroundHit)
                {
                    if (!_continueMovementOnHit)
                    {
                        _transform.position = impactPosition;
                        DeactiveAndRelease();
                        return;
                    }

                    continue;
                }

                if (!_continueMovementOnHit)
                    _transform.position = impactPosition;

                OnHit(hitCollider, hit);

                if (!_active)
                    return;

                if (!_continueMovementOnHit)
                {
                    DeactiveAndRelease();
                    return;
                }
            }

            _transform.position = targetPosition;
        }

        protected virtual Vector3 GetHitPoint(Collider hitCollider, RaycastHit hit, Vector3 castOrigin)
        {
            // Initial overlaps can return a zero point without a usable contact.
            return hit.distance <= 0f ? hitCollider.ClosestPoint(castOrigin) : hit.point;
        }

        private void SortHitsByDistance(int count)
        {
            // NonAlloc casts do not guarantee nearest-first results.
            for (int i = 1; i < count; i++)
            {
                RaycastHit hit = _hitResults[i];
                int previousIndex = i - 1;
                while (previousIndex >= 0 && _hitResults[previousIndex].distance > hit.distance)
                {
                    _hitResults[previousIndex + 1] = _hitResults[previousIndex];
                    previousIndex--;
                }

                _hitResults[previousIndex + 1] = hit;
            }
        }

        protected virtual bool IsGroundHit(Collider hitCollider)
        {
            return _useGroundLayer && ((_groundLayer.value & (1 << hitCollider.gameObject.layer)) > 0);
        }

        protected virtual bool IsTargetHit(Collider hitCollider)
        {
            return string.IsNullOrEmpty(_targetTag) || hitCollider.CompareTag(_targetTag);
        }

        protected virtual void DeactiveAndRelease()
        {
            _active = false;

            if (_releaseObjectOnHit)
                PoolManager.ReleaseObject(gameObject);
        }

        protected virtual void OnDrawGizmos()
        {
            if (_drawGizmos)
                Gizmos.DrawSphere(transform.position, _projectileRadius);
        }

        protected abstract void OnHit(Collider hitCollider, RaycastHit hit);
    }
}
