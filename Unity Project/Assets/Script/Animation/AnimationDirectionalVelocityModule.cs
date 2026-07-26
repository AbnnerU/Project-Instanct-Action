using UnityEngine;

namespace Scripts.Animation
{
    public class AnimationDirectionalVelocityModule : AnimationModuleBase
    {
        [Header("Velocity Provider")]
        [SerializeField] private MonoBehaviour _currentVelocityProviderComponent;
        [SerializeField] private Transform _referenceTransform;

        [Header("Animator Parameters")]
        [SerializeField] private string _moveXParameter = "MoveX";
        [SerializeField] private string _moveYParameter = "MoveY";
        [SerializeField] private bool _updateMoveSpeedParameter = true;
        [SerializeField] private string _moveSpeedParameter = "Speed";

        [Header("Velocity")]
        [SerializeField] private bool _ignoreVerticalVelocity = true;
        [SerializeField] private float _referenceSpeed = 4f;
        [SerializeField] private float _deadZone = 0.05f;
        [SerializeField] private float _dampTime = 0.12f;
        [SerializeField] private bool _clampDirectionalValues = true;

        private IHasCurrentVelocity _currentVelocityProvider;

        protected override void Awake()
        {
            base.Awake();
            CacheProvider();
            CacheReferenceTransform();
        }

        private void Update()
        {
            if (!_active || _animationControllerComponent == null || _currentVelocityProvider == null)
                return;

            Vector3 currentVelocity = _currentVelocityProvider.GetCurrentVelocity();

            if (_ignoreVerticalVelocity)
                currentVelocity.y = 0f;

            Vector2 directionalVelocity = GetDirectionalVelocity(currentVelocity);
            float normalizedSpeed = Mathf.Clamp01(directionalVelocity.magnitude);

            _animationControllerComponent.SetFloat(_moveXParameter, directionalVelocity.x, _dampTime);
            _animationControllerComponent.SetFloat(_moveYParameter, directionalVelocity.y, _dampTime);

            if (_updateMoveSpeedParameter)
                _animationControllerComponent.SetFloat(_moveSpeedParameter, normalizedSpeed, _dampTime);
        }

        public void SetVelocityProvider(IHasCurrentVelocity currentVelocityProvider)
        {
            _currentVelocityProvider = currentVelocityProvider;
        }

        public void SetReferenceTransform(Transform referenceTransform)
        {
            _referenceTransform = referenceTransform;
        }

        private Vector2 GetDirectionalVelocity(Vector3 currentVelocity)
        {
            if (_referenceSpeed <= 0f)
                return Vector2.zero;

            Vector3 localVelocity = GetLocalVelocity(currentVelocity);
            Vector2 directionalVelocity = new Vector2(localVelocity.x, localVelocity.z) / _referenceSpeed;

            if (_clampDirectionalValues && directionalVelocity.magnitude > 1f)
                directionalVelocity = directionalVelocity.normalized;

            if (directionalVelocity.magnitude < _deadZone)
                return Vector2.zero;

            return directionalVelocity;
        }

        private Vector3 GetLocalVelocity(Vector3 currentVelocity)
        {
            if (_referenceTransform == null)
                return transform.InverseTransformDirection(currentVelocity);

            return _referenceTransform.InverseTransformDirection(currentVelocity);
        }

        private void CacheProvider()
        {
            _currentVelocityProvider = _currentVelocityProviderComponent as IHasCurrentVelocity;

            if (_currentVelocityProvider != null)
                return;

            _currentVelocityProvider = FindVelocityProvider(GetComponents<MonoBehaviour>());

            if (_currentVelocityProvider != null)
                return;

            _currentVelocityProvider = FindVelocityProvider(GetComponentsInParent<MonoBehaviour>(true));

            if (_currentVelocityProvider != null)
                return;

            _currentVelocityProvider = FindVelocityProvider(GetComponentsInChildren<MonoBehaviour>(true));
        }

        private IHasCurrentVelocity FindVelocityProvider(MonoBehaviour[] components)
        {
            foreach (MonoBehaviour component in components)
            {
                if (component is not IHasCurrentVelocity provider) continue;

                _currentVelocityProviderComponent = component;
                return provider;
            }

            return null;
        }

        private void CacheReferenceTransform()
        {
            if (_referenceTransform != null)
                return;

            if (_currentVelocityProviderComponent is IHasAnimationReferenceTransform animationReferenceProvider)
            {
                _referenceTransform = animationReferenceProvider.GetAnimationReferenceTransform();

                if (_referenceTransform != null)
                    return;
            }

            if (_currentVelocityProviderComponent != null)
            {
                _referenceTransform = _currentVelocityProviderComponent.transform;
                return;
            }

            _referenceTransform = transform;
        }
    }
}
