using Scripts.LoadScene;
using UnityEngine;
using UnityEngine.AddressableAssets;
public class LoadScene_TryLoad : MonoBehaviour
{
    [SerializeField] private bool _tryLoadOnStart;
    [SerializeField] private AssetReference _initialScene;

    private void Start()
    {
        if (_tryLoadOnStart)
            Load();
    }

    public void Load()
    {
        if (SIS_LoadingSceneChannel.Instance)      
            SIS_LoadingSceneChannel.Instance.Load(_initialScene);
        else
            Debug.LogError("[MN_Initialization] SIS_LoadingSceneChannel creation fail");
    }
}
