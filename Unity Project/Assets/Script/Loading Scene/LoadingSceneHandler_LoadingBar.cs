using UnityEngine;
using UnityEngine.UI;

namespace Scripts.LoadScene
{
    public class LoadingSceneHandler_LoadingBar : MonoBehaviour
    {
        [SerializeField] private LoadingSceneHandler _loadingSceneHandlerComponent;
        [SerializeField] private Slider _sliderComponent;

        private void Awake()
        {
            if (_loadingSceneHandlerComponent == null)
                _loadingSceneHandlerComponent = GetComponent<LoadingSceneHandler>();

            _loadingSceneHandlerComponent.OnProgressPercentageUpdate += ProgressUpdate;
    
            _sliderComponent.minValue = 0;
            _sliderComponent.maxValue = 1;

            _sliderComponent.value = 0;
        }

        private void ProgressUpdate(float progress)
        {
            _sliderComponent.value = progress;
        }
    }
}