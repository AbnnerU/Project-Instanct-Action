using UnityEngine;

namespace Scripts.Animation
{
    public class AnimationVelocityModule : AnimationModuleBase
    {
        [Header("Velocity Provider")]
        [SerializeField] private MonoBehaviour _currentVelocityProviderComponent;

        [Header("Animator Parameters")]
        [SerializeField] private string _moveSpeedParameter = "MoveSpeed";

        [Header("Velocity")]
        [SerializeField] private bool _ignoreVerticalVelocity = true;
        [SerializeField] private float _referenceSpeed = 4f;
        [SerializeField] private float _deadZone = 0.05f;
        [SerializeField] private float _dampTime = 0.12f;

        private IHasCurrentVelocity _currentVelocityProvider;

        protected override void Awake()
        {
            base.Awake();
            CacheProvider();
        }

        private void Update()
        {
            if (!_active || _animationControllerComponent == null || _currentVelocityProvider == null)
                return;

            Vector3 currentVelocity = _currentVelocityProvider.GetCurrentVelocity();

            if (_ignoreVerticalVelocity)
                currentVelocity.y = 0f;

            float normalizedSpeed = GetNormalizedSpeed(currentVelocity);
            _animationControllerComponent.SetFloat(_moveSpeedParameter, normalizedSpeed, _dampTime);
        }

        public void SetVelocityProvider(IHasCurrentVelocity currentVelocityProvider)
        {
            _currentVelocityProvider = currentVelocityProvider;
        }

        protected override void GetReferences()
        {
            base.GetReferences();
        }

        private void CacheProvider()
        {
            _currentVelocityProvider = _currentVelocityProviderComponent as IHasCurrentVelocity;

            if (_currentVelocityProvider != null)
                return;

            MonoBehaviour[] components = GetComponents<MonoBehaviour>();

            foreach (MonoBehaviour component in components)
            {
                if (component is not IHasCurrentVelocity provider) continue;

                _currentVelocityProvider = provider;
                return;
            }
        }

        private float GetNormalizedSpeed(Vector3 currentVelocity)
        {
            if (_referenceSpeed <= 0f)
                return 0f;

            float normalizedSpeed = Mathf.Clamp01(currentVelocity.magnitude / _referenceSpeed);

            if (normalizedSpeed < _deadZone)
                return 0f;

            return normalizedSpeed;
        }
    }
}
