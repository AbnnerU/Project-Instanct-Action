using UnityEngine;

namespace Scripts.Combat
{
    public class ComboAnimationEventReceiver : MonoBehaviour
    {
        [SerializeField] private ComboControllerBase _comboController;
        private Animator _animator;
        private PostDashAttackController _postDashAttack;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _postDashAttack = GetComponentInParent<PostDashAttackController>();
            if (_comboController == null)
                _comboController = GetComponentInParent<ComboControllerBase>();
        }

        public void Attack_ExecuteHitbox(AnimationEvent animationEvent)
        {
            if (_postDashAttack != null && _postDashAttack.CanAcceptAnimationEvent(_animator, animationEvent))
            {
                _postDashAttack.ExecuteHitbox();
                return;
            }
            if (CanAccept(animationEvent))
                _comboController.AnimationEvent_ExecuteHitbox();
        }

        public void Attack_OpenComboWindow()
        {
            // Kept for old clips. Opening is now automatic in the controller.
        }

        public void Attack_CloseComboWindow(AnimationEvent animationEvent)
        {
            if (CanAccept(animationEvent))
                _comboController.AnimationEvent_CloseComboWindow();
        }

        public void Attack_Finish(AnimationEvent animationEvent)
        {
            if (_postDashAttack != null && _postDashAttack.CanAcceptAnimationEvent(_animator, animationEvent))
            {
                _postDashAttack.FinishAttack();
                return;
            }
            if (CanAccept(animationEvent))
                _comboController.AnimationEvent_FinishAttack();
        }

        private bool CanAccept(AnimationEvent animationEvent) =>
            _comboController != null && _comboController.CanAcceptAnimationEvent(_animator, animationEvent);
    }
}
