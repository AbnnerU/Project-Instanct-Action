using UnityEngine;

namespace Scripts.Sound
{
    public class AttackSwingSound : MonoBehaviour
    {
        [SerializeField] private AudioChannel _channel;
        [SerializeField] private AudioConfig _audioConfig;

        public void SetAudioConfig(AudioConfig config) => _audioConfig = config;

        // Called by an Animation Event on the same GameObject as the Animator.
        public void PlaySwingSound()
        {
            if (!isActiveAndEnabled || _channel == null || _audioConfig == null)
                return;

            _channel.AudioRequest(_audioConfig, Vector3.zero);
        }
    }
}
