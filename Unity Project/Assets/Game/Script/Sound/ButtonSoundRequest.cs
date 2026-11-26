using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Scripts.Sound
{
    public class ButtonSoundRequest : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Selectable _selectable;
        [SerializeField] private bool _muteWhenNotInteractable;

        [Header("Sound")]
        [SerializeField] private AudioChannel _channel;
        [SerializeField] private AudioConfig _onClickAudioConfig;
        [SerializeField] private AudioConfig _onPointerEnterAudioConfig;
        [SerializeField] private AudioConfig _onPointerExitAudioConfig;
        public void OnPointerClick(PointerEventData eventData) => Play(_onClickAudioConfig);
        public void OnPointerEnter(PointerEventData eventData) => Play(_onPointerEnterAudioConfig);
        public void OnPointerExit(PointerEventData eventData) => Play(_onPointerExitAudioConfig);

        private void Play(AudioConfig config)
        {
            if (_channel == null || config == null)
                return;
            if (_muteWhenNotInteractable && _selectable != null && !_selectable.IsInteractable())
                return;

            _channel.AudioRequest(config, Vector3.zero);
        }
    }
}
