using UnityEngine;

public class PanelAnimation : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private Animator _animator;
    [SerializeField] private string _openAnimation;

    private IPanelHandler _panelHandler;

    private void Awake()
    {
        _panelHandler = _panel.GetComponent<IPanelHandler>();
        _panelHandler.OnPanelOpen += OnPanelOpen;
    }

    private void OnPanelOpen(bool isOpen)
    {
        if (isOpen)
            _animator.Play(_openAnimation, 0, 1);
    }
}
