using Scripts.LoadScene;
using UnityEngine;
using UnityEngine.AddressableAssets;
public class LoadScene_TryLoad : MonoBehaviour
{
    [SerializeField] private AssetReference _initialScene;

    public void Load()
    {
        if (SIS_LoadingSceneChannel.Instance)      
            SIS_LoadingSceneChannel.Instance.Load(_initialScene);
        else
            Debug.LogError("[MN_Initialization] SIS_LoadingSceneChannel creation fail");
    }
}
