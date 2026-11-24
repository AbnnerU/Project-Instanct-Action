using UnityEngine;

namespace Scripts.Sound
{
    [CreateAssetMenu(fileName = "AudioConfig", menuName = "Assets/AudioConfig")]
    public class AudioConfig : ScriptableObject
    {
        public AudioClip audioClip;

        [SerializeField] private bool _randomClip;
        [SerializeField] private AudioClip[] _audioClips;

        [Header("Pitch")]
        [SerializeField] private float _pitch=1;
        [SerializeField] private bool _randomPitch;
        [SerializeField, Range(-3f, 3f)] private float _minPitch = 0.9f;
        [SerializeField, Range(-3f, 3f)] private float _maxPitch = 1.1f;
        [Header("General Configs")]
        [Range(0, 256)]
        public int priority = 128;

        [Range(0, 1)]
        public float volume=1;

        [Range(0, 1)]
        public float SpatialBlend=0;

        public bool loop=false;

        public AudioClip GetAudioClip()
        {
            if (!_randomClip || _audioClips == null || _audioClips.Length == 0)
                return audioClip;

            int validCount = 0;
            foreach (AudioClip clip in _audioClips)
            {
                if (clip != null)
                    validCount++;
            }

            if (validCount == 0)
                return audioClip;

            int index = Random.Range(0, validCount);
            foreach (AudioClip clip in _audioClips)
            {
                if (clip == null)
                    continue;

                if (index-- == 0)
                    return clip;
            }

            return audioClip;
        }

        public float GetPitch()
        {
            if (!_randomPitch)
                return _pitch;

            return Random.Range(_minPitch, _maxPitch);
        }
    }
}
