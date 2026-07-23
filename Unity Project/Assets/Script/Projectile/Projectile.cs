using Scripts.PoolManager;
using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private bool _drawGizmos;

    [SerializeField] private bool _active;

    [SerializeField] private Vector3 _gravityValue = Physics.gravity;

    [SerializeField] private Vector3 _projectileVelocity;

    [SerializeField] private int _damage;

    [SerializeField] private string _targetTag;

    [SerializeField] private LayerMask _layerMask;

    [SerializeField] private bool _useGroundLayer;

    [SerializeField] private LayerMask _groundLayer;

    //[SerializeField] private int _predictionPerFrame = 6;

    [SerializeField] private float _projectileRadius;

    [SerializeField] private int _maxAllocation;

    [SerializeField] private bool _releaseObjectOnHit = true;

    [SerializeField] private bool _continueMovementOnHit = false;

    public Action<IHittable> OnHitDamageableTarget;

    private RaycastHit[] _hitResults;

    private Transform _transform;

    //private Vector3 _point1;


    private void Awake()
    {
        _hitResults = new RaycastHit[_maxAllocation];

        _transform = GetComponent<Transform>();
    }

    private void OnDisable()
    {
        _active = false;
    }

    private void Update()
    {
        if (!_active)
            return;

        float currentDeltaTime = Time.deltaTime;

        Vector3 currentPosition = _transform.position;

        _projectileVelocity += _gravityValue * currentDeltaTime;

        Vector3 targetPosition = currentPosition + (_projectileVelocity * currentDeltaTime);

        Vector3 movementDirection = targetPosition - currentPosition;
        float distanceThisFrame = movementDirection.magnitude;

        if (distanceThisFrame > 0.001f)
        {
            Vector3 normalizedDirection = movementDirection / distanceThisFrame;

            int hits = Physics.SphereCastNonAlloc(
                currentPosition,
                _projectileRadius,
                normalizedDirection,
                _hitResults,
                distanceThisFrame,
                _layerMask
            );

            // Variável para não movermos o projétil se ele tiver morrido no meio do caminho
            bool hasHitStoppingObject = false;
            Vector3 finalStopPosition = targetPosition;

            for (int i = 0; i < hits; i++)
            {
                Collider hitCollider = _hitResults[i].collider;

                // Checagem de Chão/Parede
                if (_useGroundLayer && ((_groundLayer.value & (1 << hitCollider.gameObject.layer)) > 0))
                {
                    if (!_continueMovementOnHit)
                    {
                        hasHitStoppingObject = true;
                        finalStopPosition = _hitResults[i].point; // Para o projétil exatamente onde bateu
                        DeactiveAndRelease();
                        break;
                    }
                }

                // Checagem de Alvo
                if (hitCollider.CompareTag(_targetTag))
                {
                    OnHit(hitCollider.gameObject);

                    if (!_continueMovementOnHit)
                    {
                        hasHitStoppingObject = true;
                        finalStopPosition = _hitResults[i].point;
                        DeactiveAndRelease();
                        break;
                    }
                }
            }

            // Move o objeto fisicamente. 
            // Se bateu em algo e parou, vai até o ponto de impacto. 
            // Se não bateu (ou passa direto), vai até o targetPosition.
            _transform.position = hasHitStoppingObject ? finalStopPosition : targetPosition;
        }
    }

    private void DeactiveAndRelease()
    {
        _active = false;

        if (_releaseObjectOnHit)      
            PoolManager.ReleaseObject(gameObject);
    }


    public void EnableProjectile(Vector3 startPoint, Vector3 speedOrietation, Vector3 gravityValue, LayerMask layerMask, bool useGroundLayer, LayerMask groundLayer, float radius, int damage, string targetTag)
    {
        if (_transform == null)
            _transform = transform;

        _transform.position = startPoint;

        _projectileVelocity = speedOrietation;

        _gravityValue = gravityValue;

        _projectileRadius = radius;

        _damage = damage;

        _targetTag = targetTag;

        _useGroundLayer = useGroundLayer;

        _groundLayer = groundLayer;

        _layerMask = layerMask;

        _active = true;
    }

    private void OnHit(GameObject obj)
    {
        IHittable damageable = obj.GetComponent<IHittable>();
        IHittable damageableChild = obj.GetComponentInChildren<IHittable>();

        if (damageable != null && obj.CompareTag(_targetTag))
        {
            damageable.Hit(_damage);

            OnHitDamageableTarget?.Invoke(damageable);
        }
        else if(damageableChild != null && obj.CompareTag(_targetTag))
        {
            damageableChild.Hit(_damage);

            OnHitDamageableTarget?.Invoke(damageableChild);
        }
    }

    private void OnDrawGizmos()
    {
        if (_drawGizmos)
        {
            Gizmos.DrawSphere(transform.position, _projectileRadius);
        }

    }
}
