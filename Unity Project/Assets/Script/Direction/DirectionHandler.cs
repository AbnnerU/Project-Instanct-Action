using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.Direction
{
    public class DirectionHandler: MonoBehaviour, IHasActiveState
    {
        #region Fields

        public event Action<DirectionSide> OnDirectionUpdated;

        [SerializeField] private bool _active = true;
        [Header("Optional Input")]
        [SerializeField] private InputActionReference _rightInput;
        [SerializeField] private InputActionReference _leftInput;
        [Header("Direction")]
        [SerializeField] private DirectionSide _currentDirection = DirectionSide.RIGHT;
        [SerializeField] private Transform _visualTransform;
        [SerializeField] private bool _useLocalEulerAngles = true;
        [SerializeField] private DirectionRotationData _leftRotation = new DirectionRotationData(DirectionSide.LEFT, new Vector3(0f, -90f, 0f));
        [SerializeField] private DirectionRotationData _rightRotation = new DirectionRotationData(DirectionSide.RIGHT, new Vector3(0f, 90f, 0f));

        public DirectionSide CurrentDirection => _currentDirection;
        public Vector3 DirectionVector => _currentDirection == DirectionSide.RIGHT ? Vector3.right : Vector3.left;
        public bool Active => _active && _hashSet_DirectionBlockers.Count == 0;

        private readonly HashSet<UnityEngine.Object> _hashSet_DirectionBlockers = new HashSet<UnityEngine.Object>();

        #endregion

        #region Unity

        private void Awake()
        {
            UpdateVisualDirection();
        }

        private void OnEnable()
        {
            if (_rightInput != null && _rightInput.action != null)
            {
                _rightInput.action.performed += RightInput_Performed;
                _rightInput.action.Enable();
            }
            if (_leftInput != null && _leftInput.action != null)
            {
                _leftInput.action.performed += LeftInput_Performed;
                _leftInput.action.Enable();
            }
        }

        private void OnDisable()
        {
            if (_rightInput != null && _rightInput.action != null)
                _rightInput.action.performed -= RightInput_Performed;
            if (_leftInput != null && _leftInput.action != null)
                _leftInput.action.performed -= LeftInput_Performed;
        }

        #endregion

        #region Input And Blocking

        private void RightInput_Performed(InputAction.CallbackContext context)
        {
            if (Time.timeScale > 0f)
                LookRight();
        }

        private void LeftInput_Performed(InputAction.CallbackContext context)
        {
            if (Time.timeScale > 0f)
                LookLeft();
        }

        public void Enable() => _active = true;
        public void Disable() => _active = false;

        public void BlockDirection(UnityEngine.Object owner)
        {
            if (owner != null)
                _hashSet_DirectionBlockers.Add(owner);
        }

        public void UnblockDirection(UnityEngine.Object owner)
        {
            _hashSet_DirectionBlockers.Remove(owner);
        }

        #endregion

        #region Direction

        public void LookRight()
        {
            SetDirection(DirectionSide.RIGHT);
        }

        public void LookLeft()
        {
            SetDirection(DirectionSide.LEFT);
        }

        public void SetDirection(DirectionSide directionSide)
        {
            if (!Active)
                return;

            if (_currentDirection == directionSide)
            {
                UpdateVisualDirection();
                return;
            }

            _currentDirection = directionSide;
            UpdateVisualDirection();
            OnDirectionUpdated?.Invoke(_currentDirection);
        }

        private void UpdateVisualDirection()
        {
            if (_visualTransform == null) return;

            DirectionRotationData rotationData = GetDirectionRotationData();

            if (_useLocalEulerAngles)
                _visualTransform.localEulerAngles = rotationData.EulerAngles;
            else
                _visualTransform.eulerAngles = rotationData.EulerAngles;
        }

        private DirectionRotationData GetDirectionRotationData()
        {
            return _currentDirection == DirectionSide.RIGHT ? _rightRotation : _leftRotation;
        }

        #endregion
    }

    public enum DirectionSide
    {
        LEFT,
        RIGHT
    }

    [Serializable]
    public class DirectionRotationData
    {
        public DirectionSide DirectionSide;
        public Vector3 EulerAngles;

        public DirectionRotationData(DirectionSide directionSide, Vector3 eulerAngles)
        {
            DirectionSide = directionSide;
            EulerAngles = eulerAngles;
        }
    }
}
