using UnityEngine;
using UnityEngine.UI;

public class ExitButton : MonoBehaviour
{
    [SerializeField] private Button _buttonComponent;

    private void Awake() => _buttonComponent.onClick.AddListener(() => Exit());

    public void Exit() => Application.Quit();
}
