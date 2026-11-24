using UnityEngine;
using Random = UnityEngine.Random;

namespace Scripts.Animation
{
    public class HitReaction_TriggerParameter : AnimationModuleBase
    {
        [SerializeField] private GameObject _hittableReference;
        [Header("Animator Parameters")]
        [SerializeField] private string _hittedParameter = "Hitted";
        [SerializeField] private bool _cancelParameterIfIsDeath = true;
        [Min(0f)]
        [SerializeField] private float _transitionDuration = 0.05f;
        [Header("Randomize")]
        [SerializeField] private bool _randomize;
        [SerializeField] private bool _doNotRepeat;
        [SerializeField] private string _randomParamenter;
        [SerializeField, Min(1)] private int _randomRange = 1;

        private int _lastRandomIndex = -1;
        private IHittable _hittableHandler;

        private void Start()
        {
            _hittableHandler = _hittableReference.GetComponent<IHittable>();

            if (_hittableHandler != null)
                _hittableHandler.OnHitted += OnHitted;

        }

        private void OnHitted(int damage, bool isAlive)
        {
            if (!_active || _animationControllerComponent == null || (_cancelParameterIfIsDeath && isAlive == false))
                return;

            if (_randomize)
            {
                _animationControllerComponent.SetInteger(_randomParamenter, GetRandomIndex());
            }

            _animationControllerComponent.SetTrigger(_hittedParameter);
        }

        private int GetRandomIndex()
        {
            int count = Mathf.Max(1, _randomRange);
            int index;

            if (_doNotRepeat && count > 1 && _lastRandomIndex >= 0 && _lastRandomIndex < count)
            {
                index = Random.Range(0, count - 1);

                if (index >= _lastRandomIndex) 
                    index++;
            }
            else         
                index = Random.Range(0, count);
            
            _lastRandomIndex = index;
            return index;
        }
    }
}
