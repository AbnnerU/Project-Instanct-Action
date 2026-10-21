using Scripts.Direction;
using System.Collections;
using UnityEngine;

namespace Scripts.Combat
{
    public class Combo_DirectionHandler : MonoBehaviour
    {
        #region Fields

        [SerializeField] private ComboControllerBase _comboController;
        [SerializeField] private DirectionHandler _directionHandler;
        [SerializeField] private bool _waitFrame;

        #endregion

        #region Unity

        private void Awake()
        {
            if (_comboController == null)
                TryGetComponent(out _comboController);

            if (_directionHandler == null)
                TryGetComponent(out _directionHandler);
        }

        private void OnEnable()
        {
            if (_comboController == null)
                return;

            _comboController.OnAttackStarted += BlockDirection;
            _comboController.OnAttackFinished += UnblockDirection;
            _comboController.OnAttackCanceled += UnblockDirection;
            _comboController.OnComboStopped += UnblockDirection;

            if (_comboController.IsAttacking)
                BlockDirection();
        }

        private void OnDisable()
        {
            if (_comboController != null)
            {
                _comboController.OnAttackStarted -= BlockDirection;
                _comboController.OnAttackFinished -= UnblockDirection;
                _comboController.OnAttackCanceled -= UnblockDirection;
                _comboController.OnComboStopped -= UnblockDirection;
            }

            UnblockDirection();
        }

        #endregion

        #region Blocking

        private void BlockDirection()
        {
            if (_directionHandler != null)
            {
                if (_waitFrame)
                    StartCoroutine(Wait());
                else
                    _directionHandler.BlockDirection(this);
            }
        }

        IEnumerator Wait()
        {
            yield return new WaitForEndOfFrame();
            _directionHandler.BlockDirection(this);
        }

        private void UnblockDirection()
        {
            if (_directionHandler != null)
                _directionHandler.UnblockDirection(this);
        }

        #endregion
    }
}
