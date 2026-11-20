using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Scripts.Managers;
using CustomInspector;

namespace Scripts.Managers
{
    public class MN_Panels : SceneSingleton<MN_Panels>
    {
        public Action<string> OnCurrentOpenPanelUpdate;

        [Header("Configs")]
        [SerializeField] private bool _autoGatherPanelsOnStart = true;
        public string NonePanelOpenName = "None";

        [Header("Panels")]
        [SerializeField] private List<PanelConfig> _list_PanelConfigs = new List<PanelConfig>();

        [Header("Global UI Events")]
        [SerializeField] private List<PanelStateEvent> _list_GlobalEvents = new List<PanelStateEvent>();

        [SerializeField] private string _currentOpenPanelName = "None";

        public string CurrentOpenPanelName => _currentOpenPanelName;
        private readonly List<PanelConfig> _openPanelOrder = new List<PanelConfig>();
        public GameObject CurrentPanelGameObject => _openPanelOrder.Count > 0
            ? _openPanelOrder[_openPanelOrder.Count - 1].PanelGameObject : null;

        private void Start()
        {
            if (_autoGatherPanelsOnStart)
                AutoGatherPanels();

            InitializePanels();
            foreach (var config in _list_PanelConfigs)
                if (config.PanelHandlerCache != null && config.PanelHandlerCache.IsPanelOpen)
                    _openPanelOrder.Add(config);
            _currentOpenPanelName = _openPanelOrder.Count > 0
                ? _openPanelOrder[_openPanelOrder.Count - 1].PanelName : NonePanelOpenName;
            InvokeGlobalEvents(_currentOpenPanelName);
        }

        private void AutoGatherPanels()
        {
            var allPanelHandlers = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IPanelHandler>();

            foreach (var handler in allPanelHandlers)
            {
                Component handlerComponent = handler as Component;
                if (handlerComponent != null)
                {
                    if (!_list_PanelConfigs.Any(p => p.PanelGameObject == handlerComponent.gameObject))
                    {
                        _list_PanelConfigs.Add(new PanelConfig
                        {
                            PanelName = handlerComponent.gameObject.name,
                            PanelGameObject = handlerComponent.gameObject
                        });
                    }
                }
            }
        }

        private void InitializePanels()
        {
            foreach (var config in _list_PanelConfigs)
            {
                if (config.PanelGameObject != null)
                {
                    config.PanelHandlerCache = config.PanelGameObject.GetComponent<IPanelHandler>();
                    config.ActiveStateCache = config.PanelGameObject.GetComponent<IHasActiveState>();

                    if (config.PanelHandlerCache != null)
                    {
                        config.PanelHandlerCache.OnPanelOpen += (isOpen) => HandlePanelStateChanged(config, isOpen);
                    }
                    else
                        Debug.LogWarning($"[MN_Panels] The GameObject {config.PanelGameObject.name} dont have any IPanelHandler!");
                }
            }
        }

        private void HandlePanelStateChanged(PanelConfig config, bool isOpen)
        {
            GameObject previousPanel = CurrentPanelGameObject;
            _openPanelOrder.Remove(config);
            if (isOpen) _openPanelOrder.Add(config);
            _openPanelOrder.RemoveAll(p => p.PanelGameObject == null || p.PanelHandlerCache == null || !p.PanelHandlerCache.IsPanelOpen);
            _currentOpenPanelName = _openPanelOrder.Count > 0
                ? _openPanelOrder[_openPanelOrder.Count - 1].PanelName : NonePanelOpenName;

            if (previousPanel != CurrentPanelGameObject)
                InvokeGlobalEvents(_currentOpenPanelName);
        }


        private void InvokeGlobalEvents(string currentNameState)
        {
            OnCurrentOpenPanelUpdate?.Invoke(_currentOpenPanelName);

            foreach (var stateEvent in _list_GlobalEvents)
            {
                if (stateEvent.TargetPanelName == currentNameState)
                    stateEvent.OnStateEntered?.Invoke();
            }
        }

        public void CloseAll()
        {
            foreach (var config in _list_PanelConfigs)
            {
                if (config.PanelHandlerCache == null) continue;
                if (!config.PanelHandlerCache.IsPanelOpen) continue;

                config.PanelHandlerCache.ClosePanel();
            }
        }

        public bool IsCurrentOpenPanel(IPanelHandler panelHandler)
        {
            if (panelHandler == null) return false;

            PanelConfig panelConfig = _list_PanelConfigs.FirstOrDefault(config => config.PanelHandlerCache == panelHandler);

            return panelConfig != null
                && panelConfig.PanelGameObject == CurrentPanelGameObject
                && panelHandler.IsPanelOpen;
        }



        [Serializable]
        public class PanelConfig
        {
            public string PanelName;
            public GameObject PanelGameObject;

            [HideInInspector] public IPanelHandler PanelHandlerCache;
            [HideInInspector] public IHasActiveState ActiveStateCache;
        }

        [Serializable]
        public class PanelStateEvent
        {
            public string TargetPanelName;
            public UnityEvent OnStateEntered;
        }



#if UNITY_EDITOR
        private void AutoSetupPanels()
        {
            var allPanelHandlers = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IPanelHandler>();

            foreach (var handler in allPanelHandlers)
            {
                Component handlerComponent = handler as Component;
                if (handlerComponent != null)
                {
                    if (!_list_PanelConfigs.Any(p => p.PanelGameObject == handlerComponent.gameObject))
                    {
                        _list_PanelConfigs.Add(new PanelConfig
                        {
                            PanelName = handlerComponent.gameObject.name,
                            PanelGameObject = handlerComponent.gameObject
                        });
                    }
                }
            }

            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}
