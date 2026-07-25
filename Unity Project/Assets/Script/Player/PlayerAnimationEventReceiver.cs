using UnityEngine;

namespace Scripts.Player
{
    public class PlayerAnimationEventReceiver : MonoBehaviour
    {
        [SerializeField] private PlayerComboController _comboController;

        private void Awake()
        {
            if (_comboController == null)
                _comboController = GetComponentInParent<PlayerComboController>();
        }

        public void Attack_ExecuteHitbox()
        {
            _comboController?.AnimationEvent_ExecuteHitbox();
        }

        public void Attack_OpenComboWindow()
        {
            _comboController?.AnimationEvent_OpenComboWindow();
        }

        public void Attack_CloseComboWindow()
        {
            _comboController?.AnimationEvent_CloseComboWindow();
        }

        public void Attack_Finish()
        {
            _comboController?.AnimationEvent_FinishAttack();
        }
    }
}
