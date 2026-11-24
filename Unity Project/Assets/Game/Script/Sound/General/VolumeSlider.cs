using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Sound
{
    public class VolumeSlider : MonoBehaviour
    {
        [SerializeField] private Slider slider;

        [Header("Type")]
        [SerializeField] private bool musicSlider;
        [SerializeField] private bool soundEffectsSlider;
        [SerializeField] private bool ambientEffectsSlider;

        [Header("Config")]
        [SerializeField] private bool updateOnEnable=true;
        [SerializeField] private bool updateOnDisable;

        private void Awake()
        {
            if(musicSlider)
                slider.onValueChanged.AddListener(SetNewMusicVolume);
            else if(soundEffectsSlider)
                slider.onValueChanged.AddListener(SetNewSoundEffectVolume);
            else if (ambientEffectsSlider)
                slider.onValueChanged.AddListener(SetNewAmbientEffectVolume);
        }

        private void OnEnable()
        {
            if (updateOnEnable)
            {
                if (musicSlider)
                    slider.value = SoundsSettings.GetMusicVolume();
                else if (soundEffectsSlider)
                    slider.value = SoundsSettings.GetSoundEffectVolume();
                else if (ambientEffectsSlider)
                    slider.value = SoundsSettings.GetAmbientEffectVolume();
            }

        }

        private void OnDisable()
        {
            if (updateOnDisable)
            {
                if (musicSlider)
                    slider.value = SoundsSettings.GetMusicVolume();
                else if (soundEffectsSlider)
                    slider.value = SoundsSettings.GetSoundEffectVolume();
                else if (ambientEffectsSlider)
                    slider.value = SoundsSettings.GetAmbientEffectVolume();
            }
        }

        public void SetNewMusicVolume(float newValue)
        {
            SIS_SoundsManager.Instance?.SetNewMusicVolume(newValue);
        }

        public void SetNewSoundEffectVolume(float newValue)
        {
            SIS_SoundsManager.Instance?.SetNewSoundEffectVolume(newValue);
        }

        private void OnValidate()
        {
            if (musicSlider)
            {
                soundEffectsSlider = false;
                ambientEffectsSlider = false;
            }
            else if (soundEffectsSlider)
            {
                musicSlider = false;
                ambientEffectsSlider = false;
            }
        }

        public void SetNewAmbientEffectVolume(float newValue)
        {
            if (SIS_SoundsManager.Instance != null)
                SIS_SoundsManager.Instance.SetNewAmbientEffectVolume(newValue);
        }
    }
}
