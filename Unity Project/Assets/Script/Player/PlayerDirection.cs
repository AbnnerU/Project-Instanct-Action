using System;
using UnityEngine;

namespace Scripts.Player
{
    public class PlayerDirection : MonoBehaviour
    {
        public event Action<DirectionSide> OnDirectionUpdated;

        [SerializeField] private DirectionSide _currentDirection = DirectionSide.RIGHT;
        [SerializeField] private Transform _visualTransform;
        [SerializeField] private bool _flipVisualByScale = true;

        public DirectionSide CurrentDirection => _currentDirection;
        public Vector3 DirectionVector => _currentDirection == DirectionSide.RIGHT ? Vector3.right : Vector3.left;

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
            if (_currentDirection == directionSide) return;

            _currentDirection = directionSide;
            UpdateVisualDirection();
            OnDirectionUpdated?.Invoke(_currentDirection);
        }

        private void UpdateVisualDirection()
        {
            if (!_flipVisualByScale || _visualTransform == null) return;

            Vector3 scale = _visualTransform.localScale;
            scale.x = Mathf.Abs(scale.x) * (_currentDirection == DirectionSide.RIGHT ? 1f : -1f);
            _visualTransform.localScale = scale;
        }
    }

    public enum DirectionSide
    {
        LEFT,
        RIGHT
    }
}
