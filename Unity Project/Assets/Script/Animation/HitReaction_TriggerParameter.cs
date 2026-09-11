using Scripts.Combat;
using UnityEngine;

namespace Scripts.Animation
{
    public class HitReaction_TriggerParameter : AnimationModuleBase
    {
        [SerializeField] private GameObject _hittableReference;
        [Header("Animator Parameters")]
        [SerializeField] private string _hittedParameter = "Hitted";
        [Header("Immediate Reaction")]
        [Tooltip("Optional full state name. When set, interrupts the current blend instead of waiting for the trigger transition.")]
        [SerializeField] private string _hittedStateName;
        [Min(0f)]
        [SerializeField] private float _transitionDuration = 0.05f;


        private IHittable _hittableHandler;

        private void Start()
        {
            _hittableHandler = _hittableReference.GetComponent<IHittable>();

            if (_hittableHandler != null)
                _hittableHandler.OnHitted += OnHitted;
        }

        private void OnHitted(int damage)
        {
            if (!_active || _animationControllerComponent == null)
                return;

            if (!string.IsNullOrWhiteSpace(_hittedStateName))
            {
                _animationControllerComponent.ResetTrigger(_hittedParameter);
                if (_animationControllerComponent.CrossFadeInFixedTime(_hittedStateName, _transitionDuration))
                    return;
            }

            _animationControllerComponent.SetTrigger(_hittedParameter);
        }
    }
}
