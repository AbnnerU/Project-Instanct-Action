using UnityEngine;

namespace Scripts.Animation
{
    public abstract class AnimationModuleBase : MonoBehaviour
    {
        [SerializeField] protected bool _active = true;
        [SerializeField] protected CharacterAnimationController _animationControllerComponent;

        protected virtual void Reset()
        {
            GetReferences();
        }

        protected virtual void Awake()
        {
            GetReferences();
        }

        public virtual void Enable()
        {
            _active = true;
        }

        public virtual void Disable()
        {
            _active = false;
        }

        protected virtual void GetReferences()
        {
            if (_animationControllerComponent == null)
                TryGetComponent(out _animationControllerComponent);
        }
    }
}
