using System.Collections;
using UnityEngine;

namespace Scripts.Combat
{
    public class AttackApproachAssist : MonoBehaviour
    {
        #region Fields

        [Header("References")]
        [SerializeField] private ComboControllerBase _comboController;
        [SerializeField] private AttackHitbox _attackHitbox;
        [SerializeField] private Transform _targetTransform;
        [Tooltip("Its forward axis must point in the attack direction.")]
        [SerializeField] private Transform _directionReference;

        [Header("Approach")]
        [SerializeField, Min(0f)] private float _maxDistance = 1f;
        [SerializeField, Min(0f)] private float _reachMargin = 0.05f;
        [SerializeField, Min(0f)] private float _movementDuration = 0.1f;
        [SerializeField] private AnimationCurve _movementCurve = new AnimationCurve(
            new Keyframe(0f, 0f, 2f, 2f), new Keyframe(1f, 1f, 0f, 0f));

        [Header("Obstacles")]
        [Tooltip("World obstacles only. Exclude the ground and damage target layers.")]
        [SerializeField] private LayerMask _obstacleLayerMask;
        [Tooltip("The world bounds of this collider are swept to protect the character body.")]
        [SerializeField] private Collider _bodyCollider;
        [SerializeField, Min(0f)] private float _obstacleMargin = 0.02f;

        [Header("Gizmos")]
        [SerializeField] private bool _drawGizmos = true;
        [SerializeField, Min(0)] private int _debugAttackIndex;
        [SerializeField] private Color _hitboxColor = Color.yellow;
        [SerializeField] private Color _searchColor = Color.cyan;
        [SerializeField] private Color _destinationColor = Color.green;
        [SerializeField, Min(0f)] private float _debugDuration = 1f;

        private Vector3 _lastCenter;
        private Vector3 _lastExtents;
        private Quaternion _lastRotation;
        private Vector3 _lastDirection;
        private float _lastDistance;
        private float _debugUntil;
        private Coroutine _movementRoutine;

        #endregion

        #region Lifecycle

        private void Reset() => GetReferences();

        private void OnEnable()
        {
            GetReferences();
            if (_comboController != null)
            {
                _comboController.OnBeforeAttack += ComboController_OnBeforeAttack;
                _comboController.OnAttackCanceled += StopMovement;
                _comboController.OnComboFinished += StopMovement;
            }
        }

        private void OnDisable()
        {
            if (_comboController != null)
            {
                _comboController.OnBeforeAttack -= ComboController_OnBeforeAttack;
                _comboController.OnAttackCanceled -= StopMovement;
                _comboController.OnComboFinished -= StopMovement;
            }
            StopMovement();
            _debugUntil = 0f;
        }

        private void GetReferences()
        {
            if (_comboController == null) _comboController = GetComponentInParent<ComboControllerBase>();
            if (_targetTransform == null && _comboController != null) _targetTransform = _comboController.transform;
            if (_attackHitbox == null && _comboController != null) _attackHitbox = _comboController.GetComponent<AttackHitbox>();
            if (_directionReference == null && _targetTransform != null) _directionReference = _targetTransform;
            if (_bodyCollider == null && _targetTransform != null) _bodyCollider = _targetTransform.GetComponent<Collider>();
        }

        #endregion

        #region Approach

        private void ComboController_OnBeforeAttack(ComboAttackData attackData)
        {
            StopMovement();
            if (attackData == null || _attackHitbox == null || _targetTransform == null || _directionReference == null)
                return;

            // The hitbox must travel with the selected transform.
            if (!_attackHitbox.transform.IsChildOf(_targetTransform))
                return;

            Physics.SyncTransforms();
            Vector3 center = _attackHitbox.GetHitboxCenter(attackData);
            Quaternion rotation = _attackHitbox.GetHitboxRotation();
            Vector3 direction = _directionReference.forward.normalized;
            Vector3 extents = attackData.HitboxHalfExtents;
            _lastCenter = center;
            _lastExtents = extents;
            _lastRotation = rotation;
            _lastDirection = direction;
            _lastDistance = 0f;
            _debugUntil = Time.time + _debugDuration;

            if (extents.x <= 0f || extents.y <= 0f || extents.z <= 0f || _maxDistance <= 0f)
                return;

            foreach (Collider hit in Physics.OverlapBox(center, extents, rotation,
                         attackData.TargetLayerMask, attackData.QueryTriggerInteraction))
            {
                if (IsValidTarget(hit)) return;
            }

            float nearestDistance = float.PositiveInfinity;
            foreach (RaycastHit hit in Physics.BoxCastAll(center, extents, direction, rotation,
                         _maxDistance, attackData.TargetLayerMask, attackData.QueryTriggerInteraction))
            {
                if (IsValidTarget(hit.collider))
                    nearestDistance = Mathf.Min(nearestDistance, hit.distance);
            }

            if (float.IsPositiveInfinity(nearestDistance)) return;

            float distance = Mathf.Min(_maxDistance, nearestDistance + Mathf.Max(0f, _reachMargin));
            distance = LimitByObstacles(direction, distance);
            if (distance <= 0f) return;

            _lastDistance = distance;
            Vector3 destination = _targetTransform.position + direction * distance;
            if (_movementDuration <= 0f)
            {
                _targetTransform.position = destination;
                Physics.SyncTransforms();
                return;
            }
            _movementRoutine = StartCoroutine(MoveToPosition(destination));
        }

        private IEnumerator MoveToPosition(Vector3 destination)
        {
            Vector3 startPosition = _targetTransform.position;
            float duration = _movementDuration;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                yield return null;
                if (_targetTransform == null || _comboController == null ||
                    !_comboController.isActiveAndEnabled || !_comboController.Active || !_comboController.IsAttacking)
                {
                    _movementRoutine = null;
                    yield break;
                }

                elapsedTime += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsedTime / duration);
                float curveValue = _movementCurve != null && _movementCurve.length > 0
                    ? _movementCurve.Evaluate(progress) : progress;
                // Keep custom curves inside the distance approved by the initial obstacle check.
                _targetTransform.position = Vector3.Lerp(startPosition, destination, curveValue);
                Physics.SyncTransforms();
            }

            _targetTransform.position = destination;
            Physics.SyncTransforms();
            _movementRoutine = null;
        }

        private void StopMovement()
        {
            if (_movementRoutine == null) return;
            StopCoroutine(_movementRoutine);
            _movementRoutine = null;
        }

        private bool IsSelf(Collider hit) =>
            hit.transform.IsChildOf(_targetTransform) ||
            (_comboController != null && hit.transform.IsChildOf(_comboController.transform));

        private bool IsValidTarget(Collider hit)
        {
            if (hit == null || IsSelf(hit)) return false;

            IHittable hittable = hit.GetComponentInParent<IHittable>();
            if (hittable == null) hittable = hit.GetComponentInChildren<IHittable>();
            if (hittable != null && !hittable.IsAlive()) return false;

            IDamageReceiver receiver = hit.GetComponentInParent<IDamageReceiver>();
            if (receiver == null) receiver = hit.GetComponentInChildren<IDamageReceiver>();
            if (receiver is Behaviour behaviour && !behaviour.isActiveAndEnabled) return false;
            return receiver != null || hittable != null;
        }

        private float LimitByObstacles(Vector3 direction, float distance)
        {
            if (_obstacleLayerMask.value == 0) return distance;
            if (_bodyCollider == null || !_bodyCollider.enabled) return 0f;

            // A conservative world-aligned box protects the whole body, not just the attack area.
            Bounds bounds = _bodyCollider.bounds;
            foreach (Collider hit in Physics.OverlapBox(bounds.center, bounds.extents,
                         Quaternion.identity, _obstacleLayerMask, QueryTriggerInteraction.Ignore))
            {
                if (!IsSelf(hit)) return 0f;
            }

            foreach (RaycastHit hit in Physics.BoxCastAll(bounds.center, bounds.extents, direction,
                         Quaternion.identity, distance, _obstacleLayerMask, QueryTriggerInteraction.Ignore))
            {
                if (!IsSelf(hit.collider))
                    distance = Mathf.Min(distance, Mathf.Max(0f, hit.distance - _obstacleMargin));
            }
            return distance;
        }

        #endregion

        #region Gizmos

        private void OnDrawGizmosSelected()
        {
            if (!_drawGizmos) return;
            GetReferences();
            if (_attackHitbox == null || _comboController == null || _directionReference == null) return;

            if (Application.isPlaying && Time.time < _debugUntil)
            {
                DrawSearch(_lastCenter, _lastExtents, _lastRotation, _lastDirection, _lastDistance);
                return;
            }

            var attacks = _comboController.List_ComboAttackData;
            if (attacks == null || attacks.Count == 0) return;
            ComboAttackData attack = attacks[Mathf.Clamp(_debugAttackIndex, 0, attacks.Count - 1)];
            if (attack == null) return;
            DrawSearch(_attackHitbox.GetHitboxCenter(attack), attack.HitboxHalfExtents,
                _attackHitbox.GetHitboxRotation(), _directionReference.forward.normalized, 0f);
        }

        private void DrawSearch(Vector3 center, Vector3 extents, Quaternion rotation, Vector3 direction, float distance)
        {
            Matrix4x4 previousMatrix = Gizmos.matrix;
            Color previousColor = Gizmos.color;
            DrawBox(center, extents, rotation, _hitboxColor);
            DrawBox(center + direction * _maxDistance, extents, rotation, _searchColor);
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.color = _searchColor;
            for (int i = 0; i < 8; i++)
            {
                Vector3 corner = center + rotation * Vector3.Scale(extents,
                    new Vector3((i & 1) == 0 ? -1 : 1, (i & 2) == 0 ? -1 : 1, (i & 4) == 0 ? -1 : 1));
                Gizmos.DrawLine(corner, corner + direction * _maxDistance);
            }
            if (distance > 0f) DrawBox(center + direction * distance, extents, rotation, _destinationColor);
            Gizmos.matrix = previousMatrix;
            Gizmos.color = previousColor;
        }

        private static void DrawBox(Vector3 center, Vector3 extents, Quaternion rotation, Color color)
        {
            Gizmos.matrix = Matrix4x4.TRS(center, rotation, Vector3.one);
            Gizmos.color = color;
            Gizmos.DrawWireCube(Vector3.zero, extents * 2f);
        }

        #endregion
    }
}
