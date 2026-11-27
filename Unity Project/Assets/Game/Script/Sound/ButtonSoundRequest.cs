using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Scripts.Sound
{
    public class ButtonSoundRequest : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Selectable _selectable;
        [SerializeField] private bool _muteWhenNotInteractable;
        [SerializeField] private PointerEventData.InputButton[] _blockedClickButtons =
        {
            PointerEventData.InputButton.Right,
            PointerEventData.InputButton.Middle
        };

        [Header("Sound")]
        [SerializeField] private AudioChannel _channel;
        [SerializeField] private AudioConfig _onClickAudioConfig;
        [SerializeField] private AudioConfig _onPointerEnterAudioConfig;
        [SerializeField] private AudioConfig _onPointerExitAudioConfig;
        public void OnPointerClick(PointerEventData eventData)
        {
            if (_blockedClickButtons != null)
                foreach (var button in _blockedClickButtons)
                    if (eventData.button == button)
                        return;

            Play(_onClickAudioConfig);
        }
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
