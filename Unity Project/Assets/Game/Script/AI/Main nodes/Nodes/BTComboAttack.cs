using System.Collections;
using Scripts.Combat;
using UnityEngine;

namespace Scripts.BT
{
    public class BTComboAttack : BTnode
    {
        private readonly ComboControllerBase _comboController;
        private readonly float _maxAttackDuration;
        private bool _comboFinished;
        private bool _comboCanceled;

        public BTComboAttack(ComboControllerBase comboController, float maxAttackDuration)
        {
            _comboController = comboController;
            _maxAttackDuration = maxAttackDuration;
        }

        public override IEnumerator Run(BehaviorTree behaviorTree)
        {
            status = BTstatus.RUNNING;

            if (_comboController == null || !_comboController.Active)
            {
                status = BTstatus.FAILURE;
                yield break;
            }

            _comboFinished = false;
            _comboCanceled = false;

            _comboController.OnComboStopped += ComboController_OnComboStopped;
            _comboController.OnAttackCanceled += ComboController_OnAttackCanceled;

            _comboController.RequestAttack();

            if (!_comboController.IsAttacking)
            {
                Unsubscribe();
                status = BTstatus.FAILURE;
                yield break;
            }

            float elapsedTime = 0f;

            while (!_comboFinished && !_comboCanceled)
            {
                elapsedTime += Time.deltaTime;

                if (_maxAttackDuration > 0f && elapsedTime >= _maxAttackDuration)
                {
                    _comboController.CancelAttack();
                    _comboCanceled = true;
                    Debug.LogWarning("[BTComboAttack] Attack timed out. Check if the animation clip calls Attack_Finish.", _comboController);
                    break;
                }

                yield return null;
            }

            Unsubscribe();
            status = _comboFinished ? BTstatus.SUCCESS : BTstatus.FAILURE;
        }

        private void ComboController_OnComboStopped()
        {
            _comboFinished = true;
        }

        private void ComboController_OnAttackCanceled()
        {
            _comboCanceled = true;
        }

        private void Unsubscribe()
        {
            if (_comboController == null) return;

            _comboController.OnComboStopped -= ComboController_OnComboStopped;
            _comboController.OnAttackCanceled -= ComboController_OnAttackCanceled;
        }
    }
}
