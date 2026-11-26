using Scripts.DashHandler;
using UnityEngine;

namespace Scripts.Sound
{
    public class OnDashRequestSound : MonoBehaviour
    {
        [SerializeField] private DashBase _dash;
        [SerializeField] private AudioChannel _channel;
        [SerializeField] private AudioConfig _audioConfig;

        private void OnEnable()
        {
            if (_dash == null)
                _dash = GetComponent<DashBase>();

            if (_dash != null)
                _dash.OnDashStarted += HandleDashStarted;
        }

        private void OnDisable()
        {
            if (_dash != null)
                _dash.OnDashStarted -= HandleDashStarted;
        }

        private void HandleDashStarted(float direction)
        {
            if (_channel != null && _audioConfig != null)
                _channel.AudioRequest(_audioConfig, Vector3.zero);
        }
    }
}
