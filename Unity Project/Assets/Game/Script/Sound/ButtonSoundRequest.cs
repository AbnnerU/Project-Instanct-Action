using UnityEngine;
using UnityEngine.EventSystems;

namespace Scripts.Sound
{
    public class ButtonSoundRequest : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Sound")]
        [SerializeField] private AudioChannel _channel;
        [SerializeField] private AudioConfig _onClickAudioConfig;
        [SerializeField] private AudioConfig _onPointerEnterAudioConfig;
        [SerializeField] private AudioConfig _onPointerExitAudioConfig;
        public void OnPointerClick(PointerEventData eventData)
        {
            if (_onClickAudioConfig != null)
                _channel.AudioRequest(_onClickAudioConfig, Vector3.zero);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_onPointerEnterAudioConfig != null)
                _channel.AudioRequest(_onPointerEnterAudioConfig, Vector3.zero); 
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_onPointerExitAudioConfig != null)
                _channel.AudioRequest(_onPointerExitAudioConfig, Vector3.zero);
        }
    }
}
