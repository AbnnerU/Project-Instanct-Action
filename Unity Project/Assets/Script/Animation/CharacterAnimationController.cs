using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Animation
{
    public class CharacterAnimationController : MonoBehaviour
    {
        [SerializeField] private Animator _animatorComponent;
        [SerializeField] private bool _logMissingParameters;

        private readonly Dictionary<int, AnimatorControllerParameterType> _dict_AnimatorParameterTypes =
            new Dictionary<int, AnimatorControllerParameterType>();

        private void Reset()
        {
            GetReferences();
        }

        private void Awake()
        {
            GetReferences();
            CacheAnimatorParameters();
        }

        public void RefreshParameterCache()
        {
            CacheAnimatorParameters();
        }

        public bool SetFloat(string parameterName, float value)
        {
            if (!TryGetValidParameterHash(parameterName, AnimatorControllerParameterType.Float, out int parameterHash))
                return false;

            _animatorComponent.SetFloat(parameterHash, value);
            return true;
        }

        public bool SetFloat(string parameterName, float value, float dampTime)
        {
            if (!TryGetValidParameterHash(parameterName, AnimatorControllerParameterType.Float, out int parameterHash))
                return false;

            _animatorComponent.SetFloat(parameterHash, value, dampTime, Time.deltaTime);
            return true;
        }

        public bool SetBool(string parameterName, bool value)
        {
            if (!TryGetValidParameterHash(parameterName, AnimatorControllerParameterType.Bool, out int parameterHash))
                return false;

            _animatorComponent.SetBool(parameterHash, value);
            return true;
        }

        public bool SetInteger(string parameterName, int value)
        {
            if (!TryGetValidParameterHash(parameterName, AnimatorControllerParameterType.Int, out int parameterHash))
                return false;

            _animatorComponent.SetInteger(parameterHash, value);
            return true;
        }

        public bool SetTrigger(string parameterName)
        {
            if (!TryGetValidParameterHash(parameterName, AnimatorControllerParameterType.Trigger, out int parameterHash))
                return false;

            _animatorComponent.SetTrigger(parameterHash);
            return true;
        }

        public bool ResetTrigger(string parameterName)
        {
            if (!TryGetValidParameterHash(parameterName, AnimatorControllerParameterType.Trigger, out int parameterHash))
                return false;

            _animatorComponent.ResetTrigger(parameterHash);
            return true;
        }

        public bool HasParameter(string parameterName)
        {
            if (string.IsNullOrWhiteSpace(parameterName))
                return false;

            int parameterHash = Animator.StringToHash(parameterName);
            return _dict_AnimatorParameterTypes.ContainsKey(parameterHash);
        }

        private void GetReferences()
        {
            if (_animatorComponent == null)
                TryGetComponent(out _animatorComponent);
        }

        private void CacheAnimatorParameters()
        {
            _dict_AnimatorParameterTypes.Clear();

            if (_animatorComponent == null)
                return;

            foreach (AnimatorControllerParameter parameter in _animatorComponent.parameters)
                _dict_AnimatorParameterTypes[parameter.nameHash] = parameter.type;
        }

        private bool TryGetValidParameterHash(string parameterName, AnimatorControllerParameterType expectedType, out int parameterHash)
        {
            parameterHash = 0;

            if (_animatorComponent == null || string.IsNullOrWhiteSpace(parameterName))
                return false;

            parameterHash = Animator.StringToHash(parameterName);

            if (!_dict_AnimatorParameterTypes.TryGetValue(parameterHash, out AnimatorControllerParameterType parameterType))
            {
                if (_logMissingParameters)
                    Debug.LogWarning($"[CharacterAnimationController] Animator parameter '{parameterName}' ({expectedType}) was not found.", this);

                return false;
            }

            if (parameterType == expectedType)
                return true;

            if (_logMissingParameters)
                Debug.LogWarning($"[CharacterAnimationController] Animator parameter '{parameterName}' is {parameterType}, but {expectedType} was requested.", this);

            return false;
        }
    }
}
