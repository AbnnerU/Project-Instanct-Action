using Scripts.LoadScene;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Scripts.GameLoop
{
    public class MN_GameLoop : SceneSingleton<MN_GameLoop>, IPanelHandler
    {
        public event Action<bool> OnPanelOpen;

        [SerializeField] private HealthManager _playerHealth;
        [SerializeField] private AssetReference _gameplayScene;
        [Header("UI")]
        [SerializeField] private GameObject _gameOverPanel;
        [SerializeField] private Button _restartButton;
        [SerializeField] private UnityEvent UE_GameOver = new UnityEvent();

        public enum GameState { PLAYING, GAME_OVER, RESTARTING }
        public GameState State { get; private set; } = GameState.PLAYING;
        public bool IsPanelOpen { get; set; }

        private void OnEnable()
        {
            if (_playerHealth != null) 
                _playerHealth.OnDeath += GameOver;
        }

        private void Start()
        {       
            if (_playerHealth != null && !_playerHealth.IsAlive()) 
                GameOver();

            if (_restartButton)
                _restartButton.onClick.AddListener(RestartGame);
        }

        private void OnDisable()
        {
            if (_playerHealth != null) 
                _playerHealth.OnDeath -= GameOver;
        }

        private void GameOver()
        {
            if (State != GameState.PLAYING) return;

            State = GameState.GAME_OVER;

            UE_GameOver.Invoke();

            OpenPanel();
        }

        public void RestartGame()
        {
            if (State != GameState.GAME_OVER) return;
            if (_gameplayScene == null || !_gameplayScene.RuntimeKeyIsValid() || SIS_LoadingSceneChannel.Instance == null)
            {
                Debug.LogWarning("[MN_GameLoop] Missing gameplay scene or loading channel.", this);
                return;
            }

            State = GameState.RESTARTING;
            SIS_LoadingSceneChannel.Instance.LoadAndCleanSession(_gameplayScene);
        }

        public void OpenPanel()
        {
            IsPanelOpen = true;

            _gameOverPanel.SetActive(IsPanelOpen);

            OnPanelOpen?.Invoke(IsPanelOpen);
        }

        public void ClosePanel()
        {
            IsPanelOpen = false;

            _gameOverPanel.SetActive(IsPanelOpen);

            OnPanelOpen?.Invoke(IsPanelOpen);
        }
    }
}
