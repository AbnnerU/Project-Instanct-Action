using UnityEngine;

namespace Scripts.Combat
{
    public class ComboCancelOnHit : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour _hittableComponent;
        [SerializeField] private ComboControllerBase _comboController;

        private global::IHittable _hittable;

        private void Awake()
        {
            CacheHittable();

            if (_comboController == null)
                TryGetComponent(out _comboController);
        }

        private void OnEnable()
        {
            if (_hittable != null)
                _hittable.OnHitted += Hittable_OnHitted;
        }

        private void OnDisable()
        {
            if (_hittable != null)
                _hittable.OnHitted -= Hittable_OnHitted;
        }

        private void Hittable_OnHitted(int damage)
        {
            _comboController?.CancelAttack();
        }

        private void CacheHittable()
        {
            if (_hittableComponent != null)
            {
                _hittable = _hittableComponent as global::IHittable;
                return;
            }

            MonoBehaviour[] components = GetComponents<MonoBehaviour>();

            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] is not global::IHittable hittable) continue;

                _hittableComponent = components[i];
                _hittable = hittable;
                return;
            }
        }
    }
}
