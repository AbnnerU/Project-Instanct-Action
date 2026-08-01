using UnityEngine;

namespace Scripts.Abilities
{
    public abstract class AbilityBase : MonoBehaviour, IHasActiveState
    {
        private bool _initialized;
        [SerializeField] protected bool _active;

        public bool Initialized => _initialized;

        public virtual void Initialize() => _initialized = true;

        public abstract void Execute();

        public virtual void Cancel()
        {
        }

        public virtual void Deactivate()
        {
            _initialized = false;
        }

        protected virtual void OnDestroy()
        {
            Deactivate();
        }

        public virtual void Disable() => _active = false;

        public virtual void Enable() => _active = true;
    }
}
