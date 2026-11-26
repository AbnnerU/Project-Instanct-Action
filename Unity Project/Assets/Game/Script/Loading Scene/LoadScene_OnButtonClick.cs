using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Scripts.LoadScene
{
    public class LoadScene_OnButtonClick : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private AssetReference _scene;
        [SerializeField] private bool _useDelay;
        [SerializeField, Min(0f)] private float _delay;
        [SerializeField] private UnityEvent _onClick = new UnityEvent();

        private bool _isLoading;

        private void Awake()
        {
            if (_button == null)
                _button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            if (_button != null)
                _button.onClick.AddListener(Load);
        }

        private void OnDisable()
        {
            if (_button != null)
                _button.onClick.RemoveListener(Load);

            StopAllCoroutines();
            _isLoading = false;
        }

        public void Load()
        {
            if (_isLoading || !isActiveAndEnabled)
                return;

            _isLoading = true;
            _onClick.Invoke();
            if (isActiveAndEnabled)
                StartCoroutine(LoadRoutine());
        }

        private IEnumerator LoadRoutine()
        {
            if (_useDelay && _delay > 0f)
                yield return new WaitForSecondsRealtime(_delay);

            if (_scene == null || !_scene.RuntimeKeyIsValid() || SIS_LoadingSceneChannel.Instance == null)
            {
                Debug.LogError("[LoadScene_OnButtonClick] Missing scene or loading channel.", this);
                _isLoading = false;
                yield break;
            }

            SIS_LoadingSceneChannel.Instance.Load(_scene);
        }
    }
}
