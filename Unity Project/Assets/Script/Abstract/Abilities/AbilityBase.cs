using System;
using UnityEngine;

namespace Scripts.Abilities
{
    public abstract class AbilityBase : MonoBehaviour, IHasActiveState
    {
        public Action<AbilityBase> OnAbilityStarted;
        public Action<AbilityBase> OnAbilityFinished;
        public event Action<AbilityBase> OnAbilityDeactivating;

        private bool _initialized;
        private bool _deactivated;
        [SerializeField] protected bool _active;

        public bool Initialized => _initialized;

        public virtual void Initialize()
        {
            _deactivated = false;
            _initialized = true;
        }

        public abstract void Execute();

        public virtual void Cancel()
        {
        }

        public void Deactivate()
        {
            if (_deactivated) return;
            _deactivated = true;
            _initialized = false;
            _active = false;
            try { Cancel(); }
            catch (Exception exception) { Debug.LogException(exception, this); }
            if (OnAbilityDeactivating != null)
                foreach (Action<AbilityBase> listener in OnAbilityDeactivating.GetInvocationList())
                    try { listener(this); }
                    catch (Exception exception) { Debug.LogException(exception, this); }
            try { OnDeactivate(); }
            catch (Exception exception) { Debug.LogException(exception, this); }
            finally { StopAllCoroutines(); }
        }

        protected virtual void OnDeactivate() { }

        protected virtual void OnDestroy()
        {
            Deactivate();
        }


        protected virtual bool CanExecute()
        {
            return _initialized && !_deactivated && _active;
        }

        public virtual void Disable() => _active = false;

        public virtual void Enable()
        {
            if (!_deactivated) _active = true;
        }
    }
}
