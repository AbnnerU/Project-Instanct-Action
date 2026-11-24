using System.Collections;
using UnityEngine;

namespace Scripts.Sound
{
    public class OnEnableRequestSound : MonoBehaviour
    {
        [SerializeField] private AudioChannel _channel;
        [SerializeField] private AudioConfig _audioConfig;
        [SerializeField] private Transform _positionReference;
        [Header("Delay")]
        [SerializeField] private bool _useDelay;
        [SerializeField] private float _delay;

        private void OnEnable()
        {
            if (_channel == null || _audioConfig == null)
                return;

            if (_useDelay)
                StartCoroutine(Delay());
            else
                Execute();
        }

        private void Execute()
        {
            Vector3 position = _positionReference != null ? _positionReference.position : Vector3.zero;
            _channel.AudioRequest(_audioConfig, position);
        }

        IEnumerator Delay()
        {
            yield return new WaitForSeconds(_delay);

            Execute();
        }
    }
}
