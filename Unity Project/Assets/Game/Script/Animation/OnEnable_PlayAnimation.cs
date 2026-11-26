using UnityEngine;

namespace Scripts.Animation
{
    public class OnEnable_PlayAnimation : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private string _animationName;

        private void OnEnable()
        {
            if (_animator == null || string.IsNullOrWhiteSpace(_animationName))
                return;

            _animator.Play(_animationName, 0, 0f);
        }
    }
}
