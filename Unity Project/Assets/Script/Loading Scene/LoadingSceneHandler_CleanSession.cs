using UnityEngine;

namespace Scripts.LoadScene
{
    public class LoadingSceneHandler_CleanSession:MonoBehaviour
    {
        [SerializeField] private LoadingSceneHandler _loadingSceneHandlerComponent;

        private void Awake()
        {
            if(_loadingSceneHandlerComponent == null)
                _loadingSceneHandlerComponent =GetComponent<LoadingSceneHandler>();

            _loadingSceneHandlerComponent.OnCleanSessionRequired += CleanSession;
        }

        private bool CleanSession()
        {
            if (SIS_GameSession.Instance)
            {
                SIS_GameSession.Instance.ClearSession();
                return true;
            }
            else
            {
                Debug.LogError("[LoadingSceneHandler_CleanSession] Cant find any instace of SIS_GameSession");
                return false;
            }
        }
    }
}


