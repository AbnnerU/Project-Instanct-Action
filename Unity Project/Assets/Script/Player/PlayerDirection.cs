using System;
using UnityEngine;

namespace Scripts.Player
{
    public class PlayerDirection : MonoBehaviour
    {
        public event Action<DirectionSide> OnDirectionUpdated;

        [SerializeField] private DirectionSide _currentDirection = DirectionSide.RIGHT;
        [SerializeField] private Transform _visualTransform;
        [SerializeField] private bool _useLocalEulerAngles = true;
        [SerializeField] private DirectionRotationData _leftRotation = new DirectionRotationData(DirectionSide.LEFT, new Vector3(0f, -90f, 0f));
        [SerializeField] private DirectionRotationData _rightRotation = new DirectionRotationData(DirectionSide.RIGHT, new Vector3(0f, 90f, 0f));

        public DirectionSide CurrentDirection => _currentDirection;
        public Vector3 DirectionVector => _currentDirection == DirectionSide.RIGHT ? Vector3.right : Vector3.left;

        private void Awake()
        {
            UpdateVisualDirection();
        }

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
