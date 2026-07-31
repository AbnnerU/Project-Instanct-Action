using UnityEngine;

namespace Scripts.Abilities
{
    public abstract class AbilityBase : MonoBehaviour
    {
        private bool _initialized;

        public bool Initialized => _initialized;

        public virtual void Initialize()
        {
            _initialized = true;
        }

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
    }
}
