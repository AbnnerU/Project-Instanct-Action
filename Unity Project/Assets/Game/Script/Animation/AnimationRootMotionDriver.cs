using UnityEngine;

namespace Scripts.Animation
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public class AnimationRootMotionDriver : MonoBehaviour
    {
        #region Fields

        [SerializeField] private Transform _targetTransform;
        [Header("World Axes")]
        [SerializeField] private bool _usePositionX = true;
        [SerializeField] private bool _usePositionY;
        [SerializeField] private bool _usePositionZ;
        [Header("Movement")]
        [SerializeField, Min(0f)] private float _positionInfluence = 100f;
        //[SerializeField] private bool _forwardOnly = true;
        [SerializeField] private bool _applyRotation;
        //[Header("Optional Normalized Time Window")]
        //[SerializeField] private bool _useTimeWindow;
        //[SerializeField, Range(0f, 1f)] private float _windowStart;
        //[SerializeField, Range(0f, 1f)] private float _windowEnd = 1f;

        private Animator _animator;
        private Vector3 _direction;
        private string _stateName;
        private int _layer;

        public bool IsMotionActive { get; private set; }

        #endregion

        #region Lifecycle

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            // Keep extraction enabled. OnAnimatorMove owns application, including discarding idle motion.
            _animator.applyRootMotion = true;
        }

        private void OnDisable() => StopMotion();

        #endregion

        #region Motion

        public void BeginMotion(Vector3 direction, string stateName, int layer = 0)
        {
            _direction = FilterAxes(direction).normalized;
            _stateName = stateName;
            _layer = layer;
            IsMotionActive = _targetTransform != null && !string.IsNullOrEmpty(stateName);
        }

        public void StopMotion() => IsMotionActive = false;

        private void OnAnimatorMove()
        {
            //if(!IsMotionActive) return;
            //if (!isActiveAndEnabled || !IsMotionActive || _targetTransform == null ||
            //    _animator == null || Time.timeScale <= 0f || _layer < 0 || _layer >= _animator.layerCount)
            //    return;

            // Ignore blends into dash, hit reactions or idle, even before the attack bridge updates.

            //AnimatorStateInfo state = _animator.IsInTransition(_layer)
            //    ? _animator.GetNextAnimatorStateInfo(_layer)
            //    : _animator.GetCurrentAnimatorStateInfo(_layer);
            //if (!state.IsName(_stateName)) return;

            //if (_useTimeWindow && (state.normalizedTime < _windowStart || state.normalizedTime > _windowEnd))
            //    return;

            _targetTransform.position += FilterDisplacement(_animator.deltaPosition);
            if (_applyRotation)
                _targetTransform.rotation *= _animator.deltaRotation;
        }

        public Vector3 FilterDisplacement(Vector3 displacement)
        {
            displacement = FilterAxes(displacement);
            //if (_forwardOnly)
            //    displacement = _direction * Mathf.Max(0f, Vector3.Dot(displacement, _direction));
            return displacement * (Mathf.Max(0f, _positionInfluence) / 100f);
        }

        private Vector3 FilterAxes(Vector3 value) => new Vector3(
            _usePositionX ? value.x : 0f,
            _usePositionY ? value.y : 0f,
            _usePositionZ ? value.z : 0f);

        //private void OnValidate()
        //{
        //    _windowEnd = Mathf.Max(_windowStart, _windowEnd);
        //}

        #endregion
    }
}
