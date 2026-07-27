using Scripts.Combat;
using UnityEngine;

namespace Scripts.Animation
{
    public class HitReaction_TriggerParameter : AnimationModuleBase
    {
        [SerializeField] private GameObject _hittableReference;
        [Header("Animator Parameters")]
        [SerializeField] private string _hittedParameter = "Hitted";


        private IHittable _hittableHandler;

        private void Start()
        {
            _hittableHandler = _hittableReference.GetComponent<IHittable>();

            if (_hittableHandler != null)
                _hittableHandler.OnHitted += OnHitted;
        }

        private void OnHitted(int obj) => _animationControllerComponent.SetTrigger(_hittedParameter);
    }
}