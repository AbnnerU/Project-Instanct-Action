using System;
using UnityEngine;

namespace Scripts.Combat
{
    public abstract class AreaDamageRequestBase : MonoBehaviour
    {
        public event Action OnPrepared;
        public event Action OnExecuted;
        [SerializeField] private GameObject _markVisual;
        [SerializeField] private GameObject _explosionVisual;
        protected DamageRequest Request { get; private set; }
        private bool _prepared;

        public virtual void Setup(DamageRequest request)
        {
            Request = request;
            _prepared = true;

            if (_markVisual != null) 
                _markVisual.SetActive(true);

            if (_explosionVisual != null) 
                _explosionVisual.SetActive(false);

            OnPrepared?.Invoke();
        }

        public void Execute()
        {
            if (!_prepared) return;

            _prepared = false;

            if (_markVisual != null) 
                _markVisual.SetActive(false);

            if (_explosionVisual != null) 
                _explosionVisual.SetActive(true);

            ApplyDamage();

            OnExecuted?.Invoke();
        }

        protected abstract void ApplyDamage();

        protected virtual void OnDisable()
        {
            _prepared = false;
            Request = default;
        }
    }
}
