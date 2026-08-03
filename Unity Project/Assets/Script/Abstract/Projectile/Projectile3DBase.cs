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

            bool hasHitStoppingObject = false;
            Vector3 finalStopPosition = targetPosition;

            for (int i = 0; i < hits; i++)
            {
                Collider hitCollider = _hitResults[i].collider;

                if (hitCollider == null)
                    continue;

                if (IsGroundHit(hitCollider))
                {
                    if (!_continueMovementOnHit)
                    {
                        hasHitStoppingObject = true;
                        finalStopPosition = _hitResults[i].point;
                        DeactiveAndRelease();
                        break;
                    }

                    continue;
                }

                if (!IsTargetHit(hitCollider))
                    continue;

                OnHit(hitCollider, _hitResults[i]);

                if (!_continueMovementOnHit)
                {
                    hasHitStoppingObject = true;
                    finalStopPosition = _hitResults[i].point;
                    DeactiveAndRelease();
                    break;
                }
            }

            _transform.position = hasHitStoppingObject ? finalStopPosition : targetPosition;
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