using UnityEngine;

namespace Scripts.Combat
{
    public class ComboAnimationEventReceiver : MonoBehaviour
    {
        [SerializeField] private ComboControllerBase _comboController;

        private void Awake()
        {
            if (_comboController == null)
                _comboController = GetComponentInParent<ComboControllerBase>();
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
